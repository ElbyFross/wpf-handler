﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfHandler.Plugins;

namespace ExamplePlugin1
{
    /// <summary>
    /// Interaction logic for SubPlugin2Control.xaml
    /// </summary>
    public partial class SubPlugin2Control : UserControl, IPlugin
    {
        public MenuItemMeta Meta
        { get; set; } = new MenuItemMeta()
        {
            // The name of the plugin in case if localized data not found.
            defaultTitle = "Sub plugin 2",

            // Domain of the plugin into the hierarchy.
            domain = "0_main.1_sp",

            // The key from that will be looked into the localization dictionaries.
            titleDictionaryCode = "sp1dc"
        };

        public void OnStart(object sender)
        {
            // Request changing of GUI.
            API.OpenGUI(this);
        }

        public SubPlugin2Control()
        {
            InitializeComponent();
        }
    }
}
