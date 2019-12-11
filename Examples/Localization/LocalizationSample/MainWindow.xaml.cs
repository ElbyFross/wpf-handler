using System.Windows;
using System.Globalization;
using WpfHandler.UI;
using WpfHandler.UI.AutoLayout;
using WpfHandler.UI.Controls;

namespace LocalizationSample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public int LangIndex { get; protected set; } = 0;

        public MainWindow()
        {
            InitializeComponent();

            #region Binding button to the localiztion system
            // Creating GUI content.
            var content = new GUIContent()
            {
                // Adding the title that will displayed in case if localization key not found via loaded.
                DefaultTitle = "Native title", 

                // Adding the key that will be looking into the loaded dictionaries.
                TitleLocalizationResourseKey = "localizedLableCustomKey"
            };

            // Binding the content to the button's lable.
            content.BindToLable(localizedButton);
            #endregion

            #region Loading localization dictionaries
            LocalizationHandler.LoadDictionaries(
                // Request english localization as prior.
                new CultureInfo("en-US"), 
                // Request russian localization as secondary in case if english not found.
                new CultureInfo("ru-RU"));
            #endregion

            // Subscribing on the event of the language change.
            langPanel.ValueChanged += LangPanel_ValueChanged;
        }

        /// <summary>
        /// Occurs when user decide to change the current app's language.
        /// </summary>
        /// <param name="obj"></param>
        private void LangPanel_ValueChanged(IGUIField obj)
        {
            var togglePanel = obj as FlatTogglesGroup;

            switch (togglePanel.Index)
            {
                case 0: 
                    LocalizationHandler.LoadDictionaries(new CultureInfo("en-US"));
                    break;

                case 1:
                    LocalizationHandler.LoadDictionaries(new CultureInfo("ru-RU"));
                    break;

                default:
                    LocalizationHandler.UnloadDictionaries();
                    break;
            }

            // Buferizing current index.
            LangIndex = togglePanel.Index;
        }

        /// <summary>
        /// Occurs when button is pressed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LocalizedButton_Click(object sender, RoutedEventArgs e)
        {
            string message = "Localization not selected. Used default content.";

            switch (LangIndex)
            {
                case 0:
                    message = "English localization is selected.";
                    break;

                case 1:
                    message = "Russian localization is selected.";
                    break;
            }


            MessageBox.Show(message);
        }
    }
}
