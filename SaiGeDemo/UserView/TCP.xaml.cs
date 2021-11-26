using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net.Sockets;
using System.Net;

namespace SaiGeDemo.UserView
{


    public class TCPViewModel
    {
        public string IPtext { get; set; } = "127.0.0.1";

        public int Endpoint { get; set; } = 8888;

        public string Text { get; set; }
    }
    /// <summary>
    /// TCP.xaml 的交互逻辑
    /// </summary>
    public partial class TCP : UserControl
    {

        public TCPViewModel viewModel;

        public delegate void ShowLog(string LogText);

        public ShowLog ShowLogHandle = (string LogText) => { };


        private Socket tcpClient;
        public TCP()
        {
            InitializeComponent();
            viewModel = new TCPViewModel();
            this.DataContext = viewModel;


        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {           
            tcpClient.Send(Encoding.UTF8.GetBytes(viewModel.Text));
            ShowLogHandle($"发送 {viewModel.IPtext} 成功！内容：{viewModel.Text}");

        }

        private Socket InitTcp()
        {
            var Client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress iPAddress = IPAddress.Parse(viewModel.IPtext);
            EndPoint point = new IPEndPoint(iPAddress, viewModel.Endpoint);
            Client.Connect(point);
            ShowLogHandle($"连接 {viewModel.IPtext} 成功！");
           
            return Client;
        }

        private void Button_Click1(object sender, RoutedEventArgs e)
        {
            try
            {
                tcpClient = InitTcp();
            }
            catch (Exception ex)
            {
                ShowLogHandle(ex.Message);
            }
            
        }

        private void Button_Click2(object sender, RoutedEventArgs e)
        {
            tcpClient.Close();
            ShowLogHandle($"连接 {viewModel.IPtext} 关闭！");
        }

        private void Button_Click3(object sender, RoutedEventArgs e)
        {
            byte[] data = new byte[1024];
            int length = tcpClient.Receive(data);
            string message = Encoding.UTF8.GetString(data, 0, length);
            ShowLogHandle($"接收 {viewModel.IPtext} 内容：{message}！");
        }
    }
}
