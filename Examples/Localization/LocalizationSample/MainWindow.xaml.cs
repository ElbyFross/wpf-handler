using System.Windows;
using WpfHandler.UI;

namespace LocalizationSample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
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
                new System.Globalization.CultureInfo("en-US"), 
                // Request russian localization as secondary in case if english not found.
                new System.Globalization.CultureInfo("ru-RU")); 
            #endregion
        }
    }
}
