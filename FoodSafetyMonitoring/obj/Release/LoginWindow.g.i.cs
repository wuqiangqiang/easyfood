﻿#pragma checksum "..\..\LoginWindow.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "1BD33927EF2CFC7D7483F4C3C1377C4D"
//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.18444
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms.Integration;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace FoodSafetyMonitoring {
    
    
    /// <summary>
    /// LoginWindow
    /// </summary>
    public partial class LoginWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 27 "..\..\LoginWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Image min;
        
        #line default
        #line hidden
        
        
        #line 28 "..\..\LoginWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Image exit;
        
        #line default
        #line hidden
        
        
        #line 29 "..\..\LoginWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Image _img_user;
        
        #line default
        #line hidden
        
        
        #line 30 "..\..\LoginWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Image _img_password;
        
        #line default
        #line hidden
        
        
        #line 31 "..\..\LoginWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox cmbName;
        
        #line default
        #line hidden
        
        
        #line 32 "..\..\LoginWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.PasswordBox _password;
        
        #line default
        #line hidden
        
        
        #line 33 "..\..\LoginWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Image _login;
        
        #line default
        #line hidden
        
        
        #line 36 "..\..\LoginWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock txtMsg;
        
        #line default
        #line hidden
        
        
        #line 38 "..\..\LoginWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox _rememberPassword;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/检测监管系统;component/loginwindow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\LoginWindow.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            
            #line 4 "..\..\LoginWindow.xaml"
            ((FoodSafetyMonitoring.LoginWindow)(target)).KeyDown += new System.Windows.Input.KeyEventHandler(this.Window_KeyDown);
            
            #line default
            #line hidden
            return;
            case 2:
            
            #line 20 "..\..\LoginWindow.xaml"
            ((System.Windows.Controls.Primitives.Thumb)(target)).DragDelta += new System.Windows.Controls.Primitives.DragDeltaEventHandler(this.Thumb_DragDelta);
            
            #line default
            #line hidden
            return;
            case 3:
            this.min = ((System.Windows.Controls.Image)(target));
            
            #line 27 "..\..\LoginWindow.xaml"
            this.min.MouseDown += new System.Windows.Input.MouseButtonEventHandler(this.min_MouseDown);
            
            #line default
            #line hidden
            
            #line 27 "..\..\LoginWindow.xaml"
            this.min.MouseEnter += new System.Windows.Input.MouseEventHandler(this.min_MouseEnter);
            
            #line default
            #line hidden
            
            #line 27 "..\..\LoginWindow.xaml"
            this.min.MouseLeave += new System.Windows.Input.MouseEventHandler(this.min_MouseLeave);
            
            #line default
            #line hidden
            return;
            case 4:
            this.exit = ((System.Windows.Controls.Image)(target));
            
            #line 28 "..\..\LoginWindow.xaml"
            this.exit.MouseDown += new System.Windows.Input.MouseButtonEventHandler(this.exit_MouseDown);
            
            #line default
            #line hidden
            
            #line 28 "..\..\LoginWindow.xaml"
            this.exit.MouseEnter += new System.Windows.Input.MouseEventHandler(this.exit_MouseEnter);
            
            #line default
            #line hidden
            
            #line 28 "..\..\LoginWindow.xaml"
            this.exit.MouseLeave += new System.Windows.Input.MouseEventHandler(this.exit_MouseLeave);
            
            #line default
            #line hidden
            return;
            case 5:
            this._img_user = ((System.Windows.Controls.Image)(target));
            return;
            case 6:
            this._img_password = ((System.Windows.Controls.Image)(target));
            return;
            case 7:
            this.cmbName = ((System.Windows.Controls.ComboBox)(target));
            
            #line 31 "..\..\LoginWindow.xaml"
            this.cmbName.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.cmbName_SelectionChanged);
            
            #line default
            #line hidden
            
            #line 31 "..\..\LoginWindow.xaml"
            this.cmbName.GotFocus += new System.Windows.RoutedEventHandler(this._name_GotFocus);
            
            #line default
            #line hidden
            
            #line 31 "..\..\LoginWindow.xaml"
            this.cmbName.LostFocus += new System.Windows.RoutedEventHandler(this._name_LostFocus);
            
            #line default
            #line hidden
            
            #line 31 "..\..\LoginWindow.xaml"
            this.cmbName.AddHandler(System.Windows.Controls.Primitives.TextBoxBase.TextChangedEvent, new System.Windows.Controls.TextChangedEventHandler(this.cmbName_TextChanged));
            
            #line default
            #line hidden
            return;
            case 8:
            this._password = ((System.Windows.Controls.PasswordBox)(target));
            
            #line 32 "..\..\LoginWindow.xaml"
            this._password.GotFocus += new System.Windows.RoutedEventHandler(this._password_GotFocus);
            
            #line default
            #line hidden
            
            #line 32 "..\..\LoginWindow.xaml"
            this._password.LostFocus += new System.Windows.RoutedEventHandler(this._password_LostFocus);
            
            #line default
            #line hidden
            return;
            case 9:
            this._login = ((System.Windows.Controls.Image)(target));
            
            #line 33 "..\..\LoginWindow.xaml"
            this._login.MouseDown += new System.Windows.Input.MouseButtonEventHandler(this.Image_MouseDown);
            
            #line default
            #line hidden
            
            #line 33 "..\..\LoginWindow.xaml"
            this._login.MouseEnter += new System.Windows.Input.MouseEventHandler(this.Image_MouseEnter);
            
            #line default
            #line hidden
            
            #line 33 "..\..\LoginWindow.xaml"
            this._login.MouseLeave += new System.Windows.Input.MouseEventHandler(this.Image_MouseLeave);
            
            #line default
            #line hidden
            return;
            case 10:
            this.txtMsg = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 11:
            this._rememberPassword = ((System.Windows.Controls.CheckBox)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

