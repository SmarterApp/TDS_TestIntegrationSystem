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
using System.Text;

namespace AIR.Common.DomainEvents
{
    /// <summary>
    /// The EventContainer interface for getting event handlers.
    /// </summary>
    /// <remarks>
    /// You can use this if you want to use your own IoC resolver. For example here is Ninject:
    /// http://www.contentedcoder.com/2013/05/domain-events-with-ninject-and-web-api.html
    /// </remarks>
    public interface IEventContainer
    {
        /// <summary>
        /// The event handlers for a type.
        /// </summary>
        IEnumerable<IDomainEventHandler<T>> Handlers<T>(T domainEvent) where T : IDomainEvent;
    }
}
