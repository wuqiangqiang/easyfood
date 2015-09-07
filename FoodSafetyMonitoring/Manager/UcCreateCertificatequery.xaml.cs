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
    /// UcCreateCertificatequery.xaml 的交互逻辑
    /// </summary>
    public partial class UcCreateCertificatequery : UserControl
    {
        public IDBOperation dbOperation = null;
        private Dictionary<string, MyColumn> MyColumns = new Dictionary<string, MyColumn>();

        string userId = (Application.Current.Resources["User"] as UserInfo).ID;
        string deptId = (Application.Current.Resources["User"] as UserInfo).DepartmentID;

        public UcCreateCertificatequery(IDBOperation dbOperation)
        {
            InitializeComponent();
            this.dbOperation = dbOperation;

            //ComboboxTool.InitComboboxSource(_source_company, string.Format("select DISTINCT t_certificate.companyid ,t_shipper.shippername" +
            //                                 " FROM t_certificate left join t_shipper ON t_certificate.companyid = t_shipper.shipperid" +
            //                                 " WHERE t_certificate.createdeptid like '{0}%' ", deptId), "cxtj");
        }


        private void _query_Click(object sender, RoutedEventArgs e)
        {
            //清空列表
            lvlist.DataContext = null;

            DataTable table = dbOperation.GetDbHelper().GetDataSet(string.Format("call p_query_certificate_new({0},'{1}','{2}')",
                   (Application.Current.Resources["User"] as UserInfo).ID,
                   _card_no.Text,
                   _source_company.Text)).Tables[0];

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

        //private void _btn_details_Click(object sender, RoutedEventArgs e)
        //{
        //    string batch_no = (sender as Button).Tag.ToString();

        //    grid_info.Children.Add(new UcCreateCertificatedetails(dbOperation, batch_no));
        //}

        private void _btn_card_Click(object sender, RoutedEventArgs e)
        {
            string card_id = (sender as Button).Tag.ToString();
            CertificatePreview cer = new CertificatePreview(dbOperation,card_id);
            cer.ShowDialog();
        }

    }
}
