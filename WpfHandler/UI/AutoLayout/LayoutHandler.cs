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
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfHandler.UI.AutoLayout;
using WpfHandler.UI.AutoLayout.Markups;

namespace WpfHandler.UI.AutoLayout
{
    /// <summary>
    /// Provides core methods for layout controls.
    /// </summary>
    public static class LayoutHandler
    {

        /// <summary>
        /// Contains declared bindings from types to default layout controls.
        /// </summary>
        /// <remarks>
        /// Key - The source <see cref="Type"/>
        /// Value - The binded <see cref="Type"/> of the <see cref="IGUIField"/> suitable for displaing a source data.
        /// </remarks>
        private static readonly Hashtable DefaultControlsBindings = new Hashtable();
               
        /// <summary>
        /// Contains declared bindings from types to default layout controls comaptible with a enum members.
        /// </summary>
        /// <remarks>
        /// Key - The source <see cref="Type"/>
        /// Value - The binded <see cref="Type"/> of the <see cref="IGUIField"/> suitable for displaing a source data.
        /// </remarks>
        private static readonly Hashtable EnumControlsBindings = new Hashtable();

        /// <summary>
        /// Contains declared bindings from types to default layout controls comaptible with an IList types.
        /// </summary>
        /// <remarks>
        /// Key - The source <see cref="Type"/>
        /// Value - The binded <see cref="Type"/> of the <see cref="IGUIField"/> suitable for displaing a source data.
        /// </remarks>
        private static readonly Hashtable ListControlsBindings = new Hashtable();

        /// <summary>
        /// Table that contains all registread value update callbacks.
        /// 
        /// Key = Type of the binded <see cref="IGUIField"/>
        /// </summary>
        private static readonly Hashtable RegistredCallbacks = new Hashtable();

        /// <summary>
        /// Scaning asseblies to find descriptions of controls.
        /// </summary>
        static LayoutHandler()
        {
            RescanAssemblies();
        }

        /// <summary>
        /// Adding child to horizontal grid.
        /// </summary>
        /// <param name="parent">Grid that will contain child.</param>
        /// <param name="element">Element that will be added to the grid as child.</param>
        public static void HorizontalLayoutAddChild(IAddChild parent, FrameworkElement element)
        {
            // Drop ivalid elelment.
            if(!(parent is Grid grid))
            {
                throw new InvalidCastException("Parent must be `" + typeof(Grid).FullName + "`.");
            }

            // Add new column fo element.
            grid.ColumnDefinitions.Add(new ColumnDefinition()
            {
                // define required width.
                // Auto - if width of element less or equals 0, or is NaN.
                // Shared element's width in case if defined.
                Width = double.IsNaN(element.Width) || element.Width <= 0 ? 
                        new GridLength(1, GridUnitType.Star) : new GridLength(element.Width)
            });

            // Add element as child.
            parent.AddChild(element);

            // Set als column as target for element.
            Grid.SetColumn(element, grid.ColumnDefinitions.Count - 1);
        }

        /// <summary>
        /// Adding child to the bertical layout group.
        /// </summary>
        /// <param name="parent">VerticalStackPanel that will contin child.</param>
        /// <param name="element">Element that will be added to the panel as child.</param>
        public static void VerticalLayoutAddChild(IAddChild parent, FrameworkElement element)
        {
            // Validate type cast.
            if (!(parent is VirtualizingStackPanel panel))
            {
                throw new InvalidCastException("Parent mast be `" + typeof(VirtualizingStackPanel).FullName + "`.");
            }

            // Set element to the parent panel.
            panel.Children.Add(element);

            //// Drop ivalid elelment.
            //if (!(parent is Grid grid))
            //{
            //    throw new InvalidCastException("Parent must be `" + typeof(Grid).FullName + "`.");
            //}

            //// Add new column fo element.
            //grid.RowDefinitions.Add(new RowDefinition()
            //{
            //    // define required width.
            //    // Auto - if width of element less or equals 0, or is NaN.
            //    // Shared element's width in case if defined.
            //    Height = double.IsNaN(element.Height) || element.Height <= 0 ?
            //            new GridLength(1, GridUnitType.Star) : new GridLength(element.Height)
            //});

            //// Add element as child.
            //parent.AddChild(element);

            //// Set als column as target for element.
            //Grid.SetRow(element, grid.RowDefinitions.Count - 1);
        }


