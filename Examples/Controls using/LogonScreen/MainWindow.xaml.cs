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

namespace LogonScreen
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// /Occurs when user click on the Login (Enter) button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LogonScreen_LogonPanel_LoginCallback(object sender, RoutedEventArgs e)
        {

        }

        /// <summary>
        /// Occurs when user slick on the SingUp button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LogonScreen_LogonPanel_SingUpCallback(object sender, RoutedEventArgs e)
        {

        }

        /// <summary>
        /// Occurs when user continue registration after filling the base form.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LogonScreen_RegPanel_ContinueCallback(object sender, RoutedEventArgs e)
        {

        }

        /// <summary>
        /// Occurs when user cancel registration.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LogonScreen_RegPanel_BackCallback(object sender, RoutedEventArgs e)
        {

        }
    }
}
