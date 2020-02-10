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
using System.Windows;
using WpfHandler.UI.AutoLayout;
using WpfHandler.UI.Controls;

namespace WpfHandler.UI.AutoLayout.Options
{
    /// <summary>
    /// Defines a text font size for the control.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property |
                    AttributeTargets.Class | AttributeTargets.Struct,
                    AllowMultiple = false, Inherited = true)]
    public class FontWeightAttribute : Attribute, IGUILayoutOption
    {
        /// <summary>
        /// Defines the font weight.
        /// </summary>
        public enum WeightType
        {
            /// <summary>
            ///  Specifies an "Thin" font weight.
            /// </summary>
            Thin,
            /// <summary>
            /// Specifies an "Extra-light" font weight.
            /// </summary>
            ExtraLight,
            /// <summary>
            /// Specifies an "Ultra-light" font weight.
            /// </summary>
            UltraLight,
            /// <summary>
            /// Specifies a "Light" font weight.
            /// </summary>
            Light,
            /// <summary>
            /// Specifies a "Normal" font weight.
            /// </summary>
            Normal,
            /// <summary>
            /// Specifies a "Regular" font weight.
            /// </summary>
            Regular,
            /// <summary>
            /// Specifies a "Medium" font weight.
            /// </summary>
            Medium,
            /// <summary>
            /// Specifies a "Demi-bold" font weight.
            /// </summary>
            DemiBold,
            /// <summary>
            /// Specifies a "Semi-bold" font weight.
            /// </summary>
            SemiBold,
            /// <summary>
            ///  Specifies a "Bold" font weight.
            /// </summary>
            Bold,
            /// <summary>
            /// Specifies an "Extra-bold" font weight.
            /// </summary>
            ExtraBold,
            /// <summary>
            /// Specifies an "Ultra-bold" font weight.
            /// </summary>
            UltraBold,
            /// <summary>
            /// Specifies a "Black" font weight.
            /// </summary>
            Black,
            /// <summary>
            /// Specifies a "Heavy" font weight.
            /// </summary>
            Heavy,
            /// <summary>
            /// Specifies an "Extra-black" font weight.
            /// </summary>
            ExtraBlack,
            /// <summary>
            ///Specifies an "Ultra-black" font weight.
            /// </summary>
            UltraBlack
        }

        /// <summary>
        /// Weight of the font.
        /// </summary>
        public FontWeight Weight { get; protected set; }

        /// <summary>
        /// Defines the target font waght.
        /// </summary>
        /// <param name="weight"></param>
        public FontWeightAttribute(WeightType weight)
        {
            switch (weight)
            {
                case WeightType.Thin: Weight = FontWeights.Thin; return;
                case WeightType.ExtraLight: Weight = FontWeights.ExtraLight; return;
                case WeightType.UltraLight: Weight = FontWeights.UltraLight; return;
                case WeightType.Light: Weight = FontWeights.Light; return;
                case WeightType.Normal: Weight = FontWeights.Normal; return;
                case WeightType.Regular: Weight = FontWeights.Regular; return;
                case WeightType.Medium: Weight = FontWeights.Medium; return;
                case WeightType.DemiBold: Weight = FontWeights.DemiBold; return;
                case WeightType.SemiBold: Weight = FontWeights.SemiBold; return;
                case WeightType.Bold: Weight = FontWeights.Bold; return;
                case WeightType.ExtraBold: Weight = FontWeights.ExtraBold; return;
                case WeightType.UltraBold: Weight = FontWeights.UltraBold; return;
                case WeightType.Black: Weight = FontWeights.Black; return;
                case WeightType.Heavy: Weight = FontWeights.Heavy; return;
                case WeightType.ExtraBlack: Weight = FontWeights.ExtraBlack; return;
                case WeightType.UltraBlack: Weight = FontWeights.UltraBlack; return;
            }
        }

        /// <summary>
        /// Define GUI element's font weight.
        /// </summary>
        /// <param name="element">
        /// Shared UI element. Must be inheirted from 
        /// `System.Windows.Controls.Control` to affect the font properties.
        /// </param>
        public void ApplyLayoutOption(FrameworkElement element)
        {
            // Try to cast into control.
            if(element is System.Windows.Controls.Control control)
            {
                // Apply size if casted.
                control.FontWeight = Weight;
            }
        }
    }
}
