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
using System.Windows;
using BusinessLogic.Descriptros;
using WpfHandler.UI.AutoLayout;
using WpfHandler.UI.AutoLayout.Controls;
using WpfHandler.UI.AutoLayout.Options;
using WpfHandler.UI.AutoLayout.Configuration;

namespace BusinessLogic
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        readonly DataTableDescriptor tableDescriptor = new DataTableDescriptor();
        readonly NewItemTabDescriptor newElementDescriptor = new NewItemTabDescriptor();
        readonly AutoLayoutVeiw tableView = new AutoLayoutVeiw();
        readonly AutoLayoutVeiw newElementView = new AutoLayoutVeiw();

        public MainWindow()
        {
            InitializeComponent();

            // Declaring Palette as global sharable option.
            var sharedOptions = new ISharableGUILayoutOption[] 
            { new PaletteAttribute("#A69272", "#FCFEFF", "#D8E6F2", "#8396A6", "#F2E8D8")};

            // Applying sharable objects to the descriptors.
            // In that case sharable options will affect not only elements instantiated by descriptor
            // but also the descriptor Panel itself.
            //tableDescriptor.SharedLayoutOptions = sharedOptions;
            newElementDescriptor.SharedLayoutOptions = sharedOptions;

            tableDescriptor.Loaded += delegate (UIDescriptor _)
            {
                tableDescriptor.controlPanel.NewItemTab += async delegate ()
                {
                    // Switches to the `New item` tab.
                    await switchPanel.SwitchToAsync(newElementView);
                };
            };
            
            // Defining behavior for `ToTableTab` action at the `New item` form.
            newElementDescriptor.ToTableTab += async delegate ()
            {
                // Switches back to the `Table` tab.
                await switchPanel.SwitchToAsync(tableView);
            };

            // Appllying descriptor instances to the AutoLayoutVeiw.
            tableView.Descriptor = tableDescriptor;
            newElementView.Descriptor = newElementDescriptor;

            // Defining current active tab.
            switchPanel.Current = tableView;
        }
    }
}
