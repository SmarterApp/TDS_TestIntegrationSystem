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
using TDS.Shared.Data; using TDS.Shared.Configuration;
using System.Text;
using AIR.Common.Collections;
namespace TDS.Shared.Security
{
    public abstract class TDSUser
    {
        private string id;
        private List<string> roles = new List<string>();
        private long key; //user key in tblUser
        long entitykey; //RTS entity key
        private string password;
        string fullname;

        private string clietname;
        //UserCookie userInfoCookie;
        public string RTSPassword;

        private bool isAuth = false; //is authenticated
        private bool isNew = false; //is the user just login?


        public string ClientName
        {
            get { return clietname; }
            set { clietname = value; }
        }

        public long Key
        {
            get { return key; }
            set { key = value; }
        }
        public long EntityKey
        {
            get { return entitykey; }
            set { entitykey = value; }
        }
        public string ID
        {
            get { return id; }
            set { id = value; }
        }
        public string Password
        {
            get { return password; }
            set { password = value; }
        }
        public List<string> Roles
        {
            get { return roles; }
            set { roles = value; }
        }
        public string GetRoles(string delimiter)
        {
            return Roles.Join(delimiter);         
        }

        public string FullName
        {
            get { return fullname; }
            set { fullname = value; }
        }
        public bool IsAuth
        {
            get { return isAuth; }
            set { isAuth = value; }
        }

        public bool IsNew
        {
            get { return isNew; }
            set { isNew = value; }
        }

        public bool HasRole(string role)
        {
            if (this.roles == null || this.roles.Count < 1)
                return false;
            for (int i = 0; i < this.roles.Count; i++) //sequencial search since very fews
            {
                if (role == this.roles[i])
                    return true;
            }
            return false;
        }

        public ReturnStatus ReturnedStatus {get; set;}

        public string PrintUser()
        {
            return string.Format("<div>Key: {0}</div><div>UserID: {1}</div><div>UserRoles: {2}</div>", Key, ID, Roles);
        }
    }    
}
