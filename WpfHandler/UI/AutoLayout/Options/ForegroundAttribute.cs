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
using System.Windows.Media;
using WpfHandler.UI.AutoLayout;

namespace WpfHandler.UI.AutoLayout.Options
{
    /// <summary>
    /// Define GUI element's foreground brush.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property |
                    AttributeTargets.Class | AttributeTargets.Struct,
                    AllowMultiple = false, Inherited = true)]
    public class ForegroundAttribute : ColorAttribute, IGUILayoutOption
    {
        /// <summary>
        /// Instiniating attribute with applied brush.
        /// </summary>
        /// <param name="brush">Target brush.</param>
        /// <remarks>No supported via attribute.</remarks>
        public ForegroundAttribute(Brush brush) : base(brush) { }

        /// <summary>
        /// Instiniating attribute with applied brush.
        /// </summary>
        /// <param name="brush">Target brush.</param>
        /// <remarks>No supported via attribute.</remarks>
        public ForegroundAttribute(SolidColorBrush brush) : base(brush) { }

        /// <summary>
        /// Instiniating attribute with applied brush.
        /// </summary>
        /// <param name="colorCode">
        /// Trying to apply string color code as brush by using <see cref="ColorConverter"/> rules.
        /// Not throw excption in case if color's code invalid to prevent UI crash.</param>
        public ForegroundAttribute(string colorCode) : base(colorCode) { }

        /// <summary>
        /// Instiniating attribute with applied color.
        /// </summary>
        /// <param name="color">Target color.</param>
        /// <remarks>No supported via attribute.</remarks>
        public ForegroundAttribute(Color color) : base(color) { }

        /// <summary>
        /// Define GUI element's foreground brush.
        /// </summary>
        /// <param name="element">
        /// Shared UI element. Must be inheirted from 
        /// `System.Windows.Controls.Control` to affect the font properties.
        /// </param>
        public void ApplyLayoutOption(FrameworkElement element)
        {
            // Try to cast into control.
            if (element is System.Windows.Controls.Control control)
            {
                control.Foreground = Brush;
            }
        }
    }
}
