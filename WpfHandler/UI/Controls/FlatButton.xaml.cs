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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfHandler.UI.AutoLayout;

namespace WpfHandler.UI.Controls
{
    /// <summary>
    /// Interaction logic for FlatButton.xaml
    /// </summary>
    public partial class FlatButton : UserControl, ILabel
    {
        /// <summary>
        /// Bridging XAML declaring and the member.
        /// </summary>
        public static readonly DependencyProperty LabelProperty = DependencyProperty.Register(
          "Label", typeof(string), typeof(FlatButton), new PropertyMetadata("Sample"));

        /// <summary>
        /// Bridging XAML declaring and the member.
        /// </summary>
        public static readonly DependencyProperty ClickCallbackProperty = DependencyProperty.Register(
          "ClickCallback", typeof(Action<object>), typeof(FlatButton));

        /// <summary>
        /// Text that will be displayed on the button.
        /// </summary>
        public string Label
        {
            get { return (string)this.GetValue(LabelProperty); }
            set { this.SetValue(LabelProperty, value); }
        }

        /// <summary>
        /// Offset applied to label.
        /// </summary>
        public Thickness LabelMargin
        {
            get
            {
                var offset = -main.Height / 2;
                return new Thickness(0, offset, 0, offset);
            }
        }

        /// <summary>
        /// Not supported.
        /// </summary>
        public float LabelWidth { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }

        /// <summary>
        /// Method that will has been calling during click on button.
        /// </summary>
        public Action<object> ClickCallback
        {
            get { return (Action<object>)this.GetValue(ClickCallbackProperty); }
            set { this.SetValue(ClickCallbackProperty, value); }
        }

        /// <summary>
        /// Default constuctor.
        /// </summary>
        public FlatButton()
        {
            InitializeComponent();
            DataContext = this;

            // Try to load default style
            try
            {
                if (Application.Current.FindResource("FlatButton") is Style style)
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
            ClickCallback?.Invoke(this);
        }
    }
}
