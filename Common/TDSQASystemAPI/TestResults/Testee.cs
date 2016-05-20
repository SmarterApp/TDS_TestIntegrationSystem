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
using System.Text;
using System.Xml;
using System.Web;
using System.Globalization;
using System.Xml.Serialization;
using System.Linq;

namespace TDSQASystemAPI.TestResults
{
    [System.SerializableAttribute()]
    [XmlTypeAttribute(AnonymousType = true)]
    public class Testee
	{
        public enum DistrictType : int
        {
            OP = 0,
            //CONTROL = 1,
            DEMO = 1
        };

		#region Properties

        [XmlIgnore]
        private long entityKey;
        /// <summary>
        /// Returns the student's RTS key
        /// </summary>
        [XmlAttribute("key")]
        public long EntityKey
        {
            get
            {
                return entityKey;
            }
            set { entityKey = value; }
        }

        [XmlIgnore]
        private bool isDemo
        {
            get
            {
                return Convert.ToBoolean(IsDemoInt);
            }
            set
            {
                IsDemoInt = Convert.ToInt32(value);
            }
        }
        /// <summary>
        /// Returns the flag indicating whether or not this is a demo student
        /// </summary>
        [XmlIgnore]
        public bool IsDemo
        {
            get
            {
                return isDemo;
            }
        }
        [XmlAttribute("isDemo")]
        public int IsDemoInt = 0;


        /// <summary>
        /// Returns the IsDemo flag as the DistrictType enum. !IsDemo = DistrictType.OP, IsDemo = DistrictType.DEMO
        /// </summary>
        [XmlIgnore]
        public DistrictType TesteeDistrictType
        {
            get
            {
                return (DistrictType)Enum.Parse(typeof(Testee.DistrictType), IsDemo ? "1" : "0");
            }
        }

        /// <summary>
        /// Returns the intial first name that TDS pulled when the student logged in.  String.Empty if
        /// the attribute is not in the file.
        /// </summary>
        [XmlIgnore]
        public string FirstName
		{
			get
			{
                return GetAttributeValueAsString(TesteeAttribute.AttributeName.FIRST_NAME, TesteeProperty.PropertyContext.INITIAL);
			}
		}

        /// <summary>
        /// Returns the intial last name that TDS pulled when the student logged in.  String.Empty if
        /// the attribute is not in the file.
        /// </summary>
        [XmlIgnore]
        public string LastName
		{
			get
			{
                return GetAttributeValueAsString(TesteeAttribute.AttributeName.LAST_NAME, TesteeProperty.PropertyContext.INITIAL);
			}
		}

        /// <summary>
        /// Returns the intial enrolled grade that TDS pulled when the student logged in.  String.Empty if
        /// the attribute is not in the file.
        /// </summary>
        [XmlIgnore]
        public string EnrolledGrade
        {
            get
            {
                return GetAttributeValueAsString(TesteeAttribute.AttributeName.ENROLLED_GRADE, TesteeProperty.PropertyContext.INITIAL);
            }
        }

        /// <summary>
        /// Returns the intial date of birth that TDS pulled when the student logged in.  DateTime.MinValue if 
        /// the attribute is not in the file.
        /// </summary>
        [XmlIgnore]
        public DateTime BirthDate
		{
			get
			{
                DateTime birthDate;
                TesteeAttribute a = GetAttribute(TesteeAttribute.AttributeName.BIRTH_DATE, TesteeProperty.PropertyContext.INITIAL);
                if (a == null || !TryParseDOB(a.Value, out birthDate))
                    birthDate = DateTime.MinValue;
				return birthDate;
			}
		}

        /// <summary>
        /// Returns a sorted list of TesteeAttributes by name, context
        /// </summary>
        [XmlElement("ExamineeAttribute", Order = 1)]
        public List<TesteeAttribute> TesteeAttributes;

        /// <summary>
        /// Returns the number of distinct testeeattribute names in the collection.  This does
        /// not count an attribute name more than once even if it has multiple contexts.
        /// </summary>
        [XmlIgnore]
        public int TesteeAttributeCount
        {
            get
            {
                List<String> names = new List<string>();
                foreach (TesteeAttribute att in TesteeAttributes)
                    if (!names.Contains(att.Name, StringComparer.InvariantCultureIgnoreCase))
                        names.Add(att.Name);
                return names.Count;
            }
        }

        /// <summary>
        /// Returns a sorted list of TesteeRelationship by name, context.  If there are multiple relationships
        /// with the same Name, Context, they'll be sorted by EntityKey
        /// </summary>
        [XmlElement("ExamineeRelationship", Order = 2)]
        public List<TesteeRelationship> TesteeRelationships;

		#endregion

        public Testee() { }
        public Testee(long entityKey, bool isDemo)
        {
            this.entityKey = entityKey;
            TesteeAttributes = new List<TesteeAttribute>();
            TesteeRelationships = new List<TesteeRelationship>();
			this.isDemo = isDemo;
        }

        /// <summary>
        /// Add a TesteeAttribute to this Testee.  Returns true if the name/context combination was
        /// added to the collection.  Returns false if it already existed and was not added.
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public bool AddAttribute(TesteeAttribute a)
        {
            bool added = false;
            if (!TesteeAttributes.Contains(a))
            {
                TesteeAttributes.Add(a);
                TesteeAttributes.Sort();
                added = true;
            }
            return added;
        }

