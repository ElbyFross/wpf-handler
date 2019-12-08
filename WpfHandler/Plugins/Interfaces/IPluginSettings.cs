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

namespace WpfHandler.Plugins
{
    /// <summary>
    /// Provide possibility to implement setting ui block in application.
    /// </summary>
    public interface IPluginSettings
    {
        /// <summary>
        /// Meta data that contains description for main menu integration.
        /// </summary>
        MenuItemMeta Meta { get; set; }

        /// <summary>
        /// Return control that can be displayed as block of settings menu.
        /// </summary>
        UserControl GUI { get; }
    }
}
