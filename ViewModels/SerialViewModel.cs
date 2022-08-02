using _7._12_debug_assistant.Views;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using _7._12_debug_assistant.Service;
using System.Windows.Threading;
using System.Windows;
using System.Collections.ObjectModel;

namespace _7._12_debug_assistant.ViewModels
{
    public class SerialViewModel : BindableBase
    {
        private SerialPort serialPort1;
        private DelegateCommand openCommand;
        public DelegateCommand OpenCommand =>
            openCommand ??= new DelegateCommand(Open);
        private DelegateCommand closeCommand;
        public DelegateCommand CloseCommand =>
            closeCommand ??= new DelegateCommand(Close);

        private void Close()
        {
            serialPort1.Close();
        }

        private string sendTextBox;
        public string SendTextBox
        {
            get { return sendTextBox; }
            set { SetProperty(ref sendTextBox, value); }
        }

        private string reciveTextBox;
        public string ReciveTextBox
        {
            get { return reciveTextBox; }
            set { SetProperty(ref reciveTextBox, value); }
        }
        /// <summary>
        /// 串口号
        /// </summary>
        private string  comboBox;
        public string  ComboBox
        {
            get { return comboBox; }
            set { SetProperty(ref comboBox, value); }
        }
        /// <summary>
        /// 波特率
        /// </summary>
        private string baudcomboBox;
        public string BaudcomboBox
        {
            get { return baudcomboBox; }
            set { SetProperty(ref baudcomboBox, value); }
        }
        /// <summary>
        /// 校验位
        /// </summary>
        private string  cRCcomboBox;
        public string  CRCcomboBox
        {
            get { return cRCcomboBox; }
            set { SetProperty(ref cRCcomboBox, value); }
        }
        /// <summary>
        /// 数据位
        /// </summary>
        private string  datacomboBox;
        public string  DatacomboBox
        {
            get { return datacomboBox; }
            set { SetProperty(ref datacomboBox, value); }
        }
        /// <summary>
        /// 停止位
        /// </summary>
        private string  stopcomboBox;
        public string  StopcomboBox
        {
            get { return stopcomboBox; }
            set { SetProperty(ref stopcomboBox, value); }
        }
        private bool checkBox1;
        public bool CheckBox1
        {
            get { return checkBox1; }
            set { SetProperty(ref checkBox1, value); }
        }

        private bool checkBox2;
        public bool CheckBox2
        {
            get { return checkBox2; }
            set { SetProperty(ref checkBox2, value); }
        }
        private string openserial;

        public string Openserial
        {
            get { return openserial; }
            set { openserial = value; }
        }

        public bool iIsOpenFlag = true;

        private ObservableCollection<string> baudData = new ObservableCollection<string>() { "9600", "12800", "38400", "115200" };
        public ObservableCollection<string> BaudData
        {
            get { return baudData; }
            set { baudData = value; }
        }
        private ObservableCollection<string> crcData = new ObservableCollection<string>() { "NONE", "ODD", "EVEN", "MARK","SPACE" };

        public ObservableCollection<string> CRCData
        {
            get { return crcData; }
            set { crcData = value; }
        }
        private ObservableCollection<string> data = new ObservableCollection<string>() 
        { "5", "6", "7","8"};

        public ObservableCollection<string> Data
        {
            get { return data; }
            set { data = value; }
        }

        private ObservableCollection<string> stopData = new ObservableCollection<string>() 
        { "1", "1.5", "2" };

        public ObservableCollection<string> StopData
        {
            get { return stopData ; }
            set { stopData = value; }
        }

        public SerialViewModel()
        {
            //OpenCommand = new DelegateCommand<string>();
           

        }

