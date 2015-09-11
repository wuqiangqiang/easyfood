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
using FoodSafetyMonitoring.Common;
using FoodSafetyMonitoring.dao;
using System.Data;
using FoodSafetyMonitoring.Manager.UserControls;
using Toolkit = Microsoft.Windows.Controls;

namespace FoodSafetyMonitoring.Manager
{
    /// <summary>
    /// UserControl1.xaml 的交互逻辑
    /// </summary>
    public partial class SysWarningInfo : UserControl
    {
        private IDBOperation dbOperation;
        private DataTable current_table;
        private Dictionary<string, MyColumn> MyColumns = new Dictionary<string, MyColumn>();
        private string user_flag_tier;
        private string dept_name;

        public SysWarningInfo(IDBOperation dbOperation)
        {
            InitializeComponent();

            this.dbOperation = dbOperation;
            user_flag_tier = (Application.Current.Resources["User"] as UserInfo).FlagTier;

            ////初始化查询条件
            //reportDate_kssj.SelectedDate = DateTime.Now.AddDays(-1);
            //reportDate_jssj.SelectedDate = DateTime.Now;
            //检测单位
            switch (user_flag_tier)
            {
                case "0": //_dept_name.Text = "省:";
                    dept_name = "省名称";
                    break;
                case "1": //_dept_name.Text = "市(州):";
                    dept_name = "市(州)单位名称";
                    break;
                case "2": //_dept_name.Text = "区县:";
                    dept_name = "区县名称";
                    break;
                case "3": //_dept_name.Text = "检测单位:";
                    dept_name = "检测单位名称";
                    break;
                case "4": //_dept_name.Text = "检测单位:";
                    dept_name = "检测单位名称";
                    break;
                default: break;
            }
            //ComboboxTool.InitComboboxSource(_detect_dept, "call p_dept_cxtj(" + (Application.Current.Resources["User"] as UserInfo).ID + ")", "cxtj");
            ////检测项目
            //ComboboxTool.InitComboboxSource(_detect_item, "SELECT ItemID,ItemNAME FROM t_det_item WHERE  (tradeId ='1'or tradeId ='2' or tradeId ='3' or ifnull(tradeId,'') = '') and OPENFLAG = '1' order by orderId", "cxtj");
            ////检测对象
            //ComboboxTool.InitComboboxSource(_detect_object, "SELECT objectId,objectName FROM t_det_object WHERE  (tradeId ='1'or tradeId ='2' or tradeId ='3' or ifnull(tradeId,'') = '') and OPENFLAG = '1'", "cxtj");
            ////检测结果
            //ComboboxTool.InitComboboxSource(_detect_result, "SELECT resultId,resultName FROM t_det_result where openFlag='1'");

            MyColumns.Add("zj", new MyColumn("zj", "主键") { BShow = false });
            //MyColumns.Add("districtid", new MyColumn("districtid", "区id") { BShow = false });
            //MyColumns.Add("districtname", new MyColumn("districtname", "区县") { BShow = true,Width = 10 });
            MyColumns.Add("partid", new MyColumn("partid", "检测单位id") { BShow = false });
            MyColumns.Add("partname", new MyColumn("partname", dept_name) { BShow = true, Width = 18 });
            MyColumns.Add("itemid", new MyColumn("itemid", "检测项目id") { BShow = false });
            MyColumns.Add("itemname", new MyColumn("itemname", "检测项目") { BShow = true, Width = 14 });
            MyColumns.Add("objectid", new MyColumn("objectid", "检测对象id") { BShow = false });
            MyColumns.Add("objectname", new MyColumn("objectname", "检测对象") { BShow = true, Width = 12 });
            MyColumns.Add("yang_like", new MyColumn("yang_like", "疑似阳性") { BShow = true, Width = 12 });
            MyColumns.Add("yang", new MyColumn("yang", "阳性") { BShow = true, Width = 12 });
            MyColumns.Add("count", new MyColumn("count", "合计数量") { BShow = true, Width = 12 });
            MyColumns.Add("yang_like_sum", new MyColumn("yang_like_sum", "疑似阳性合计") { BShow = false });
            MyColumns.Add("sum_num", new MyColumn("sum_num", "总行数") { BShow = false });

            _tableview.MyColumns = MyColumns;
            _tableview.BShowDetails = true;
            
            _tableview.DetailsRowEnvent += new UcTableOperableView_NoTitle.DetailsRowEventHandler(_tableview_DetailsRowEnvent);
            _tableview.GetDataByPageNumberEvent += new UcTableOperableView_NoTitle.GetDataByPageNumberEventHandler(_tableview_GetDataByPageNumberEvent);
            GetData();
        }

