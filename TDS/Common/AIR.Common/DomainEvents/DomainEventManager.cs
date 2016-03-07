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

namespace AIR.Common.DomainEvents
{
    /// <summary>
    /// A simple domain event framework for applications.
    /// </summary>
    /// <remarks>
    /// http://martinfowler.com/eaaDev/DomainEvent.html
    /// http://www.udidahan.com/2009/06/14/domain-events-salvation/
    /// http://lostechies.com/jimmybogard/2010/04/08/strengthening-your-domain-domain-events/
    /// http://barn.headscape.co.uk/code/using-domain-events/
    /// http://jaysmith.github.io/DomainEvents/
    /// Which Layer for Domain Events, EventHandlers, Dispatcher? http://stackoverflow.com/q/3925184
    /// Where to raise persistence-dependent domain events? http://stackoverflow.com/a/5887203
    /// </remarks>
    public static class DomainEventManager
    {
        /// <summary>
        /// So that each thread has its own callbacks.
        /// </summary>
        /// <remarks>
        /// You can use this to add and then remove events for a specific request.
        /// You should clear this when you are finished with the thread.
        /// </remarks>
        [ThreadStatic]
        private static List<Delegate> _actions;

        /// <summary>
        /// The container for getting handlers.
        /// </summary>
        /// <remarks>
        /// You can use this if you don't want to use ServiceManager.
        /// </remarks>
        public static IEventContainer Container { get; set; }

        /// <summary>
        /// Registers a callback for the given domain event.
        /// </summary>
        public static void Register<T>(Action<T> callback) where T : IDomainEvent
        {
            if (_actions == null)
            {
                _actions = new List<Delegate>();
            }

            _actions.Add(callback);
        }

        /// <summary>
        /// Clears callbacks passed to Register.
        /// </summary>
        public static void ClearCallbacks()
        {
            _actions = null;
        }

        /// <summary>
        /// Raises the given domain event.
        /// </summary>
        public static void Raise<T>(T domainEvent) where T : class, IDomainEvent
        {
            // We have three places to look for events..

            // 1: Check ServiceLocator for registered event handlers.
            IEnumerable<IDomainEventHandler<T>> handlers = ServiceLocator.ResolveAll<IDomainEventHandler<T>>();

            foreach (IDomainEventHandler<T> handler in handlers)
            {
                handler.Handle(domainEvent);
            }

            // 2: Check if there is a custom container to look up handlers.
            if (Container != null)
            {
                foreach (IDomainEventHandler<T> handler in Container.Handlers(domainEvent))
                {
                    handler.Handle(domainEvent);
                }
            }

            // 3: Look through registered delegates on this thread.
            if (_actions != null)
            {
                _actions.OfType<Action<T>>().ForEach(cb => cb(domainEvent));
            }
        }
    }
}