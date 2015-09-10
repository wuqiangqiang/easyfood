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
    public partial class UcCertificateDayReportDetails : UserControl
    {
        private Dictionary<string, MyColumn> MyColumns = new Dictionary<string, MyColumn>();
        private IDBOperation dbOperation;
        private DataTable currenttable;
        private string user_flag_tier;
        public string Sj { get; set; }
        public string DeptId { get; set; }
        public string CerType { get; set; }

        public UcCertificateDayReportDetails(IDBOperation dbOperation, string sj, string deptId, string certype)
        {
            InitializeComponent();

            this.dbOperation = dbOperation;
            this.Sj = sj;
            this.DeptId = deptId;
            this.CerType = certype;
            user_flag_tier = (Application.Current.Resources["User"] as UserInfo).FlagTier;

            getdata();

            _tableview.DetailsRowEnvent += new UcTableOperableView_NoTitle.DetailsRowEventHandler(_tableview_DetailsRowEnvent);
        }

        private void getdata()
        {
            MyColumns.Add("cardid", new MyColumn("cardid", "检疫证号") { BShow = true, Width = 12 });
            MyColumns.Add("cdate", new MyColumn("cdate", "出证时间") { BShow = true, Width = 16 });
            MyColumns.Add("createdeptid", new MyColumn("createdeptid", "出证部门id") { BShow = false });
            MyColumns.Add("info_name", new MyColumn("info_name", "出证部门") { BShow = true, Width = 10 });
            MyColumns.Add("createuserid", new MyColumn("createuserid", "检疫员id") { BShow = false });
            MyColumns.Add("info_user", new MyColumn("info_user", "检疫员") { BShow = true, Width = 10 });
            MyColumns.Add("companyid", new MyColumn("companyid", "货主id") { BShow = false });
            MyColumns.Add("companyname", new MyColumn("companyname", "货主") { BShow = true, Width = 10 });
            MyColumns.Add("objectcount", new MyColumn("objectcount", "检疫头数") { BShow = true, Width = 10 });
            MyColumns.Add("type", new MyColumn("type", "检疫证类型") { BShow = true, Width = 10 });

            DataTable table = dbOperation.GetDbHelper().GetDataSet(string.Format("call p_certificate_report_day_details('{0}','{1}','{2}')",
                                Sj, DeptId,CerType)).Tables[0];

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
            DataRow[] rows = currenttable.Select("cardid = '" + id + "'");
            string type = rows[0]["type"].ToString();

            if(type == "动物证")
            {
                CertificatePreview cer = new CertificatePreview(dbOperation, id);
                cer.ShowDialog();
            }
            else if(type == "产品证")
            {
                CertificateProductPreview cer = new CertificateProductPreview(dbOperation, id);
                cer.ShowDialog();
            }

        }
    }
}