        /// <summary>
        /// Registrating bool property into auto layout ui.
        /// </summary>
        /// <param name="control">Instiniated layout control.</param>
        /// <param name="descriptor">Descriptor that hold fields or properties.</param>
        /// <param name="member">Member in descriptor instance that will be used as target for value update.</param>
        /// <param name="defautltValue">Value that will be setted by default.</param>
        public static void RegistrateField(
            this IGUIField control, UIDescriptor descriptor,
            MemberInfo member, object defautltValue)
        {
            #region Declaretion & Initializtion
            // Preventing the null field.
            if(defautltValue == null)
            {
                try
                {
                    var type = UIDescriptor.MembersHandler.GetSpecifiedMemberType(member);

                    if (type.GetConstructor(new Type[0]) != null)
                    {
                        // Instiniating the default value.
                        defautltValue = Activator.CreateInstance(type);

                        UIDescriptor.MembersHandler.SetValue(member, descriptor, defautltValue);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(string.Format("Custructor crash!" +
                        "\nMember: {0}\n" +
                        "Descriptor: {1}\n\n" +
                        "Details: {2}",
                    member.Name,
                    descriptor.GetType().FullName,
                    ex.Message));
                };
            }

            // Apply default value.
            control.Value = defautltValue;

            // Declaring registration params.
            Action<IGUIField, object[]> handler = null;
            PropertyInfo propMember = null;
            FieldInfo fieldMember = null;
            #endregion

            #region Handlers
            // Instiniate UI field update callback for property members.
            void PropChangeCallback(IGUIField _, object[] __)
            {
                // Try to set value.
                try { propMember.SetValue(descriptor, control.Value); }
                catch (NotSupportedException) { }
                catch (Exception ex)
                { MessageBox.Show("Backward member binding corupted.\n\nDetails:\n" + ex.Message); }
            }
            // Instiniate UI field update callback for fields members.
            void FieldChangeCallback(IGUIField _, object[] __)
            {
                // Try to set value.
                try { fieldMember.SetValue(descriptor, control.Value); }
                catch (NotSupportedException) { }
                catch (Exception ex)
                { MessageBox.Show("Backward member binding corupted.\n\nDetails:\n" + ex.Message); }
            }
            #endregion

            #region Configuration
            // Configurating registration params and handlers.
            if (member is PropertyInfo prop)
            {
                propMember = prop;
                handler = PropChangeCallback;
            }
            else if (member is FieldInfo field)
            {
                fieldMember = field;
                handler = FieldChangeCallback;
            }
            #endregion

            #region Registration
            // To to registrate control into handler.
            try { RegistredCallbacks.Add(control, handler); }
            catch { throw new NotSupportedException("Instance of the ILayoutControl could be registred only once."); }

            // Subscribe on value change.
            control.ValueChanged += handler;
            #endregion
        }

        /// <summary>
        /// Unbind layout control from auto layout handler.
        /// </summary>
        /// <param name="control">Target layout control.</param>
        public static void UnregistrateField(this IGUIField control)
        {
            try
            {
                // Unregistreting of registred callback.
                control.ValueChanged -= (Action<IGUIField, object[]>)RegistredCallbacks[control];
            }
            catch
            {
                // Log error.
                //MessageBox.Show("You trying to unregistred layout control " +
                //    "that was not registred into the auto layout handler.\n" +
                //    "Use `LayoutHandler.RegistrateField` before.");
            }
        }

        /// <summary>
        /// Rescaning solution for Layout controls bindings.
        /// </summary>
        public static void RescanAssemblies()
        {
            // Clearing current meta.
            DefaultControlsBindings.Clear();
            EnumControlsBindings.Clear();
            ListControlsBindings.Clear();

            // Load query's processors.
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly assembly in assemblies)
            {
                #region Operation with types
                // Get all types for assembly.
                foreach (Type type in assembly.GetTypes())
                {
                    #region Validate
                    // Skip if it's absctract. 
                    if (type.IsAbstract) continue;
                    // Skip if it's interface.
                    if (type.IsInterface) continue;
                    // Skip if not implement <see cref="IGUIField"/>.
                    if (type.GetInterface(typeof(IGUIField).FullName) == null) continue;
                    #endregion
                    
                    #region Define target binding table
                    // Table that will contain link to the type.
                    Hashtable targetTable = null;

                    // Looking for complex elements.
                    // Is element defied to the displaing enums.
                    var isEnum = type.GetCustomAttribute<EnumsCompatibleAttribute>();
                    if (isEnum != null) targetTable = EnumControlsBindings;
                    else
                    {
                        // Is focused on work with collections.
                        var isList = type.GetCustomAttribute<ListCompatibleAttribute>();
                        if (isList != null) targetTable = ListControlsBindings;
                        else targetTable = DefaultControlsBindings; // Not specified.
                    }
                    #endregion

                    #region Performing all existing descriptors.
                    // Trying to find descriptor to types binding.
                    var bindingDescriptors = type.GetCustomAttributes<TypesCompatibleAttribute>();
                    
                    // Perform binding for all descriptors.
                    foreach (var desc in bindingDescriptors)
                    {
                        // Applying all shared bindings.
                        foreach (var bindedType in desc.CompatibleWith)
                        {
                            // Bind type to control.
                            BindLayoutControlToType(targetTable, type, bindedType);
                        }
                    }
                    #endregion
                }
                #endregion
            }
        }

