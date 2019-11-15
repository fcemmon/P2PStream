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
using System.IO;
using System.Net;

// including the M2Mqtt Library
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace LocalVideoStreamApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : Window
    {
        MqttClient client;
        byte[] data;
        public MainWindow()
        {
            InitializeComponent();
        //    client = new MqttClient("broker.streaming.video");
            client = new MqttClient("broker.mqtt-dashboard.com", 1883, false, null, null, MqttSslProtocols.None);
            string clientId = Guid.NewGuid().ToString();
            client.Connect(clientId, "admin", "12345678");
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            client.Disconnect();

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
                string sFileName = dlg.FileName;
                if (dlg.Multiselect)
                {
                    string[] arrAllFiles = dlg.FileNames; //used when Multiselect = true 
                }

                BitmapImage bitmapImage = new BitmapImage(new Uri(sFileName)); ;
                JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bitmapImage));
                using (MemoryStream ms = new MemoryStream())
                {
                    encoder.Save(ms);
                    data = ms.ToArray();
                }
            }
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            string Topic = "stream/file";

            // publish a message with QoS 2
            client.Publish(Topic, data, MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);
        }
    }
}
