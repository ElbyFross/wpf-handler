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
using System.Windows.Controls;

namespace WpfHandler.Plugins
{
    /// <summary>
    /// Class that profide simplifyed way to integrate WPF plugins to client.
    /// </summary>
    public static class API
    {
        /// <summary>
        /// Load plugins from assembly and instiniate them to list.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<IPlugin> LoadPluginsEnumerable()
        {
            // Load query's processors.
            System.Reflection.Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            //Console.WriteLine("ASSEMBLIES PROCEED: {0}\n", assemblies.Length);
            Console.WriteLine("\nDETECTED PLUGINS:");
            foreach (System.Reflection.Assembly assembly in assemblies)
            {
                // Get all types for assembly.
                foreach (System.Type type in assembly.GetTypes())
                {
                    // Check if this type is subclass of query.
                    if (type.GetInterface("IPlugin") != null)
                    {
                        // Instiniating querie processor.
                        Plugins.IPlugin instance = (Plugins.IPlugin)Activator.CreateInstance(type);
                        Console.WriteLine("{0}", type.Name);
                        yield return instance;
                    }
                }
            }
        }

        /// <summary>
        /// Load plugins from assembly and instiniate them to list.
        /// </summary>
        /// <param name="list"></param>
        public static System.Collections.ObjectModel.ObservableCollection<IPlugin> LoadPluginsCollection()
        {
            System.Collections.ObjectModel.ObservableCollection<IPlugin> collection = 
                new System.Collections.ObjectModel.ObservableCollection<IPlugin>();

            // Load query's processors.
            System.Reflection.Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            //Console.WriteLine("ASSEMBLIES PROCEED: {0}\n", assemblies.Length);
            Console.WriteLine("\nDETECTED PLUGINS:");
            foreach (System.Reflection.Assembly assembly in assemblies)
            {
                // Get all types for assembly.
                foreach (Type type in assembly.GetTypes())
                {
                    // Check if this type is subclass of query.
                    if (!type.IsAbstract && 
                        !type.IsInterface &&
                        type.GetInterface("IPlugin") != null)
                    {
                        // Instiniating querie processor.
                        IPlugin instance = (IPlugin)Activator.CreateInstance(type);
                        collection.Add(instance);
                        Console.WriteLine("{0}", type.Name);
                    }
                }
            }
            return collection;
        }

        /// <summary>
        /// Oppenig plugin GUI.
        /// </summary>
        /// <param name="plugin">Target plugin.</param>
        /// <param name="panelName">Name of the Panel control that would contrin content.</param>
        public static void OpenGUI(IPlugin plugin, string panelName = "canvas")
        {
            // Drop invalid types.
            if (!(plugin is UIElement pluginUI))
            {
                return;
            }

            Window main = Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive);
            if (main != null)
            {
                Panel panel = (Panel)main.FindName(panelName);

                if (panel != null)
                {
                    panel.Children.Clear();
                    panel.Children.Add(pluginUI);
                }
            }
        }

        /// <summary>
        /// Sorting items in collection by tomain recommended orders and hierarchy depth.
        /// </summary>
        /// <param name="plugins"></param>
        public static void SortByDomains(ICollection<IPlugin> plugins)
        {
            var pluginsList = plugins.ToList();

            pluginsList.Sort(
                delegate (IPlugin p1, IPlugin p2)
                {
                    // Split fomaints to parts.
                    string[] p1DomainParts = p1.Meta.domain.Split('.');
                    string[] p2DomainParts = p2.Meta.domain.Split('.');

                    int p1DomainOrderBufer = 0;
                    int p2DomainOrderBufer = 0;

                    int maxDepth = Math.Min(p1DomainParts.Length, p2DomainParts.Length);

                    // Compare on every available depth of domain until solution.
                    for (int i = 0; i < maxDepth; i++)
                    {
                        // Try to get orders
                        if (!Int32.TryParse(p1DomainParts[i].Split('_')[0], out p1DomainOrderBufer))
                            p1DomainOrderBufer = p1DomainParts[i].GetHashCode();
                        if (!Int32.TryParse(p2DomainParts[i].Split('_')[0], out p2DomainOrderBufer))
                            p2DomainOrderBufer = p2DomainParts[i].GetHashCode();

                        // If Plugin 1 get hiether order.
                        if (p1DomainOrderBufer < p2DomainOrderBufer)
                            return -1;

                        // If Plugin 2 get hiether order.
                        if (p1DomainOrderBufer > p2DomainOrderBufer)
                            return 1;
                    }

                    // If Pligin 2 is subdomain of Pligin 1.
                    if (p2DomainParts.Length > p1DomainParts.Length)
                        return -1;

                    // If Pligin 1 is subdomain of Pligin 2.
                    if (p1DomainParts.Length > p2DomainParts.Length)
                        return 1;

                    // If domain has the a one level of the depth and has a conflicts in order or has not a declered orders, then stop sorting.
                    return 0;
                });

            // Update uniform collection.
            plugins.Clear();
            foreach (IPlugin plugin in pluginsList)
            {
                plugins.Add(plugin);
            }
        }
    }
}
