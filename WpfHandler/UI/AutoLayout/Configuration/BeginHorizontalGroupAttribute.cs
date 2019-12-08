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
using System.Windows.Markup;
using System.Windows.Controls;
using WpfHandler.UI.AutoLayout;

namespace WpfHandler.UI.AutoLayout.Configuration
{
    /// <summary>
    /// Starting horizontal layout group.
    /// Will wait EndHorizontal to over the last begined group.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class BeginHorizontalGroupAttribute : Attribute, ILayerBeginAttribute
    {
        /// <summary>
        /// Layer that opereted into the handler.
        /// </summary>
        public LayoutLayer Layer
        {
            get { return _Layer; }
        }

        /// <summary>
        /// Bufer that contains layer.
        /// </summary>
        private LayoutLayer _Layer;

        /// <summary>
        /// Going one layer deeper into UI layout.
        /// All child element will be placed in horizontal order till calling of the EndGroup element.
        /// </summary>
        /// <param name="layer">Curent layer. Refernece eill be changed to the new layer after performing.</param>
        /// <param name="args">Not using into that elelment.</param>
        public void OnLayout(ref LayoutLayer layer, params object[] args)
        {
            // Create grid.
            IAddChild root  = new Grid();

            // Set new layer.
            layer = layer.GoDeeper(root);
            _Layer = layer;
        }
    }
}
