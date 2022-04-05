using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text.Json;
using System.Net;
using System.IO;
using System.Windows;
using System.Linq;
using System.Net.Http;
using System;
using System.Threading.Tasks;

namespace Wpf_MVVM
{
    public class ApplicationViewModel : INotifyPropertyChanged
    {
        static readonly HttpClient client = new();
        static readonly string uri = "https://localhost:44339/MyProject/";

        public event PropertyChangedEventHandler? PropertyChanged;
        private readonly JsonSerializerOptions options = new() { WriteIndented = true };

        public ObservableCollection<DataPage> DataPages { get; set; }
        private DataPage selectedDataPage, dataPage;

        //Добавление записи в БД ✔
        private RelayCommand addCommand;
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

                        SendData("Create?str=" + str);
                    }
                });
            }
        }

        //Вывести данные в DataGrid ✔

        private RelayCommand showCommand;
        public RelayCommand ShowCommand
        {
            get
            {
                return showCommand ??= new RelayCommand(async obj =>
                {
                    try
                    {
                        string? json = await GetData("Read");
                        if (json != null)
                        {
                            DataPages.Clear();

                            foreach (var item in JsonSerializer.Deserialize<DataList>(json).Data)
                            {
                                DataPages.Add(item);
                            }
                        }
                    }
                    catch (HttpRequestException e)
                    {
                        MessageBox.Show(e.Message);
                    }
                });
            }
        }

        //Применить изменения к строке ✔
        private RelayCommand modifyCommand;
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
 
                        SendData("Update?str=" + str);
                    }
                });
            }
        }

        //Удаление элемента ✔
        private RelayCommand removeCommand;
        public RelayCommand RemoveCommand
        {
            get
            {
                return removeCommand ??= new RelayCommand(obj =>
                {
                    if (obj is DataPage dataPage)
                    {
                        dataPage = obj as DataPage;

                        SendData("Delete?id=" + dataPage.Id.ToString());
                    }
                },
                 (obj) => DataPages.Count > 0);
            }
        }

        //Свойства интерфеса
        //Получение данных из DG
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
        //Получение и присвоение данных textbox и combobox
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
        static async Task<string> GetData(string datareques)
        {
            try
            {
                return await client.GetStringAsync(uri + datareques);
            }catch(HttpRequestException e)
            {
                MessageBox.Show(e.Message);
                return null;
            }
        }
        static void SendData(string datareques )
        {
            try
            {
                HttpRequestMessage request = new();
                request.RequestUri = new Uri(uri + datareques);
                HttpResponseMessage response = client.Send(request);

                MessageBox.Show($"Status:{response.StatusCode}");
            }
            catch (HttpRequestException e)
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
        public static void MyDebug(string str) => MessageBox.Show(str);
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
