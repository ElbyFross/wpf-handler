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
using System.Windows.Markup;
using System.Reflection;
using System.Windows.Controls;
using WpfHandler.UI.AutoLayout.Configuration;
using WpfHandler.UI.Virtualization;

namespace WpfHandler.UI.AutoLayout
{
    /// <summary>
    /// Part of UIDescriptor class that defines members participating virtualization system.
    /// </summary>
    public abstract partial class UIDescriptor : IVirtualizedCollection
    {
        /// <summary>
        /// Defines is virtalization enable or not.
        /// </summary>
        [HideInInspector]
        public bool IsVirtualized { get; set; } = true;

        /// <summary>
        /// How many members will be virtualized during one tic before validation.
        /// </summary>
        [HideInInspector]
        public int VirtualizedItemsPack { get; set; } = 10;

        /// <summary>
        /// List with virtualized items.
        /// </summary>
        [HideInInspector]
        public List<FrameworkElement> VirtualizedElements { get; } = new List<FrameworkElement>();

        /// <summary>
        /// Is collection must uncload and descroy controls out of the view bounds?
        /// </summary>
        [HideInInspector]
        public virtual bool UnloadHidded { get; set; } = false;

        /// <summary>
        /// Count a current item in the pack.
        /// </summary>
        private int virtualizedPackCounter;

        /// <summary>
        /// Last element instantiated during virtualization.
        /// </summary>
        private FrameworkElement lastVirtualizedElement;

        /// <summary>
        /// Insiniate UI by descriptor's attributes map and add it as child to parent element.
        /// </summary>
        /// <param name="root">UI element that would contain instiniated UI elemets.</param>
        public async Task BindToAsync(Panel root)
        {
            // Instiniate first UILayer.
            var layer = new LayoutLayer()
            {
                root = root // Thet binding target as root for cuurent layer.
            };

            await BindToAsync(layer);
        }
        
