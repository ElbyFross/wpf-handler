﻿//Copyright 2019 Volodymyr Podshyvalov
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
using WpfHandler.UI.Controls;
using WpfHandler.UI.AutoLayout;
using WpfHandler.UI.AutoLayout.Configuration;
using WpfHandler.UI.AutoLayout.Markups;

namespace WpfHandler.UI.AutoLayout.Options
{
    /// <summary>
    /// Redefines a label's width for the ILable objects.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property |
                    AttributeTargets.Class | AttributeTargets.Struct,
                    AllowMultiple = false, Inherited = true)]
    public class LabelWidthAttribute : LayoutSizeAttribute, ISharableGUILayoutOption
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
        /// <param name="width">Target label width.</param>
        public LabelWidthAttribute(double width) : base(width) { }

        /// <summary>
        /// Define max allowed height of the GUI element.
        /// NaN size value will skiped.
        /// </summary>
        /// <param name="element">Shared UI element. Must has implemented <see cref="ILabel"/> interface.</param>
        public void ApplyLayoutOption(FrameworkElement element)
        {
            // Skip if undefined.
            if (double.IsNaN(Size)) return;

            // Buferize shared element.
            BindedLabel = element;

            // Try to cast as label.
            if (element is ILabel label)
            {
                try
                {
                    // Apply size if element loaded.
                    if (element.IsLoaded)
                    {
                        // Applying shared size.
                        label.LabelWidth = (float)Size;
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
                    MessageBox.Show("Label width configuration failed.\n\n Details:\n" + ex.Message);
                }
            }
        }

        /// <summary>
        /// Occurs when label element will load.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LabelWidthAttribute_Loaded(object sender, RoutedEventArgs e)
        {
            // Apply requested size.
            if (sender is ILabel label)
            {
                try
                {
                    label.LabelWidth = (float)Size;
                }
                catch (NotSupportedException) 
                { 
                    // Not important 
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        "ILable `" + label.Label + "` cause an exception" +
                        " during applying of the " +
                        "LabelWidth parameter.\n\nDetails:\n" + ex.Message);
                }
            }
        }
    }
}
