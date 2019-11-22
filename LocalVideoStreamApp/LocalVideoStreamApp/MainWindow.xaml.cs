using System;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Net.Sockets;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;

namespace LocalVideoStreamApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : Window
    {
        private string filePath;
        Socket client;
        IPEndPoint srvEP;
        public MainWindow()
        {
            InitializeComponent();
            initilizeClient();
        }

        private void initilizeClient()
        {
            client = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            label.Content = "Insert Android Device IP";
            textBox.Text = "";
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            base.OnClosed(e);
            App.Current.Shutdown();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            // Configure open file dialog box
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.FileName = "Image"; // Default file name
            dlg.DefaultExt = ".png"; // Default file extension
            dlg.Filter = "All Files (*.*)|*.*"; // Filter files by extension
            dlg.Multiselect = true;

            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // Open document
                filePath = dlg.FileName;
                if (dlg.Multiselect)
                {
                    string[] arrAllFiles = dlg.FileNames; //used when Multiselect = true 
                }
            }
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(textBox.Text)) {
                string ipServer = textBox.Text.ToString();
                srvEP = new IPEndPoint(IPAddress.Parse(ipServer), 12346);

                BitmapImage bitmapImage = new BitmapImage(new Uri(filePath)); ;
                JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bitmapImage));
                byte[] data;
                int blockSize = 40 * 1024;
                using (MemoryStream ms = new MemoryStream())
                {
                    encoder.Save(ms);
                    data = ms.ToArray();
                }
                long cntRecive = data.Length / blockSize;
                long remainder = data.Length % blockSize;

                if (remainder > 0) cntRecive++;
                string cnt_str = cntRecive.ToString();
                var RequestData = Encoding.ASCII.GetBytes(cnt_str);
                client.SendTo(RequestData, srvEP);

                Thread.Sleep(1000);

                for (int i = 0; i < (remainder > 0 ? cntRecive - 1 : cntRecive); i++)
                {
                    //ms.Read(buffer, 0, 300);
                    byte[] buffer = SubArray(data, i * blockSize, blockSize);
                    client.SendTo(buffer, srvEP);
                    Thread.Sleep(100);
                }

                if (remainder > 0)
                {
                    int cnt = (int)cntRecive - 1;
                    //ms.Read(buffer, 0, (int)remainder);
                    byte[] buffer = SubArray(data, blockSize * cnt, (int)remainder);
                    client.SendTo(buffer, 0, (int)remainder, SocketFlags.None, srvEP);
                }

                RequestData = Encoding.ASCII.GetBytes(cnt_str);
            }
        }

        private byte[] SubArray(byte[] data, int index, int length)
        {
            byte[] result = new byte[length];
            Array.Copy(data, index, result,0, length);
            return result;
        }
    }
}
