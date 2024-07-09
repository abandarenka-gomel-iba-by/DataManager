using CsvHelper;
using Microsoft.Win32;
using System;
using System.Windows;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Collections.Generic;

namespace DataManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IRecordRepository _recordRepository;

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
            _csvImporter = new CsvImporter(_viewModel);
            _dataExporter = new DataExporter();
            _filterForm = new FilterForm();

            Loaded += MainWindow_Loaded;
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadDataAsync();
        }

        private async void btnImport_Click(object sender, RoutedEventArgs e)
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
                    await _csvImporter.ImportCsv(filePath);
                }
                catch (CsvHelperException ex)
                {
                    MessageBox.Show($"CSV format error: {ex.Message}", "Import Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    _csvImporter.HideProgress();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred: {ex.Message}", "Import Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    _csvImporter.HideProgress();
                }

                await LoadDataAsync();
            }
        }

        private void btnExportExcel_Click(object sender, RoutedEventArgs e)
        {
            var saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Excel files (*.xlsx)|*.xlsx";
            if (saveFileDialog.ShowDialog() == true)
            {
                var records = dataGrid.ItemsSource as List<Record>;
                _dataExporter.ExportToExcel(records, saveFileDialog.FileName);
            }
        }

        private void btnExportXml_Click(object sender, RoutedEventArgs e)
        {
            var saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "XML files (*.xml)|*.xml";
            if (saveFileDialog.ShowDialog() == true)
            {
                var records = dataGrid.ItemsSource as List<Record>;
                _dataExporter.ExportToXml(records, saveFileDialog.FileName);
            }
        }

        private async Task LoadDataAsync()
        {
            dataGrid.ItemsSource = await _recordRepository.GetRecords();
        }

        private void dpFromDate_SelectedDateChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            _filterForm.FromDate = dpFromDate.SelectedDate;
        }

        private void dpToDate_SelectedDateChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            _filterForm.ToDate = dpToDate.SelectedDate;
        }

        private void txtFirstNameFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            _filterForm.FirstName = txtFirstNameFilter.Text.Trim();
        }

        private void txtLastNameFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            _filterForm.LastName = txtLastNameFilter.Text.Trim();
        }

        private void txtCityFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            _filterForm.City = txtCityFilter.Text.Trim();
        }

        private void txtCountryFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            _filterForm.Country = txtCountryFilter.Text.Trim();
        }

        private async void btnFilter_Click(object sender, RoutedEventArgs e)
        {
            dataGrid.ItemsSource = await _recordRepository.Search(_filterForm);
        }

        private async void btnClear_Click(object sender, RoutedEventArgs e)
        {
            await LoadDataAsync();
        }
    }
}
