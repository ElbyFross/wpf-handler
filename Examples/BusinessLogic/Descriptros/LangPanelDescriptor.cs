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
using System.IO;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WpfHandler.UI;
using WpfHandler.UI.AutoLayout;
using WpfHandler.UI.AutoLayout.Options;
using WpfHandler.UI.AutoLayout.Markups;
using WpfHandler.UI.AutoLayout.Configuration;
using WpfHandler.UI.AutoLayout.Controls;
using WpfHandler.UI.Controls;

namespace BusinessLogic.Descriptros
{
    public class LangPanelDescriptor : UIDescriptor
    {
        /// <summary>
        /// Enum that will used as a source for toggles group.
        /// </summary>
        [Content]
        public LangOptions lang;

        [HideInInspector]
        private static readonly string langDictsPath =
           Directory.GetParent(Assembly.GetExecutingAssembly().Location).Parent.Parent.FullName + "\\langs\\";

        public enum LangOptions
        {
            En, Ru
        }

        public LangPanelDescriptor()
        {
            IsVirtualized = false;
        }

        public override void OnLoaded()
        {
            LocalizationHandler.LoadDictionaries(
                // Getting local lang folder from the source project.
                langDictsPath,
                // Request english localization as prior.
                new CultureInfo("en-US"),
                // Request russian localization as secondary in case if english not found.
                new CultureInfo("ru-RU"));


            // Subdcribing on lang value update.
            GetField("lang").ValueChanged += OnLanguageChanged;
        }

        /// <summary>
        /// Occurs when language changed.
        /// </summary>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        private void OnLanguageChanged(IGUIField arg1, object[] arg2)
        {
            switch(lang)
            {
                case LangOptions.En:
                    LocalizationHandler.LoadDictionaries(
                            langDictsPath,
                            new CultureInfo("en-US"));
                    break;

                case LangOptions.Ru:
                    LocalizationHandler.LoadDictionaries(
                            langDictsPath,
                            new CultureInfo("ru-RU"));
                    break;
            }
        }
    }
}
