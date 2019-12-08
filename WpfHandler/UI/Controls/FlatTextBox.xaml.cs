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
using System.Reflection;
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
using WpfHandler.UI.AutoLayout;

namespace WpfHandler.UI.Controls
{
    /// <summary>
    /// Interaction logic for FlatTextBox.xaml
    /// </summary>
    [AutoLayout.Configuration.TypesCompatible(typeof(int), typeof(float), typeof(double), typeof(string))]
    public partial class FlatTextBox : TextFieldControl, IGUIField
    {
        #region Properties
        /// <summary>
        /// Event that will occure in case if value of the field will be changed.
        /// Will cause updating of the BindedMember value.
        /// </summary>
        public event Action<IGUIField> ValueChanged;
        
        /// <summary>
        /// Memeber that will be used as source\target for the value into UI.
        /// </summary>
        public MemberInfo BindedMember { get; set; }

        /// <summary>
        /// Returns reference to the label block of UI element.
        /// </summary>
        public override FrameworkElement LabelElement { get { return labelElement; } }

        /// <summary>
        /// Returns reference to the field block of UI element.
        /// </summary>
        public override FrameworkElement FieldElement { get { return fieldElement; } }
        #endregion

        #region Local members
        /// <summary>
        /// Bufer that contains last valid stored text from textbox.
        /// </summary>
        private string textPropertyBufer;
        #endregion

        /// <summary>
        /// Default constructor.
        /// Looking for the `FlatTextBox` Style resource. Use default if not found.
        /// </summary>
        public FlatTextBox() : base()
        {
            InitializeComponent();
            DataContext = this;

            // Try to load default style
            try
            {
                if (Application.Current.FindResource("FlatTextBox") is Style style)
                {
                    Style = style;
                }
            }
            catch
            {
                // Not found in dictionary. Not important.
            }

            // Subscribe on events.
            textBox.TextChanged += TextBox_TextChanged;
        }

        #region API
        /// <summary>
        /// Configurate GUI element and bind it to auto layout handler.
        /// </summary>
        /// <param name="layer">Target UI layer.</param>
        /// <param name="args">Must contains: <see cref="UIDescriptor"/> and <see cref="MemberInfo"/></param>
        public void OnLayout(ref LayoutLayer layer, params object[] args)
        {            
            // Find required referendes.
            MemberInfo member = null;

            // Trying to get shared properties.
            foreach (object obj in args)
            {
                if (obj is MemberInfo)
                {
                    member = (MemberInfo)obj;
                    break;
                }
            }

            // Defining field value mode.
            var type = UIDescriptor.MembersHandler.GetSpecifiedMemberType(member);
            DefineModeByType(type);
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
        #endregion

        #region Callbacks               
        /// <summary>
        /// Will occure when password will be changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Copying value from internal sub-control.
            Text = textBox.Text;

            // Drop if the same value.
            if (Text.Equals(textPropertyBufer))
                return;

            // Validate value.
            switch (ValueMode)
            {
                case TextFieldControl.Mode.Int:
                    if (!Int32.TryParse(Text, out _))
                    {
                        Text = textPropertyBufer;
                    }
                    break;

                case TextFieldControl.Mode.Float:
                    if (!float.TryParse(Text, out _))
                    {
                        Text = textPropertyBufer;
                    }
                    break;
            }

            // Buferize las valid value.
            textPropertyBufer = Text;

            // Inform autolayout handler about changes.
            ValueChanged?.Invoke(this);
        }

        /// <summary>
        /// Init configs when all properties applied.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextFieldControl_Loaded(object sender, RoutedEventArgs e)
        {
            // Set default value for numerical values.
            switch (ValueMode)
            {
                case Mode.Int:
                    if (string.IsNullOrEmpty(Text) || // Is empty
                        !int.TryParse(Text, out int _)) // Is invalid
                        Text = 0.ToString();
                    textPropertyBufer = Text;
                    break;

                case Mode.Float:
                    if(string.IsNullOrEmpty(Text) || // Is empty
                        !float.TryParse(Text, out float _)) // Is invalid
                        Text = 0.ToString();
                    textPropertyBufer = Text;
                    break;
            }
        }
        #endregion
    }
}
