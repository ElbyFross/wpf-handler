﻿//Copyright 2019 Volodymyr Podshyvalov
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
using System.Reflection;
using System.Text;
using System.Windows.Threading;
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
using WpfHandler.UI.AutoLayout.Markups;

namespace WpfHandler.UI.Controls
{
    /// <summary>
    /// TODO: Operating by group of the toggles.
    /// </summary>
    [TypesCompatible(typeof(Object), typeof(Enum))]
    [EnumsCompatible]
    public partial class FlatTogglesGroup : UserControl, ILayoutOrientation, ILabel, IGUIField
    {
        #region Dependency properties
        /// <summary>
        /// Property that bridging control's property between XAML and code.
        /// </summary>
        public static readonly DependencyProperty LabelProperty = DependencyProperty.Register(
          "Label", typeof(string), typeof(FlatTogglesGroup));

        /// <summary>
        /// Property that bridging control's property between XAML and code.
        /// </summary>
        public static readonly DependencyProperty LabelWidthProperty = DependencyProperty.Register(
          "LabelWidth", typeof(float), typeof(FlatTogglesGroup), new PropertyMetadata(float.NaN));
        
        /// <summary>
        /// Property that bridging control's property between XAML and code.
        /// </summary>
        public static readonly DependencyProperty FieldsContentProperty = DependencyProperty.Register(
          "FieldsContent", typeof(Array), typeof(FlatTogglesGroup));
        #endregion

        #region Public members
        /// <summary>
        /// Type that binded to that GUI element.
        /// </summary>
        public Type BindedEnumType { get; protected set; }

        /// <summary>
        /// Return an array with values of the binded tnum type.
        /// Using the <see cref="FieldsContent"/> in case if <see cref="BindedEnumType"/> is null.
        /// </summary>
        public Array Values
        {
            get
            {
                if (BindedEnumType == null) return FieldsContent;

                if (_Values == null)
                {
                    #region Validating source
                    // Check if the source exist.
                    //if (BindedEnumType == null)
                    //    throw new NullReferenceException("You must call OnGUI before calling that Values property.");

                    // Check if the source is enum.
                    if (!BindedEnumType.IsEnum)
                        throw new NotSupportedException("The BindedEnumType is not Enum.");
                    #endregion

                    // Getting values.
                    _Values = BindedEnumType.GetEnumValues();
                }

                return _Values;
            }
        }

        /// <summary>
        /// Layout orientation of the UI elements.
        /// </summary>
        public Orientation Orientation
        {
            get { return _Orientation; }
            set
            {
                // Updating value.
                _Orientation = value;

                UpdateElementsLayout();
            }
        }

        /// <summary>
        /// Instiniated element in direct order.
        /// </summary>
        public FrameworkElement[] Elements
        {
            get
            {
                // Init element if not exist.
                if (_Elements == null)
                {
                    InstiniateElements();
                }
                return _Elements;
            }
            protected set
            {
                _Elements = value;
            }
        }
        
