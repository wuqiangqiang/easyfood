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
using System.Data.Odbc;
using Microsoft.Office.Interop.Excel;

namespace FoodSafetyMonitoring.Manager
{
    /// <summary>
    /// UcDetectInquire.xaml 的交互逻辑
    /// </summary>
    public partial class SysDetectReport : UserControl
    {
        System.Data.DataTable ProvinceCityTable;
        public IDBOperation dbOperation = null;
        private Dictionary<string, MyColumn> MyColumns = new Dictionary<string, MyColumn>();

        string userId = (System.Windows.Application.Current.Resources["User"] as UserInfo).ID;
        string user_flag_tier = (System.Windows.Application.Current.Resources["User"] as UserInfo).FlagTier;

        Exporting_window load;

        public SysDetectReport(IDBOperation dbOperation)
        {
            InitializeComponent();
            this.dbOperation = dbOperation;
            ProvinceCityTable = System.Windows.Application.Current.Resources["省市表"] as System.Data.DataTable;
            DataRow[] rows = ProvinceCityTable.Select("pid = '0001'");

            //画面初始化-检测单列表画面
            dtpStartDate.SelectedDate = DateTime.Now.AddDays(-1);
            dtpEndDate.SelectedDate = DateTime.Now;
            ComboboxTool.InitComboboxSource(_source_company1, string.Format(" call p_user_company('{0}','') ", userId), "cxtj");
            ComboboxTool.InitComboboxSource(_detect_station, string.Format("call p_user_dept('{0}')", userId), "cxtj");
            ComboboxTool.InitComboboxSource(_detect_person1, string.Format("call p_user_detuser('{0}')", userId), "cxtj");

            ComboboxTool.InitComboboxSource(_province1, rows, "cxtj");
            _province1.SelectionChanged += new SelectionChangedEventHandler(_province1_SelectionChanged);
            //20150707检测师改为连动（受监测站点影响）
            _detect_station.SelectionChanged += new SelectionChangedEventHandler(_detect_station_SelectionChanged);

            //如果登录用户的部门是站点级别，则将查询条件检测单位赋上默认值
            if (user_flag_tier == "4")
            {
                _detect_station.SelectedIndex = 1;
            }

        }

