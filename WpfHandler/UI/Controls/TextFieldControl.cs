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
using System.Windows;
using System.Windows.Controls;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
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
    /// Defines base members for all text field for providing uniform way to operate with GUI elemetns.
    /// </summary>
    public abstract class TextFieldControl : UserControl, ILabel
    {
        /// <summary>
        /// Mode of value operating.
        /// </summary>
        public enum Mode
        {
            /// <summary>
            /// Type not defined yet.
            /// </summary>
            Undefined,
            /// <summary>
            /// Allow any string value.
            /// </summary>
            String,
            /// <summary>
            /// Allow only int formated values.
            /// </summary>
            Int,
            /// <summary>
            /// Allow only float values.
            /// </summary>
            Float,
            /// <summary>
            /// Allow only double values.
            /// </summary>
            Double,
            /// <summary>
            /// TODO: Use custom regex to define if value is valid.
            /// </summary>
            Regex
        }

        #region Dependency properties
        /// <summary>
        /// Property that bridging control's property between XAML and code.
        /// </summary>
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
          "Text", typeof(string), typeof(TextFieldControl));

        /// <summary>
        /// Property that bridging control's property between XAML and code.
        /// </summary>
        public static readonly DependencyProperty ValueModeProperty = DependencyProperty.Register(
          "ValueMode", typeof(Mode), typeof(TextFieldControl));

        /// <summary>
        /// Property that bridging control's property between XAML and code.
        /// </summary>
        public static readonly RoutedEvent TextChangedEvent = EventManager.RegisterRoutedEvent("TextChanged",
            RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(TextFieldControl));

        /// <summary>
        /// Property that bridging control's property between XAML and code.
        /// </summary>
        public static readonly DependencyProperty LabelProperty = DependencyProperty.Register(
          "Label", typeof(string), typeof(TextFieldControl));

        /// <summary>
        /// Property that bridging control's property between XAML and code.
        /// </summary>
        public static readonly DependencyProperty LabelWidthProperty = DependencyProperty.Register(
          "LabelWidth", typeof(float), typeof(TextFieldControl), new PropertyMetadata(120.0f));

        /// <summary>
        /// Property that bridging control's property between XAML and code.
        /// </summary>
        public static readonly DependencyProperty TextBoxForegroundProperty = DependencyProperty.Register(
          "TextBoxForeground", typeof(Brush), typeof(TextFieldControl),
          new PropertyMetadata(new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFF"))));

        /// <summary>
        /// Property that bridging control's property between XAML and code.
        /// </summary>
        public static readonly DependencyProperty TextBoxBackgroundProperty = DependencyProperty.Register(
          "TextBoxBackground", typeof(Brush), typeof(TextFieldControl),
          new PropertyMetadata(new SolidColorBrush((Color)ColorConverter.ConvertFromString("#00a9e9"))));
        #endregion

        #region Properties
        /// <summary>
        /// Occurs when content changes in the text element.
        /// </summary>
        public event TextChangedEventHandler TextChanged
        {
            add
            {
                // добавление обработчика
                base.AddHandler(TextBox.TextChangedEvent, value);
            }
            remove
            {
                // удаление обработчика
                base.RemoveHandler(TextBox.TextChangedEvent, value);
            }
        }

        /// <summary>
        /// Text in textbox.
        /// </summary>
        public virtual string Text
        {
            get { return (string)this.GetValue(TextProperty); }
            set { this.SetValue(TextProperty, value); }
        }

        /// <summary>
        /// Text in textbox.
        /// </summary>
        public virtual Mode ValueMode
        {
            get { return (Mode)this.GetValue(ValueModeProperty); }
            set { this.SetValue(ValueModeProperty, value); }
        }

        /// <summary>
        /// Applyong value as lable content.
        /// </summary>
        /// <remarks>
        /// Allow only the <see cref="string"/> values.</remarks>
        public new object Content
        {
            get { return Label; }
            set { Label = value as string; }
        }

        /// <summary>
        /// Uniform value.
        /// Allowed type depends from @ValueMode.
        /// </summary>
        public virtual object Value
        {
            get
            {
                try
                {
                    // Applying the value.
                    switch (ValueMode)
                    {
                        case Mode.String:
                        case Mode.Regex: return Text;

                        case Mode.Int: return int.Parse(Text);
                        case Mode.Float: return float.Parse(Text);
                        case Mode.Double: return double.Parse(Text);
                    }
                }
                catch { }

                return null;
            }
            set
            {
                // Definig mode if not defined yet.
                if (ValueMode == Mode.Undefined)
                {
                    DefineModeByType(value.GetType());
                }

                try
                {
                    if (ValueMode != Mode.Regex)
                    {
                        // Trying to conver value to the string.
                        Text = value.ToString();
                    }
                    else
                    {
                        // TODO: Aplly after regex validation.
                        throw new NotSupportedException();
                    }
                }
                catch { }
            }
        }

        /// <summary>
        /// Text in label field.
        /// </summary>
        public string Label
        {
            get { return (string)GetValue(LabelProperty); }
            set { SetValue(LabelProperty, value); }
        }

        /// <summary>
        /// Width of label field.
        /// </summary>
        public float LabelWidth
        {
            get { return (float)GetValue(LabelWidthProperty); }
            set
            {
                // Buferize requested value.
                _LabelWidth = value;

                // Set value but apply at least 25 point to input field.
                float appliedSize = (float)Math.Min(_LabelWidth, ActualWidth - 25);

                // Appling value.
                SetValue(LabelWidthProperty, appliedSize);
            }
        }

        /// <summary>
        /// Bufer that contains las requested label width.
        /// </summary>
        protected float _LabelWidth;

        /// <summary>
        /// Color of the text in textbox.
        /// </summary>
        public Brush TextBoxForeground
        {
            get { return (Brush)GetValue(TextBoxForegroundProperty); }
            set { SetValue(TextBoxForegroundProperty, value); }
        }

        /// <summary>
        /// Collor of the text box backplate.
        /// </summary>
        public Brush TextBoxBackground
        {
            get { return (Brush)GetValue(TextBoxBackgroundProperty); }
            set { SetValue(TextBoxBackgroundProperty, value); }
        }
        #endregion

        #region Customization block
        /// <summary>
        /// Returns reference to the label block of UI element.
        /// </summary>
        public abstract FrameworkElement LabelElement { get; }

        /// <summary>
        /// Returns reference to the field block of UI element.
        /// </summary>
        public abstract FrameworkElement FieldElement { get; }
        #endregion

        #region Constructor & desctructor
        /// <summary>
        /// Initizlize base UserControl contructor.
        /// </summary>
        protected TextFieldControl()
        {
            // Subscribing on the base events.
            SizeChanged += TextFieldControl_SizeChanged; 
            Loaded += TextFieldControl_Loaded;
        }

        /// <summary>
        /// Releasing unmanaged memory.
        /// </summary>
        ~TextFieldControl()
        {
            // Insubscribe from internal events.
            try { SizeChanged -= TextFieldControl_SizeChanged; } catch { }
            try { Loaded -= TextFieldControl_Loaded; } catch { }
        }
        #endregion

        #region API
        /// <summary>
        /// Recomputing dinamic layout values for providing hight quiality view.
        /// </summary>
        public virtual void RecomputeLayout()
        {
            var lableExist = !string.IsNullOrEmpty(Label);

            if (lableExist)
            {
                // Show label.
                LabelElement.Visibility = Visibility.Visible;

                // Warping the input field.
                Grid.SetColumn(FieldElement, 2);
            }
            else
            {
                // Hide label.
                LabelElement.Visibility = Visibility.Collapsed;

                // Spreading the input field.
                Grid.SetColumn(FieldElement, 0);
            }

            // Reqcomputing width.
            LabelWidth = _LabelWidth;
        }

        /// <summary>
        /// Defining an element mode by the target type.
        /// </summary>
        /// <param name="type">Type applyied to that field.</param>
        public void DefineModeByType(Type type)
        {
            var typeCode = Type.GetTypeCode(type);
            switch (typeCode)
            {
                case TypeCode.Single: ValueMode = Mode.Float; break;
                case TypeCode.Double: ValueMode = Mode.Double; break;
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64: ValueMode = Mode.Int; break;
                default: ValueMode = Mode.String; break;
            }
        }
        #endregion

        #region Callbacks
        /// <summary>
        /// Init configs when all properties applied.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextFieldControl_Loaded(object sender, RoutedEventArgs e)
        {
            // Update default request label size.
            _LabelWidth = LabelWidth;

            // Recomputing dinamic layout.
            RecomputeLayout();
        }

        /// <summary>
        /// Occurs when element size will shanged.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextFieldControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (IsLoaded)
            {
                RecomputeLayout();
            }
        }
        #endregion
    }
}
