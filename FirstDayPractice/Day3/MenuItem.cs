using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace FirstDayPractice.Day3
{
    class MenuItem
    {
        public string Title { get; set; }
        public UserControl Content {  get; set; }

        public MenuItem(string title, UserControl content)
        {
            this.Title = title;
            this.Content = content;
        }
    }
}
