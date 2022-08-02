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
    public  class Net_portViewModel:BindableBase
    {
        Socket Serversocket=null;
        Socket Listensocket = null;

        private string ipAddress;
        public string IpAddress
        {
            get { return ipAddress; }
            set { SetProperty(ref ipAddress, value); }
        }

        private bool iPEnable=true;

        public bool IPEnable
        {
            get { return iPEnable; }
            set { SetProperty(ref iPEnable, value);  }
        }
        private bool portEnable=true;

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
        public DelegateCommand openCommand;
        public DelegateCommand OpenCommand =>
             openCommand ??= new DelegateCommand(Connect);
        //OpenCommand = new DelegateCommand(Connect);
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

        private string  reciveData;
        public string  ReciveData
        {
            get { return reciveData; }
            set { SetProperty(ref reciveData ,value); }
        }

        private string sendData;
        public string SendData
        {
            get { return sendData; }
            set { SetProperty(ref sendData, value); }
        }

        private string  content= "⊙  打 开";

        public string  Content
        {
            get { return content; }
            set { SetProperty(ref content, value); }
        }

        private string  open="White";
        public string  Open
        {
            get { return open; }
            set { SetProperty(ref open, value); }
        }
        private ObservableCollection<string> clientList;

        public ObservableCollection<string> ClientList
        {
            get { return clientList; }
            set { SetProperty(ref clientList, value); }
        }

        private bool hexRT=false  ;

        public bool HexRT
        {
            get { return hexRT; }
            set { SetProperty(ref hexRT, value); }
        }
        private bool hexST = false;

        public bool HexST
        {
            get { return hexST; }
            set { SetProperty(ref hexST, value); }
        }
        private string selectedItem;

        public string  SelectedItem
        {
            get { return selectedItem; }
            set { SetProperty (ref selectedItem ,value); }
        }

        Dictionary<string, Socket> dicSocket = new Dictionary<string, Socket>();
        //创建监听连接的线程
        Thread AcceptSocketThread;
        //接收客户端发送消息的线程
        Thread threadReceive;
        StringEdge[] Edge = new StringEdge[1] { new StringEdge() };
        public Net_portViewModel()
        {
            ClientList = new ObservableCollection<string>();
            Edge[0].CurrentValue = Content;
            IpAddress = GetIP();

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
            //    foreach (IPAddress ipa in localaddr)
            //    {
            //        if (ipa.AddressFamily == AddressFamily.InterNetwork)
                   
            //    }
            return localaddr.ToString();

        }
        /// <summary>
        /// 默认是没有监听的
        /// </summary>
        private bool bListenFlag = true;

        /// <summary>
        /// 连接
        /// </summary>
        private void Connect()
        {
            try
            { 
                if(Edge[0].CurrentValue== "⊙  关 闭" && Edge[0].ValueChanged)
                {
                    bListenFlag = false;
                }
                else
                {
                    bListenFlag = true;
                }
                if (bListenFlag)
                {
                    Listensocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    //获取ip地址
                    IPAddress ip = IPAddress.Parse(this.IpAddress.Trim());
                    //创建端口号
                    IPEndPoint ipPoint = new IPEndPoint(ip, Convert.ToInt32(this.Port.Trim()));
                    //绑定IP地址和端口号
                    Listensocket.Bind(ipPoint);
                    ReciveData = "监听成功 \r\n";
                    Open = "Red";
                    Content = "⊙  关 闭";
                    //Serversocket.RemoteEndPoint.ToString();
                    Edge[0].CurrentValue = Content;
                    Listensocket.Listen(10);
                   // bListenFlag = false ;
                    this.IPEnable = false;
                    this.PortEnable = false;
                    AcceptSocketThread = new Thread(new ParameterizedThreadStart(StartListen));
                    AcceptSocketThread.IsBackground = true;
                    AcceptSocketThread.Start(Listensocket);
                }
                else
                {
                    if (Listensocket != null)//如果scoket 不是null才能调用colse函数
                    {
                        Listensocket.Close();
                        Listensocket = null;
                    }
                    if (Serversocket != null)
                    {
                        Serversocket.Close();
                        Serversocket = null;
                    }
                    //终止线程
                    if (AcceptSocketThread != null)
                    {
                        AcceptSocketThread.Abort();
                        AcceptSocketThread = null;
                    }

                    if (threadReceive!= null)
                    {
                        threadReceive.Abort();
                        threadReceive = null;
                    }
                    this.IPEnable = true;
                    this.PortEnable = true;
                    Open = "Black";
                    Content = "⊙  打 开";
                    Edge[0].CurrentValue=Content;
                    ReciveData =("停止监听成功\r \n");
                    bListenFlag = false;
                }

            }
            catch (Exception ex)
            {
                ReciveData = "监听失败！ \r\n";
                Open = "Green";
            }
            
        }
        protected virtual void Invoke(Action action) => OnUIThread(action);

        private void OnUIThread(Action action)
        {
            try
            {
                Application.Current?.Dispatcher.BeginInvoke(action);
            }
            catch (Exception ex)
            {

            }
        }

        /// 等待客户端的连接，并且创建与之通信用的Socket
        private void StartListen(object obj)
        {
            Socket socketWatch = obj as Socket;
            while (true)
            {
                //try
                //{
                //等待客户端的连接，并且创建一个用于通信的Socket
                Serversocket = socketWatch.Accept();
                //获取远程主机的ip地址和端口号,RemoteEndPoint,LocalEndPoint
                string strIp = Serversocket.RemoteEndPoint.ToString();
                dicSocket.Add(strIp, Serversocket);
                Invoke(() =>
                {
                    ClientList.Add(strIp);
                });
               
                //this.cmb_Socket.Invoke(setCmbCallBack, strIp);

                string strMsg = "上线通知：" + "[" + Serversocket.RemoteEndPoint + "]  " + System.DateTime.Now.ToString("f");
                ReciveData = strMsg;
                //使用回调
               // TCPServerRXBox.Invoke(setCallBack, strMsg);

                //定义接收客户端消息的线程
                Thread threadReceive = new Thread(new ParameterizedThreadStart(Receive));
                threadReceive.IsBackground = true;
                threadReceive.Start(Serversocket);
                //}
                //catch
                //{
                //    MessageBox.Show("监听故障！");
                //}

            }

        }

        /// <summary>
        ///  服务器端不停的接收客户端发送的消息
        /// </summary>
        /// <param name="obj"></param>
        private void Receive(object obj)
        {
            Socket socketSend = obj as Socket;
            while (true)
            {

                //客户端连接成功后，服务器接收客户端发送的消息
                byte[] buffer = new byte[2048];
                int count = 0;
                try
                {
                    count = socketSend.Receive(buffer);
                }
                catch
                {
                    count = 0;
                }

                //实际接收到的有效字节数               
                if (count == 0)//count 表示客户端关闭，要退出循环
                {
                    //提示客户端离线"[" + socketSend.RemoteEndPoint + "]  " + System.DateTime.Now.ToString("f") + "\r\n";
                    string strReceiveMsg = "下线通知：" + "[" + socketSend.RemoteEndPoint + "]  " + System.DateTime.Now.ToString("f");
                   // ReciveData.Invoke(receiveCallBack, strReceiveMsg);
                    ReciveData= strReceiveMsg;
                    dicSocket.Remove(socketSend.RemoteEndPoint.ToString());
                    Invoke(() =>
                    {
                        ClientList.Remove(socketSend.RemoteEndPoint.ToString());
                    });
                    //cmb_Socket.Items.Remove(socketSend.RemoteEndPoint.ToString());
                   // this.cmb_Socket.Invoke(removeCmbCallBack, socketSend.RemoteEndPoint.ToString());
                    break; 
                }
                else
                {
                    string str = Encoding.Default.GetString(buffer, 0, count);
                    if (HexRT==true)//16进制显示
                    {
                        string strHex = "[" + socketSend.RemoteEndPoint + "]  " + System.DateTime.Now.ToString("f") + "\r\n";
                        for (int i = 0; i < count; i++)
                        {
                            strHex += buffer[i].ToString("X2") + " ";
                        }
                        ReciveData=strHex;

                    }
                    else
                    {//字符串显示
                        string strReceiveMsg = "[" + socketSend.RemoteEndPoint + "]  " + System.DateTime.Now.ToString("f")+str;
                       // strReceiveMsg = str;
                       ReciveData= strReceiveMsg;
                    }
                }
            }
        }

        private void ServerSendData()
        {
            if (HexST==true)
            {
                string strSend = SendData;//获取发送框的数据
                string strSend1 = strSend.Replace(" ", "");
                bool isHexa = Regex.IsMatch(strSend1, @"[A-Fa-f0-9]+$");// @"[A-Fa-f0-9]+$","^[0-9A-Fa-f]+$"
                if (isHexa == false) 
                {
                    strSend=FormatConert.StringToHexString(strSend, Encoding.UTF8);
                    SendData=strSend;
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
                        buff[count] = Convert.ToByte(item, 16);
                            //byte.Parse(item, System.Globalization.NumberStyles.HexNumber);//格式化字符串为十六进制数值
                        count++;

                    }
                    if (SelectedItem == null)
                    {
                        MessageBox.Show("请选择需要发送客户端！");
                        return;
                    }
                    string ip = SelectedItem;
                    dicSocket[ip].Send(buff);
                }
                catch
                {
                    MessageBox.Show("请检查16进制数是否有用空格隔开", "错误");
                    return;
                }
            }
            else
            {
                if (bListenFlag == false)//TCP服务器未打开
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

                    if (SelectedItem==null)
                    {
                        MessageBox.Show("请选择需要发送客户端！");
                        return;
                    }
                    //获得用户选择的IP地址
                    string ip = SelectedItem;
                    //this.cmb_Socket.SelectedValue = ip;
                    dicSocket[ip].Send(newBuffer);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("给客户端发送消息出错:" + ex.Message);
                }
            }
        }
    }
}
