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
    /// Interaction logic for BindingView.xaml
    /// </summary>
    public partial class BindingView : UserControl
    {
        User usr = new User { Name = "Yalung", Age = 21 };

        public BindingView()
        {
            InitializeComponent();
            this.DataContext = usr;
        }

        private void OneWayButton_Click(object sender, RoutedEventArgs e)
        {
            string message = usr.Name + " is " + usr.Age;
            MessageBox.Show(message);
        }

        private void TwoWayButton_Click(object sender, RoutedEventArgs e)
        {
            string message = usr.Name + " is " + usr.Age;
            MessageBox.Show(message);
        }
    }
}
