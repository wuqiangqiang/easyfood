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
using System.Data;
using FoodSafetyMonitoring.Common;
using Toolkit = Microsoft.Windows.Controls;
using FoodSafetyMonitoring.Manager.UserControls;

namespace FoodSafetyMonitoring.Manager
{
    /// <summary>
    /// UcDetectInquire.xaml 的交互逻辑
    /// </summary>
    public partial class SysDetectReport : UserControl
    {
        DataTable ProvinceCityTable;
        public IDBOperation dbOperation = null;
        private Dictionary<string, MyColumn> MyColumns = new Dictionary<string, MyColumn>();

        string userId = (Application.Current.Resources["User"] as UserInfo).ID;

        public SysDetectReport(IDBOperation dbOperation)
        {
            InitializeComponent();
            this.dbOperation = dbOperation;
            ProvinceCityTable = Application.Current.Resources["省市表"] as DataTable;
            DataRow[] rows = ProvinceCityTable.Select("pid = '0001'");

            //画面初始化-检测单列表画面
            dtpStartDate.SelectedDate = DateTime.Now.AddDays(-1);
            dtpEndDate.SelectedDate = DateTime.Now;
            ComboboxTool.InitComboboxSource(_source_company1, string.Format(" call p_user_company('{0}','') ", userId), "cxtj");
            ComboboxTool.InitComboboxSource(_detect_station, string.Format("call p_user_dept('{0}')", userId), "cxtj");
            ComboboxTool.InitComboboxSource(_detect_person1, string.Format("call p_user_detuser('{0}')", userId), "cxtj");

            ComboboxTool.InitComboboxSource(_province1, rows, "cxtj");
            _province1.SelectionChanged += new SelectionChangedEventHandler(_province1_SelectionChanged);
            //20150707检测师改为连动（受监测站点影响）
            _detect_station.SelectionChanged += new SelectionChangedEventHandler(_detect_station_SelectionChanged);

        }

        void _province1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_province1.SelectedIndex > 0)
            {
                DataRow[] rows = ProvinceCityTable.Select("pid = '" + (_province1.SelectedItem as Label).Tag.ToString() + "'");
                ComboboxTool.InitComboboxSource(_city1, rows, "cxtj");
                //20150707被检单位改为连动（受来源产地影响）
                ComboboxTool.InitComboboxSource(_source_company1, string.Format(" call p_user_company('{0}','{1}') ", userId, (_province1.SelectedItem as Label).Tag.ToString()), "cxtj");
                _city1.SelectionChanged += new SelectionChangedEventHandler(_city1_SelectionChanged);
            }
        }


        void _city1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_city1.SelectedIndex > 0)
            {
                DataRow[] rows = ProvinceCityTable.Select("pid = '" + (_city1.SelectedItem as Label).Tag.ToString() + "'");
                ComboboxTool.InitComboboxSource(_region1, rows, "cxtj");
                //20150707被检单位改为连动（受来源产地影响）
                ComboboxTool.InitComboboxSource(_source_company1, string.Format(" call p_user_company('{0}','{1}') ", userId, (_city1.SelectedItem as Label).Tag.ToString()), "cxtj");
                _region1.SelectionChanged += new SelectionChangedEventHandler(_region1_SelectionChanged);
            }
        }

        //20150707被检单位改为连动（受来源产地影响）
        void _region1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_region1.SelectedIndex > 0)
            {
                ComboboxTool.InitComboboxSource(_source_company1, string.Format("call p_user_company('{0}','{1}')", userId, (_region1.SelectedItem as Label).Tag.ToString()), "cxtj");
            }
        }

        //20150707检测师改为连动（受检测单位影响）
        void _detect_station_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_detect_station.SelectedIndex > 0)
            {
                ComboboxTool.InitComboboxSource(_detect_person1, string.Format("SELECT RECO_PKID,INFO_USER,NUMB_USER FROM sys_client_user where fk_dept = '{0}'", (_detect_station.SelectedItem as Label).Tag.ToString()), "cxtj");
            }
            else if (_detect_station.SelectedIndex == 0)
            {
                ComboboxTool.InitComboboxSource(_detect_person1, string.Format("call p_user_detuser('{0}')", userId), "cxtj");
            }
        }

        private void _query_Click(object sender, RoutedEventArgs e)
        {
            if (dtpStartDate.SelectedDate.Value.Date > dtpEndDate.SelectedDate.Value.Date)
            {
                Toolkit.MessageBox.Show("开始日期大于结束日期，请重新选择！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            System.Data.DataTable table = GetData();

            _sj.Visibility = Visibility.Visible;
            _hj.Visibility = Visibility.Visible;
            _title.Text = table.Rows.Count.ToString();

            if (table.Rows.Count == 0)
            {
                Toolkit.MessageBox.Show("没有查询到数据！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
        }

        private System.Data.DataTable GetData()
        {
            DataTable table = dbOperation.GetDbHelper().GetDataSet(string.Format("call p_detect_report({0},'{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}')",
                   (Application.Current.Resources["User"] as UserInfo).ID,
                   ((DateTime)dtpStartDate.SelectedDate).ToShortDateString(),
                   ((DateTime)dtpEndDate.SelectedDate).ToShortDateString(),
                   _province1.SelectedIndex < 1 ? "" : (_province1.SelectedItem as Label).Tag,
                   _city1.SelectedIndex < 1 ? "" : (_city1.SelectedItem as Label).Tag,
                   _region1.SelectedIndex < 1 ? "" : (_region1.SelectedItem as Label).Tag,
                   _source_company1.SelectedIndex < 1 ? "" : (_source_company1.SelectedItem as Label).Tag,
                    _detect_station.SelectedIndex < 1 ? "" : (_detect_station.SelectedItem as Label).Tag,
                   _detect_person1.SelectedIndex < 1 ? "" : (_detect_person1.SelectedItem as Label).Tag)).Tables[0];

            return table;
        }

        private void _export_Click(object sender, RoutedEventArgs e)
        {
            
        }

    }
}
