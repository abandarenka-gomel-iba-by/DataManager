using CsvHelper;
using Microsoft.Win32;
using System;
using System.Windows;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Windows.Input;

namespace DataManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IRecordRepository _recordRepository;

        public static RoutedCommand CustomRoutedCommand = new RoutedCommand();

        private AppDbContext _context;
        private CsvImporter _csvImporter;
        private DataExporter _dataExporter;
        private ImportViewModel _viewModel;
        private FilterForm _filterForm;

        public MainWindow()
        {
            InitializeComponent();

            _viewModel = (ImportViewModel)DataContext;
            _context = new AppDbContext();
            _recordRepository = new RecordRepository(new AppDbContext());
            _csvImporter = new CsvImporter();
            _dataExporter = new DataExporter();
            _filterForm = (FilterForm)FindResource("filterForm");

            _csvImporter.ProgressChanged += CsvImporter_ProgressChanged;
            Loaded += MainWindow_Loaded;
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadDataAsync();
        }

        private async void ImportCSV_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "CSV files (*.csv)|*.csv";
            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                if (!filePath.EndsWith(".csv"))
                {
                    MessageBox.Show("Please select a valid CSV file.", "Invalid File", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                try
                {
                    await _csvImporter.ImportCsvAsync(filePath);
                }
                catch (CsvHelperException ex)
                {
                    MessageBox.Show($"CSV format error: {ex.Message}", "Import Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    _viewModel.StatusBarVisibility = Visibility.Collapsed;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred: {ex.Message}", "Import Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    _viewModel.StatusBarVisibility = Visibility.Collapsed;
                }

                await LoadDataAsync();
            }
        }

        private async void ExportToExcel_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Excel files (*.xlsx)|*.xlsx";
            if (saveFileDialog.ShowDialog() == true)
            {
                var records = dataGrid.ItemsSource as List<Record>;
                await _dataExporter.ExportToExcelAsync(records, saveFileDialog.FileName);
            }
        }

        private async void ExportToXML_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "XML files (*.xml)|*.xml";
            if (saveFileDialog.ShowDialog() == true)
            {
                var records = dataGrid.ItemsSource as List<Record>;
                await _dataExporter.ExportToXmlAsync(records, saveFileDialog.FileName);
            }
        }

        private async Task LoadDataAsync()
        {
            dataGrid.ItemsSource = await _recordRepository.GetRecordsAsync();
        }

        private async void Filter_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            dataGrid.ItemsSource = await _recordRepository.SearchAsync(_filterForm);
        }

        private async void Clear_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            _filterForm.ClearForm();
            await LoadDataAsync();
        }

        private void CsvImporter_ProgressChanged(object sender, ProgressChange e)
        {
            Dispatcher.Invoke(() =>
            {
                int percent = (int)((double)e.Current / e.Total * 100);
                _viewModel.StatusBarVisibility = Visibility.Visible;
                _viewModel.StatusMessage = e.Message;
                _viewModel.Progress = (int)((double)e.Current / e.Total * 100);
            });
        }
    }
}
