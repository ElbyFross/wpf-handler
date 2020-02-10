//Copyright 2019 Volodymyr Podshyvalov
//
//Licensed under the Apache License, Version 2.0 (the "License");
//you may not use this file except in compliance with the License.
//You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
//Unless required by applicable law or agreed to in writing, software
//distributed under the License is distributed on an "AS IS" BASIS,
//WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//See the License for the specific language governing permissions and
//limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WpfHandler.UI.AutoLayout;
using WpfHandler.UI.AutoLayout.Configuration;
using WpfHandler.UI.AutoLayout.Markups;


namespace WpfHandler.UI.AutoLayout.Controls
{
    /// <summary>
    /// Allow to add custom label element to the UI.
    /// </summary>
    [TypesCompatible(typeof(GUIContent))]
    public class LabelAttribute : GUIContentAttribute, IGUIField
    {
        /// <summary>
        /// Hanels label content. Allows only <see cref="GUIContent"/> values.
        /// </summary>
        public object Value
        {
            get => Content;
            set
            {
                if(value is GUIContent content)
                {
                    Content = content;
                    ValueChanged?.Invoke(this);
                }
            }
        }

        /// <summary>
        /// Member binded to the UI element during instiniation by using of <see cref="UIDescriptor"/> as source.
        /// </summary>
        public MemberInfo BindedMember { get; set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public LabelAttribute() : base () { }

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
        /// Occurs when contrent is changed.
        /// </summary>
        public event Action<IGUIField> ValueChanged;

        /// <summary>
        /// Spawn label element into the UI.
        /// </summary>
        /// <param name="layer"></param>
        /// <param name="args"></param>
        public virtual void OnLayout(ref LayoutLayer layer, params object[] args)
        {
            // Instiniate element.
            var element = new System.Windows.Controls.Label
            {
                // Set content.
                Content = Content.GetTitle()
            };

            // Attaching to layout.
            layer.ApplyControl(element);
        }

        /// <summary>
        /// Callback that occurs when content dictionaries are reloaded.
        /// Updating label's content.
        /// </summary>
        public override void LanguagesDictionariesUpdated()
        {
            throw new NotImplementedException();
        }
    }
}
