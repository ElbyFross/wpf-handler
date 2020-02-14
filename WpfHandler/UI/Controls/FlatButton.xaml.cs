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
using System.Windows.Threading;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfHandler.UI.AutoLayout;
using WpfHandler.UI.AutoLayout.Configuration;
using WpfHandler.UI.AutoLayout.Markups;
using System.Windows.Controls.Primitives;
using System.Reflection;

namespace WpfHandler.UI.Controls
{
    /// <summary>
    /// Interaction logic for FlatButton.xaml
    /// </summary>
    /// <remarks>
    /// Can be instantiated via the UIDescriptor declaration by an `Action` or `RoutedEventHandler` member.
    /// </remarks>
    [TypesCompatible(typeof(Action), typeof(RoutedEventHandler))]
    public partial class FlatButton : UserControl, ILabel, IGUIField
    {
        /// <summary>
        /// Event that will be called when button will be pressed.
        /// </summary>
        public static readonly RoutedEvent ClickEvent;

        /// <summary>
        /// Bridging XAML declaring and the member.
        /// </summary>
        public static readonly DependencyProperty LabelProperty = DependencyProperty.Register(
          "Label", typeof(string), typeof(FlatButton), new PropertyMetadata("Sample"));

        /// <summary>
        /// Not supported.
        /// </summary>
        public event Action<IGUIField, object[]> ValueChanged;

        /// <summary>
        /// Text that will be displayed on the button.
        /// </summary>
        public string Label
        {
            get { return (string)base.GetValue(LabelProperty); }
            set { base.SetValue(LabelProperty, value); }
        }
        
        /// <summary>
        /// Occurs when a <see cref="FlatButton"/>> is clicked.
        /// </summary>
        public event RoutedEventHandler Click
        {
            add => this.AddHandler(ClickEvent, value);
            remove => this.RemoveHandler(ClickEvent, value);
        }

        /// <summary>
        /// Not supported.
        /// </summary>
        public float LabelWidth { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }

        /// <summary>
        /// Not supported.
        /// </summary>
        public object Value { get => throw new NotSupportedException(); set { ValueChanged?.Invoke(this, new object[0]); } }

        /// <summary>
        /// AS member instance binded to the element via an UIDescriptor.
        /// </summary>
        public MemberInfo BindedMember { get; set; }

        static FlatButton()
        {
            ClickEvent = EventManager.RegisterRoutedEvent(
                 "Click", RoutingStrategy.Bubble,
                 typeof(RoutedEventHandler), typeof(FlatButton));
        }

        /// <summary>
        /// Default constuctor.
        /// </summary>
        public FlatButton()
        {
            InitializeComponent();
            base.DataContext = this;

            // Try to load default style
            try
            {
                if (Application.Current.FindResource("FlatButton") is Style style)
                {
                    base.Style = style;
                }
            }
            catch
            {
                // Not found in dictionary. Not important.
            }
        }

        /// <summary>
        /// Occurs when internal button clicked.
        /// </summary>
        /// <param name="_"></param>
        /// <param name="__"></param>
        private void FlatButton_Click(object _, RoutedEventArgs __)
        {
            RoutedEventArgs e = new RoutedEventArgs(ClickEvent, this);
            RaiseEvent(e);

            ValueChanged?.Invoke(this, new object[0]);
        }

        /// <summary>
        /// Connecting element to the UI handler.
        /// </summary>
        /// <param name="layer">Target UI layer.</param>
        /// <param name="args">Must contains: <see cref="UIDescriptor"/> and <see cref="MemberInfo"/></param>
        /// <remarks>
        /// Allows only a `RoutedEventHandler` or an `Action` delegate as value.
        /// </remarks>
        public void OnLayout(ref LayoutLayer layer, params object[] args)
        {
            #region Looking for shared data
            // Find required referendes.
            UIDescriptor desc = null;
            MemberInfo member = null;

            // Trying to get shared properties.
            foreach (object obj in args)
            {
                if (obj is UIDescriptor) desc = (UIDescriptor)obj;
                if (obj is MemberInfo) member = (MemberInfo)obj;
            }
            #endregion

            Type handlerType = UIDescriptor.MembersHandler.GetSpecifiedMemberType(member);
            if(handlerType.Equals(typeof(RoutedEventHandler)))
            {
                Click += (RoutedEventHandler)UIDescriptor.MembersHandler.GetValue(member, desc);
            }
            else
            {
                Click += delegate(object sender, RoutedEventArgs routedEventArgs)
                {
                    ((Action)UIDescriptor.MembersHandler.GetValue(member, desc))?.Invoke();
                };
            }
        }
    }
}
