using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day9_BindingINotify_UDT_XACT
{
    public class Qualification : INotifyPropertyChanged
    {
        private int _sno;
        private string? _department;
        private string? _university;
        private string? _grade;

        public int SNo
        {
            get => _sno;
            set { _sno = value; OnPropertyChanged(nameof(SNo)); }
        }

        public string? Department
        {
            get => _department;
            set { _department = value; OnPropertyChanged(nameof(Department)); }
        }

        public string? University
        {
            get => _university;
            set { _university = value; OnPropertyChanged(nameof(University)); }
        }

        public string? Grade
        {
            get => _grade;
            set { _grade = value; OnPropertyChanged(nameof(Grade)); }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
