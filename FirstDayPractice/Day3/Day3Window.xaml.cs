using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using FirstDayPractice.Day3.UserControls;


namespace FirstDayPractice.Day3
{
    /// <summary>
    /// Interaction logic for Day3Window.xaml
    /// </summary>
    public partial class Day3Window : Window
    {
        private List<MenuItem> _menuItems; //menu as list
        public Day3Window()
        {
            InitializeComponent();

            //adding menu item
            _menuItems = new List<MenuItem>
            {
                new MenuItem("Binding", new BindingView()),
                new MenuItem("Resources", new ResourcesView()),
                new MenuItem("Template", new TemplateView()),
                new MenuItem("Triggers", new TriggerView())
            };
            
            SideMenu.ItemsSource= _menuItems;

        }

        private void SideMenu_SelectionCHanged(object sender, SelectionChangedEventArgs e)
        {
            if(SideMenu.SelectedItem is MenuItem selectedMenu)
            {
                ContentArea.Content = selectedMenu.Content;
            }
        }
    }
}