        void _province1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_province1.SelectedIndex > 0)
            {
                DataRow[] rows = ProvinceCityTable.Select("pid = '" + (_province1.SelectedItem as System.Windows.Controls.Label).Tag.ToString() + "'");
                ComboboxTool.InitComboboxSource(_city1, rows, "cxtj");
                //20150707被检单位改为连动（受来源产地影响）
                ComboboxTool.InitComboboxSource(_source_company1, string.Format(" call p_user_company('{0}','{1}') ", userId, (_province1.SelectedItem as System.Windows.Controls.Label).Tag.ToString()), "cxtj");
                _city1.SelectionChanged += new SelectionChangedEventHandler(_city1_SelectionChanged);
            }
        }


        void _city1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_city1.SelectedIndex > 0)
            {
                DataRow[] rows = ProvinceCityTable.Select("pid = '" + (_city1.SelectedItem as System.Windows.Controls.Label).Tag.ToString() + "'");
                ComboboxTool.InitComboboxSource(_region1, rows, "cxtj");
                //20150707被检单位改为连动（受来源产地影响）
                ComboboxTool.InitComboboxSource(_source_company1, string.Format(" call p_user_company('{0}','{1}') ", userId, (_city1.SelectedItem as System.Windows.Controls.Label).Tag.ToString()), "cxtj");
                _region1.SelectionChanged += new SelectionChangedEventHandler(_region1_SelectionChanged);
            }
        }

        //20150707被检单位改为连动（受来源产地影响）
        void _region1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_region1.SelectedIndex > 0)
            {
                ComboboxTool.InitComboboxSource(_source_company1, string.Format("call p_user_company('{0}','{1}')", userId, (_region1.SelectedItem as System.Windows.Controls.Label).Tag.ToString()), "cxtj");
            }
        }

        //20150707检测师改为连动（受检测单位影响）
        void _detect_station_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_detect_station.SelectedIndex > 0)
            {
                ComboboxTool.InitComboboxSource(_detect_person1, string.Format("SELECT RECO_PKID,INFO_USER,NUMB_USER FROM sys_client_user where fk_dept = '{0}'", (_detect_station.SelectedItem as System.Windows.Controls.Label).Tag.ToString()), "cxtj");
            }
            else if (_detect_station.SelectedIndex == 0)
            {
                ComboboxTool.InitComboboxSource(_detect_person1, string.Format("call p_user_detuser('{0}')", userId), "cxtj");
            }
        }

        private void _query_Click(object sender, RoutedEventArgs e)
        {
            if (dtpStartDate.SelectedDate.Value.Date > dtpEndDate.SelectedDate.Value.Date)
            {
                Toolkit.MessageBox.Show("开始日期大于结束日期，请重新选择！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            System.Data.DataTable table = GetData();

            livst.DataContext = table;
            _sj.Visibility = Visibility.Visible;
            _hj.Visibility = Visibility.Visible;
            _title.Text = table.Rows.Count.ToString();

            if (table.Rows.Count == 0)
            {
                Toolkit.MessageBox.Show("没有查询到数据！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
        }

        private System.Data.DataTable GetData()
        {
            System.Data.DataTable table = dbOperation.GetDbHelper().GetDataSet(string.Format("call p_detect_report({0},'{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}')",
                   (System.Windows.Application.Current.Resources["User"] as UserInfo).ID,
                   ((DateTime)dtpStartDate.SelectedDate).ToShortDateString(),
                   ((DateTime)dtpEndDate.SelectedDate).ToShortDateString(),
                   _province1.SelectedIndex < 1 ? "" : (_province1.SelectedItem as System.Windows.Controls.Label).Tag,
                   _city1.SelectedIndex < 1 ? "" : (_city1.SelectedItem as System.Windows.Controls.Label).Tag,
                   _region1.SelectedIndex < 1 ? "" : (_region1.SelectedItem as System.Windows.Controls.Label).Tag,
                   _source_company1.SelectedIndex < 1 ? "" : (_source_company1.SelectedItem as System.Windows.Controls.Label).Tag,
                    _detect_station.SelectedIndex < 1 ? "" : (_detect_station.SelectedItem as System.Windows.Controls.Label).Tag,
                   _detect_person1.SelectedIndex < 1 ? "" : (_detect_person1.SelectedItem as System.Windows.Controls.Label).Tag)).Tables[0];

            return table;
        }

        private void _export_Click(object sender, RoutedEventArgs e)
        {
            if (user_flag_tier != "4")
            {
                if (_detect_station.SelectedIndex < 1)
                {
                    Toolkit.MessageBox.Show("只能导出一个检测单位的数据，请选择！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
            }

            System.Data.DataTable exporttable = GetData();

            if (exporttable.Rows.Count == 0)
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

                    load = new Exporting_window();
                    load.Show();

                    Workbook excelWB = excelApp.Workbooks.Add(System.Type.Missing);    //创建工作簿（WorkBook：即Excel文件主体本身）  
                    Worksheet excelWS = (Worksheet)excelWB.Worksheets[1];   //创建工作表（即Excel里的子表sheet） 1表示在子表sheet1里进行数据导出 
                    excelWS.Name = "瘦肉精自检表";

                    //excelWS.Cells.NumberFormat = "@";     //  如果数据中存在数字类型 可以让它变文本格式显示 
                    //导出表头
                    excelWS.Cells[1, 1] = exporttable.Rows[0][0].ToString() + "瘦肉精自检表";
                    //合并单元格 
                    Range excelRange = excelWS.get_Range("A1", "N1");
                    excelRange.Merge(excelRange.MergeCells);
                    excelRange.RowHeight = 50;
                    //设置字体大小  
                    excelRange.Font.Size = 18;
                    excelRange.Font.Bold = 10;
                    // 文本水平居中方式  
                    excelRange.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;

                    //导出列名
                    excelWS.Cells[2, 7] = "盐酸克伦特罗";
                    excelWS.Cells[2, 9] = "莱克多巴胺";
                    excelWS.Cells[3, 9] = "沙丁胺醇";
                    //合并单元格
                    excelWS.get_Range("G2", "H2").Merge(excelWS.get_Range("G2", "H2").MergeCells);
                    excelWS.get_Range("G2", "H2").HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                    excelWS.get_Range("I2", "J2").Merge(excelWS.get_Range("I2", "J2").MergeCells);
                    excelWS.get_Range("I2", "J2").HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                    excelWS.get_Range("K2", "L2").Merge(excelWS.get_Range("K2", "L2").MergeCells);
                    excelWS.get_Range("K2", "L2").HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;

                    excelWS.Cells[3, 1] = "货主";
                    excelWS.Cells[3, 2] = "数量";
                    excelWS.Cells[3, 3] = "产地";
                    excelWS.Cells[3, 4] = "检疫证号";
                    excelWS.Cells[3, 5] = "耳标号";
                    excelWS.Cells[3, 6] = "抽检时间";
                    excelWS.Cells[3, 7] = "数量";
                    excelWS.Cells[3, 8] = "阳性数";
                    excelWS.Cells[3, 9] = "数量";
                    excelWS.Cells[3, 10] = "阳性数";
                    excelWS.Cells[3, 11] = "数量";
                    excelWS.Cells[3, 12] = "阳性数";
                    excelWS.Cells[3, 13] = "检测人";
                    excelWS.Cells[3, 14] = "监督人";
                    //合并单元格
                    excelWS.get_Range("A2", "A3").Merge(excelWS.get_Range("A2", "A3").MergeCells);
                    excelWS.get_Range("A2", "A3").HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                    excelWS.get_Range("B2", "B3").Merge(excelWS.get_Range("B2", "B3").MergeCells);
                    excelWS.get_Range("B2", "B3").HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                    excelWS.get_Range("C2", "C3").Merge(excelWS.get_Range("C2", "C3").MergeCells);
                    excelWS.get_Range("C2", "C3").HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                    excelWS.get_Range("D2", "D3").Merge(excelWS.get_Range("D2", "D3").MergeCells);
                    excelWS.get_Range("D2", "D3").HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                    excelWS.get_Range("E2", "E3").Merge(excelWS.get_Range("E2", "E3").MergeCells);
                    excelWS.get_Range("E2", "E3").HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                    excelWS.get_Range("F2", "F3").Merge(excelWS.get_Range("F2", "F3").MergeCells);
                    excelWS.get_Range("F2", "F3").HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                    excelWS.get_Range("M2", "M3").Merge(excelWS.get_Range("M2", "M3").MergeCells);
                    excelWS.get_Range("M2", "M3").HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                    excelWS.get_Range("N2", "N3").Merge(excelWS.get_Range("N2", "N3").MergeCells);
                    excelWS.get_Range("N2", "N3").HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                    excelWS.get_Range("G3", "l3").HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;

                    Range range = null;
                    range = excelWS.get_Range(excelWS.Cells[2, 1], excelWS.Cells[exporttable.Rows.Count + 3, 14]);  //设置表格左上角开始显示的位置 
                    range.ColumnWidth = 10; //设置单元格的宽度
                    range.Borders.LineStyle = 1; //设置单元格边框的粗细  
                    //文本自动换行  
                    range.WrapText = true;

                    //特殊设置-产地
                    excelWS.get_Range("C2").ColumnWidth = 20; //设置单元格的宽度

                    //将数据导入到工作表的单元格  
                    for (int i = 0; i < exporttable.Rows.Count; i++)
                    {
                        for (int j = 1; j < exporttable.Columns.Count; j++)
                        {
                            excelWS.Cells[i + 4, j] = exporttable.Rows[i][j].ToString();
                        }
                    }

                    //设置内容单元格的高度
                    excelWS.get_Range(excelWS.Cells[4, 1], excelWS.Cells[exporttable.Rows.Count + 3, 1]).RowHeight = 25;
                    excelWS.get_Range(excelWS.Cells[4, 1], excelWS.Cells[exporttable.Rows.Count + 3, 1]).HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;

                    excelWB.SaveAs(excelFilePath);  //将其进行保存到指定的路径  
                    excelWB.Close();
                    excelApp.Quit();
                    KillAllExcel(excelApp); //释放可能还没释放的进程  
                    load.Close();
                    Toolkit.MessageBox.Show("文件导出成功！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch
                {
                    load.Close();
                    Toolkit.MessageBox.Show("无法创建Excel对象，可能您的机子Office版本有问题！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
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