        private void Open()
        {
            if (iIsOpenFlag)
            {
                if (serialPort1.IsOpen)//如果串口1是打开的状态，关闭串口1
                {
                    serialPort1.Close();
                }
                serialPort1.BaudRate = int.Parse(BaudcomboBox);//字符串转化为16进制 串口波特率
                serialPort1.PortName = ComboBox;//串口号
                serialPort1.DataBits = Convert.ToInt32(DatacomboBox);//串口数据位
                switch (CRCcomboBox)
                {                  //串口奇偶校验位
                    case "0":
                        serialPort1.Parity = Parity.None;
                        break;
                    case "1":
                        serialPort1.Parity = Parity.Even;
                        break;
                    case "2":
                        serialPort1.Parity = Parity.Odd;
                        break;
                    default:
                        serialPort1.Parity = Parity.None;
                        break;
                }
                switch (StopcomboBox)
                {                  //串口奇偶校验位
                    case "1":
                        serialPort1.StopBits = StopBits.One;
                        break;
                    case "0":
                        serialPort1.StopBits = StopBits.Two;
                        break;

                    default:
                        serialPort1.StopBits = StopBits.One;
                        break;
                }
                serialPort1.Open();
                if (serialPort1.IsOpen)
                {
                    iIsOpenFlag = false;
                    //ComboBox.IsEnabled = false;//失能
                    ////   BaudcomboBox.IsEnabled = false;
                    //StopcomboBox.IsEnabled = false;
                    //DatacomboBox.IsEnabled = false;
                    //CRCcomboBox.IsEnabled = false;
                    //Openserial = "Red";
                }
            }
            else
            {
                serialPort1.Close();
                if (serialPort1.IsOpen == false)
                {
                    iIsOpenFlag = true;
                    //ComboBox.IsEnabled = true;//失能
                    //                          //      BaudcomboBox.IsEnabled = true;
                    //StopcomboBox.IsEnabled = true;
                    //DatacomboBox.IsEnabled = true;
                    //CRCcomboBox.IsEnabled = true;
                    MessageBox.Show("串口关闭");
                }
            }
        }
        private string openbtnContent="打开串口";

        public string OpenbtnContent
        {
            get { return openbtnContent; }
            set
            {
                SetProperty(ref openbtnContent, value);
            }
        }


        Dispatcher Dispatcher;
        private StringBuilder builder = new StringBuilder();//避免在事件处理方法中反复的创建，定义到外面。
        private long received_count = 0;//接收计数
        /// <summary>
        /// Data Recive
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)//读取下位机的数据，显示在textBlock中
        {
            int len = this.serialPort1.BytesToRead;
            byte[] buffer = new byte[len];
            this.serialPort1.Read(buffer, 0, len);
            builder.Remove(0, builder.Length);//清除字符串构造器的内容 // string strData = BitConverter.ToString(buffer, 0, len);
                                              //Dispatcher.Invoke(() =>
                                              //{
                                              //    this.ReciveTextBox.Text += strData;
                                              //    this.ReciveTextBox.Text += " ";//字符分隔-
                                              //});                                             
            Dispatcher.Invoke((EventHandler)(delegate  //因为要访问ui资源，所以需要使用invoke方式同步ui。
            {
                if (true )//16进制显示
                {
                    //依次的拼接出16进制字符串
                    foreach (byte b in buffer)
                    {
                        SendTextBox=(b.ToString("X2") + " ");
                    }

                }
                else
                {
                    //直接按ASCII规则转换成字符串
                    builder.Append(Encoding.ASCII.GetString(buffer));
                }
                //追加的形式添加到文本框末端，并滚动到最后。
                this.ReciveTextBox=(builder.ToString());

                //修改接收计数
                //labelGetCount.Text = "Get:" + received_count.ToString();
            }));

        }

        /// <summary>
        /// 字符串转16进制字符
        /// </summary>
        public void HexConvert()
        {
            if (CheckBox1)
            {
                ReciveTextBox = FormatConert.StringToHexString(ReciveTextBox, Encoding.UTF8);
            }
            else
            {
                //byte [] bytes = Encoding.UTF8.GetBytes(ReciveTextBox);
                ReciveTextBox = FormatConert.HexStringToString(ReciveTextBox, Encoding.ASCII);
            }

        }

    }
}
