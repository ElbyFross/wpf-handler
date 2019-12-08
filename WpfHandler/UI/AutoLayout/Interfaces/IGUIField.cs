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

namespace WpfHandler.UI.AutoLayout
{
    using System.Reflection;

    /// <summary>
    /// Implementation of that interface allow to use that control in auto layout user interfaces.
    /// </summary>
    public interface IGUIField : IGUIElement
    {
        /// <summary>
        /// Event that will occure in case if value of the field will be changed.
        /// Will cause updating of the BindedMember value.
        /// 
        /// IGUIField - sender.
        /// </summary>
        event System.Action<IGUIField> ValueChanged;

        /// <summary>
        /// Value of that control.
        /// </summary>
        object Value { get; set; }

        /// <summary>
        /// Memeber that will be used as source\target for the value into UI.
        /// </summary>
        MemberInfo BindedMember { get; set; }
    }
}
