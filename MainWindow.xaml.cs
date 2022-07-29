using _7._12_debug_assistant.ViewModel;
using Prism.Regions;
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

namespace _7._12_debug_assistant
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
      public   MainViewModel mainViewModel;
        //public IRegionManager regionManager;
        public MainWindow(IRegionManager regionManager)
        {
            InitializeComponent();
            mainViewModel=new MainViewModel(regionManager);
            this.DataContext=mainViewModel;
            BtnMin.Click += (s, e) =>
            {
                this.WindowState = WindowState.Minimized;
            };
            //最小化按钮事件

            ColorZone.MouseMove += (s, e) =>
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                    this.DragMove();
            };
            //点击顶部拖拽

            BtnMax.Click += (s, e) =>
            {//判断是否以及最大化，最大化就还原窗口，否则最大化
                if (this.WindowState == WindowState.Maximized)
                    this.WindowState = WindowState.Normal;
                //还原按钮事件
                else
                    this.WindowState = WindowState.Maximized;
            };

            BtnClose.Click += (s, e) =>
            {
                this.Close();
            };
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //llll.Content = mainViewModel.Body;
        }
    }
}
