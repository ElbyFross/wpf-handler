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

namespace WpfHandler.UI.AutoLayout
{
    /// <summary>
    /// Class that provides adopting of members by AutoLayout UI.
    /// </summary>
    public abstract partial class UIDescriptor
    {
        /// <summary>
        /// Is that descriptor loaded?
        /// </summary>
        [HideInInspector]
        public bool IsLoaded { get; private set; }

        /// <summary>
        /// Options applyed during initialization.
        /// </summary>
        [HideInInspector]
        public ISharableGUILayoutOption[] SharedLayoutOptions { get; set; } =
            new ISharableGUILayoutOption[0];

        /// <summary>
        /// A root layer for that descriptor.
        /// </summary>
        [HideInInspector]
        public LayoutLayer RootLayer { get; private set; }

        /// <summary>
        /// Occurs when come of UI elemets binded to the descriptor was updated.
        /// </summary>
        public event Action<UIDescriptor, IGUIField, object[]> ValueChanged;

        /// <summary>
        /// Will occurs when the descripor get loaded state.
        /// </summary>
        public event Action<UIDescriptor> Loaded;

        /// <summary>
        /// Current active UI layer.
        /// </summary>
        [HideInInspector]
        LayoutLayer activeLayer;

        /// <summary>
        /// The table that contais instiniated elements.
        /// Key - <see cref="MemberInfo"/>.
        /// Value - <see cref="IGUIField"/>.
        /// </summary>
        [HideInInspector]
        Hashtable RegistredFields { get; set; } = new Hashtable();

        /// <summary>
        /// Insiniate UI by descriptor's attributes map and add it as child to parent element.
        /// </summary>
        /// <param name="root">UI element that would contain instiniated UI elemets.</param>
        public void BindTo(Panel root)
        {
            // Instiniate first UILayer.
            var layer = new LayoutLayer()
            {
                root = root // Thet binding target as root for cuurent layer.
            };

            BindTo(layer);
        }

        /// <summary>
        /// Insiniate UI by descriptor's attributes map and add it as child to parent element.
        /// </summary>
        /// <param name="layer">UI layer that will used as a root for the descriptor's elements.</param>
        public void BindTo(LayoutLayer layer)
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

                // Instantiating a field from member.
                var field = InstantiateMember(ref activeLayer, memberMeta, globalOptions);

                // Skip in case if not instantiated.
                if (field == null) continue;

                // Applying to the layout.
                activeLayer?.ApplyControl(field as FrameworkElement);
            }

            // Marking as loaded.
            IsLoaded = true;

            // Calling the local handler.
            OnLoaded();

