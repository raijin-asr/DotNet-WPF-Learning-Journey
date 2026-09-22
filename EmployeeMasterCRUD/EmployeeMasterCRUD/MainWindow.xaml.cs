using System.Text;
using System.Windows;
using System.Windows.Controls;

using Microsoft.Data.SqlClient;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using System.IO;
using EmployeeMasterCRUD.Day7StoredProcedurePractice;
using EmployeeMasterCRUD.Day8_UDT_Byte;


namespace EmployeeMasterCRUD
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string connectionString = "Server=localhost;Database=EmployeeMasterDB;User Id=sa;Password=123;Encrypt=True;TrustServerCertificate=True;";
        private int selectedEmployeeId = -1; //-1 means no employee selected
        private ObservableCollection<Qualification> qualifications = new ObservableCollection<Qualification>(); //special list to add or remove items from datagrid

        private string currentPhotoPath = "";
         
        public MainWindow()
        {
            InitializeComponent();
            dgQualification.ItemsSource= qualifications;
            
        }
        
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadEmployees(); 
        }

        private void LoadEmployees()
        {
            cmbCode.Items.Clear(); //clear combobox and prevent duplicate
            cmbCode.Items.Add("-- New Employee --");

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT e.Id FROM Employee e JOIN Qualification q ON e.Id = q.EmpId WHERE q.PhotoType = 'normal' ", conn);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read()) 
                {
                    cmbCode.Items.Add(reader.GetInt32(0));
                }
                conn.Close();
            }

            cmbCode.SelectedItem = "-- New Employee --";
        }

        private void cmbCode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbCode.SelectedItem == null) return;

            if (cmbCode.SelectedItem.ToString() == "-- New Employee --")
            {
                ResetForm();
                selectedEmployeeId = -1;
            }
            else
            {
                selectedEmployeeId = Convert.ToInt32(cmbCode.SelectedItem);
                LoadEmployeeDetails(selectedEmployeeId);
                LoadQualifications(selectedEmployeeId);
                UpdateSerialNumbers();
            }

        }

        private void LoadEmployeeDetails(int empId)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT Name, Address, Photo FROM Employee WHERE Id= @Id", conn);
                cmd.Parameters.AddWithValue("@Id", empId);

                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    txtName.Text = reader.GetString(0);
                    txtAddress.Text = reader.GetString(1);
                    if (!reader.IsDBNull(2)) // Check if Photo column is not null
                    {
                        currentPhotoPath = reader.GetString(2);
                        imgPhoto.Source = new BitmapImage(new Uri(currentPhotoPath, UriKind.Absolute));
                    }
                    else
                    {
                        currentPhotoPath = "";
                        imgPhoto.Source = new BitmapImage(new Uri("pack://application:,,,/Images/no-image.png"));
                    }
                    
                }
                conn.Close();
            }
        }


        private void LoadQualifications(int empId)
        { 
            qualifications.Clear(); //clear current list of qualifications when switching employee or resetting form

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT Department,University,Grade FROM Qualification WHERE EmpId= @EmpId AND  PhotoType= @PhotoType", conn);
                cmd.Parameters.AddWithValue("@EmpId", empId);
                cmd.Parameters.AddWithValue("@PhotoType", "normal");

                SqlDataReader reader = cmd.ExecuteReader();
                int sno = 1;
                while (reader.Read())
                {
                    qualifications.Add(new Qualification
                    {
                        SNo = sno++,
                        Department = reader.GetString(0),
                        University = reader.GetString(1),
                        Grade = reader.GetString(2)
                    });
             
                }
                conn.Close();
            }
            dgQualification.ItemsSource = qualifications; 
        }

        private void UpdateSerialNumbers()
        {
            for (int i = 0; i < qualifications.Count; i++)
            {
                qualifications[i].SNo = i + 1;
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            int empId;
            string name = txtName.Text;
            string address = txtAddress.Text;
            bool isNewEmp = false;

            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(address))
            {
                MessageBox.Show("Name and Address are required!");
                return;
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                if (cmbCode.SelectedItem != null && cmbCode.SelectedItem.ToString() != "-- New Employee --")
                {
                    //update part
                    empId = Convert.ToInt32(cmbCode.SelectedItem); //id for updating existing employee
                    SqlCommand cmdUpdate = new SqlCommand("UPDATE Employee SET Name=@Name, Address=@Address,Photo=@Photo WHERE Id=@Id", conn);
                    cmdUpdate.Parameters.AddWithValue("@Id", empId);
                    cmdUpdate.Parameters.AddWithValue("@Name", name);
                    cmdUpdate.Parameters.AddWithValue("@Address", address);
                    if (string.IsNullOrEmpty(currentPhotoPath)) { currentPhotoPath = "pack://application:,,,/Images/no-image.png"; }

                    cmdUpdate.Parameters.AddWithValue("@Photo", currentPhotoPath);
                    cmdUpdate.ExecuteNonQuery();

                    //since WPF does not know which rows are new, modified and deleted, its easy to delete and re-insert.
                    SqlCommand cmdDel = new SqlCommand("DELETE FROM Qualification WHERE EmpId = @EmpId and PhotoType= @PhotoType", conn);
                    cmdDel.Parameters.AddWithValue("@EmpId", empId);
                    cmdDel.ExecuteNonQuery();

                }
                else
                {
                    //insert new employee
                    SqlCommand cmdAdd = new SqlCommand("INSERT INTO Employee(Name, Address, Photo) OUTPUT INSERTED.Id VALUES(@Name, @Address, @Photo)", conn);
                    cmdAdd.Parameters.AddWithValue("@Name", name);
                    cmdAdd.Parameters.AddWithValue("@Address", address);
                    if(string.IsNullOrEmpty(currentPhotoPath)) { currentPhotoPath = "pack://application:,,,/Images/no-image.png"; }

                    cmdAdd.Parameters.AddWithValue("@Photo", currentPhotoPath);

                    bool hasValidQualification = qualifications.Any(q =>
                   !string.IsNullOrWhiteSpace(q.Department) ||
                   !string.IsNullOrWhiteSpace(q.University) ||
                   !string.IsNullOrWhiteSpace(q.Grade));

                    if (!hasValidQualification)
                    {
                        MessageBox.Show("At least one qualification entry is required!");
                        return;
                    }

                    empId = (int)cmdAdd.ExecuteScalar(); // get new ID
                    isNewEmp = true;
                }

               
                // since need to insert qualification for both Add and Update operation
                foreach (var q in qualifications)
                {
                    // Skip blank rows of datagrid (if user hasn't filled anything)
                    if (string.IsNullOrWhiteSpace(q.Department) &&
                        string.IsNullOrWhiteSpace(q.University) &&
                        string.IsNullOrWhiteSpace(q.Grade))
                        continue;

                    SqlCommand cmdQ = new SqlCommand("INSERT INTO Qualification (Department, University, Grade, EmpId, PhotoType) VALUES (@Dept, @Univ, @Grade, @EmpId,@PhotoType)", conn);
                    cmdQ.Parameters.AddWithValue("@Dept", q.Department ?? "");
                    cmdQ.Parameters.AddWithValue("@Univ", q.University ?? "");
                    cmdQ.Parameters.AddWithValue("@Grade", q.Grade ?? "");
                    cmdQ.Parameters.AddWithValue("@EmpId", empId);
                    cmdQ.Parameters.AddWithValue("@PhotoType", "normal");
                    cmdQ.ExecuteNonQuery();
                }
                conn.Close();
            }
            if (isNewEmp) 
            {
                MessageBox.Show($"New Employee {name} created!");
            }
            else 
            {
                MessageBox.Show($"Employee {name} updated!");
            }
            LoadEmployees();
            //cmbCode.SelectedItem= empId; //reselect employee

        }
     
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if(cmbCode.SelectedItem == null) return;

            int empId= Convert.ToInt32(cmbCode.SelectedItem);

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
               

                SqlCommand cmdQuali = new SqlCommand("DELETE FROM Qualification WHERE EmpId= @EmpId and PhotoType= @PhotoType", conn);
                cmdQuali.Parameters.AddWithValue("@EmpId", empId);
                cmdQuali.Parameters.AddWithValue("@PhotoType", "normal");
                cmdQuali.ExecuteNonQuery();

                SqlCommand cmdEmp = new SqlCommand("DELETE FROM Employee WHERE Id= @Id", conn);
                cmdEmp.Parameters.AddWithValue("@Id", empId);
                cmdEmp.Parameters.AddWithValue("@PhotoType", "normal");
                cmdEmp.ExecuteNonQuery();

                conn.Close();
            }

            MessageBox.Show($"Employee {empId} Deleted.");
            ResetForm();
            LoadEmployees(); //refresh comboox
        }
        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            ResetForm();
        }

        private void ResetForm()
        {
            txtName.Clear();
            txtAddress.Clear();
            cmbCode.SelectedItem = "-- New Employee --";
            currentPhotoPath = "";
            imgPhoto.Source = new BitmapImage(new Uri("pack://application:,,,/Images/no-image.png"));

            qualifications.Clear();
            qualifications.Add(new Qualification());
            UpdateSerialNumbers();
        }

        //image part
        private void imgPhoto_MouseDown(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Image files (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png";

            if (dlg.ShowDialog() == true)
            {
                currentPhotoPath = dlg.FileName;
                imgPhoto.Source = new BitmapImage(new Uri(currentPhotoPath));
            }
        }

        private void GoToSPWindow_Click(object sender, RoutedEventArgs e)
        {
            EmployeeSP spWindow = new EmployeeSP();
            spWindow.Show();
        }
        private void GoToTVPWindow_Click(object sender, RoutedEventArgs e)
        {
            Day8_UDT_Byte_SP tvpWindow = new Day8_UDT_Byte_SP();
            tvpWindow.Show();
        }

    }
}