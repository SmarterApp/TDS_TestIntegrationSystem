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
using System.Xml.Serialization;

namespace TDS.ItemScoringEngine
{
    [XmlType(TypeName = "Binding")]
    public class VarBinding : IEquatable<VarBinding>
    {
        public static readonly VarBinding ALL = new VarBinding() { Name = "*", Type = String.Empty, Value = String.Empty };
     
        [XmlAttribute("name")]
        public String Name { get; set; }
        [XmlAttribute("type")]
        public String Type { get; set; }
        [XmlAttribute("value")]
        public String Value { get; set; }

        public bool Equals(VarBinding other)
        {
            return Name == other.Name && Type == other.Type && Value == other.Value;
        }

        public override bool Equals(Object obj)
        {
            if (obj == null)
                return false;

            VarBinding varBinding = obj as VarBinding;
            if (varBinding == null)
                return false;
            else
                return Equals(varBinding);
        }

        public override int GetHashCode()
        {
            return (Name + ":" + Type + ":" + Value).GetHashCode();
        }

        public static bool operator ==(VarBinding obj1, VarBinding obj2)
        {
            if ((object)obj1 == null || ((object)obj2) == null)
                return Object.Equals(obj1, obj2);

            return obj1.Equals(obj2);
        }

        public static bool operator !=(VarBinding obj1, VarBinding obj2)
        {
            if (obj1 == null || obj2 == null)
                return !Object.Equals(obj1, obj2);

            return !(obj1.Equals(obj2));
        }
    }
}
