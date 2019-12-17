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
using WpfHandler.UI.Controls;

namespace SwitchPanelExample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Panles that will be handled via the SwitchPanel
        Panel1 panel1 = new Panel1();
        Panel2 panel2 = new Panel2();


        /// <summary>
        /// How many time will take swich animation of forms.
        /// </summary>
        public TimeSpan FormsAnimationDuration { get; set; }


        /// <summary>
        /// Current active panel.
        /// </summary>
        public UserControl CurrentPanel { get; set; }

        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Occurs when widow is loaded.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Applying animations duration.
            switchPanel.Duration = new TimeSpan(0, 0, 0, 0, 300);

            // Applying current panel.
            switchPanel.Current = panel1;
            CurrentPanel = panel1;
        }


        // Occurs when the "next panel" button declared at the xaml clicked.
        private void NextPanelButton_Click(object sender, RoutedEventArgs e)
        {
            // Switch the current active panel.
            if (CurrentPanel is Panel1) CurrentPanel = panel2;
            else CurrentPanel = panel1;

            // Requiesting swith to the next panel.
            // Using an "Alpha swipe animation.
            switchPanel.SwitchTo(CurrentPanel, SwitchPanel.AnimationType.AlphaSwipe);
        }
    }
}
