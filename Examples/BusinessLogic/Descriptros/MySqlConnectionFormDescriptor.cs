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
using System.Windows;
using WpfHandler.UI;
using WpfHandler.UI.AutoLayout;
using WpfHandler.UI.AutoLayout.Options;
using WpfHandler.UI.AutoLayout.Markups;
using WpfHandler.UI.AutoLayout.Configuration;
using WpfHandler.UI.AutoLayout.Controls;
using WpfHandler.UI.Controls;

namespace BusinessLogic.Descriptros
{
    /// <summary>
    /// Descriptor for MySql server connection form.
    /// </summary>
    public class MySqlConnectionFormDescriptor : UIDescriptor
    {
        [BeginHorizontalGroup]

            [MinWidth(350)]
            [HorizontalAlign(HorizontalAlignment.Center)]
            [Background("#ff4757")]
            [Foreground("#FCFEFF")]
            [FontSize(20)]
            public GUIContent header = new GUIContent("Configurate connection params", null, "serverCon_header");
        
            [Width(40)]
            [Background("#ff4757")]
            public LangPanelDescriptor lang;

        [EndGroup]

        [MinWidth(350)]
        [HorizontalAlign(HorizontalAlignment.Center)]
        [Background("#D8E6F2")]
        public GUIContent description = new GUIContent(
            "Set the parameters required to connection a MySQL server.", null, "serverCon_desc");

        [Space(20)]

        [Width(250)]
        [Content("Server", null, "serverCon_server")]
        public string server = "127.0.0.1";

        [Width(250)]
        [Content("Database", null, "serverCon_db")]
        public string database = "wpfh-examples";

        [Width(250)]
        [Content("Port", null, "serverCon_port")]
        public int port = 3306;

        [Width(250)]
        [Content("User ID", null, "serverCon_uid")]
        public string userId = "root";

        [Width(250)]
        [Content("Password", null, "serverCon_pass")]
        public string password;

        [Space]

        [HorizontalAlign(HorizontalAlignment.Center)]
        [BeginHorizontalGroup]
        [Content("Cancel", null, "serverCon_cancel")]
        public Action cancel;

        [HorizontalAlign(HorizontalAlignment.Center)]
        [Content("Connect", null, "serverCon_connect")]
        public Action Connect;
    }
}
