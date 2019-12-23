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
using System.ComponentModel;

namespace WpfHandler.UI.AutoLayout.Configuration
{
    /// <summary>
    /// Defines an order of the member into an auto-generated UI.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field,
    Inherited = true, AllowMultiple = false)]
    [ImmutableObject(true)]
    public sealed class OrderAttribute : Attribute
    {
        private readonly int order;

        /// <summary>
        /// The prority order of an element.
        /// </summary>
        public int Order { get { return order; } }

        /// <summary>
        /// Default constuctor.
        /// </summary>
        /// <param name="order">The prority order of an element.</param>
        public OrderAttribute(int order) { this.order = order; }
    }
}
