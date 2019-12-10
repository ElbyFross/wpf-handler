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
using System.Linq;
using System.Windows;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WpfHandler.UI.Controls;
using WpfHandler.UI.AutoLayout.Configuration;

namespace WpfHandler.UI
{
    /// <summary>
    /// Provides the API to managing the localization system.
    /// </summary>
    public static class LocalizationHandler
    {
        /// <summary>
        /// Event that occurs when languages dictionary will be loaded.
        /// </summary>
        public static event Action LanguagesDictionariesUpdated;

        /// <summary>
        /// Connecting instiniated control with label to localization updates.
        /// </summary>
        /// <param name="lable">UI control that has a lable to content bridging.</param>
        public static void BindToLable(this GUIContent content, ILabel lable)
        {
            // Forwarding request.
            content.BindToLable(lable, null);
        }

        /// <summary>
        /// Connecting instiniated control with label to localization updates.
        /// </summary>
        /// <param name="lable">UI control that has a lable to content bridging.</param>
        /// <param name="sourceMember">
        /// Member infor that could be used as source for auto generated 
        /// title in case if GUIContent not provided in resources.</param>
        public static void BindToLable(this GUIContent content, ILabel lable, MemberInfo sourceMember)
        {
            // Innstiniating localization attribute that will manage the lable.
            var localizationAttribute = new ContentAttribute
            {
                // Applying shared content.
                Content = content
            };

            // Binding content to the lable.
            localizationAttribute.BindToLable(lable, sourceMember);
        }

        /// <summary>
        /// Scaning for language dictionaries in XAML files stored by <see cref="Plugins.Constants.PLUGINS_DIR"/>,
        /// and loading them to Merged dictionaries.
        /// Loading only relative to the new culture if found. Leave a previous culture if not.
        /// 
        /// Require files format: *.lang.CULTURE_CODE.xaml, where culture code equal current translation of the app. 
        /// Example: plugin.feed.lang.en-US.xaml
        /// </summary>
        /// <param name="targetCulture">Culture that will be serched with hightest priority.</param>
        /// <param name="secondaryCulture">Culture that will be prefured in case of target not implemented. If also not implemented than will be used first entry.</param>
        public static void LoadDictionaries(CultureInfo targetCulture, CultureInfo secondaryCulture)
        {
            #region Validate and fix base conditions
            // Validate directory.
            if (!Directory.Exists(Plugins.Constants.PLUGINS_DIR))
            {
                Directory.CreateDirectory(Plugins.Constants.PLUGINS_DIR);
                Console.WriteLine("PLUGINS DIRECTORY NOT FOUND. NEW ONE WAS CREATED.");
            }
            #endregion

            // Request dictionaries update.
            Dictionaries.API.UpdateDictionariesGroup(Plugins.Constants.PLUGINS_DIR, "lang", targetCulture.Name, secondaryCulture.Name);

            // Update culture.
            System.Threading.Thread.CurrentThread.CurrentUICulture = targetCulture;

            // Inform subscribers.
            LanguagesDictionariesUpdated?.Invoke();
        }
    }
}
