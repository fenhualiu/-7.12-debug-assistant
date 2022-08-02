using _7._12_debug_assistant.ViewModels;
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

namespace _7._12_debug_assistant.Views
{
    /// <summary>
    /// UserControl3.xaml 的交互逻辑
    /// </summary>
    public partial class TCPClient : UserControl
    {
       public TCP_ClientViewModel tCP_ClientViewModel;
        public TCPClient()
        {
            InitializeComponent();
            tCP_ClientViewModel=new TCP_ClientViewModel();
            DataContext = tCP_ClientViewModel;
        }
    }
}
