﻿using System;
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
using Toolkit = Microsoft.Windows.Controls;
using DBUtility;
using System.Data;
using FoodSafetyMonitoring.Common;
using System.ComponentModel;
using System.Collections.ObjectModel;
using FoodSafetyMonitoring.dao;
using System.Data.Odbc;
using Microsoft.Office.Interop.Excel;
using System.Windows.Forms;
 

namespace FoodSafetyMonitoring.Manager
{
    /// <summary>
    /// Test.xaml 的交互逻辑
    /// </summary>
    public partial class SysDeptManager : System.Windows.Controls.UserControl, IClickChildMenuInitUserControlUI
    {
        FamilyTreeViewModel departmentViewModel;
        private IDBOperation dbOperation;

        readonly Dictionary<string, string> cityLevelDictionary = new Dictionary<string, string>() { { "0", "国家" }, { "1", "省级" }, { "2", "市(州)" }, { "3", "区县" }, { "4", "检测单位" } };
        private Department department;
        private System.Data.DataTable ProvinceCityTable = null;
        private string user_flag_tier;
        private System.Data.DataTable SupplierTable;
        Importing_window load;

        public SysDeptManager(IDBOperation dbOperation)
        {
            InitializeComponent();
            this.dbOperation = dbOperation;
            ProvinceCityTable = System.Windows.Application.Current.Resources["省市表"] as System.Data.DataTable;
            user_flag_tier = (System.Windows.Application.Current.Resources["User"] as UserInfo).FlagTier.ToString();
            SupplierTable = dbOperation.GetDbHelper().GetDataSet("select supplierId,supplierName from t_supplier").Tables[0];
            InitUserControlUI();
        }

        #region IClickChildMenuInitUserControlUI 成员

        private int flag_init = 0;//初始化,0未初始化,1已初始化

        public void InitUserControlUI()
        {
            if (flag_init == 1)
            {
                return;
            }
            flag_init = 1;

            System.Data.DataTable table = dbOperation.GetDepartment();
            if (table != null)
            {
                department = new Department();
                //DataRow[] rows = table.Select("FK_CODE_DEPT='0'");
                //if (rows.Length == 0)
                //{
                //    return;
                //}
                DataRow[] rows = table.Select();
                department.Name = rows[0]["INFO_NAME"].ToString();
                department.Row = rows[0];
                //对应湖北省级有3个部门（101 湖北畜安处，102 湖北动监处，103 湖北屠宰办），数据库中存在下级部门的是102
                string deptId = "";
                if (rows[0]["INFO_CODE"].ToString() == "101" || rows[0]["INFO_CODE"].ToString() == "103")
                {
                    deptId = "102";
                }
                else
                {
                    deptId = rows[0]["INFO_CODE"].ToString();
                }
                rows = table.Select("FK_CODE_DEPT='" + deptId + "'");
                foreach (DataRow row1 in rows)
                {
                    Department department1 = new Department();
                    department1.Parent = department;
                    department1.Row = row1;
                    department1.Name = row1["INFO_NAME"].ToString();
                    rows = table.Select("FK_CODE_DEPT='" + row1["INFO_CODE"].ToString() + "'");
                    foreach (DataRow row2 in rows)
                    {
                        Department department2 = new Department();
                        department2.Parent = department1;
                        department2.Row = row2;
                        department2.Name = row2["INFO_NAME"].ToString();
                        rows = table.Select("FK_CODE_DEPT='" + row2["INFO_CODE"].ToString() + "'");
                        foreach (DataRow row3 in rows)
                        {
                            Department department3 = new Department();
                            department3.Parent = department2;
                            department3.Row = row3;
                            department3.Name = row3["INFO_NAME"].ToString();
                            rows = table.Select("FK_CODE_DEPT='" + row3["INFO_CODE"].ToString() + "'");
                            foreach (DataRow row4 in rows)
                            {
                                Department department4 = new Department();
                                department4.Parent = department3;
                                department4.Row = row4;
                                department4.Name = row4["INFO_NAME"].ToString();
                                department3.Children.Add(department4);
                            }
                            department2.Children.Add(department3);
                        }
                        department1.Children.Add(department2);
                    }
                    department.Children.Add(department1);
                }

                departmentViewModel = new FamilyTreeViewModel(department);
                _treeView.DataContext = departmentViewModel;
            }
        }

        #endregion

        private void _detect_method1_Checked(object sender, RoutedEventArgs e)
        {
            if ((sender as System.Windows.Controls.CheckBox).Name == "_direct_station")
            {
                _direct_station_2.IsChecked = false;
                _cultivate_station.IsChecked = false;
                _slaughter_station.IsChecked = false;
            }
            else if ((sender as System.Windows.Controls.CheckBox).Name == "_direct_station_2")
            {
                _direct_station.IsChecked = false;
                _cultivate_station.IsChecked = false;
                _slaughter_station.IsChecked = false;
            }
            else if ((sender as System.Windows.Controls.CheckBox).Name == "_cultivate_station")
            {
                _direct_station_2.IsChecked = false;
                _direct_station.IsChecked = false;
                _slaughter_station.IsChecked = false;
            }
            else if ((sender as System.Windows.Controls.CheckBox).Name == "_slaughter_station")
            {
                _direct_station_2.IsChecked = false;
                _direct_station.IsChecked = false;
                _cultivate_station.IsChecked = false;
            }
        }


        //保存按钮
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            Department department = _add.Tag as Department;

