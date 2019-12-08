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
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using WpfHandler.UI.AutoLayout.Configuration;

namespace WpfHandler.UI.Controls
{
    /// <summary>
    /// Creating UI element for work with binded collections.
    /// </summary>
    [TypesCompatible(typeof(Object))]
    [EnumerableCompatible]
    public partial class AutoCollection : CollectionControl
    {
        #region Dependency properties
        /// <summary>
        /// Property that bridging control's property between XAML and code.
        /// </summary>
        public static readonly DependencyProperty BackplateBackgroundProperty = DependencyProperty.Register(
          "BackplateBackground", typeof(Brush), typeof(AutoCollection),
          new PropertyMetadata(Brushes.WhiteSmoke));

        /// <summary>
        /// Property that bridging control's property between XAML and code.
        /// </summary>
        public static readonly DependencyProperty SpliterColorProperty = DependencyProperty.Register(
          "SpliterColor", typeof(Brush), typeof(AutoCollection),
          new PropertyMetadata(Brushes.LightGray));

        /// <summary>
        /// Property that bridging control's property between XAML and code.
        /// </summary>
        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register(
          "CornerRadius", typeof(double), typeof(AutoCollection),
          new PropertyMetadata(7.0d));

        /// <summary>
        /// Property that bridging control's property between XAML and code.
        /// </summary>
        public static readonly DependencyProperty SplitersDrawProperty = DependencyProperty.Register(
          "SplitersDraw", typeof(bool), typeof(AutoCollection),
          new PropertyMetadata(true));

        /// <summary>
        /// Property that bridging control's property between XAML and code.
        /// </summary>
        public static readonly DependencyProperty AddButtonVisibileProperty = DependencyProperty.Register(
          "AddButtonVisibile", typeof(bool), typeof(AutoCollection),
          new PropertyMetadata(true));

        /// <summary>
        /// Property that bridging control's property between XAML and code.
        /// </summary>
        public static readonly DependencyProperty RemoveButtonVisibileProperty = DependencyProperty.Register(
          "RemoveButtonVisibile", typeof(bool), typeof(AutoCollection),
          new PropertyMetadata(true));

        /// <summary>
        /// Property that bridging control's property between XAML and code.
        /// </summary>
        public static readonly RoutedEvent OnAddEvent = EventManager.RegisterRoutedEvent("OnAddClick",
            RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(AutoCollection));

        /// <summary>
        /// Property that bridging control's property between XAML and code.
        /// </summary>
        public static readonly RoutedEvent OnRemoveEvent = EventManager.RegisterRoutedEvent("OnRemoveClick",
            RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(AutoCollection));
        #endregion

        #region Public members
        /// <summary>
        /// If the add button available for an user.
        /// </summary>
        public bool AddButtonVisibile
        {
            get { return (bool)this.GetValue(AddButtonVisibileProperty); }
            set
            {
                // Updating stored value.
                this.SetValue(AddButtonVisibileProperty, value);

                // Recomputing UI.
                RecomputeLayout();
            }
        }

        /// <summary>
        /// Is the remove button available for an user.
        /// </summary>
        public bool RemoveButtonVisibile
        {
            get { return (bool)this.GetValue(RemoveButtonVisibileProperty); }
            set 
            { 
                // Updating stored value.
                this.SetValue(RemoveButtonVisibileProperty, value);

                // Recomputing UI.
                RecomputeLayout();
            }
        }

        /// <summary>
        /// Is the spliting lines between content elements are visible.
        /// </summary>
        public bool SplitersDraw { get; set; } = true;

        /// <summary>
        /// Radius of the rounded corders.
        /// </summary>
        public double CornerRadius
        {
            get { return (double)this.GetValue(CornerRadiusProperty); }
            set { this.SetValue(CornerRadiusProperty, value); }
        }

        /// <summary>
        /// Color of the spliters.
        /// </summary>
        public Brush SpliterColor
        {
            get { return (Brush)this.GetValue(SpliterColorProperty); }
            set
            {
                this.SetValue(SpliterColorProperty, value);
                try
                {
                    // Is spliter active?
                    if (SplitersDraw)
                    {
                        // Update every element.
                        foreach (FrameworkElement element in Elements)
                        {

                            // Operating if element is a panel.
                            if (element is Panel panel)
                            {
                                // Getting spliter element.
                                var spliter = panel.Children[1] as Grid;

                                // Applying new color.
                                spliter.Background = value;
                            }
                        }
                    }
                }
                catch { }
            }
        }

        /// <summary>
        /// Color of the backplate.
        /// </summary>
        public Brush BackplateBackground
        {
            get { return (Brush)this.GetValue(BackplateBackgroundProperty); }
            set { this.SetValue(BackplateBackgroundProperty, value); }
        }

