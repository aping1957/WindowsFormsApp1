using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;
using System.Net;
using System.Net.Sockets;
using WindowsFormsApp1.Properties;

namespace WindowsFormsApp1
{
     public partial class Form1 : Form
    {
        private static Socket ConnectSocket(string server, int port)
        {
            Socket s = null;
            IPHostEntry hostEntry = null;

            // Get host related information.
            hostEntry = Dns.GetHostEntry(server);

            // Loop through the AddressList to obtain the supported AddressFamily. This is to avoid
            // an exception that occurs when the host IP Address is not compatible with the address family
            // (typical in the IPv6 case).
            foreach (IPAddress address in hostEntry.AddressList)
            {
                IPEndPoint ipe = new IPEndPoint(address, port);
                Socket tempSocket =
                    new Socket(ipe.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                tempSocket.Connect(ipe);

                if (tempSocket.Connected)
                {
                    s = tempSocket;
                    break;
                }
                else
                {
                    continue;
                }
            }
            return s;
        }
        
        // This method requests the home page content for the specified server.
        private static string SocketSendReceive(string server, int port)
        {
            string request = "GET /znyx.asp HTTP/1.1\r\nHost: " + server +
                "\r\nConnection: Close\r\n\r\n";
            Byte[] bytesSent = Encoding.ASCII.GetBytes(request);
            Byte[] bytesReceived = new Byte[256];
            string page = "";

            // Create a socket connection with the specified server and port.
            using (Socket s = ConnectSocket(server, port))
            {

                if (s == null)
                    return ("Connection failed");

                // Send request to the server.
                s.Send(bytesSent, bytesSent.Length, 0);

                // Receive the server home page content.
                int bytes = 0;
                page = "Default HTML page on " + server + ":\r\n";

                // The following will block until the page is transmitted.
                do
                {
                    bytes = s.Receive(bytesReceived, bytesReceived.Length, 0);
                    page = page + Encoding.ASCII.GetString(bytesReceived, 0, bytes);
                }
                while (bytes > 0);
            }

            return page;
        }

        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            textBoxRcv.Text += serialPort1.ReadExisting();
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string[] portList = SerialPort.GetPortNames();
            for (int i = 0; i < portList.Length; i++)
            {
                string name = portList[i];
                comboBox1.Items.Add(name);
            }
            comboBox1.SelectedIndex = 0;
            textBox_BaudRate.Text = Settings.Default.BaudRate;
            ButtonPort.BackColor = Color.DarkBlue;
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            //send
            if (textBox_Send.Text!=null)
            {
                serialPort1.Write(textBox_Send.Text);
            }
        }

        private void ButtonPort_Click(object sender, EventArgs e)
        {
            //textBox_Send.Text = comboBox1.Text;
            if (serialPort1.IsOpen == false)
            {
                serialPort1.PortName = comboBox1.Text;
                serialPort1.BaudRate = Convert.ToInt32(textBox_BaudRate.Text);
                serialPort1.Parity = Parity.None;
                serialPort1.DataBits = 8;
                serialPort1.StopBits = StopBits.One;
                serialPort1.Handshake = Handshake.None;
                serialPort1.RtsEnable = true;
                serialPort1.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
                try
                {
                    serialPort1.Open();

                    // 打开属性变为关闭属性
                    ButtonPort.Text = "关闭串口";
                    ButtonPort.BackColor = Color.Red;
                    toolStripStatusLabel2.Text = "串口已打开";
                }
                catch (Exception)
                {
                    MessageBox.Show("串口连接失败！\r\n可能原因：串口被占用", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                serialPort1.Close();   // 关闭串口

                // 打开属性变为关闭属性
                ButtonPort.Text = "打开串口";
                ButtonPort.BackColor = Color.DarkBlue;
                toolStripStatusLabel2.Text = "串口已关闭";
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text= "时间:" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");  // 显示当前时间
        }

        private void button_web_Click(object sender, EventArgs e)
        {
            textBox_web.Text = SocketSendReceive("www.smer-c.com", 80);
        }
    }
}
