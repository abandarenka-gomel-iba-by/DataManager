using System.Windows.Input;

namespace DataManager
{
    internal class AppCommands
    {
        static AppCommands()
        {
            Filter = new RoutedCommand("Filter", typeof(MainWindow));
            Clear = new RoutedCommand("Clear", typeof(MainWindow));
            ImportCSV = new RoutedCommand("ImportCSV", typeof(MainWindow));
            ExportToExcel = new RoutedCommand("ExportToExcel", typeof(MainWindow));
            ExportToXML = new RoutedCommand("ExportToXML", typeof(MainWindow));
            ChangeLanguage = new RoutedCommand("ChangeLanguage", typeof(MainWindow));
        }

        public static RoutedCommand Filter { get; set; }
        public static RoutedCommand Clear { get; set; }
        public static RoutedCommand ImportCSV { get; set; }
        public static RoutedCommand ExportToExcel { get; set; }
        public static RoutedCommand ExportToXML { get; set; }
        public static RoutedCommand ChangeLanguage { get; set; }
    }
}