        /// <summary>
        /// Text in label field.
        /// </summary>
        public Array FieldsContent
        {
            get { return (Array)GetValue(FieldsContentProperty); }
            set { SetValue(FieldsContentProperty, value); }
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
        /// <remarks>
        /// Not affective in case if <see cref="Orientation"/> is non the <see cref="System.Windows.Controls.Orientation.Horizontal"/>.
        /// </remarks>
        public float LabelWidth
        {
            get { return (float)GetValue(LabelWidthProperty); }
            set
            {
                // Drops in case if orientation not is horizontal.
                if (Orientation != Orientation.Horizontal) return;

                // Buferize requested value.
                LastLabelWidth = value;

                // Set value but apply at least 25 point to input field.
                float appliedSize = (float)Math.Min(LastLabelWidth, ActualWidth - 25);
                appliedSize = Math.Max(0, appliedSize);

                // Appling value.
                SetValue(LabelWidthProperty, appliedSize);              
            }
        }

        /// <summary>
        /// Value of that control.
        /// </summary>
        /// <remarks>
        /// Set allowed only for the enum values.
        /// For applying custom list use <see cref="FieldsContent"/> property.
        /// </remarks>
        public object Value
        {
            get => Values.GetValue(Index);
            set
            {
                // Initialize if core data is invalid.
                if(BindedEnumType == null)
                {
                    var type = value.GetType();
                    // Drop if source is not enum.
                    if (!type.IsEnum) throw new NotSupportedException("Shared member must be enum");
                    // Storing received type.
                    BindedEnumType = type;
                }

                // Looking for relative index.
                int targetIndex = -1;
                for (int i = 0; i < Values.Length; i++)
                {
                    // Comparing values.
                    if(Values.GetValue(i).Equals(value))
                    {
                        // Set as target indes if found.
                        targetIndex = i;
                        break;
                    }
                }

                // Applying index
                Index = targetIndex;
            }
        }

        /// <summary>
        /// Current selected element in the group.
        /// </summary>
        public int Index
        {
            get { return _Index; }
            set
            {
                // Preventing out of index exception.
                _Index = Math.Min(value, Values.Length);
                // Check if less then 0.
                _Index = Math.Max(_Index, 0);

                if (Elements != null)
                {
                    // Updating UI.
                    ((SelectableFlatButton)Elements[Index]).Selected = true;

                    // Inform subscribers.
                    ValueChanged?.Invoke(this, new object[0]);
                }
            }
        }

        /// <summary>
        /// Returns current selected UI element.
        /// </summary>
        public FrameworkElement SelectedElement
        {
            get { return Elements[Index]; }
        }

        /// <summary>
        /// Memeber that will be used as source\target for the value into UI.
        /// </summary>
        /// <remarks>
        /// The SET option is not supported.
        /// </remarks>
        public MemberInfo BindedMember
        {
            get { return _BindedMember; }
            set => throw new NotSupportedException();
        }

        /// <summary>
        /// Panel that contains instiniated elemtnts.
        /// </summary>
        public Panel ItemsPanel {get; protected set; }
        #endregion

        #region Protected members
        /// <summary>
        /// Bufer that contains las requested label width.
        /// </summary>
        protected float LastLabelWidth = 0;

        /// <summary>
        /// Bufer that contains current layout orientation.
        /// </summary>
        protected Orientation _Orientation = Orientation.Vertical;
        
        /// <summary>
        /// Bufer that contains selected index.
        /// </summary>
        protected int _Index = 0;

        /// <summary>
        /// Bufer that contains reference to the binded member.
        /// </summary>
        protected MemberInfo _BindedMember;

        /// <summary>
        /// Bufer that contains the values of the binded enum.
        /// </summary>
        protected Array _Values;

        /// <summary>
        /// Array that contains instiniated UI elements.
        /// </summary>
        protected FrameworkElement[] _Elements;
        #endregion

        /// <summary>
        /// Event that will occure in case if value of the field will be changed.
        /// Will cause updating of the BindedMember value.
        /// 
        /// IGUIField - sender.
        /// </summary>
        public event Action<IGUIField, object[]> ValueChanged;


        /// <summary>
        /// Initialize component.
        /// </summary>
        public FlatTogglesGroup()
        {
            InitializeComponent();
            DataContext = this;
        }

        /// <summary>
        /// Modify current layer's layout according to GUI element requirments.
        /// Calls once during UI spawn.
        /// </summary>
        /// <param name="layer">Target GUI layer.</param>
        /// <param name="args">Shared arguments. Must contains <see cref="MemberInfo"/>.</param>
        /// <remarks>
        /// Allow castomization enum's element by adding multiply <see cref="ContentAttribute"/>. 
        /// First attribute will applied to the common label. 
        /// Any next <see cref="ContentAttribute"/> will be related to the elements by the direct order.
        /// </remarks>
        public void OnLayout(ref LayoutLayer layer, params object[] args)
        {
            //Dispatcher.Invoke(DispatcherPriority.Background,
            //      new Action(delegate ()
            //      {
                      #region Getting shared data
                      // Find required referendes.
                      MemberInfo member = null;

                      // Trying to get shared properties.
                      foreach (object obj in args)
                      {
                          if (obj is MemberInfo)
                          {
                              member = (MemberInfo)obj;
                              break;
                          }
                      }

                      // Drop if member not shared.
                      if (member == null) return;

                      // Buferizing
                      _BindedMember = member;
                      #endregion

                      #region Getting description info
                      // Getting member's type.
                      Type memberType = UIDescriptor.MembersHandler.GetSpecifiedMemberType(member);
                      // Drop if source is not enum.
                      if (!memberType.IsEnum) throw new NotSupportedException("Shared member must be enum");
                      // Storing received type.
                      BindedEnumType = memberType;
                      #endregion

                      // Instiniating UI elements.
                      InstiniateElements();
                  //}));
        }

        /// <summary>
        /// Applying relevant layout to the elements.
        /// </summary>
        public void UpdateElementsLayout()
        {
            if (_Elements == null) return;

            // Finilising old layout if exist.
            if (ItemsPanel != null)
            {
                // Unbind elements from deprecated layout.
                foreach (FrameworkElement element in Elements)
                {
                    ItemsPanel.Children.Remove(element);
                }

                // Removing layout.
                canvas.Children.Remove(ItemsPanel);
            }

            if (Orientation == Orientation.Horizontal)
            {
                // Instiniating the panel.
                ItemsPanel = new Grid()
                {
                    VerticalAlignment = VerticalAlignment.Center
                };

                // Applying elements to the new layout.
                foreach (FrameworkElement element in Elements)
                {
                    LayoutHandler.HorizontalLayoutAddChild(ItemsPanel, element);
                }
            }
            else
            {
                // Instiniating the panel.
                ItemsPanel = new VirtualizingStackPanel()
                { Orientation = Orientation.Vertical };

                // Applying elements to the new layout.
                foreach (FrameworkElement element in Elements)
                {
                    LayoutHandler.VerticalLayoutAddChild(ItemsPanel, element);
                }

            }

            // Applying layout to the canvas.
            canvas.Children.Add(ItemsPanel);

            // Defining visibility.
            if (LastLabelWidth <= 0 || string.IsNullOrEmpty(Label))
            {
                // Hide the label.
                label.Visibility = Visibility.Collapsed;

                // Expand an items panel.
                if (ItemsPanel != null)
                {
                    Grid.SetRow(ItemsPanel, 0);
                    Grid.SetRowSpan(ItemsPanel, 2);
                }
            }
            else
            {
                // Show the label.
                label.Visibility = Visibility.Visible;

                // Warping an items panel.
                if (ItemsPanel != null)
                {
                    Grid.SetRow(ItemsPanel, 1);
                    Grid.SetRowSpan(ItemsPanel, 1);
                }
            }
        }


        /// <summary>
        /// Instiniating UI elements from binded meta.
        /// </summary>
        protected void InstiniateElements()
        {
            if (BindedEnumType != null) InstiniateEnumElements();
            else InstiniateCustomElements();
        }

        /// <summary>
        /// Instiniating elelements for the custom <see cref="FieldsContent"/> list.
        /// </summary>
        protected void InstiniateCustomElements()
        {
            // Instiniating the array.
            Elements = new FrameworkElement[FieldsContent.Length];

            // Generating uniquem token for that UI group.
            var groupToken = Guid.NewGuid().ToString();

            // Perform oparation for every element.
            for (int i = 0; i < FieldsContent.Length; i++)
            {
                // Store index relavant for that element for local methods.
                var localIndexBufer = i;

                // Instiniating new UI element.
                var element = new SelectableFlatButton()
                {
                    Group = groupToken
                };

                // Handling the button click.
                element.Click += delegate (object sender, RoutedEventArgs args)
                {
                    // Updating current selected index.
                    _Index = localIndexBufer;

                    // Inform subscribers.
                    ValueChanged?.Invoke(this, new object[0]);
                };

                // Adding to the collection.
                Elements[i] = element;

                // Binding the content to the field.
                ((GUIContent)FieldsContent.GetValue(i)).BindToLabel(element);
            }

            // Activating current option.
            ((SelectableFlatButton)Elements[Index]).Selected = true;
        }

        /// <summary>
        /// Instiniated elements by the binded Enum type.
        /// </summary>
        protected void InstiniateEnumElements()
        {
            #region Getting meta data
            // Getting default members.
            var names = BindedEnumType.GetEnumNames();
            var values = BindedEnumType.GetEnumValues();

            // Getting defined content.
            var typeContents = BindedEnumType?.GetCustomAttributes<ContentAttribute>().ToArray();
            var memberContents = BindedMember?.GetCustomAttributes<ContentAttribute>().ToArray();

            // Instiniating the array.
            Elements = new FrameworkElement[names.Length];
            #endregion

            #region Instiniating UI elements
            // Generating uniquem token for that UI group.
            var groupToken = Guid.NewGuid().ToString();

            // Perform oparation for every element.
            for (int i = 0; i < names.Length; i++)
            {
                // Store index relavant for that element for local methods.
                var localIndexBufer = i;

                // Instiniating new UI element.
                var element = new SelectableFlatButton()
                {
                    Group = groupToken
                };

                // Handling the button click.
                element.Click += delegate (object sender, RoutedEventArgs args)
                {
                    // Updating current selected index.
                    _Index = localIndexBufer;

                    // Inform subscribers.
                    ValueChanged?.Invoke(this, new object[0]);
                };

                // Adding to the collection.
                Elements[i] = element;

                // Applying label's value
                try
                {
                    // Trying to bind a dynamic content.
                    int index = i + 1;
                    if (memberContents?.Count() > index)
                        memberContents[index].BindToLabel(element);
                    else
                        typeContents[i + 1].BindToLabel(element);
                }
                catch { element.Label = names[i]; } // Loading fom the member's data..
            }

            // Activating current option.
            ((SelectableFlatButton)Elements[Index]).Selected = true;
            #endregion
        }

        /// <summary>
        /// Occurs when the element is loaded.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            LastLabelWidth = (float)label.Width;

            // Set elements to the layout.
            UpdateElementsLayout();
        }
    }
}
