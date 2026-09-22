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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FirstDayPractice.Day3.UserControls
{
    /// <summary>
    /// Interaction logic for ResourcesView.xaml
    /// </summary>
    public partial class ResourcesView : UserControl
    {
        public ResourcesView()
        {
            InitializeComponent();
        }

        private void changeResourceButton_Click(object sender, RoutedEventArgs e)
        {
            var newBrush = new SolidColorBrush(Colors.Orange);
            this.Resources["brushResource"] = newBrush;
        }

        private void changeResourceDictionaryButton_Click(object sender, RoutedEventArgs e)
        {
            var newBrush = new SolidColorBrush(Colors.Blue);

            // Look into the application's merged dictionaries
            foreach (var dict in Application.Current.Resources.MergedDictionaries)
            {
                if (dict.Contains("brushResourceDictionary"))
                {
                    dict["brushResourceDictionary"] = newBrush;
                    break;
                }
            }
        }
    }
}
