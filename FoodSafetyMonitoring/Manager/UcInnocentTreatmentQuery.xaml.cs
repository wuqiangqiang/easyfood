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
using System.Data.Odbc;
using Microsoft.Office.Interop.Excel;

namespace FoodSafetyMonitoring.Manager
{
    /// <summary>
    /// UcInnocentTreatmentQuery.xaml 的交互逻辑
    /// </summary>
    public partial class UcInnocentTreatmentQuery : UserControl
    {
        private IDBOperation dbOperation;
        string userId = (System.Windows.Application.Current.Resources["User"] as UserInfo).ID;
        string loginid = (System.Windows.Application.Current.Resources["User"] as UserInfo).LoginName;
        string username = (System.Windows.Application.Current.Resources["User"] as UserInfo).ShowName;
        string deptId = (System.Windows.Application.Current.Resources["User"] as UserInfo).DepartmentID;
        string user_flag_tier = (System.Windows.Application.Current.Resources["User"] as UserInfo).FlagTier;

        public UcInnocentTreatmentQuery(IDBOperation dbOperation)
        {
            InitializeComponent();

            this.dbOperation = dbOperation;

            dtpStartDate.SelectedDate = DateTime.Now;
            dtpEndDate.SelectedDate = DateTime.Now;

            //申报人姓名
            ComboboxTool.InitComboboxSource(_shipper_name, string.Format("SELECT sbrid,sbrname FROM t_record_sbr WHERE openflag = '1' and createdeptid = '{0}'", deptId), "lr");
            //检测单位
            ComboboxTool.InitComboboxSource(_detect_station, string.Format("call p_user_dept('{0}')", userId), "cxtj");
            //检测师
            ComboboxTool.InitComboboxSource(_detect_person1, string.Format("call p_user_detuser('{0}')", userId), "cxtj");

            //如果登录用户的部门是站点级别，则将查询条件检测单位赋上默认值
            if (user_flag_tier == "4")
            {
                _detect_station.SelectedIndex = 1;
            }
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

            System.Data.DataTable table = dbOperation.GetDbHelper().GetDataSet(string.Format("call p_innocent_query('{0}','{1}','{2}','{3}','{4}','{5}','{6}')",
                              deptId,
                              ((DateTime)dtpStartDate.SelectedDate).ToShortDateString(),
                              ((DateTime)dtpEndDate.SelectedDate).ToShortDateString(),
                              _detect_station.SelectedIndex < 1 ? "" : (_detect_station.SelectedItem as System.Windows.Controls.Label).Tag,
                              _detect_person1.SelectedIndex < 1 ? "" : (_detect_person1.SelectedItem as System.Windows.Controls.Label).Tag,
                               shipper_name,_qua_cardid.Text)).Tables[0];

            _sj.Visibility = Visibility.Visible;
            _hj.Visibility = Visibility.Visible;
            _title.Text = (table.Rows.Count - 1).ToString();

            livst.DataContext = table;
            if ((table.Rows.Count - 1) == 0)
            {
                Toolkit.MessageBox.Show("没有查询到数据！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
        }

        private void _export_Click(object sender, RoutedEventArgs e)
        {
            if (dtpStartDate.SelectedDate.Value.Date != dtpEndDate.SelectedDate.Value.Date)
            {
                Toolkit.MessageBox.Show("只能导出同一天的数据（开始日期必须等于结束日期），请重新选择！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (user_flag_tier != "4")
            {
                if (_detect_station.SelectedIndex < 1)
                {
                    Toolkit.MessageBox.Show("只能导出一个检疫分站的数据，请选择！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
            }

            string shipper_name;
            if (_shipper_name.SelectedIndex == 0)
            {
                shipper_name = "";
            }
            else
            {
                shipper_name = _shipper_name.Text;
            }

            System.Data.DataTable exporttable = dbOperation.GetDbHelper().GetDataSet(string.Format("call p_innocent_query('{0}','{1}','{2}','{3}','{4}','{5}','{6}')",
                              deptId,
                              ((DateTime)dtpStartDate.SelectedDate).ToShortDateString(),
                              ((DateTime)dtpEndDate.SelectedDate).ToShortDateString(),
                              _detect_station.SelectedIndex < 1 ? "" : (_detect_station.SelectedItem as System.Windows.Controls.Label).Tag,
                              _detect_person1.SelectedIndex < 1 ? "" : (_detect_person1.SelectedItem as System.Windows.Controls.Label).Tag,
                               shipper_name, _qua_cardid.Text)).Tables[0];

            if (exporttable.Rows.Count - 1 == 0)
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
                    excelWS.Name = "屠宰检疫无害化处理情况日汇总表";

                    //excelWS.Cells.NumberFormat = "@";     //  如果数据中存在数字类型 可以让它变文本格式显示 
                    //导出表头
                    excelWS.Cells[1, 1] = "屠宰检疫无害化处理情况日汇总表";
                    //合并单元格 
                    Range excelRange = excelWS.get_Range("A1", "J1");
                    excelRange.Merge(excelRange.MergeCells);
                    excelRange.RowHeight = 50;
                    //设置字体大小  
                    excelRange.Font.Size = 18;
                    excelRange.Font.Bold = 10;
                    // 文本水平居中方式  
                    excelRange.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;

                    //导出标题
                    excelWS.Cells[2, 1] = "动物卫生监督所(分所)名称：" + exporttable.Rows[0][0].ToString();
                    excelWS.Cells[2, 7] = "屠宰场名称：" + exporttable.Rows[0][1].ToString();
                    //设置行高
                    Range excelRange2 = excelWS.get_Range("A2");
                    excelRange2.RowHeight = 25;
                    //合并单元格
                    excelWS.get_Range("A2", "F2").Merge(excelWS.get_Range("A2", "F2").MergeCells);
                    excelWS.get_Range("G2", "J2").Merge(excelWS.get_Range("G2", "J2").MergeCells);

                    //导出列名
                    excelWS.Cells[3, 4] = "宰前检查";
                    excelWS.Cells[3, 6] = "同步检疫";
                    excelWS.Cells[3, 8] = "检疫人员";
                    //合并单元格
                    excelWS.get_Range("D3", "E3").Merge(excelWS.get_Range("D3", "E3").MergeCells);
                    excelWS.get_Range("D3", "E3").HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                    excelWS.get_Range("F3", "G3").Merge(excelWS.get_Range("F3", "G3").MergeCells);
                    excelWS.get_Range("F3", "G3").HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                    excelWS.get_Range("H3", "I3").Merge(excelWS.get_Range("H3", "I3").MergeCells);
                    excelWS.get_Range("H3", "I3").HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;


                    excelWS.Cells[4, 1] = "货主姓名";
                    excelWS.Cells[4, 2] = "产地";
                    excelWS.Cells[4, 3] = "《检疫处理通知单》编号";
                    excelWS.Cells[4, 4] = "不合格数(头、只、羽、匹)";
                    excelWS.Cells[4, 5] = "无害化处理方式";
                    excelWS.Cells[4, 6] = "不合格数(头、只、羽、匹)";
                    excelWS.Cells[4, 7] = "无害化处理方式";
                    excelWS.Cells[4, 8] = "官方兽医姓名";
                    excelWS.Cells[4, 9] = "协检员";
                    excelWS.Cells[4, 10] = "备注";
                    //合并单元格
                    excelWS.get_Range("A3", "A4").Merge(excelWS.get_Range("A3", "A4").MergeCells);
                    excelWS.get_Range("A3", "A4").HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                    excelWS.get_Range("B3", "B4").Merge(excelWS.get_Range("B3", "B4").MergeCells);
                    excelWS.get_Range("B3", "B4").HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                    excelWS.get_Range("C3", "C4").Merge(excelWS.get_Range("C3", "C4").MergeCells);
                    excelWS.get_Range("C3", "C4").HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                    excelWS.get_Range("J3", "J4").Merge(excelWS.get_Range("J3", "J4").MergeCells);
                    excelWS.get_Range("J3", "J4").HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;

                    Range range = null;
                    range = excelWS.get_Range(excelWS.Cells[3, 1], excelWS.Cells[exporttable.Rows.Count + 4, 10]);  //设置表格左上角开始显示的位置 
                    range.ColumnWidth = 10; //设置单元格的宽度
                    range.Borders.LineStyle = 1; //设置单元格边框的粗细  
                    //文本自动换行  
                    range.WrapText = true;

                    //特殊设置-产地、编号
                    excelWS.get_Range("B3").ColumnWidth = 28; //设置单元格的宽度
                    excelWS.get_Range("C3").ColumnWidth = 25; //设置单元格的宽度

                    //将数据导入到工作表的单元格  
                    for (int i = 0; i < exporttable.Rows.Count; i++)
                    {
                        for (int j = 3; j < exporttable.Columns.Count; j++)
                        {
                            excelWS.Cells[i + 5, j - 2] = exporttable.Rows[i][j].ToString();
                        }
                    }

                    //赋值合计
                    excelWS.Cells[exporttable.Rows.Count + 4, 1] = "合计";

                    //赋值检疫日期
                    excelWS.Cells[exporttable.Rows.Count + 5, 8] = "检疫日期：  " + dtpStartDate.SelectedDate.Value.Year + "年" +
                                                         dtpStartDate.SelectedDate.Value.Month + "月" + dtpStartDate.SelectedDate.Value.Day +
                                                         "日";
                    //设置内容单元格的高度和居中
                    excelWS.get_Range(excelWS.Cells[5, 1], excelWS.Cells[exporttable.Rows.Count + 5, 1]).RowHeight = 25;
                    excelWS.get_Range(excelWS.Cells[5, 1], excelWS.Cells[exporttable.Rows.Count + 5, 1]).HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;

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
