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
        public int VirtualizedItemsPack { get; set; } = 3;

        /// <summary>
        /// List with virtualized items.
        /// </summary>
        [HideInInspector]
        public List<VirtualizedItemMeta> VirtualizedElements { get; } = new List<VirtualizedItemMeta>();

        /// <summary>
        /// TODO: ATTENTION: Not supported
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

            // Sort in declaretion order.
            members = orderedMembers.Concat(disorderedMembers).ToArray();

            // Perform all descriptor map.
            foreach (MemberInfo member in members)
            {
                var memberMeta = new MembersHandler.MemberMeta(member);

                #region Validation
                // Skip if the member is not field or property.
                if (!memberMeta.IsValue) continue;

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
                            if (isVisible)
                            {
                                // Unsubscribing from event.
                                rootPanel.SizeChanged -= VirtValHandler;

                                // Unblocking the thread.
                                unlocked = true;
                            }
                        }

                        // Unblocking instantiation of next group of elements.
                        while (!unlocked)
                        {
                            await Task.Delay(5);
                        }
                    }
                }
                #endregion

                var virtualizedElement = RegistredFields[member] as IGUIField;

                if (virtualizedElement == null)
                {
                    // Instantiating a member.
                    var field = InstantiateMember(ref activeLayer, memberMeta, globalOptions);

                    // Skip in case if not instantiated.
                    if (field == null) continue;

                    // Storing in virtualization meta.
                    var meta = new VirtualizedItemMeta(
                        field,
                        ref activeLayer,
                        member);

                    // Settup into virtualization system.
                    lastVirtualizedElement = meta.Element ?? lastVirtualizedElement;
                    //VirtualizedElements.Add(meta);

                    // Applying to the layout.
                    activeLayer?.ApplyControl(field as FrameworkElement);
                }
                else
                {
                    // Getting all attributes.
                    IEnumerable<Attribute> attributes = memberMeta.Member.GetCustomAttributes<Attribute>(true);

                    virtualizedElement.OnLayout(ref layer, this, memberMeta.Member, globalOptions, attributes);

                    // Applying to the layout.
                    layer?.ApplyControl(virtualizedElement as FrameworkElement);
                }

                // Incrementing of virtualized pack elements counter.
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