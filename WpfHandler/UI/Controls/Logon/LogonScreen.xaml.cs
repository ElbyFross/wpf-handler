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
    /// Interaction logic for LogonScreen.xaml
    /// </summary>
    public partial class LogonScreen : UserControl
    {
        public static readonly DependencyProperty OperationCancelCallbackProperty = DependencyProperty.Register(
          "OperationCancelCallback", typeof(Action<object>), typeof(LogonScreen));


        /// <summary>
        /// Method that will has been calling during click on button.
        /// </summary>
        public Action<object> LogonPanel_LoginCallback
        {
            get { return logonPanel.LoginCallback; }
            set { logonPanel.LoginCallback = value; }
        }
        
        /// <summary>
        /// Method that will has been calling during click on button.
        /// </summary>
        public Action<object> LogonPanel_SingUpCallback
        {
            get { return logonPanel.SingUpCallback; }
            set { logonPanel.SingUpCallback = value; }
        }


        /// <summary>
        /// Method that will has been calling during continue on button.
        /// </summary>
        public Action<object> RegPanel_ContinueCallback
        {
            get { return registrationPanel.ContinueCallback; }
            set { registrationPanel.ContinueCallback = value; }
        }

        /// <summary>
        /// Method that will has been calling during back on button.
        /// </summary>
        public Action<object> RegPanel_BackCallback
        {
            get { return registrationPanel.BackCallback; }
            set { registrationPanel.BackCallback = value; }
        }

        /// <summary>
        /// Method that will has been calling during click on operation cancel button.
        /// </summary>
        public Action<object> OperationCancelCallback { get; set; }

        /// <summary>
        /// How many time will take swich animation of forms.
        /// </summary>
        public TimeSpan FormsAnimationDuration
        {
            get; set;
        } = new TimeSpan(0, 0, 0, 0, 300);

        /// <summary>
        /// Returns current active form.
        /// </summary>
        public UIElement CurrentForm
        {
            get
            {
                if (switchPanel.current.Children.Count == 0)
                {
                    return null;
                }
                return switchPanel.current.Children[0];
            }
        }

        /// <summary>
        /// Instance of logon panel.
        /// </summary>
        public readonly LogonPanel logonPanel = new LogonPanel();

        /// <summary>
        /// Instance of refistration panel.
        /// </summary>
        public readonly RegistrationPanel registrationPanel = new RegistrationPanel();

        public LogonScreen()
        {
            #region WPF Init
            InitializeComponent();
            DataContext = this;
            #endregion

            LogonPanel_SingUpCallback += LogonPanel_SingUpCallbackHandler;
            LogonPanel_LoginCallback += LogonPanel_LoginCallbackHandler;

            RegPanel_BackCallback += RegPanel_BackCallbackHandler;
            RegPanel_ContinueCallback += RegPanel_ContinueCallbackHandler;
        }

        ~LogonScreen()
        {
            try { LogonPanel_SingUpCallback -= LogonPanel_SingUpCallbackHandler; } catch { };
            try { LogonPanel_LoginCallback -= LogonPanel_LoginCallbackHandler; } catch { };

            try { RegPanel_BackCallback -= RegPanel_BackCallbackHandler; } catch { };
            try { RegPanel_ContinueCallback -= RegPanel_ContinueCallbackHandler; } catch { };
        }

        /// <summary>
        /// Clear all temporal data and the data from forms.
        /// </summary>
        public void Clear()
        {
            logonPanel.Clear();
            registrationPanel.Clear();
        }

        private void LogonPanel_Loaded(object sender, RoutedEventArgs e)
        {
            switchPanel.Duration = FormsAnimationDuration;
            switchPanel.current.Children.Add(logonPanel);
        }

        private void LogonPanel_SingUpCallbackHandler(object sender)
        {
            switchPanel.SwitchTo(registrationPanel, SwitchPanel.AnimationType.AlphaSwipe);
        }

        private void LogonPanel_LoginCallbackHandler(object sender)
        {
            //switchPanel.SwitchTo(registrationPanel);
        }


        private void RegPanel_BackCallbackHandler(object sender)
        {
            switchPanel.SwitchTo(logonPanel, SwitchPanel.AnimationType.AlphaSwipe);
        }

        private void RegPanel_ContinueCallbackHandler(object sender)
        {
            //MessageBox.Show("Operation indefined.");
        }
    }
}
