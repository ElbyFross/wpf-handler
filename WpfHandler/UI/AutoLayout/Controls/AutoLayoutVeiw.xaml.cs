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

                _Descriptor.ValueChanged += Descriptor_ValueChanged;

                // Informs subscribers.
                ValueChanged?.Invoke(this, new object[0]);
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
        public event Action<IGUIField, object[]> ValueChanged;

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
        /// Occurs when one of the shild elements of descriptor is changed.
        /// Infoms <see cref="ValueChanged"/> event subscribers about.
        /// </summary>
        /// <param name="sender">Sender descriptor.</param>
        /// <param name="field">Changed field.</param>
        /// <param name="args">Shared arguments.</param>
        private void Descriptor_ValueChanged(UIDescriptor sender, IGUIField field, object[] args)
        {
            ValueChanged?.Invoke(this, new object[] {sender, field }.Concat(args).ToArray());
        }

        
        /// <summary>
        /// Sharable options applied to an element instance.
        /// </summary>
        protected List<ISharableGUILayoutOption> appliedSharableOptions;

        /// <summary>
        /// Connecting element to the UI handler.
        /// </summary>
        /// <param name="layer">Target UI layer.</param>
        /// <param name="args">Must contains: <see cref="UIDescriptor"/> and <see cref="MemberInfo"/></param>
        /// <remarks>
        /// Allows only a `RoutedEventHandler` or an `Action` delegate as value.
        /// </remarks>
        public virtual void OnLayout(ref LayoutLayer layer, params object[] args)
        {
            //try
            //{
            //    // Lookinf for the sahrable options attributes.
            //    if (args != null)
            //    {
            //        #region Looking for shared data
            //        // Find required referendes.
            //        UIDescriptor desc = args[0] as UIDescriptor;
            //        MemberInfo member = args[1] as MemberInfo;

            //        // Looking for sharable attributes applied to the descriptor type.
            //        var globalAttributes = ((IEnumerable<Attribute>)args[2]).
            //            Where(m => m.GetType().GetInterface(typeof(ISharableGUILayoutOption).FullName) != null);

            //        // Looking for sharable attributes applied to the member.
            //        var localAttributes = ((IEnumerable<Attribute>)args[3]).
            //            Where(m => m.GetType().GetInterface(typeof(ISharableGUILayoutOption).FullName) != null);

            //        appliedSharableOptions = new List<ISharableGUILayoutOption>();
            //        foreach (Attribute attr in globalAttributes) appliedSharableOptions.Add(attr as ISharableGUILayoutOption);
            //        foreach (Attribute attr in localAttributes) appliedSharableOptions.Add(attr as ISharableGUILayoutOption);
            //        foreach (ISharableGUILayoutOption attr in desc.SharedLayoutOptions) appliedSharableOptions.Add(attr);
            //        #endregion
            //    }
            //}
            //catch { }

            //if (_Descriptor != null)
            //{
            //    if (appliedSharableOptions != null)
            //    {
            //        _Descriptor.SharedLayoutOptions = appliedSharableOptions.ToArray();
            //    }
            //    // Binding the descriptor to the `root`.
            //    if (_Descriptor.IsVirtualized)
            //    {
            //        //value.BindTo(root);
            //        _ = _Descriptor.BindToAsync(root);
            //    }
            //    else
            //    {
            //        _Descriptor.BindTo(root);
            //    }
            //}
        }

    }
}
