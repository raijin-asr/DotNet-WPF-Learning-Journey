using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Microsoft.Data.SqlClient;
using Microsoft.Win32;
using System.Xml.Linq;
using System.Data;

namespace EmployeeMasterCRUD.Day7StoredProcedurePractice
{
   
    public partial class EmployeeSP : Window
    {
        private string connectionString = "Server=localhost;Database=EmployeeMasterDB;User Id=sa;Password=123;Encrypt=True;TrustServerCertificate=True;";
        private int selectedEmployeeId = -1; //-1 means no employee selected
        private ObservableCollection<Qualification> qualifications = new ObservableCollection<Qualification>(); //special list to add or remove items from datagrid

        private string currentPhotoPath = "";

        public EmployeeSP()
        {
            InitializeComponent();
            try
            {
                LoadEmployees("PhotoPath");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }

            dgQualification.ItemsSource = qualifications;
        }

        private void LoadEmployees(string photoType)
        {
            cmbCode.Items.Clear(); //clear combobox and prevent duplicate
            cmbCode.Items.Add("-- New Employee --");

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("GetAllEmployeeIds", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@PhotoType", photoType);

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
                LoadEmployeeDetails(selectedEmployeeId, "PhotoPath");
                LoadQualifications(selectedEmployeeId, "PhotoPath");
                UpdateSerialNumbers();
            }

        }

        private void LoadEmployeeDetails(int empId, string photoType)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("GetEmployeeById", conn);
                cmd.CommandType= CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", empId);
                cmd.Parameters.AddWithValue("@PhotoType", photoType);

                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    txtName.Text = reader.GetString(0);
                    txtAddress.Text = reader.GetString(1);
                    string? photoFromDb = reader.IsDBNull(2) ? null : reader.GetString(2);

                    // Check for null or empty string before setting image source
                    if (!string.IsNullOrWhiteSpace(photoFromDb))
                    {
                        currentPhotoPath = photoFromDb;
                        try
                        {
                            imgPhoto.Source = new BitmapImage(new Uri(currentPhotoPath, UriKind.Absolute));
                        }
                        catch (UriFormatException)
                        {
                            // Fallback to default image if URI is malformed
                            imgPhoto.Source = new BitmapImage(new Uri("pack://application:,,,/Images/no-image.png"));
                        }
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
                    SqlCommand cmdUpdate = new SqlCommand("UpdateEmployee", conn);
                    cmdUpdate.CommandType = CommandType.StoredProcedure;

                    cmdUpdate.Parameters.AddWithValue("@Id", empId);
                    cmdUpdate.Parameters.AddWithValue("@Name", name);
                    cmdUpdate.Parameters.AddWithValue("@Address", address);
                    cmdUpdate.Parameters.AddWithValue("@Photo", currentPhotoPath);
                    cmdUpdate.ExecuteNonQuery();

                    //since WPF does not know which rows are new, modified and deleted, its easy to delete and re-insert.
                    SqlCommand cmdDel = new SqlCommand("DeleteQualificationsByEmpId", conn);
                    cmdDel.CommandType = CommandType.StoredProcedure;

                    cmdDel.Parameters.AddWithValue("@EmpId", empId);
                    cmdDel.Parameters.AddWithValue("@PhotoType", "PhotoPath");



                    cmdDel.ExecuteNonQuery();

                }
                else
                {
                    //insert new employee
                    SqlCommand cmdAdd = new SqlCommand("insertEmployee", conn);
                    cmdAdd.CommandType = CommandType.StoredProcedure;

                    cmdAdd.Parameters.AddWithValue("@Name", name);
                    cmdAdd.Parameters.AddWithValue("@Address", address);
                    cmdAdd.Parameters.AddWithValue("@Photo", currentPhotoPath);

                    bool hasValidQualification1 = qualifications.Any(q =>
                        !string.IsNullOrWhiteSpace(q.Department) ||
                        !string.IsNullOrWhiteSpace(q.University) ||
                        !string.IsNullOrWhiteSpace(q.Grade));

                    if (!hasValidQualification1)
                    {
                        MessageBox.Show("At least one qualification entry is required!");
                        return;
                    }

                    empId = Convert.ToInt32(cmdAdd.ExecuteScalar()); // get new ID
                    isNewEmp = true;
                }
                bool hasValidQualification = qualifications.Any(q =>
                 !string.IsNullOrWhiteSpace(q.Department) ||
                 !string.IsNullOrWhiteSpace(q.University) ||
                 !string.IsNullOrWhiteSpace(q.Grade));

                if (!hasValidQualification)
                {
                    MessageBox.Show("At least one qualification entry is required!");
                    return;
                }

                // since need to insert qualification for both Add and Update operation
                foreach (var q in qualifications)
                {
                    // Skip blank rows of datagrid (if user hasn't filled anything)
                    if (string.IsNullOrWhiteSpace(q.Department) &&
                        string.IsNullOrWhiteSpace(q.University) &&
                        string.IsNullOrWhiteSpace(q.Grade))
                        continue;

                    SqlCommand cmdQ = new SqlCommand("InsertQualification", conn);
                    cmdQ.CommandType = CommandType.StoredProcedure;

                    cmdQ.Parameters.AddWithValue("@Department", q.Department ?? "");
                    cmdQ.Parameters.AddWithValue("@University", q.University ?? "");
                    cmdQ.Parameters.AddWithValue("@Grade", q.Grade ?? "");
                    cmdQ.Parameters.AddWithValue("@EmpId", empId);
                    cmdQ.Parameters.AddWithValue("@PhotoType", "PhotoPath"); 

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
            LoadEmployees("PhotoPath");
            //cmbCode.SelectedItem= empId; //reselect employee

        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (cmbCode.SelectedItem == null || cmbCode.SelectedItem.ToString() == "-- New Employee --") return;

            int empId = Convert.ToInt32(cmbCode.SelectedItem);

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmdDelEmp = new SqlCommand("DeleteQualificationsByEmpId", conn);
                cmdDelEmp.CommandType = CommandType.StoredProcedure;
                cmdDelEmp.Parameters.AddWithValue("@EmpId", empId);
                cmdDelEmp.Parameters.AddWithValue("@PhotoType", "PhotoPath");
                cmdDelEmp.ExecuteNonQuery();

                SqlCommand cmdDelQuali = new SqlCommand("DeleteEmployee", conn);
                cmdDelQuali.CommandType = CommandType.StoredProcedure;
                cmdDelQuali.Parameters.AddWithValue("@Id", empId);
                cmdDelQuali.ExecuteNonQuery();

                conn.Close();
            }

            MessageBox.Show($"Employee {empId} Deleted.");
            ResetForm();
            LoadEmployees("PhotoPath"); //refresh comboox
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

        //helper method to convert image to byte array

    }
}
