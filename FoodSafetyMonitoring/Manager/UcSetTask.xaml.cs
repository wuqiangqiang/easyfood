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
    /// UcSetSamplingRate.xaml 的交互逻辑
    /// </summary>
    public partial class UcSetTask : UserControl
    {
        private IDBOperation dbOperation;
        private System.Data.DataTable currenttable;
        private string user_flag_tier;
        private string deptid;
        private List<DeptItem> list = new List<DeptItem>();

        public UcSetTask(IDBOperation dbOperation)
        {
            InitializeComponent();
            this.dbOperation = dbOperation;
            user_flag_tier = (System.Windows.Application.Current.Resources["User"] as UserInfo).FlagTier;
            deptid = (System.Windows.Application.Current.Resources["User"] as UserInfo).DepartmentID;

            _tableview.ModifyRowEnvent += new UcTableOperableView_NoPages.ModifyRowEventHandler(_tableview_ModifyRowEnvent);
            Load_table();
        }

        public void Load_table()
        {
            Dictionary<string, MyColumn> MyColumns = new Dictionary<string, MyColumn>();

            System.Data.DataTable table = dbOperation.GetDbHelper().GetDataSet(string.Format("call p_task_details({0})",
                              (System.Windows.Application.Current.Resources["User"] as UserInfo).ID)).Tables[0];

            currenttable = table;

            list.Clear();
            for (int i = 0; i < table.Rows.Count; i++)
            {
                DeptItem info = new DeptItem();
                //info.DeptId = table.Rows[i][0].ToString();
                info.DeptName = table.Rows[i][1].ToString();
                //info.ItemId = table.Rows[i][2].ToString();
                info.ItemName = table.Rows[i][3].ToString();
                info.Task = table.Rows[i][4].ToString();
                list.Add(info);
            }

            //得到行和列标题 及数量            
            string[] DeptNames = list.Select(t => t.DeptName).Distinct().ToArray();
            string[] ItemNames = list.Select(t => t.ItemName).Distinct().ToArray();

            //创建DataTable
            System.Data.DataTable tabledisplay = new System.Data.DataTable();

            //表中第一行第一列交叉处一般显示为第1列标题
            //tabledisplay.Columns.Add(new DataColumn("序号"));
            //MyColumns.Add("序号", new MyColumn("序号", "序号") { BShow = true, Width = 10 });
            switch (user_flag_tier)
            {
                case "0": tabledisplay.Columns.Add(new DataColumn("省名称"));
                    MyColumns.Add("省名称", new MyColumn("省名称", "省名称") { BShow = true, Width = 20 });
                    break;
                case "1": tabledisplay.Columns.Add(new DataColumn("市(州)单位名称"));
                    MyColumns.Add("市(州)单位名称", new MyColumn("市(州)单位名称", "市(州)单位名称") { BShow = true, Width = 20 });
                    break;
                case "2": tabledisplay.Columns.Add(new DataColumn("区县名称"));
                    MyColumns.Add("区县名称", new MyColumn("区县名称", "区县名称") { BShow = true, Width = 20 });
                    break;
                case "3": tabledisplay.Columns.Add(new DataColumn("检测单位名称"));
                    MyColumns.Add("检测单位名称", new MyColumn("检测单位名称", "检测单位名称") { BShow = true, Width = 20 });
                    break;
                case "4": tabledisplay.Columns.Add(new DataColumn("检测单位名称"));
                    MyColumns.Add("检测单位名称", new MyColumn("检测单位名称", "检测单位名称") { BShow = true, Width = 20 });
                    break;
                default: break;
            }

            //表中后面每列的标题其实是列分组的关键字
            for (int i = 0; i < ItemNames.Length; i++)
            {
                DataColumn column = new DataColumn(ItemNames[i]);
                tabledisplay.Columns.Add(column);
                MyColumns.Add(ItemNames[i], new MyColumn(ItemNames[i], ItemNames[i]) { BShow = true, Width = 15 });
            }

            //为表中各行生成数据
            for (int i = 0; i < DeptNames.Length; i++)
            {
                var row = tabledisplay.NewRow();
                //每行第0列为行分组关键字
                //row[0] = i + 1;
                row[0] = DeptNames[i];
                //每行的其余列为行列交叉对应的汇总数据
                for (int j = 0; j < ItemNames.Length; j++)
                {
                    string num = list.Where(t => t.DeptName == DeptNames[i] && t.ItemName == ItemNames[j]).Select(t => t.Task).FirstOrDefault();
                    if (num == null || num == "")
                    {
                        num = '0'.ToString();
                    }
                    row[ItemNames[j]] = num;
                }

                tabledisplay.Rows.Add(row);
            }

            if (table.Rows.Count != 0)
            {
                //表格最后添加合计行
                tabledisplay.Rows.Add(tabledisplay.NewRow()[1] = "合计");
                for (int j = 1; j < tabledisplay.Columns.Count; j++)
                {
                    int sum = 0;
                    for (int i = 0; i < tabledisplay.Rows.Count - 1; i++)
                    {
                        sum += Convert.ToInt32(tabledisplay.Rows[i][j].ToString());
                    }
                    //sum_column += sum;
                    tabledisplay.Rows[tabledisplay.Rows.Count - 1][j] = sum;
                }

                System.Data.DataTable tasktable = dbOperation.GetDbHelper().GetDataSet("select t_det_item.ItemNAME,task " +
                                      "from t_task_assign_new left JOIN t_det_item ON t_task_assign_new.iid = t_det_item.ItemID " +
                                      "where t_task_assign_new.did = " + deptid).Tables[0];

                List<ItemTask> listtask = new List<ItemTask>();
                listtask.Clear();
                for (int i = 0; i < tasktable.Rows.Count; i++)
                {
                    ItemTask task = new ItemTask();
                    task.ItemName = tasktable.Rows[i][0].ToString();
                    task.Task = tasktable.Rows[i][1].ToString();
                    listtask.Add(task);
                }

                if (user_flag_tier != "1")
                {
                    //表格最后添加上级分配任务量
                    tabledisplay.Rows.Add(tabledisplay.NewRow()[1] = "上级下达任务量");
                    for (int j = 1; j < tabledisplay.Columns.Count; j++)
                    {
                        string task = listtask.Where(s => s.ItemName == tabledisplay.Columns[j].ColumnName.ToString()).Select(s => s.Task).FirstOrDefault();
                        if (task == null || task == "")
                        {
                            task = '0'.ToString();
                        }

                        tabledisplay.Rows[tabledisplay.Rows.Count - 1][j] = task;
                    }

                    //表格最后添加未分配量
                    tabledisplay.Rows.Add(tabledisplay.NewRow()[1] = "未分配量");

                    for (int j = 1; j < tabledisplay.Columns.Count; j++)
                    {
                        int rwl = Convert.ToInt32(tabledisplay.Rows[tabledisplay.Rows.Count - 2][j].ToString());
                        int yfp = Convert.ToInt32(tabledisplay.Rows[tabledisplay.Rows.Count - 3][j].ToString());
                        int wfp = rwl - yfp;
                        tabledisplay.Rows[tabledisplay.Rows.Count - 1][j] = wfp;
                    }
                }
            }

            _tableview.BShowModify = true;
            _tableview.MyColumns = MyColumns;
            _tableview.Table = tabledisplay;
        }

        void _tableview_ModifyRowEnvent(string id)
        {
            string dept_id;

            DataRow[] rows = currenttable.Select("PART_NAME = '" + id + "'");
            dept_id = rows[0]["PART_ID"].ToString();

            SetTask sam = new SetTask(dbOperation, dept_id, id, this);
            sam.ShowDialog();
        }

        public class DeptItem
        {
            //public string DeptId { get; set; }

            public string DeptName { get; set; }

            //public string ItemId { get; set; }

            public string ItemName { get; set; }

            public string Task { get; set; }
        }
        public class ItemTask
        {
            public string ItemName { get; set; }

            public string Task { get; set; }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            System.Data.DataTable dt = LoadExcel("D:\\111"); //通过路径获取到的数据  

            //此时我们就可以用这数据进行处理了，比如绑定到显示数据的控件当中去  
            MessageBox.Show("导入成功");
        }

        //获取表格中的数据  
        public System.Data.DataTable LoadExcel(string pPath)
        {

            string connString = "Driver={Driver do Microsoft Excel(*.xls)};DriverId=790;SafeTransactions=0;ReadOnly=1;MaxScanRows=16;Threads=3;MaxBufferSize=2024;UserCommitSync=Yes;FIL=excel 8.0;PageTimeout=5;";  //连接字符串    

            //简单解释下这个连续字符串，Driver={Driver do Microsoft Excel(*.xls)} 这种连接写法不需要创建一个数据源DSN，DRIVERID表示驱动ID，Excel2003后都使用790，  

            //FIL表示Excel文件类型，Excel2007用excel 8.0，MaxBufferSize表示缓存大小， 如果你的文件是2010版本的，也许会报错，所以要找到合适版本的参数设置。  

            connString += "DBQ=" + pPath; //DBQ表示读取Excel的文件名（全路径）  
            OdbcConnection conn = new OdbcConnection(connString);
            OdbcCommand cmd = new OdbcCommand();
            cmd.Connection = conn;
            //获取Excel中第一个Sheet名称，作为查询时的表名  
            string sheetName = this.GetExcelSheetName(pPath);
            string sql = "select * from [" + sheetName.Replace('.', '#') + "$]";
            cmd.CommandText = sql;
            OdbcDataAdapter da = new OdbcDataAdapter(cmd);
            DataSet ds = new DataSet();
            try
            {
                da.Fill(ds);
                return ds.Tables[0];    //返回Excel数据中的内容，保存在DataTable中  
            }
            catch (Exception x)
            {
                ds = null;
                throw new Exception("从Excel文件中获取数据时发生错误！可能是Excel版本问题，可以考虑降低版本或者修改连接字符串值");
            }
            finally
            {
                cmd.Dispose();
                cmd = null;
                da.Dispose();
                da = null;
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
                conn = null;
            }
        }

        // 获取工作表名称  
        private string GetExcelSheetName(string pPath)
        {
            //打开一个Excel应用  
            Microsoft.Office.Interop.Excel.Application excelApp;
            Workbook excelWB;//创建工作簿（WorkBook：即Excel文件主体本身）  
            Workbooks excelWBs;
            Worksheet excelWS;//创建工作表（即Excel里的子表sheet）  

            Sheets excelSts;

            excelApp = new Microsoft.Office.Interop.Excel.Application();
            if (excelApp == null)
            {
                throw new Exception("打开Excel应用时发生错误！");
            }
            excelWBs = excelApp.Workbooks;
            //打开一个现有的工作薄  
            excelWB = excelWBs.Add(pPath);
            excelSts = excelWB.Sheets;
            //选择第一个Sheet页  
            //excelWS = excelSts.get_Item(1);
            string sheetName = "111";

            //ReleaseCOM(excelWS);
            ReleaseCOM(excelSts);
            ReleaseCOM(excelWB);
            ReleaseCOM(excelWBs);
            excelApp.Quit();
            ReleaseCOM(excelApp);
            return sheetName;
        }

        // 释放资源  
        private void ReleaseCOM(object pObj)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(pObj);
            }
            catch
            {
                throw new Exception("释放资源时发生错误！");
            }
            finally
            {
                pObj = null;
            }
        } 
    }
}
