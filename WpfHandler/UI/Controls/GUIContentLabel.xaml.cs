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
using WpfHandler.UI.AutoLayout.Markups;

namespace WpfHandler.UI.Controls
{
    /// <summary>
    /// Interaction logic for GUIContentLabel.xaml
    /// </summary>
    [TypesCompatible(typeof(GUIContent))]
    public partial class GUIContentLabel : UserControl, IGUIField, ILabel
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public GUIContentLabel()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Curent value of the element.
        /// Allows only <see cref="GUIContent"/> instances.
        /// </summary>
        public object Value { get => LabelContent; set => LabelContent = value as GUIContent; }

        /// <summary>
        /// The content binded to the label.
        /// </summary>
        public GUIContent LabelContent
        {
            get => _LabelContent;
            set
            {
                _LabelContent = value;
                ValueChanged?.Invoke(this);
            }
        }

        /// <summary>
        /// A bember binded to the UI element as a source during instatiation of the element by <see cref="UIDescriptor"/>.
        /// </summary>
        public MemberInfo BindedMember { get; set; }

        /// <summary>
        /// String content displayed at label at the moment.
        /// </summary>
        public string Label { get => (string)label.Content; set => label.Content = value; }

        /// <summary>
        /// Manages the width of the entire element.
        /// </summary>
        public float LabelWidth { get => (float)Width; set => Width = value; }

        /// <summary>
        /// Occurs when <see cref="LabelContent"/> of <see cref="Value"/> property is changed.
        /// </summary>
        public event Action<IGUIField> ValueChanged;

        /// <summary>
        /// Bufer that contains current content.
        /// </summary>
        protected GUIContent _LabelContent = GUIContent.None;

        /// <summary>
        /// Configurates GUI element and bind it to auto layout handler.
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

            // Binding the label to localization system.
            LabelContent.BindToLabel(this, member);
        }
    }
}
