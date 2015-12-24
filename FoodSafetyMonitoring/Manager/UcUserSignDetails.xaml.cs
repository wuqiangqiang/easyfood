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
    /// UcUserSignDetails.xaml 的交互逻辑
    /// </summary>
    public partial class UcUserSignDetails : UserControl
    {
        private IDBOperation dbOperation;
        private Dictionary<string, MyColumn> MyColumns = new Dictionary<string, MyColumn>();
        public string UserId { get; set; }
        public string Kssj { get; set; }
        public string Jssj { get; set; }

        public UcUserSignDetails(IDBOperation dbOperation, string userid, string kssj, string jssj)
        {
            InitializeComponent();

            this.dbOperation = dbOperation;
            this.UserId = userid;
            this.Kssj = kssj;
            this.Jssj = jssj;

            MyColumns.Add("id", new MyColumn("id", "代码") { BShow = false });
            MyColumns.Add("username", new MyColumn("username", "检测师名称") { BShow = true, Width = 10 });
            MyColumns.Add("checkdate", new MyColumn("checkdate", "签到时间") { BShow = true, Width = 18 });
            MyColumns.Add("checkaddress", new MyColumn("checkaddress", "签到地点") { BShow = true, Width = 20 });
            MyColumns.Add("isnear", new MyColumn("isnear", "是否在附近") { BShow = true, Width = 18 });
            MyColumns.Add("checkurl", new MyColumn("checkurl", "地图") { BShow = false });
            MyColumns.Add("sum_num", new MyColumn("sum_num", "总行数") { BShow = false });

            _tableview.MyColumns = MyColumns;
            _tableview.BShowMap = true;
            _tableview.MapRowEnvent += new UcTableOperableView_NoTitle.MapRowEventHandler(_tableview_MapRowEnvent);
            _tableview.GetDataByPageNumberEvent += new UcTableOperableView_NoTitle.GetDataByPageNumberEventHandler(_tableview_GetDataByPageNumberEvent);
            _tableview.PageIndex = 1;
            GetData();
        }

        private void GetData()
        {
            DataTable table = dbOperation.GetDbHelper().GetDataSet(string.Format("call p_user_sign_details('{0}','{1}','{2}',{3},{4})",
                                Kssj, Jssj, UserId,
                              (_tableview.PageIndex - 1) * _tableview.RowMax,
                              _tableview.RowMax)).Tables[0];

            _tableview.Table = table;
        }

        void _tableview_GetDataByPageNumberEvent()
        {
            GetData();
        }

        void _tableview_MapRowEnvent(string id)
        {
            int orderid = int.Parse(id);
            userSignMap map = new userSignMap(dbOperation, orderid);
            map.ShowDialog();
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Hidden;
        }
    }
}