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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
using System.Reflection;
using System.Windows.Controls;
using WpfHandler.UI.AutoLayout;


namespace WpfHandler.UI.Virtualization
{
    /// <summary>
    /// Metadata that describes a virtualized UI element.
    /// </summary>
    public class VirtualizedItemMeta
    {
        /// <summary>
        /// Instantiated UI element.
        /// </summary>
        public FrameworkElement Element { get; protected set; }

        /// <summary>
        /// Instantiated field element.
        /// </summary>
        public IGUIField Field { get; protected set; }

        /// <summary>
        /// Root layer that currently hod element.
        /// </summary>
        public LayoutLayer Root { get; protected set; }

        /// <summary>
        /// Binded member.
        /// </summary>
        public MemberInfo Member { get; protected set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="element">Instantiated element.</param>
        /// <param name="layer">Current root layout layer.</param>
        /// <param name="member">Binded member.</param>
        public VirtualizedItemMeta(object element, ref LayoutLayer layer, MemberInfo member)
        {
            Element = element as FrameworkElement;
            Field = element as IGUIField;
            Root = layer;
            Member = member;
        }
    }
}
