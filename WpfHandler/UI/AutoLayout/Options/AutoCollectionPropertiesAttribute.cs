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
using WpfHandler.UI.Controls;

namespace WpfHandler.UI.AutoLayout.Options
{
    /// <summary>
    /// Allow to define <see cref="AutoCollection"/> properties.
    /// </summary>
    public class AutoCollectionPropertiesAttribute : Attribute, IGUILayoutOption
    {
        /// <summary>
        /// Radius of the rounded corders.
        /// Will affect default only if greater or equal to 0.
        /// </summary>
        public double CornerRadius { get; set; } = -1.0f;

        /// <summary>
        /// Is the spliting lines between content elements are visible.
        /// </summary>
        public bool SplitersDraw { get; set; } = true;

        /// <summary>
        /// Color of the spliters.
        /// </summary>
        public string SpliterColor { get; set; } = null;

        /// <summary>
        /// Color of the backplate.
        /// </summary>
        public string BackplateBackground { get; set; } = null;

        /// <summary>
        /// If the add button available for an user.
        /// </summary>
        public bool AddButtonVisibile { get; set; } = true;

        /// <summary>
        /// Is the remove button available for an user.
        /// </summary>
        public bool RemoveButtonVisibile { get; set; } = true;


        /// <summary>
        /// Applying collection properties to the elements if it's inheirted from the <see cref="AutoCollection"/>
        /// </summary>
        /// <param name="element">The target UI element.</param>
        /// <remarks>
        /// Not causing an error if applying to element with a different type.
        /// </remarks>
        public void ApplyLayoutOption(FrameworkElement element)
        {
            // Trying to get the collection object.
            var collection = element as AutoCollection;

            // Drop id not supported.
            if (collection == null) return;

            // Configurating the control buttons.
            collection.AddButtonVisibile = AddButtonVisibile;
            collection.RemoveButtonVisibile = RemoveButtonVisibile;

            // Setting splitters color if defined.
            if (SpliterColor != null)
            {
                try
                {
                    // Trying to get the color.
                    Brush color = new SolidColorBrush(
                    (Color)ColorConverter.ConvertFromString(SpliterColor));

                    collection.SpliterColor = color;
                }
                catch { };
            }

            // Setting backplate color if defined.
            if (BackplateBackground != null)
            {
                try
                {
                    // Trying to get the color.
                    Brush color = new SolidColorBrush(
                    (Color)ColorConverter.ConvertFromString(BackplateBackground));

                    // Applying the color.
                    collection.BackplateBackground = color;
                } 
                catch { };
            }
            
            // Applying radiuses if requested.
            if (CornerRadius >= 0.0d)
            {
                collection.CornerRadius = CornerRadius;
            }
        }
    }
}
