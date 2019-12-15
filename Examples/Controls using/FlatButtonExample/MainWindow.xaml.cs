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

namespace FlatButtonExample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Instiniating the button.
            var button = new WpfHandler.UI.Controls.FlatButton()
            {
                Label = "Code behid button"
            };

            // Adding the button to the layout.
            contentHolder.Children.Add(button);

            // Subscribing on the click event to handle the button.
            button.Click += CodeBehindButton_Click;
        }

        /// <summary>
        /// Handler binded to the button instiniated from the code behind.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CodeBehindButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("The code behind button pressed.");
        }

        /// <summary>
        /// Handler binded to the flat button OnClick event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FlatButton_CMB_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("The button decalred via XAML pressed.");
        }
    }
}
