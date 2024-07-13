using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Configuration;

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
            int totalRecords = await GetTotalRecordsAsync(filePath);
            int processedRecords = 0;
            OnProgressChanged(new ProgressChange(totalRecords, processedRecords, "Starting import..."));

            await foreach (var batch in ReadRecordsAsync(filePath, config))
            {
                await SaveBatchAsync(batch);
                processedRecords += batch.Count;
                UpdateProgress(processedRecords, totalRecords);
            }
        }

        private async Task<int> GetTotalRecordsAsync(string filePath)
        {
            return await Task.Run(() => File.ReadLines(filePath).Count() - 1);
        }

        private async IAsyncEnumerable<List<Record>> ReadRecordsAsync(string filePath, CsvConfiguration config)
        {
            int batchSize = int.Parse(ConfigurationManager.AppSettings["BatchSize"]);
            var batch = new List<Record>();

            using (var reader = new StreamReader(filePath))
            using (var csv = new CsvReader(reader, config))
            {
                csv.Context.RegisterClassMap<RecordMap>();
                await foreach (var record in csv.GetRecordsAsync<Record>())
                {
                    batch.Add(record);

                    if (batch.Count >= batchSize)
                    {
                        yield return batch;
                        batch = new List<Record>();
                    }
                }

                if (batch.Count > 0)
                {
                    yield return batch;
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

        private void UpdateProgress(int processedRecords, int totalRecords)
        {
            string message = "Loading...";
            if (processedRecords == totalRecords)
            {
                message = "Done";
            }
            OnProgressChanged(new ProgressChange(totalRecords, processedRecords, message));
        }

        protected virtual void OnProgressChanged(ProgressChange e)
        {
            ProgressChanged?.Invoke(this, e);
        }
    }
}
