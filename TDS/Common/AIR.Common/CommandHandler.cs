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

namespace AIR.Common
{
    public interface ICommand {}

    public interface IHandler { }

    public interface IHandler<in TCommand> : IHandler where TCommand : ICommand
    {
        void Handle(TCommand command);
    }

    public interface IHandler<in TCommand, out TResult> : IHandler where TCommand : ICommand
    {
        TResult Handle(TCommand command);
    }

    /// <summary>
    /// Simple CQRS
    /// </summary>
    /// <remarks>
    /// http://martinfowler.com/bliki/CQRS.html
    /// http://abdullin.com/journal/2010/9/26/theory-of-cqrs-command-handlers-sagas-ars-and-event-subscrip.html
    /// http://www.amazedsaint.com/2011/11/cqrs-dilemma-and-related-random.html
    /// http://lucisferre.net/2012/01/30/some-complaints-about-cqrs-and-event-sourcing/
    /// </remarks>
    public static class CommandHandler
    {
        private static readonly Dictionary<Type, Func<IHandler>> _handlers = new Dictionary<Type, Func<IHandler>>();

        /// <summary>
        /// Register a command to a specific interface.
        /// </summary>
        public static void Register<TCommand>(Func<IHandler> handlerCreator) where TCommand : ICommand
        {
            _handlers[typeof(TCommand)] = handlerCreator;
        }

        private static Func<IHandler> GetHandler(Type type)
        {
            Func<IHandler> handlerCreator;
            _handlers.TryGetValue(type, out handlerCreator);
            return handlerCreator;
        }

        public static TResult Send<TCommand, TResult>(TCommand args) where TCommand : ICommand where TResult : class
        {
            TResult result = null;
            Func<IHandler> handlerCreator = GetHandler(args.GetType());

            if (handlerCreator != null)
            {
                IHandler<TCommand, TResult> handler = handlerCreator() as IHandler<TCommand, TResult>;

                if (handler != null)
                {
                    result = handler.Handle(args);
                }
            }

            return result;
        }
    }
}