            // Inform subscribers.
            Loaded?.Invoke(this);
        }

        /// <summary>
        /// Unbinding descriptor from panel.
        /// Will affect only memeber defied into descriptor and leave and other GUI elements childed to the panel.
        /// </summary>
        /// <param name="root">Root panle that was binding target for descriptor.</param>
        public void UnbindFrom(Panel root)
        {
            // Get all memebers.
            var members = GetType().GetMembers();

            // Checking every child into the root.
            for (int i = 0; i < root.Children.Count; i++)
            {
                // Getting current child.
                var child = root.Children[i];

                // If child is layout control and has a binded memeber.
                if (child is IGUIField control &&
                    control.BindedMember != null)
                {
                    #region Validation
                    // Checking is the binded memeber is a part of the descriptor.
                    bool thisDesc = false;
                    foreach (var member in members)
                    {
                        // Check if binded member is the one from the descriptor.
                        if (member.Equals(control.BindedMember))
                        {
                            thisDesc = true;
                            break;
                        }
                    }

                    // Skip if not possesed to that descriptor.
                    if (!thisDesc) continue;
                    #endregion

                    #region Unbind control from UI
                    // Unsubscribe element from hadler's events.
                    LayoutHandler.UnregistrateField(control);

                    // Remove from UI.
                    root.Children.RemoveAt(i);
                    i--;
                    #endregion

                    #region Sub descriptor processing
                    // If member is UI descriptor.
                    if (control is Panel subPanel &&
                        MembersHandler.GetSpecifiedMemberType(control.BindedMember).IsSubclassOf(typeof(UIDescriptor)))
                    {
                        try
                        {
                            // Requiest descriptor unbinding.
                            ((UIDescriptor)control.Value).UnbindFrom(subPanel);
                        }
                        catch (Exception ex)
                        {
                            // Log error.
                            MessageBox.Show("Subpanel UIDescriptor unbind operation failed." +
                                "\n\nDetails:\n" + ex.Message);
                        }
                    }
                    #endregion
                }
            }
        }

        /// <summary>
        /// Safely init layout control element and sighn up on internal hadler events.
        /// </summary>
        /// <param name="control">The GUI control that will be instiniated.</param>
        /// <param name="member">The member that will be binded to the GUI.</param>
        /// <param name="value">Value that will applied as default.</param>
        public void ControlSignUp(IGUIField control, MemberInfo member, object value)
        {
            try
            {
                // Registrate member in auto layout handler.
                control.RegistrateField(this, member, value);

                // Subscribe the global event handler.
                control.ValueChanged += OnValueChangedCallback;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                "Control sign up failed.\n\n" +
                "Member: " + member.Name + "\n" +
                "Details: " + ex.Message);
            }
        }

        /// <summary>
        /// Return an UI field binded to the member.
        /// </summary>
        /// <param name="memberName">The name of the member from the source descriptor.</param>
        /// <returns>A GUI Field binded to the member.</returns>
        public IGUIField GetFieldByMember(string memberName)
        {
            // Looking for the member.
            MemberInfo member = GetType().GetField(memberName); // via felds.
            if (member == null) member = GetType().GetProperty(memberName); // via properties.

            return GetFieldByMember(member);
        }

        /// <summary>
        /// Return an UI field binded to the member.
        /// </summary>
        /// <param name="member">The member from the source descriptor.</param>
        /// <returns>A GUI Field binded to the member.</returns>
        public IGUIField GetFieldByMember(MemberInfo member)
        {
            if (IsVirtualized)
            {
                if (RegistredFields[member] is IGUIField field)
                {
                    return field;
                }
                else
                {
                    // Creating virtual layer.
                    var virtualLayer = new LayoutLayer()
                    { root = new VirtualizingStackPanel() };

                    // Getting options applied to the descriptor.
                    var globalOptions = Attribute.GetCustomAttributes(GetType(), typeof(Attribute)).Where
                        (f => f.GetType().GetInterface(typeof(IGUILayoutOption).FullName) != null);

                    // Virtualizing field.
                    var element = InstantiateMember(ref virtualLayer, member, globalOptions);

                    // Generating metadata.
                    var meta = new Virtualization.VirtualizedItemMeta(element, ref virtualLayer, member);

                    // Registring via the virtualized elements.
                    VirtualizedElements.Add(meta);

                    return meta.Field;
                }
            }
            else
            {
                return RegistredFields[member] as IGUIField;
            }
        }

        /// <summary>
        /// Trying to bind control to the auto layout handler.
        /// </summary>
        /// <param name="control">Control that would be binded.</param>
        /// <param name="args">Must contains <see cref="UIDescriptor"/> 
        /// and <see cref="MemberInfo"/> for success performing.</param>
        /// <returns>Is control was binded?</returns>
        public static bool TryToBindControl(IGUIField control, params object[] args)
        {
            try
            {
                // Trying to bind.
                ToBindControl(control, args);

                // Success if esception not occured
                return true;
            }
            catch
            {
                // Inform about binding fail.
                return false;
            }
        }

        /// <summary>
        /// Bind control to the auto layout handler.
        /// </summary>
        /// <param name="control">Control that would be binded.</param>
        /// <param name="args">Must contains <see cref="UIDescriptor"/> and <see cref="MemberInfo"/>.</param>
        public static void ToBindControl(IGUIField control, params object[] args)
        {
            // Find required referendes.
            UIDescriptor desc = null;
            MemberInfo member = null;

            // Trying to get shared properties.
            foreach (object obj in args)
            {
                if (obj is UIDescriptor) desc = (UIDescriptor)obj;
                if (obj is MemberInfo) member = (MemberInfo)obj;
            }

            // Request binding.
            ToBindControl(control, desc, member);
        }

        /// <summary>
        /// Bind control to the auto layout handler.
        /// </summary>
        /// <param name="control">Control that would be binded.</param>
        /// <param name="descriptor">Source descriptor.</param>
        /// <param name="member">Member from the descriptor.</param>
        public static void ToBindControl(IGUIField control, UIDescriptor descriptor, MemberInfo member)
        {
            // Drop control sign up in case if member not shared.
            if (descriptor == null) throw new NullReferenceException("Instance of the UIDescriptor not shared with @args");
            if (member == null) throw new NullReferenceException("Instance of the MemberInfo not shared with @args");

            // Detecting default value seted up into descriptor.
            object defaultValue;
            // Getting from property member.
            if (member is PropertyInfo pi) defaultValue = pi.GetValue(descriptor);
            // Getting from field memeber.
            else if (member is FieldInfo fi) defaultValue = fi.GetValue(descriptor);
            // Member cast is invalid and not supported intor that operation.
            else throw new InvalidCastException("@member must inherit PropertyInfo of FieldInfo");

            // Sign up this control on desctiptor events.
            descriptor.ControlSignUp(control, member, defaultValue);
        }

        /// <summary>
        /// Handler that will be called when all the element will be loaded and be ready to use.
        /// </summary>
        public virtual void OnLoaded()
        {

        }

        /// <summary>
        /// Handels the IGuiField.ValueChanged event and forward to the global one.
        /// </summary>
        /// <param name="sender">Fieled initiated the event.</param>
        /// <param name="args">Shared arguments.</param>
        protected virtual void OnValueChangedCallback(IGUIField sender, object[] args)
        {
            // Inform subscribers.
            ValueChanged?.Invoke(this, sender, args);
        }

        /// <summary>
        /// Handels the UIDescriptor.ValueChanged event and forward to the ipper one.
        /// </summary>
        /// <param name="arg1">Descriptor initiated the event initiated the event.</param>
        /// <param name="arg2">Fieled initiated the event.</param>
        /// <param name="args">Shared arguments</param>
        protected void OnValueChangedCallback(UIDescriptor arg1, IGUIField arg2, object[] args)
        {
            // Inform subscribers.
            ValueChanged?.Invoke(arg1, arg2, args);
        }
    }
}
