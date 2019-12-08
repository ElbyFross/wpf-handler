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

namespace WpfHandler.UI.AutoLayout.Configuration
{
    /// <summary>
    /// Defines the types the compatible with the member.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true, Inherited = true)]
    public class TypesCompatibleAttribute : Attribute, IGUIElementBindingAttribute
    {
        /// <summary>
        /// Type that compatible with the member.
        /// </summary>
        public Type[] CompatibleWith;

        /// <summary>
        /// Configurating types compatible with the memeber.
        /// </summary>
        /// <param name="types">COmpatible types.</param>
        public TypesCompatibleAttribute(params Type[] types)
        {
            CompatibleWith = types ?? new Type[0];
        }
    }
}
