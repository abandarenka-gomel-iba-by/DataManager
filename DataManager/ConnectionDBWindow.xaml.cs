using System.Windows;

namespace DataManager
{
    /// <summary>
    /// Interaction logic for ConnectionDBWindow.xaml
    /// </summary>
    public partial class ConnectionDBWindow : Window
    {
        public string ConnectionString { get; set; }

        public ConnectionDBWindow(string currentConnectionString)
        {
            InitializeComponent();

            ConnectionStringTextBox.Text = currentConnectionString;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            ConnectionString = ConnectionStringTextBox.Text;
            this.DialogResult = true;
        }
    }
}
