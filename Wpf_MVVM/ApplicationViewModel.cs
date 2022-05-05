using System.Collections.ObjectModel;
using System.Text.Json;
using System.Net;
using System.Windows;
using System.Linq;
using System.Net.Http;
using System;
using System.Threading.Tasks;
using Calcium.UIModel;
using Calcium.UIModel.Input;

namespace Wpf_MVVM
{
    public class ApplicationViewModel : ViewModelBase
    {
        static readonly HttpClient client = new();
        private readonly JsonSerializerOptions options = new() { WriteIndented = true };

        static readonly string URL = "https://localhost:44339/MyProject/";

        private DataPage? selectedData;
        public DataPage? SelectedData
        {
            get => selectedData;
            set
            {
                Set(ref selectedData, value);
                if(value!=null) TextBoxData = value.Clone() as DataPage;
            }
        }

        private static DataPage? textboxdata;
        public DataPage? TextBoxData
        {
            get => textboxdata;
            set
            {
                if (value.Deptstr != null) value.DeptL = value.Deptstr.Split("\r\n").ToList();
                Set(ref textboxdata, value);
            }
        }

        private static ObservableCollection<DataPage>? dataPages;
        public static ObservableCollection<DataPage>? DataPages
        {
            get => dataPages;
            set
            {
                dataPages = value;
            }
        }

        public ActionCommand AddCommand { get; }
        public ActionCommand ShowCommand { get; }
        public ActionCommand ModifyCommand { get; }
        public ActionCommand RemoveCommand { get; }

        public ApplicationViewModel()
        {
            TextBoxData = new();
            DataPages = new();

            AddCommand = new(_ => Add());
            ShowCommand = new(_ => Show());
            RemoveCommand = new(_ => Delete());
            ModifyCommand = new(_=> Modify());
        }

        //Добавление записи в БД ✔
        void Add()
        {
            if (TextBoxData != null)
            {
                string str = JsonSerializer.Serialize(textboxdata, options);
                SendData("Create?str=" + str);
            }
        }

        //Вывести данные в DataGrid ✔
        async void Show()
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

        //Применить изменения к строке ✔
        void Modify()
        {
            if (TextBoxData != null)
            {
                string str = JsonSerializer.Serialize(TextBoxData, options);
                SendData("Update?str=" + str);
            }
        }

        //Удаление элемента ✔
        void Delete()
        {
            if (TextBoxData != null)
            {
                SendData("Delete?id=" + TextBoxData.Id.ToString());
            }
        }
       
        //Получение данных с сервера
        async Task<string> GetData(string datareques)
        {
            try
            {
                return await client.GetStringAsync(URL + datareques);
            }
            catch (HttpRequestException e)
            {
                MessageBox.Show(e.Message);
                return null;
            }
        }
        //Отправка данных на сервер
        void SendData(string datareques)
        {
            try
            {
                HttpRequestMessage request = new();
                request.RequestUri = new Uri(URL + datareques);
                HttpResponseMessage response = client.Send(request);

                MessageBox.Show($"Status:{response.StatusCode}");

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    Show();
                }
            }
            catch (HttpRequestException e)
            {
                MessageBox.Show(e.Message);
            }
        }
        //Технические методы
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
    }
}
