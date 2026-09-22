using System.Text;
using System.Windows;
using FirstDayPractice.Day3;
using FirstDayPractice.Day4;
using FirstDayPractice.Day6_;


namespace FirstDayPractice
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

        }

        private void GoToCalculator_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new CalculatorPage());
        }

        private void GoToNewWindow_Click(object sender, RoutedEventArgs e)
        {
            NewPopupWindow popWindow = new NewPopupWindow();
            popWindow.Show();
        }

        private void GoToCalculatorWindow_Click(object sender, RoutedEventArgs e)
        {
            Calculator calcWindow = new Calculator();
            calcWindow.Show(); 
        }

        private void GoToControlWindow_Click(object sender, RoutedEventArgs e)
        {
            ControlsPracticeWindow ctrlWindow = new ControlsPracticeWindow();
            ctrlWindow.Show();
        }

        private void GoToDay3Window_Click(object sender, RoutedEventArgs e)
        {
            Day3Window day3Window = new Day3Window();
            day3Window.Show();
        }

        private void GoToDay4Window_Click(object sender, RoutedEventArgs e)
        {
            MasterNewVisionDesign day4Window = new MasterNewVisionDesign();
            day4Window.Show();
        }

        private void GoToDay6Window_Click(object sender, RoutedEventArgs e)
        {
            Day6Window day6Window = new Day6Window();
            day6Window.Show();
        }
    }
}