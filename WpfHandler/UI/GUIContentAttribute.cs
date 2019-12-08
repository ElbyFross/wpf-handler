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
using WpfHandler.Dictionaries;

namespace WpfHandler.UI
{
    /// <summary>
    /// Define members for attributes focused on managing GUIContent.
    /// </summary>
    public abstract class GUIContentAttribute : LocalizableContentAttribute
    {
        /// <summary>
        /// Content applied to that GUI element.
        /// </summary>
        public GUIContent Content { get; set; }

        /// <summary>
        /// Instiniate new atribute with null GUI content.
        /// </summary>
        public GUIContentAttribute() : base()
        {
            Content = null;
        }

        /// <summary>
        /// Auto initialize content with shared title value.
        /// </summary>
        /// <param name="title">Title that will be showed up into the label.</param>
        /// <remarks>If the title value is null or empty then instiniating <see cref="GUIContent.None"/>.</remarks>
        public GUIContentAttribute(string title) : base()
        {
            if (string.IsNullOrEmpty(title))
            {
                Content = GUIContent.None;
            }
            else
            {
                Content = new GUIContent(title);
            }
        }

        /// <summary>
        /// Constructor that allow to set title.
        /// </summary>
        /// <param name="title">Title of that element.</param>
        /// <param name="description">Description of that element.</param>
        public GUIContentAttribute(string title, string description) : base()
        {
            Content = new GUIContent
            {
                DefaultTitle = title,
                DefaultDescription = description
            };
        }

        /// <summary>
        /// Initialize all allowed fields.
        /// </summary>
        /// <param name="defaultTitle">Title that would be used by default if localization dictionary not found.</param>
        /// <param name="defaultDescription">Default description if localization dictionary not found.</param>
        /// <param name="decriptionLocalizationResourseKey">Key of description content in localized dynamic dictionary.</param>
        public GUIContentAttribute(
            string defaultTitle,
            string defaultDescription,
            string decriptionLocalizationResourseKey) : base()
        {
            Content = new GUIContent
            {
                DefaultTitle = defaultTitle,
                DefaultDescription = defaultDescription,
                DecriptionLocalizationResourseKey = decriptionLocalizationResourseKey
            };
        }

        /// <summary>
        /// Initialize all allowed fields.
        /// </summary>
        /// <param name="defaultTitle">Title that would be used by default if localization dictionary not found.</param>
        /// <param name="defaultDescription">Default description if localization dictionary not found.</param>
        /// <param name="titleLocalizationResourseKey">Key of title content in localized dynamic dictionary.</param>
        /// <param name="decriptionLocalizationResourseKey">Key of description content in localized dynamic dictionary.</param>
        public GUIContentAttribute(
            string defaultTitle,
            string defaultDescription,
            string titleLocalizationResourseKey,
            string decriptionLocalizationResourseKey) : base()
        {
            Content = new GUIContent
            {
                DefaultTitle = defaultTitle,
                DefaultDescription = defaultDescription,
                TitleLocalizationResourseKey = titleLocalizationResourseKey,
                DecriptionLocalizationResourseKey = decriptionLocalizationResourseKey
            };
        }

    }
}
