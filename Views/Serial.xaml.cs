using _7._12_debug_assistant.ViewModels;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace _7._12_debug_assistant.Views
{
    /// <summary>
    /// UserControl1.xaml 的交互逻辑
    /// </summary>
    public partial class Serial : UserControl
    {
        public SerialViewModel serialViewModel;
        public  SerialPort serialPort1=new SerialPort();
        public bool iIsOpenFlag = true;
        public Serial()
        {
            InitializeComponent();
            serialViewModel=new SerialViewModel();
            this .DataContext = serialViewModel;
            serialPort1.Close();
            string[] ports = SerialPort.GetPortNames();//获取已有的串口数目
            Array.Sort(ports);//自动排列顺序
            ComboBox.Items.Add(ports);//添加串口
            ComboBox.SelectedIndex = ComboBox.Items.Count > 0 ? 0 : -1;//判断串口数是否大于0

        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ReciveTextBox.Text = "";
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            SendTextBox.Text = "";
        }
        /// <summary>
        /// 打开串口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (iIsOpenFlag)
            {
                try
                {
                    if (serialPort1.IsOpen)//如果串口1是打开的状态，关闭串口1
                    {
                        serialPort1.Close();
                    }
                    serialPort1.BaudRate = Convert.ToInt32(BaudcomboBox.Text);//字符串转化为16进制 串口波特率
                    serialPort1.PortName = ComboBox.Text;//串口号
                    serialPort1.DataBits = Convert.ToInt32(DatacomboBox.Text);//串口数据位
                    switch (CRCcomboBox.SelectedIndex)
                    {                  //串口奇偶校验位
                        case 0:
                            serialPort1.Parity = Parity.None;
                            break;
                        case 1:
                            serialPort1.Parity = Parity.Even;
                            break;
                        case 2:
                            serialPort1.Parity = Parity.Odd;
                            break;
                        default:
                            serialPort1.Parity = Parity.Mark;
                            break;
                    }
                    switch (StopcomboBox.SelectedIndex)
                    {                  //串口奇偶校验位
                        case 1:
                            serialPort1.StopBits = StopBits.One;
                            break;
                        case 0:
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
                        ComboBox.IsEnabled = false;//失能
                        BaudcomboBox.IsEnabled = false;
                        StopcomboBox.IsEnabled = false;
                        DatacomboBox.IsEnabled = false;
                        CRCcomboBox.IsEnabled = false;
                        Openserial.Background = Brushes.Red;
                        Openserial.Content = "关闭串口";
                    }
                    serialPort1.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);//添加数据接收事件
                }
                catch
                {
                    MessageBox.Show("请打开串口！");
                }              
            }
            else
            {
                serialPort1.Close();
                if (serialPort1.IsOpen == false)
                {
                    iIsOpenFlag = true;
                    ComboBox.IsEnabled = true;//失能
                    BaudcomboBox.IsEnabled = true;
                    StopcomboBox.IsEnabled = true;
                    DatacomboBox.IsEnabled = true;
                    CRCcomboBox.IsEnabled = true;
                    Openserial.Content = "打开串口";
                }
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
            builder.Remove(0, builder.Length);//清除字符串构造器的内容
           // string strData = BitConverter.ToString(buffer, 0, len);
            //Dispatcher.Invoke(() =>
            //{
            //    this.ReciveTextBox.Text += strData;
            //    this.ReciveTextBox.Text += " ";//字符分隔-
            //});
            Dispatcher.Invoke((EventHandler)(delegate  //因为要访问ui资源，所以需要使用invoke方式同步ui。
            {
                if ((bool)(checkHexRX.IsChecked))//16进制显示
                {
                    //依次的拼接出16进制字符串
                    foreach (byte b in buffer)
                    {
                        SendTextBox.AppendText(b.ToString("X2") + " ");
                    }

                }
                else
                {
                    //直接按ASCII规则转换成字符串
                    builder.Append(Encoding.ASCII.GetString(buffer));
                }
                //追加的形式添加到文本框末端，并滚动到最后。
                this.ReciveTextBox.AppendText(builder.ToString());

                //修改接收计数
                //labelGetCount.Text = "Get:" + received_count.ToString();
            }));

        }

        private void HandTX_Click(object sender, RoutedEventArgs e)
        {
            if (iIsOpenFlag == true)//串口未打开
            {
                MessageBox.Show("请打开串口！", "错误");
                return;
            }

            if ((bool)checkHexTx.IsChecked)//16进制发送
            {
                string strSend = SendTextBox.Text;//获取发送框的数据
                string strSendWithoutNull = strSend.Trim();//回删除了string字符串首部和尾部空格的字符串
                string strSendWithoutComma = strSendWithoutNull.Replace(',', ' ');//去掉英文逗号
                string strSendWithoutComma1 = strSendWithoutComma.Replace("0x", " ");//去掉0x
                string strSendWithoutComma2 = strSendWithoutComma1.Replace("0X", " ");//去掉0X
                //string strSendWithoutComma2 = strSendWithoutComma1.Replace(" ", "");//去掉字符串中的空格

                string[] strArray = strSendWithoutComma2.Split(' ');//以空格为基础分割字符串为字符数组
                int iStrLength = strArray.Length;//获取长度
                try
                {
                    foreach (string item in strArray)
                    {
                        int count = 1;
                        byte[] buff = new byte[count];  //新建字符数组
                        buff[0] = byte.Parse(item, System.Globalization.NumberStyles.HexNumber);//格式化字符串为十六进制数值  
                        serialPort1.Write(buff, 0, count);
                    }
                }
                catch
                {
                    MessageBox.Show("请输入正确的16进制数", "错误");
                }


            }
            else//字符串发送
            {
                if (iIsOpenFlag == true)//串口未打开
                {
                    MessageBox.Show("请打开串口！", "错误");
                    return;
                }

                try
                {
                    serialPort1.WriteLine(SendTextBox.Text);
                }
                catch
                {
                    MessageBox.Show("发送失败！", "错误");

                }

            }
        }
        public Timer timer = new Timer();
       // public bool IsStart=false;
        private void AutoSend_Click(object sender, RoutedEventArgs e)
        {            
            if(timer.Enabled == false)
            {
                try
                {
                    timer.Enabled = true;//打开定时器
                    timer.Interval = double.Parse(Intervals.Text);
                    timer.Elapsed += Timer_Elapsed;
                    AutoSend.Content = "停止发送";
                }
                catch
                {
                    MessageBox.Show("请检查时间输入格式");
                }               
            }
            else
            {
                try
                {
                    timer.Enabled = false;
                    AutoSend.Content = "自动发送";
                }
                catch
                {
                    MessageBox.Show("关闭计时器失败！");
                }
            }
           
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            serialPort1.WriteLine(SendTextBox.Text);
        }
    }
}
