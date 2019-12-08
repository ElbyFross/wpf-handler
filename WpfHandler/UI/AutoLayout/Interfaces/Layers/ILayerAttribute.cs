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

using System.Windows.Markup;

namespace WpfHandler.UI.AutoLayout
{
    /// <summary>
    /// Define attribute like one that operate with layout layers.
    /// </summary>
    public interface ILayerAttribute : IGUIElement
    {
        /// <summary>
        /// Layer that opereted into the handler.
        /// </summary>
        LayoutLayer Layer { get; }
    }
}
