using CsvHelper;
using CsvHelper.Configuration;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace DataManager
{
    internal class CsvImporter
    {
        public ImportViewModel _viewModel;

        public CsvImporter(ImportViewModel viewModel)
        {
            _viewModel = viewModel;
        }

        public async Task ImportCsv(string filePath)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ";",
            };
            int totalRecords = File.ReadLines(filePath).Count() - 1;
            int processedRecords = 0;
            _viewModel.StatusBarVisibility = Visibility.Visible;
            _viewModel.StatusMessage = "Starting import...";
            _viewModel.Progress = 0;

            using (var reader = new StreamReader(filePath))
            using (var csv = new CsvReader(reader, config))
            {
                int batchSize = 1000;
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
            _viewModel.StatusMessage = message;
            _viewModel.Progress = (int)((double)processedRecords / totalRecords * 100);
        }

        public void HideProgress()
        {
            _viewModel.StatusBarVisibility = Visibility.Collapsed;
        }
    }
}
