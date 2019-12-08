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

namespace WpfHandler.UI.Controls
{
    /// <summary>
    /// Provides uinform way to operate selecteble elements.
    /// </summary>
    public interface ISelectableControl
    {
        /// <summary>
        /// Is that elemet selected.
        /// </summary>
        bool Selected { get; set; }

        /// <summary>
        /// Group of buttons that will allow auto deselect other buttons from that group.
        /// </summary>
        string Group { get; set; }

        /// <summary>
        /// Is group allow few selected buttons in one group.
        /// </summary>
        bool MultiSelection { get; set; }
    }
}
