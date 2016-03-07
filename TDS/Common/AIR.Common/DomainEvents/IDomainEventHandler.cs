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
    /// Marker interface so we can look this up using reflection.
    /// </summary>
    /// <remarks>
    /// If we can get rid of this we should.
    /// </remarks>
    public interface IDomainEventHandler {}

    /// <summary>
    /// This interface is used for handlers.
    /// </summary>
    /// <typeparam name="T">The event data type.</typeparam>
    public interface IDomainEventHandler<in T> : IDomainEventHandler where T : IDomainEvent
    {
        void Handle(T args);
    }
}
