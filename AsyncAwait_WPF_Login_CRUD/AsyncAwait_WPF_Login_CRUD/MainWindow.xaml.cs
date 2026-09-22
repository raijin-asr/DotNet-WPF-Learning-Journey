using System.Data;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Microsoft.Data.SqlClient;


namespace AsyncAwait_WPF_Login_CRUD
{

    public partial class MainWindow : Window
    {
        private string connectionString = "Server=localhost;Database=PersonDB_asr;User Id=sa;Password=123;Encrypt=True;TrustServerCertificate=True;";

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txtUsername.Text = Properties.Settings.Default.Username;
            txtPassword.Password = Properties.Settings.Default.Password;
            chkRemember.IsChecked = !string.IsNullOrEmpty(Properties.Settings.Default.Username);
        }

        private async void Login_Click(object sender, RoutedEventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Password;

            try
            {
                using SqlConnection conn = new SqlConnection(connectionString);
                await conn.OpenAsync();

                using SqlCommand cmd = new SqlCommand("LoginPersonAuth", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@Username", username);
                cmd.Parameters.AddWithValue("@Password", password);

                using SqlDataReader reader = await cmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    var person = new PersonAuthModel
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1)
                    };

                    //store credential locally if Remember is checked
                    if (chkRemember.IsChecked == true)
                    {
                        Properties.Settings.Default.Username = username;
                        Properties.Settings.Default.Password = password;
                        Properties.Settings.Default.Save();
                    }
                    else
                    {
                        Properties.Settings.Default.Reset();
                    }

                    MessageBox.Show($"Login successful!!! Welcome {person.Name}");

                    var adminDashboard = new AdminDashboardWindow(person);
                    adminDashboard.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Invalid Username or Password!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Login Failed: {ex.Message}");
            }
        }

        private void SignupLink_Click(object sender, RoutedEventArgs e)
        {
            var signup = new SignupWindow();
            signup.Show();
            this.Close();
        }
    }
}