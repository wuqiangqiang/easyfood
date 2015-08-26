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
    /// UcSetSamplingRate.xaml 的交互逻辑
    /// </summary>
    public partial class UcSetTask : UserControl
    {
        private IDBOperation dbOperation;
        private DataTable currenttable;
        private string user_flag_tier;
        private string deptid;
        private List<DeptItem> list = new List<DeptItem>();

        public UcSetTask(IDBOperation dbOperation)
        {
            InitializeComponent();
            this.dbOperation = dbOperation;
            user_flag_tier = (Application.Current.Resources["User"] as UserInfo).FlagTier;
            deptid = (Application.Current.Resources["User"] as UserInfo).DepartmentID;

            _tableview.ModifyRowEnvent += new UcTableOperableView_NoPages.ModifyRowEventHandler(_tableview_ModifyRowEnvent);
            Load_table();
        }

        public void Load_table()
        {
            Dictionary<string, MyColumn> MyColumns = new Dictionary<string, MyColumn>();

            DataTable table = dbOperation.GetDbHelper().GetDataSet(string.Format("call p_task_details({0})",
                              (Application.Current.Resources["User"] as UserInfo).ID)).Tables[0];

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
            DataTable tabledisplay = new DataTable();

            //表中第一行第一列交叉处一般显示为第1列标题
            //tabledisplay.Columns.Add(new DataColumn("序号"));
            //MyColumns.Add("序号", new MyColumn("序号", "序号") { BShow = true, Width = 10 });
            switch (user_flag_tier)
            {
                case "0": tabledisplay.Columns.Add(new DataColumn("省名称"));
                    MyColumns.Add("省名称", new MyColumn("省名称", "省名称") { BShow = true, Width = 20 });
                    break;
                case "1": tabledisplay.Columns.Add(new DataColumn("市(州)名称"));
                    MyColumns.Add("市(州)名称", new MyColumn("市(州)名称", "市(州)名称") { BShow = true, Width = 20 });
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

                DataTable tasktable = dbOperation.GetDbHelper().GetDataSet("select t_det_item.ItemNAME,task "+
                                      "from t_task_assign left JOIN t_det_item ON t_task_assign.iid = t_det_item.ItemID "+
                                      "where t_task_assign.did = " + deptid).Tables[0];

                List<ItemTask> listtask = new List<ItemTask>();
                listtask.Clear();
                for (int i = 0; i < tasktable.Rows.Count; i++)
                {
                    ItemTask task = new ItemTask();
                    task.ItemName = tasktable.Rows[i][0].ToString();
                    task.Task = tasktable.Rows[i][1].ToString();
                    listtask.Add(task);
                }

                //表格最后添加上级分配任务量
                tabledisplay.Rows.Add(tabledisplay.NewRow()[1] = "上级分配任务量");
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
    }
}
