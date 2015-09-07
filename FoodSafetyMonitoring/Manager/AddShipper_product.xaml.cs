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
using System.Windows.Shapes;
using FoodSafetyMonitoring.Common;
using FoodSafetyMonitoring.dao;
using System.Data;
using Toolkit = Microsoft.Windows.Controls;
using FoodSafetyMonitoring.Manager.UserControls;

namespace FoodSafetyMonitoring.Manager
{
    /// <summary>
    /// AddShipper.xaml 的交互逻辑
    /// </summary>
    public partial class AddShipper_product : Window
    {
        private IDBOperation dbOperation;
        private string userId = (Application.Current.Resources["User"] as UserInfo).ID;
        private string deptId = (Application.Current.Resources["User"] as UserInfo).DepartmentID;
        private string shipperFlag;

        public AddShipper_product(IDBOperation dbOperation, string shipperflag)
        {
            InitializeComponent();
            this.dbOperation = dbOperation;
            this.shipperFlag = shipperflag;
            this._id.Text = dbOperation.GetDbHelper().GetSingle(string.Format("select f_create_shipper_product('{0}')", shipperFlag)).ToString();

        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (_name.Text.Trim().Length == 0)
            {
                Toolkit.MessageBox.Show("请输入姓名！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (_phone.Text.Trim().Length == 0)
            {
                Toolkit.MessageBox.Show("请输入电话！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (_address.Text.Trim().Length == 0)
            {
                Toolkit.MessageBox.Show("请输入地址！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            //判断货主代码是否存在，若存在则必须重新打开画面
            bool exit_flag = dbOperation.GetDbHelper().Exists(string.Format("SELECT count(shipperid) from t_shipper_product where shipperid ='{0}' and shipperflag = '{1}'", _id.Text, shipperFlag));
            if (exit_flag)
            {
                Toolkit.MessageBox.Show("该货主代码已存在，请先关闭本画面再打开，重新添加！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            string sql = string.Format("insert into t_shipper_product(shipperid,shippername,phone,address,createuserid,createdate,createdeptid,shipperflag) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}')"
                            , _id.Text, _name.Text, _phone.Text, _address.Text, userId,
                            System.DateTime.Now, deptId, shipperFlag);

            int i = dbOperation.GetDbHelper().ExecuteSql(sql);
            if (i > 0)
            {
                Toolkit.MessageBox.Show("货主信息添加成功！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
                return;
            }
            else
            {
                Toolkit.MessageBox.Show("货主信息添加失败！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
        }

        private void Thumb_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            this.Left += e.HorizontalChange;
            this.Top += e.VerticalChange;
        }

        private void exit_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.Close();
            }
        }

        private void exit_MouseEnter(object sender, MouseEventArgs e)
        {
            exit.Source = new BitmapImage(new Uri("pack://application:,," + "/res/close_on.png"));
        }

        private void exit_MouseLeave(object sender, MouseEventArgs e)
        {
            exit.Source = new BitmapImage(new Uri("pack://application:,," + "/res/close.png"));
        }

        private void _phone_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true;
        }
        private void _phone_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(String)))
            {
                String text = (String)e.DataObject.GetData(typeof(String));
                if (!isNumberic(text))
                { e.CancelCommand(); }
            }
            else { e.CancelCommand(); }
        }

        private void _phone_PreviewTextInput(object sender, TextCompositionEventArgs e)
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
