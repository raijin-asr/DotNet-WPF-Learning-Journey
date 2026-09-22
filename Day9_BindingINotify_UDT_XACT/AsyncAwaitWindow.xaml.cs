using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Day9_BindingINotify_UDT_XACT
{
    /// <summary>
    /// Interaction logic for AsyncAwaitWindow.xaml
    /// </summary>
    public partial class AsyncAwaitWindow : Window
    {
        private int _syncCount = 0; // Field to hold the synchronous count

        public AsyncAwaitWindow()
        {
            InitializeComponent();
            _ = LoadOnStartupAsync(); //async method at constructor
        }
        private async Task LoadOnStartupAsync()
        {
            await Task.Delay(2000);
            lblLoad.Content = "Loaded!";
        }

        private async void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            LoadButton.IsEnabled = false;
            StatusText.Text = "Loading Data, Please Wait....";
            OutputBox.Text = string.Empty;

            try
            {
                string result = await LoadDataAsync();
                OutputBox.Text = result;
                StatusText.Text = "Data Loaded Completed!";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                LoadButton.IsEnabled=true;
            }

        }

        private async Task<string> LoadDataAsync()
        {
            await Task.Delay(2000); //simulate network/file delay

            return "Async data loaded successfully at" + DateTime.Now.ToLongDateString();
        }
        private void SyncCounterButton_Click(object sender, RoutedEventArgs e)
        {
            _syncCount++; // Increment the count
            lblSyncCount.Content = $"Count: {_syncCount}"; // Update the label
        }

    }
}
