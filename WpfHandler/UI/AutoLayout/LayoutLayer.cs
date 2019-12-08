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
using System.Windows.Markup;
using System.Reflection;
using System.Windows.Controls;


namespace WpfHandler.UI.AutoLayout
{
    /// <summary>
    /// Contains data about layout layer to relative element.
    /// </summary>
    public class LayoutLayer
    {
        /// <summary>
        /// Element on that will contains child elements.
        /// </summary>
        public IAddChild root;

        /// <summary>
        /// Parent layer that contains that element.
        /// </summary>
        public LayoutLayer Parent
        {
            get { return _Parent; }
            protected set
            {
                // Drop recursive reference.
                if (value.Equals(this)) return;

                // update value,
                _Parent = value;
            }
        }

        /// <summary>
        /// Check does the layer has a parent.
        /// </summary>
        public bool HasParent
        {
            get { return _Parent != null; }
        }

        /// <summary>
        /// Computing how many layers existed in that UI group.
        /// If Parent layer not exist still count itself as 1 layer.
        /// </summary>
        public int Depth
        {
            get { return Parent == null ? 1 : 1 + Parent.Depth; }
        }

        /// <summary>
        /// Bufer that containes reference to the parent layer.
        /// </summary>
        private LayoutLayer _Parent;

        /// <summary>
        /// Orientation of that UI layer.
        /// </summary>
        public Orientation orientation = Orientation.Vertical;

        /// <summary>
        /// Create next UI layer and set it as active.
        /// </summary>
        /// <param name="nextLayerRoot">Element that will be root of the new layer.</param>
        public LayoutLayer GoDeeper(IAddChild nextLayerRoot)
        {
            // Validate shared reference.
            if (nextLayerRoot == null)
            {
                throw new NullReferenceException("Root element can't be null.");
            }

            // Add shared root as child on current layer.
            root.AddChild(nextLayerRoot);

            // Configurate new layer.
            var newLayer = new LayoutLayer()
            {
                root = nextLayerRoot, // Set shared element as root
                Parent = this // Set reference to current active layer like on the parent.
            };

            // Define orientation of the layout group.
            if (nextLayerRoot is StackPanel)
                newLayer.orientation = Orientation.Vertical;
            else
                newLayer.orientation = Orientation.Horizontal;

            // Return new layer.
            return newLayer;
        }

        /// <summary>
        /// Change current layer to previous one.
        /// </summary>
        public LayoutLayer GoUpper()
        {
            // Check if current layout has a parent.
            if (HasParent)
            {
                // Return parent if exist.
                return Parent;
            }

            // Return this if that higher layer.
            return this;
        }

        /// <summary>
        ///  Applying UI element to the GUI.
        /// </summary>
        /// <param name="element">Shared UI element.</param>
        public void ApplyControl(FrameworkElement element)
        {
            // Drop invalid.
            if (element == null) return;

            // Adding herader to layout.
            if (orientation == Orientation.Horizontal)
                LayoutHandler.HorizontalLayoutAddChild(root, element);
            else
                LayoutHandler.VerticalLayoutAddChild(root, element);
        }
    }
}
