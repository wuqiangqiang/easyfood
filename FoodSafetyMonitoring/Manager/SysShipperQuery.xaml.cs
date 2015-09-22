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
using System.Data.Odbc;
using Microsoft.Office.Interop.Excel;

namespace FoodSafetyMonitoring.Manager
{
    /// <summary>
    /// SysShipperQuery.xaml 的交互逻辑
    /// </summary>
    public partial class SysShipperQuery : UserControl
    {
        private IDBOperation dbOperation;
        private System.Data.DataTable exporttable;
        private string deptId;
        private Dictionary<string, MyColumn> MyColumns = new Dictionary<string, MyColumn>();
        private string shipperflag;

        public SysShipperQuery(IDBOperation dbOperation)
        {
            InitializeComponent();

            this.dbOperation = dbOperation;
            deptId = (System.Windows.Application.Current.Resources["User"] as UserInfo).DepartmentID;

            shipperflag = dbOperation.GetDbHelper().GetSingle("select ifnull(a.shipperflag,'') as shipperflag " +
                                    " from sys_client_sysdept a " +
                                    " where INFO_CODE = " + deptId).ToString();
        }


        private void _query_Click(object sender, RoutedEventArgs e)
        {
            string shipper_id = _shipper_id.Text.Trim();
            string shipper_name = _shipper_name.Text.Trim();


            System.Data.DataTable table = dbOperation.GetDbHelper().GetDataSet(string.Format("select shipperid,shippername,phone,region,town,village from t_shipper " +
                               "where shipperflag = '{0}' and (shipperid = '{1}' or '{2}' = '') and (shippername like '{3}%' or '{4}' = '')",
                               shipperflag, shipper_id, shipper_id, shipper_name,shipper_name)).Tables[0];

            
            lvlist.DataContext = table;
            exporttable = table;

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
            string shipper_id = _shipper_id.Text.Trim();
            string shipper_name = _shipper_name.Text.Trim();


           System.Data.DataTable table = dbOperation.GetDbHelper().GetDataSet(string.Format("select shipperid,shippername,phone,region,town,village from t_shipper " +
                               "where shipperflag = '{0}' and (shipperid = '{1}' or '{2}' = '') and (shippername like '{3}%' or '{4}' = '')",
                               shipperflag, shipper_id, shipper_id, shipper_name, shipper_name)).Tables[0];

            lvlist.DataContext = table;
            exporttable = table;

        }

        private void _btn_modify_Click(object sender, RoutedEventArgs e)
        {
            string shipper_id = (sender as System.Windows.Controls.Button).Tag.ToString();
            ModifyShipper ship = new ModifyShipper(dbOperation, shipper_id, shipperflag,this);
            ship.ShowDialog();
        }

        private void _export_Click(object sender, RoutedEventArgs e)
        {
            if (exporttable != null)
            {
                if(exporttable.Rows.Count == 0)
                {
                    Toolkit.MessageBox.Show("导出内容为空，请确认！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                //打开对话框
                System.Windows.Forms.SaveFileDialog saveFile = new System.Windows.Forms.SaveFileDialog();
                saveFile.Filter = "Excel(*.xlsx)|*.xlsx|Excel(*.xls)|*.xls";
                saveFile.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                if (saveFile.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
                {
                    return;
                }
                var excelFilePath = saveFile.FileName;
                if (excelFilePath != "")
                {
                    if (System.IO.File.Exists(excelFilePath))
                    {
                        try
                        {
                            System.IO.File.Delete(excelFilePath);
                        }
                        catch (Exception ex)
                        {
                            Toolkit.MessageBox.Show("导出文件时出错,文件可能正被打开！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                            return;
                        }
                    }

                    try
                    {
                        //创建Excel  
                        Microsoft.Office.Interop.Excel.Application excelApp = new Microsoft.Office.Interop.Excel.Application();

                        if (excelApp == null)
                        {
                            Toolkit.MessageBox.Show("无法创建Excel对象，可能您的机子未安装Excel程序！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                            return;
                        }
                        Workbook excelWB = excelApp.Workbooks.Add(System.Type.Missing);    //创建工作簿（WorkBook：即Excel文件主体本身）  
                        Worksheet excelWS = (Worksheet)excelWB.Worksheets[1];   //创建工作表（即Excel里的子表sheet） 1表示在子表sheet1里进行数据导出 
                        excelWS.Name = "货主信息";

                        //excelWS.Cells.NumberFormat = "@";     //  如果数据中存在数字类型 可以让它变文本格式显示 
                        //导出列名
                        excelWS.Cells[1, 1] = "货主代码";
                        excelWS.Cells[1, 2] = "货主";
                        excelWS.Cells[1, 3] = "电话";
                        excelWS.Cells[1, 4] = "县(区)";
                        excelWS.Cells[1, 5] = "乡(镇)";
                        excelWS.Cells[1, 6] = "村(场)";

                        //将数据导入到工作表的单元格  
                        for (int i = 0; i < exporttable.Rows.Count; i++)
                        {
                            for (int j = 0; j < exporttable.Columns.Count; j++)
                            {
                                excelWS.Cells[i + 2, j + 1] = exporttable.Rows[i][j].ToString();
                            }
                        }

                        Range range = null;
                        range = excelWS.get_Range(excelWS.Cells[1, 1], excelWS.Cells[exporttable.Rows.Count + 1, 6]);  //设置表格左上角开始显示的位置 
                        range.ColumnWidth = 15; //设置单元格的宽度
                        range.RowHeight = 22;//设置单元格的高度
                        range.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;

                        excelWB.SaveAs(excelFilePath);  //将其进行保存到指定的路径  
                        excelWB.Close();
                        excelApp.Quit();
                        KillAllExcel(excelApp); //释放可能还没释放的进程  
                        Toolkit.MessageBox.Show("文件导出成功！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch
                    {
                        Toolkit.MessageBox.Show("无法创建Excel对象，可能您的机子Office版本有问题！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                        return;
                    }

                }
            }
        }

        public bool KillAllExcel(Microsoft.Office.Interop.Excel.Application excelApp)
        {
            try
            {
                if (excelApp != null)
                {
                    excelApp.Quit();
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApp);
                    //释放COM组件，其实就是将其引用计数减1     
                    //System.Diagnostics.Process theProc;     
                    foreach (System.Diagnostics.Process theProc in System.Diagnostics.Process.GetProcessesByName("EXCEL"))
                    {
                        //先关闭图形窗口。如果关闭失败.有的时候在状态里看不到图形窗口的excel了，     
                        //但是在进程里仍然有EXCEL.EXE的进程存在，那么就需要释放它     
                        if (theProc.CloseMainWindow() == false)
                        {
                            theProc.Kill();
                        }
                    }
                    excelApp = null;
                    return true;
                }
            }
            catch
            {
                return false;
            }
            return true;
        }  
    }
}
