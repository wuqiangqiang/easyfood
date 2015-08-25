using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using FoodSafetyMonitoring.dao;
using FoodSafetyMonitoring.Common;
using System.Data;
using FoodSafetyMonitoring.Manager.UserControls;
using Toolkit = Microsoft.Windows.Controls;

namespace FoodSafetyMonitoring.Manager
{
    /// <summary>
    /// SysMonthAnalysis.xaml 的交互逻辑
    /// </summary>
    public partial class SysYearAnalysis : UserControl
    {
        private IDBOperation dbOperation;
        private readonly List<string> year = new List<string>() { "2014",
            "2015", 
            "2016",
            "2017",
            "2018",
            "2019",
            "2020"};//初始化变量


        public SysYearAnalysis(IDBOperation dbOperation)
        {
            InitializeComponent();
            this.dbOperation = dbOperation;

            _year.ItemsSource = year;
            _year.SelectedIndex = 1;

            //_webBrowser.Source = new Uri(string.Format("http://www.zrodo.com:8040/ulsocialevent/getMapTesttt.do?user_id={0}", (Application.Current.Resources["User"] as UserInfo).ID));
        }

        private void _query_Click(object sender, RoutedEventArgs e)
        {

        }

        private void _export_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}