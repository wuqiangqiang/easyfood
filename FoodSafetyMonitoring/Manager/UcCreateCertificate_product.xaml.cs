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
using System.Printing;

namespace FoodSafetyMonitoring.Manager
{
    /// <summary>
    /// UcCreateCertificate_product.xaml 的交互逻辑
    /// </summary>
    public partial class UcCreateCertificate_product : UserControl
    {
        public IDBOperation dbOperation = null;
        string userId = (Application.Current.Resources["User"] as UserInfo).ID;
        string loginid = (Application.Current.Resources["User"] as UserInfo).LoginName;
        string username = (Application.Current.Resources["User"] as UserInfo).ShowName;
        string deptId = (Application.Current.Resources["User"] as UserInfo).DepartmentID;
        private string shipperflag;


        public UcCreateCertificate_product(IDBOperation dbOperation)
        {
            InitializeComponent();

            this.dbOperation = dbOperation;
            _user_name.Text = username;
            _user_id.Text = loginid;
            _nian.Text = ConvertStr.convert_nian(DateTime.Now.Year.ToString());
            _yue.Text = ConvertStr.convert_yue(DateTime.Now.Month.ToString());
            _day.Text = ConvertStr.convert_day(DateTime.Now.Day.ToString());

            //出证人所属部门货主信息flag,生产单位信息
            DataTable table = dbOperation.GetDbHelper().GetDataSet("select ifnull(a.shipperflag,'') as shipperflag, " +
                                    " tzcname,tzcarea,tzcaddress,tzccardid "+
                                    " from sys_client_sysdept a " +
                                    " where INFO_CODE = " + deptId ).Tables[0];

            if (table.Rows.Count != 0)
            {
                shipperflag = table.Rows[0][0].ToString();
                _dept_area.Text = table.Rows[0][2].ToString();
                _dept_address.Text = table.Rows[0][3].ToString();
                _dept_name.Text = table.Rows[0][1].ToString();
                _cz_cardid.Text = table.Rows[0][4].ToString();
            }

            //产品检疫证号
            string product_cardid = dbOperation.GetDbHelper().GetSingle(string.Format("select f_get_productcardid('{0}')", deptId)).ToString();
            _card_id.Text = product_cardid;
            //协检员
            ComboboxTool.InitComboboxSource(_help_user, string.Format("call p_user_helpuser({0})", userId), "lr");
            //产品名称
            ComboboxTool.InitComboboxSource(_product_name, "SELECT productid,productname FROM t_product WHERE openflag = '1' and deptflag = '" + shipperflag + "'", "lr");
            _product_name.SelectionChanged += new SelectionChangedEventHandler(_product_name_SelectionChanged);
            _product_name.SelectedIndex = 1;
            //生产单位
            //ComboboxTool.InitComboboxSource(_dept_name, string.Format("call p_get_dept_tz({0})", userId), "lr");
            //_dept_name.SelectionChanged += new SelectionChangedEventHandler(_dept_name_SelectionChanged);
        }

