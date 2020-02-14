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

using WpfHandler.UI.AutoLayout;

namespace SampleShapesControlApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            alView.Descriptor = new ControlPanel();
            alView.Descriptor.ValueChanged += Descriptor_ValueChanged;
        }

        private void Descriptor_ValueChanged(UIDescriptor sender, IGUIField changedElement, object[] args)
        {
            if (!(sender is ControlPanel controlPanel)) throw new NotSupportedException();

            ellipseShape.Visibility = Visibility.Collapsed;
            rectShape.Visibility = Visibility.Collapsed;

            switch (controlPanel.shapeType)
            {
                case ShapeType.Ellipse: ellipseShape.Visibility = Visibility.Visible; break;
                case ShapeType.Rectangle: rectShape.Visibility = Visibility.Visible; break;
            }

            shapesHolder.Width = controlPanel.width;
            shapesHolder.Height = controlPanel.height;
        }
    }
}
