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
using FoodSafetyMonitoring.Common;

namespace FoodSafetyMonitoring.Manager
{
    /// <summary>
    /// UcCertificateProductDetails.xaml 的交互逻辑
    /// </summary>
    public partial class UcCertificateProductDetails : UserControl
    {
        List<string> Cer_details;
        public UcCertificateProductDetails(List<string> cer_details)
        {
            InitializeComponent();
            Cer_details = cer_details;

            _card_id.Text = Cer_details[0];
            _company.Text = Cer_details[1];
            _product_name.Text = Cer_details[3];
            //针对清真存在.5的情况，判断如下：
            //判断输入的是否为小数
            bool flag = true;
            string object_count = Cer_details[4];
            foreach (char c in object_count)
            {
                if (c == '.')
                {
                    flag = false;
                }
            }

            if(flag == true)
            {
                _object_count.Text = ConvertStr.convert_object(object_count) + Cer_details[5];
            }
            else
            {
                string count_dot = ConvertStr.convert_object_dot(object_count);
                count_dot = count_dot.Replace("零点半", "半" + Cer_details[5]);
                count_dot = count_dot.Replace("点", Cer_details[5]);

                _object_count.Text = count_dot;
            }
            
            _product_area.Text = Cer_details[6];
            _dept_name.Text = Cer_details[7];
            _dept_area.Text = Cer_details[8];
            _mdd.Text = Cer_details[9];
            _cz_cardid.Text = Cer_details[2];
            _bz.Text = Cer_details[10];
            _user_name.Text = Cer_details[11];
            _user_id.Text = Cer_details[12];
            _nian.Text = ConvertStr.convert_nian(Cer_details[13]);
            _yue.Text = ConvertStr.convert_yue(Cer_details[14]);
            _day.Text = ConvertStr.convert_day(Cer_details[15]);
        }

    }
}
