using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsyncAwait_WPF_Login_CRUD
{
    public class PersonAuthModel
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Username { get; set; }
        public bool RememberMe { get; set; }
    }
}
