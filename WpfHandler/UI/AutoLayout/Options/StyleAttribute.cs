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
using System.Windows;
using WpfHandler.UI.AutoLayout;

namespace WpfHandler.UI.AutoLayout.Options
{
    /// <summary>
    /// Define custom style from resources that would be applied to the GUI element.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class StyleAttribute : Attribute, IGUILayoutOption
    {
        /// <summary>
        /// Resource key that would be looking for style loading.
        /// </summary>
        public string ResourceKey { get; set; }

        /// <summary>
        /// Marker that enable or disabling showing of error messages.
        /// </summary>
        public bool ShowErrorMessages { get; set; } = true;

        /// <summary>
        /// Trying to apply requested style.
        /// </summary>
        /// <param name="element">Shared UI element.</param>
        public void ApplyLayoutOption(FrameworkElement element)
        {
            try
            {
                // Try to find requested resource.
                if (Application.Current.FindResource(ResourceKey) is System.Windows.Style style)
                {
                    // Apply style if found.
                    element.Style = style;
                }
            }
            catch
            {
                // Log error if requested.
                if (ShowErrorMessages)
                {
                    // Not found in dictionary. 
                    MessageBox.Show("Style by key `" + (ResourceKey ?? "None") + "` not found.");
                }
            }
        }
    }
}
