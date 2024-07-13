using CsvHelper;
using MahApps.Metro.Controls;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DataManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private IRecordRepository _recordRepository;

        public static RoutedCommand CustomRoutedCommand = new RoutedCommand();

        private AppDbContext _context;
        private CsvImporter _csvImporter;
        private DataExporter _dataExporter;
        private ImportViewModel _viewModel;
        private FilterForm _filterForm;
        private ObservableCollection<Record> _records;

        public MainWindow()
        {
            InitializeComponent();
            InitializeDatabase();

            _viewModel = (ImportViewModel)DataContext;
            _recordRepository = new RecordRepository(_context);
            _csvImporter = new CsvImporter();
            _dataExporter = new DataExporter();
            _filterForm = (FilterForm)FindResource("filterForm");
            _records = new ObservableCollection<Record>();
            dataGrid.ItemsSource = _records;

            _csvImporter.ProgressChanged += CsvImporter_ProgressChanged;
            Loaded += MainWindow_Loaded;
        }

        private void InitializeDatabase()
        {
            while (true)
            {
                try
                {
                    string currentConnectionString = ConfigurationManager.ConnectionStrings["AppDbContext"].ConnectionString;
                    Debug.WriteLine("Current Connection String: " + currentConnectionString);

                    _context = new AppDbContext();
                    _context.Database.Initialize(false);
                    break;
                }
                catch (Exception ex)
                {
                    HandleDisplayError(ex, "Failed to connect to the database. Please edit the connection string.");
                    string connectionString = ConfigurationManager.ConnectionStrings["AppDbContext"].ConnectionString;
                    ConnectionDBWindow connectionStringWindow = new ConnectionDBWindow(connectionString);
                    if (connectionStringWindow.ShowDialog() == true)
                    {
                        UpdateConnectionString(connectionStringWindow.ConnectionString);
                    }
                    else
                    {
                        Application.Current.Shutdown();
                        break;
                    }
                }
            }
        }

        private void UpdateConnectionString(string newConnectionString)
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.ConnectionStrings.ConnectionStrings["AppDbContext"].ConnectionString = newConnectionString;
            config.Save(ConfigurationSaveMode.Modified, true);
            ConfigurationManager.RefreshSection("connectionStrings");
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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
                    HandleDisplayError(ex, "CSV format error");
                    _viewModel.StatusBarVisibility = Visibility.Collapsed;
                }
                catch (Exception ex)
                {
                    HandleDisplayError(ex, "An error occurred");
                    _viewModel.StatusBarVisibility = Visibility.Collapsed;
                }

                await LoadDataAsync();
            }
        }
        private void HandleDisplayError(Exception ex, string errorMessage)
        {
            MessageBox.Show($"{errorMessage}: {ex.Message}", "Import Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
            await foreach (var record in _recordRepository.GetRecordsAsync())
            {
                _records.Add(record);
            }
        }

        private async void Filter_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            _records.Clear();
            await foreach (var record in _recordRepository.SearchAsync(_filterForm))
            {
                _records.Add(record);
            }
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

        private void SetLanguage(string language)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo(language);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(language);

            var metroResourceDictionaries = Application.Current.Resources.MergedDictionaries
                .Where(d => d.Source != null && d.Source.OriginalString.Contains("MahApps.Metro"))
                .ToList();

            Application.Current.Resources.MergedDictionaries.Clear();

            foreach (var dict in metroResourceDictionaries)
            {
                Application.Current.Resources.MergedDictionaries.Add(dict);
            }

            ResourceDictionary languageDict = new ResourceDictionary()
            {
                Source = new Uri($"/Dictionary-{language}.xaml", UriKind.Relative)
            };
            Application.Current.Resources.MergedDictionaries.Add(languageDict);
        }

        private void ChangeLanguage_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.OriginalSource is MenuItem menuItem && menuItem.Tag is string language)
            {
                SetLanguage(language);
            }
        }
    }
}
