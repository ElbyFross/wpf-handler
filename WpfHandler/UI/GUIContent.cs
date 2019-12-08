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
using System.Reflection;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfHandler.UI
{
    /// <summary>
    /// Define content of GUI lement.
    /// </summary>
    [Serializable]
    public class GUIContent : FrameworkElement
    {
        /// <summary>
        /// Event that will occure when content properties will updated.
        /// </summary>
        public event Action<GUIContent> ContentUpdated;

        /// <summary>
        /// Empty content.
        /// Can't change none state after instiniation.
        /// </summary>
        public static GUIContent None
        {
            get
            {
                // Create new instance if not created yet.
                return new GUIContent()
                {
                    _isNone = true // Mark as none.
                };
            }
        }

        #region Public members
        /// <summary>
        /// Title that would be used by default in case if dynamic dictionry not found.
        /// in case if null then would be automaticly generated from the name of the binded member.
        /// </summary>
        public string DefaultTitle
        {
            get { return _DefaultTitle; }
            set
            {
                _DefaultTitle = value;
                ContentUpdated?.Invoke(this);
            }
        }

        /// <summary>
        /// Description that would be used by default in case if dynamic dictionry not found.
        /// </summary>
        public string DefaultDescription
        {
            get { return _DefaultDescription; }
            set
            {
                _DefaultDescription = value;
                ContentUpdated?.Invoke(this);
            }
        }

        /// <summary>
        /// Key of resource from dynamic dictionary that would be used for loading of localized title.
        /// </summary>
        public string TitleLocalizationResourseKey
        {
            get { return _TitleLocalizationResourseKey; }
            set
            {
                _TitleLocalizationResourseKey = value;
                ContentUpdated?.Invoke(this);
            }
        }

        /// <summary>
        /// Key of resource from dynamic dictionary that would be used for loading of localized description.
        /// </summary>
        public string DecriptionLocalizationResourseKey
        {
            get { return _DecriptionLocalizationResourseKey; }
            set
            {
                _DecriptionLocalizationResourseKey = value;
                ContentUpdated?.Invoke(this);
            }
        }

        /// <summary>
        /// Is that conent is defult none.
        /// </summary>
        public bool IsNone
        {
            get { return _isNone ; }
        }
        #endregion

        #region Private members
        /// <summary>
        /// Bufer that contains last defined title.
        /// </summary>
        private string _Title;

        /// <summary>
        /// Bufer that contains last defined description.
        /// </summary>
        private string _Description;

        /// <summary>
        /// is that content is none.
        /// </summary>
        private bool _isNone = false;

        /// <summary>
        /// Title that would be used by default in case if dynamic dictionry not found.
        /// in case if null then would be automaticly generated from the name of the binded member.
        /// </summary>
        private string _DefaultTitle = null;

        /// <summary>
        /// Description that would be used by default in case if dynamic dictionry not found.
        /// </summary>
        private string _DefaultDescription = null;

        /// <summary>
        /// Key of resource from dynamic dictionary that would be used for loading of localized title.
        /// </summary>
        private string _TitleLocalizationResourseKey = null;

        /// <summary>
        /// Key of resource from dynamic dictionary that would be used for loading of localized description.
        /// </summary>
        private string _DecriptionLocalizationResourseKey = null;
        #endregion

        #region Constructors & destructor
        /// <summary>
        /// Default constructor.
        /// </summary>
        public GUIContent() 
        {
            // Subscribe on events.
            Dictionaries.API.LanguagesDictionariesUpdated += API_LanguagesDictionariesUpdated;
        }

        /// <summary>
        /// Constructor that allow to set title.
        /// </summary>
        /// <param name="title">Title of the element.</param>
        public GUIContent(string title) : base()
        {
            DefaultTitle = title;

            // Subscribe on events.
            Dictionaries.API.LanguagesDictionariesUpdated += API_LanguagesDictionariesUpdated;
        }

        /// <summary>
        /// Constructor that allow to set title.
        /// </summary>
        /// <param name="title">Title of that element.</param>
        /// <param name="description">Description of that element.</param>
        public GUIContent(string title, string description) : base()
        {
            DefaultTitle = title;
            DefaultDescription = description;

            // Subscribe on events.
            Dictionaries.API.LanguagesDictionariesUpdated += API_LanguagesDictionariesUpdated;
        }

        /// <summary>
        /// Initialize all allowed fields.
        /// </summary>
        /// <param name="defaultTitle">Title that would be used by default if localization dictionary not found.</param>
        /// <param name="defaultDescription">Default description if localization dictionary not found.</param>
        /// <param name="decriptionLocalizationResourseKey">Key of description content in localized dynamic dictionary.</param>
        public GUIContent(
            string defaultTitle,
            string defaultDescription,
            string decriptionLocalizationResourseKey) : base()
        {
            this.DefaultTitle = defaultTitle;
            this.DefaultDescription = defaultDescription;
            this.DecriptionLocalizationResourseKey = decriptionLocalizationResourseKey;

            // Subscribe on events.
            Dictionaries.API.LanguagesDictionariesUpdated += API_LanguagesDictionariesUpdated;
        }

        /// <summary>
        /// Initialize all allowed fields.
        /// </summary>
        /// <param name="defaultTitle">Title that would be used by default if localization dictionary not found.</param>
        /// <param name="defaultDescription">Default description if localization dictionary not found.</param>
        /// <param name="titleLocalizationResourseKey">Key of title content in localized dynamic dictionary.</param>
        /// <param name="decriptionLocalizationResourseKey">Key of description content in localized dynamic dictionary.</param>
        public GUIContent(
            string defaultTitle, 
            string defaultDescription, 
            string titleLocalizationResourseKey,
            string decriptionLocalizationResourseKey) : base()
        {
            this.DefaultTitle = defaultTitle;
            this.DefaultDescription = defaultDescription;
            this.TitleLocalizationResourseKey = titleLocalizationResourseKey;
            this.DecriptionLocalizationResourseKey = decriptionLocalizationResourseKey;

            // Subscribe on events.
            Dictionaries.API.LanguagesDictionariesUpdated += API_LanguagesDictionariesUpdated;
        }

        /// <summary>
        /// Resliasing unmanged memory.
        /// </summary>
        ~GUIContent()
        {
            // Unsubscribe from events.
            Dictionaries.API.LanguagesDictionariesUpdated -= API_LanguagesDictionariesUpdated;
        }
        #endregion

        #region API
        /// <summary>
        /// Define relevant title for certain member.
        /// </summary>
        /// <returns>Relevant title based on internal data of content.</returns>
        public string GetTitle()
        {
            return GetTitle(null);
        }

        /// <summary>
        /// Define relevant title for certain member.
        /// </summary>
        /// <param name="member">Binded member that would be used as source of auto generated member title.</param>
        /// <returns>Relevant title of the member.</returns>
        public string GetTitle(MemberInfo member)
        {
            if (_Title == null)
            {
                try
                {
                    // load title from dictionary.
                    _Title = FindResource(TitleLocalizationResourseKey) as string;
                }
                catch
                {
                    if (member != null)
                    {
                        // Set default title or dict code if title not found.
                        _Title = DefaultTitle ?? Format(member.Name);
                    }
                    else
                    {
                        _Title = DefaultTitle;
                    }
                }
            }

            return _Title;
        }

        /// <summary>
        /// Define relevant description for certain member.
        /// </summary>
        /// <returns>Relevant description of the member.</returns>
        public string GetDescription()
        {
            if (_Description == null)
            {
                try
                {
                    // load title from dictionary.
                    _Description = FindResource(TitleLocalizationResourseKey) as string;
                }
                catch
                {
                    // Set default description or dict code if title not found.
                    _Description = DefaultDescription;
                }
            }

            return _Description;
        }

        /// <summary>
        /// Format member name to user reandly veiw.
        /// </summary>
        /// <param name="memberName">Name of the member.</param>
        /// <returns>Formated name.</returns>
        public string Format(string memberName)
        {
            // Set first symbol to upper case.
            var firstSymbol = memberName[0];
            if (!Char.IsUpper(firstSymbol))
            {
                var upperEntrySymbol = Char.ToUpper(firstSymbol); // Get up case symbool.
                memberName = upperEntrySymbol + memberName.Substring(1, memberName.Length - 1); // Replace symbol.
            }

            // Bufers.
            string result = "";
            var length = memberName.Length; 

            // Copy name with formating.
            for (int i = 0; i < memberName.Length; i++)
            {
                var c = memberName[i];

                // Add space before new word.
                if (Char.IsUpper(c) && i < length - 1)
                {
                    // If next symbol has no up case.
                    if (!Char.IsUpper(memberName[i + 1]) &&
                        i > 0 )
                    {
                        result += " ";
                    }
                }

                // Copping character.
                result += c;
            }

            return result;
        }

        /// <summary>
        /// Clearing current data. To allow update.
        /// </summary>
        public void Clear()
        {
            _Description = null;
            _Title = null;
        }
        #endregion

        #region Callbacks
        /// <summary>
        /// WIll occure when new pool of language dictionaries will loaded.
        /// Drop current value to allow reloading.
        /// </summary>
        private void API_LanguagesDictionariesUpdated()
        {
            // Drop data.
            Clear();
        }
        #endregion
    }
}
