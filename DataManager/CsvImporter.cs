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
            //int totalRecords = File.ReadLines(filePath).Count() - 1;
            var lines = await Task.Run(() => File.ReadLines(filePath));
            int totalRecords = lines.Count() - 1;
            int processedRecords = 0;
            OnProgressChanged(new ProgressChange(totalRecords, processedRecords, "Starting import..."));

            using (var reader = new StreamReader(filePath))
            using (var csv = new CsvReader(reader, config))
            {
                int batchSize = int.Parse(ConfigurationManager.AppSettings["BatchSize"]);
                var batch = new List<Record>();

                while (await csv.ReadAsync())
                {
                    csv.Context.RegisterClassMap<RecordMap>();
                    var record = csv.GetRecord<Record>();
                    batch.Add(record);
                    processedRecords++;

                    if (batch.Count >= batchSize)
                    {
                        await SaveBatchAsync(batch);
                        batch.Clear();
                        UpdateProgress(processedRecords, totalRecords);
                    }
                }

                if (batch.Count > 0)
                {
                    await SaveBatchAsync(batch);
                    UpdateProgress(processedRecords, totalRecords);
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
