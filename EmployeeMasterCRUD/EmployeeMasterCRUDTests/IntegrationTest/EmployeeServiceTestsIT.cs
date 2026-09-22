using Microsoft.VisualStudio.TestTools.UnitTesting;
using EmployeeMasterCRUD.UnitTest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeMasterCRUD.IntegraionTest.Tests
{
    [TestClass()]
    public class EmployeeServiceTestsIT
    {
        [TestMethod()]
        public void EmployeeServiceTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetAllEmployeeIdsTest()
        {
            Assert.Fail();
        }
        [TestMethod()]
        public void GetAllEmployeeIds_ReturnsIds()
        {
            // Arrange
            string connectionString = "Server=localhost;Database=EmployeeMasterDB;User Id=sa;Password=123;Encrypt=True;TrustServerCertificate=True;";
            var service = new EmployeeServiceIT(connectionString);

            // Act
            List<int> ids = service.GetAllEmployeeIds();

            // Assert
            Assert.IsNotNull(ids);
            Assert.IsTrue(ids.Count >= 0); // Could be 0, or more
        }
    }
}