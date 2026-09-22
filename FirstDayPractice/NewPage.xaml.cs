
using System.Windows;
using System.Windows.Controls;


namespace FirstDayPractice
{
    /// <summary>
    /// Interaction logic for NewPage.xaml
    /// </summary>
    public partial class NewPage : Page
    {
        public NewPage()
        {
            InitializeComponent();
        }


        private void OpenPopup_CLick(object sender, RoutedEventArgs e)
        {
            var popup = new NewPopupWindow();
            popup.Show();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ReportPage myReportPage = new ReportPage(this.peopleListBox.SelectedItem); //passing data of selected name
            this.NavigationService.Navigate(myReportPage);
        }
    }
}
