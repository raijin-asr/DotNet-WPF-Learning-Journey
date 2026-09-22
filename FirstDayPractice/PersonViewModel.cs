using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirstDayPractice
{
    public class PersonViewModel
    {
        public ObservableCollection<Person> Persons { get; set; }

        public PersonViewModel() 
        {
            Persons = new ObservableCollection<Person>
            {
                new Person{ Name="Ameer Rai", Department="Technical"},
                new Person{ Name="Krishna Nepal", Department="Sales"}
            };
        }
    }
}