        //private void _query_Click(object sender, RoutedEventArgs e)
        //{
        //    if (reportDate_kssj.SelectedDate.Value.Date > reportDate_jssj.SelectedDate.Value.Date)
        //    {
        //        Toolkit.MessageBox.Show("开始时间大于结束时间，请重新选择！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
        //        return;
        //    }

        //    _tableview.GetDataByPageNumberEvent += new UcTableOperableView_NoTitle.GetDataByPageNumberEventHandler(_tableview_GetDataByPageNumberEvent);
        //    GetData();
        //    //_tableview.Title = string.Format("数据统计时间:{0}年{1}月{2}日到{3}年{4}月{5}日", reportDate_kssj.SelectedDate.Value.Year, reportDate_kssj.SelectedDate.Value.Month, reportDate_kssj.SelectedDate.Value.Day,
        //    //              reportDate_jssj.SelectedDate.Value.Year, reportDate_jssj.SelectedDate.Value.Month, reportDate_jssj.SelectedDate.Value.Day);
        //    //_title.Text = string.Format("▪ 数据统计时间:{0}年{1}月{2}日到{3}年{4}月{5}日  合计{6}条数据", reportDate_kssj.SelectedDate.Value.Year, reportDate_kssj.SelectedDate.Value.Month, reportDate_kssj.SelectedDate.Value.Day,
        //    //              reportDate_jssj.SelectedDate.Value.Year, reportDate_jssj.SelectedDate.Value.Month, reportDate_jssj.SelectedDate.Value.Day, _tableview.RowTotal);

        //    _sj.Visibility = Visibility.Visible;
        //    _hj.Visibility = Visibility.Visible;
        //    _title.Text = _tableview.RowTotal.ToString();
        //    _tableview.PageIndex = 1;

        //    if (_tableview.RowTotal == 0)
        //    {
        //        Toolkit.MessageBox.Show("没有查询到数据！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
        //        return;
        //    }
            
        //}

        private void GetData()
        {
            DataTable table = dbOperation.GetDbHelper().GetDataSet(string.Format("call p_warning_info_new({0},{1},{2})",
                              (Application.Current.Resources["User"] as UserInfo).ID,
                              (_tableview.PageIndex - 1) * _tableview.RowMax,
                              _tableview.RowMax)).Tables[0];

            _tableview.Table = table;
            current_table = table;

            string sum = "";
            if(table.Rows.Count != 0)
            {
                sum = table.Rows[0][10].ToString();
                _sj.Visibility = Visibility.Visible;
                _hj.Visibility = Visibility.Visible;
                _title.Text = sum;
            }
            
            _tableview.PageIndex = 1;

            //if (_tableview.RowTotal == 0)
            //{
            //    Toolkit.MessageBox.Show("没有查询到数据！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
            //    return;
            //}
        }

        void _tableview_GetDataByPageNumberEvent()
        {
            GetData();
        }

        void _tableview_DetailsRowEnvent(string id)
        {
            string dept_id;
            string item_id;
            string object_id;

            int selectrow = int.Parse(id);

            dept_id = current_table.Rows[selectrow - 1][1].ToString();
            item_id = current_table.Rows[selectrow - 1][3].ToString();
            object_id = current_table.Rows[selectrow - 1][5].ToString();

            if (user_flag_tier == "3" || user_flag_tier == "4")
            {
                grid_info.Children.Add(new UcWarningdetails(dbOperation, dept_id, item_id, object_id));
            }
            else if (user_flag_tier == "2")
            {
                grid_info.Children.Add(new UcWarningDept(dbOperation, dept_id, item_id, object_id));
            }
            else
            {
                grid_info.Children.Add(new UcWarningCountry(dbOperation, dept_id, item_id, object_id));
            }
        }

        private void _export_Click(object sender, RoutedEventArgs e)
        {
            DataTable table = dbOperation.GetDbHelper().GetDataSet(string.Format("call p_warning_info_new({0},{1},{2})",
                              (Application.Current.Resources["User"] as UserInfo).ID,
                              0,
                              _tableview.RowTotal)).Tables[0];

            _tableview.ExportExcel(table);
        }

    }
}
