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
using WpfHandler.UI.AutoLayout;
using WpfHandler.UI.AutoLayout.Controls;
using WpfHandler.UI.AutoLayout.Options;
using WpfHandler.UI.AutoLayout.Configuration;

namespace BusinessLogic.Descriptros
{
    /// <summary>
    /// UI Descriptor fot the tab with a form for adding a new data member to the database.
    /// </summary>
    [Foreground("White")]
    [Background("#C1625F")]
    [LabelWidth(100)]
    public class NewItemTabDescriptor : UIDescriptor
    {
        [Content("Back", null, "nit_back")]
        public Action ToTableTab;

        [HorizontalAlign(System.Windows.HorizontalAlignment.Center)]
        [Width(400)]
        public SubPanel form = new SubPanel();

        public class SubPanel : UIDescriptor
        {
            [Space(25)]
            public string title = "New item";
            public string description;
            public float price;

            [FontWeight(FontWeightAttribute.WeightType.ExtraBold)]
            [HorizontalAlign(System.Windows.HorizontalAlignment.Center)]
            public Action AddItem;
        }

        public NewItemTabDescriptor()
        {
            // Return back to the table as soon as data applyed.
            form.AddItem += delegate () { ToTableTab?.Invoke(); };
        }
    }
}
