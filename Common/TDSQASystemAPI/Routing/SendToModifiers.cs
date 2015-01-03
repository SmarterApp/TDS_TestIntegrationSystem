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
using System.Threading.Tasks;

namespace TDSQASystemAPI.Routing
{
    public enum SendTo { DoR = 0, RB = 1, Handscoring = 2, Unknown };

    //TODO: should make this not modifiers but settings?  Load all from project metadata?
    public class SendToModifiersTyped : IEnumerable<KeyValuePair<SendTo, bool>>
    {
        protected Dictionary<string, bool> sendToModifiers { get; set; }

        protected SendToModifiersTyped()
        {
            sendToModifiers = new Dictionary<string, bool>();
        }

        public bool this[SendTo key]
        {
            get
            {
                return sendToModifiers[key.ToString()];
            }
            set
            {
                sendToModifiers[key.ToString()] = value;
            }
        }

        public bool ContainsKey(SendTo key)
        {
            return sendToModifiers.ContainsKey(key.ToString());
        }

        #region IEnumerable<KeyValuePair<SendTo,bool>> Members

        public IEnumerator<KeyValuePair<SendTo, bool>> GetEnumerator()
        {
            foreach (KeyValuePair<string, bool> kvp in sendToModifiers)
            {
                SendTo t;
                if (!Enum.TryParse<SendTo>(kvp.Key, out t))
                    t = SendTo.Unknown;
                yield return new KeyValuePair<SendTo, bool>(t, kvp.Value);
            }
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion
    }

    public class SendToModifiers : SendToModifiersTyped, IEnumerable<KeyValuePair<string, bool>>
    {
        public SendToModifiers()
            : base()
        {
        }

        public bool this[string key]
        {
            get
            {
                return base.sendToModifiers[key];
            }
            set
            {
                base.sendToModifiers[key] = value;
            }
        }

        #region IEnumerable<KeyValuePair<string,bool>> Members

        public new IEnumerator<KeyValuePair<string, bool>> GetEnumerator()
        {
            return base.sendToModifiers.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion

        public bool ContainsKey(string key)
        {
            return base.sendToModifiers.ContainsKey(key);
        }

        public bool ShouldSend(string key)
        {
            return !ContainsKey(key) || base.sendToModifiers[key];
        }
    }
}
