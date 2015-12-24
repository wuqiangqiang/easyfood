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
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;

namespace FoodSafetyMonitoring.Manager
{
    /// <summary>
    /// SysCompanyQuery.xaml 的交互逻辑
    /// </summary>
    public partial class SysCompanyQuery : UserControl
    {
        DataTable ProvinceCityTable;
        private IDBOperation dbOperation;
        private string user_flag_tier;
        private DataTable exporttable;
        private Dictionary<string, MyColumn> MyColumns = new Dictionary<string, MyColumn>();

        public SysCompanyQuery(IDBOperation dbOperation)
        {
            InitializeComponent();

            this.dbOperation = dbOperation;
            ProvinceCityTable = Application.Current.Resources["省市表"] as DataTable;
            DataRow[] rows = ProvinceCityTable.Select("pid = '0001'");
            user_flag_tier = (Application.Current.Resources["User"] as UserInfo).FlagTier;

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
            //检测单位
            ComboboxTool.InitComboboxSource(_detect_dept, "call p_dept_cxtj(" + (Application.Current.Resources["User"] as UserInfo).ID + ")", "cxtj");
            //来源产地
            ComboboxTool.InitComboboxSource(_province1, rows, "cxtj");
            _province1.SelectionChanged += new SelectionChangedEventHandler(_province1_SelectionChanged);
        }

        void _province1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_province1.SelectedIndex > 0)
            {
                DataRow[] rows = ProvinceCityTable.Select("pid = '" + (_province1.SelectedItem as Label).Tag.ToString() + "'");
                ComboboxTool.InitComboboxSource(_city1, rows, "cxtj");
                _city1.SelectionChanged += new SelectionChangedEventHandler(_city1_SelectionChanged);
            }
        }


        void _city1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_city1.SelectedIndex > 0)
            {
                DataRow[] rows = ProvinceCityTable.Select("pid = '" + (_city1.SelectedItem as Label).Tag.ToString() + "'");
                ComboboxTool.InitComboboxSource(_region1, rows, "cxtj");
            }
        }

        private void _query_Click(object sender, RoutedEventArgs e)
        {
            string company_name = _company_name.Text;
            System.Data.DataTable table = dbOperation.GetDbHelper().GetDataSet(string.Format("call p_query_company('{0}','{1}','{2}','{3}','{4}','{5}')",
                                          (Application.Current.Resources["User"] as UserInfo).ID,
                                          _province1.SelectedIndex < 1 ? "" : (_province1.SelectedItem as Label).Tag,
                                          _city1.SelectedIndex < 1 ? "" : (_city1.SelectedItem as Label).Tag,
                                          _region1.SelectedIndex < 1 ? "" : (_region1.SelectedItem as Label).Tag,
                                          _detect_dept.SelectedIndex < 1 ? "" : (_detect_dept.SelectedItem as Label).Tag,
                                          company_name)).Tables[0];


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
                saveFile.Filter = "Text documents (.pdf)|*.pdf";
                saveFile.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                if (saveFile.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
                {
                    return;
                }
                var pdfFilePath = saveFile.FileName;
                if (pdfFilePath != "")
                {
                    if (System.IO.File.Exists(pdfFilePath))
                    {
                        try
                        {
                            System.IO.File.Delete(pdfFilePath);
                        }
                        catch (Exception ex)
                        {
                            Toolkit.MessageBox.Show("导出文件时出错,文件可能正被打开！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                            return;
                        }
                    }

                    try
                    {
                        Document document = new Document();
                        PdfWriter.GetInstance(document, new FileStream(pdfFilePath, FileMode.Create));
                        // 添加文档内容
                        document.Open();

                        //设置中文是字体，否则，中文存不了
                        BaseFont bfHei = BaseFont.CreateFont(@"C:\Windows\Fonts\simfang.ttf", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
                        iTextSharp.text.Font font = new iTextSharp.text.Font(bfHei, 10);

                        PdfPTable table = new PdfPTable(3);
                        table.AddCell(new Phrase("检测单位", font));
                        table.AddCell(new Phrase("来源单位", font));
                        table.AddCell(new Phrase("来源产地", font));
                        for (int i = 0; i < exporttable.Rows.Count; i++)
                        {
                            table.AddCell(new Phrase(exporttable.Rows[i][1].ToString(), font));
                            table.AddCell(new Phrase(exporttable.Rows[i][3].ToString(), font));
                            table.AddCell(new Phrase(exporttable.Rows[i][4].ToString(), font));
                        }

                        document.Add(table);
                        document.Close();
                        Toolkit.MessageBox.Show("文件导出成功！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);

                    }
                    catch
                    {
                        Toolkit.MessageBox.Show("无法创建pdf对象！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                        return;
                    }

                }
            }
        }
    }
}
