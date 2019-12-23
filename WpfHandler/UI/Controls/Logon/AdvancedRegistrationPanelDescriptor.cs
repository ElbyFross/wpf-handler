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
using WpfHandler.UI.AutoLayout;
using WpfHandler.UI.AutoLayout.Controls;
using WpfHandler.UI.AutoLayout.Configuration;
using WpfHandler.UI.AutoLayout.Options;

namespace WpfHandler.UI.Controls.Logon
{
    /// <summary>
    /// A form for storing advanced user's info.
    /// </summary>
    [Serializable]
    public class AdvancedRegistrationPanelDescriptor : FormDescriptor
    {
        /// <summary>
        /// An user email.
        /// </summary>
        public string Email { get; set; } = "";

        /// <summary>
        /// An user cell.
        /// </summary>
        public string ContactPhone { get; set; } = "";

        /// <summary>
        /// A country where have been living a user.
        /// </summary>
        [Space(12)]
        public string Country { get; set; } = "";

        /// <summary>
        /// A city where have been living a user.
        /// </summary>
        public string City { get; set; } = "";

        /// <summary>
        /// A year of a user birth.
        /// </summary>
        [Space(12)]
        [Width(160)]
        [BeginHorizontalGroup]
        [Content("Birthday: YYYY", null, "regFormAdvc_bYear")]
        public int BirthYear { get; set; } = 1960;

        /// <summary>
        /// A month of a user birth.
        /// </summary>
        [Content("MM", null, "regFormAdvc_bMonth")]
        [Width(60)]
        [LabelWidth(35)]
        [HorizontalAlign(System.Windows.HorizontalAlignment.Left)]
        public int BirthMonth { get; set; } = 1;

        /// <summary>
        /// A day of a user birth.
        /// </summary>
        [Content("DD", null, "regFormAdvc_bDay")]
        [Width(60)]
        [LabelWidth(35)]
        [HorizontalAlign(System.Windows.HorizontalAlignment.Left)]
        public int BirthDay { get; set; } = 1;
    }
}
