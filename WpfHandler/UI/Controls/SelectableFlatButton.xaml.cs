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
using System.Collections;
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

namespace WpfHandler.UI.Controls
{
    /// <summary>
    /// Interaction logic for SelectableFlatButton.xaml
    /// </summary>
    public partial class SelectableFlatButton : UserControl, ILabel, IToggleControl
    {
        /// <summary>
        /// Event that will be called when button will be pressed.
        /// </summary>
        public static readonly RoutedEvent ClickEvent;

        #region Dependency properties
        /// <summary>
        /// Bridging XAML declaring and the member.
        /// </summary>
        public static readonly DependencyProperty LabelProperty = DependencyProperty.Register(
          "Label", typeof(string), typeof(SelectableFlatButton), new PropertyMetadata("Sample"));

        /// <summary>
        /// Bridging XAML declaring and the member.
        /// </summary>
        public static readonly DependencyProperty SelectedProperty = DependencyProperty.Register(
          "Selected", typeof(bool), typeof(SelectableFlatButton));

        /// <summary>
        /// Bridging XAML declaring and the member.
        /// </summary>
        public static readonly DependencyProperty GroupProperty = DependencyProperty.Register(
          "Group", typeof(string), typeof(SelectableFlatButton), new PropertyMetadata("default"));

        /// <summary>
        /// Bridging XAML declaring and the member.
        /// </summary>
        public static readonly DependencyProperty MultiSelectionProperty = DependencyProperty.Register(
          "MultiSelection", typeof(bool), typeof(SelectableFlatButton), new PropertyMetadata(false));

        /// <summary>
        /// Bridging XAML declaring and the member.
        /// </summary>
        public static readonly DependencyProperty HightlightBackgroundProperty = DependencyProperty.Register(
          "HightlightBackground", typeof(Brush), typeof(SelectableFlatButton),
          new PropertyMetadata(Brushes.Yellow));
        #endregion

        #region Static members
        /// <summary>
        /// Ivent that will bne called when button wil selected.
        /// </summary>
        public static event Action<SelectableFlatButton> ButtonActivated;

        /// <summary>
        /// Hashtable that contain registred groups's items.
        /// </summary>
        private static readonly Hashtable activeGroupsItems = new Hashtable();
        #endregion

        #region Public members
        /// <summary>
        /// Text that will be displayed on the button.
        /// </summary>
        public string Label
        {
            get { return (string)GetValue(LabelProperty); }
            set { SetValue(LabelProperty, value); }
        }

        /// <summary>
        /// Occurs when a <see cref="SelectableFlatButton"/> is clicked.
        /// </summary>
        public event RoutedEventHandler Click
        {
            add => this.AddHandler(SelectableFlatButton.ClickEvent, value);
            remove => this.RemoveHandler(SelectableFlatButton.ClickEvent, value);
        }

        /// <summary>
        /// Does that button selected.
        /// </summary>
        public bool Selected
        {
            get { return (bool)main.GetValue(SelectedProperty); }
            set
            {
                // Update value.
                main.SetValue(SelectedProperty, value);

                // Update UI.
                mark.Opacity = value ? 1 : 0;
            }
        }

        /// <summary>
        /// Group of buttons that will allow auto deselect other buttons from that group.
        /// </summary>
        public string Group
        {
            get { return (string)this.GetValue(GroupProperty); }
            set { this.SetValue(GroupProperty, value); }
        }

        /// <summary>
        /// Is group allow few selected buttons in one group.
        /// </summary>
        public bool MultiSelection
        {
            get { return (bool)button.GetValue(MultiSelectionProperty); }
            set { button.SetValue(MultiSelectionProperty, value); }
        }

        /// <summary>
        /// Color of highlight mark.
        /// </summary>
        public Brush HightlightBackground
        {
            get { return (Brush)mark.GetValue(BackgroundProperty); }
            set { mark.SetValue(BackgroundProperty, value); }
        }

        /// <summary>
        /// Not supported.
        /// </summary>
        public float LabelWidth { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
        #endregion


        #region Constructors
        static SelectableFlatButton()
        {
            ClickEvent = EventManager.RegisterRoutedEvent(
                "Click", RoutingStrategy.Bubble,
                typeof(RoutedEventHandler), typeof(SelectableFlatButton));
        }

        /// <summary>
        /// Default button.
        /// </summary>
        public SelectableFlatButton()
        {
            InitializeComponent();
            DataContext = main;
        }
        #endregion

        #region API
        /// <summary>
        /// Trying to get curernt selected button in certain group.
        /// </summary>
        /// <param name="group">Code of buttons' group.</param>
        /// <param name="button">Selected button. Null if not selected.</param>
        /// <returns>is any button selected?</returns>
        public static bool TryToGetSelected(string group, out SelectableFlatButton button)
        {
            // Lood collection by group.
            if (string.IsNullOrEmpty(group) &&
                activeGroupsItems[group] is List<SelectableFlatButton> collection)
            {
                // Check every registred button.
                foreach (SelectableFlatButton bufer in collection)
                {
                    // Selected found.
                    if (bufer.Selected)
                    {
                        button = bufer;
                        return true;
                    }
                }

                // Not found.
                button = null;
                return false;
            }
            else
            {
                // invalid data.
                button = null;
                return false;
            }
        }
        #endregion

        #region Callbacks
        /// <summary>
        /// Callback that will be called when button will be clicked.
        /// </summary>
        /// <param name="clicked"></param>
        /// <param name="__">Not using.</param>
        public void SelectedCallback(object clicked, RoutedEventArgs __)
        {
            // Inform subscribers.
            ButtonActivated?.Invoke(this);

            // Hightlights as selected.
            Selected = true;

            // Manage collection of that group.
            if (!MultiSelection &&
                Group != null &&
                activeGroupsItems[Group] is List<SelectableFlatButton> collection)
            {
                foreach (SelectableFlatButton button in collection)
                {
                    // Skip if the same answer.
                    if (button.Equals(this)) continue;

                    // Disable hightlight.
                    button.Selected = false;
                }
            }
        }

        /// <summary>
        /// Callback that will be called when UI finish loading.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UI_Loaded(object sender, RoutedEventArgs e)
        {
            // Registrate button in group.
            if (!string.IsNullOrEmpty(Group))
            {
                if (activeGroupsItems[Group] is List<SelectableFlatButton> collection)
                {
                    collection.Add(this);
                }
                else
                {
                    activeGroupsItems.Add(Group, new List<SelectableFlatButton>() { this });
                }
            }

            // Singup groups processing.
            Click += SelectedCallback;

            // Update selected status.
            Selected = Selected;

            // Update hightlight color.
            HightlightBackground = HightlightBackground;
        }

        /// <summary>
        /// Callback for button click.
        /// </summary>
        /// <param name="_">
        /// Not using. Will be overided on `this` cause listener 
        /// can't khow reference tp the child button control.</param>
        /// <param name="__">Not using.</param>
        public void OnButtonClick(object _, RoutedEventArgs __)
        {
            RoutedEventArgs e = new RoutedEventArgs(ClickEvent, this);
            RaiseEvent(e);
        }
        #endregion
    }
}
