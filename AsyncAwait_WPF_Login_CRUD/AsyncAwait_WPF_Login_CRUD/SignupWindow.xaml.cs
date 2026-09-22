using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
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
using System.Xml.Linq;
using Microsoft.Data.SqlClient;
using static Azure.Core.HttpHeader;

namespace AsyncAwait_WPF_Login_CRUD
{
    /// <summary>
    /// Interaction logic for SignupWindow.xaml
    /// </summary>
    public partial class SignupWindow : Window
    {
        private string connectionString = "Server=localhost;Database=PersonDB_asr;User Id=sa;Password=123;Encrypt=True;TrustServerCertificate=True;";

        public SignupWindow()
        {
            InitializeComponent();
        }

        private async void SignupButton_Click(object sender, RoutedEventArgs e)
        {
            string name = txtName.Text.Trim();
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Password;

            try
            {
                using SqlConnection conn = new SqlConnection(connectionString);
                await conn.OpenAsync();

                using SqlCommand cmd = new SqlCommand("SignupPersonAuth", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@Name", name);
                cmd.Parameters.AddWithValue("@Username", username);
                cmd.Parameters.AddWithValue("@Password", password);

                await cmd.ExecuteNonQueryAsync();
                MessageBox.Show("Signup successful!");

                var login = new MainWindow();
                login.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Signup failed: {ex.Message}");
            }
        }
    }
}
