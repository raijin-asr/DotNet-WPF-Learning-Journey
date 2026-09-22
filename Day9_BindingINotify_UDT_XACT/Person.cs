using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Day9_BindingINotify_UDT_XACT
{
    public class Person : INotifyPropertyChanged
    {
        private int _id;
        private string? _name;
        private string? _address;
        private byte[]? _photo;
        public string DisplayText => Id == 0 ? "-- New Person --" : Id.ToString();


        public int Id
        {
            get => _id;
            set { _id = value; OnPropertyChanged(nameof(Id)); }
        }

        public string? Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged(nameof(Name)); }
        }

        public string? Address
        {
            get => _address;
            set { _address = value; OnPropertyChanged(nameof(Address)); }
        }

        public byte[]? Photo
        {
            get => _photo;
            set { 
                _photo = value; 
                OnPropertyChanged(nameof(Photo));
            }
        }

        //implementation of INotifyPropertyChanged
        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
