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
    /// Define orentation of the UI element.
    /// </summary>
    public class OrientationAttribute : Attribute, IGUILayoutOption
    {
        /// <summary>
        /// Orientation 
        /// </summary>
        public System.Windows.Controls.Orientation Value = System.Windows.Controls.Orientation.Horizontal;

        /// <summary>
        /// Default cosntructor.
        /// </summary>
        /// <param name="orientation">Target UI orientation.</param>
        public OrientationAttribute(System.Windows.Controls.Orientation orientation)
        {
            Value = orientation;
        }

        /// <summary>
        /// Applying orientation to the UI element.
        /// </summary>
        /// <param name="element">Must implements Interfaces.ILayoutOrientation.</param>
        public void ApplyLayoutOption(FrameworkElement element)
        {
            // Cast to valid type.
            if(element is ILayoutOrientation control)
            {
                // Apply layout orientation.
                control.Orientation = Value;
            }
        }
    }
}
