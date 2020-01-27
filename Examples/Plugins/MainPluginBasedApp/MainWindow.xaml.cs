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

namespace MainPluginBasedApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// An empty constructor.
        /// </summary>
        public MainWindow()
        {
            #region Only for example
            _ = new ExamplePlugin1.MainPluginControl();
            _ = new ExamplePlugin2.PluginControl();
            #endregion

            // Loading assemblies for a plugins directory.
            UniformDataOperator.AssembliesManagement.AssembliesHandler.LoadAssemblies(
                AppDomain.CurrentDomain.BaseDirectory + "plugins\\");

            // Looking for replaced types that could be used by handlers.
            UniformDataOperator.AssembliesManagement.Modifiers.TypeReplacer.RescanAssemblies();

            InitializeComponent();

            // Subscribing on the event that will occurs after control loading.
            Loaded += MainWindow_Loaded;            
        }

        /// <summary>
        /// A callback for handling of the loading event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // Starting a first plugin.
                catalogView.StartPlugin(0);
            }
            catch (IndexOutOfRangeException)
            {
                MessageBox.Show("Plugins not found.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("First plugin caused exception.\n\nDetails:\n" + ex.Message);
            }
        }
    }
}
