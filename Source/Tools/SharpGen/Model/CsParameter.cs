﻿// Copyright (c) 2010-2011 SharpDX - Alexandre Mutel
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.Text;
using SharpGen.Config;

namespace SharpGen.Model
{
    public class CsParameter : CsMarshalBase, ICloneable
    {
        public CsParameter()
        {
            // Default is In
            Attribute = CsParameterAttribute.In;
        }

        public CsParameterAttribute Attribute { get; set; }

        public bool IsOptionnal { get; set; }

        public bool IsUsedAsReturnType { get; set; }

        private const int SizeOfLimit = 16;

        protected override void UpdateFromTag(MappingRule tag)
        {
            base.UpdateFromTag(tag);
            if (tag.ParameterUsedAsReturnType.HasValue)
                IsUsedAsReturnType = tag.ParameterUsedAsReturnType.Value;
        }

        public bool IsFixed
        {
            get
            {
                if (Attribute == CsParameterAttribute.Ref || Attribute == CsParameterAttribute.RefIn)
                {
                    if (IsRefInValueTypeOptional || IsRefInValueTypeByValue)
                        return false;
                    return true;
                }
                if (Attribute == CsParameterAttribute.Out && !IsBoolToInt)
                    return true;
                if (IsArray)
                    return true;
                return false;
            }
        }

        public string TempName
        {
            get { return Name + "_"; }
        }

        public bool IsRef
        {
            get { return Attribute == CsParameterAttribute.Ref; }
        }

        public bool IsRefIn
        {
            get { return Attribute == CsParameterAttribute.RefIn; }
        }

        public bool IsIn
        {
            get { return Attribute == CsParameterAttribute.In; }
        }

        public bool IsOut
        {
            get { return Attribute == CsParameterAttribute.Out; }
        }

        public bool IsPrimitive
        {
            get { return PublicType.Type != null && PublicType.Type.IsPrimitive; }
        }

        public bool IsString
        {
            get { return PublicType.Type == typeof (string); }
        }

        public bool IsValueType
        {
            get { return PublicType is CsStruct || PublicType is CsEnum || (PublicType.Type != null && (PublicType.Type.IsValueType || PublicType.Type.IsPrimitive)); }
        }

        public bool HasNativeValueType
        {
            get { return (PublicType is CsStruct && (PublicType as CsStruct).HasMarshalType) ; }
        }

        public bool IsRefInValueTypeOptional
        {
            get { return IsRefIn && IsValueType && !IsArray && IsOptionnal; }
        }

        public bool IsRefInValueTypeSmall
        {
            get { return IsRefIn && IsValueType && !IsArray && PublicType.SizeOf <= SizeOfLimit && !HasNativeValueType; }
        }

        public bool IsRefInValueTypeByPointer
        {
            get { return IsRefInValueTypeSmall && HasPointer; }
        }

        public bool IsRefInValueTypeByValue
        {
            get { return IsRefInValueTypeSmall; }
        }

        public string ParamName
        {
            get
            {
                StringBuilder builder = new StringBuilder();
                if (Attribute == CsParameterAttribute.Out && (!IsArray || PublicType.Name == "string"))
                    builder.Append("out ");
                else if ((Attribute == CsParameterAttribute.Ref || Attribute == CsParameterAttribute.RefIn) && !IsArray)
                {
                    if (!(IsRefInValueTypeOptional || IsRefInValueTypeByValue))
                        builder.Append("ref ");
                }

                if (IsRefIn && IsValueType && !IsArray && IsOptionnal)
                    builder.Append(PublicType.QualifiedName + "?");
                else
                    builder.Append(PublicType.QualifiedName);

                if (IsArray && PublicType.Name != "string")
                    builder.Append("[]");
                builder.Append(" ");
                builder.Append(Name);
                return builder.ToString();
            }
        }

        //IntPtr pDevice_;
        //SharpDX.Interop.CalliVoid(_nativePointer, 3 * 4, &pDevice_);
        //pDevice = new SharpDX.Direct3D11.Device(pDevice_);

        //for (int i = 0; i < pSamplers.Length; i++)
        //    pSamplers[i] = new SamplerState(pSamplers_[i]);

        public string CallName
        {
            get
            {
                if (IsOut)
                {
                    if (PublicType is CsInterface)
                    {
                        if (IsArray && IsOptionnal)
                            return Name + "==null?(void*)0:" + TempName;
                        return "&" + TempName;
                    }
                    if (IsArray)
                    {
                        //if (IsValueType && IsOptionnal)
                        //{
                        //    //return Name + "==null?(void*)IntPtr.Zero:" + TempName;
                        //}
                        //else
                        //{
                        //    return TempName;
                        //}
                        return TempName;
                    }
                    if (IsFixed && !HasNativeValueType)
                    {
                        if (IsUsedAsReturnType)
                            return "&" + Name;
                        return TempName;
                    }
                    if (HasNativeValueType || IsBoolToInt)
                    {
                        return "&" + TempName;
                    }
                    if (IsValueType)
                    {
                        return "&" + Name;
                    }
                    else
                    {
                        return TempName;
                    }
                }
                if (IsRefInValueTypeOptional)
                    return "(" + Name + ".HasValue)?&" + TempName + ":(void*)IntPtr.Zero";
                if (IsRefInValueTypeSmall)
                    return "&" + Name;
                if (PublicType.QualifiedName == Global.Name + ".Color4" && MarshalType.Type == typeof(int))
                {
                    return Name + ".ToArgb()";
                }
                if (PublicType is CsEnum && !IsArray)
                    return "unchecked((int)" + Name + ")";
                if (PublicType.Type == typeof (string))
                    return "(void*)" + TempName;
                //if (PublicType.Type == typeof (byte))
                //    return "(int)" + Name;
                if (PublicType is CsInterface && Attribute == CsParameterAttribute.In && !IsArray)
                    return "(void*)((" + Name + " == null)?IntPtr.Zero:" + Name + ".NativePointer)";
                if (IsArray)
                {
                    if (HasNativeValueType || IsBoolToInt)
                    {
                        //if (IsOptionnal)
                        //    return Name + "==null?(void*)IntPtr.Zero:" + TempName;
                        return TempName;
                    }
                    if (IsValueType && IsOptionnal)
                    {
                        //return Name + "==null?(void*)IntPtr.Zero:" + TempName;
                        return TempName;
                    }
                }
                if (IsBoolToInt)
                    return "(" + Name + "?1:0)";
                if (IsFixed && !HasNativeValueType)
                    return TempName;
                if (PublicType.Type == typeof (IntPtr) && !IsArray)
                    return "(void*)" + Name;
                if (HasNativeValueType)
                    return "&" + TempName;
                if (PublicType.Name == Global.Name + ".Size")
                    return "(void*)" + Name;
                return Name;
            }
        }

        public object Clone()
        {
            var parameter = (CsParameter)MemberwiseClone();
            parameter.Parent = null;
            return parameter;
        }
    }
}