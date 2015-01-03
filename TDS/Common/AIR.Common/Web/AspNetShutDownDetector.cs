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
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web.Hosting;

namespace AIR.Common.Web
{
    public class AspNetShutDownDetector : IRegisteredObject
    {
        private readonly Action _onShutdown;

        public AspNetShutDownDetector(Action onShutdown)
        {
            _onShutdown = onShutdown;
        }

        public void Initialize()
        {
            try
            {
                HostingEnvironment.RegisterObject(this);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        public void Stop(bool immediate)
        {
            try
            {
                _onShutdown();
            }
            catch
            {
                // Swallow the exception as Stop should never throw
                // TODO: Log exceptions
            }
            finally
            {
                HostingEnvironment.UnregisterObject(this);
            }
        }
    }
}
