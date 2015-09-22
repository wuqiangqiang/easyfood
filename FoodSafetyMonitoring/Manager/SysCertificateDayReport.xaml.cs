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
using System.Windows.Forms.Integration;
using System.Data;
using FoodSafetyMonitoring.Common;
using FoodSafetyMonitoring.Manager.UserControls;
using Toolkit = Microsoft.Windows.Controls;

namespace FoodSafetyMonitoring.Manager
{
    /// <summary>
    /// SysCertificateDayReport.xaml 的交互逻辑
    /// </summary>
    public partial class SysCertificateDayReport : UserControl
    {
        private IDBOperation dbOperation;
        private string user_flag_tier;
        private string user_id;
        private string dept_id;
        private Dictionary<string, MyColumn> MyColumns = new Dictionary<string, MyColumn>();
        private DateTime report_day;
        private string cer_type;
        private string dept_name;
        private DataTable currenttable;

        public SysCertificateDayReport(IDBOperation dbOperation)
        {
            InitializeComponent();

            this.dbOperation = dbOperation;
            user_flag_tier = (Application.Current.Resources["User"] as UserInfo).FlagTier;
            user_id = (Application.Current.Resources["User"] as UserInfo).ID;
            dept_id = (Application.Current.Resources["User"] as UserInfo).DepartmentID;

            reportDate.SelectedDate = DateTime.Now;
            switch (user_flag_tier)
            {
                case "0": _dept_name.Text = "选择省:";
                    dept_name = "省名称";
                    break;
                case "1": _dept_name.Text = "选择市(州):";
                    dept_name = "市(州)单位名称";
                    break;
                case "2": _dept_name.Text = "选择区县:";
                    dept_name = "区县名称";
                    break;
                case "3": _dept_name.Text = "选择检测单位:";
                    dept_name = "检测单位名称";
                    break;
                case "4": _dept_name.Text = "选择检测单位:";
                    dept_name = "检测单位名称";
                    break;
                default: break;
            }

            //检测单位
            ComboboxTool.InitComboboxSource(_detect_dept, "call p_dept_cxtj(" + user_id + ")", "cxtj");
            //电子证类型
            DataTable dt_type = new DataTable();
            dt_type.Columns.Add(new DataColumn("typeid"));
            dt_type.Columns.Add(new DataColumn("typename"));
            var row = dt_type.NewRow();
            row["typeid"] = "0";
            row["typename"] = "动物证";
            dt_type.Rows.Add(row);
            var row2 = dt_type.NewRow();
            row2["typeid"] = "1";
            row2["typename"] = "产品证";
            dt_type.Rows.Add(row2);

            ComboboxTool.InitComboboxSource(_cer_type, dt_type, "cxtj");

            //如果登录用户的部门是站点级别，则将查询条件检测单位赋上默认值
            if (user_flag_tier == "4")
            {
                _detect_dept.SelectedIndex = 1;
            }

            _tableview.DetailsRowEnvent += new UcTableOperableView_NoPages.DetailsRowEventHandler(_tableview_DetailsRowEnvent);
        }

        private void _query_Click(object sender, RoutedEventArgs e)
        {
            report_day = (DateTime)reportDate.SelectedDate;
            cer_type = _cer_type.SelectedIndex < 1 ? "" : (_cer_type.SelectedItem as Label).Tag.ToString();

            grid_info.Children.Clear();
            grid_info.Children.Add(_tableview);
            MyColumns.Clear();
            MyColumns.Add("part_id", new MyColumn("part_id", "部门id") { BShow = false });
            MyColumns.Add("part_name", new MyColumn("part_name", dept_name) { BShow = true, Width = 16 });
            switch (cer_type)
            {
                case "": MyColumns.Add("animal", new MyColumn("animal", "动物证") { BShow = true, Width = 10 });
                    MyColumns.Add("animal_num", new MyColumn("animal_num", "动物检疫头数") { BShow = true, Width = 10 });
                    MyColumns.Add("product", new MyColumn("product", "产品证") { BShow = true, Width = 10 });
                    MyColumns.Add("product_num", new MyColumn("product_num", "产品检疫头数") { BShow = true, Width = 10 });
                    MyColumns.Add("sum_cer", new MyColumn("sum_cer", "检疫证合计") { BShow = true, Width = 10 });
                    MyColumns.Add("sum_num", new MyColumn("sum_num", "检疫头数合计") { BShow = true, Width = 10 });
                    break;
                case "0": MyColumns.Add("animal", new MyColumn("animal", "动物证") { BShow = true, Width = 10 });
                    MyColumns.Add("animal_num", new MyColumn("animal_num", "动物检疫头数") { BShow = true, Width = 10 });
                    MyColumns.Add("product", new MyColumn("product", "产品证") { BShow = false });
                    MyColumns.Add("product_num", new MyColumn("product_num", "产品检疫头数") { BShow = false });
                    MyColumns.Add("sum_cer", new MyColumn("sum_cer", "检疫证合计") { BShow = false });
                    MyColumns.Add("sum_num", new MyColumn("sum_num", "检疫头数合计") { BShow = false });
                    break;
                case "1": MyColumns.Add("animal", new MyColumn("animal", "动物证") { BShow = false });
                    MyColumns.Add("animal_num", new MyColumn("animal_num", "动物检疫头数") { BShow = false });
                    MyColumns.Add("product", new MyColumn("product", "产品证") { BShow = true, Width = 10 });
                    MyColumns.Add("product_num", new MyColumn("product_num", "产品检疫头数") { BShow = true, Width = 10 });
                    MyColumns.Add("sum_cer", new MyColumn("sum_cer", "检疫证合计") { BShow = false });
                    MyColumns.Add("sum_num", new MyColumn("sum_num", "检疫头数合计") { BShow = false });
                    break;
                default: break;
            }

            DataTable table = dbOperation.GetDbHelper().GetDataSet(string.Format("call p_certificate_report_day('{0}','{1}','{2}')",
                               dept_id, report_day,
                               _detect_dept.SelectedIndex < 1 ? "" : (_detect_dept.SelectedItem as Label).Tag)).Tables[0];

            currenttable = table;
            _tableview.MyColumns = MyColumns;
            _tableview.BShowDetails = true;
            _tableview.Table = table;

            if (table.Rows.Count == 0)
            {
                Toolkit.MessageBox.Show("没有查询到数据！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

        }

        void _tableview_DetailsRowEnvent(string id)
        {
            string dept_id;

            DataRow[] rows = currenttable.Select("PART_NAME = '" + id + "'");
            dept_id = rows[0]["PART_ID"].ToString();

            if (user_flag_tier == "3" || user_flag_tier == "4")
            {
                grid_info.Children.Add(new UcCertificateDayReportDetails(dbOperation, report_day.ToString(), dept_id, cer_type));
            }
            else if (user_flag_tier == "2")
            {
                grid_info.Children.Add(new UcCertificateDayReportDept(dbOperation, report_day.ToString(), dept_id, cer_type));
            }
            else
            {
                grid_info.Children.Add(new UcCertificateDayReportCountry(dbOperation, report_day.ToString(), dept_id, cer_type));
            }
        }

        private void _export_Click(object sender, RoutedEventArgs e)
        {
            _tableview.ExportExcel();
        }
    }
}
