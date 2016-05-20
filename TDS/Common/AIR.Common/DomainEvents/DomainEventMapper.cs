/*******************************************************************************
* Educational Online Test Delivery System
* Copyright (c) 2014 American Institutes for Research
*
* Distributed under the AIR Open Source License, Version 1.0
* See accompanying file AIR-License-1_0.txt or at
* http://www.smarterapp.org/documents/American_Institutes_for_Research_Open_Source_Software_License.pdf
******************************************************************************/
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using AIR.Common.Utilities;

namespace AIR.Common.DomainEvents
{
    /// <summary>
    /// This is used to map domain event handlers. It uses the simple ServiceLocator provided in this assembly.
    /// </summary>
    /// <remarks>
    /// If you use your own IoC framework you should implement this code yourself. 
    /// </remarks>
    public class DomainEventMapper
    {
        /// <summary>
        /// Find all the types in an assembly that inherit from handler interface.
        /// </summary>
        public static IEnumerable<Type> GetHandlerTypes(Assembly assembly)
        {
            Type handlerInterface = typeof(IDomainEventHandler);
            Type[] types = assembly.GetTypes();
            var handlers = types.Where(t => t.IsClass && !t.IsAbstract && handlerInterface.IsAssignableFrom(t));

            foreach (Type handlerType in handlers)
            {
                yield return handlerType;
            }
        }

        /// <summary>
        /// Find all the types in all assemblies that inherit from handler interface.
        /// </summary>
        public static IEnumerable<Type> GetHandlerTypes()
        {
            ReadOnlyCollection<Assembly> binFolderAssemblies = AssemblyLocator.GetBinFolderAssemblies();

            foreach (Assembly assembly in binFolderAssemblies)
            {
                foreach (Type handlerType in GetHandlerTypes(assembly))
                {
                    yield return handlerType;
                }
            }
        }

        /// <summary>
        /// Find all the methods for a handler interface that handle a domain event.
        /// </summary>
        private static void FindMethods(Type handlerType)
        {
            ParameterInfo[] parameters = null;
            foreach (MethodInfo minfo in handlerType
                .GetMethods(BindingFlags.Instance | BindingFlags.Public)
                .Where(mi => mi.ReturnType == typeof(void) &&
                                (parameters = mi.GetParameters()).Length == 1 &&
                                typeof(IDomainEvent).IsAssignableFrom(parameters[0].ParameterType)))
            {
                var eventType = parameters[0].ParameterType;

            }
        }

        /// <summary>
        /// Scan all the assemblies for event handlers and map them ServiceLocator.
        /// </summary>
        public static void Load()
        {
            foreach (Type handlerType in GetHandlerTypes())
            {
                // TODO: http://www.codewrecks.com/blog/index.php/2012/08/04/scan-all-assembly-of-the-directory-to-find-domainevent-handlers-and-command-executors/
            }
        }
    }
}
