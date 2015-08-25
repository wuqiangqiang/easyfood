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
using System.Data;
using FoodSafetyMonitoring.dao;
using FoodSafetyMonitoring.Manager.UserControls;

namespace FoodSafetyMonitoring.Manager
{
    /// <summary>
    /// UcWarningReportCountry.xaml 的交互逻辑
    /// </summary>
    public partial class UcWarningReportDept : UserControl
    {
        private IDBOperation dbOperation;
        private string user_flag_tier;
        private Dictionary<string, MyColumn> MyColumns = new Dictionary<string, MyColumn>();
        public string Kssj { get; set; }
        public string Jssj { get; set; }
        public string DeptId { get; set; }
        public string ItemId { get; set; }
        public string ReviewFlag { get; set; }

        public UcWarningReportDept(IDBOperation dbOperation, string kssj, string jssj, string dept_id, string item_id, string review_id)
        {
            InitializeComponent();

            this.dbOperation = dbOperation;
            this.Kssj = kssj;
            this.Jssj = jssj;
            this.DeptId = dept_id;
            this.ItemId = item_id;
            this.ReviewFlag = review_id;
            user_flag_tier = (Application.Current.Resources["User"] as UserInfo).FlagTier;

            MyColumns.Add("zj", new MyColumn("zj", "序号") { BShow = false, Width = 5 });
            MyColumns.Add("partid", new MyColumn("partid", "检测单位id") { BShow = false });
            MyColumns.Add("partname", new MyColumn("partname", "检测单位") { BShow = true, Width = 18 });
            MyColumns.Add("yang", new MyColumn("yang", "阳性预警数") { BShow = true, Width = 12 });
            MyColumns.Add("yang_like", new MyColumn("yang_like", "疑似阳性预警数") { BShow = true, Width = 14 });
            MyColumns.Add("count", new MyColumn("count", "预警数合计") { BShow = true, Width = 12 });
            MyColumns.Add("sum_num", new MyColumn("sum_num", "总行数") { BShow = false });

            switch (ReviewFlag)
            {
                case "0": MyColumns.Add("review_yes", new MyColumn("review_yes", "已复核数") { BShow = false, Width = 12 });
                    MyColumns.Add("review_no", new MyColumn("review_no", "未复核数") { BShow = true, Width = 12 });
                    break;
                case "1": MyColumns.Add("review_yes", new MyColumn("review_yes", "已复核数") { BShow = true, Width = 12 });
                    MyColumns.Add("review_no", new MyColumn("review_no", "未复核数") { BShow = false, Width = 12 });
                    break;
                case "": MyColumns.Add("review_yes", new MyColumn("review_yes", "已复核数") { BShow = true, Width = 12 });
                    MyColumns.Add("review_no", new MyColumn("review_no", "未复核数") { BShow = true, Width = 12 });
                    break;
                default: break;
            }

            _tableview.MyColumns = MyColumns;
            _tableview.BShowDetails = true;

            _tableview.DetailsRowEnvent += new UcTableOperableView_NoPages.DetailsRowEventHandler(_tableview_DetailsRowEnvent);
            GetData();
        }

        private void GetData()
        {
            DataTable table = dbOperation.GetDbHelper().GetDataSet(string.Format("call p_warning_report_country('{0}','{1}','{2}','{3}','{4}')",
                               Kssj, Jssj, DeptId, ItemId, ReviewFlag)).Tables[0];

            _tableview.Table = table;
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Hidden;
        }

        void _tableview_DetailsRowEnvent(string id)
        {
            string dept_id;

            dept_id = id;


            UcWarningReportDetails daydetails = new UcWarningReportDetails(dbOperation, Kssj, Jssj, dept_id, ItemId, ReviewFlag);
            daydetails.SetValue(Grid.RowProperty, 0);
            daydetails.SetValue(Grid.RowSpanProperty, 2);

            grid_info.Children.Add(daydetails);
            
        }
    }
}
