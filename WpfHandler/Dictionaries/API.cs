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

namespace WpfHandler.Dictionaries
{
    /// <summary>
    /// Class that provide methods for controll WPF application localization.
    /// </summary>
    public static class API
    {
        /// <summary>
        /// Event that will occure when languages dictionary will be loaded.
        /// </summary>
        public static event Action LanguagesDictionariesUpdated;

        /// <summary>
        /// Scaning for language dictionaries in XAML files, and load them to Merged dictionaries.
        /// Loading new theme by code if found. Leave already loaded if overrided dictionary not found.
        /// 
        /// Require files format: *.theme.THEME_CODE.xaml, where theme code equal current theme selected on the app. 
        /// Example: plugin.feed.theme.blueTheme.xaml
        /// </summary>
        /// <param name="themeCode"></param>
        public static void LoadXAML_Thems(string themeCode)
        {
            #region Validate and fix base conditions
            // Validate directory.
            if (!Directory.Exists(Plugins.Constants.THEMES_DIR))
            {
                Directory.CreateDirectory(Plugins.Constants.THEMES_DIR);
                Console.WriteLine("THEMES DIRECTORY NOT FOUND. NEW ONE WAS CREATED.");
            }
            #endregion
            
            UpdateDictionariesGroup(Plugins.Constants.THEMES_DIR, "theme", themeCode);
        }

        /// <summary>
        /// Scaning for language dictionaries in XAML files, and load them to Merged dictionaries.
        /// Load relative to new culture if found. Leave previous culture if not.
        /// 
        /// Require files format: *.lang.CULTURE_CODE.xaml, where culture code equal current translation of the app. 
        /// Example: plugin.feed.lang.en-US.xaml
        /// </summary>
        /// <param name="targetCulture">Culture that will be serched with hightest priority.</param>
        /// <param name="secondaryCulture">Culture that will be prefured in case of target not implemented. If also not implemented than will be used first entry.</param>
        public static void LoadXAML_LangDicts(CultureInfo targetCulture, CultureInfo secondaryCulture)
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
            UpdateDictionariesGroup(Plugins.Constants.PLUGINS_DIR, "lang", targetCulture.Name, secondaryCulture.Name);
            
            // Update culture.
            System.Threading.Thread.CurrentThread.CurrentUICulture = targetCulture;

            // Inform subscribers.
            LanguagesDictionariesUpdated?.Invoke();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="groupCode"></param>
        /// <param name="subCodes">Codes that will be required from group of plugins.
        /// Will be looced in order of prefernces. Lower index - more prefered.
        /// Will load any exist in case if any requested code not found.
        /// 
        /// Exmples: 
        /// Culture code for localization plugins. 
        /// Theme code for design plugins.
        /// </param>
        public static void UpdateDictionariesGroup(string directory, string groupCode, params string[] subCodes)
        {

            #region Find localization files
            // Load all lang files.
            Regex searchPattern = new Regex(@"\w*."+ groupCode + ".[0-9a-z-]*.xaml", RegexOptions.IgnoreCase);
            var xamlDicts = Directory.EnumerateFiles(directory, "*.xaml", SearchOption.AllDirectories)
                .Where(s => searchPattern.IsMatch(s));


            // Detect plugins domains and select more relevant.
            Hashtable pluginDomainsMap = new Hashtable();

            string rootName = null; // Varaiable that avoid allocating of memmory on every loop's step.
            string subPluginCodeBufer = null; // Variable that contain sub code of that dictionary (culture code, theme code, etc.).
            int groupCodeIndex = 0; // Bufer that avoid allocating for every loop's step.

            // Register every found dictionary.
            foreach (string domain in xamlDicts)
            {
                // Get plugin domain.
                rootName = domain.Substring(domain.LastIndexOf('\\') + 1);
                groupCodeIndex = rootName.LastIndexOf("."+groupCode+ ".");

                // Detect file culture.
                subPluginCodeBufer = rootName.Substring(groupCodeIndex + 6);
                subPluginCodeBufer = subPluginCodeBufer.Substring(0, subPluginCodeBufer.IndexOf('.'));

                rootName = rootName.Substring(0, groupCodeIndex);

                // Load map list for this domain.
                if (!(pluginDomainsMap[rootName] is List<DomainContainer> domainMap))
                {
                    // Create new if not found.
                    domainMap = new List<DomainContainer>();
                    pluginDomainsMap.Add(rootName, domainMap);
                }
                // Add data to list.
                domainMap.Add(new DomainContainer() 
                { key = subPluginCodeBufer, pluginDomain = rootName, path = domain });
            }

            // Select most relevant domains.
            List<DomainContainer> relevantDomains = new List<DomainContainer>();
            foreach (string domain in pluginDomainsMap.Keys)
            {
                bool detected = false;
                // Load map list for this domain.
                if (pluginDomainsMap[domain] is List<DomainContainer> domainMap)
                {
                    DomainContainer reservContainer = null;
                    foreach (DomainContainer dc in domainMap)
                    {
                        // If target found.
                        if (dc.key == subCodes[0])
                        {
                            detected = true;
                            relevantDomains.Add(dc);
                            break;
                        }

                        // If found secondary code contaier then save it.
                        for (int i = 1; i < subCodes.Length; i++)
                        {
                            var request = subCodes[i];

                            // Skip if previlaged one already found.
                            if (reservContainer != null && reservContainer.key == request) break;

                            // Set as most relevant reserve container.
                            if (request == dc.key)
                            {
                                reservContainer = dc;
                                break;
                            }
                        }
                    }

                    // Start next domain if found.
                    if (detected) continue;

                    // Apply reserv contaier if found.
                    if (reservContainer != null)
                    {
                        relevantDomains.Add(reservContainer);
                        continue;
                    }

                    // Apply first detected plugin dictionary domain if any specified not detected.
                    if (domainMap.Count > 0)
                    {
                        relevantDomains.Add(domainMap[0]);
                    }
                }
            }
            #endregion

            #region Change localization.
            // Bufer for dict loading. 
            foreach (DomainContainer domain in relevantDomains)
            {
                // Dict pattern.
                Regex regex = new Regex(@"\w*" + domain.pluginDomain + "." + groupCode + ".[0-9a-z-]*.xaml", RegexOptions.IgnoreCase);

                // Look for conflict dictionary among loaded.
                ResourceDictionary rdForRemove = null;
                foreach (ResourceDictionary conflict_rd in Application.Current.Resources.MergedDictionaries)
                {
                    // Check os the file if match to patern.
                    if (regex.IsMatch(conflict_rd.Source.OriginalString))
                    {
                        // Set as target for remove.
                        rdForRemove = conflict_rd;
                        break;
                    }
                }

                // Load new as source.
                string formatedPath = domain.path.Replace("\\", "/");
                ResourceDictionary myResourceDictionary = new ResourceDictionary
                {
                    Source = new Uri("pack://siteoforigin:,,,/" + formatedPath, UriKind.Absolute)
                };

                // Remove conflict dictionary if found and insert new.
                if (rdForRemove != null)
                {
                    int ind = Application.Current.Resources.MergedDictionaries.IndexOf(rdForRemove);
                    Application.Current.Resources.MergedDictionaries.Remove(rdForRemove);
                    Application.Current.Resources.MergedDictionaries.Insert(ind, myResourceDictionary);
                }
                // Add as new if conflicts not found.
                else
                {
                    Application.Current.Resources.MergedDictionaries.Add(myResourceDictionary);
                    //Collection<ResourceDictionary> col = Application.Current.Resources.MergedDictionaries;
                }
            }
            #endregion
        }
    }
}