        void _product_name_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_product_name.SelectedIndex > 0)
            {
                string object_type = dbOperation.GetDbHelper().GetSingle("select unit from t_product where productid =" + (_product_name.SelectedItem as Label).Tag.ToString()).ToString();
                _object_type.Text = object_type;
            }
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (_shipper_id.Text.Trim().Length != 0)
                {
                    DataTable table = dbOperation.GetDbHelper().GetDataSet("select shippername,address from t_shipper_product where shipperid =" + _shipper_id.Text + " and shipperflag = '" + shipperflag + "'").Tables[0];
                    if (table.Rows.Count != 0)
                    {
                        _shipper.Text = table.Rows[0][0].ToString();
                        _mdd.Text = table.Rows[0][1].ToString();
                    }
                    else
                    {
                        Toolkit.MessageBox.Show("该货主不存在！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                        _shipper.Text = "";
                        _mdd.Text = "";
                        return;
                    }
                }
                else
                {
                    _shipper.Text = "";
                    _mdd.Text = "";
                }
                
            }
        }

        private void _shipper_id_LostFocus(object sender, RoutedEventArgs e)
        {
            if(_shipper_id.Text.Trim().Length != 0)
            {
                DataTable table = dbOperation.GetDbHelper().GetDataSet("select shippername,address from t_shipper_product where shipperid =" + _shipper_id.Text + " and shipperflag = '" + shipperflag + "'").Tables[0];
                if (table.Rows.Count != 0)
                {
                    _shipper.Text = table.Rows[0][0].ToString();
                    _mdd.Text = table.Rows[0][1].ToString();
                }
                else
                {
                    Toolkit.MessageBox.Show("该货主不存在！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                    _shipper.Text = "";
                    _mdd.Text = "";
                    return;
                }
            }
            else
            {
                _shipper.Text = "";
                _mdd.Text = "";
            }
            
        } 

        //void _dept_name_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    if (_dept_name.SelectedIndex > 0)
        //    {
        //        DataTable table = dbOperation.GetDbHelper().GetDataSet("select sys_city.name as city,b.name as country,address" + 
        //                            " from sys_client_sysdept a LEFT JOIN sys_city ON a.city = sys_city.id"+
        //                            " LEFT JOIN sys_city b ON a.country = b.id"+
        //                            " where INFO_CODE = " + (_dept_name.SelectedItem as Label).Tag.ToString()).Tables[0];
        //        _dept_area.Text = table.Rows[0][0].ToString() + "市" + table.Rows[0][1].ToString();
        //        _dept_address.Text = table.Rows[0][2].ToString();
        //    }
        //}

        private void _add_Click(object sender, RoutedEventArgs e)
        {
            AddShipper_product ship = new AddShipper_product(dbOperation, shipperflag);
            ship.ShowDialog();
        }

        private void _create_Click(object sender, RoutedEventArgs e)
        {
            if (_card_id.Text.Trim().Length == 0)
            {
                Toolkit.MessageBox.Show("请输入检疫证号！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            bool exit_flag = dbOperation.GetDbHelper().Exists(string.Format("SELECT count(productcardid) from t_certificate_product where productcardid ='{0}'", _card_id.Text));
            if (exit_flag)
            {
                Toolkit.MessageBox.Show("检疫证号已存在，请重新输入！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (_help_user.SelectedIndex < 1)
            {
                Toolkit.MessageBox.Show("请选择协检员！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (_shipper_id.Text.Trim().Length == 0)
            {
                Toolkit.MessageBox.Show("请输入货主代码！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (_product_name.SelectedIndex < 1)
            {
                Toolkit.MessageBox.Show("请选择产品名称！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (_object_count.Text.Trim().Length == 0)
            {
                Toolkit.MessageBox.Show("请输入数量！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (_dept_name.Text.Trim().Length == 0)
            {
                Toolkit.MessageBox.Show("请输入生产单位！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (_cz_cardid.Text.Trim().Length == 0)
            {
                Toolkit.MessageBox.Show("请输入检疫标志号！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            string sql = string.Format("INSERT INTO t_certificate_product(productcardid,companyid,companyname," +
                                        "cardid,objectid,objectname,objectcount,productarea,deptname," +
                                        "deptarea,destinationarea,bz,createdeptid,createuserid,createdate,createloginid,helpuserid)" +
                                        " values ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}'," +
                                        "'{10}','{11}','{12}','{13}','{14}','{15}','{16}')"
                                        ,_card_id.Text, _shipper_id.Text, _shipper.Text,
                                        _cz_cardid.Text, (_product_name.SelectedItem as Label).Tag.ToString(), _product_name.Text, 
                                        _object_count.Text + _object_type.Text, _dept_area.Text,
                                        _dept_name.Text, 
                                        _dept_address.Text, _mdd.Text, _bz.Text,
                                        deptId, userId, System.DateTime.Now, loginid,(_help_user.SelectedItem as Label).Tag.ToString());

            int i = dbOperation.GetDbHelper().ExecuteSql(sql);
            if (i >= 0)
            {
                List<string> cer_details = new List<string>() {_card_id.Text,_shipper.Text,_cz_cardid.Text, _product_name.Text, _object_count.Text ,
                             _object_type.Text, _dept_area.Text,_dept_name.Text, _dept_address.Text, _mdd.Text, _bz.Text,username,loginid,
                            System.DateTime.Now.Year.ToString(),System.DateTime.Now.Month.ToString(),System.DateTime.Now.Day.ToString() };

                UcCertificateProductDetails cer = new UcCertificateProductDetails(cer_details);

                //grid_info.Children.Add(cer);

                PrintDialog dialog = new PrintDialog();
                //if (dialog.ShowDialog() == true)
                //{
                    dialog.PrintQueue = GetPrinter();
                    //Size printSize = new Size(dialog.PrintableAreaWidth, dialog.PrintableAreaHeight);
                    //cer.Measure(printSize);
                    //cer.Arrange(new Rect(0, 0, dialog.PrintableAreaWidth, dialog.PrintableAreaHeight));
                    Size printSize = new Size(793, 529);
                    cer.Measure(printSize);
                    cer.Arrange(new Rect(0, 0, 793, 529));

                    dialog.PrintVisual(cer, "产品检疫证");
                //}

                //Toolkit.MessageBox.Show("电子出证单生成成功！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                clear();
                return;
            }
            else
            {
                Toolkit.MessageBox.Show("电子出证单生成失败！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
        }

        public static PrintQueue GetPrinter(string printerName = null)
        {
            try
            {
                PrintQueue selectedPrinter = null;
                if (!string.IsNullOrEmpty(printerName))
                {
                    var printers = new LocalPrintServer().GetPrintQueues();
                    selectedPrinter = printers.FirstOrDefault(p => p.Name == printerName);
                }
                else
                {
                    selectedPrinter = LocalPrintServer.GetDefaultPrintQueue();
                }
                return selectedPrinter;
            }
            catch
            {
                return null;
            }
        }

        private void clear()
        {
            _card_id.Text = Convert.ToString( Convert.ToInt64(_card_id.Text) + 1);
            _shipper_id.Text = "";
            _shipper.Text = "";
            _object_count.Text = "";
            _mdd.Text = "";
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
