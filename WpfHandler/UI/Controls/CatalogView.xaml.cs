using System;
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
using System.Collections.ObjectModel;
using WpfHandler.Plugins;

namespace WpfHandler.UI.Controls
{
    /// <summary>
    /// Interaction logic for CatalogView.xaml
    /// </summary>
    public partial class CatalogView : UserControl
    {
        /// <summary>
        /// A collection that contains all loaded plugins.
        /// </summary>
        public ObservableCollection<IPlugin> Plugins { get; set; }

        /// <summary>
        /// A collection that contains menu's controls.
        /// </summary>
        public ObservableCollection<FrameworkElement> MenuButtons { get; protected set; } =
            new ObservableCollection<FrameworkElement>();

        /// <summary>
        /// A default constructor.
        /// </summary>
        public CatalogView()
        {
            InitializeComponent();

            try
            {
                UpdateCatolog();
            }
            catch { };
        }
        
        /// <summary>
        /// Updating the collection of catalog items.
        /// </summary>
        public void UpdateCatolog()
        {
            // Load plugins.
            Plugins = API.LoadPluginsCollection();

            // Sort plugins
            API.SortByDomains(Plugins);

            #region Load main menu      
            // Add hardcoded UI to collection.
            foreach (FrameworkElement fe in MainMenu.Items)
            {
                MenuButtons.Add(fe);
            }

            // Connect all plugins to main menu to provide access via UI.
            foreach (IPlugin plugin in Plugins)
            {
                if (plugin.Meta != null)
                {
                    // Compute hierarchy level
                    int _hierarchyLevel = plugin.Meta.domain.Split('.').Length;

                    // Add space before paragraph.
                    if (_hierarchyLevel <= 1)
                    {
                        MenuButtons.Add(new ItemsControl() { Height = 20 });
                    }

                    // Try to load name from dictionary.
                    string title = null;
                    try
                    {
                        // load title from dictionary.
                        title = FindResource(plugin.Meta.titleDictionaryCode) as string;
                    }
                    catch
                    {
                        // Set default title or dict code if title not found.
                        title = plugin.Meta.defaultTitle ?? plugin.Meta.titleDictionaryCode;
                    }

                    // Create button by meta.
                    MenuButtons.Add(
                    new CatalogButton()
                    {
                        Label = title,
                        // Set uniformed text offset in hierarchy tree.
                        HierarchyLevel = _hierarchyLevel,
                        // Set root level as bool, others as thin.
                        FontWeight = _hierarchyLevel > 1 ? FontWeights.Thin : FontWeights.SemiBold,
                        // Setup plugin activator
                        ClickCallback = plugin.OnStart
                    });
                }
            }

            // Clear previos collection.
            MainMenu.Items.Clear();
            // Apply plugins to item source.
            MainMenu.ItemsSource = MenuButtons;
            #endregion
        }

        /// <summary>
        /// Starting th eplugin by index.
        /// </summary>
        /// <param name="index">Index of the loaded plugin.</param>
        /// <exception cref="IndexOutOfRangeException">
        /// Occurs when index not valid relative for a loaded plugins list.
        /// </exception>
        public void StartPlugin(int index)
        {
            Plugins[index].OnStart(this);
        }

        /// <summary>
        /// Starting the plugin by a <see cref="MenuItemMeta.domain"/> value.
        /// </summary>
        /// <param name="domain">A plugins's domain name.</param>
        /// <exception cref="KeyNotFoundException">
        /// Occurs in case if the domain not found among loaded plugins.
        /// </exception>
        public void StartPlugin(string domain)
        {
            // Check an every loaded plugin.
            foreach(IPlugin plugin in Plugins)
            {
                // Comapring the domains.
                if(plugin.Meta.domain.Equals(domain))
                {
                    plugin.OnStart(this);
                    return;
                }
            }

            // Throwing exception cause the plugin not found.
            throw new KeyNotFoundException(
                "Plugin with a requested domein \"" + 
                domain + "\" not found among loaded.");
        }
    }
}
