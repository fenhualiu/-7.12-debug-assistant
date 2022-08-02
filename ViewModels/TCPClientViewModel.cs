using _7._12_debug_assistant.Service;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace _7._12_debug_assistant.ViewModels
{
    public  class TCP_ClientViewModel:BindableBase
    {
        private string ipAddress;
        public string IpAddress
        {
            get { return ipAddress; }
            set { SetProperty(ref ipAddress, value); }
        }
        public Action<string> onstringrecieved;

        private bool iPEnable=true ;

        public bool IPEnable
        {
            get { return iPEnable; }
            set { SetProperty(ref iPEnable, value); }
        }
        private bool portEnable=true  ;

        public bool PortEnable
        {
            get { return portEnable; }
            set { SetProperty(ref portEnable, value); }
        }

        private string port;
        public string Port
        {
            get { return port; }
            set { SetProperty(ref port, value); }
        }
        public DelegateCommand connectCommand;
        public DelegateCommand ConnectCommand =>
             connectCommand ??= new DelegateCommand(ClientConnect);
        // public DelegateCommand<string> ConnectCommand { get; set; };
        // ConnectCommand = new DelegateCommand(ClientConnect);
        private DelegateCommand clearCommand;
        public DelegateCommand ClearCommand =>
             clearCommand ??= new DelegateCommand(Clear);
        private DelegateCommand sendCommand;
        public DelegateCommand SendCommand =>
             sendCommand ??= new DelegateCommand(ServerSendData);

        private void Clear()
        {
            ReciveData = " ";
        }

        private DelegateCommand clearCommand1;
        public DelegateCommand ClearCommand1 =>
            clearCommand1 ??= new DelegateCommand(Clear1);

        private void Clear1()
        {
            SendData = " ";
        }

        private string reciveData;
        public string ReciveData
        {
            get { return reciveData; }
            set { SetProperty(ref reciveData, value); }
        }

        private string sendData;
        public string SendData
        {
            get { return sendData; }
            set { SetProperty(ref sendData, value); }
        }
        private string  content= "⊙  连 接";

        public string   Content
        {
            get { return content; }
            set { SetProperty(ref content, value); }
        }

        private string connect= "White";
        public string Connect
        {
            get { return connect; }
            set { SetProperty(ref connect, value); }
        }

        private bool hexRT;
        public bool HexRT
        {
            get { return hexRT; }
            set { SetProperty(ref hexRT, value); }
        }
        private bool hexST;

        public bool HexST
        {
            get { return hexST; }
            set { SetProperty(ref hexST, value); }
        }
        Dictionary<string ,Socket> dicSocket= new Dictionary<string, Socket>();
        /// <summary>
        /// TCP客户端连接成功标志
        /// </summary>
        bool bClientConnetFlag = true ;
        Socket Clientsocket=null;
        Thread threadClintReceive=null;
        StringEdge[] stringEdge = new StringEdge[1] { new StringEdge() };
        public TCP_ClientViewModel()
        {
            IpAddress=GetIP();
            stringEdge[0].CurrentValue = Content;
        }

        private void ClientConnect()
        {
            try 
            {
                if(stringEdge[0].CurrentValue== "⊙  断 开" && stringEdge[0].ValueChanged)
                {
                    bClientConnetFlag=false;
                }
                else
                {
                    bClientConnetFlag=true; 
                }
                if (bClientConnetFlag)
                {
                    Clientsocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    //获取ip地址
                    IPAddress ip = IPAddress.Parse(this.IpAddress.Trim());
                    //创建端口号
                    IPEndPoint ipPoint = new IPEndPoint(ip, Convert.ToInt32(this.Port.Trim()));
                    Clientsocket.Connect(ip, Convert.ToInt32(this.Port.Trim()));
                    ReciveData = "连接成功!";
                    //bClientConnetFlag=false;
                    IPEnable = false;
                    PortEnable = false;
                    Connect = "Red";
                    Content = "⊙  断 开";
                    stringEdge[0].CurrentValue = Content;
                    Thread threadClintReceive = new Thread(new ParameterizedThreadStart(Receive));
                    //开启一个新的线程不停的接收服务器发送消息的线程
                    //threadClintReceive = new Thread(new ThreadStart(clientReceive));
                    //设置为后台线程
                    threadClintReceive.IsBackground = true;
                    threadClintReceive.Start();
                }
                else
                {
                    try
                    {
                        if (Clientsocket != null)//如果scoket 不是null才能调用colse函数
                        {
                            Clientsocket.Close();
                            Clientsocket = null;
                        }

                        // 终止线程
                        if (threadClintReceive != null)
                        {
                            threadClintReceive.Abort();
                            threadClintReceive = null;
                        }
                        //改变状态
                        IPEnable = true;
                        PortEnable = true;
                        Connect = "Black";
                        Content= "⊙  连 接";
                        stringEdge[0].CurrentValue = Content;
                        ReciveData = "连接已经断开!";
                        bClientConnetFlag = false  ;

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("断开TCP连接异常" + ex.ToString());
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("连接失败！"+ex.Message);
            }
        }

        private void Receive(object obj)
        {
            while (true)
            {

                //客户端连接成功后，服务器接收客户端发送的消息
                byte[] buffer = new byte[2048];
                int count = 0;
                try
                {
                    count = Clientsocket.Receive(buffer);//套接字接收数据，存入缓冲区
                }
                catch
                {
                    count = 0;
                }

                //实际接收到的有效字节数               
                if (count == 0)//count 表示客户端关闭，要退出循环
                {
                    //提示客户端离线"[" + socketSend.RemoteEndPoint + "]  " + 
                    string strReceiveMsg = "下线通知：" + "[" + ReciveData+ "]  " + System.DateTime.Now.ToString("f");
                    ReciveData=strReceiveMsg;
                   

                    //dicSocket.Remove(Clientsocket.RemoteEndPoint.ToString());
                    break;
                }
                else
                {
                    string str = Encoding.Default.GetString(buffer, 0, count);
                    if (HexRT == true)//16进制显示
                    {
                        string strHex = "[" + Clientsocket.RemoteEndPoint + "]  " + System.DateTime.Now.ToString("f") + "\r\n";
                        for (int i = 0; i < count; i++)
                        {
                            strHex += buffer[i].ToString("X2") + " ";
                        }
                        ReciveData += strHex;

                    }
                    else
                    {//字符串显示
                        string strReceiveMsg = "[" + Clientsocket.RemoteEndPoint + "]  " + System.DateTime.Now.ToString("f")+ "\r\n"+str;
                       // strReceiveMsg = str;
                        ReciveData += strReceiveMsg;
                        onstringrecieved?.Invoke(strReceiveMsg);
                    }
                }
            }
        }
        /// <summary>
        /// 获取本机ip地址
        /// </summary>
        /// <returns></returns>
        public string GetIP()
        {
            string hostName = Dns.GetHostName();   //获取本机名
            IPHostEntry localhost = Dns.GetHostByName(hostName);    //方法已过期，可以获取IPv4的地址
            //IPHostEntry localhost = Dns.GetHostEntry(hostName);GetHostByName   //获取IPv6地址
            IPAddress localaddr = localhost.AddressList[0];

            return localaddr.ToString();
        }

        private void ServerSendData()
        {
            if (HexST == true)
            {
                string strSend = SendData;//获取发送框的数据
                string strSend1 = strSend.Replace(" ", "");
                bool isHexa = Regex.IsMatch(strSend1, @"[A-Fa-f0-9]+$");// @"[A-Fa-f0-9]+$","^[0-9A-Fa-f]+$"
                if (isHexa == false)
                {
                    strSend = FormatConert.StringToHexString(strSend, Encoding.UTF8);
                    SendData = strSend;
                }
                string strSendWithoutNull = strSend.Trim();//回删除了string字符串首部和尾部空格的字符串
                string strSendWithoutComma = strSendWithoutNull.Replace(',', ' ');//去掉英文逗号
                string strSendWithoutComma1 = strSendWithoutComma.Replace("0x", " ");//去掉0x
                string strSendWithoutComma2 = strSendWithoutComma1.Replace("0X", " ");//去掉0X

                string[] strArray = strSendWithoutComma2.Split(' ');//以空格为基础分割字符串为字符数组
                int iStrLength = strArray.Length;//获取长度
                try
                {
                    byte[] buff = new byte[iStrLength];
                    int count = 0;
                    foreach (string item in strArray)
                    {
                        //新建字符数组
                        buff[count] = byte.Parse(item, System.Globalization.NumberStyles.HexNumber);//格式化字符串为十六进制数值
                        count++;
                    }
                    Clientsocket.Send(buff);
                }
                catch
                {
                    MessageBox.Show("请检查16进制数是否有用空格隔开", "错误");
                }
            }
            else
            {
                if (bClientConnetFlag == false)//TCP服务器未打开
                {
                    MessageBox.Show("TCP服务器未启动，请启动服务器！");
                    return;
                }
                try
                {
                    //提取需要发送字符
                    string strMsg = this.SendData.Trim();
                    byte[] buffer = System.Text.Encoding.Default.GetBytes(strMsg);
                    List<byte> list = new List<byte>(strMsg.Length + 1);
                    //list.Add(0);
                    list.AddRange(buffer);
                    //将泛型集合转换为数组
                    byte[] newBuffer = list.ToArray();
                    Clientsocket.Send(newBuffer);                  
                }
                catch (Exception ex)
                {
                    MessageBox.Show("给客户端发送消息出错:" + ex.Message);
                }
            }
        }
    }
}
