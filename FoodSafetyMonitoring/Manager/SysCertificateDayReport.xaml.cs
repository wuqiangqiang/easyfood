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
        public SysCertificateDayReport(IDBOperation dbOperation)
        {
            InitializeComponent();

            this.dbOperation = dbOperation;
        }

        private void _query_Click(object sender, RoutedEventArgs e)
        {

        }

        private void _export_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
