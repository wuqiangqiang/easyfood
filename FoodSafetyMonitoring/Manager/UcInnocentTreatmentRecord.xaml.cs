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
    public partial class UcInnocentTreatmentRecord : UserControl
    {
        public IDBOperation dbOperation = null;
        string userId = (Application.Current.Resources["User"] as UserInfo).ID;
        string loginid = (Application.Current.Resources["User"] as UserInfo).LoginName;
        string username = (Application.Current.Resources["User"] as UserInfo).ShowName;
        string deptId = (Application.Current.Resources["User"] as UserInfo).DepartmentID;
        private string shipperflag;
        private string sbrid;
        private string areaid;
        private string animalid;

        public UcInnocentTreatmentRecord(IDBOperation dbOperation)
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
            //无害化处理方式
            ComboboxTool.InitComboboxSource(_function_zq, string.Format("select functionid,functionname from t_function where openflag = '1'"), "lr");
            ComboboxTool.InitComboboxSource(_function_tb, string.Format("select functionid,functionname from t_function where openflag = '1'"), "lr");

            //单位
            DataTable dt_type = dbOperation.GetDbHelper().GetDataSet("select animalid,unit from t_animal_new where deptflag =" + shipperflag).Tables[0];
            _object_type_zq.Text = dt_type.Rows[0][1].ToString();
            _object_type_tb.Text = dt_type.Rows[0][1].ToString();
            _object_type_san.Text = "公斤";
        }

        //private void TextBox_KeyDown(object sender, KeyEventArgs e)
        //{
        //    if (e.Key == Key.Enter)
        //    {
        //        if (_qua_card_id.Text.Trim().Length != 0)
        //        {
        //            //DataTable table = dbOperation.GetDbHelper().GetDataSet(string.Format("select sbrid,sbrname,areaid,area,objecttype,animalid,no_zq,no_tb from t_quarantine_record where qua_cardid ='{0}' and createdeptid ='{1}'",
        //            //                             _qua_card_id.Text,deptId)).Tables[0];
        //            DataTable table = dbOperation.GetDbHelper().GetDataSet(string.Format("select objecttype,animalid from t_quarantine_record where qua_cardid ='{0}' and createdeptid ='{1}'",
        //                                         _qua_card_id.Text, deptId)).Tables[0];
        //            if (table.Rows.Count != 0)
        //            {
        //                //sbrid = table.Rows[0][0].ToString();
        //                //_shipper_name.Text = table.Rows[0][1].ToString();
        //                //areaid = table.Rows[0][2].ToString();
        //                //_address.Text = table.Rows[0][3].ToString();
        //                //_object_type_zq.Text = table.Rows[0][4].ToString();
        //                //animalid = table.Rows[0][5].ToString();
        //                //_no_zq.Text = table.Rows[0][6].ToString();

        //                //if (animalid == "1")//动物种类是鸡
        //                //{
        //                //    _object_type_tb.Text = "羽";
        //                //    _no_tb.Text = table.Rows[0][7].ToString();
        //                //}
        //                //else
        //                //{
        //                //    _object_type_tb.Text = "公斤";
        //                //    _no_tb.Text = "";
        //                //}

        //                _object_type_zq.Text = table.Rows[0][0].ToString();
        //                animalid = table.Rows[0][1].ToString();
        //                if (animalid == "1")//动物种类是鸡
        //                {
        //                    _object_type_tb.Text = "羽";
        //                }
        //                else
        //                {
        //                    _object_type_tb.Text = table.Rows[0][0].ToString();
        //                }
        //                _object_type_san.Text = "公斤";
        //            }
        //            else
        //            {
        //                Toolkit.MessageBox.Show("该检疫处理通知单不存在！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
        //                //_shipper_name.Text = "";
        //                //_address.Text = "";
        //                _object_type_zq.Text = "";
        //                _object_type_tb.Text = "";
        //                _object_type_san.Text = "";
        //                //_no_zq.Text = "";
        //                //_no_tb.Text = "";
        //                return;
        //            }
        //        }
        //        else
        //        {
        //            //_shipper_name.Text = "";
        //            //_address.Text = "";
        //            _object_type_zq.Text = "";
        //            _object_type_tb.Text = "";
        //            _object_type_san.Text = "";
        //            //_no_zq.Text = "";
        //            //_no_tb.Text = "";
        //        }
        //    }
        //}

        //private void _card_id_LostFocus(object sender, RoutedEventArgs e)
        //{
        //    if (_qua_card_id.Text.Trim().Length != 0)
        //    {
        //        //DataTable table = dbOperation.GetDbHelper().GetDataSet(string.Format("select sbrid,sbrname,areaid,area,objecttype,animalid,no_zq,no_tb from t_quarantine_record where qua_cardid ='{0}' and createdeptid ='{1}'",
        //        //                             _qua_card_id.Text,deptId)).Tables[0];
        //        DataTable table = dbOperation.GetDbHelper().GetDataSet(string.Format("select objecttype,animalid from t_quarantine_record where qua_cardid ='{0}' and createdeptid ='{1}'",
        //                                     _qua_card_id.Text, deptId)).Tables[0];
        //        if (table.Rows.Count != 0)
        //        {
        //            //sbrid = table.Rows[0][0].ToString();
        //            //_shipper_name.Text = table.Rows[0][1].ToString();
        //            //areaid = table.Rows[0][2].ToString();
        //            //_address.Text = table.Rows[0][3].ToString();
        //            //_object_type_zq.Text = table.Rows[0][4].ToString();
        //            //animalid = table.Rows[0][5].ToString();
        //            //_no_zq.Text = table.Rows[0][6].ToString();

        //            //if (animalid == "1")//动物种类是鸡
        //            //{
        //            //    _object_type_tb.Text = "羽";
        //            //    _no_tb.Text = table.Rows[0][7].ToString();
        //            //}
        //            //else
        //            //{
        //            //    _object_type_tb.Text = "公斤";
        //            //    _no_tb.Text = "";
        //            //}

        //            _object_type_zq.Text = table.Rows[0][0].ToString();
        //            animalid = table.Rows[0][1].ToString();
        //            if (animalid == "1")//动物种类是鸡
        //            {
        //                _object_type_tb.Text = "羽";
        //            }
        //            else
        //            {
        //                _object_type_tb.Text = table.Rows[0][0].ToString();
        //            }
        //            _object_type_san.Text = "公斤";
        //        }
        //        else
        //        {
        //            Toolkit.MessageBox.Show("该检疫处理通知单不存在！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
        //            //_shipper_name.Text = "";
        //            //_address.Text = "";
        //            _object_type_zq.Text = "";
        //            _object_type_tb.Text = "";
        //            _object_type_san.Text = "";
        //            //_no_zq.Text = "";
        //            //_no_tb.Text = "";
        //            return;
        //        }
        //    }
        //    else
        //    {
        //        //_shipper_name.Text = "";
        //        //_address.Text = "";
        //        _object_type_zq.Text = "";
        //        _object_type_tb.Text = "";
        //        _object_type_san.Text = "";
        //        //_no_zq.Text = "";
        //        //_no_tb.Text = "";
        //    }

        //}

        private void clear()
        {
            _qua_card_id.Text = "";
            _shipper_name.Text = "";
            _address.Text = "";
            _no_zq.Text = "";
            _no_tb.Text = "";
            _san_zl.Text = "";
            //_object_type_zq.Text = "";
            //_object_type_tb.Text = "";
            //_object_type_san.Text = "";
            _san.IsChecked = false;
            _function_zq.SelectedIndex = 0;
            _function_tb.SelectedIndex = 0;
            _bz.Text = "";
            _entering_datetime.Text = string.Format("{0:g}", System.DateTime.Now);

        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (_qua_card_id.Text.Trim().Length == 0)
            {
                Toolkit.MessageBox.Show("请输入检疫处理通知单编号！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (_shipper_name.SelectedIndex < 1)
            {
                Toolkit.MessageBox.Show("请选择存在的申报人！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (_address.SelectedIndex < 1)
            {
                Toolkit.MessageBox.Show("请选择存在的产地！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (_no_zq.Text.Trim().Length != 0)
            {
                if (Convert.ToDouble(_no_zq.Text) > 0 )
                {
                    if (_function_zq.SelectedIndex < 1)
                    {
                        Toolkit.MessageBox.Show("请选择宰前无害化处理方式！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                        return;
                    }
                }
            }

            if (_no_tb.Text.Trim().Length != 0)
            {
                if (Convert.ToDouble(_no_tb.Text) > 0 )
                {
                    if (_function_tb.SelectedIndex < 1)
                    {
                        Toolkit.MessageBox.Show("请选择同步无害化处理方式！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                        return;
                    }
                }
            }
            

            if (_help_user.SelectedIndex < 1)
            {
                Toolkit.MessageBox.Show("请选择协检员！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            //三腺修割标志位
            string san_flag;
            if (_san.IsChecked == true)
            {
                san_flag = "1";
            }
            else
            {
                san_flag = "0";
            }

            string sql = string.Format("INSERT INTO t_innocent_record(qua_cardid,sbrid,sbrname,areaid,area," +
                                        "animalid,no_zq,objecttype_zq,function_zq,no_tb,objecttype_tb,function_tb," +
                                        "bz,createuserid,createdate,createdeptid,helpuserid,tzcname,san_flag,san_zl)" +
                                        " values ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}'," +
                                        "'{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}' )"
                                        , _qua_card_id.Text, (_shipper_name.SelectedItem as Label).Tag.ToString(), _shipper_name.Text,
                                        (_address.SelectedItem as Label).Tag.ToString(), _address.Text, animalid,
                                        _no_zq.Text, _object_type_zq.Text, _function_zq.SelectedIndex < 1 ? "" : (_function_zq.SelectedItem as Label).Tag.ToString(),
                                        _no_tb.Text, _object_type_tb.Text, _function_tb.SelectedIndex < 1 ? "" : (_function_tb.SelectedItem as Label).Tag.ToString(),
                                        _bz.Text,userId, System.DateTime.Now, deptId, (_help_user.SelectedItem as Label).Tag.ToString(),
                                        _slaughter_site.Text, san_flag, _san_zl.Text);

            int i = dbOperation.GetDbHelper().ExecuteSql(sql);
            if (i >= 0)
            {
                Toolkit.MessageBox.Show("无害化处理情况保存成功！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                clear();
                return;
            }
            else
            {
                Toolkit.MessageBox.Show("无害化处理情况保存失败！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            clear();
            _help_user.SelectedIndex = 0;
        }

        private void san_checked(object sender, RoutedEventArgs e)
        {
            _bz.Text = "三腺修割";
        }

        private void san_unchecked(object sender, RoutedEventArgs e)
        {
            _bz.Text = "";
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
