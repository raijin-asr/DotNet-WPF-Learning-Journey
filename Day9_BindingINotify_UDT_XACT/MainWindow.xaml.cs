using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Microsoft.Win32;

using Microsoft.Data.SqlClient;
using System.ComponentModel;


namespace Day9_BindingINotify_UDT_XACT
{

    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        
        private string connectionString = "Server=localhost;Database=PersonDB_asr;User Id=sa;Password=123;Encrypt=True;TrustServerCertificate=True;";

        public ObservableCollection<Person> PersonList { get; set; } = new(); //this is list to store person

        private Person _selectedPerson = new();
        public Person SelectedPerson
        {
            get => _selectedPerson;
            set
            {
                _selectedPerson = value;
                OnPropertyChanged(nameof(SelectedPerson));
            }
        }


        private ObservableCollection<Qualification> qualifications = new(); //special list to add or remove items from datagrid

        private byte[]? selectedPhotoBytes = null;

        // INotifyProperty Implementation
        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            dgQualification.ItemsSource = qualifications;

            try
            {
                LoadPersonId();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void LoadPersonId()
        {
            PersonList.Clear();
            PersonList.Add(new Person { Id = 0});
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("GetPersonID", conn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        PersonList.Add(new Person { Id = reader.GetInt32(0) });
                    }
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading person IDs: {ex.Message}");
            }

            SelectedPerson = PersonList[0];
            OnPropertyChanged(nameof(SelectedPerson));
        }

        private void cmbCode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbCode.SelectedItem is not Person selected) return;

            if (selected.Id == 0) 
            {
                ResetForm();
            }
            else
            {
                LoadPersonDetails(selected.Id);
                LoadQualifications(selected.Id);
                UpdateSerialNumbers();
            }

        }

        private void LoadPersonDetails(int personId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new("GetPersonById", conn) { CommandType = CommandType.StoredProcedure };
                cmd.Parameters.AddWithValue("@Id", personId);

                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    SelectedPerson.Name = reader.GetString(0);
                    SelectedPerson.Address = reader.GetString(1);

                    //Check for null or empty string before setting image source
                    if (!reader.IsDBNull(2))
                        {
                            selectedPhotoBytes = (byte[])reader["Photo"];
                            imgPhoto.Source = ConvertToImage(selectedPhotoBytes);
                            SelectedPerson.Photo = selectedPhotoBytes;
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
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading person details: {ex.Message}");
            }
        }


        private void LoadQualifications(int personId)
        {
            qualifications.Clear(); //clear current list of qualifications when switching Person or resetting form
            try 
            {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("GetBindQualificationsByPersonId", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@PersonId", personId);

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

            if (qualifications.Count == 0)
                qualifications.Add(new Qualification());

            UpdateSerialNumbers();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading qualifications: {ex.Message}");
            }
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
            string name = SelectedPerson.Name?.Trim()??"";
            string address = SelectedPerson.Address?.Trim() ?? "";

            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(address))
            {
                MessageBox.Show("Name and address cannot be empty.");
                return;
            }

            if (!qualifications.Any(q => !string.IsNullOrWhiteSpace(q.Department) || !string.IsNullOrWhiteSpace(q.University) || !string.IsNullOrWhiteSpace(q.Grade)))
            {
                MessageBox.Show("At least one qualification entry is required!");
                return;
            }

            int personId = SelectedPerson.Id;
            bool isNewPerson = personId ==0; 

            DataTable qualificationTable = GetQualificationsDataTable();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("SaveOrUpdatePersonWithQualifications", conn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Id", personId);
                    cmd.Parameters.AddWithValue("@Name", name);
                    cmd.Parameters.AddWithValue("@Address", address);
                    cmd.Parameters.Add("@Photo", SqlDbType.VarBinary).Value = selectedPhotoBytes ?? (object)DBNull.Value;


                    SqlParameter qParam = cmd.Parameters.AddWithValue("@Qualifications", qualificationTable);
                    qParam.SqlDbType = SqlDbType.Structured;
                    qParam.TypeName = "QualificationTVP";

                    object result = cmd.ExecuteScalar();
                    personId = Convert.ToInt32(result);

                    conn.Close();
                }

                MessageBox.Show(isNewPerson ? $"New Person '{name}' created!" : $"Person '{name}' updated!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving/updating person: {ex.Message}");
                return;
            }
            LoadPersonId();
            cmbCode.SelectedIndex = 0; 

        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedPerson.Id <= 0) return;

            try 
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmdDel = new SqlCommand("DeletePersonAndQualifications", conn);
                    cmdDel.CommandType = CommandType.StoredProcedure;

                    cmdDel.Parameters.AddWithValue("@Id", SelectedPerson.Id);
                    cmdDel.ExecuteNonQuery();

                    conn.Close();
                }

                MessageBox.Show($"Person ({SelectedPerson.Id}) Deleted.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting person: {ex.Message}");
            }

            ResetForm();
            LoadPersonId(); //refresh comboox
        }
        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            ResetForm();
        }

        private void ResetForm()
        {
            SelectedPerson = new Person { Id = 0 }; 
            //cmbCode.SelectedIndex = 0;

            selectedPhotoBytes = null;
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
                selectedPhotoBytes = File.ReadAllBytes(dlg.FileName);
                imgPhoto.Source = new BitmapImage(new Uri(dlg.FileName));
                SelectedPerson.Photo = selectedPhotoBytes;
            }
        }

        //helper method to convert image to byte array
        public BitmapImage ConvertToImage(byte[] imageData)
        {
            
            using (MemoryStream ms = new MemoryStream(imageData))
            {
                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.StreamSource = ms;
                image.EndInit();
                image.Freeze(); 
                return image;
            }
        }


        private DataTable GetQualificationsDataTable() //must pass DataTable to use TVP to transmit multiple rows of degree(slc, bachelor, etc)
        {
            var dt = new DataTable();
            dt.Columns.Add("Department", typeof(string));
            dt.Columns.Add("University", typeof(string));
            dt.Columns.Add("Grade", typeof(string));

            foreach (var q in qualifications)
            {
                if (string.IsNullOrWhiteSpace(q.Department) &&
                    string.IsNullOrWhiteSpace(q.Department) &&
                    string.IsNullOrWhiteSpace(q.Department))
                    continue;

                dt.Rows.Add(q.Department ?? "", q.University ?? "", q.Grade ?? "");
            }
            return dt;
        }

        private void btnAsync_Click(object sender, RoutedEventArgs e)
        {
            AsyncAwaitWindow asyncAwaitWindow = new AsyncAwaitWindow();
            asyncAwaitWindow.Show();
        }
    }
}