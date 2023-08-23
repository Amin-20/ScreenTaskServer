using ScreenRecorder_Server_Side.Commands;
using ScreenRecorder_Server_Side.Helpers;
using ScreenRecorder_Server_Side.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScreenRecorder_Server_Side.Domain.ViewModel
{
    public class MainWindowViewModel : BaseViewModel
    {
        public RelayCommand StartServerCommand { get; set; }

        private ObservableCollection<Client> allClients;

        public ObservableCollection<Client> AllClients
        {
            get { return allClients; }
            set { allClients = value; OnPropertyChanged(); }
        }

        [Obsolete]
        public MainWindowViewModel()
        {
            StartServerCommand = new RelayCommand((obj) =>
            {
                ImageHelper helper = new ImageHelper();

                Task.Run(() =>
                {
                    NetworkHelper.Connect();
                }).Wait(100);

                Task.Run(async () =>
                {
                    while (true)
                    {
                        await Task.Delay(5);
                        App.Current.Dispatcher.Invoke(() =>
                        {
                            try
                            {
                                AllClients = new ObservableCollection<Client>(NetworkHelper.Clients);
                                foreach (var item in NetworkHelper.Clients)
                                {
                                    string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + $"\\{item.TcpClient.Client.RemoteEndPoint.ToString().Replace(":", ".")}.Images";
                                    if (!Directory.Exists(folderPath))
                                    {
                                        helper.CreateFolder(folderPath);
                                    }

                                    Task.Run(() =>
                                    {
                                        var stream = item.TcpClient.GetStream();
                                        var br = new BinaryReader(stream);
                                        while (true)
                                        {
                                            try
                                            {
                                                br = new BinaryReader(stream);
                                                var imageBytes = br.ReadBytes(130000);
                                                var path = helper.GetImagePath(imageBytes, folderPath);
                                                    App.Current.Dispatcher.Invoke(() =>
                                                    {
                                                        AllClients[NetworkHelper.Clients.IndexOf(item)] = new Client
                                                        {
                                                            TcpClient = item.TcpClient,
                                                            ImagePath = path
                                                        };
                                                    });
                                            }
                                            catch (Exception)
                                            {
                                                Console.WriteLine($"{item.TcpClient.Client.RemoteEndPoint}  disconnected");
                                            }
                                        }
                                    });
                                }
                            }
                            catch (Exception)
                            {
                                AllClients.Clear();
                            }
                        });
                    }
                });
            });
        }
    }
}