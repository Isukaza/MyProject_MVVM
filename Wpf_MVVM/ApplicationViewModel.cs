using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text.Json;
using System.Net;
using System.IO;
using System.Windows;
using System.Linq;

namespace Wpf_MVVM
{
    public class ApplicationViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public ObservableCollection<DataPage> DataPages { get; set; }
        private readonly JsonSerializerOptions options = new() { WriteIndented = true };

        private DataPage selectedDataPage, dataPage;

        private RelayCommand addCommand;
        private RelayCommand showCommand;
        private RelayCommand removeCommand;
        private RelayCommand modifyCommand;

        //Добавление записи в БД ✔
        public RelayCommand AddCommand
        {
            get
            {
                return addCommand ??= new RelayCommand(obj =>
                {
                    if (obj is DataPage dataPage)
                    {
                        dataPage = obj as DataPage;
                        string str = JsonSerializer.Serialize(dataPage, options);

                        MyDebug(dataPage);
                        
                        //ConnectServer("/Create?str=" + str);
                    }
                });
            }
        }

        //Вывести данные в DataGrid ✔
        public RelayCommand ShowCommand
        {
            get
            {
                return showCommand ??= new RelayCommand(obj =>
                {
                    try
                    {
                        HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://localhost:44339/MyProject/Read");

                        using HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                        using StreamReader reader = new(response.GetResponseStream());

                        DataPages.Clear();
                        foreach (var item in JsonSerializer.Deserialize<DataList>(reader.ReadToEnd()).Data)
                        {
                            DataPages.Add(item);
                        }
                    }
                    catch (WebException e)
                    {
                        MessageBox.Show(e.Message);
                    }
                });
            }
        }

        //Применить изменения к строке ✔
        public RelayCommand ModifyCommand
        {
            get
            {
                return modifyCommand ??= new RelayCommand(obj =>
                {
                    if (obj is DataPage dataPage)
                    {
                        dataPage = obj as DataPage;
                        string str = JsonSerializer.Serialize(dataPage, options);
                        ConnectServer("/Update?str=" + str);
                    }
                });
            }
        }

        //Удаление элемента ✔
        public RelayCommand RemoveCommand
        {
            get
            {
                return removeCommand ??= new RelayCommand(obj =>
                {
                    if (obj is DataPage dataPage)
                    {
                        dataPage = obj as DataPage;
                        ConnectServer("/Delete?id=" + dataPage.Id.ToString());
                    }
                },
                 (obj) => DataPages.Count > 0);
            }
        }


        //Свойства интерфеса
        public DataPage SelectedDataPage
        {
            get { return selectedDataPage; }
            set
            {
                if (value != null)
                {
                    selectedDataPage = value;
                    SelectedItemDataGrid = value.Clone() as DataPage;
                }
                OnPropertyChanged("SelectedDataPage");
            }
        }
        public DataPage SelectedItemDataGrid
        {
            get { return dataPage; }
            set
            {
                if (value.Deptstr != null) value.DeptL = value.Deptstr.Split("\r\n").ToList();
                dataPage = value;
                OnPropertyChanged("SelectedItemDataGrid");
            }
        }

        public ApplicationViewModel()
        {
            DataPages = new()
            {
                new DataPage { Id = 1, Name = "Test", Company = "Microsoft", DeptL = new() { "Programmer", "Designer" }, Login = "Test", Password = "TestData" }   
            };
        }

        //Технические методы
        public static void ConnectServer(string datareques )
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://localhost:44339/MyProject" + datareques);
                using HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                MessageBox.Show($"Status:{response.StatusCode}");
                
            }
            catch (WebException e)
            {
                MessageBox.Show(e.Message);
            }
        }
        public static void MyDebug(DataPage page) => MessageBox.Show(
                $"Id:{page.Id}\n" +
                $"Name:{page.Name}\n" +
                $"Company:{page.Company}\n" +
                $"Dept:{page.Deptstr}\n" +
                $"Login:{page.Login}\n" +
                $"Password:{page.Password}",
                "Debug Message"
                );
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
