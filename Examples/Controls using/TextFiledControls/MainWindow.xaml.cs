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

namespace TextFiledControls
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            #region Bind descriptor to the autolayout
            // Instiniating our custom UI descriptor.
            var descriptor = new CustomAutolayoutDescriptor();

            // Applying the descriptor to the autolayot view.
            // After that the view will generate and bind the fields.
            alView.Descriptor = descriptor;
            #endregion


            #region Adding the field via code
            var codeBehidField = new WpfHandler.UI.Controls.FlatTextBox()
            {
                Label = "Code behind field",
                Value = "0.34",
                ValueMode = WpfHandler.UI.Controls.TextFieldControl.Mode.Float,
            };
            contentHolder.Children.Add(codeBehidField);
            #endregion
        }

        /// <summary>
        /// Occurs when filed's value is changed.
        /// </summary>
        /// <param name="obj">The reference to the sender field.</param>
        private void FlatPasswordBox_ValueChanged(WpfHandler.UI.AutoLayout.IGUIField obj)
        {
            output.Content = obj.Value;
        }
    }
}
