using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using System.Windows;

namespace Wpf_MVVM
{
    public class DataList
    {
        private List<DataPage> data = new();
        public List<DataPage> Data
        {
            get { return data; }
            set { data = value; }
        }
    }
    public class DataPage : INotifyPropertyChanged, ICloneable
    {
        private int id;
        private string name;
        private string company;
        private List<string> deptL;
        private string login;
        private string password;
        private string deptstr ="";

        public int Id
        {
            get { return id; }
            set
            {
                id = value;
                OnPropertyChanged("Id");
            }
        }
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                OnPropertyChanged("Name");
            }
        }
        public string Company
        {
            get { return company; }
            set
            {
                company = value;
                OnPropertyChanged("Company");
            }
        }
        public List<string> DeptL
        {
            get { return deptL; }
            set
            {
                deptL = value;
                OnPropertyChanged("DeptL");
            }
        }
        [JsonIgnore]
        public string Deptstr
        {
            get
            {
                if (deptL != null) deptstr = string.Join("\r\n", deptL);
                return deptstr;
            }
            set
            {
                if (deptstr != null) deptL = value.Split("\r\n").ToList();
                deptstr = value;
                OnPropertyChanged("Deptstr");
            }
        }
        public string Login
        {
            get { return login; }
            set
            {
                login = value;
                OnPropertyChanged("Login");
            }
        }
        public string Password
        {
            get { return password; }
            set
            {
                password = value;
                OnPropertyChanged("Password");
            }
        }
        public object Clone()
        {
            return MemberwiseClone();
        }
        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
