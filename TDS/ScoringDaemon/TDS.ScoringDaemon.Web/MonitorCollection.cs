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

namespace TDS.ScoringDeamon.Web
{
    public class MonitorCollection
    {
        private static Dictionary<string, ReponseRepoMonitor> _collection = new Dictionary<string, ReponseRepoMonitor>();

        private static String MakeKey(string clientName, string environment) { return clientName.ToUpper() + "|" + environment.ToUpper(); }

        public static void Add(ReponseRepoMonitor reponseRepoMonitor)
        {
            _collection.Add(MakeKey(reponseRepoMonitor.ClientName,reponseRepoMonitor.Environment), reponseRepoMonitor);
        } 

        public static ReponseRepoMonitor Lookup(String clientName, String environment)
        {
            ReponseRepoMonitor result = null;
            _collection.TryGetValue(MakeKey(clientName,environment), out result);
            return result;
        }

        public static ReponseRepoMonitor Remove(String clientName, String environment)
        {
            ReponseRepoMonitor reponseRepoMonitor = Lookup(clientName, environment);
            if (reponseRepoMonitor != null)
            {
                _collection.Remove(MakeKey(clientName, environment));
            }
            return reponseRepoMonitor;
        }

        public static List<ReponseRepoMonitor> GetAll()
        {
            return _collection.Values.ToList();
        }
    }
}
