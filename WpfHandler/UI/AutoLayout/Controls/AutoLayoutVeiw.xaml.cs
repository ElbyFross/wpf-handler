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
using WpfHandler.UI.Controls;
using WpfHandler.UI.AutoLayout;
using WpfHandler.UI.AutoLayout.Configuration;
using WpfHandler.UI.AutoLayout.Markups;

namespace WpfHandler.UI.AutoLayout.Controls
{
    /// <summary>
    /// Allow to connect auto generated GUI to the XAML descriptor.
    /// </summary>
    [TypesCompatible(typeof(UIDescriptor))]
    public partial class AutoLayoutVeiw : UserControl, IGUIField
    {
        /// <summary>
        /// Descriptor binded to the view.
        /// </summary>
        public UIDescriptor Descriptor
        {
            get { return _Descriptor; }
            set
            {
                // Finalizes current GUI.
                _Descriptor?.UnbindFrom(root);

                if (value != null)
                {
                    // Binding the descriptor to the `root`.
                    if (value.IsVirtualized)
                    {
                        _ = value.BindToAsync(root);
                    }
                    else
                    {
                        value.BindTo(root);
                    }
                }

                // Updates stored value.
                _Descriptor = value;

                // Informs subscribers.
                ValueChanged?.Invoke(this);
            }
        }

        /// <summary>
        /// Operate with a value of <see cref="Descriptor"/> property.
        /// </summary>
        public object Value
        {
            get => Descriptor;
            set
            {
                // Trying to get descriptor.
                if (value is UIDescriptor desc) Descriptor = desc;
            }
        }

        /// <summary>
        /// The UI Descriptor member binded as a source for the element.
        /// </summary>
        public MemberInfo BindedMember { get; set; }
        
        /// <summary>
        /// BUfer that contains connected descriptor.
        /// </summary>
        protected UIDescriptor _Descriptor;

        /// <summary>
        /// Occurs when 
        /// </summary>
        public event Action<IGUIField> ValueChanged;

        /// <summary>
        /// Initialize component.
        /// </summary>
        public AutoLayoutVeiw()
        {
            InitializeComponent();
            DataContext = this;

            main.Loaded += Main_Loaded;
        }

        /// <summary>
        /// Releasing unmanaged memory.
        /// </summary>
        ~AutoLayoutVeiw()
        {
            // Unsubscribe from events.
            try { main.Loaded -= Main_Loaded; } catch { }
        }

        /// <summary>
        /// Update dat after loading.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Main_Loaded(object sender, RoutedEventArgs e)
        { }

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
        }
    }
}
