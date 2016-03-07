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
using System.Linq;

namespace AIR.Common
{
    /// <summary>
    /// The service locator is a design pattern used to encapsulate the processes 
    /// involved in obtaining a service with a strong abstraction layer. This pattern uses 
    /// a central registry known as the "service locator" which on request returns the information 
    /// necessary to perform a certain task. The basic idea behind a service locator is to have an 
    /// object that knows how to get hold of all of the services that an application might need. 
    /// </summary>
    /// <remarks>
    /// What is this? http://martinfowler.com/articles/injection.html#UsingAServiceLocator
    /// Service Locator is an Anti-Pattern? http://blog.ploeh.dk/2010/02/03/ServiceLocatorIsAnAntiPattern.aspx
    /// </remarks>
    public static class ServiceLocator
    {
        private static readonly Dictionary<string, Func<object>> _registrations = new Dictionary<string, Func<object>>();

        private static string CreateKey<T>()
        {
            return typeof (T).Name;
        }

        private static string CreateKey<T>(string name)
        {
            return String.Format("{0}-{1}", CreateKey<T>(), name);
        }

        private static void RegisterInternal<T>(string name, Func<T> instanceCreator) where T : class
        {
            if (instanceCreator == null)
            {
                throw new ArgumentNullException("instanceCreator");
            }

            _registrations[name] = instanceCreator;
        }

        public static void Register<T>(string name, Func<T> instanceCreator) where T : class
        {
            RegisterInternal(CreateKey<T>(name), instanceCreator);
        }

        public static void Register<T>(Func<T> instanceCreator) where T : class
        {
            RegisterInternal(CreateKey<T>(), instanceCreator);
        }

        /*
        public static void Register<T, S>() where T : class where S : class
        {
            Register(() => Activator.CreateInstance(typeof(S)) as T);
        }
        */

        private static T ResolveInternal<T>(string name) where T : class
        {
            Func<object> instanceCreator;

            if (_registrations.TryGetValue(name, out instanceCreator))
            {
                T instance = instanceCreator() as T;

                if (instance != null)
                {
                    return instance;
                }

                // throw new Exception("Could not find registered object instance.");
            }

            return null;
        }

        public static T Resolve<T>(string name) where T : class
        {
            return ResolveInternal<T>(CreateKey<T>(name));
        }

        public static T Resolve<T>() where T : class
        {
            return ResolveInternal<T>(CreateKey<T>());
        }

        public static IEnumerable<T> ResolveAll<T>() where T : class
        {
            return _registrations.Values.OfType<Func<T>>().Select(func => func());
        }
    }

}