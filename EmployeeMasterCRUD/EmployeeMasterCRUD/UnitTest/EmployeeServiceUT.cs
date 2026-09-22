using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace EmployeeMasterCRUD.UnitTest
{
    public interface IEmployeeRepository //this interface defines the contract for the repository that will be used by EmployeeServiceUT.
    {
        List<int> GetAllEmployeeIds();
    }
    //this unit test  testable because it depends on an interface, not a database.
    public class EmployeeServiceUT
    {
        private readonly IEmployeeRepository _repository; //A private field to hold the actual object that will provide the data.. It's declared as the interface type — so it can accept any class that implements that interface.

        public EmployeeServiceUT(IEmployeeRepository repository) //The constructor takes an IEmployeeRepository as a parameter. This allows you to pass in any class that implements the IEmployeeRepository interface, making it easy to swap out the actual database access with a mock or fake version for testing.
        {
            _repository = repository; //This is called dependency injection — you're "injecting" the dependency into the service.
        }

        public int GetEmployeeCount() //this method is testable because it depends on an interface, not a database.
        {
            var employees = _repository.GetAllEmployeeIds();
            return employees.Count;
        }
    }
}
