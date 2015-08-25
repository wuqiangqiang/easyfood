﻿using System;
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
    public partial class SysQuarterAnalysis : UserControl
    {
        private IDBOperation dbOperation;
        private string page_url;
        private readonly List<string> year = new List<string>() { "2014",
            "2015", 
            "2016",
            "2017",
            "2018",
            "2019",
            "2020"};//初始化变量

        private readonly List<string> month = new List<string>() { 
            "第一季度", 
            "第二季度",
            "第三季度",
            "第四季度"};//初始化变量

        public SysQuarterAnalysis(IDBOperation dbOperation)
        {
            InitializeComponent();
            this.dbOperation = dbOperation;

            _year.ItemsSource = year;
            _year.SelectedIndex = 1;

            _month.ItemsSource = month;
            _month.SelectedIndex = 2;

            //地址从数据库中获取
            page_url = dbOperation.GetDbHelper().GetSingle("select quarterreport from t_url ").ToString();
            if (page_url == null)
            {
                page_url = "";
            }
        }

        private void _query_Click(object sender, RoutedEventArgs e)
        {
            if (page_url != "")
            {
                _webBrowser.Source = new Uri(string.Format(page_url));
            }
        }

        private void _export_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}