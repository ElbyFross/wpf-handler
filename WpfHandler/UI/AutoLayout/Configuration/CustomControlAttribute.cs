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
using WpfHandler.UI.AutoLayout;
using WpfHandler.UI.AutoLayout.Controls;

namespace WpfHandler.UI.AutoLayout.Configuration
{
    /// <summary>
    /// Allow to override of default control for relative field type to custom one.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class CustomControlAttribute : Attribute
    {
        /// <summary>
        /// Control's type that would be instiniated into GUI.
        /// </summary>
        public Type ControlType
        {
            get { return _ControlType; }
            set
            {
                // Check if type has implementation of ILayoutControl
                if (value is IGUIField)
                {
                    _ControlType = value;
                }
                else
                {
                    // Log error.
                    throw new InvalidCastException("Allowed only types with implemented `"
                        + typeof(IGUIField).FullName + "` interface.");
                }
            }
        }

        /// <summary>
        /// Buffer tht contains stored type.
        /// </summary>
        private Type _ControlType;

        /// <summary>
        /// Configurating attribute.
        /// </summary>
        /// <param name="controlType">Type that would be instiniated during GUI spawn.</param>
        public CustomControlAttribute(Type controlType)
        {
            // Try to set value to the bufer.
            ControlType = controlType;
        }
    }
}
