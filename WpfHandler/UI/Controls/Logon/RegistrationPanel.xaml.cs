//Copyright 2019 Volodymyr Podshyvalov
//
//Licensed under the Apache License, Version 2.0 (the "License");
//you may not use this file except in compliance with the License.
//You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
//Unless required by applicable law or agreed to in writing, software
//distributed under the License is distributed on an "AS IS" BASIS,
//WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//See the License for the specific language governing permissions and
//limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfHandler.UI.Controls.Logon
{
    /// <summary>
    /// Interaction logic for RegistrationPanel.xaml
    /// </summary>
    public partial class RegistrationPanel : UserControl
    {
        public static readonly DependencyProperty ContinueCallbackProperty = DependencyProperty.Register(
          "ContinueCallback", typeof(Action<object>), typeof(RegistrationPanel));

        public static readonly DependencyProperty BackCallbackProperty = DependencyProperty.Register(
          "BackCallback", typeof(Action<object>), typeof(RegistrationPanel));

        #region Properties
        /// <summary>
        /// Method that will has been calling during click on button.
        /// </summary>
        public Action<object> ContinueCallback
        {
            get { return (Action<object>)this.GetValue(ContinueCallbackProperty); }
            set { this.SetValue(ContinueCallbackProperty, value); }
        }

        /// <summary>
        /// Method that will has been calling during click on button.
        /// </summary>
        public Action<object> BackCallback
        {
            get { return (Action<object>)this.GetValue(BackCallbackProperty); }
            set { this.SetValue(BackCallbackProperty, value); }
        }

        /// <summary>
        /// Margine of scalable panel.
        /// </summary>
        public Thickness RegFormMargin
        {
            get
            {
                return new Thickness(0, 0, 0, ActualHeight / 2 - regPanel_FormBlock.ActualHeight / 2);
            }
        }

        /// <summary>
        /// Auto-scalable form width.
        /// </summary>
        public double RegFormWidth
        {
            get
            {
                return Math.Min(Math.Max(150, ActualWidth - 20), 400);
            }
        }

        /// <summary>
        /// Current login value in the field.
        /// </summary>
        public string Login
        {
            get { return regLoginField.Text; }
            set { regLoginField.Text = value; }
        }

        /// <summary>
        /// Current password value in the field.
        /// </summary>
        public string Password
        {
            get { return regPasswordField.Text; }
            set { regPasswordField.Text = value; }
        }

        /// <summary>
        /// Current password's confirmation value in the field.
        /// </summary>
        public string PasswordConfirmation
        {
            get { return regPasswordField2.Text; }
            set { regPasswordField2.Text = value; }
        }
        
        /// <summary>
        /// Current first name value in the field.
        /// </summary>
        public string FirstName
        {
            get { return regFNameField.Text; }
            set { regFNameField.Text = value; }
        }

        /// <summary>
        /// Current first name value in the field.
        /// </summary>
        public string MiddleName
        {
            get { return regMNameField.Text; }
            set { regMNameField.Text = value; }
        }

        /// <summary>
        /// Current first name value in the field.
        /// </summary>
        public string LastName
        {
            get { return regLNameField.Text; }
            set { regLNameField.Text = value; }
        }

        /// <summary>
        /// Does "password not match" error label is enabled.
        /// </summary>
        public bool PasswordNotMatchErrorLabel
        {
            get { return regPanel_error_lPNM.Visibility == Visibility.Visible; }
            set { regPanel_error_lPNM.Visibility = value ? Visibility.Visible : Visibility.Collapsed; }
        }

        /// <summary>
        /// Does "fiill all field" error label is enabled.
        /// </summary>
        public bool FillAllFieldErrorLabel
        {
            get { return regPanel_error_lFAF.Visibility == Visibility.Visible; }
            set { regPanel_error_lFAF.Visibility = value ? Visibility.Visible : Visibility.Collapsed; }
        }
        
        /// <summary>
        /// Error message tha twould displayed at UI's label.
        /// </summary>
        public string ErrorMessage
        {
            set
            {
                // Hide message if null.
                if (value == null)
                {
                    regPanel_error_server.Visibility = Visibility.Collapsed;
                    return;
                }

                // Show message.
                regPanel_error_server.Visibility = Visibility.Visible;
                regPanel_error_server.Content = value;
            }
        }

        /// <summary>
        /// Check if passwords is the same and not null.
        /// </summary>
        public bool IsPasswordsTheSame
        {
            get
            {
                if (regPasswordField.Text == null ||
                   regPasswordField2.Text == null)
                {
                    return false;
                }

                return string.Equals(regPasswordField.Text, regPasswordField2.Text);
            }
        }
        #endregion

        #region Constructor\desctructor
        public RegistrationPanel()
        {
            #region WPF Init
            InitializeComponent();
            DataContext = this;

            // Subscribe on events
            SizeChanged += MainWindow_SizeChanged;

            // Subscribe delegate on buttons.
            regBack.ClickCallback += BackCallbackHandler;
            regContinue.ClickCallback += ContinueCallbackHandler;
            #endregion
        }

        ~RegistrationPanel()
        {
            // Unsubscribe from events.
            SizeChanged -= MainWindow_SizeChanged;

            // Unsubscribe delegate from buttons.
            try { regBack.ClickCallback -= BackCallbackHandler; } catch { };
            try { regContinue.ClickCallback -= ContinueCallbackHandler; } catch { };
        }
        #endregion

        /// <summary>
        /// Clear all data filled to form.
        /// </summary>
        public void Clear()
        {
            Login = "";
            Password = "";
            PasswordConfirmation = "";
            FirstName = "";
            MiddleName = "";
            LastName = "";
            PasswordNotMatchErrorLabel = false;
            FillAllFieldErrorLabel = false;
            ErrorMessage = null;
        }

        #region Callbacks
        /// <summary>
        /// Callback that will has been calling when widow size will be changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // Update size of control panel.
            BindingOperations.GetBindingExpression(regPanel_FormBlock, MarginProperty).UpdateTarget();

            // Update size of control panel.
            BindingOperations.GetBindingExpression(regPanel_FormBlock, WidthProperty).UpdateTarget();
        }

        /// <summary>
        /// Callback that will caling when panel will loaded.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LogonPanel_Loaded(object sender, RoutedEventArgs e)
        {
            // Call recomputing of size.
            MainWindow_SizeChanged(sender, null);
        }

        /// <summary>
        /// Callback to continue button.
        /// </summary>
        /// <param name="sender"></param>
        private void ContinueCallbackHandler(object sender)
        {
            ContinueCallback?.Invoke(sender);
        }

        /// <summary>
        /// Callback to back button.
        /// </summary>
        /// <param name="sender"></param>
        private void BackCallbackHandler(object sender)
        {
            BackCallback?.Invoke(sender);
        }
        #endregion
    }
}