        /// <summary>
        /// Add a TesteeRelationship to this Testee.  Returns true if the name/context combination was
        /// added to the collection.  Returns false if it already existed and was not added.
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        public bool AddRelationship(TesteeRelationship r)
        {
            bool added = false;
            if (!TesteeRelationships.Contains(r))
            {
                TesteeRelationships.Add(r);
                TesteeRelationships.Sort();
                added = true;
            }
            return added;
        }

        /// <summary>
        /// Removes a TesteeRelationship from the collection
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        public bool RemoveRelationship(TesteeRelationship r)
        {
            if (r == null)
                return false;

            bool removed = false;

            if (TesteeRelationships.Contains(r))
                removed = TesteeRelationships.Remove(r);

            return removed;
        }

        /// <summary>
        /// Returns a TesteeAttribute by name, context.  Null if it doesn't exist.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public TesteeAttribute GetAttribute(string name, TesteeProperty.PropertyContext context)
        {
            return GetAttribute(name, new List<TesteeProperty.PropertyContext>() { context });
        }

        /// <summary>
        /// Returns the first TesteeAttribute by name and context given a list of TesteeProperty.PropertyContexts
        /// in order of priority.  Will start with the first element in the list; if that context
        /// is not in the collection, it'll try the 2nd, and so on until a TesteeAttribute is found.  If
        /// no TesteeAttribute is found in the collection, null is returned.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="contextPriority"></param>
        /// <returns></returns>
        public TesteeAttribute GetAttribute(string name, List<TesteeProperty.PropertyContext> contextPriority)
        {
            TesteeAttribute a = null;

            foreach (TesteeProperty.PropertyContext context in contextPriority)
            {
                a = TesteeAttributes.FirstOrDefault(x => x.Name.Equals(name) && x.Context.Equals(context.ToString()));
                if (a != null)
                    break;
            }

            return a;
        }

        /// <summary>
        /// Returns a List of TesteeRelationship by name, context.  Empty list if it doesn't exist.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public List<TesteeRelationship> GetRelationships(string name, TesteeProperty.PropertyContext context)
        {
            return GetRelationships(name, new List<TesteeProperty.PropertyContext>() { context });
        }

        /// <summary>
        /// Returns the first List of TesteeRelationships by name and context given a list of TesteeProperty.PropertyContexts
        /// in order of priority.  Will start with the first element in the list; if that context
        /// is not in the collection, it'll try the 2nd, and so on until a List of TesteeRelationships is found.  If
        /// no TesteeRelationships are found in the collection, an empty List is returned.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="contextPriority"></param>
        /// <returns></returns>
        public List<TesteeRelationship> GetRelationships(string name, List<TesteeProperty.PropertyContext> contextPriority)
        {
            List<TesteeRelationship> rels = new List<TesteeRelationship>();
            foreach (TesteeProperty.PropertyContext context in contextPriority)
            {
                rels = TesteeRelationships.Where(x => x.Context.Equals(context.ToString()) && x.Name.Equals(name)).ToList();
                if (rels.Count > 0)
                    break;
            }

            return rels;
        }

        /// <summary>
        /// Returns a single TesteeRelationship by name, context, entityKey.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="context"></param>
        /// <param name="entityKey"></param>
        /// <returns></returns>
        public TesteeRelationship GetRelationship(string name, TesteeProperty.PropertyContext context, long entityKey)
        {
            return GetRelationship(name, new List<TesteeProperty.PropertyContext>() { context }, entityKey);
        }

        /// <summary>
        /// Returns a single TesteeRelationship by name, context, entityKey.  Context is given as a list of TesteeProperty.PropertyContexts
        /// in order of priority.  Will start with the first element in the list; if that contect is not in the collection, it'll try the 2nd, 
        /// and so on until found.  If nothing is found, returns null.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="contextPriority"></param>
        /// <param name="entityKey"></param>
        /// <returns></returns>
        public TesteeRelationship GetRelationship(string name, List<TesteeProperty.PropertyContext> contextPriority, long entityKey)
        {
            List<TesteeRelationship> rels = GetRelationships(name, contextPriority);
            if (rels.Count == 0)
                return null;
            else
                return rels.Find(r => r.EntityKey == entityKey);
        }

        /// <summary>
        /// Private helper method to return the TesteeAttribute.Value for a given
        /// name, context.  String.Empty if the TesteeAttribute doesn't exist.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        private string GetAttributeValueAsString(string name, TesteeProperty.PropertyContext context)
        {
            string value = String.Empty;
            TesteeAttribute a = GetAttribute(name, context);
            if (a != null)
                value = a.Value;
            return value;
        }

        /// <summary>
        /// Encapsulate the DOB format used in RTS
        /// </summary>
        /// <param name="dob"></param>
        /// <param name="birthDate"></param>
        /// <returns></returns>
        public static bool TryParseDOB(string dob, out DateTime birthDate)
        {
            return DateTime.TryParseExact(dob, "MMddyyyy", CultureInfo.CurrentCulture, DateTimeStyles.None, out birthDate);
        }
    }
}
