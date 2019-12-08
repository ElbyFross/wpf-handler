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

namespace WpfHandler.UI.Controls
{
    /// <summary>
    /// Attribute for managing layout size of some element.
    /// Provides base members.
    /// </summary>
    public abstract class LayoutSizeAttribute : Attribute, ILayoutSize
    {
        /// <summary>
        /// Value that will be used in the element's propeties.
        /// </summary>
        public double Size { get; set; } = double.NaN;

        /// <summary>
        /// Default constructor.
        /// Use auto width.
        /// </summary>
        public LayoutSizeAttribute() { }

        /// <summary>
        /// Set requested width as Size.
        /// </summary>
        /// <param name="width"></param>
        public LayoutSizeAttribute(double width)
        {
            Size = width;
        }
    }
}
