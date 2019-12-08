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
using WpfHandler.UI.AutoLayout;

namespace WpfHandler.UI.AutoLayout.Configuration
{
    /// <summary>
    /// Close the last started layout group.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class EndGroupAttribute : Attribute, ILayerEndAttribute
    {
        /// <summary>
        /// Reference to the layer that had been got by handler doring GoUpper operation.
        /// </summary>
        public LayoutLayer Layer
        {
            get { return _Layer; }
        }

        /// <summary>
        /// Bufer that contains operated layer.
        /// </summary>
        private LayoutLayer _Layer;

        /// <summary>
        /// Trying to go to the upper UI's layer.
        /// </summary>
        /// <param name="layer">Current layer. Reference will be changed on relevant one.</param>
        /// <param name="args">Not using in that element.</param>
        public void OnLayout(ref LayoutLayer layer, params object[] args)
        {
            // Trying to go to the upper UI's layer.
            layer = layer.GoUpper();
            _Layer = layer;
        }
    }
}
