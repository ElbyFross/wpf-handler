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

namespace WpfHandler.Dictionaries
{
    /// <summary>
    /// Base attribute that binding UI element to the common auto localization system.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property |
                    AttributeTargets.Class | AttributeTargets.Struct,
                    AllowMultiple = true, Inherited = true)]
    public abstract class LocalizableContentAttribute : Attribute
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public LocalizableContentAttribute()
        {
            API.LanguagesDictionariesUpdated += LanguagesDictionariesUpdated;
        }

        /// <summary>
        /// Unsubscribe from events.
        /// </summary>
        ~LocalizableContentAttribute()
        {
            API.LanguagesDictionariesUpdated -= LanguagesDictionariesUpdated;
        }

        /// <summary>
        /// Occurs when would reloaded dynamic dictionaries.
        /// </summary>
        public abstract void LanguagesDictionariesUpdated();
    }
}
