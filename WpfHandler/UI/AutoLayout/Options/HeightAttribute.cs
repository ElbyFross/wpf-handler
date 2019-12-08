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
using WpfHandler.UI.AutoLayout;
using WpfHandler.UI.Controls;

namespace WpfHandler.UI.AutoLayout.Options
{
    /// <summary>
    /// Define height of the GUI element.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property |
                    AttributeTargets.Class | AttributeTargets.Struct,
                    AllowMultiple = false, Inherited = true)]
    public class HeightAttribute : LayoutSizeAttribute, IGUILayoutOption
    {
        /// <summary>
        /// Default constructor.
        /// Using auto height.
        /// </summary>
        public HeightAttribute() : base() { }

        /// <summary>
        /// Set requested height as Size.
        /// </summary>
        /// <param name="height"></param>
        public HeightAttribute(double height) : base(height) { }

        /// <summary>
        /// Define height of the GUI element.
        /// </summary>
        /// <param name="element">Shared UI element.</param>
        public void ApplyLayoutOption(FrameworkElement element)
        {
            element.Height = Size;
        }
    }
}
