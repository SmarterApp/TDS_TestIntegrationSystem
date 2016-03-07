/*******************************************************************************
* Educational Online Test Delivery System
* Copyright (c) 2014 American Institutes for Research
*
* Distributed under the AIR Open Source License, Version 1.0
* See accompanying file AIR-License-1_0.txt or at
* http://www.smarterapp.org/documents/American_Institutes_for_Research_Open_Source_Software_License.pdf
******************************************************************************/
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeExtensions.cs" company="WebOS - http://www.coolwebos.com">
//   Copyright © Dixin 2010 http://weblogs.asp.net/dixin
// </copyright>
// <summary>
//   Defines the TypeExtensions type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AIR.Common.Dynamic
{
    using System;
    using System.Linq;
    using System.Reflection;

    internal static class TypeExtensions
    {
        #region Methods

        internal static FieldInfo GetBaseField(this Type type, string name)
        {
            Type @base = type.BaseType;
            if (@base == null)
            {
                return null;
            }

            return @base.GetTypeField(name) ?? @base.GetBaseField(name);
        }

        internal static PropertyInfo GetBaseIndex(this Type type, params object[] args)
        {
            Type @base = type.BaseType;
            if (@base == null)
            {
                return null;
            }

            return @base.GetTypeIndex(args) ?? @base.GetBaseIndex(args);
        }

        internal static MethodInfo GetBaseMethod(this Type type, string name, params object[] args)
        {
            Type @base = type.BaseType;
            if (@base == null)
            {
                return null;
            }

            return @base.GetTypeMethod(name, args) ?? @base.GetBaseMethod(name, args);
        }

        internal static PropertyInfo GetBaseProperty(this Type type, string name)
        {
            Type @base = type.BaseType;
            if (@base == null)
            {
                return null;
            }

            return @base.GetTypeProperty(name) ?? @base.GetBaseProperty(name);
        }

        internal static MethodInfo GetInterfaceMethod(this Type type, string name, params object[] args)
        {
            return
                type.GetInterfaces().Select(type.GetInterfaceMap).SelectMany(mapping => mapping.TargetMethods)
                    .FirstOrDefault(
                        method =>
                        method.Name.Split('.').Last().Equals(name, StringComparison.Ordinal) &&
                        method.GetParameters().Count() == args.Length &&
                        method.GetParameters().Select(
                            (parameter, index) =>
                            parameter.ParameterType.IsAssignableFrom(args[index].GetType())).Aggregate(
                                true, (a, b) => a && b));
        }

        internal static FieldInfo GetTypeField(this Type type, string name)
        {
            return
                type.GetFields(
                    BindingFlags.GetField | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public |
                    BindingFlags.NonPublic).FirstOrDefault(
                        field => field.Name.Equals(name, StringComparison.Ordinal));
        }

        internal static PropertyInfo GetTypeIndex(this Type type, params object[] args)
        {
            return
                type.GetProperties(
                    BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.Static |
                    BindingFlags.Public | BindingFlags.NonPublic).FirstOrDefault(
                        property =>
                        property.GetIndexParameters().Any() &&
                        property.GetIndexParameters().Select(
                            (parameter, index) => parameter.ParameterType == args[index].GetType()).Aggregate(
                                true, (a, b) => a && b));
        }

        internal static MethodInfo GetTypeMethod(this Type type, string name, params object[] args)
        {
            return
                type.GetMethods(
                    BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.Static |
                    BindingFlags.Public | BindingFlags.NonPublic).FirstOrDefault(
                        method =>
                        method.Name.Equals(name, StringComparison.Ordinal) &&
                        method.GetParameters().Count() == args.Length &&
                        method.GetParameters().Select(
                            (parameter, index) => parameter.ParameterType == args[index].GetType()).Aggregate(
                                true, (a, b) => a && b));
        }

        internal static PropertyInfo GetTypeProperty(this Type type, string name)
        {
            return
                type.GetProperties(
                    BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.Static |
                    BindingFlags.Public | BindingFlags.NonPublic).FirstOrDefault(
                        property => property.Name.Equals(name, StringComparison.Ordinal));
        }

        #endregion
    }
}