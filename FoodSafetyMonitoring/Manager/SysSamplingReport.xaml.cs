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
using FoodSafetyMonitoring.Common;
using System.Data;
using FoodSafetyMonitoring.Manager.UserControls;
using Toolkit = Microsoft.Windows.Controls;

namespace FoodSafetyMonitoring.Manager
{
    /// <summary>
    /// SysSamplingReport.xaml 的交互逻辑
    /// </summary>
    public partial class SysSamplingReport : UserControl
    {
        private IDBOperation dbOperation;
        private string user_flag_tier;
        private DataTable currenttable;
        private Dictionary<string, MyColumn> MyColumns = new Dictionary<string, MyColumn>();

        public SysSamplingReport(IDBOperation dbOperation)
        {
            InitializeComponent();

            this.dbOperation = dbOperation;
            user_flag_tier = (Application.Current.Resources["User"] as UserInfo).FlagTier;

            dtpStartDate.SelectedDate = DateTime.Now.AddDays(-1);
            dtpEndDate.SelectedDate = DateTime.Now;

            //检测单位
            switch (user_flag_tier)
            {
                case "0": _dept_name.Text = "选择省:";
                    break;
                case "1": _dept_name.Text = "选择市(州):";
                    break;
                case "2": _dept_name.Text = "选择区县:";
                    break;
                case "3": _dept_name.Text = "选择检测单位:";
                    break;
                case "4": _dept_name.Text = "选择检测单位:";
                    break;
                default: break;
            }
            ComboboxTool.InitComboboxSource(_detect_dept, "call p_dept_cxtj(" + (Application.Current.Resources["User"] as UserInfo).ID + ")", "cxtj");
            //检测项目
            ComboboxTool.InitComboboxSource(_detect_item, "SELECT ItemID,ItemNAME FROM t_det_item WHERE  (tradeId ='1'or tradeId ='2' or tradeId ='3' or ifnull(tradeId,'') = '') and OPENFLAG = '1' order by orderId", "cxtj");

            //如果登录用户的部门是站点级别，则将查询条件检测单位赋上默认值
            if (isDept())
            {
                _detect_dept.SelectedIndex = 1;
            }
        }

        //判断登录用户的部门级别
        private bool isDept()
        {
            string flag = dbOperation.GetDbHelper().GetSingle("select FLAG_TIER from sys_client_sysdept where INFO_CODE =" + (Application.Current.Resources["User"] as UserInfo).DepartmentID).ToString();

            if (flag == "4")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void _query_Click(object sender, RoutedEventArgs e)
        {

        }

        private void _export_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
