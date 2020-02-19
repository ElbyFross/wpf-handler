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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
using System.Reflection;
using System.Windows.Controls;
using WpfHandler.UI.AutoLayout.Configuration;


namespace WpfHandler.UI.Virtualization
{
    /// <summary>
    /// Collection that will descplay only the elements displayed at the visible page.
    /// </summary>
    public interface IVirtualizedCollection
    {
        /// <summary>
        /// Defines is virtalization enable or not.
        /// </summary>
        bool IsVirtualized { get; set; }

        /// <summary>
        /// How many items will instatiated during one tic.
        /// </summary>
        int VirtualizedItemsPack { get; set; }

        /// <summary>
        /// List with virtualized items.
        /// </summary>
        List<VirtualizedItemMeta> VirtualizedElements { get; }

        /// <summary>
        /// Is collection must uncload and descroy controls out of the view bounds?
        /// </summary>
        bool UnloadHidded { get; set; }
    }
}
