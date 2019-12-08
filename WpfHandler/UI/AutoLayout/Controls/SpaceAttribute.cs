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
using System.Windows.Controls;
using WpfHandler.UI.AutoLayout;
using WpfHandler.UI.Controls;

namespace WpfHandler.UI.AutoLayout.Controls
{
    /// <summary>
    /// Adding space between UI elements.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class SpaceAttribute : Attribute, ILayoutSize, IGUIElement
    {
        /// <summary>
        /// Size of the space.
        /// </summary>
        public double Size { get; set; } = double.NaN;

        /// <summary>
        /// Initialize space into 10 points.
        /// </summary>
        public SpaceAttribute()
        {
            Size = 10;
        }

        /// <summary>
        /// Set custom step value.
        /// </summary>
        /// <param name="space">Size of step.</param>
        public SpaceAttribute(float space)
        {
            this.Size = space;
        }

        /// <summary>
        /// Instiniating Space GUI lement.
        /// </summary>
        /// <param name="layer">Target GUI layer.</param>
        /// <param name="args">Not using in that element.</param>
        public void OnLayout(ref LayoutLayer layer, params object[] args)
        {
            // Instiniate GUI element.
            var canvas = new Canvas();

            // Convifurate layout's size.
            if (layer.orientation == Orientation.Horizontal)
            {
                canvas.Width = Size;
            }
            else
            {
                canvas.Height = Size;
            }

            // Add element to the root.
            layer.root.AddChild(canvas);
        }
    }
}
