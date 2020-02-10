using System;
using System.Windows;
using WpfHandler.UI.AutoLayout.Controls;
using BusinessLogic.Descriptros;

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

            // Appllying descriptor instances to the AutoLayoutVeiw.
            tableView.Descriptor = tableDescriptor;
            newElementView.Descriptor = newElementDescriptor;

            // Defining current active tab.
            switchPanel.Current = tableView;

            tableDescriptor.controlPanel.NewItemTab += async delegate ()
            {
                // Switches to the `New item` tab.
                await switchPanel.SwitchToAsync(
                    newElementView,
                    WpfHandler.UI.Controls.SwitchPanel.AnimationType.None);
            };

            // Defining behavior for `ToTableTab` action at the `New item` form.
            newElementDescriptor.ToTableTab += async delegate ()
            {
                // Switches back to the `Table` tab.
                await switchPanel.SwitchToAsync(
                    tableView,
                    WpfHandler.UI.Controls.SwitchPanel.AnimationType.None);
            };
        }
    }
}
