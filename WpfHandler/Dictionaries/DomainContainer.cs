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

namespace WpfHandler.Dictionaries
{
    /// <summary>
    /// Class that provide information about dictionary domain.
    /// </summary>
    public class DomainContainer
    {
        /// <summary>
        /// Key code of this dictionary.
        /// </summary>
        public string key;

        /// <summary>
        /// Full domain of plugin.
        /// </summary>
        public string pluginDomain;

        /// <summary>
        /// Path to dictionary xaml.
        /// </summary>
        public string path;
    }
}
