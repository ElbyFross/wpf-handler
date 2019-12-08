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
    /// Abstract class that provides common API for implementin collection controls.
    /// </summary>
    /// <remarks>
    /// Fully compatible with <see cref="UIDescriptor"/> and auto layout handlers.
    /// </remarks>
    public abstract class CollectionControl : UserControl, IGUIField, IList
    {
        #region Dependency properties
        /// <summary>
        /// Property that bridging control's property between XAML and code.
        /// </summary>
        public static readonly DependencyProperty DragAllowedProperty = DependencyProperty.Register(
          "DragAllowed", typeof(bool), typeof(CollectionControl),
          new PropertyMetadata(true));
        #endregion

        #region Public properties
        /// <summary>
        /// Is list allows to drag elements.
        /// </summary>
        /// <remarks>
        /// Can be true only in case if<see cref="IsFixedSize"/> is false.
        /// </remarks>
        public virtual bool DragAllowed
        {
            get { return (bool)this.GetValue(DragAllowedProperty); }
            set 
            { 
                // Applying the value.
                this.SetValue(DragAllowedProperty, value);

                // Reconfigurating a style of the list.
                ConfigurateStyles();
            }
        }

        /// <summary>
        /// Reference to the list box that will contains spawned elements.
        /// </summary>
        public abstract ListBox ListContent { get; }

        /// <summary>
        /// Source collection applied to the UI.
        /// </summary>
        /// <remarks>
        /// Object must has implemented <see cref="IList"/> interface.
        /// </remarks>
        public object Value
        {
            get { return source; }
            set
            {
                // Drop if applied source is not IEnumerable.
                if (!(value is IList list))
                    throw new InvalidCastException("Source member mus implement IEnumerable interface.");

                // Updating referece.
                source = list;

                // Inform subscribers.
                ValueChanged?.Invoke(this);
            }
        }

        /// <summary>
        /// Member from UIDescriptor binded to the UI leement.
        /// </summary>
        public MemberInfo BindedMember { get; set; }
        
        /// <summary>
        /// UI elemets existing into the current collection.
        /// </summary>
        public ObservableCollection<FrameworkElement> Elements { get; } = new ObservableCollection<FrameworkElement>();

        /// <summary>
        /// Collection that contains  instiniated GUI fields.
        /// </summary>
        public ObservableCollection<IGUIField> Fields { get; } = new ObservableCollection<IGUIField>();

        /// <summary>
        /// Gets a value indicating whether the System.Collections.IList is read-only.
        /// </summary>
        public bool IsReadOnly => source.IsReadOnly;

        /// <summary>
        /// Gets a value indicating whether the System.Collections.IList has a fixed size.
        /// </summary>
        public bool IsFixedSize => source.IsFixedSize;

        /// <summary>
        /// Gets the number of elements contained in the System.Collections.ICollection.
        /// </summary>
        public int Count => source.Count;

        /// <summary>
        /// Gets an object that can be used to synchronize access to the System.Collections.ICollection.
        /// </summary>
        public object SyncRoot => throw new NotImplementedException();

        /// <summary>
        /// Gets a value indicating whether access to the System.Collections.ICollection
        /// is synchronized (thread safe).
        /// </summary>
        public bool IsSynchronized => throw new NotImplementedException();

        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get or set</param>
        /// <returns>The element at the specified index.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// index is not a valid index in the System.Collections.IList.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// The property is set and the System.Collections.IList is read-only.
        /// </exception>
        public object this[int index] 
        { 
            get => Fields[index].Value;
            set => Fields[index].Value = value;
        }
        #endregion

        #region Events
        /// <summary>
        /// Will occure when source or one from elements will change.
        /// </summary>
        public event Action<IGUIField> ValueChanged;
        #endregion

        #region Protected members
        /// <summary>
        /// Mapp of GUI elements binding.
        /// <see cref="IGUIField"/> - key
        /// <see cref="int"/> - index in the list.
        /// </summary>
        protected readonly Hashtable indexMap = new Hashtable();

        /// <summary>
        /// Bufer that contains source sollection.
        /// </summary>
        protected IList source;
        #endregion


        /// <summary>
        /// Default constructor.
        /// </summary>
        public CollectionControl()
        {
            // Subcribing on the after loading event.
            this.Loaded += delegate (object sender, RoutedEventArgs e)
            {
                // Apply a suitable style.
                ConfigurateStyles();

                // Recomputing UI.
                UpdateElementsWidth(null, null);

                // Subscribing of the size update.
                ListContent.SizeChanged += UpdateElementsWidth;
            };
        }

        #region API
        /// <summary>
        /// Adds an item to the System.Collections.IList.
        /// </summary>
        /// <param name="value">The object to add to the System.Collections.IList.</param>
        /// <returns>
        /// The position into which the new element was inserted, or -1 to indicate that
        /// the item was not inserted into the collection.
        /// </returns>
        /// <exception cref="System.NotSupportedException">
        /// The System.Collections.IList is read-only.-or- The System.Collections.IList has
        /// a fixed size.-or- <see cref="IGUIField"/> for the object type not registred.
        /// </exception>
        public int Add(object value)
        {
            // The bufer that contains position of element into the collection.
            int position = source.Count;

            // Inserting object.
            Insert(position, value);

            // Returning the defined pisition.
            return position;
        }

        /// <summary>
        /// Determines whether the System.Collections.IList contains a specific value.
        /// </summary>
        /// <param name="value">The object to locate in the System.Collections.IList.</param>
        /// <returns>
        /// true if the System.Object is found in the System.Collections.IList; otherwise,
        /// false.
        /// </returns>
        /// <remarks>
        /// In case if value is <see cref="IGUIField"/> then looking via registred UI fileds.-or-
        /// Otherwise looking via the <see cref="source"/> list.
        /// </remarks>
        public bool Contains(object value)
        {
            // Looking for an registred fields.
            if (value is IGUIField field) return Fields.Contains(field);

            // Looking for background value.
            return source.Contains(value);
        }

        /// <summary>
        /// Removes all items from the System.Collections.IList.
        /// </summary>
        /// <exception cref="System.NotSupportedException">
        /// The System.Collections.IList is read-only.
        /// </exception>
        public void Clear()
        {
            // Clearing the source.
            source.Clear();

            // Clearing registred fields.
            Fields.Clear();

            // Clearing UI elements.
            Elements.Clear();

            // Clearing index map.
            indexMap.Clear();

            // Inform subscribers.
            ValueChanged?.Invoke(this);
        }

        /// <summary>
        /// Determines the index of a specific item in the System.Collections.IList.
        /// </summary>
        /// <param name="value">The object to locate in the System.Collections.IList.</param>
        /// <returns>The index of value if found in the list; otherwise, -1.</returns>
        /// <remarks>
        /// In case if value is <see cref="IGUIField"/> then looking via registred UI fileds.-or-
        /// Otherwise looking via the <see cref="source"/> list.
        /// </remarks>
        public int IndexOf(object value)
        {
            // Looking for an registred field.
            if(value is IGUIField field) return Fields.IndexOf(field);

            return source.IndexOf(value);
        }

        /// <summary>
        /// Inserts an item to the System.Collections.IList at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which value should be inserted.</param>
        /// <param name="value">The object to insert into the System.Collections.IList.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// index is not a valid index in the System.Collections.IList.
        /// </exception>
        /// <exception cref="System.NotSupportedException">
        /// The System.Collections.IList is read-only.-or- The System.Collections.IList has
        /// a fixed size.
        /// </exception>
        /// <exception cref="System.NullReferenceException">
        /// value is null reference in the System.Collections.IList.
        /// </exception>
        public void Insert(int index, object value)
        {
            if (IsFixedSize)
            {
                throw new NotSupportedException(
                    "Can't add an element to the fixed size collection");
            }

            // If is already instiniated field.
            if (value is IGUIField field)
            {
                // Inserting the element to the source.
                source.Insert(index, field.Value);

                // Inserting ellement to the maping table.
                indexMap.Add(field, index);

                // Inserting reference to the field.
                Fields.Insert(index, field);

                // Add to the elements.
                Elements.Insert(index, GetRoot((FrameworkElement)field));

                // Subscribing on the index of the value changing.
                field.ValueChanged += CollectionElementValueChanged;
            }
            // If a new source value.
            else
            {
                // Getting data.
                source.Insert(index, value);

                // Calling an item registration.
                var element = ItemRegistration(index);

                // Adding element to the list.
                if (element != null) Elements.Add(element);
            }

            // Informing subscribers.
            ValueChanged?.Invoke(this);

            FrameworkElement GetRoot(FrameworkElement element)
            {
                if (element.Parent != null) return GetRoot(element.Parent as FrameworkElement);
                else return element;
            }
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the System.Collections.IList.
        /// </summary>
        /// <param name="value">The object to remove from the System.Collections.IList.</param>
        /// <exception cref="System.NotSupportedException">
        /// The System.Collections.IList is read-only.-or- The System.Collections.IList has
        /// a fixed size.
        /// </exception>
        public void Remove(object value)
        {
            // Looking for element into the list.
            int index = IndexOf(value);

            // Drop if not found.
            if (index == -1) return;

            // Removing by the index.
            RemoveAt(index);
        }

        /// <summary>
        /// Removes the System.Collections.IList item at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the item to remove.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// index is not a valid index in the System.Collections.IList.
        /// </exception>
        /// <exception cref="System.NotSupportedException">
        /// The System.Collections.IList is read-only.-or- The System.Collections.IList has
        /// a fixed size.
        /// </exception>
        public void RemoveAt(int index)
        {
            if (IsFixedSize)
            {
                throw new NotSupportedException(
                    "Can't remove an element from the fixed size collection");
            }

            // Removing value for the source.
            source.RemoveAt(index);

            // Removing the UI element.
            Elements.RemoveAt(index);

            // Getting registred filed.
            var field = Fields[index];
            List<IGUIField> registredFields = new List<IGUIField>();
            foreach (IGUIField collectionField in indexMap.Keys)
            {
                registredFields.Add(collectionField);
            }

            for(int i =0; i < registredFields.Count; i++)
            {
                var collectionField = registredFields[i];
                var internalIndex = (int)indexMap[collectionField];
                if (internalIndex > index)
                {
                    // Decrementing stored index.
                    indexMap[collectionField] = internalIndex - 1;
                }
            }
            // Removing field from index map.
            indexMap.Remove(field);

            // Unregistring the field  from collections.
            Fields.RemoveAt(index);

            // Unsubscribing from the index of the value changing.
            field.ValueChanged -= CollectionElementValueChanged;

            // Informing subscribers.
            ValueChanged?.Invoke(this);
        }

        /// <summary>
        /// Copies the elements of the <see cref="source"/> to an System.Array,
        /// starting at a particular System.Array index.
        /// </summary>
        /// <param name="array">The one-dimensional System.Array that is the destination of the elements copied
        /// from System.Collections.ICollection. The System.Array must have zero-based indexing.</param>
        /// <param name="index">The zero-based index in array at which copying begins.</param>
        /// <exception cref="System.ArgumentNullException">
        /// array is null.
        /// </exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// index is less than zero.
        /// </exception>
        /// <exception cref="System.ArgumentNullException">
        /// array is multidimensional.-or- The number of elements in the source System.Collections.ICollection
        /// is greater than the available space from index to the end of the destination
        /// array.-or-The type of the source System.Collections.ICollection cannot be cast
        /// automatically to the type of the destination array.
        /// </exception>
        public void CopyTo(Array array, int index)
        {
            // Copying the source.
            source.CopyTo(array, index);
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An System.Collections.IEnumerator object that can be used to iterate through
        /// the collection.
        /// </returns>
        public IEnumerator GetEnumerator()
        {
            return source.GetEnumerator();
        }
        #endregion

        /// <summary>
        /// Configurating collection and binding element to the descriptors handler.
        /// </summary>
        /// <param name="layer"></param>
        /// <param name="args"></param>
        public virtual void OnLayout(ref LayoutLayer layer, params object[] args)
        {
            // Clearing data.
            ListContent.Items.Clear();
            Elements.Clear();
            
            // Update GUI.
            for (int i = 0; i < source.Count; i++)
            {
                // Instinating an element.
                var element = ItemRegistration(i);

                // Adding element to the list.
                if (element != null) Elements.Add(element);
            }

            // Applying elements as source.
            ListContent.ItemsSource = Elements;
        }
        
        #region Local
        /// <summary>
        /// Requiesting recomputing of the elements width.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void UpdateElementsWidth(object sender, SizeChangedEventArgs e)
        {
            // Drop if the list not loaded yet.
            if(ListContent.IsLoaded == false)
            {
                // Subscribing on the after loadign event.
                ListContent.Loaded += delegate (object sender, RoutedEventArgs e)
                {
                    // Recal layout computing.
                    UpdateElementsWidth(sender, null);
                };
                // Drop.
                return;
            }

            // Update width of an every element.
            foreach (FrameworkElement field in Elements)
            {
                if (field != null)
                {
                    field.Width = ListContent.ActualWidth - 12;
                }
            }
        }

        /// <summary>
        /// Configurating styles applyed to the controls.
        /// </summary>
        protected void ConfigurateStyles()
        {
            // Defining contaier style.
            Style itemContainerStyle = new Style(typeof(ListBoxItem));

            // Prefenting drag into the fixed size collections.
            if(IsFixedSize)
            {
                this.SetValue(DragAllowedProperty, false);
            }

            // Enable drop possibility.
            if (DragAllowed)
            {
                itemContainerStyle.Setters.Add(new Setter(ListBoxItem.AllowDropProperty, true));
                itemContainerStyle.Setters.Add(new EventSetter(
                    ListBoxItem.PreviewMouseLeftButtonDownEvent,
                    new MouseButtonEventHandler(OnPreviewMouseLeftButtonDown)));
                itemContainerStyle.Setters.Add(new EventSetter(
                    ListBoxItem.DropEvent,
                    new DragEventHandler(OnItemDrop)));
            }
            // Disable drop possibility.
            else
            {
                itemContainerStyle.Setters.Add(new Setter(ListBoxItem.AllowDropProperty, false));
            }

            // Applying style to the list.
            ListContent.ItemContainerStyle = itemContainerStyle;
        }
        #endregion

        #region Callbacks
        /// <summary>
        /// Regestrating element for source collection.
        /// Allow to create bachward binding of UI to the source object.
        /// </summary>
        /// <param name="index">Index of the element into the <see cref="source"/> collection.</param>
        protected virtual FrameworkElement ItemRegistration(int index)
        {
            // Getting data.
            var obj = source[index];

            // Gettign type of the binded UI element.
            var controlType = LayoutHandler.GetBindedControl(obj.GetType(), true);

            // Drop if control not available.
            if (controlType == null) return null;

            // Instiniating UI element.
            var element = (IGUIField)Activator.CreateInstance(controlType);

            // Applying default value.
            element.Value = obj;

            // Adding ellement to the maping table.
            indexMap.Add(element, index);

            // Adding reference to the field.
            Fields.Add(element);

            // Subscribing on the index of the value changing.
            element.ValueChanged += CollectionElementValueChanged;

            return (FrameworkElement)element;
        }

        /// <summary>
        /// Occurs when field's valu will be changed via UI element.
        /// </summary>
        /// <param name="obj">The GUI element that initialize event.</param>
        protected virtual void CollectionElementValueChanged(IGUIField obj)
        {
            // If index for that element is registred.
            if (indexMap.ContainsKey(obj))
            {
                var index = (int)indexMap[obj];

                source[index] = obj.Value;
                
                // Inform subscribers.
                ValueChanged?.Invoke(this);
            }
            else
            {
                MessageBox.Show("IGUIField not binded to the list.");
            }
        }

        /// <summary>
        /// Occurs when item is selected by LMB click.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Drop if selected element is not a common grid or selectable border.
            if (!e.OriginalSource.GetType().Equals(typeof(Border))
                && !e.OriginalSource.GetType().Equals(typeof(Grid)))
            {
                return;
            }

            if (sender is ListBoxItem)
            {
                ListBoxItem draggedItem = sender as ListBoxItem;
                DragDrop.DoDragDrop(draggedItem, draggedItem.DataContext, DragDropEffects.Move);
                draggedItem.IsSelected = true;
            }
        }

        /// <summary>
        /// Occurs when an element was droped after drag.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnItemDrop(object sender, DragEventArgs e)
        {
            var droppedData = e.Data.GetData(e.Data.GetFormats()[0]) as FrameworkElement;
            var target = ((ListBoxItem)(sender)).DataContext as FrameworkElement;

            int removedIdx = ListContent.Items.IndexOf(droppedData);
            int targetIdx = ListContent.Items.IndexOf(target);

            // Drop if the same position.
            if (removedIdx == targetIdx) return;

            IGUIField dropedField = Fields[removedIdx];
            
            RemoveAt(removedIdx);
            Insert(targetIdx, dropedField);

            // Inform subscribers.
            ValueChanged?.Invoke(this);
        }       
        #endregion
    }
}
