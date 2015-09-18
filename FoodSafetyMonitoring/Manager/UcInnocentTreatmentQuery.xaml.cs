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
    /// UcInnocentTreatmentQuery.xaml 的交互逻辑
    /// </summary>
    public partial class UcInnocentTreatmentQuery : UserControl
    {
        private IDBOperation dbOperation;
        string userId = (Application.Current.Resources["User"] as UserInfo).ID;
        string loginid = (Application.Current.Resources["User"] as UserInfo).LoginName;
        string username = (Application.Current.Resources["User"] as UserInfo).ShowName;
        string deptId = (Application.Current.Resources["User"] as UserInfo).DepartmentID;

        public UcInnocentTreatmentQuery(IDBOperation dbOperation)
        {
            InitializeComponent();

            this.dbOperation = dbOperation;

            dtpStartDate.SelectedDate = DateTime.Now;
            dtpEndDate.SelectedDate = DateTime.Now;

            //申报人姓名
            ComboboxTool.InitComboboxSource(_shipper_name, string.Format("SELECT sbrid,sbrname FROM t_record_sbr WHERE openflag = '1' and createdeptid = '{0}'", deptId), "lr");
        }

        private void _query_Click(object sender, RoutedEventArgs e)
        {
            string shipper_name;
            if (_shipper_name.SelectedIndex == 0)
            {
                shipper_name = "";
            }
            else
            {
                shipper_name = _shipper_name.Text;
            }

            DataTable table = dbOperation.GetDbHelper().GetDataSet(string.Format("call p_innocent_query('{0}','{1}','{2}','{3}','{4}')",
                              deptId,
                              ((DateTime)dtpStartDate.SelectedDate).ToShortDateString(),
                              ((DateTime)dtpEndDate.SelectedDate).ToShortDateString(),
                               shipper_name,_qua_cardid.Text)).Tables[0];

            livst.DataContext = table;

        }

        private void _export_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
