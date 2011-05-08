﻿using System.Xml.Serialization;

namespace SharpGen.Config
{
    [XmlType("map")]
    public class MappingRule : MappingBaseRule
    {
        public MappingRule()
        {
            IsFinalMappingName = false;
            MethodCheckReturnType = true;
        }

        [XmlIgnore]
        public bool? MethodCheckReturnType { get; set; }
        [XmlAttribute("check")]
        public bool _MethodCheckReturnType_ { get { return MethodCheckReturnType.Value; } set { MethodCheckReturnType = value; } } public bool ShouldSerialize_MethodCheckReturnType_() { return MethodCheckReturnType != null; }

        /// <summary>
        /// General visibility for Methods
        /// </summary>
        [XmlIgnore]
        public Visibility? Visibility { get; set; }
        [XmlAttribute("visibility")]
        public Visibility _Visibility_ { get { return Visibility.Value; } set { Visibility = value; } } public bool ShouldSerialize_Visibility_() { return Visibility != null; }

        /// <summary>
        /// General visibility for DefaultCallback class
        /// </summary>
        [XmlIgnore]
        public Visibility? NativeCallbackVisibility { get; set; }
        [XmlAttribute("callback-visibility")]
        public Visibility _NativeCallbackVisibility_ { get { return NativeCallbackVisibility.Value; } set { NativeCallbackVisibility = value; } } public bool ShouldSerialize_NativeCallbackVisibility_() { return NativeCallbackVisibility != null; }

        [XmlIgnore]
        public bool? NameKeepUnderscore { get; set; }
        [XmlAttribute("keep-underscore")]
        public bool  _NameKeepUnderscore_ { get { return NameKeepUnderscore.Value; } set { NameKeepUnderscore = value; } } public bool ShouldSerialize_NameKeepUnderscore_() { return NameKeepUnderscore != null; }

        /// <summary>
        /// Name of a native callback
        /// </summary>
        [XmlAttribute("callback-name")]
        public string NativeCallbackName { get; set; }

        /// <summary>
        /// Used for methods, to force a method to not be translated to a property
        /// </summary>
        [XmlIgnore]
        public bool? Property { get; set; }
        [XmlAttribute("property")]
        public bool _Property_ { get { return Property.Value; } set { Property = value; } } public bool ShouldSerialize_Property_() { return Property != null; }

        /// <summary>
        /// Mapping name
        /// </summary>
        [XmlAttribute("name-tmp")]
        public string MappingName { get; set; }
        public bool ShouldSerializeMappingName() { return IsFinalMappingName.HasValue && !IsFinalMappingName.Value; }

        /// <summary>
        /// Mapping name
        /// </summary>
        [XmlAttribute("name")]
        public string MappingNameFinal
        {
            get { return MappingName; }
            set
            {
                MappingName = value;
                IsFinalMappingName = true;
            }
        }
        public bool ShouldSerializeMappingNameFinal() { return !IsFinalMappingName.HasValue || IsFinalMappingName.Value; }

        //[XmlAttribute("final")]
        //public bool _IsFinalMappingName_ { get { return IsFinalMappingName.Value; } set { IsFinalMappingName = value; } } public bool ShouldSerialize_IsFinalMappingName_() { return IsFinalMappingName != null; }

        /// <summary>
        /// True if the MappingName doesn't need any further rename processing
        /// </summary>
        [XmlIgnore]
        public bool? IsFinalMappingName { get; set; }
        //[XmlAttribute("final")]
        //public bool _IsFinalMappingName_ { get { return IsFinalMappingName.Value; } set { IsFinalMappingName = value; } } public bool ShouldSerialize_IsFinalMappingName_() { return IsFinalMappingName != null; }

        /// <summary>
        /// True if a struct should used a native value type marshalling
        /// </summary>
        [XmlIgnore]
        public bool? StructHasNativeValueType { get; set; }
        [XmlAttribute("native")]
        public bool _StructHasNativeValueType_ { get { return StructHasNativeValueType.Value; } set { StructHasNativeValueType = value; } } public bool ShouldSerialize_StructHasNativeValueType_() { return StructHasNativeValueType != null; }

        /// <summary>
        /// True if a struct should be used as a class instead of struct (imply StructHasNativeValueType)
        /// </summary>
        [XmlIgnore]
        public bool? StructToClass { get; set; }
        [XmlAttribute("struct-to-class")]
        public bool _StructToClass_ { get { return StructToClass.Value; } set { StructToClass = value; } } public bool ShouldSerialize_StructToClass_() { return StructToClass != null; }

        /// <summary>
        /// True if a struct is using some Custom Marshal (imply StructHasNativeValueType)
        /// </summary>
        [XmlIgnore]
        public bool? StructCustomMarshall { get; set; }
        [XmlAttribute("marshal")]
        public bool _StructCustomMarshall_ { get { return StructCustomMarshall.Value; } set { StructCustomMarshall = value; } } public bool ShouldSerialize_StructCustomMarshall_() { return StructCustomMarshall != null; }

