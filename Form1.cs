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
using WindowsFormsApp1.Properties;

namespace WindowsFormsApp1
{
     public partial class Form1 : Form
    {
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
    }
}
