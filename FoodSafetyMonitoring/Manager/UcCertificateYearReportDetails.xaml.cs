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
    /// UcCertificateDayReportDetails.xaml 的交互逻辑
    /// </summary>
    public partial class UcCertificateYearReportDetails : UserControl
    {
        private Dictionary<string, MyColumn> MyColumns = new Dictionary<string, MyColumn>();
        private IDBOperation dbOperation;
        private DataTable currenttable;
        private string user_flag_tier;
        public string Kssj { get; set; }
        public string Jssj { get; set; }
        public string DeptId { get; set; }
        public string CerType { get; set; }

        public UcCertificateYearReportDetails(IDBOperation dbOperation, string kssj, string jssj, string deptId, string certype)
        {
            InitializeComponent();

            this.dbOperation = dbOperation;
            this.Kssj = kssj;
            this.Jssj = jssj;
            this.DeptId = deptId;
            this.CerType = certype;
            user_flag_tier = (Application.Current.Resources["User"] as UserInfo).FlagTier;

            MyColumns.Add("cardid", new MyColumn("cardid", "检疫证号") { BShow = true, Width = 12 });
            MyColumns.Add("cdate", new MyColumn("cdate", "出证时间") { BShow = true, Width = 16 });
            MyColumns.Add("createdeptid", new MyColumn("createdeptid", "出证部门id") { BShow = false });
            MyColumns.Add("info_name", new MyColumn("info_name", "出证部门") { BShow = true, Width = 16 });
            MyColumns.Add("createuserid", new MyColumn("createuserid", "检疫员id") { BShow = false });
            MyColumns.Add("info_user", new MyColumn("info_user", "检疫员") { BShow = true, Width = 12 });
            MyColumns.Add("companyid", new MyColumn("companyid", "货主id") { BShow = false });
            MyColumns.Add("companyname", new MyColumn("companyname", "货主") { BShow = true, Width = 12 });
            MyColumns.Add("objectcount", new MyColumn("objectcount", "检疫头数") { BShow = true, Width = 12 });
            MyColumns.Add("type", new MyColumn("type", "检疫证类型") { BShow = true, Width = 12 });
            MyColumns.Add("sum_num", new MyColumn("sum_num", "总行数") { BShow = false });

            _tableview.MyColumns = MyColumns;
            _tableview.BShowDetails = true;
            _tableview.GetDataByPageNumberEvent += new UcTableOperableView_NoTitle.GetDataByPageNumberEventHandler(_tableview_GetDataByPageNumberEvent);
            _tableview.DetailsRowEnvent += new UcTableOperableView_NoTitle.DetailsRowEventHandler(_tableview_DetailsRowEnvent);
            _tableview.PageIndex = 1;

            getdata();
        }

        private void getdata()
        {

            DataTable table = dbOperation.GetDbHelper().GetDataSet(string.Format("call p_certificate_report_year_details('{0}','{1}','{2}','{3}',{4},{5})",
                                Kssj, Jssj, DeptId, CerType, (_tableview.PageIndex - 1) * _tableview.RowMax,
                              _tableview.RowMax)).Tables[0];

            currenttable = table;
            _tableview.Table = table;
            _sj.Visibility = Visibility.Visible;
            _hj.Visibility = Visibility.Visible;
            _title.Text = _tableview.RowTotal.ToString();
        }

        void _tableview_GetDataByPageNumberEvent()
        {
            getdata();
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Hidden;
        }

        void _tableview_DetailsRowEnvent(string id)
        {
            DataRow[] rows = currenttable.Select("cardid = '" + id + "'");
            string type = rows[0]["type"].ToString();

            if (type == "动物证")
            {
                CertificatePreview cer = new CertificatePreview(dbOperation, id);
                cer.ShowDialog();
            }
            else if (type == "产品证")
            {
                CertificateProductPreview cer = new CertificateProductPreview(dbOperation, id);
                cer.ShowDialog();
            }

        }
    }
}