        /// <summary>
        /// True if a struct is using some a Custom New for the Native struct (imply StructHasNativeValueType)
        /// </summary>
        [XmlIgnore]
        public bool? StructCustomNew { get; set; }
        [XmlAttribute("new")]
        public bool _StructCustomNew_ { get { return StructCustomNew.Value; } set { StructCustomNew = value; } } public bool ShouldSerialize_StructCustomNew_() { return StructCustomNew != null; }

        /// <summary>
        /// True to force a struct with Native marshalling to have the method __MarshalTo generated
        /// </summary>
        [XmlIgnore]
        public bool? StructForceMarshalToToBeGenerated { get; set; }
        [XmlAttribute("marshalto")]
        public bool _StructForceMarshalToToBeGenerated_ { get { return StructForceMarshalToToBeGenerated.Value; } set { StructForceMarshalToToBeGenerated = value; } } public bool ShouldSerialize_StructForceMarshalToToBeGenerated_() { return StructForceMarshalToToBeGenerated != null; }

        /// <summary>
        /// Mapping type name
        /// </summary>
        [XmlAttribute("type")]
        public string MappingType { get; set; }

        /// <summary>
        /// Pointer to modify the type
        /// </summary>
        [XmlAttribute("pointer")]
        public string Pointer { get; set; }

        /// <summary>
        /// ArrayDimension
        /// </summary>
        [XmlIgnore]
        public int? TypeArrayDimension { get; set; }
        [XmlAttribute("array")]
        public int _TypeArrayDimension_ { get { return TypeArrayDimension.Value; } set { TypeArrayDimension = value; } } public bool ShouldSerialize_TypeArrayDimension_() { return TypeArrayDimension != null; }

        /// <summary>
        /// Used for enums, to tag enums that are used as flags
        /// </summary>
        [XmlIgnore]
        public bool? EnumHasFlags { get; set; }
        [XmlAttribute("flags")]
        public bool _EnumHasFlags_ { get { return EnumHasFlags.Value; } set { EnumHasFlags = value; } } public bool ShouldSerialize_EnumHasFlags_() { return EnumHasFlags != null; }

        /// <summary>
        /// Used for enums, to tag enums that should have none value (0)
        /// </summary>
        [XmlIgnore]
        public bool? EnumHasNone { get; set; }
        [XmlAttribute("none")]
        public bool _EnumHasNone_ { get { return EnumHasNone.Value; } set { EnumHasNone = value; } } public bool ShouldSerialize_EnumHasNone_() { return EnumHasNone != null; }

        /// <summary>
        /// Used for interface to mark them as callback interface
        /// </summary>
        [XmlIgnore]
        public bool? IsCallbackInterface { get; set; }
        [XmlAttribute("callback")]
        public bool _IsCallbackInterface_ { get { return IsCallbackInterface.Value; } set { IsCallbackInterface = value; } } public bool ShouldSerialize_IsCallbackInterface_() { return IsCallbackInterface != null; }

        /// <summary>
        /// Used for interface to mark them as dual-callback interface
        /// </summary>
        [XmlIgnore]
        public bool? IsDualCallbackInterface { get; set; }
        [XmlAttribute("callback-dual")]
        public bool _IsDualCallbackInterface_ { get { return IsDualCallbackInterface.Value; } set { IsDualCallbackInterface = value; } } public bool ShouldSerialize_IsDualCallbackInterface_() { return IsDualCallbackInterface != null; }

        /// <summary>
        /// DLL name attached to a function
        /// </summary>
        [XmlAttribute("dll")]
        public string FunctionDllName { get; set; }

        /// <summary>
        /// DLL name attached to a function
        /// </summary>
        [XmlAttribute("macro-dll")]
        public string FunctionDllNameFromMacro { get; set; }

        /// <summary>
        /// Parameter Attribute
        /// </summary>
        [XmlIgnore]
        public ParamAttribute? ParameterAttribute { get; set; }
        [XmlAttribute("attribute")]
        public ParamAttribute _ParameterAttribute_ { get { return ParameterAttribute.Value; } set { ParameterAttribute = value; } } public bool ShouldSerialize_ParameterAttribute_() { return ParameterAttribute != null; }

        /// <summary>
        /// Parameter is tagged to be used as a return type
        /// </summary>
        [XmlIgnore]
        public bool? ParameterUsedAsReturnType { get; set; }
        [XmlAttribute("return")]
        public bool _ParameterUsedAsReturnType_ { get { return ParameterUsedAsReturnType.Value; } set { ParameterUsedAsReturnType = value; } } public bool ShouldSerialize_ParameterUsedAsReturnType_() { return ParameterUsedAsReturnType != null; }

        /// <summary>
        /// ClassType attached to a function
        /// </summary>
        //[XmlAttribute("class")]
        [XmlAttribute("group")]
        public string CsClass { get; set; }

        //public override string ToString()
        //{
        //    return Utilities.PropertiesToString(this);
        //}
    }
}