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

using WpfHandler.UI;
using WpfHandler.UI.AutoLayout;
using WpfHandler.UI.AutoLayout.Options;
using WpfHandler.UI.AutoLayout.Configuration;

namespace BusinessLogic.Descriptros
{
    public class HeaderDescriptor : UIDescriptor
    {
        [BeginHorizontalGroup]
        [Width(50)]
        public GUIContent idLabel = new GUIContent("| ID", null, "th_id");
        [Width(250)]
        public GUIContent titleLabel = new GUIContent("| Title", null, "th_title");
        public GUIContent descLabel = new GUIContent("| Description", null, "th_desc");
        [Width(50)]
        public GUIContent priceLabel = new GUIContent("| Price", null, "th_price");
    }
}