            //新增
            if (state == "add")
            {
                System.Data.DataTable dt = new System.Data.DataTable();
                DataRow row = department.Row.Table.NewRow();
                row.ItemArray = (object[])department.Row.ItemArray.Clone();
                row["FK_CODE_DEPT"] = row["INFO_CODE"];

                int maxID = 0;

                if (department.Children.Count == 0)
                {
                    maxID = Convert.ToInt32(row["INFO_CODE"].ToString() + "01");
                    row["INFO_CODE"] = maxID;
                }
                else
                {
                    for (int i = 0; i < department.Children.Count; i++)
                    {
                        int v = Convert.ToInt32(department.Children[i].Row["INFO_CODE"].ToString());
                        if (maxID < v)
                        {
                            maxID = v;
                        }
                    }
                    row["INFO_CODE"] = maxID + 1;
                }

                row["INFO_NAME"] = _station.Text;
                row["FLAG_TIER"] = (Convert.ToInt32(department.Row["FLAG_TIER"].ToString()) + 1);
                if (_lower_area.SelectedIndex == -1 && row["FLAG_TIER"].ToString() != "4")
                {
                    Toolkit.MessageBox.Show("请选择所在地!", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                if (_station.Text == "")
                {
                    if (row["FLAG_TIER"].ToString() == "4")
                    {
                        Toolkit.MessageBox.Show("请输入检测单位名称!", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                        return;
                    }
                    else
                    {
                        Toolkit.MessageBox.Show("请输入部门名称!", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                        return;
                    }
                }

                if (user_flag_tier == "0")
                {
                    if (_Supplier.SelectedIndex < 1)
                    {
                        Toolkit.MessageBox.Show("请选择供应商!", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                        return;
                    }
                }

                //保存是否直属信息
                if(_city_flag.Text == "是")
                {
                    row["isdept"] = "1";
                }
                else
                {
                    row["isdept"] = "0";
                }

                string lower_area_id = "";

                //根据当前部门的级别来赋省，市，区的值
                switch (row["FLAG_TIER"].ToString())
                {
                    case "0": row["Province"] = ""; 
                              row["City"] = ""; 
                              row["Country"] = ""; 
                              break;
                    case "1": if (_lower_area.Text != "")
                              {
                                  lower_area_id = ProvinceCityTable.Select("name='" + _lower_area.Text + "'")[0]["id"].ToString();
                              }
                              row["Province"] = lower_area_id; 
                              row["City"] = ""; 
                              row["Country"] = ""; 
                              break;
                    case "2": if (_lower_area.Text != "")
                              {
                                  lower_area_id = ProvinceCityTable.Select("name='" + _lower_area.Text + "' and pid = '" + department.Row["Province"] + "'")[0]["id"].ToString();
                              }
                              row["Province"] = department.Row["Province"].ToString(); 
                              row["City"] = lower_area_id; 
                              row["Country"] = ""; 
                              break;
                    case "3": if (_lower_area.Text != "")
                              {
                                  lower_area_id = ProvinceCityTable.Select("name='" + _lower_area.Text + "' and pid = '" + department.Row["City"] + "'")[0]["id"].ToString();
                              }
                              row["Province"] = department.Row["Province"].ToString(); 
                              row["City"] = department.Row["City"].ToString();
                              row["Country"] = lower_area_id;
                              break;
                    case "4": row["Province"] = department.Row["Province"].ToString(); 
                              row["City"] = department.Row["City"].ToString();
                              row["Country"] = department.Row["Country"].ToString();
                              break;
                    default: break;
                }
                row["INFO_NAME"] = _station.Text;
                row["address"] = _address.Text;
                row["CONTACTER"] = _principal_name.Text;
                row["tel"] = _phone.Text;
                row["phone"] = _contact_number.Text;
                row["supplierId"] = (_Supplier.SelectedItem as System.Windows.Controls.Label).Tag.ToString();
                //row["title"] = _title.Text;
                //row["INFO_NOTE"] = _note.Text;

                //获取画面上的检测点类型
                string type = "";
                if (_direct_station.IsChecked == true)
                {
                    row["type"] = "2";
                    type = "2";
                }
                else if (_cultivate_station.IsChecked == true)
                {
                    row["type"] = "1";
                    type = "1";
                }
                else if (_slaughter_station.IsChecked == true)
                {
                    row["type"] = "0";
                    type = "0";
                }
                else if (_direct_station_2.IsChecked == true)
                {
                    row["type"] = "3";
                    type = "3";
                }

                if (row["type"].ToString() == ""   && row["FLAG_TIER"].ToString() == "4")
                {
                    Toolkit.MessageBox.Show("请选择检测点性质!", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                Department newDepartment = new Department();
                newDepartment.Parent = department;
                newDepartment.Name = _station.Text;
                newDepartment.Row = row;

                string sql = String.Format("insert into sys_client_sysdept (INFO_CODE,INFO_NAME,FLAG_TIER,FK_CODE_DEPT,PROVINCE,CITY,COUNTRY,ADDRESS,CONTACTER,TEL,PHONE,TYPE,supplierId,isdept) values " +
                    "('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}');"
              , row["INFO_CODE"], row["INFO_NAME"], row["FLAG_TIER"], row["FK_CODE_DEPT"]
              , row["PROVINCE"], row["CITY"], row["COUNTRY"], row["ADDRESS"], row["CONTACTER"], row["TEL"], row["PHONE"], row["TYPE"], row["supplierId"], row["isdept"]);

                try
                {
                    int count = dbOperation.GetDbHelper().ExecuteSql(sql);
                    if (count == 1)
                    {
                        ////如果是养殖场类型的部门，把信息插入t_company表中
                        //if(row["TYPE"].ToString() == "1")
                        //{
                        //    int n = dbOperation.GetDbHelper().ExecuteSql(string.Format("INSERT INTO t_company (COMPANYNAME,AREAID,OPENFLAG,cuserid,cdate,sysdeptid) VALUES('{0}','{1}','{2}','{3}','{4}','{5}')",
                        //                                          row["INFO_NAME"],row["COUNTRY"],'1',(Application.Current.Resources["User"] as UserInfo).ID,
                        //                                          DateTime.Now, row["INFO_CODE"]));
                        //    if (n != 1)
                        //    {
                        //        Toolkit.MessageBox.Show("被检单位添加失败！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                        //        return;
                        //    }
                        //}
                        Toolkit.MessageBox.Show("保存成功！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                        Common.SysLogEntry.WriteLog("部门管理", (System.Windows.Application.Current.Resources["User"] as UserInfo).ShowName, Common.OperationType.Modify, "新增部门信息");
                    }
                    else
                    {
                        Toolkit.MessageBox.Show("保存失败！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                        return;
                    }
                }
                catch (Exception ee)
                {
                    System.Diagnostics.Debug.WriteLine("SysDeptManager.btnSave_Click" + ee.Message);
                    Toolkit.MessageBox.Show("数据更新失败!稍后尝试!");
                    return;
                }
                state = "view";

                department.Children.Add(newDepartment);
                departmentViewModel = new FamilyTreeViewModel(this.department);

                departmentViewModel.SearchText = _station.Text;
                departmentViewModel.SearchCommand.Execute(null);
                _add.Tag = newDepartment;
                _import.Tag = newDepartment;
                _edit.Tag = newDepartment;



            }//修改
            else if (state == "edit")
            {
                //获取画面上的检测点类型
                string type = "";
                if (_direct_station.IsChecked == true)
                {
                    type = "2";
                }
                else if (_cultivate_station.IsChecked == true)
                {
                    type = "1";
                }
                else if (_slaughter_station.IsChecked == true)
                {
                    type = "0";
                }
                else if (_direct_station_2.IsChecked == true)
                {
                    type = "3";
                }

                //保存是否直属信息
                string is_dept = "";
                if (_city_flag.Text == "是")
                {
                    is_dept = "1";
                }
                else
                {
                    is_dept = "0";
                }

                string sql = String.Format("UPDATE sys_client_sysdept set INFO_NAME='{0}',ADDRESS='{1}',CONTACTER='{2}',TEL='{3}',PHONE='{4}',TYPE='{5}',supplierId = '{6}',isdept = '{7}'  where INFO_CODE='{8}';"
                , _station.Text, _address.Text, _principal_name.Text, _phone.Text, _contact_number.Text, type, (_Supplier.SelectedItem as System.Windows.Controls.Label).Tag, is_dept, department.Row["INFO_CODE"]);

                try
                {
                    int count = dbOperation.GetDbHelper().ExecuteSql(sql);
                    if (count == 1)
                    {
                        ////如果是养殖场类型的部门，把修改的部门名称保存进t_company表中
                        //if (type == "1")
                        //{
                        //    int n = dbOperation.GetDbHelper().ExecuteSql(string.Format("update t_company set COMPANYNAME = '{0}' where sysdeptid = '{1}'",
                        //                                                  _station.Text,department.Row["INFO_CODE"]));
                        //    if (n != 1)
                        //    {
                        //        Toolkit.MessageBox.Show("被检单位修改失败！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                        //        return;
                        //    }
                        //}

                        Toolkit.MessageBox.Show("保存成功！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                        Common.SysLogEntry.WriteLog("部门管理", (System.Windows.Application.Current.Resources["User"] as UserInfo).ShowName, Common.OperationType.Modify, "修改部门信息");
                    }
                    else
                    {
                        Toolkit.MessageBox.Show("保存失败！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                }
                catch (Exception ee)
                {
                    System.Diagnostics.Debug.WriteLine("SysDeptManager.btnSave_Click" + ee.Message);
                      Toolkit.MessageBox.Show("数据更新失败!稍后尝试!");
                    return;
                }
                state = "view";

                //更新树形表
                department.Row["INFO_NAME"] = _station.Text;
                department.Name = _station.Text;
                department.Row["address"] = _address.Text;
                department.Row["contacter"] = _principal_name.Text;
                department.Row["tel"] = _phone.Text;
                department.Row["phone"] = _contact_number.Text;
                department.Row["supplierId"] = (_Supplier.SelectedItem as System.Windows.Controls.Label).Tag.ToString();
                department.Row["isdept"] = is_dept;
                department.Row["type"] = type;
                _edit.IsEnabled = true;


            }
            else
            {
                return;
            }

            _treeView.DataContext = null;
            _treeView.DataContext = departmentViewModel;
            _detail_info.IsEnabled = false;
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            _detail_info.IsEnabled = false;
            _add.IsEnabled = true;
            _import.IsEnabled = true;
            _edit.IsEnabled = true; 
            //刷新部门详细信息
            Department department = _add.Tag as Department;
            DataRow row = department.Row;
            load_DeptDetails(row);
        }

        //private void txtSearch_ImageClick(object sender, EventArgs e)
        //{
        //    departmentViewModel.SearchText = txtSearch.Text;
        //    departmentViewModel.SearchCommand.Execute(null);
        //}

        //根据代码获取所在地和下级城市
        private void GetCityByCode(string flag_tier, string province, string city, string country, ref string name, ref List<string> citys)
        {
            switch (flag_tier)
            {
                case "0": name = ""; citys = GetList("0001","0"); break;
                case "1": name = GetName(province); citys = GetList(province, "1"); break;
                case "2": name = GetName(province) + GetName(city); citys = GetList(city,"2"); break;
                case "3": name = GetName(province) + GetName(city) + GetName(country); citys = new List<string>(); break;
                case "4": name = GetName(province) + GetName(city) + GetName(country); citys = new List<string>(); break;
                default: break;
            }
        }

        //根据代码获取当前城市名
        private string GetName(string code)
        {
            DataRow[] rows = ProvinceCityTable.Select("id='" + code + "'");
            if (rows.Length > 0)
            {
                return rows[0]["name"].ToString();
            }
            else
            {
                return "";
            }
        }

        //根据代码获取下级所有城市
        private List<string> GetList(string code,string dept_flag)
        {
            List<string> list = new List<string>();
            //DataRow[] rows = ProvinceCityTable.Select("pid='" + code + "'");
            System.Data.DataTable table = dbOperation.GetDbHelper().GetDataSet(string.Format("call p_area('{0}','{1}')", code, dept_flag)).Tables[0];
            DataRow[] rows = table.Select(); 
            foreach (DataRow row in rows)
            {
                list.Add(row["name"].ToString());
            }
            return list;
        }

        private string state = "view";
        private TextBlock text_treeView = null;

        //省市上双击事件
        private void TextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount > 0)
            {
                text_treeView = sender as TextBlock;
                Department department = (sender as TextBlock).Tag as Department;
                _add.Tag = department;
                _import.Tag = department;
                _edit.Tag = department;
                DataRow row = department.Row;

                load_DeptDetails(row);
            }
        }

        private void load_DeptDetails(DataRow row)
        {
            _contact_number.Text = "";
            _phone.Text = "";
            _principal_name.Text = "";
            _address.Text = "";
            _area_flag.Text = "";
            _station_flag.Text = "";
            _station_property_flag.Text = "";

            _detail_info_all.Visibility = Visibility.Visible;
            _detail_info.IsEnabled = false;
            _regional_level.Tag = row["FLAG_TIER"].ToString();
            _regional_level.Text = cityLevelDictionary[row["FLAG_TIER"].ToString()];
            _station_property.Visibility = Visibility.Hidden;
            //隐藏是否直属
            _is_dept.Visibility = Visibility.Hidden;
            _city_flag.Visibility = Visibility.Hidden;
            _lower_area.Visibility = Visibility.Hidden;
            _edit.IsEnabled = true;
            _add.IsEnabled = true;
            _import.IsEnabled = true;
            
            if (row["FLAG_TIER"].ToString() == "4")
            {
                _station_name.Text = "检测单位名称:";
            }
            else
            {
                _station_name.Text = "部门名称:";
            }

            //部门放开后，不可再用此控制
            //if (user_flag_tier == row["FLAG_TIER"].ToString())
            //{
            //    _add.Visibility = Visibility.Visible;
            //    _delete.Visibility = Visibility.Hidden;
            //    _edit.Visibility = Visibility.Visible;
            //}
            //else
            //{
            //    _add.Visibility = Visibility.Hidden;
            //    _delete.Visibility = Visibility.Visible;
            //    _edit.Visibility = Visibility.Visible;
            //}

            _add.Visibility = Visibility.Visible;
            _import.Visibility = Visibility.Visible;
            _edit.Visibility = Visibility.Visible;
            //如果存在下级部门则不出现删除按钮，否则出现
            bool result3 = dbOperation.GetDbHelper().Exists(string.Format("select count(INFO_CODE) from sys_client_sysdept where FK_CODE_DEPT = '{0}'", row["INFO_CODE"].ToString()));
            if (result3)
            {
                _delete.Visibility = Visibility.Hidden;
            }
            else
            {
                _delete.Visibility = Visibility.Visible;
            }
            
            if (_regional_level.Text == "检测单位")
            {
                _add.Visibility = Visibility.Hidden;
                _import.Visibility = Visibility.Hidden;
                _station_property.Visibility = Visibility.Visible;
                //显示是否直属
                _is_dept.Visibility = Visibility.Visible;
                _city_flag.Visibility = Visibility.Visible;
            }

            if (user_flag_tier == "0" && row["FLAG_TIER"].ToString() != "0")
            {
                _Supplier_name.Visibility = Visibility.Visible;
                _Supplier.Visibility = Visibility.Visible;
            }
            else
            {
                _Supplier_name.Visibility = Visibility.Hidden;
                _Supplier.Visibility = Visibility.Hidden;
            }

            ComboboxTool.InitComboboxSource(_Supplier, "select supplierId,supplierName from t_supplier", "lr");
            DataRow[] rows = SupplierTable.Select("supplierId = '" + row["supplierId"] + "'");
            if (rows.Length > 0)
            {
                _Supplier.Text = rows[0]["supplierName"].ToString();
            }

            string city = "";
            List<string> citys = null;
            GetCityByCode(row["FLAG_TIER"].ToString(), row["province"].ToString(), row["city"].ToString(), row["country"].ToString(), ref city, ref citys);

            _belong_to.Text = city;
            _lower_area.ItemsSource = citys;
            _lower_area.SelectedIndex = -1;

            //下级单位全部均已添加完，则不出现添加下级按钮
            if (_lower_area.Items.Count == 0 && row["FLAG_TIER"].ToString() != "3")
            {
                _add.Visibility = Visibility.Hidden;
                _import.Visibility = Visibility.Hidden;
            }

            _phone.Text = row["tel"].ToString();
            _contact_number.Text = row["phone"].ToString();
            _address.Text = row["address"].ToString();
            _principal_name.Text = row["contacter"].ToString();
            _station.Text = row["INFO_NAME"].ToString();

            //是否直属
            if (row["isdept"].ToString() == "0")
            {
                _city_flag.SelectedIndex = 1;
            }
            else
            {
                _city_flag.SelectedIndex = 0;
            }

            if (department.Parent != null)
            {
                _superior_department.Text = department.Parent.Row["INFO_NAME"].ToString();
            }
            else
            {
                if (row["FLAG_TIER"].ToString() == "0")
                {
                    _superior_department.Text = "无";
                }
                else
                {
                    _superior_department.Text = dbOperation.GetDbHelper().GetSingle(string.Format("select INFO_NAME FROM sys_client_sysdept WHERE INFO_CODE = '{0}'", row["FK_CODE_DEPT"].ToString())).ToString();
                }
            }

            _direct_station.IsChecked = false;
            _cultivate_station.IsChecked = false;
            _slaughter_station.IsChecked = false;
            _direct_station_2.IsChecked = false;
            if (row["type"].ToString().Length != 0)
            {
                if (Convert.ToInt32(row["type"].ToString()) == 2)
                {
                    _direct_station.IsChecked = true;
                }
                else if (Convert.ToInt32(row["type"].ToString()) == 1)
                {
                    _cultivate_station.IsChecked = true;
                }
                else if (Convert.ToInt32(row["type"].ToString()) == 0)
                {
                    _slaughter_station.IsChecked = true;
                }
                else if (Convert.ToInt32(row["type"].ToString()) == 3)
                {
                    _direct_station_2.IsChecked = true;
                }
            }

            //_delete.Visibility = Visibility.Hidden;
            //if (department.Children.Count == 0)
            //{
            //    _delete.Visibility = Visibility.Visible;
            //}
        }

        private void _edit_Click(object sender, RoutedEventArgs e)
        {
            _detail_info.IsEnabled = true;
            //_station_property.IsEnabled = false;
            state = "edit";
            _edit.IsEnabled = false;
            _station_flag.Text = "(必填)";
            _station_property_flag.Text = "(必填)";

            Department department = _edit.Tag as Department;
            //对应湖北省级有3个部门（101 湖北畜安处，102 湖北动监处，103 湖北屠宰办），数据库中存在下级部门的是102
            string deptId = "";
            if (department.Row["INFO_CODE"].ToString() == "101" || department.Row["INFO_CODE"].ToString() == "103")
            {
                deptId = "102";
            }
            else
            {
                deptId = department.Row["INFO_CODE"].ToString();
            }
            bool result = dbOperation.GetDbHelper().Exists(string.Format("select count(INFO_CODE) from sys_client_sysdept where FK_CODE_DEPT ='{0}'", deptId));
            if (result)
            {
                _Supplier.IsEnabled = false;
            }
            else
            {
                _Supplier.IsEnabled = true;
            }

            //修改的话，所在地下拉框显示
            //_lower_area.Visibility = Visibility.Visible;
            //_belong_to.Visibility = Visibility.Hidden;
        }

        private void _add_Click(object sender, RoutedEventArgs e)
        {
            if (_station.Text.Length == 0)
            {
                Toolkit.MessageBox.Show("检测点名称不能为空!");
                return;
            }
            state = "add";

            _add.IsEnabled = false;
            _add.Visibility = Visibility.Hidden;
            _import.Visibility = Visibility.Hidden;
            _edit.Visibility = Visibility.Hidden;
            _detail_info.IsEnabled = true;
            _station_property.IsEnabled = true;
            Department department = _add.Tag as Department;

            if (user_flag_tier == "0")
            {
                _Supplier_name.Visibility = Visibility.Visible;
                _Supplier.Visibility = Visibility.Visible;
                _Supplier.IsEnabled = true;
                ComboboxTool.InitComboboxSource(_Supplier, "select supplierId,supplierName from t_supplier", "lr");
            }

            string regional_level = (Convert.ToInt32(_regional_level.Tag.ToString()) + 1).ToString();
            _regional_level.Text = cityLevelDictionary[regional_level];
            _regional_level.Tag = regional_level;

            _station_flag.Text = "(必填)";
            
            //如果当前添加的是检测单位，则显示检测点性质信息
            if (_regional_level.Text == "检测单位")
            {
                
                _station_property.Visibility = Visibility.Visible;
                _station_property_flag.Text = "(必填)";
                //显示是否直属
                _is_dept.Visibility = Visibility.Visible;
                _city_flag.Visibility = Visibility.Visible;
                _city_flag.SelectedIndex = 1;
            }
            else
            {
                _area_flag.Text = "(必填)";
            }

            //判断所在地下拉有无值，如果没有值，则显示文本框不显示下拉框；有值则显示下拉框不显示文本框
            if (_lower_area.Items.Count == 0)
            {
                _lower_area.Visibility = Visibility.Hidden;
                _belong_to.Visibility = Visibility.Visible;
                _area_flag.Text = "";
            }
            else
            {
                _lower_area.Visibility = Visibility.Visible;
                //_belong_to.Visibility = Visibility.Hidden;
                //_belong_to.Text = "";
            }

            if (_regional_level.Text == "检测单位")
            {
                _station_name.Text = "检测单位名称:";
            }
            else
            {
                _station_name.Text = "部门名称:";
            }

            //新增部门，画面上字段进行初始化
            _superior_department.Text = _station.Text;
            _station.Text = "";
            _principal_name.Text = "";
            _phone.Text = "";
            _contact_number.Text = "";
            _address.Text = "";

        }

        private void _import_Click(object sender, RoutedEventArgs e)
        {
           System.Data.DataTable importdt = new System.Data.DataTable();
           importdt = GetDataFromExcelByCom();
           if (importdt != null)
           {
               if (importdt.Rows.Count != 0)
               {
                   //获取当前部门的信息
                   Department department = _import.Tag as Department;
                   
                   for (int i = 0; i < importdt.Rows.Count; i++)
                   {
                       DataRow row = department.Row.Table.NewRow();
                       row.ItemArray = (object[])department.Row.ItemArray.Clone();
                       row["FK_CODE_DEPT"] = row["INFO_CODE"];
                       row["FLAG_TIER"] = (Convert.ToInt32(department.Row["FLAG_TIER"].ToString()) + 1);

                       //
                       int maxID = 0;

                       if (department.Children.Count == 0)
                       {
                           maxID = Convert.ToInt32(row["INFO_CODE"].ToString() + "01");
                           row["INFO_CODE"] = maxID;
                       }
                       else
                       {
                           for (int j = 0; j < department.Children.Count; j++)
                           {
                               int v = Convert.ToInt32(department.Children[j].Row["INFO_CODE"].ToString());
                               if (maxID < v)
                               {
                                   maxID = v;
                               }
                           }
                           row["INFO_CODE"] = maxID + 1;
                       }

                       row["INFO_NAME"] =importdt.Rows[i][0].ToString();
                       if (row["INFO_NAME"].ToString() == "")
                       {
                           load.Close();
                           Toolkit.MessageBox.Show("检测单位名称不能为空！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                           return;
                       }
                       row["address"] = importdt.Rows[i][9].ToString();
                       row["CONTACTER"] = importdt.Rows[i][6].ToString();
                       row["tel"] = importdt.Rows[i][8].ToString();
                       row["phone"] = importdt.Rows[i][7].ToString();

                       string city_flag = importdt.Rows[i][5].ToString();
                       if (row["FLAG_TIER"].ToString() == "4")
                       {
                           if (city_flag == "")
                           {
                               load.Close();
                               Toolkit.MessageBox.Show("是否直属不能为空！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                               return;
                           }
                           else
                           {
                               if (city_flag == "是")
                               {
                                   row["isdept"] = "1";
                               }
                               else
                               {
                                   row["isdept"] = "0";
                               }
                           }
                       }

                       string type = importdt.Rows[i][4].ToString();
                       if (row["FLAG_TIER"].ToString() == "4")
                       {
                           if (city_flag == "")
                           {
                               load.Close();
                               Toolkit.MessageBox.Show("检测站点性质不能为空！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                               return;
                           }
                           else
                           {
                               if (type == "屠宰场")
                               {
                                   row["type"] = "0";
                               }
                               else if (type == "养殖场")
                               {
                                   row["type"] = "1";
                               }
                               else if (type == "检疫站")
                               {
                                   row["type"] = "2";
                               }
                               else if (type == "加工企业")
                               {
                                   row["type"] = "3";
                               }
                           }
                       }
                      

                       //根据当前部门的级别来赋省，市，区的值
                       string provice = importdt.Rows[i][1].ToString();
                       string city = importdt.Rows[i][2].ToString();
                       string country = importdt.Rows[i][3].ToString();

                       if (row["FLAG_TIER"].ToString() == "4" || row["FLAG_TIER"].ToString() == "3")
                       { 
                           if(provice == "" || city == "" || country == "")
                           {
                               load.Close();
                               Toolkit.MessageBox.Show("省市区不能为空！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                               return;
                           }
                       }
                       else if (row["FLAG_TIER"].ToString() == "2")
                       {
                           if (provice == "" || city == "" )
                           {
                               load.Close();
                               Toolkit.MessageBox.Show("省市不能为空！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                               return;
                           }
                       }
                       else if (row["FLAG_TIER"].ToString() == "1")
                       {
                           if (provice == "" )
                           {
                               load.Close();
                               Toolkit.MessageBox.Show("省不能为空！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                               return;
                           }
                       }
                       
                       switch (row["FLAG_TIER"].ToString())
                       {
                           case "0": row["Province"] = "";
                               row["City"] = "";
                               row["Country"] = "";
                               break;
                           case "1":  
                               row["Province"] = ProvinceCityTable.Select("name='" + provice + "'")[0]["id"].ToString();
                               row["City"] = "";
                               row["Country"] = "";
                               break;
                           case "2": 
                               row["Province"] = ProvinceCityTable.Select("name='" + provice + "'")[0]["id"].ToString();
                               row["City"] = ProvinceCityTable.Select("name='" + city + "' and pid = '" + row["Province"] + "'")[0]["id"].ToString();
                               row["Country"] = "";
                               break;
                           case "3": 
                               row["Province"] = ProvinceCityTable.Select("name='" + provice + "'")[0]["id"].ToString();
                               row["City"] = ProvinceCityTable.Select("name='" + city + "' and pid = '" + row["Province"] + "'")[0]["id"].ToString();
                               row["Country"] = ProvinceCityTable.Select("name='" + country + "' and pid = '" + row["City"] + "'")[0]["id"].ToString();
                               break;
                           case "4":
                               row["Province"] = ProvinceCityTable.Select("name='" + provice + "'")[0]["id"].ToString();
                               row["City"] = ProvinceCityTable.Select("name='" + city + "' and pid = '" + row["Province"] + "'")[0]["id"].ToString();
                               row["Country"] = ProvinceCityTable.Select("name='" + country + "' and pid = '" + row["City"] + "'")[0]["id"].ToString();
                               break;
                           default: break;
                       }

                       Department newDepartment = new Department();
                       newDepartment.Parent = department;
                       newDepartment.Name = row["INFO_NAME"].ToString();
                       newDepartment.Row = row;

                       string sql = String.Format("insert into sys_client_sysdept (INFO_CODE,INFO_NAME,FLAG_TIER,FK_CODE_DEPT,PROVINCE,CITY,COUNTRY,ADDRESS,CONTACTER,TEL,PHONE,TYPE,supplierId,isdept) values " +
                           "('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}');"
                     , row["INFO_CODE"], row["INFO_NAME"], row["FLAG_TIER"], row["FK_CODE_DEPT"]
                     , row["PROVINCE"], row["CITY"], row["COUNTRY"], row["ADDRESS"], row["CONTACTER"], row["TEL"], row["PHONE"], row["TYPE"], row["supplierId"], row["isdept"]);

                       try
                       {
                           int count = dbOperation.GetDbHelper().ExecuteSql(sql);
                           if (count == 1)
                           {
                               //Toolkit.MessageBox.Show("保存成功！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                               Common.SysLogEntry.WriteLog("部门管理", (System.Windows.Application.Current.Resources["User"] as UserInfo).ShowName, Common.OperationType.Modify, "新增部门信息");
                           }
                           else
                           {
                               load.Close();
                               Toolkit.MessageBox.Show("保存失败！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                               return;
                           }
                       }
                       catch (Exception ee)
                       {
                           load.Close();
                           System.Diagnostics.Debug.WriteLine("SysDeptManager.btnSave_Click" + ee.Message);
                           Toolkit.MessageBox.Show("数据更新失败!稍后尝试!");
                           return;
                       }
                       state = "view";

                       department.Children.Add(newDepartment);

                       if (i == importdt.Rows.Count - 1)
                       {
                           load.Close();
                           Toolkit.MessageBox.Show("保存成功！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);

                           departmentViewModel = new FamilyTreeViewModel(this.department);
                           load_DeptDetails(row);
                           departmentViewModel.SearchText = row["INFO_NAME"].ToString();
                           departmentViewModel.SearchCommand.Execute(null);
                           _treeView.DataContext = null;
                           _treeView.DataContext = departmentViewModel;
                           _add.Tag = newDepartment;
                           _import.Tag = newDepartment;
                           _edit.Tag = newDepartment;
                       }
                   }
               }
               else
               {
                   load.Close();
                   Toolkit.MessageBox.Show("导入excel内容为空，请确认！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                   return;
               }
           }
           else
           {
               load.Close();
               Toolkit.MessageBox.Show("导入excel内容有错，请确认！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
               return;
           }
        }

        //读取Excel中的内容
        System.Data.DataTable GetDataFromExcelByCom(bool hasTitle = true)
        {
            //打开对话框
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = "Excel(*.xls)|*.xls|Excel(*.xlsx)|*.xlsx";
            openFile.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            openFile.Multiselect = false;
            if (openFile.ShowDialog() == DialogResult.Cancel)
            {
                return null;
            }
            var excelFilePath = openFile.FileName;

            try
            {
                load = new Importing_window();
                load.Show();
                //this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

                Microsoft.Office.Interop.Excel.Application app = new Microsoft.Office.Interop.Excel.Application();
                Sheets sheets;
                object oMissiong = System.Reflection.Missing.Value;
                Workbook workbook = null;//创建工作簿
                System.Data.DataTable dt = new System.Data.DataTable();

                try
                {
                    if (app == null)
                    {
                        return null;
                    }
                    workbook = app.Workbooks.Open(excelFilePath, oMissiong, oMissiong, oMissiong, oMissiong, oMissiong,
                        oMissiong, oMissiong, oMissiong, oMissiong, oMissiong, oMissiong, oMissiong, oMissiong, oMissiong);

                    sheets = workbook.Worksheets;

                    //将数据读入到DataTable中
                    Worksheet worksheet = (Worksheet)sheets.get_Item(1);//读取第一张表  
                    if (worksheet == null)
                    {
                        return null;
                    }

                    int iRowCount = worksheet.UsedRange.Rows.Count;
                    int iColCount = worksheet.UsedRange.Columns.Count;
                    //生成列头
                    for (int i = 0; i < iColCount; i++)
                    {
                        var name = "column" + i;
                        if (hasTitle)
                        {
                            var txt = ((Range)worksheet.Cells[1, i + 1]).Text.ToString();
                            if (!string.IsNullOrEmpty(txt))
                            {
                                name = txt;
                            }
                        }
                        while (dt.Columns.Contains(name))
                        {
                            name = name + "_1";//重复行名称会报错。
                        }
                        dt.Columns.Add(new DataColumn(name, typeof(string)));
                    }
                    //生成行数据
                    Range range;
                    int rowIdx = hasTitle ? 2 : 1;
                    for (int iRow = rowIdx; iRow <= iRowCount; iRow++)
                    {
                        DataRow dr = dt.NewRow();
                        for (int iCol = 1; iCol <= iColCount; iCol++)
                        {
                            range = (Range)worksheet.Cells[iRow, iCol];
                            dr[iCol - 1] = (range.Value2 == null) ? "" : range.Text.ToString();
                        }
                        dt.Rows.Add(dr);
                    }
                    return dt;
                }
                catch { return null; }
                finally
                {
                    workbook.Close(false, oMissiong, oMissiong);
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(workbook);
                    workbook = null;
                    app.Workbooks.Close();
                    app.Quit();
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(app);
                    app = null;
                }

            }
            catch
            {
                Toolkit.MessageBox.Show("无法导入，可能您的机子Office版本有问题！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return null;
            }
        }

        private void _delete_Click(object sender, RoutedEventArgs e)
        {
            Department department = _add.Tag as Department;
            bool result = dbOperation.GetDbHelper().Exists(string.Format("select count(ORDERID) from t_detect_report where DETECTID ='{0}'", department.Row["INFO_CODE"]));
            if (result)
            {
                Toolkit.MessageBox.Show("该检测单位已有对应的检测单数据，不能删除！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            bool result2 = dbOperation.GetDbHelper().Exists(string.Format("select count(RECO_PKID) from sys_client_user where fk_dept = '{0}'", department.Row["INFO_CODE"]));
            if (result2)
            {
                Toolkit.MessageBox.Show("该部门已被用户占用，不能删除！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            bool result3 = dbOperation.GetDbHelper().Exists(string.Format("select count(INFO_CODE) from sys_client_sysdept where FK_CODE_DEPT = '{0}'", department.Row["INFO_CODE"]));
            if (result3)
            {
                Toolkit.MessageBox.Show("该部门已有下级部门，不能删除！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (Toolkit.MessageBox.Show("确定要删除该部门吗？", "系统询问", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    string sql = String.Format("delete from sys_client_sysdept where INFO_CODE='{0}'", department.Row["INFO_CODE"]);
                    int count = dbOperation.GetDbHelper().ExecuteSql(sql);
                    if (count == 1)
                    {
                        //if (department.Row["TYPE"].ToString() == "1")
                        //{
                        //    int n = dbOperation.GetDbHelper().ExecuteSql(string.Format("update t_company set OPENFLAG = '0' where sysdeptid = '{0}'",
                        //                                                  department.Row["INFO_CODE"]));
                        //    if (n != 1)
                        //    {
                        //        Toolkit.MessageBox.Show("被检单位修改失败！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                        //        return;
                        //    }
                        //}
                        Toolkit.MessageBox.Show("删除成功！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                        Common.SysLogEntry.WriteLog("部门管理", (System.Windows.Application.Current.Resources["User"] as UserInfo).ShowName, Common.OperationType.Modify, "删除部门信息");
                    }
                    else
                    {
                        Toolkit.MessageBox.Show("删除失败！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                }
                catch (Exception ee)
                {
                    System.Diagnostics.Debug.WriteLine("SysDeptManager._delete_Click" + ee.Message);
                    Toolkit.MessageBox.Show("数据删除失败!稍后尝试!");
                    return;
                }
            }
            else
            {
                return;
            }
            department.Parent.Children.Remove(department);
            _treeView.DataContext = null;
            departmentViewModel = new FamilyTreeViewModel(this.department);
            _treeView.DataContext = departmentViewModel;

            string searchTxt = "";
            if (department.Parent.Children.Count == 0)
            {
                searchTxt = department.Parent.Name;
                
                //详细信息刷新
                DataRow row = department.Parent.Row;
                load_DeptDetails(row);
                _add.Tag = department.Parent;
                _import.Tag = department.Parent;
                _edit.Tag = department.Parent;
            }
            else
            {
                searchTxt = department.Parent.Children[0].Name;

                //详细信息刷新
                DataRow row = department.Parent.Children[0].Row;
                load_DeptDetails(row);
                _add.Tag = department.Parent.Children[0];
                _import.Tag = department.Parent.Children[0];
                _edit.Tag = department.Parent.Children[0];
            }
            departmentViewModel.SearchText = searchTxt;
            departmentViewModel.SearchCommand.Execute(null);   
        }


        private void Phone_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(String)))
            {
                String text = (String)e.DataObject.GetData(typeof(String));
                if (!isNumberic(text))
                { e.CancelCommand(); }
            }
            else { e.CancelCommand(); }
        }

        private void Phone_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true;
        }

        private void Phone_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!isNumberic(e.Text))
            {
                e.Handled = true;
            }
            else
               e.Handled = false;
        }

        private void Contact_Number_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(String)))
            {
                String text = (String)e.DataObject.GetData(typeof(String));
                if (!isNumberic(text))
                { e.CancelCommand(); }
            }
            else { e.CancelCommand(); }
        }

        private void Contact_Number_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true;
        }

        private void Contact_Number_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!isNumberic(e.Text))
            {
                e.Handled = true;
            }
            else
                e.Handled = false;
        }

        //isDigit是否是数字
        public static bool isNumberic(string _string)
        {
            if (string.IsNullOrEmpty(_string))

                return false;
            foreach (char c in _string)
            {
                if (!char.IsDigit(c))
                    //if(c<'0' c="">'9')//最好的方法,在下面测试数据中再加一个0，然后这种方法效率会搞10毫秒左右
                    return false;
            }
            return true;
        }

    }



    public class FamilyTreeViewModel
    {
        #region Data

        readonly ReadOnlyCollection<DepartmentViewModel> _firstGeneration;
        readonly DepartmentViewModel _rootPerson;
        readonly ICommand _searchCommand;

        IEnumerator<DepartmentViewModel> _matchingPeopleEnumerator;
        string _searchText = String.Empty;

        #endregion // Data

        #region Constructor

        public FamilyTreeViewModel(Department rootPerson)
        {
            _rootPerson = new DepartmentViewModel(rootPerson);

            _firstGeneration = new ReadOnlyCollection<DepartmentViewModel>(
                new DepartmentViewModel[] 
                { 
                    _rootPerson 
                });

            _searchCommand = new SearchFamilyTreeCommand(this);
        }

        #endregion // Constructor

        #region Properties

        #region FirstGeneration

        /// <summary>
        /// Returns a read-only collection containing the first person 
        /// in the family tree, to which the TreeView can bind.
        /// </summary>
        public ReadOnlyCollection<DepartmentViewModel> FirstGeneration
        {
            get { return _firstGeneration; }
        }



        #endregion // FirstGeneration

        #region SearchCommand

        /// <summary>
        /// Returns the command used to execute a search in the family tree.
        /// </summary>
        public ICommand SearchCommand
        {
            get { return _searchCommand; }
        }

        private class SearchFamilyTreeCommand : ICommand
        {
            readonly FamilyTreeViewModel _familyTree;

            public SearchFamilyTreeCommand(FamilyTreeViewModel familyTree)
            {
                _familyTree = familyTree;
            }

            public bool CanExecute(object parameter)
            {
                return true;
            }

            event EventHandler ICommand.CanExecuteChanged
            {
                // I intentionally left these empty because
                // this command never raises the event, and
                // not using the WeakEvent pattern here can
                // cause memory leaks.  WeakEvent pattern is
                // not simple to implement, so why bother.
                add { }
                remove { }
            }

            public void Execute(object parameter)
            {
                _familyTree.PerformSearch();
            }
        }

        #endregion // SearchCommand

        #region SearchText

        /// <summary>
        /// Gets/sets a fragment of the name to search for.
        /// </summary>
        public string SearchText
        {
            get { return _searchText; }
            set
            {
                if (value == _searchText)
                    return;

                _searchText = value;

                _matchingPeopleEnumerator = null;
            }
        }

        #endregion // SearchText

        #endregion // Properties

        #region Search Logic

        void PerformSearch()
        {
            if (_matchingPeopleEnumerator == null || !_matchingPeopleEnumerator.MoveNext())
                this.VerifyMatchingPeopleEnumerator();

            var person = _matchingPeopleEnumerator.Current;

            if (person == null)
                return;

            // Ensure that this person is in view.
            if (person.Parent != null)
                person.Parent.IsExpanded = true;

            person.IsSelected = true;
        }

        void VerifyMatchingPeopleEnumerator()
        {
            var matches = this.FindMatches(_searchText, _rootPerson);
            _matchingPeopleEnumerator = matches.GetEnumerator();

            if (!_matchingPeopleEnumerator.MoveNext())
            {
                System.Windows.MessageBox.Show(
                    "No matching names were found.",
                    "Try Again",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                    );
            }
        }

        IEnumerable<DepartmentViewModel> FindMatches(string searchText, DepartmentViewModel person)
        {
            if (person.NameContainsText(searchText))
                yield return person;

            foreach (DepartmentViewModel child in person.Children)
                foreach (DepartmentViewModel match in this.FindMatches(searchText, child))
                    yield return match;
        }

        #endregion // Search Logic
    }

    public class DepartmentViewModel : INotifyPropertyChanged
    {
        #region Data

        readonly ReadOnlyCollection<DepartmentViewModel> _children;
        readonly DepartmentViewModel _parent;
        readonly Department _department;

        bool _isExpanded;
        bool _isSelected;

        #endregion // Data

        #region Constructors

        public DepartmentViewModel(Department department)
            : this(department, null)
        {
        }

        private DepartmentViewModel(Department department, DepartmentViewModel parent)
        {
            _department = department;
            _parent = parent;

            _children = new ReadOnlyCollection<DepartmentViewModel>(
                    (from child in _department.Children
                     select new DepartmentViewModel(child, this))
                     .ToList<DepartmentViewModel>());
        }

        #endregion // Constructors

        #region Person Properties

        public ReadOnlyCollection<DepartmentViewModel> Children
        {
            get { return _children; }
        }

        public string Name
        {
            get { return _department.Name; }
        }

        public DataRow Row
        {
            get { return _department.Row; }
        }

        public Department Own
        {
            get { return _department.Own; }
        }

        public BitmapImage Img
        {
            get { return _department.Img; }
        }

        #endregion // Person Properties

        #region Presentation Members

        #region IsExpanded

        /// <summary>
        /// Gets/sets whether the TreeViewItem 
        /// associated with this object is expanded.
        /// </summary>
        public bool IsExpanded
        {
            get { return _isExpanded; }
            set
            {
                if (value != _isExpanded)
                {
                    _isExpanded = value;
                    this.OnPropertyChanged("IsExpanded");
                }

                // Expand all the way up to the root.
                if (_isExpanded && _parent != null)
                    _parent.IsExpanded = true;
            }
        }

        #endregion // IsExpanded

        #region IsSelected

        /// <summary>
        /// Gets/sets whether the TreeViewItem 
        /// associated with this object is selected.
        /// </summary>
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (value != _isSelected)
                {
                    _isSelected = value;
                    this.OnPropertyChanged("IsSelected");
                }
            }
        }

        #endregion // IsSelected

        #region NameContainsText

        public bool NameContainsText(string text)
        {
            if (String.IsNullOrEmpty(text) || String.IsNullOrEmpty(this.Name))
                return false;

            return this.Name.IndexOf(text, StringComparison.InvariantCultureIgnoreCase) > -1;
        }

        #endregion // NameContainsText

        #region Parent

        public DepartmentViewModel Parent
        {
            get { return _parent; }
        }

        #endregion // Parent

        #endregion // Presentation Members

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion // INotifyPropertyChanged Members
    }

    public class Department
    {
        readonly List<Department> _children = new List<Department>();
        public IList<Department> Children
        {
            get { return _children; }
        }

        public string Name { get; set; }
        public Department Parent { get; set; }

        public BitmapImage Img
        {
            get
            {
                string uri = @"/Manager/Images/all.png";
                switch (Row["FLAG_TIER"].ToString())
                {
                    case "0": uri = @"/Manager/Images/all.png"; break;
                    case "1": uri = @"/Manager/Images/provice.png"; break;
                    case "2": uri = @"/Manager/Images/city.png"; break;
                    case "3": uri = @"/Manager/Images/country.png"; break;
                    case "4": uri = @"/Manager/Images/dept.png"; break;
                    default: break;
                }

                return new BitmapImage(new Uri("pack://application:,," + uri));
            }
        }
        public DataRow Row { get; set; }

        public Department Own { get { return this; } }
    }

}
