using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WpfHandler.UI.AutoLayout;


namespace WpfHandler.UI.AutoLayout.Controls
{
    /// <summary>
    /// Allow to add custom label element to the UI.
    /// </summary>
    public class LabelAttribute : GUIContentAttribute
    {
        /// <summary>
        /// Auto initialize content with shared title value.
        /// </summary>
        /// <param name="title">Title that will be showed up into the label.</param>
        public LabelAttribute(string title) : base(title) { }

        /// <summary>
        /// Constructor that allow to set title.
        /// </summary>
        /// <param name="title">Title of that element.</param>
        /// <param name="description">Description of that element.</param>
        public LabelAttribute(string title, string description) : base(title, description) { }

        /// <summary>
        /// Initialize all allowed fields.
        /// </summary>
        /// <param name="defaultTitle">Title that would be used by default if localization dictionary not found.</param>
        /// <param name="defaultDescription">Default description if localization dictionary not found.</param>
        /// <param name="decriptionLocalizationResourseKey">Key of description content in localized dynamic dictionary.</param>
        public LabelAttribute(
            string defaultTitle,
            string defaultDescription,
            string decriptionLocalizationResourseKey) :
            base(defaultTitle, defaultDescription, decriptionLocalizationResourseKey) { }

        /// <summary>
        /// Initialize all allowed fields.
        /// </summary>
        /// <param name="defaultTitle">Title that would be used by default if localization dictionary not found.</param>
        /// <param name="defaultDescription">Default description if localization dictionary not found.</param>
        /// <param name="titleLocalizationResourseKey">Key of title content in localized dynamic dictionary.</param>
        /// <param name="decriptionLocalizationResourseKey">Key of description content in localized dynamic dictionary.</param>
        public LabelAttribute(
            string defaultTitle,
            string defaultDescription,
            string titleLocalizationResourseKey,
            string decriptionLocalizationResourseKey) : 
            base(defaultTitle, defaultDescription, titleLocalizationResourseKey, decriptionLocalizationResourseKey) { }

        /// <summary>
        /// Spawn label element into the UI.
        /// </summary>
        /// <param name="layer"></param>
        /// <param name="args"></param>
        public virtual void OnGUI(ref LayoutLayer layer, params object[] args)
        {
            // Instiniate element.
            var element = new System.Windows.Controls.Label
            {
                // Set content.
                Content = Content.GetTitle()
            };

            // Attaching to layout.
            layer.root.AddChild(element);
        }

        /// <summary>
        /// TODO: Callback that occurs when content dictionaries are reloaded.
        /// Updating label's content.
        /// </summary>
        public override void LanguagesDictionariesUpdated()
        {
            throw new NotImplementedException();
        }
    }
}
