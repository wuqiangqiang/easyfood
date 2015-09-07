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
using FoodSafetyMonitoring.Manager.UserControls;
using Toolkit = Microsoft.Windows.Controls;

namespace FoodSafetyMonitoring.Manager
{
    /// <summary>
    /// SysShipperQuery.xaml 的交互逻辑
    /// </summary>
    public partial class SysShipperQuery_product : UserControl
    {
        private IDBOperation dbOperation;
        private string deptId;
        private Dictionary<string, MyColumn> MyColumns = new Dictionary<string, MyColumn>();
        private string shipperflag;

        public SysShipperQuery_product(IDBOperation dbOperation)
        {
            InitializeComponent();

            this.dbOperation = dbOperation;
            deptId = (Application.Current.Resources["User"] as UserInfo).DepartmentID;

            shipperflag = dbOperation.GetDbHelper().GetSingle("select ifnull(a.shipperflag,'') as shipperflag " +
                                    " from sys_client_sysdept a " +
                                    " where INFO_CODE = " + deptId).ToString();
        }


        private void _query_Click(object sender, RoutedEventArgs e)
        {
            //string dept = deptId.Substring(0,5);
            DataTable table;
            if (_shipper_id.Text.Trim().Length == 0)
            {
                table = dbOperation.GetDbHelper().GetDataSet(string.Format("select shipperid,shippername,phone,address from t_shipper_product " +
                               "where shipperflag = '{0}'", shipperflag)).Tables[0];
            }
            else
            {
                table = dbOperation.GetDbHelper().GetDataSet(string.Format("select shipperid,shippername,phone,address from t_shipper_product " +
                               "where shipperid = '{0}' and shipperflag = '{1}'", _shipper_id.Text, shipperflag)).Tables[0];
            }

            lvlist.DataContext = table;

            _sj.Visibility = Visibility.Visible;
            _hj.Visibility = Visibility.Visible;
            _title.Text = table.Rows.Count.ToString();

            if (table.Rows.Count == 0)
            {
                Toolkit.MessageBox.Show("没有查询到数据！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
        }

        public void refresh()
        {
            DataTable table;
            if (_shipper_id.Text.Trim().Length == 0)
            {
                table = dbOperation.GetDbHelper().GetDataSet(string.Format("select shipperid,shippername,phone,address from t_shipper_product " +
                              "where shipperflag = '{0}'", shipperflag)).Tables[0];
            }
            else
            {
                table = dbOperation.GetDbHelper().GetDataSet(string.Format("select shipperid,shippername,phone,address from t_shipper_product " +
                               "where shipperid = '{0}' and shipperflag = '{1}'", _shipper_id.Text, shipperflag)).Tables[0];
            }

            lvlist.DataContext = table;
        }

        private void _btn_modify_Click(object sender, RoutedEventArgs e)
        {
            string shipper_id = (sender as Button).Tag.ToString();
            ModifyShipper_product ship = new ModifyShipper_product(dbOperation, shipper_id,shipperflag, this);
            ship.ShowDialog();
        }
    }
}
