using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmployeeMasterCRUD.UnitTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;

namespace EmployeeMasterCRUD.IntegraionTest.Tests
{
    [TestClass()]
    public class EmployeeServiceTestsUT
    {

        //This test checks: “What happens if there are employees?”
        [TestMethod()]
        public void GetEmployeeCount_ReturnsCorrectCount()
        {
            // Arrange
            var mockRepo = new Mock<IEmployeeRepository>(); //create a mock(a fake version) prevents real database access — we simulate the behavior.
            
            mockRepo.Setup(repo => repo.GetAllEmployeeIds())
                    .Returns(new List<int> { 101, 102, 103 }); //You tell the mock: “If someone calls GetAllEmployeeIds(), return this list: [101, 102, 103]”

            var service = new EmployeeServiceUT(mockRepo.Object); //pass the mock object into the EmployeeService. This way, EmployeeService thinks it's talking to a real repository — but it's fake!

            // Act
            int count = service.GetEmployeeCount(); //call the method you want to test: GetEmployeeCount(). It will use the mock repository, which returns 3 employee IDs.

            // Assert
            Assert.AreEqual(3, count); //check if the count is 3. If it is, the test passes. If not, it fails.
        }

        //This test checks: “What happens if there are no employees?”
        [TestMethod()]
        public void GetEmployeeCount_EmptyList_ReturnsZero()
        {
            // Arrange
            var mockRepo = new Mock<IEmployeeRepository>();
            mockRepo.Setup(repo => repo.GetAllEmployeeIds())
                    .Returns(new List<int>()); // Here, you tell the mock: “If someone calls GetAllEmployeeIds(), return an empty list.”

            var service = new EmployeeServiceUT(mockRepo.Object);

            // Act
            int count = service.GetEmployeeCount();

            // Assert
            Assert.AreEqual(0, count);
        }
    }
}