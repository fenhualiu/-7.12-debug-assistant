﻿using _7._12_debug_assistant.ViewModels;
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
    /// UserControl2.xaml 的交互逻辑
    /// </summary>
    public partial class Netport : UserControl
    {
        public Net_portViewModel net_PortViewModel;
        public Netport()
        {
            InitializeComponent();
            net_PortViewModel=new Net_portViewModel();
            DataContext = net_PortViewModel;
        }
    }
}
