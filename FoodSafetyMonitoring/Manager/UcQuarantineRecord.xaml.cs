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

namespace FoodSafetyMonitoring.Manager
{
    /// <summary>
    /// UcQuarantineRecord.xaml 的交互逻辑
    /// </summary>
    public partial class UcQuarantineRecord : UserControl
    {
        public IDBOperation dbOperation = null;
        string userId = (Application.Current.Resources["User"] as UserInfo).ID;
        string loginid = (Application.Current.Resources["User"] as UserInfo).LoginName;
        string username = (Application.Current.Resources["User"] as UserInfo).ShowName;
        string deptId = (Application.Current.Resources["User"] as UserInfo).DepartmentID;
        private string shipperflag;

        public UcQuarantineRecord(IDBOperation dbOperation)
        {
            InitializeComponent();
            this.dbOperation = dbOperation;

            //检疫员所属部门货主信息flag,屠宰场名称，检疫分站名称
            DataTable table = dbOperation.GetDbHelper().GetDataSet("select ifnull(a.shipperflag,'') as shipperflag, " +
                                    " tzcname,INFO_NAME" +
                                    " from sys_client_sysdept a " +
                                    " where INFO_CODE = " + deptId).Tables[0];

            if (table.Rows.Count != 0)
            {
                shipperflag = table.Rows[0][0].ToString();
                _detect_site.Text = table.Rows[0][2].ToString();
                _slaughter_site.Text = table.Rows[0][1].ToString();
            }
            //申报人姓名
            ComboboxTool.InitComboboxSource(_shipper_name, string.Format("SELECT sbrid,sbrname FROM t_record_sbr WHERE openflag = '1' and createdeptid = '{0}'", deptId), "lr");
            //产地
            ComboboxTool.InitComboboxSource(_address, string.Format("SELECT areaid,areaname FROM t_record_area WHERE openflag = '1' and createdeptid = '{0}'", deptId), "lr");
            //协检员
            ComboboxTool.InitComboboxSource(_help_user, string.Format("call p_user_helpuser({0})", userId), "lr");
            //官方兽医姓名
            _user_name.Text = username;
            //录入时间
            _entering_datetime.Text = string.Format("{0:g}", System.DateTime.Now);
            //屠宰动物种类
            ComboboxTool.InitComboboxSource(_animal, "SELECT animalid,animalname FROM t_animal_new WHERE openflag = '1' and deptflag = '" + shipperflag + "'", "lr");
            _animal.SelectionChanged += new SelectionChangedEventHandler(_animal_SelectionChanged);
            _animal.SelectedIndex = 1;
            //临床情况
            DataTable dt_quater = new DataTable();
            dt_quater.Columns.Add(new DataColumn("quaterid"));
            dt_quater.Columns.Add(new DataColumn("quatername"));
            var row = dt_quater.NewRow();
            row["quaterid"] = "0";
            row["quatername"] = "良好";
            dt_quater.Rows.Add(row);
            var row2 = dt_quater.NewRow();
            row2["quaterid"] = "1";
            row2["quatername"] = "异常";
            dt_quater.Rows.Add(row2);
            ComboboxTool.InitComboboxSource(_quater, dt_quater, "lr");

            //是否佩戴规定的畜禽标识
            DataTable dt_object_flag = new DataTable();
            dt_object_flag.Columns.Add(new DataColumn("flagid"));
            dt_object_flag.Columns.Add(new DataColumn("flagname"));
            var row3 = dt_object_flag.NewRow();
            row3["flagid"] = "1";
            row3["flagname"] = "是";
            dt_object_flag.Rows.Add(row3);
            var row4 = dt_object_flag.NewRow();
            row4["flagid"] = "0";
            row4["flagname"] = "否";
            dt_object_flag.Rows.Add(row4);
            ComboboxTool.InitComboboxSource(_object_flag, dt_object_flag, "lr");

        }

