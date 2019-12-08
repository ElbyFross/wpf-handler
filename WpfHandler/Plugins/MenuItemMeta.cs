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
using System.Text;
using System.Linq;

namespace WpfHandler.Plugins
{
    /// <summary>
    /// Contain information that will be used for connection to the automatic navigation menu in client application 
    /// </summary>
    [System.Serializable]
    public class MenuItemMeta
    {
        /// <summary>
        /// Path of item in menu hierarchy.
        /// 
        /// Domain fragment format: [prority]_domainPart.
        /// Priority will be used for sorting of plugins in menu. 
        /// If not defined then will be auto changed to SUBDOMAIN.GetHashcode().
        /// </summary>
        /// 
        /// <remarks>
        /// Attention: 
        /// 0_DomainName != DomainName
        /// 0_DomainName.SubdomainName != 10_DomainName.SubdomainName
        /// 
        /// Example of plugins menu map:
        /// 0_main
        ///     0_main.0_plugin_1
        ///     0_main.1_plugin_2
        ///     
        /// big_plugin_3
        ///     big_plugin_3.10_plugin1
        ///     big_plugin_3.20_pl_2
        ///         big_plugin_3.20_pl_2.minor_plugin
        ///     big_plugin_3.p_3
        ///     
        /// big_plugin_3 // Dublicated plugin's domain. Will be added to menu but all childs will applied to first entry plugin.
        /// </remarks>
        public string domain = "0_main.0_new_plugin";

        /// <summary>
        /// Code of resource in language xaml dictionary that will contain translated title.
        /// </summary>
        /// <remarks>
        /// For avoidance of conflicts recommended naming format is: "p_" + author + "_" + plugin_name + "_" + title.
        /// </remarks>
        public string titleDictionaryCode = "p_author_myPlugin_title";

        /// <summary>
        /// Title that will be showed in case if dictionary not found.
        /// </summary>
        public string defaultTitle = null;


        #region Constructors
        /// <summary>
        /// Default empty constructor.
        /// </summary>
        public MenuItemMeta() { }

        /// <summary>
        /// Initializing core memebers.
        /// </summary>
        /// <param name="domain">Item domain into hierarchy.</param>
        /// <param name="titleDictionaryCode">The resource key in XAML resource dictionary for getting the title.</param>
        public MenuItemMeta(string domain, string titleDictionaryCode)
        {
            this.domain = domain;
            this.titleDictionaryCode = titleDictionaryCode;
        }
        #endregion
    }
}