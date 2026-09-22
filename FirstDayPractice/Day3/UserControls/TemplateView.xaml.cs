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
    /// Interaction logic for TemplateView.xaml
    /// </summary>
    public partial class TemplateView : UserControl
    {
        User user = new User { Name = "Ameer", Age = 90 }; //direct storing single user data
        List<User> users = new List<User>(); //using list to store more user data
        public TemplateView()
        {
            InitializeComponent();
            users.Add(user);
            users.Add(new User { Name = "Max", Age = 80 });
            this.DataContext = users; //saving users to DataContext
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string message= user.Name + " is " + user.Age;
            MessageBox.Show(message);
        }
    }

}
