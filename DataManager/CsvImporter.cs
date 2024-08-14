using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;

namespace DataManager
{
    internal class CsvImporter
    {
        public event EventHandler<ProgressChange> ProgressChanged;

        public async Task ImportCsvAsync(string filePath)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ";",
            };
            long totalBytes = new FileInfo(filePath).Length;
            long processedBytes = 0;

            OnProgressChanged(new ProgressChange(totalBytes, processedBytes, "Starting import..."));

            await foreach (var batch in ReadRecordsAsync(filePath, config))
            {
                await SaveBatchAsync(batch.Records);
                processedBytes = batch.ProcessedBytes;
                UpdateProgress(totalBytes, processedBytes);
            }
        }

        private async IAsyncEnumerable<(List<Record> Records, long ProcessedBytes)> ReadRecordsAsync(string filePath, CsvConfiguration config)
        {
            int batchSize = int.Parse(ConfigurationManager.AppSettings["BatchSize"]);
            var batch = new List<Record>();
            long processedBytes = 0;

            using (var reader = new StreamReader(filePath))
            using (var csv = new CsvReader(reader, config))
            {
                csv.Context.RegisterClassMap<RecordMap>();
                await foreach (var record in csv.GetRecordsAsync<Record>())
                {
                    batch.Add(record);
                    processedBytes = reader.BaseStream.Position;

                    if (batch.Count >= batchSize)
                    {
                        yield return (batch, processedBytes);
                        batch = new List<Record>();
                    }
                }

                if (batch.Count > 0)
                {
                    yield return (batch, processedBytes);
                }
            }
        }

        private async Task SaveBatchAsync(List<Record> batch)
        {
            using (var context = new AppDbContext())
            {
                context.Records.AddRange(batch);
                await context.SaveChangesAsync();
            }
        }

        private void UpdateProgress(long total, long processed)
        {
            string message = "Loading...";
            if (processed >= total)
            {
                message = "Done";
            }
            OnProgressChanged(new ProgressChange(total, processed, message));
        }

        protected virtual void OnProgressChanged(ProgressChange e)
        {
            ProgressChanged?.Invoke(this, e);
        }
    }
}
