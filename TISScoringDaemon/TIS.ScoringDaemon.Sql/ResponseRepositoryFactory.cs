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
using TDS.ScoringDaemon.Abstractions;

namespace TIS.ScoringDaemon.Sql
{
    class ResponseRepositoryFactory : IResponseRespositoryFactory
    {
        private Dictionary<string, IResponseRepository> repositories = new Dictionary<string, IResponseRepository>();  // collection of repos already created
        private readonly Object _lock = new Object();

        public IResponseRepository Create(string connectionString, string clientName, string environment)
        {
            string key = connectionString.ToUpper() + "|" + clientName.ToUpper() + "|" + environment.ToUpper();
            IResponseRepository repo = null;
            if (repositories.TryGetValue(key, out repo))
                return repo;

            // we need to create a new one
            lock (_lock)
            {
                if (repositories.TryGetValue(key, out repo))
                    return repo;
                repo = new ResponseRepository(connectionString, clientName);
                repositories.Add(key, repo);
            }

            return repo;
        }
    }
}
