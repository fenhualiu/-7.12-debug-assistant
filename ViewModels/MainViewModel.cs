using _7._12_debug_assistant.Model;
using _7._12_debug_assistant.Views;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;

namespace _7._12_debug_assistant.ViewModel
{
    public  class MainViewModel : BindableBase
    {
        public DelegateCommand<string> OpenCommand { get; set; }//用来打开各种模块
        private IRegionManager regionManaer;
        public MainViewModel(IRegionManager regionManaer)
        {
            OpenCommand = new DelegateCommand<string>(Open);

        }

        private Object body;

        public Object Body
        {
            get { return body; }
            set { body = value; RaisePropertyChanged(); }
        }

        private void Open(string obj)
        {
            switch (obj)
            {
                case "Serial": Body = new Serial(); break;
                case "Net-port": Body = new Netport(); break;
                case "UserControl3": Body = new UserControl3(); break;
            }
            //NavigationWindow wds = new NavigationWindow();
            //wds.Source = new Uri("Views.UserControl1.xaml", UriKind.Relative);
            //wds.Show();
        }
        //public void Open(string obj)
        //{
        //    regionManaer.Regions["ContentRegion"].RequestNavigate(obj);
        //}

        public DelegateCommand<MenuBar> NavigateCommand { get; private set; }
        //动态菜单，创建一个动态属性集合,而后在构造函数实例化


    }
}
