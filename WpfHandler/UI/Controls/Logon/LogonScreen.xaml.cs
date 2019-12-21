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
        #region Dependancy properties
        /// <summary>
        /// Property that bridging control's property between XAML and code.
        /// </summary>
        public static readonly RoutedEvent OperationCancelCallbackProperty = EventManager.RegisterRoutedEvent("OperationCancelCallback",
            RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(LogonScreen));

        /// <summary>
        /// Property that bridging control's property between XAML and code.
        /// </summary>
        public static readonly RoutedEvent LogonPanel_LoginCallbackProperty = EventManager.RegisterRoutedEvent("LogonPanel_LoginCallback",
            RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(LogonScreen));

        /// <summary>
        /// Property that bridging control's property between XAML and code.
        /// </summary>
        public static readonly RoutedEvent LogonPanel_SignUpCallbackProperty = EventManager.RegisterRoutedEvent("LogonPanel_SignUpCallback",
            RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(LogonScreen));

        /// <summary>
        /// Property that bridging control's property between XAML and code.
        /// </summary>
        public static readonly RoutedEvent RegPanel_ContinueCallbackProperty = EventManager.RegisterRoutedEvent("RegPanel_ContinueCallback",
            RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(LogonScreen));

        /// <summary>
        /// Property that bridging control's property between XAML and code.
        /// </summary>
        public static readonly RoutedEvent RegPanel_BackCallbackProperty = EventManager.RegisterRoutedEvent("RegPanel_BackCallback",
            RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(LogonScreen));
        #endregion

        /// <summary>
        /// Method that will has been calling during click on button.
        /// </summary>
        public event RoutedEventHandler LogonPanel_LoginCallback
        {
            add => logonPanel.loginButton.Click += value;
            remove => logonPanel.loginButton.Click -= value;
        }

        /// <summary>
        /// Method that will has been calling during click on button.
        /// </summary>
        public event RoutedEventHandler LogonPanel_SignUpCallback
        {
            add => logonPanel.signupButton.Click += value;
            remove => logonPanel.signupButton.Click -= value;
        }


        /// <summary>
        /// Method that will has been calling during continue on button.
        /// </summary>
        public event RoutedEventHandler RegPanel_FormsFilledEventHandler
        {
            add => registrationPanel.FormsFilled += value;
            remove => registrationPanel.FormsFilled -= value;
        }

        /// <summary>
        /// Method that will has been calling during back on button.
        /// </summary>
        public event RoutedEventHandler RegPanel_CancelEventHandler
        {
            add => registrationPanel.Cancel += value;
            remove => registrationPanel.Cancel -= value;
        }

        /// <summary>
        /// Method that will has been calling during click on operation cancel button.
        /// </summary>
        public RoutedEventHandler OperationCancelCallback { get; set; }

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

        /// <summary>
        /// Default constructor.
        /// </summary>
        public LogonScreen()
        {
            #region WPF Init
            InitializeComponent();
            DataContext = this;
            #endregion

            LogonPanel_SignUpCallback += LogonPanel_SignUpCallbackHandler;
            LogonPanel_LoginCallback += LogonPanel_LoginCallbackHandler;

            RegPanel_CancelEventHandler += RegPanel_BackCallbackHandler;
            RegPanel_FormsFilledEventHandler += RegPanel_ContinueCallbackHandler;
        }

        /// <summary>
        /// Releasing unmanaged memeory.
        /// </summary>
        ~LogonScreen()
        {
            try { LogonPanel_SignUpCallback -= LogonPanel_SignUpCallbackHandler; } catch { };
            try { LogonPanel_LoginCallback -= LogonPanel_LoginCallbackHandler; } catch { };

            try { RegPanel_CancelEventHandler -= RegPanel_BackCallbackHandler; } catch { };
            try { RegPanel_FormsFilledEventHandler -= RegPanel_ContinueCallbackHandler; } catch { };
        }

        /// <summary>
        /// Clear all temporal data and the data from forms.
        /// </summary>
        public void Clear()
        {
            logonPanel.Clear();
            registrationPanel.Clear();
        }

        private void UILoaded(object sender, RoutedEventArgs e)
        {
            //Binding backgroundBinding = new Binding()
            //{
            //    Path = new PropertyPath("Background"),
            //    Source = main.Background,
            //    Mode = BindingMode.TwoWay
            //};
            //logonPanel.SetBinding(LogonPanel.BackgroundProperty, backgroundBinding);
            //registrationPanel.SetBinding(RegistrationPanel.BackgroundProperty, backgroundBinding);

            logonPanel.Background = main.Background;
            registrationPanel.Background = main.Background;

            switchPanel.Duration = FormsAnimationDuration;
            switchPanel.current.Children.Add(logonPanel);
        }

        private void LogonPanel_SignUpCallbackHandler(object sender, RoutedEventArgs e)
        {
            _ = switchPanel.SwitchToAsync(registrationPanel, SwitchPanel.AnimationType.AlphaSwipe);
        }

        private void LogonPanel_LoginCallbackHandler(object sender, RoutedEventArgs e)
        {
            //switchPanel.SwitchTo(registrationPanel);
        }


        private void RegPanel_BackCallbackHandler(object sender, RoutedEventArgs e)
        {
            _ = switchPanel.SwitchToAsync(logonPanel, SwitchPanel.AnimationType.AlphaSwipe);
        }

        private void RegPanel_ContinueCallbackHandler(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show("Operation indefined.");
        }
    }
}
