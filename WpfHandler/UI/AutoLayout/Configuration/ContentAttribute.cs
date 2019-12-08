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
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using WpfHandler.UI.Controls;

namespace WpfHandler.UI.AutoLayout.Configuration
{
    /// <summary>
    /// Defining content applied to the member.
    /// </summary>
    public class ContentAttribute : GUIContentAttribute
    {
        /// <summary>
        /// Return new attribute instance with None content.
        /// </summary>
        public static ContentAttribute Empty
        {
            get
            {
                return new ContentAttribute()
                {
                    Content = GUIContent.None
                };
            }
        }

        /// <summary>
        /// Instiniated control with a label.
        /// </summary>
        public ILabel BindedLabel { get; protected set; }

        /// <summary>
        /// Member info that will be used to auto generation of the field's label content.
        /// </summary>
        public MemberInfo BindedMember { get; protected set; }

        /// <summary>
        /// Instiniate new atribute with null GUI content.
        /// </summary>
        public ContentAttribute() : base() { }

        /// <summary>
        /// Auto initialize content with shared title value.
        /// </summary>
        /// <param name="title">Title that will be showed up into the label.</param>
        public ContentAttribute(string title) : base(title) { }

        /// <summary>
        /// Constructor that allow to set title.
        /// </summary>
        /// <param name="title">Title of that element.</param>
        /// <param name="description">Description of that element.</param>
        public ContentAttribute(string title, string description) : base(title, description) { }

        /// <summary>
        /// Initialize all allowed fields.
        /// </summary>
        /// <param name="defaultTitle">Title that would be used by default if localization dictionary not found.</param>
        /// <param name="defaultDescription">Default description if localization dictionary not found.</param>
        /// <param name="decriptionLocalizationResourseKey">Key of description content in localized dynamic dictionary.</param>
        public ContentAttribute(
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
        public ContentAttribute(
            string defaultTitle,
            string defaultDescription,
            string titleLocalizationResourseKey,
            string decriptionLocalizationResourseKey) :
            base(defaultTitle, defaultDescription, titleLocalizationResourseKey, decriptionLocalizationResourseKey) { }

        /// <summary>
        /// Callback that will occurs in case of updating of the language dictionaries.
        /// </summary>
        public override void LanguagesDictionariesUpdated()
        {
            // Updating label.
            if (Content != null)
            {
                // Updating lable's value.
                BindedLabel.Label = Content.GetTitle(BindedMember);
            }
            else
            {
                // hidding label.
                BindedLabel.LabelWidth = 0;
            }
        }

        /// <summary>
        /// Connecting instiniated control with label to localization updates.
        /// </summary>
        /// <param name="lable">UI control that has a lable to content bridging.</param>
        public void BindToLable(ILabel lable)
        {
            // Forwarding request.
            BindToLable(lable, null);
        }

        /// <summary>
        /// Connecting instiniated control with label to localization updates.
        /// </summary>
        /// <param name="lable">UI control that has a lable to content bridging.</param>
        /// <param name="sourceMember">
        /// Member infor that could be used as source for auto generated 
        /// title in case if GUIContent not provided in resources.</param>
        public void BindToLable(ILabel lable, MemberInfo sourceMember)
        {
            // Store shared references as binded.
            BindedLabel = lable;
            BindedMember = sourceMember;

            // Throw exception if control not shared.
            if (BindedLabel == null) 
                throw new NotSupportedException( "Require `" + 
                    typeof(ILabel).FullName + "` UI control shared via args[].");

            // Udate content.
            LanguagesDictionariesUpdated();
        }
    }
}
