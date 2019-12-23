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
using WpfHandler.UI.Controls;
using WpfHandler.UI.AutoLayout.Options;
using WpfHandler.UI.AutoLayout.Configuration;
using WpfHandler.UI.AutoLayout.Controls;

namespace TogglesGroupExample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Instance of the current active main window.
        /// </summary>
        public static MainWindow Active { get; protected set; }

        /// <summary>
        /// Brush that applied for the window on the lunch time.
        /// </summary>
        public Brush DefaultBrush { get; protected set; }

        public MainWindow()
        {
            // Set widows as active.
            Active = this;

            InitializeComponent();

            // Buferizing the current background as default.
            DefaultBrush = Background;

            // Instiniating the example descriptor.
            var descriptor = new ExampleDescripotor();

            // Applying the descriptor as the source of auto layout view from the xaml.
            alView.Descriptor = descriptor;
        }

        private void FlatTogglesGroup_ValueChanged(IGUIField obj)
        {
            var group = obj as FlatTogglesGroup;
            MessageBox.Show("\"" + group.Value + "\" is selected.");
        }
    }

    /// <summary>
    /// Setting the foreground color of all descriptor's elements.
    /// </summary>
    [Foreground("White")]
    public class ExampleDescripotor : UIDescriptor
    {
        #region Source types
        public enum SourceEnum
        {
            EnumOption1,
            EnumOption2,
            EnumOption3,
            EnumOption4,
        }

        public enum StateEnum
        {
            Off, On
        }
        #endregion

        #region Declating UI members
        /// <summary>
        /// The toggle group with a horizontal layout of the options.
        /// </summary>
        [Orientation(Orientation.Horizontal)]
        public StateEnum highlightState = StateEnum.Off;

        /// <summary>
        /// Default view toggle group.
        /// </summary>
        public SourceEnum codeBehindEnum = SourceEnum.EnumOption3;
        #endregion

        #region Handlers
        /// <summary>
        /// Occurs when auto layout handler finish the task and UI elements became ready to use.
        /// </summary>
        public override void OnLoaded()
        {
            // Getting the field binded to the 'highlightState' member.
            var field = GetFieldByMember("highlightState");

            // Subsribing on the value change event.
            field.ValueChanged += delegate (IGUIField objw)
            {
                StateEnum state = (StateEnum)objw.Value;

                switch (state)
                {
                    case StateEnum.On:
                        MainWindow.Active.Background = Brushes.DarkSlateBlue;
                        break;
                    case StateEnum.Off:
                        MainWindow.Active.Background = MainWindow.Active.DefaultBrush;
                        break;
                }
            };
        }
        #endregion
    }
}
