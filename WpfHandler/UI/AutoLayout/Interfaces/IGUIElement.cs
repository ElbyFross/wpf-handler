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
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfHandler.UI.AutoLayout
{
    /// <summary>
    /// Implementing of that interface allow to modify current layout during calling.
    /// </summary>
    public interface IGUIElement
    {
        /// <summary>
        /// Modify current layer's layout according to GUI element requirments.
        /// Calls once during UI spawn.
        /// </summary>
        /// <param name="layer">Target GUI layer.</param>
        /// <param name="args">Shared arguments.</param>
        void OnLayout(ref LayoutLayer layer, params object[] args);
    }
}
