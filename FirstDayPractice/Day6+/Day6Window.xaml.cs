using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace FirstDayPractice.Day6_
{
    /// <summary>
    /// Interaction logic for Day6Window.xaml
    /// </summary>
    public partial class Day6Window : Window
    {
        public static RoutedCommand SayHelloCommand = new RoutedCommand(); //define command

        public Day6Window()
        {
            InitializeComponent();
            EventPanel.AddHandler(FancyButton.GlowChangedEvent, new RoutedEventHandler(MainPanel_GlowChanged)); //Event Manager
            
            CommandBinding binding = new CommandBinding(SayHelloCommand, ExecuteSayHello); //Command binding
            this.CommandBindings.Add(binding);

        }
        private void YesNoCancelButton_Click(object sender, RoutedEventArgs e)
        {
            if (e.Source is Button clicked)
            {
                MessageBox.Show($"You clicked: {clicked.Content}");
            }
        }

        private void BubblingPanel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("Bubbling: Event traveled from Button up to StackPanel.");
        }

        private void TunnelingPanel_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("Tunneling: Event traveled from Window down to StackPanel.");
        }

        private void Button_CLick_Button(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Button Handler");
            //e.Handled = true; //only show button msg, an no route to stackpanel 
        }
        private void Button_Click_StackPanel(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("StackPanel handler");
        }
        private void StackPanel_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("StackPanel PreviewMouseDown (Tunneling)");
        }

        private void StackPanel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("StackPanel MouseDown (Bubbling)");
            Debug.WriteLine("StackPanel MouseDown (Bubbling)");

        }

        private void Button_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("Button PreviewMouseDown (Tunneling)");
        }

        private void Button_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("Button MouseDown (Bubbling)");
            Debug.WriteLine("Button MouseDown (Bubbling)");

        }

        private void ApplyButtonStyle(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn)
            {
                MessageBox.Show($"Styled Button Clicked: {btn?.Content}");
            }
        }

        private void Direct_Button_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn)
            {
                MessageBox.Show($"Direct Button Clicked: {btn.Content}");
            }
        }

        //calling fancyButton part
        private void FancyButton_GlowChanged(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("FancyButton raised GlowChanged event");
        }
        private void MainPanel_GlowChanged(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("MainPanel received GlowChanged (bubbled up)");
        }

        private void OnButtonKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Source is Button source)

            {
                if (e.Key == Key.Left)  // If Left Arrow key pressed
                {
                    source.Background = Brushes.Red; // Change color
                }
                else
                {
                    source.Background = Brushes.AliceBlue; // Reset color
                }
            }
        }

        private void OnMouseEnter(object sender, MouseEventArgs e)
        {
            if (e.Source is Button source)
            {
                source.Background = Brushes.Green;
            }
        }
        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
            if (e.Source is Button source)
            {
                source.Background = Brushes.Yellow;
            }
        }

        private void OnTextInputKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.O && Keyboard.Modifiers == ModifierKeys.Control)
            {
                MessageBox.Show("You pressed (Ctrl + O)");
                e.Handled = true;
            }
        }
        private void OnTextInputButtonClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("You opened a file(Button)");
            e.Handled = true;
        }

        //command part
        private void ExecuteSayHello(object sender, ExecutedRoutedEventArgs e)
        {
            MessageBox.Show("Command Says Hello");
        }
    }
}