        /// <summary>
        /// Bindted lsit element that will manage items.
        /// </summary>
        public override ListBox ListContent => contentPanel;

        /// <summary>
        /// The delegate that will be called during Add button click.
        /// In case if initialized then will be called instead default.
        /// </summary>
        public Action<object> OnAddClick;

        /// <summary>
        /// The delegate that will be called during Remove button click.
        /// In case if initialized then will be called instead default.
        /// </summary>
        public Action<object> OnRemoveClick;
        #endregion

        #region Constructor & destructors
        /// <summary>
        /// Initialize that component.
        /// Looking for the `AutoCollection` Style resource. Use default if not found.
        /// </summary>
        public AutoCollection() : base()
        {
            InitializeComponent();
            base.DataContext = this;

            // Try to load default style
            try
            {
                if (Application.Current.FindResource("AutoCollection") is Style style)
                {
                    Style = style;
                }
            }
            catch
            {
                // Not found in dictionary. Not important.
            }

            Loaded += AutoCollection_Loaded;
        }
        #endregion

        /// <summary>
        /// Recomputing element layout.
        /// </summary>
        protected void RecomputeLayout()
        {
            // Getting current states.
            bool addButtonState = AddButtonVisibile;
            bool removeButtonState = RemoveButtonVisibile;
            
            // Prevent changing in case of fixed size.
            if(IsFixedSize)
            {
                addButtonState = false;
                removeButtonState = false;
            }

            // Updating the backplate visibility.
            bool twoButtonsEnabled = addButtonState && removeButtonState;
            buttonsBridge.Visibility = twoButtonsEnabled ? Visibility.Visible : Visibility.Collapsed;

            // Updating buttons visibility.
            addButtonGroup.Visibility = addButtonState ? Visibility.Visible : Visibility.Collapsed;
            removeButtonGroup.Visibility = removeButtonState ? Visibility.Visible : Visibility.Collapsed;

            // Updating the state of the bridge between the panel and buttons.
            cornerBackplate.Visibility = removeButtonGroup.Visibility;

            // Updating the grid layout.
            bool anyButtonEnabled = addButtonState || removeButtonState;
            Grid.SetRowSpan(collectionPanel, anyButtonEnabled ? 1 : 2);
        }

        #region Callbacks
        /// <summary>
        /// Registrating an item of the list.
        /// </summary>
        /// <param name="index">An index of item into the source collection.</param>
        /// <returns></returns>
        protected override FrameworkElement ItemRegistration(int index)
        {
            // Getting base element.
            var element = base.ItemRegistration(index);

            // Adding spliters if requested.
            if(SplitersDraw)
            {
                // Instiniating panel that will contains the layout.
                var panel = new StackPanel()
                {
                    Orientation = Orientation.Vertical
                };
                
                // Instiniating spliter.
                var spliter = new Grid()
                {
                    Height = 1,
                    MaxHeight = 1,
                    Background = SpliterColor,
                    Margin = new Thickness(-3, 3, -3 ,0),
                    IsHitTestVisible = false
                };
                
                // Add new elements to the layout parent.
                panel.Children.Add(element);
                panel.Children.Add(spliter);

                // Returning parent as target element.
                return panel;
            }

            // Returning defult element.
            return element;
        }

        /// <summary>
        /// Occurs when user pressing add button.
        /// Adding an element of the type that described 
        /// in the first generic type argument of the source <see cref="IList"/>.
        /// </summary>
        /// <param name="sender">An element that caused that callback.</param>
        protected void OnAdd(object sender)
        {
            // Call the custom callback instead default.
            if(OnAddClick != null)
            {
                OnAddClick.Invoke(this);
                return;
            }

            try
            {
                // Trying to get generic type.
                var defaultElementType = source.GetType().GenericTypeArguments[0];

                // Instiniating object.
                var element = Activator.CreateInstance(defaultElementType);

                //Adding object to the collection.
                Add(element);
            }
            catch (Exception ex)
            {
                // Log message.
                MessageBox.Show(
                    "ERROR:" +
                    "\nCan't add new element." +
                    "\n\nDeltails:\n" +
                    ex.Message);
            }
        }

        /// <summary>
        /// Occurs when user pressing remove button.
        /// </summary>
        /// <param name="sender">>An element that caused that callback.</param>
        protected void OnRemove(object sender)
        {
            // Call the custom callback instead default.
            if (OnRemoveClick != null)
            {
                OnRemoveClick.Invoke(this);
                return;
            }


            var index = ListContent.SelectedIndex; // Getting the selected index.
            if (index == -1) return; // Drop if not selected.

            RemoveAt(index); // Requesting removing.
        }

        /// <summary>
        /// Occurs when the contol is loaded.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AutoCollection_Loaded(object sender, RoutedEventArgs e)
        {
            RecomputeLayout();
        }
        #endregion
    }
}
