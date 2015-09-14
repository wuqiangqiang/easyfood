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
        private string page_url;
        private string user_id;
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
            user_id = (Application.Current.Resources["User"] as UserInfo).ID.ToString();

            _year.ItemsSource = year;
            _year.SelectedIndex = 1;

            //地址从数据库中获取
            page_url = dbOperation.GetDbHelper().GetSingle("select yearreport from t_url ").ToString();
            if (page_url == null)
            {
                page_url = "";
            }
        }

        private void _query_Click(object sender, RoutedEventArgs e)
        {
            if (page_url != "")
            {
                _webBrowser.Source = new Uri(string.Format(page_url, user_id, "1", _year.Text));
            }
        }

        private void _export_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}