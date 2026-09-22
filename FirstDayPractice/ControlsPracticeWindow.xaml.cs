using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Navigation;
using Microsoft.Win32;
using System.IO;
using System.Windows.Xps.Packaging;
using System.Windows.Input;

namespace FirstDayPractice
{
    /// <summary>
    /// Interaction logic for ControlsPracticeWindow.xaml
    /// </summary>
    public partial class ControlsPracticeWindow : Window
    {
        private bool _tab2Loaded = false;

        public ControlsPracticeWindow()
        {
            InitializeComponent();
            DataContext = new PersonViewModel(); //data binding

            // Load a sample XPS document if available
            string xpsPath = @"C:\Users\Dev62\source\repos\FirstDayPractice\FirstDayPractice\Files\sample.xps";

            if (File.Exists(xpsPath))
            {
                XpsDocument xpsDoc = new XpsDocument(xpsPath, FileAccess.Read);
                docViewer.Document = xpsDoc.GetFixedDocumentSequence();
            }
            else
            {
                MessageBox.Show("XPS file not found at: " + xpsPath);
            }

            //play WebBrowser-based YouTube player
            //youtubeBrowser.Navigate("https://www.youtube.com/embed/t9ivUosw_iI?autoplay=1");

        }
        private async void MainTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!IsLoaded) return;

            var tabControl = sender as TabControl;
            if (tabControl == null) return;

            if(tabControl.SelectedIndex == 1 && !_tab2Loaded)
            {
                _tab2Loaded = true;

                //show loading panel and hide real content
                Tab2Loading.Visibility = Visibility.Visible;
                Tab2MainContentGrid.Visibility = Visibility.Collapsed;

                await Task.Delay(2000);

                Tab2Loading.Visibility = Visibility.Collapsed;
                Tab2MainContentGrid.Visibility = Visibility.Visible;
            }
        }

        //context menu part
        private void Bold_Checked(object sender, RoutedEventArgs e)
        {
            textBox1.FontWeight = FontWeights.Bold;
        }

        private void Bold_Unchecked(object sender, RoutedEventArgs e)
        {
            textBox1.FontWeight = FontWeights.Normal;
        }

        private void Italic_Checked(object sender, RoutedEventArgs e)
        {
            textBox1.FontStyle = FontStyles.Italic;
        }

        private void Italic_Unchecked(object sender, RoutedEventArgs e)
        {
            textBox1.FontStyle = FontStyles.Normal;
        }

        private void IncreaseFont_Click(object sender, RoutedEventArgs e)
        {
            if (textBox1.FontSize < 18)
            {
                textBox1.FontSize += 2;
            }
        }

        private void DecreaseFont_Click(object sender, RoutedEventArgs e)
        {
            if (textBox1.FontSize > 10)
            {
                textBox1.FontSize -= 2;
            }
        }

        //toolbar part
        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Open button is clicked.");
        }
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Close button is clicked.");
        }
        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Exit button is clicked.");
        } private void ExtraButton1_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Extra button 1 is clicked.");
        } private void ExtraButton2_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Extra button 2 is clicked.");
        }

        //open link in browser
        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
            e.Handled = true;
        }

        //dialog box part to open, save and print
        private void OpenFileMenuItem_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
             openFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                MessageBox.Show("Opened File: " + openFileDialog.FileName);
            }
        }

        private void SaveFileMenuItem_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";

            if (saveFileDialog.ShowDialog() == true)
            {
                File.WriteAllText(saveFileDialog.FileName, "New text added successful!.");
                MessageBox.Show("File saved: " + saveFileDialog.FileName);
            }
        }

        private void PrintMenuItem_Click(object sender, RoutedEventArgs e)
        {
            PrintDialog printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == true)
            {
                FlowDocument doc = new FlowDocument(new Paragraph(new Run("This is Sample print text")));
                doc.Name = "PrintDoc";

                IDocumentPaginatorSource idpSource = doc;
                printDialog.PrintDocument(idpSource.DocumentPaginator, "Printing");
            }
        }

        private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        //media control
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            mediaPlayer.Play();
        }
        private void mediaPlayer_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            mediaPlayer.Visibility = Visibility.Collapsed;
            fallbackText.Visibility = Visibility.Visible;
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
                this.DragMove();
        }

        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void Maximize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = this.WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
