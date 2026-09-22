

using System.Windows;
using System.Windows.Controls;

using System.Windows.Input;
using System.Windows.Media.Imaging;

using Microsoft.Data.SqlClient;
using Microsoft.Win32;
using System.Data;
using System.Collections.ObjectModel;
using System.IO;

namespace EmployeeMasterCRUD.Day8_UDT_Byte
{
    /// <summary>
    /// Interaction logic for Day8_UDT_Byte_SP.xaml
    /// </summary>
    /// 

    public partial class Day8_UDT_Byte_SP : Window
    {
        private string connectionString = "Server=localhost;Database=EmployeeMasterDB;User Id=sa;Password=123;Encrypt=True;TrustServerCertificate=True;";
        private int selectedEmployeeId = -1; //-1 means no employee selected
        private ObservableCollection<Qualification> qualifications = new ObservableCollection<Qualification>(); //special list to add or remove items from datagrid

        private byte[]? selectedPhotoBytes = null;

        public Day8_UDT_Byte_SP()
        {
            InitializeComponent();
            try
            {
                LoadEmployees();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }

            dgQualification.ItemsSource = qualifications;
        }

        private void LoadEmployees()
        {
            cmbCode.Items.Clear(); //clear combobox and prevent duplicate
            cmbCode.Items.Add("-- New Employee --");

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("GetAllEmployeePhotoByteIds", conn);
                cmd.CommandType = CommandType.StoredProcedure;

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
                LoadQualifications(selectedEmployeeId, "PhotoByte");
                UpdateSerialNumbers();
            }

        }

        private void LoadEmployeeDetails(int empId)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("GetEmployeeByteById", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", empId);

                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    txtName.Text = reader.GetString(0);
                    txtAddress.Text = reader.GetString(1);

                    // Check for null or empty string before setting image source
                    if (!reader.IsDBNull(2))
                    {
                        selectedPhotoBytes = (byte[])reader["Photo"];
                        imgPhoto.Source = ConvertToImage(selectedPhotoBytes);
                    }
                    else
                    {
                        selectedPhotoBytes = null;
                        imgPhoto.Source = new BitmapImage(new Uri("pack://application:,,,/Images/no-image.png"));
                    }

                }
                conn.Close();
            }
        }


        private void LoadQualifications(int empId, string photoType)
        {
            qualifications.Clear(); //clear current list of qualifications when switching employee or resetting form

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("GetQualificationsByEmpId", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@EmpId", empId);
                cmd.Parameters.AddWithValue("@PhotoType", photoType);

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
            string name = txtName.Text.Trim();
            string address = txtAddress.Text.Trim();
            byte[]? photoBytes = selectedPhotoBytes;
            DataTable qualificationTable = GetQualificationsDataTable();

            bool isNewEmp = false;

            bool hasValidQualification = qualifications.Any(q =>
             !string.IsNullOrWhiteSpace(q.Department) ||
             !string.IsNullOrWhiteSpace(q.University) ||
             !string.IsNullOrWhiteSpace(q.Grade));

            if (!hasValidQualification)
            {
                MessageBox.Show("At least one qualification entry is required!");
                return;
            }

            if (cmbCode.SelectedItem == null || cmbCode.SelectedItem.ToString() == "-- New Employee --")
            {
                empId = 0;  // or -1, to indicate new insert
                isNewEmp = true;
            }
            else
            {
                empId = Convert.ToInt32(cmbCode.SelectedItem);
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand("SaveOrUpdateEmployeeWithQualifications", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Id", empId);
                cmd.Parameters.AddWithValue("@Name", name);
                cmd.Parameters.AddWithValue("@Address", address);
                cmd.Parameters.Add("@Photo", SqlDbType.VarBinary).Value = (object?)photoBytes ?? DBNull.Value;
                cmd.Parameters.AddWithValue("@PhotoType", "PhotoByte");

            

                SqlParameter qParam = cmd.Parameters.AddWithValue("@Qualifications", qualificationTable);
                qParam.SqlDbType = SqlDbType.Structured;
                qParam.TypeName = "QualificationTVP";

                object result = cmd.ExecuteScalar();
                empId = Convert.ToInt32(result);

                conn.Close();
            }

            selectedEmployeeId = empId;

            MessageBox.Show(isNewEmp ? $"New Employee '{name}' created!" : $"Employee '{name}' updated!");

            LoadEmployees();
            cmbCode.SelectedItem = empId;  // reselect employee


        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (cmbCode.SelectedItem == null) return;

            int empId = Convert.ToInt32(cmbCode.SelectedItem);

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmdDel = new SqlCommand("DeleteEmployeePhotoBytesAndQualifications", conn);
                cmdDel.CommandType= CommandType.StoredProcedure;

                cmdDel.Parameters.AddWithValue("@Id", empId);
                cmdDel.Parameters.AddWithValue("@PhotoType", "PhotoByte");
                cmdDel.ExecuteNonQuery();

                conn.Close();
            }

            MessageBox.Show($"Employee ({empId}) Deleted.");
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
            
            imgPhoto.Source = new BitmapImage(new Uri("pack://application:,,,/Images/no-image.png"));
            selectedPhotoBytes = null;

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
                selectedPhotoBytes = File.ReadAllBytes(dlg.FileName);
                imgPhoto.Source = new BitmapImage(new Uri(dlg.FileName));
            }
        }

        //helper method to convert image to byte array
        public BitmapImage ConvertToImage(byte[] imageData)
        {
            if (imageData == null || imageData.Length == 0)
                return new BitmapImage(new Uri("pack://application:,,,/Images/no-image.png"));

            using (MemoryStream ms = new MemoryStream(imageData))
            {
                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.StreamSource = ms;
                image.EndInit();
                image.Freeze(); // Optional but recommended for thread-safety
                return image;
            }
        }

    
        private DataTable GetQualificationsDataTable() //must pass DataTable to use TVP to transmit multiple rows of degree(slc, bachelor, etc)
        {
            var dt = new DataTable();
            dt.Columns.Add("Department", typeof(string));
            dt.Columns.Add("University", typeof(string));
            dt.Columns.Add("Grade", typeof(string));
            dt.Columns.Add("PhotoType", typeof(string));

            foreach (var q in qualifications)
            {
                if(string.IsNullOrWhiteSpace(q.Department) && 
                    string.IsNullOrWhiteSpace(q.Department) &&
                    string.IsNullOrWhiteSpace(q.Department))
                    continue;

                DataRow dr = dt.NewRow();
                dr["Department"] = q.Department ?? "";
                dr["University"] = q.University ?? "";
                dr["Grade"] = q.Grade ?? "";
                dr["PhotoType"] = "PhotoByte"; 
                dt.Rows.Add(dr);
            }
            return dt;
        }

    }
}
