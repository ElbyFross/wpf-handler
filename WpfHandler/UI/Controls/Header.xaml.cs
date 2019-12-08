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
    /// Interaction logic for Header.xaml
    /// </summary>
    [AutoLayout.Configuration.TypesCompatibleAttribute(typeof(bool))]
    public partial class Header : UserControl, IGUIField, ILabel
    {
        #region Dependency properties
        /// <summary>
        /// Bridging XAML declaring and the member.
        /// </summary>
        public static readonly DependencyProperty LabelProperty = DependencyProperty.Register(
            "Label", typeof(string), typeof(Header), new PropertyMetadata("Sample"));

        /// <summary>
        /// Bridging XAML declaring and the member.
        /// </summary>
        public static readonly DependencyProperty GUIContentProperty = DependencyProperty.Register(
            "GUIContent", typeof(GUIContent), typeof(Header));//, new PropertyMetadata(GUIContent.None));

        /// <summary>
        /// Bridging XAML declaring and the member.
        /// </summary>
        public static readonly DependencyProperty ActiveProperty = DependencyProperty.Register(
            "Active", typeof(bool), typeof(Header), new PropertyMetadata(true));

        /// <summary>
        /// Bridging XAML declaring and the member.
        /// </summary>
        public static readonly DependencyProperty ArrowVisibleProperty = DependencyProperty.Register(
            "ArrowVisible", typeof(bool), typeof(Header), new PropertyMetadata(true));
        #endregion

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
        /// Layer thst will be managed by that header.
        /// </summary>
        public LayoutLayer ChildLayer { get; set; }

        /// <summary>
        /// Text in label field.
        /// </summary>
        public string Label
        {
            get { return (string)this.GetValue(LabelProperty); }
            set { this.SetValue(LabelProperty, value); }
        }

        /// <summary>
        /// Is this header active.
        /// </summary>
        public bool Active
        {
            get { return (bool)this.GetValue(ActiveProperty); }
            set 
            { 
                this.SetValue(ActiveProperty, value); 

                // Show active state UI.
                if(value)
                {
                    listControlUi_Hided.Visibility = Visibility.Collapsed;
                    listControlUi_Showed.Visibility = Visibility.Visible;

                    if (ChildLayer != null)
                    {
                        // Update child layer visibility.
                        ((FrameworkElement)ChildLayer.root).Visibility = Visibility.Visible;
                    }
                }
                // Activate hidded state UI.
                else
                {
                    listControlUi_Hided.Visibility = Visibility.Visible;
                    listControlUi_Showed.Visibility = Visibility.Collapsed;
                    if (ChildLayer != null)
                    {
                        // Update child layer visibility.
                        ((FrameworkElement)ChildLayer.root).Visibility = Visibility.Collapsed;
                    }
                }
            }
        }

        /// <summary>
        /// Is this header active state arrow is visible.
        /// </summary>
        public bool ArrowVisible
        {
            get { return (bool)this.GetValue(ArrowVisibleProperty); }
            set
            {
                this.SetValue(ArrowVisibleProperty, value);

                // Update UI.
                listControlUi.Visibility = value ? Visibility.Visible : Visibility.Collapsed;

                // Inform auto layout handler.
                ValueChanged?.Invoke(this);
            }
        }

        /// <summary>
        /// Is this header active state arrow is visible.
        /// </summary>
        public GUIContent GUIContent
        {
            get { return (GUIContent)GetValue(GUIContentProperty); }
            set
            {
                // Update value into property.
                this.SetValue(GUIContentProperty, value);

                // Update UI.
                if (value != null)
                {
                    this.Label = value.GetTitle();
                }
                else
                {
                    this.Label = "Undefined";
                }
            }
        }

        /// <summary>
        /// Uniform value of that property.
        /// Allow only bool. 
        /// Deine state of @Active property.
        /// </summary>
        public object Value
        {
            get { return Active; }
            set
            {
                if(value is bool state)
                {
                    Active = state;
                }
            }
        }

        /// <summary>
        /// Not supported.
        /// </summary>
        public float LabelWidth { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
        #endregion

        /// <summary>
        /// Default costructor.
        /// </summary>
        public Header()
        {
            InitializeComponent();
            DataContext = this;
        }

        /// <summary>
        /// Connecting element to the UI handler.
        /// </summary>
        /// <param name="layer">Target UI layer.</param>
        /// <param name="args">Must contains: <see cref="UIDescriptor"/> and <see cref="MemberInfo"/></param>
        public void OnLayout(ref LayoutLayer layer, params object[] args)
        {
            #region Looking for shared data
            // Find required referendes.
            UIDescriptor desc = null;
            MemberInfo member = null;
             
            // Trying to get shared properties.
            foreach(object obj in args)
            {
                if (obj is UIDescriptor) desc = (UIDescriptor)obj;
                if (obj is MemberInfo) member = (MemberInfo)obj;
            }
            #endregion

            #region Adding element to the GUI
            // Drop control sign up in case if member not shared.
            if (desc == null || member == null)
            {
                // Adding herader to layout.
                layer?.ApplyControl(this as FrameworkElement);

                // Set active as default.
                Active = true;
            }
            else
            {
                // Connecting to the wrong member. Must connect tot ht virtual bool one.
                // Sing up this control on desctiptor events.
                desc.ControlSignUp(this, member, true);
            }
            #endregion

            #region Start child managment group
            // Starting new vertical layout group.
            new AutoLayout.Configuration.BeginVerticalGroupAttribute().OnLayout(ref layer, args);

            // Store new layer as child.
            ChildLayer = layer;
            #endregion
        }

        /// <summary>
        /// Occurs when layer state button was clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Toggle current state.
            Active = !Active;
        }
    }
}
