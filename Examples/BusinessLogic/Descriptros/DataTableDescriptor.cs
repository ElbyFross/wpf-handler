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

namespace BusinessLogic.Descriptros
{
    // Defining thge text color for content of that descriptor.
    [Foreground("White")]
    public class DataTableDescriptor : UIDescriptor
    {
        // Definig foreground collor for this descriptor.
        // Because the fact that descriptor is isolated UI entity the attributes of descriptor from
        // the top will not affect the internal content.
        [Foreground("White")]

        // Setting alighn to the center (left by default).
        [HorizontalAlign(HorizontalAlignment.Center)]
        public class ControlPanel : UIDescriptor
        {
            // Starting horizontal layout.
            [BeginHorizontalGroup]

            // Binding element to the localization system.
            [Content("Refresh", null, "dtt_refresh")]
            // `Action` and `RoutingEvetnHandler` member instinated FlatButton by default.
            public Action Refresh;

            [Content("Add element", null, "dtt_add")]
            public Action NewItemTab;
        }
        
        // Applying custom background color to the control panel.
        [Background("#C1625F")]
        public ControlPanel controlPanel = new ControlPanel();

        [Background("#C1625F")]
        // Casomizing the font weight for the header pabel.
        [FontWeight(FontWeightAttribute.WeightType.DemiBold)]
        public HeaderDescriptor header = new HeaderDescriptor();

        // Making a space into layout.
        [Space]

        // Limiting the element height.
        [MaxHeight(200)]
        // Configurating AutoCollection element.
        [AutoCollectionProperties(
            AddButtonVisibile = false, // Disabling add button. Element will be added via the another tab.
            DragAllowed = false, // Disablin elements reorder, because user not controls an order of the database source.
            BackplateBackground = "SteelBlue")] // defining backplate color of the collection element.
        public List<TableRowDescriptor> table = new List<TableRowDescriptor>();
    }
}
