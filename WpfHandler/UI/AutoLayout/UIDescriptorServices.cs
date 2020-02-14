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
    /// Part of class defines service features of desscriptor.
    /// </summary>
    public partial class UIDescriptor
    {
        /// <summary>
        /// Inserts ISharableGUILayoutOption instances that still not included to the top options. 
        /// </summary>
        /// <param name="topOptions">Priority options from the topper layer.</param>
        /// <param name="localOptions">Enumerable coolaction of objects that contains options for that layer.</param>
        /// <param name="instance">Is output coolection must be a new instance of the data will insterted into existed one.</param>
        /// <returns>Collection with sharable attributes suitable for sharing to the next deeper layer.</returns>
        public static IList<ISharableGUILayoutOption> InsertSharableOptions(
            IList<ISharableGUILayoutOption> topOptions,
            IEnumerable localOptions,
            bool instance)
        {
            // Copining content.
            IList<ISharableGUILayoutOption> resultCollection;
            List<ISharableGUILayoutOption> bufer = new List<ISharableGUILayoutOption>();

            // Checking local options.
            foreach (object attribute in localOptions)
            {
                if (!(attribute is IGUILayoutOption option)) continue;

                // Check is is Sharable option.
                if (option is ISharableGUILayoutOption sharableOption)
                {
                    bool conflicted = false;
                    var optionType = option.GetType();

                    // Chacking if the same type already included to the list.
                    foreach (ISharableGUILayoutOption topSO in topOptions)
                    {
                        if (topSO.GetType().Equals(optionType))
                        {
                            conflicted = true;
                            break;
                        }
                    }

                    // Add to sharable options list in case if not conflicted by the type.
                    if (!conflicted)
                    {
                        bufer.Add(sharableOption);
                    }
                }
            }


            // Instiniating new collection if requested.
            if (instance)
            {
                // Copying collection.
                resultCollection = new List<ISharableGUILayoutOption>(topOptions);
                // Concating bufer with the collection.
                resultCollection = bufer.Concat(resultCollection).ToList();
            }
            // Set base as reference.
            else
            {
                // Applying top collection as current.
                resultCollection = topOptions;

                // Inserting the data from bufer.
                for (int i = 0; i < bufer.Count; i++)
                {
                    resultCollection.Insert(i, bufer[i]);
                }
            }

            return resultCollection;
        }

        /// <summary>
        /// Instantiating member of descriptor.
        /// </summary>
        /// <param name="layer">Layer that will be shared to the elemen OnLayout handler.</param>
        /// <param name="member">Target member.</param>
        /// <param name="globalOptions">Options of the descriptor.</param>
        /// <returns></returns>
        private object InstantiateMember(ref LayoutLayer layer,
            MemberInfo member,
            IEnumerable<Attribute> globalOptions)
        {
            return InstantiateMember(ref layer, new MembersHandler.MemberMeta(member), globalOptions);
        }

        /// <summary>
        /// Instantiating member of descriptor.
        /// </summary>
        /// <param name="layer">Layer that will be shared to the elemen OnLayout handler.</param>
        /// <param name="memberMeta">Target member.</param>
        /// <param name="globalOptions">Options of the descriptor.</param>
        /// <returns></returns>
        private object InstantiateMember(ref LayoutLayer layer,
            MembersHandler.MemberMeta memberMeta,
            IEnumerable<Attribute> globalOptions)
        {
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
            
            // Getting all attributes.
            IEnumerable<Attribute> attributes = memberMeta.Member.GetCustomAttributes<Attribute>(true);

            #region Perform general layout attributes
            // Perform general attributes.
            foreach (Attribute attr in attributes)
            {
                // Skip if an option.
                if (attr is IGUILayoutOption) continue;

                // Apply layout control to GUI.
                if (attr is IGUIElement attrControl)
                {
                    attrControl.OnLayout(ref layer, this, memberMeta);
                }
            }
            #endregion


            #region Defining UI field type
            // Check if default control was overrided by custom one.
            var customControlDesc = memberMeta.Member.GetCustomAttribute<CustomControlAttribute>();
            Type controlType;
            if (customControlDesc != null && // Is overriding requested?
                customControlDesc.ControlType != null) // Is target type is not null
            {
                // Set redefined control like target to instinitation.
                controlType = customControlDesc.ControlType;
            }
            else
            {
                // Looking for the certain control only for derect defined descriptors.
                if (memberMeta.SourceType.IsSubclassOf(typeof(UIDescriptor)))
                {
                    // Set binded type like target to instiniation.
                    controlType = LayoutHandler.GetBindedControl(memberMeta.SourceType, false);
                }
                else
                {
                    // Set binded type like target to instiniation.
                    controlType = LayoutHandler.GetBindedControl(memberMeta.SourceType, true);
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
                    ContentAttribute localizationHandler;

                    // Try to get described one.
                    if (UniformDataOperator.AssembliesManagement.MembersHandler.
                        TryToGetAttribute(memberMeta.Member, out ContentAttribute attribute))
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
                    localizationHandler.BindToLabel(label, memberMeta.Member);
                }
                #endregion

                #region Perform Layout options
                // Check if spawned control is framework element.
                if (control is FrameworkElement fEl)
                {
                    // Applying options to the element.
                    ApplyOptionsHandler(fEl, attributes);
                }
                #endregion

                #region Binding to a layout
                // Sign up this control on desctiptor events.
                TryToBindControl(control, this, memberMeta.Member);

                // Adding instiniated element to the layout.
                //activeLayer?.ApplyControl(control as FrameworkElement);

                // Initialize control.
                control.OnLayout(ref layer, this, memberMeta.Member, globalOptions, attributes);

                // Adding field to the registration table.
                RegistredFields.Add(memberMeta.Member, control);
                #endregion

                return control;
            }
            else
            {
                // Check if that just other descriptor.
                if (memberMeta.SourceType.IsSubclassOf(typeof(UIDescriptor)))
                {
                    //#region Configurating layout
                    //// Add horizontal shift for sub descriptor.
                    //new BeginHorizontalGroupAttribute().OnLayout(ref activeLayer);
                    //new Controls.SpaceAttribute().OnLayout(ref activeLayer);

                    // Add vertical group.
                    var vertGroup = new BeginVerticalGroupAttribute();
                    vertGroup.OnLayout(ref layer, this, memberMeta);
                    //#endregion

                    #region Applying options to the new root
                    // Applying options to the element.
                    ApplyOptionsHandler(vertGroup.Layer.root as FrameworkElement, attributes);

                    #endregion

                    #region Looking for descriptor object.
                    // Bufer that will contain value of the descriptor.
                    // Trying to get value via reflection.
                    UIDescriptor subDesc = memberMeta.Property != null ?
                        memberMeta.Property.GetValue(this) as UIDescriptor : // Operate like property.
                        memberMeta.Field.GetValue(this) as UIDescriptor;

                    // Instiniate default in case if value is null.
                    if (subDesc == null)
                    {
                        try
                        {
                            // Insiniate empty constructor.
                            subDesc = Activator.CreateInstance(memberMeta.SourceType) as UIDescriptor;
                        }
                        catch (Exception ex)
                        {
                            // Log error.
                            MessageBox.Show("UIDescriptor must contain empty constructor, " +
                                "or be instiniated before calling into UI." +
                                "\n\nDetails:\n" + ex.Message);

                            // Skip to the next member.
                            return null;
                        }

                        // Updating stored value for current member.
                        if (memberMeta.Property != null) memberMeta.Property.SetValue(this, subDesc);
                        else memberMeta.Field.SetValue(this, subDesc);
                    }

                    // Defining the sharable options.
                    var sharableOption = InsertSharableOptions(SharedLayoutOptions, attributes, true);
                    sharableOption = InsertSharableOptions(sharableOption, globalOptions, false);
                    subDesc.SharedLayoutOptions = sharableOption.ToArray();
                    #endregion

                    // Binding descriptor to the UI.
                    Panel subPanel = (Panel)layer.root;

                    if (subDesc.IsVirtualized)
                        _ = subDesc.BindToAsync(subPanel);
                    else
                        subDesc.BindTo(subPanel);

                    // End descriptor layer.
                    new EndGroupAttribute().OnLayout(ref layer);

                    // Subscribing on the sub descriptor changed update.
                    subDesc.ValueChanged += OnValueChangedCallback;

                    return subPanel;
                }
            }

            return null;
        }

        /// <summary>
        /// Handling tasks with members suitable for UI descriptor's operations.
        /// </summary>
        public static class MembersHandler
        {
            /// <summary>
            /// Container that defines member's metadata.
            /// </summary>
            public class MemberMeta
            { 
                /// <summary>
                /// Metadata source member.
                /// </summary>
                public MemberInfo Member { get; private set; }

                /// <summary>
                /// Type of the member info source.
                /// </summary>
                public Type SourceType
                {
                    get
                    {
                        if (_sourceType == null)
                        {
                            _sourceType = GetSpecifiedMemberType(Member);
                        }
                        return _sourceType;
                    }
                }

                /// <summary>
                /// Is that member's source field or property.
                /// </summary>
                public bool IsValue
                {
                    get
                    {
                        if (!_infoLoaded) UpdateInfo();
                        return IsField || IsProperty;
                    }
                }

                /// <summary>
                /// Member as a field info
                /// </summary>
                public FieldInfo Field 
                { 
                    get
                    {
                        if (!_infoLoaded && _fieldInfo == null) UpdateInfo();
                        return _fieldInfo;
                    }
                }

                /// <summary>
                /// Is that member is field.
                /// </summary>
                public bool IsField
                {
                    get
                    {
                        if(!_infoLoaded) UpdateInfo();
                        return _fieldInfo != null;
                    }
                }

                /// <summary>
                /// Member as a property info.
                /// </summary>
                public PropertyInfo Property
                {
                    get
                    {
                        if (!_infoLoaded && _propInfo == null) UpdateInfo();
                        return _propInfo;
                    }
                }

                /// <summary>
                /// Is that member is a prooperty.
                /// </summary>
                public bool IsProperty
                {
                    get
                    {
                        if (!_infoLoaded) UpdateInfo();
                        return _propInfo != null;
                    }
                }

                private Type _sourceType;
                private FieldInfo _fieldInfo;
                private PropertyInfo _propInfo;
                private bool _infoLoaded = false;

                /// <summary>
                /// Configurates member core data.
                /// </summary>
                /// <param name="member">Target member.</param>
                public MemberMeta(MemberInfo member)
                {
                    Member = member;
                }

                /// <summary>
                /// Updating metadata.
                /// </summary>
                private void UpdateInfo()
                {
                    // Marking as loaded.
                    _infoLoaded = true;

                    // Validating member.
                    if (Member == null) return;

                    // Receiving info.
                    GetSpecifiedMemberInfo(Member, out PropertyInfo _prop, out FieldInfo _field);

                    // Storing result.
                    _fieldInfo = _field;
                    _propInfo = _prop;
                }
            }


            /// <summary>
            /// Get info suitable for field and properties members.
            /// </summary>
            /// <param name="member"></param>
            /// <param name="propInfo"></param>
            /// <param name="fieldInfo"></param>
            /// <returns>Is the member is property of field?</returns>
            public static bool GetSpecifiedMemberInfo(MemberInfo member,
                out PropertyInfo propInfo, out FieldInfo fieldInfo)
            {
                propInfo = null;
                fieldInfo = null;

                // Check if is property.
                if (member is PropertyInfo propBufer)
                {
                    // Getting stored value.
                    propInfo = propBufer;
                    return true;
                }
                // Check if is field.
                else if (member is FieldInfo fieldBufer)
                {
                    fieldInfo = fieldBufer;
                    return true;
                }

                return false;
            }

            /// <summary>
            /// Setingt value to the member.
            /// </summary>
            /// <param name="member">PropertyInfo ot FieldInfo instance.</param>
            /// <param name="target">Object that contains member.</param>
            /// <param name="value">Value to set.</param>
            public static void SetValue(MemberInfo member, object target, object value)
            {
                // Trying to get specified memebers.
                if (GetSpecifiedMemberInfo(member, out PropertyInfo pi, out FieldInfo fi))
                {
                    if (pi != null) pi.SetValue(target, value); // Operate as property.
                    else fi.SetValue(target, value); // Operate as field.
                }
                else
                {
                    throw new NotSupportedException("SetValue can be applied only to peopreties and fields.");
                }
            }


            /// <summary>
            /// Setingt value to the member.
            /// </summary>
            /// <param name="member">Member's metadata.</param>
            /// <param name="target">Object that contains member.</param>
            /// <param name="value">Value to set.</param>
            public static void SetValue(MemberMeta member, object target, object value)
            {
                // Trying to get specified memebers.
                if (member.IsValue)
                {
                    if (member.IsProperty) member.Property.SetValue(target, value); // Operate as property.
                    else member.Field.SetValue(target, value); // Operate as field.
                }
                else
                {
                    throw new NotSupportedException("SetValue can be applied only to peopreties and fields.");
                }
            }

            /// <summary>
            /// Getting value to the member.
            /// </summary>
            /// <param name="member">Member's metadata.</param>
            /// <param name="target">Object that contains member.</param>
            public static object GetValue(MemberMeta member, object target)
            {
                // Trying to get specified memebers.
                if(member.IsValue)
                { 
                    if (member.IsProperty) return member.Property.GetValue(target); // Operate as property.
                    else return member.Field.GetValue(target); // Operate as field.
                }
                else
                {
                    throw new NotSupportedException("SetValue can be applied only to peopreties and fields.");
                }
            }

            /// <summary>
            /// Getting value to the member.
            /// </summary>
            /// <param name="member">PropertyInfo ot FieldInfo instance.</param>
            /// <param name="target">Object that contains member.</param>
            public static object GetValue(MemberInfo member, object target)
            {
                // Trying to get specified memebers.
                if (GetSpecifiedMemberInfo(member, out PropertyInfo pi, out FieldInfo fi))
                {
                    if (pi != null) return pi.GetValue(target); // Operate as property.
                    else return fi.GetValue(target); // Operate as field.
                }
                else
                {
                    throw new NotSupportedException("SetValue can be applied only to peopreties and fields.");
                }
            }

            /// <summary>
            /// Getting the type of the member.
            /// </summary>
            /// <param name="member">
            /// Member for looking type. Allowed PropertyInfo or FieldInfo.
            /// </param>
            /// <returns>Type of the member.</returns>
            public static Type GetSpecifiedMemberType(MemberInfo member)
            {
                // Trying to get specified memebers.
                if (GetSpecifiedMemberInfo(member, out PropertyInfo pi, out FieldInfo fi))
                {
                    if (pi != null) return pi.PropertyType; // Operate as property.
                    else return fi.FieldType; // Operate as field.
                }
                else
                {
                    throw new NotSupportedException("Get_Type can be applied only to peopreties and fields.");
                }
            }
        }
    }
}
