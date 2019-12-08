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
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfHandler.UI.AutoLayout;

namespace WpfHandler.UI.Controls
{
    /// <summary>
    /// Interaction logic for CatalogButton.xaml
    /// </summary>
    public partial class CatalogButton : UserControl, ILabel
    {
        #region Dependency properties
        /// <summary>
        /// Bridging XAML declaring and the member.
        /// </summary>
        public static readonly DependencyProperty LabelProperty = DependencyProperty.Register(
          "Label", typeof(string), typeof(CatalogButton));

        //public static readonly DependencyProperty HierarchyLevelProperty = DependencyProperty.Register(
        //  "HierarchyLevel", typeof(int), typeof(CatalogButton));

        /// <summary>
        /// Bridging XAML declaring and the member.
        /// </summary>
        public static readonly DependencyProperty UnfocusedBackgroundColorProperty = DependencyProperty.Register(
          "UnfocusedBackgroundColor", typeof(Brush), typeof(CatalogButton),
          new PropertyMetadata(Brushes.Transparent));

        /// <summary>
        /// Bridging XAML declaring and the member.
        /// </summary>
        public static readonly DependencyProperty FocusedBackgroundColorProperty = DependencyProperty.Register(
          "FocusedBackgroundColor", typeof(Brush), typeof(CatalogButton),
           new PropertyMetadata(new SolidColorBrush((Color)ColorConverter.ConvertFromString("#00A8E8"))));

        /// <summary>
        /// Bridging XAML declaring and the member.
        /// </summary>
        public static readonly DependencyProperty TextColorProperty = DependencyProperty.Register(
          "TextColor", typeof(Brush), typeof(CatalogButton),
          new PropertyMetadata(Brushes.White));

        /// <summary>
        /// Bridging XAML declaring and the member.
        /// </summary>
        public static readonly DependencyProperty ClickCallbackProperty = DependencyProperty.Register(
          "ClickCallback", typeof(System.Action<object>), typeof(CatalogButton));
        #endregion

        #region Properties
        /// <summary>
        /// Text that will be displayed on the button.
        /// </summary>
        public string Label
        {
            get { return (string)this.GetValue(LabelProperty); }
            set { this.SetValue(LabelProperty, value); }
        }

        /// <summary>
        /// Define offset of the button text. 
        /// </summary>
        public int HierarchyLevel { get; set; } = 1;                

        /// <summary>
        /// Compute margine relative to hierarchy level.
        /// </summary>
        public Thickness AutoMargin
        {
            get
            {
                return new Thickness(HierarchyLevel * 20, Margin.Top - 5, Margin.Right, Margin.Bottom - 5);
            }
        }

        /// <summary>
        /// Collor of button when it unfocused.
        /// </summary>
        public Brush UnfocusedBackgroundColor { get; set; }

        /// <summary>
        /// Color of button when it focused.
        /// </summary>
        public Brush FocusedBackgroundColor { get; set; }
        
        /// <summary>
        /// Collor of the text.
        /// </summary>
        public Brush TextColor { get; set; }

        /// <summary>
        /// Method that will has been calling during click on button.
        /// </summary>
        public Action<object> ClickCallback { get; set; }

        /// <summary>
        /// Not supported.
        /// </summary>
        public float LabelWidth { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
        #endregion

        /// <summary>
        /// Initialisign the catalog button.
        /// </summary>
        /// <remarks>
        /// Trying to find "MenuButton" resource as <see cref="Style"/>. 
        /// Applying in case if found.
        /// </remarks>
        public CatalogButton()
        {
            InitializeComponent();
            DataContext = this;

            // Try to load default style
            try
            {
                if (Application.Current.FindResource("MenuButton") is Style style)
                {
                    this.Style = style;
                }
            }
            catch
            { 
                // Not found in dictionary. Not important.
            }
        }

        /// <summary>
        /// Callback that will has been calling when button will be clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CatalogButton_Click(object sender, RoutedEventArgs e)
        {
            // Call target handler if avalaiable.
            ClickCallback?.Invoke(sender);
        }
    }
}
