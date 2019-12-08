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
using System.Reflection;
using WpfHandler.UI.AutoLayout;

namespace WpfHandler.UI.Controls
{
    /// <summary>
    /// Interaction logic for FlatPasswordBox.xaml
    /// </summary>
    public partial class FlatPasswordBox : TextFieldControl, IGUIField
    {
        #region Properties
        /// <summary>
        /// Event that will occure in case if value of the field will be changed.
        /// Will cause updating of the BindedMember value.
        /// </summary>
        public event Action<IGUIField> ValueChanged;

        /// <summary>
        /// Text in textbox.
        /// </summary>
        public override string Text
        {
            get { return passwordBox.Password; }
            set { passwordBox.Password = value; }
        }

        /// <summary>
        /// Member binded to that element by the auto layout handler.
        /// </summary>
        public MemberInfo BindedMember { get; set; }

        /// <summary>
        /// Returns reference to the label block of UI element.
        /// </summary>
        public override FrameworkElement LabelElement { get { return lableElement; } }

        /// <summary>
        /// Returns reference to the field block of UI element.
        /// </summary>
        public override FrameworkElement FieldElement { get { return fieldElement; } }
        #endregion

        /// <summary>
        /// Defalut constructor.
        /// 
        /// Trying to load `FlatPasswordBox` as @Style resource.
        /// </summary>
        public FlatPasswordBox()
        {
            InitializeComponent();
            DataContext = this;

            // Try to load default style
            try
            {
                if (Application.Current.FindResource("FlatPasswordBox") is Style style)
                {
                    this.Style = style;
                }
            }
            catch
            {
                // Not found in dictionary. Not important.}
            }
        }

        /// <summary>
        /// Callback that will occure when password will changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            // Inform autolayout handler about changes.
            ValueChanged?.Invoke(this);
        }

        /// <summary>
        /// Not supported.
        /// </summary>
        /// <param name="layer"></param>
        /// <param name="args"></param>
        public void OnLayout(ref LayoutLayer layer, params object[] args)
        {
            throw new NotSupportedException();
        }
               
        /// <summary>
        /// Recomputing dinamic layout values for providing hight quiality view.
        /// </summary>
        public override void RecomputeLayout()
        {
            base.RecomputeLayout();

            // Ipdating spliter visibility.
            if (string.IsNullOrEmpty(Label)) spliter.Visibility = Visibility.Collapsed;
            else spliter.Visibility = Visibility.Visible;
        }
    }
}