        void _animal_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_animal.SelectedIndex > 0)
            {
                string object_type = dbOperation.GetDbHelper().GetSingle("select unit from t_animal_new where animalid =" + (_animal.SelectedItem as Label).Tag.ToString()).ToString();
                _object_type.Text = object_type;
                _object_type_zq.Text = object_type;
                _object_type_zq2.Text = object_type;
                _object_type_tb.Text = object_type;
                _object_type_tb2.Text = object_type;
            }
        }

        //private void ObjectCount_KeyDown(object sender, KeyEventArgs e)
        //{
        //    if (e.Key == Key.Enter)
        //    {
        //        if (_object_count.Text.Trim().Length != 0)
        //        {
        //            _ok_zq.Text = _object_count.Text;
        //            _no_zq.Text = "0";
        //            _ok_tb.Text = _object_count.Text;
        //            _no_tb.Text = "0";
        //        }
        //        else
        //        {
        //            _ok_zq.Text = "";
        //            _no_zq.Text = "";
        //            _ok_tb.Text = "";
        //            _no_tb.Text = "";
        //        }
        //    }
        //}

        //private void ObjectCount_LostFocus(object sender, RoutedEventArgs e)
        //{
        //    if (_object_count.Text.Trim().Length != 0)
        //    {
        //        _ok_zq.Text = _object_count.Text;
        //        _no_zq.Text = "0";
        //        _ok_tb.Text = _object_count.Text;
        //        _no_tb.Text = "0";
        //    }
        //    else
        //    {
        //        _ok_zq.Text = "";
        //        _no_zq.Text = "";
        //        _ok_tb.Text = "";
        //        _no_tb.Text = "";
        //    }
        //}

        private void clear()
        {
            _shipper_name.SelectedIndex = 0;
            _address.SelectedIndex = 0;
            _animal.SelectedIndex = 1;
            _object_count.Text = "";
            _quater.SelectedIndex = 0;
            _object_flag.SelectedIndex = 0;
            _card_id.Text = "";
            _ok_zq.Text = "";
            _no_zq.Text = "";
            _ok_tb.Text = "";
            _no_tb.Text = "";
            _card_id_tb.Text = "";
            _qua_card_id.Text = "";
            _bz.Text = "";
            _entering_datetime.Text = string.Format("{0:g}", System.DateTime.Now);

        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {

            //if (_shipper_name.SelectedIndex == 0 || _shipper_name.Text == "")
            //{
            //    Toolkit.MessageBox.Show("申报人姓名不能为空！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
            //    return;
            //}

            //if (_address.SelectedIndex == 0 || _address.Text == "")
            //{
            //    Toolkit.MessageBox.Show("产地不能为空！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
            //    return;
            //}

            //if (_animal.SelectedIndex < 1)
            //{
            //    Toolkit.MessageBox.Show("请选择屠宰动物种类！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
            //    return;
            //}

            //if (_object_count.Text.Trim().Length == 0)
            //{
            //    Toolkit.MessageBox.Show("请输入入场数量！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
            //    return;
            //}

            //if (Convert.ToInt32(_object_count.Text) <= 0)
            //{
            //    Toolkit.MessageBox.Show("入场数量必须大于0！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
            //    return;
            //}

            //if (_quater.SelectedIndex < 1)
            //{
            //    Toolkit.MessageBox.Show("请选择临床情况！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
            //    return;
            //}

            //if (_object_flag.SelectedIndex < 1)
            //{
            //    Toolkit.MessageBox.Show("请选择是否佩戴规定的畜禽标识！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
            //    return;
            //}

            //if (_ok_zq.Text.Trim().Length == 0)
            //{
            //    Toolkit.MessageBox.Show("请输入宰前检查合格数！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
            //    return;
            //}

            //if (_no_zq.Text.Trim().Length == 0)
            //{
            //    Toolkit.MessageBox.Show("请输入宰前检查不合格数！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
            //    return;
            //}

            //if (Convert.ToInt32(_ok_zq.Text) + Convert.ToInt32(_no_zq.Text) != Convert.ToInt32(_object_count.Text))
            //{
            //    Toolkit.MessageBox.Show("宰前检查(合格数+不合格数) != 入场数量,请确认！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
            //    return;
            //}

            //if (_ok_tb.Text.Trim().Length == 0)
            //{
            //    Toolkit.MessageBox.Show("请输入同步检疫合格数！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
            //    return;
            //}

            //if (_no_tb.Text.Trim().Length == 0)
            //{
            //    Toolkit.MessageBox.Show("请输入同步检疫不合格数！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
            //    return;
            //}

            //if (Convert.ToInt32(_ok_tb.Text) + Convert.ToInt32(_no_tb.Text) != Convert.ToInt32(_ok_zq.Text))
            //{
            //    Toolkit.MessageBox.Show("同步检疫(合格数+不合格数) != 宰前检查合格数,请确认！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
            //    return;
            //}

            if (_help_user.SelectedIndex < 1)
            {
                Toolkit.MessageBox.Show("请选择协检员！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            //判断需不需要输入检疫处理通知单编号
            //if (_quater.SelectedIndex == 2 || Convert.ToDouble(_no_zq.Text) > 0 || Convert.ToDouble(_no_tb.Text) > 0)
            //{
            //    if (_qua_card_id.Text.Trim().Length == 0)
            //    {
            //        Toolkit.MessageBox.Show("请输入检疫处理通知单编号！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
            //        return;
            //    } 

            //    //判断检疫处理通知单编号是否存在
            //    bool exit_id = dbOperation.GetDbHelper().Exists(string.Format("SELECT count(id) from t_quarantine_record where qua_cardid ='{0}' and createdeptid = '{1}'", _qua_card_id.Text, deptId));
            //    if (exit_id)
            //    {
            //        Toolkit.MessageBox.Show("检疫处理通知单编号已存在，请确认后重新输入！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
            //        return;
            //    }
            //}

            string sbr_id = "";
            string sbr_name = "";

            //判断申报人是否存在，若不存在则插入数据库
            if (_shipper_name.Text != "-请选择-")
            {
                bool exit_flag = dbOperation.GetDbHelper().Exists(string.Format("SELECT count(sbrid) from t_record_sbr where sbrname ='{0}' and createdeptid = '{1}'", _shipper_name.Text, deptId));
                if (!exit_flag)
                {
                    int n = dbOperation.GetDbHelper().ExecuteSql(string.Format("INSERT INTO t_record_sbr (sbrname,openflag,createuserid,createdeptid,createdate) VALUES('{0}','{1}','{2}','{3}','{4}')",
                                                                  _shipper_name.Text, '1', userId, deptId, DateTime.Now));
                    if (n == 1)
                    {
                        sbr_id = dbOperation.GetDbHelper().GetSingle(string.Format("SELECT sbrid from t_record_sbr where sbrname ='{0}' and createdeptid = '{1}'", _shipper_name.Text, deptId)).ToString();
                    }
                    else
                    {
                        Toolkit.MessageBox.Show("申报人添加失败！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                        return;
                    }
                }
                else
                {
                    sbr_id = dbOperation.GetDbHelper().GetSingle(string.Format("SELECT sbrid from t_record_sbr where sbrname ='{0}' and createdeptid = '{1}'", _shipper_name.Text, deptId)).ToString();
                }
                sbr_name = _shipper_name.Text;
            }
            else
            {
                sbr_name = "";
            }


            string area_id = "";
            string area_name = "";

            //判断产地是否存在，若不存在则插入数据库
            if (_address.Text != "-请选择-")
            {
                bool exit_flag2 = dbOperation.GetDbHelper().Exists(string.Format("SELECT count(areaid) from t_record_area where areaname ='{0}' and createdeptid = '{1}'", _address.Text, deptId));
                if (!exit_flag2)
                {
                    int n = dbOperation.GetDbHelper().ExecuteSql(string.Format("INSERT INTO t_record_area (areaname,openflag,createuserid,createdeptid,createdate) VALUES('{0}','{1}','{2}','{3}','{4}')",
                                                                  _address.Text, '1', userId, deptId, DateTime.Now));
                    if (n == 1)
                    {
                        area_id = dbOperation.GetDbHelper().GetSingle(string.Format("SELECT areaid from t_record_area where areaname ='{0}' and createdeptid = '{1}'", _address.Text, deptId)).ToString();
                    }
                    else
                    {
                        Toolkit.MessageBox.Show("产地添加失败！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                        return;
                    }
                }
                else
                {
                    area_id = dbOperation.GetDbHelper().GetSingle(string.Format("SELECT areaid from t_record_area where areaname ='{0}' and createdeptid = '{1}'", _address.Text, deptId)).ToString();
                }
                area_name = _address.Text;
            }
            else
            {
                area_name = "";
            }
            

            string sql = string.Format("INSERT INTO t_quarantine_record(sbrid,sbrname,areaid,area," +
                                        "animalid,objectcount,objecttype,quater,objectflag,cardid_rc,ok_zq,no_zq,ok_tb," +
                                        "no_tb,cardid_tb,createuserid,createdate,createdeptid,helpuserid,tzcname,bz,qua_cardid)" +
                                        " values ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}'," +
                                        "'{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}','{20}','{21}')"
                                        , sbr_id, sbr_name, area_id, area_name, _animal.SelectedIndex < 1 ? "" : (_animal.SelectedItem as Label).Tag.ToString(),
                                        _object_count.Text, _object_type.Text, _quater.SelectedIndex < 1 ? "" : (_quater.SelectedItem as Label).Tag.ToString(),
                                        _object_flag.SelectedIndex < 1 ? "" : (_object_flag.SelectedItem as Label).Tag.ToString(), 
                                        _card_id.Text,_ok_zq.Text,_no_zq.Text,_ok_tb.Text,_no_tb.Text,_card_id_tb.Text,
                                        userId, _entering_datetime.Text + " " + DateTime.Now.ToLongTimeString().ToString(), deptId, (_help_user.SelectedItem as Label).Tag.ToString(),
                                        _slaughter_site.Text, _bz.Text, _qua_card_id.Text);

            int i = dbOperation.GetDbHelper().ExecuteSql(sql);
            if (i >= 0)
            {
                Toolkit.MessageBox.Show("检疫记录保存成功！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                clear();
                return;
            }
            else
            {
                Toolkit.MessageBox.Show("检疫记录保存失败！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            clear();
            _help_user.SelectedIndex = 0;
        }     

        private void Object_Count_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(String)))
            {
                String text = (String)e.DataObject.GetData(typeof(String));
                if (!isNumberic(text))
                { e.CancelCommand(); }
            }
            else { e.CancelCommand(); }
        }

        private void Object_Count_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true;
        }

        private void Object_Count_PreviewTextInput(object sender, TextCompositionEventArgs e)
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
}
