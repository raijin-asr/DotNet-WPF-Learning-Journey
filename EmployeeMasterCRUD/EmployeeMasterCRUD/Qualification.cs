using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeMasterCRUD
{
    public class Qualification
    {
        public int SNo { get; set; }  // Serial number (not stored in DB)
        public string Department { get; set; }
        public string University { get; set; }
        public string Grade { get; set; }
    }
}
