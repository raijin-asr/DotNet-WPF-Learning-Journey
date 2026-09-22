using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace EmployeeMasterCRUD.IntegraionTest
{
    public class EmployeeServiceIT
    {
        private readonly string _connectionString;

        public EmployeeServiceIT(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<int> GetAllEmployeeIds()
        {
            List<int> employeeIds = new List<int>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("GetAllEmployeePhotoByteIds", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    employeeIds.Add(reader.GetInt32(0));
                }

                conn.Close();
            }

            return employeeIds;
        }
    }
}