        /// <summary>
        /// Binds <see cref="IGUIField"/> to the source type to using into auto generate ui panels based on <see cref="UIDescriptor"/> content.
        /// </summary>
        /// <param name="table">Target table for binding the element.</param>
        /// <param name="controlType">Type with implemented <see cref="IGUIField"/> interface.</param>
        /// <param name="sourceType">Type that will cause spawning of binded <see cref="IGUIField"/> during building of auto-generated UIs.</param>
        public static void BindLayoutControlToType(Hashtable table, Type controlType, Type sourceType)
        {
            if (table[sourceType] is IGUIField)
            {
                // If control already defined for that type then override it.
                table[sourceType] = controlType;
            }
            else
            {
                // Registrate new binding if not registred yet.
                table.Add(sourceType, controlType);
            }
        }

        /// <summary>
        /// Looking got control binded to the source type.
        /// </summary>
        /// <param name="sourceType">Type of the member that will be applied to the UI control.</param>
        /// <param name="isInherited">Should it look into entire inheritance hierarchy of the source type till not found the binded one?</param>
        /// <returns>Type of found control. Null if not found.</returns>
        public static Type GetBindedControl(Type sourceType, bool isInherited)
        {
            #region Enums
            // Is source type is enum?
            if ( (sourceType.IsEnum || sourceType.Equals(typeof(Enum)) ) &&
                // Check if requested type has binding into enum compatibe table.
                EnumControlsBindings[sourceType] is Type enumControl)
                {
                    // Return binded control if found.
                    return enumControl;
                }
            #endregion

            #region Collections
            // If has implemented IEnumerable type then will auto applied to collections GUI elements.
            if (sourceType.GetInterface(typeof(IEnumerable).FullName) != null)
            {
                // Getting the generic type.
                Type genericType = null;

                if(sourceType.IsSubclassOf(typeof(Array)))
                {
                    // operate like array.
                    genericType = sourceType.GetElementType();
                }
                else
                {
                    // If has generic types.
                    if (sourceType.GenericTypeArguments.Length > 0)
                    {
                        // Operate like generic.
                        genericType = sourceType.GenericTypeArguments[0];
                    }
                }

                if (genericType != null)
                {

                    // Lookin for compatible UI element by the entire hierarchy.
                    do
                    {
                        // Check if requested type has binding into enumerable collections compatibe table.
                        if (ListControlsBindings[genericType] is Type collectionControl)
                        {
                            // Return binded control if found.
                            return collectionControl;
                        }

                        // Moving deeper.
                        genericType = genericType.BaseType;
                    }
                    while (genericType != null);
                }
            }
            #endregion

            #region Direct types binding
            // Check if requested type has binding by direct binding.
            if (DefaultControlsBindings[sourceType] is Type baseControl)
            {
                // Return binded control if found.
                return baseControl;
            }
            #endregion

            #region Deep search
            // The source type has no a binding.
            // Looking deeply into the assembly hierarchy.
            if (isInherited && // Is looking in depth requested.
                sourceType.BaseType != null) // is still not on the ground level.
            {
                return GetBindedControl(sourceType.BaseType, true);
            }
            else
            {
                // Grouon of hierarchy reached and compatible type not found.
                return null;
            }
            #endregion
        }

        /// <summary>
        /// Checks does an element visible for user in the given  container.
        /// </summary>
        /// <param name="element">Element for check.</param>
        /// <param name="container">Container that defines visibility area bounds.</param>
        /// <returns>Result of check.</returns>
        public static bool IsUserVisible(FrameworkElement element, FrameworkElement container)
        {
            if (!element.IsVisible)
                return false;

            Rect bounds = element.TransformToAncestor(container).TransformBounds(new Rect(0.0, 0.0, element.RenderSize.Width, element.RenderSize.Height));
            Rect rect = new Rect(-100.0, -100.0, container.RenderSize.Width + 100, container.RenderSize.Height + 100);
            return rect.Contains(bounds.TopLeft) || rect.Contains(bounds.BottomRight);
        }
    }
}
