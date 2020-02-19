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
using WpfHandler.UI.Controls;
using WpfHandler.UI.AutoLayout;
using WpfHandler.UI.AutoLayout.Configuration;
using WpfHandler.UI.AutoLayout.Markups;

namespace WpfHandler.UI.AutoLayout.Options
{
    /// <summary>
    /// Defines element's palette.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property |
                    AttributeTargets.Class | AttributeTargets.Struct,
                    AllowMultiple = false, Inherited = true)]
    public class PaletteAttribute : Attribute, ISharableGUILayoutOption
    {
        /// <summary>
        /// Array that contains the list of color codes that defines the palette.
        /// </summary>
        /// <remarks>
        /// Recomended scheme:
        /// - Background
        /// - Forground
        /// - other colors...
        /// </remarks>
        public Brush[] Palette { get; protected set; }

        /// <summary>
        /// Default constructior.
        /// </summary>
        /// <param name="palette">The collor palette.</param>
        public PaletteAttribute(params string[] palette)
        {
            Palette = new Brush[palette.Length];
            Color colorBufer;

            for (int i = 0; i < palette.Length; i++)
            {
                try
                {
                    var colorCode = palette[i];

                    // Skip if passed.
                    if (string.IsNullOrEmpty(colorCode)) continue;

                    // Trying to convert color code to the color.
                    colorBufer = (Color)ColorConverter.ConvertFromString(colorCode);
                }
                catch
                {
                    // Applying transperent in case of failure.
                    Palette[i] = Brushes.Transparent;
                    continue;
                }

                // Creating a brush from the color.
                Palette[i] = new SolidColorBrush(colorBufer);
            }
        }

        /// <summary>
        /// Applying palette to the element.
        /// </summary>
        /// <param name="element">Target GUI element.</param>
        /// <remarks>
        /// Applyable only to the <see cref="IPaletteCompatible"/> or
        /// `System.Windows.Controls.Control` elements.
        /// </remarks>
        public void ApplyLayoutOption(FrameworkElement element)
        {
            if (element is IPaletteCompatible paletteElement)
            {
                paletteElement.Palette = Palette;
            }
            else if (element is System.Windows.Controls.Control control)
            {
                try
                {
                    control.Background = Palette[0] ?? control.Background;
                    control.Foreground = Palette[1] ?? control.Foreground;
                }
                catch { };
            }
            else if (element is System.Windows.Controls.Panel panel)
            {
                try { panel.Background = Palette[0] ?? panel.Background; } catch { }
            }
        }
    }
}
