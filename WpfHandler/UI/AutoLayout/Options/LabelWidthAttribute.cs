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
    /// Redefines lable width for the ILable objects.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property |
                    AttributeTargets.Class | AttributeTargets.Struct,
                    AllowMultiple = false, Inherited = true)]
    public class LabelWidthAttribute : LayoutSizeAttribute, IGUILayoutOption
    {
        /// <summary>
        /// Lalbe binded to that attribute.
        /// </summary>
        public FrameworkElement BindedLabel { get; protected set; }

        /// <summary>
        /// Default constructor.
        /// Using auto width.
        /// </summary>
        public LabelWidthAttribute() : base() { }

        /// <summary>
        /// Set requested maximum height as Size.
        /// </summary>
        /// <param name="width">Target lable width.</param>
        public LabelWidthAttribute(double width) : base(width) { }

        /// <summary>
        /// Define max allowed height of the GUI element.
        /// NaN size value will skiped.
        /// </summary>
        /// <param name="element">Shared UI element. Must has implemented @ILable interface.</param>
        public void ApplyLayoutOption(FrameworkElement element)
        {
            // Skip if undefined.
            if (double.IsNaN(Size)) return;

            // Buferize shared element.
            BindedLabel = element;

            // Try to cast as lable.
            if (element is ILabel lable)
            {
                try
                {
                    // Apply size if element loaded.
                    if (element.IsLoaded)
                    {
                        // Applying shared size.
                        lable.LabelWidth = (float)Size;
                    }
                    // Wait till loading in other case.
                    else
                    { 
                        element.Loaded += LabelWidthAttribute_Loaded;
                    }
                }
                catch (NotSupportedException) { } // Not important if not supported.
                catch (Exception ex)
                {
                    // Log error.
                    MessageBox.Show("Lable width configuration failed.\n\n Details:\n" + ex.Message);
                }
            }
        }

        /// <summary>
        /// Occurs when lable element will load.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LabelWidthAttribute_Loaded(object sender, RoutedEventArgs e)
        {
            // Unsubscribing from events.
            BindedLabel.Loaded -= LabelWidthAttribute_Loaded;

            // Apply requested size.
            if (BindedLabel is ILabel lable)
            {
                lable.LabelWidth = (float)Size;
            }
        }
    }
}
