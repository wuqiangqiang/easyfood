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
using FoodSafetyMonitoring.dao;
using System.Data;
using FoodSafetyMonitoring.Common;
using FoodSafetyMonitoring.Manager.UserControls;

namespace FoodSafetyMonitoring.Manager
{
    /// <summary>
    /// userSignMap.xaml 的交互逻辑
    /// </summary>
    public partial class userSignMap : Window
    {
        public IDBOperation dbOperation = null;
        public int Id;

        public userSignMap(IDBOperation dbOperation, int id)
        {
            InitializeComponent();

            this.dbOperation = dbOperation;
            this.Id = id;

            //图片地址改为从数据库中获取
            string picture_url = dbOperation.GetDbHelper().GetSingle("select pictureurl from t_url ").ToString();
            if (picture_url == "")
            {
                picture_url = "http://www.zrodo.com:8080/xmjc/";
            }

            string url = dbOperation.GetDbHelper().GetSingle(string.Format("select url from sys_sign_in where id = '{0}'", Id)).ToString();
            if (url != "" )
            {
                _img.Source = new BitmapImage(new Uri(picture_url + url));
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
    }
}
