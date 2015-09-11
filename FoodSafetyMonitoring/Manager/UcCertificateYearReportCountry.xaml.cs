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
    /// UcCertificateDayReportCountry.xaml 的交互逻辑
    /// </summary>
    public partial class UcCertificateYearReportCountry : UserControl
    {
        private Dictionary<string, MyColumn> MyColumns = new Dictionary<string, MyColumn>();
        private IDBOperation dbOperation;
        private DataTable currenttable;
        private string user_flag_tier;
        public string Kssj { get; set; }
        public string Jssj { get; set; }
        public string DeptId { get; set; }
        public string CerType { get; set; }


        public UcCertificateYearReportCountry(IDBOperation dbOperation, string kssj, string jssj, string deptId, string certype)
        {
            InitializeComponent();

            this.dbOperation = dbOperation;
            this.Kssj = kssj;
            this.Jssj = jssj;
            this.DeptId = deptId;
            this.CerType = certype;
            user_flag_tier = (Application.Current.Resources["User"] as UserInfo).FlagTier;

            getdata();

            _tableview.DetailsRowEnvent += new UcTableOperableView_NoPages.DetailsRowEventHandler(_tableview_DetailsRowEnvent);
        }

        private void getdata()
        {
            MyColumns.Add("part_id", new MyColumn("part_id", "部门id") { BShow = false });
            MyColumns.Add("part_name", new MyColumn("part_name", "区县名称") { BShow = true, Width = 16 });
            switch (CerType)
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

            DataTable table = dbOperation.GetDbHelper().GetDataSet(string.Format("call p_certificate_report_year_country('{0}','{1},'{2}'')",
                                Kssj,Jssj, DeptId)).Tables[0];

            currenttable = table;

            _tableview.MyColumns = MyColumns;
            _tableview.BShowDetails = true;
            _tableview.Table = table;
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Hidden;
        }

        void _tableview_DetailsRowEnvent(string id)
        {
            string dept_id;

            DataRow[] rows = currenttable.Select("PART_NAME = '" + id + "'");
            dept_id = rows[0]["PART_ID"].ToString();

            if (user_flag_tier == "2")
            {
                UcCertificateYearReportDetails daydetails = new UcCertificateYearReportDetails(dbOperation, Kssj, Jssj, dept_id, CerType);
                daydetails.SetValue(Grid.RowProperty, 0);
                daydetails.SetValue(Grid.RowSpanProperty, 2);

                grid_info.Children.Add(daydetails);
            }
            else
            {
                UcCertificateYearReportDept daydetails = new UcCertificateYearReportDept(dbOperation, Kssj, Jssj, dept_id, CerType);
                daydetails.SetValue(Grid.RowProperty, 0);
                daydetails.SetValue(Grid.RowSpanProperty, 2);

                grid_info.Children.Add(daydetails);
            }


        }
    }
}