        /// <summary>
        /// Binding UIDescriptor content to the layer's root.
        /// </summary>
        /// <param name="layer">Target layer.</param>
        public async Task BindToAsync(LayoutLayer layer)
        {
            // Storing layer.
            RootLayer = layer;
            activeLayer = layer;

            #region Getting descripting data
            // Getting relevant type.
            var selfType = this.GetType();

            // Getting options applied to every memeber.
            var globalOptions = Attribute.GetCustomAttributes(selfType, typeof(Attribute)).Where
                (f => f.GetType().GetInterface(typeof(IGUILayoutOption).FullName) != null);

            // Get all memebers.
            var members = selfType.GetMembers();

            // Sorting by order.
            var orderedMembers = members.Where(f => f.GetCustomAttribute<OrderAttribute>() != null).
                OrderBy(f => f.GetCustomAttribute<OrderAttribute>().Order);

            // Sorting disordered members by metadata.
            var disorderedMembers = members.Where(f => f.GetCustomAttribute<OrderAttribute>() == null).
                OrderBy(f => f.MetadataToken);
            #endregion

            #region Definig handler
            // The handler that will apply options to the element.
            void ApplyOptionsHandler(
                FrameworkElement element,
                IEnumerable<Attribute> localAttributes)
            {
                if (element == null) return;

                // Applying global options
                foreach (IGUILayoutOption option in globalOptions)
                {
                    option.ApplyLayoutOption(element);
                }

                // Perform options attributes.
                foreach (Attribute attr in localAttributes)
                {
                    // Skip if not an option.
                    if (!(attr is IGUILayoutOption option)) continue;

                    // Applying option to the element.
                    option.ApplyLayoutOption(element);
                }

                // Applying the shared options.
                foreach (ISharableGUILayoutOption option in SharedLayoutOptions)
                {
                    // Applying option to the element.
                    option.ApplyLayoutOption(element);
                }
            }
            #endregion

            // Sort in declaretion order.
            members = orderedMembers.Concat(disorderedMembers).ToArray();

            // Perform all descriptor map.
            foreach (MemberInfo member in members)
            {               
                #region Validation
                // Skip if the member is not field or property.
                if (!MembersHandler.GetSpecifiedMemberInfo(
                    member, out PropertyInfo prop, out FieldInfo field))
                    continue;

                // Skip if member excluded from instpector.
                if (member.GetCustomAttribute<HideInInspectorAttribute>() != null)
                    continue;
                #endregion

                #region Virtualization
                // Suspending virtualization
                if (IsVirtualized &&
                   virtualizedPackCounter == VirtualizedItemsPack)
                {
                    // Droping the counter.
                    virtualizedPackCounter = 0;

                    // Waiting till loading.
                    while (!lastVirtualizedElement.IsLoaded)
                    {
                        await Task.Delay(5);
                    }

                    Panel rootPanel = (Panel)RootLayer.root;

                    // Checking if the last element still in the visible bounds.
                    bool isVisible = LayoutHandler.IsUserVisible(lastVirtualizedElement, Window.GetWindow(lastVirtualizedElement));

                    // Suspending if the last not visible till view update.
                    if (!isVisible)
                    {
                        // Marker that using for blocking the thread.
                        bool unlocked = false;

                        // Waiting till the root will change a size/
                        rootPanel.SizeChanged += VirtValHandler;
                        void VirtValHandler(object sender, SizeChangedEventArgs e)
                        {
                            // Checking if the last element is already visible.
                            isVisible = LayoutHandler.IsUserVisible(lastVirtualizedElement, Window.GetWindow(lastVirtualizedElement));
                            if(isVisible)
                            {
                                // Unsubscribing from event.
                                rootPanel.SizeChanged -= VirtValHandler;

                                // Unblocking the thread.
                                unlocked = true;
                            }
                        }

                        // Unblocking instantiation of next group of elements.
                        while(!unlocked)
                        {
                            await Task.Delay(5);
                        }
                    }
                }
                #endregion

                // Getting all attributes.
                IEnumerable<Attribute> attributes = member.GetCustomAttributes<Attribute>(true);

                // Allocating and defining types.
                var memberType = MembersHandler.GetSpecifiedMemberType(member);
                Type controlType = null;

                #region Perform general layout attributes
                // Perform general attributes.
                foreach (Attribute attr in attributes)
                {
                    // Skip if an option.
                    if (attr is IGUILayoutOption) continue;

                    // Apply layout control to GUI.
                    if (attr is IGUIElement attrControl)
                    {
                        attrControl.OnLayout(ref activeLayer, this, member);
                    }
                }
                #endregion


                #region Defining UI field type
                // Check if default control was overrided by custom one.
                var customControlDesc = member.GetCustomAttribute<CustomControlAttribute>();
                if (customControlDesc != null && // Is overriding requested?
                    customControlDesc.ControlType != null) // Is target type is not null
                {
                    // Set redefined control like target to instinitation.
                    controlType = customControlDesc.ControlType;
                }
                else
                {
                    // Looking for the certain control only for derect defined descriptors.
                    if (memberType.IsSubclassOf(typeof(UIDescriptor)))
                    {
                        // Set binded type like target to instiniation.
                        controlType = LayoutHandler.GetBindedControl(memberType, false);
                    }
                    else
                    {
                        // Set binded type like target to instiniation.
                        controlType = LayoutHandler.GetBindedControl(memberType, true);
                    }
                }
                #endregion

                // Is control defined to that member?
                if (controlType != null)
                {
                    // Instiniating target control by the type.
                    var control = (IGUIField)Activator.CreateInstance(controlType);

                    #region Set prefix label
                    // Is spawned elelment has a label.
                    if (control is UI.Controls.ILabel label)
                    {
                        // Instiniating handle that will provide managmend of the control.
                        ContentAttribute localizationHandler = null;

                        // Try to get described one.
                        if (UniformDataOperator.AssembliesManagement.MembersHandler.
                            TryToGetAttribute(member, out ContentAttribute attribute))
                        {
                            // Buferize if found.
                            localizationHandler = attribute;
                        }
                        else
                        {
                            // Initialize new one.
                            localizationHandler = ContentAttribute.Empty;
                        }

                        // Binding spawned element to the conent.
                        localizationHandler.BindToLabel(label, member);
                    }
                    #endregion

                    #region Perform Layout options
                    // Check if spawned control is framework element.
                    if (control is FrameworkElement fEl)
                    {
                        // Settup into virtualization system.
                        lastVirtualizedElement = fEl;
                        VirtualizedElements.Add(fEl);

                        // Applying options to the element.
                        ApplyOptionsHandler(fEl, attributes);
                    }
                    #endregion

                    #region Binding to a layout
                    // Sign up this control on desctiptor events.
                    TryToBindControl(control, this, member);

                    // Initialize control.
                    control.OnLayout(ref activeLayer, this, member, globalOptions, attributes);

                    // Adding field to the registration table.
                    RegistredFields.Add(member, control);
                    #endregion
                }
                else
                {
                    // Check if that just other descriptor.
                    if (memberType.IsSubclassOf(typeof(UIDescriptor)))
                    {
                        //#region Configurating layout
                        //// Add horizontal shift for sub descriptor.
                        //new BeginHorizontalGroupAttribute().OnLayout(ref activeLayer);
                        //new Controls.SpaceAttribute().OnLayout(ref activeLayer);

                        // Add vertical group.
                        var vertGroup = new BeginVerticalGroupAttribute();
                        vertGroup.OnLayout(ref activeLayer, this, member);
                        //#endregion

                        #region Applying options to the new root
                        // Applying options to the element.
                        ApplyOptionsHandler(vertGroup.Layer.root as FrameworkElement, attributes);
                        #endregion

                        #region Looking for descriptor object.
                        // Bufer that will contain value of the descriptor.
                        UIDescriptor subDesc = null;

                        // Trying to get value via reflection.
                        subDesc = prop != null ?
                            prop.GetValue(this) as UIDescriptor : // Operate like property.
                            field.GetValue(this) as UIDescriptor; // Operate like fields.

                        // Instiniate default in case if value is null.
                        if (subDesc == null)
                        {
                            try
                            {
                                // Insiniate empty constructor.
                                subDesc = Activator.CreateInstance(memberType) as UIDescriptor;
                            }
                            catch (Exception ex)
                            {
                                // Log error.
                                MessageBox.Show("UIDescriptor must contain empty constructor, " +
                                    "or be instiniated before calling into UI." +
                                    "\n\nDetails:\n" + ex.Message);

                                // Skip to the next member.
                                continue;
                            }

                            // Updating stored value for current member.
                            if (prop != null) prop.SetValue(this, subDesc);
                            else field.SetValue(this, subDesc);
                        }

                        // Defining the sharable options.
                        var sharableOption = InsertSharableOptions(SharedLayoutOptions, attributes, true);
                        sharableOption = InsertSharableOptions(sharableOption, globalOptions, false);
                        subDesc.SharedLayoutOptions = sharableOption.ToArray();
                        #endregion

                        // Binding descriptor to the UI.
                        var panel = (Panel)activeLayer.root;

                        // Settup into virtualization system.
                        lastVirtualizedElement = panel;
                        VirtualizedElements.Add(panel);

                        // Binding a sub descriptor to the panel.
                        _ = subDesc.BindToAsync(panel);

                        // End descriptor layer.
                        new EndGroupAttribute().OnLayout(ref activeLayer);

                    }
                }

                virtualizedPackCounter++;
            }

            // Marking as loaded.
            IsLoaded = true;

            // Calling the local handler.
            OnLoaded();

            // Inform subscribers.
            Loaded?.Invoke(this);
        }

    }
}
