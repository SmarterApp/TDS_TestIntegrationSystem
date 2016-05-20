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
using TDSQASystemAPI.Config;
using TDSQASystemAPI.TestResults;
using AIR.Common;
using System.Xml;
using System.Xml.Xsl;
using TDSQASystemAPI.ExceptionHandling;

namespace TDSQASystemAPI.Routing
{
    public abstract class Target
    {
        public enum TargetType { WebService = 0, SFTP, Custom }
        private enum Variables { Target = 0, TargetType, XMLVersion, Transform, Order }
        [Flags] public enum TargetClass { General = 1, Handscoring = 2, DoR = 4 }

        public DateTime CreateDate { get; private set; }
        public string Name { get; private set; }
        public TargetType Type { get; private set; }
        public TargetClass Class { get; private set; }
        public XMLAdapter.AdapterType XmlVersion { get; private set; }
        public FileTransformSpec TransformSpec { get; private set; }
        public IFileTransformArgs TransformArgs { get; private set; }

        protected Target()
        {
        }

        protected Target(string targetName, TargetClass targetClass, TargetType type, XMLAdapter.AdapterType xmlVersion, FileTransformSpec transformSpec, IFileTransformArgs transformArgs)
        {
            this.Name = targetName;
            this.Type = type;
            this.Class = targetClass;
            this.XmlVersion = xmlVersion;
            this.TransformSpec = transformSpec;
            this.TransformArgs = transformArgs ?? new NoFileTransformArgs();
        }

        protected Target(string targetName, TargetClass targetClass, TargetType type, XMLAdapter.AdapterType xmlVersion, FileTransformSpec transformSpec)
            : this(targetName, targetClass, type, xmlVersion, transformSpec, new NoFileTransformArgs())
        {
        }

        // use for targets that do not output a file
        protected Target(string targetName, TargetClass targetClass, TargetType type)
            : this(targetName, targetClass, type, XMLAdapter.AdapterType.OSS, null, null)
        {
        }

        private static Target Create(int projectID, string targetName)
        {
            TargetClass targetClass;
            if (!Enum.TryParse<TargetClass>(targetName, true, out targetClass))
            {
                // handscoring targets must have GroupName thats starts with Handscoring (case-insensitive)
                if (targetName.StartsWith("Handscoring", StringComparison.InvariantCultureIgnoreCase))
                    targetClass = TargetClass.Handscoring;
                else
                    targetClass = TargetClass.General;
            }

            MetaDataEntry e = ServiceLocator.Resolve<ConfigurationHolder>().GetFromMetaData(projectID, targetName, Variables.TargetType.ToString());
            
            TargetType type;
            if (e == null || !Enum.TryParse<TargetType>(e.TextVal ?? "", true, out type))
                type = TargetType.Custom;

            e = ServiceLocator.Resolve<ConfigurationHolder>().GetFromMetaData(projectID, targetName, Variables.XMLVersion.ToString());
            XMLAdapter.AdapterType xmlVersion;
            if(e == null || !Enum.TryParse<XMLAdapter.AdapterType>(e.TextVal, true, out xmlVersion))
                xmlVersion = XMLAdapter.AdapterType.TDS; // default to proprietary

            // if a transform is configured, load it
            FileTransformSpec transformSpec = null;
            e = ServiceLocator.Resolve<ConfigurationHolder>().GetFromMetaData(projectID, targetName, Variables.Transform.ToString());
            if (e != null)
            {
                string xsltName = e.TextVal;
                bool validateTransform = Convert.ToBoolean(e.IntVal); // int val can be used to enforce validation.  Default = 0/false.
                transformSpec = FileTransformSpec.Create(xsltName, validateTransform);
            }

            Target t = null;

            ITargetFactory f = ServiceLocator.Resolve<ITargetFactory>();
            if (f == null)
                throw new QAException("There is no ITargetFactory in the service repository.", QAException.ExceptionType.ConfigurationError);
            t = f.Create(targetName, targetClass, type, xmlVersion, transformSpec);
            if(t == null)
                throw new QAException(String.Format("Could not create Target: {0}", targetName), QAException.ExceptionType.ConfigurationError); 

            t.CreateDate = DateTime.Now;

            return t;
        }

        /// <summary>
        /// Send the test result to the target
        /// </summary>
        /// <param name="tr"></param>
        /// <returns>true if the result was sent; false if not (possibly due to business rules)</returns>
        public ITargetResult Send(TestResult testResult)
        {
            return Send(testResult, null);
        }

        public abstract ITargetResult Send(TestResult testResult, Action<object> outputProcessor, params object[] inputArgs);

        /// <summary>
        /// returns the test result as a serialized and transformed (if configured) XmlDocument
        /// </summary>
        /// <param name="testResult"></param>
        /// <returns></returns>
        protected XmlDocument GetPayloadAsXmlDocument(TestResult testResult)
        {
            ITestResultSerializerFactory serializerFactory = ServiceLocator.Resolve<ITestResultSerializerFactory>();
            if (serializerFactory == null)
                throw new ApplicationException("There is no ITestResultSerializerFactory registered with the ServiceLocator.");
            XmlDocument doc = testResult.ToXml(Name, serializerFactory);
            if (TransformSpec == null)
                return doc;

            doc = new XmlDocument();
            doc.LoadXml(TransformSpec.Transform(doc, testResult, TransformArgs.MakeArgumentList(testResult, this)));
            return doc;
        }

        /// <summary>
        /// returns the test result as a serialized and transformed (if configured) string
        /// </summary>
        /// <param name="testResult"></param>
        /// <returns></returns>
        protected string GetPayloadAsString(TestResult testResult)
        {
            ITestResultSerializerFactory serializerFactory = ServiceLocator.Resolve<ITestResultSerializerFactory>();
            if (serializerFactory == null)
                throw new ApplicationException("There is no ITestResultSerializerFactory registered with the ServiceLocator.");
            XmlDocument doc = testResult.ToXml(Name, serializerFactory);
            if (TransformSpec != null)
                return TransformSpec.Transform(doc, testResult, TransformArgs.MakeArgumentList(testResult, this));
            else
                return Utilities.Serialization.XmlDocumentToString(doc);
        }


        /// <summary>
        /// Get all Targets that are turned on sorted by the configured order (asc)
        ///  If order is not specified for a subset, ordered targets will come first.
        ///  If order is not specified for any target, order is non-deterministic
        /// </summary>
        /// <param name="projectID">TestResult.ProjectID</param>
        /// <returns>
        /// A list of the Targets
        /// Empty list if no Targets are configured and turned on
        /// </returns>
        public static List<Target> GetOrderedTargets(int projectID, TargetClass targetClassFlags)
        {
            List<Target> targets = new List<Target>();
            foreach (string name in GetOrderedTargetNames(projectID, targetClassFlags))
            {
                targets.Add(Create(projectID, name));
            }

            return targets;
        }

        /// <summary>
        /// Get names of all targets that are turned on sorted by the configured order (asc)
        ///  If order is not specified for a subset, ordered targets will come first.
        ///  If order is not specified for any target, order is non-deterministic
        /// </summary>
        /// <param name="projectID">TestResult.ProjectID</param>
        /// <param name="targetClassFlags">TargetClass to include.  Can use | to include multiple classes</param>
        /// <returns>
        /// A list of the groupNames, which can be used to get configuration for the target.
        /// Empty list if no targets are configured and turned on
        /// </returns>
        public static List<string> GetOrderedTargetNames(int projectID, TargetClass targetClassFlags)
        {
            List<MetaDataEntry> targetNames = ServiceLocator.Resolve<ConfigurationHolder>().GetFromMetaData(projectID, Variables.Target.ToString()).FindAll(e => e.IntVal == 1 // turned on
                && (targetClassFlags.HasFlag(TargetClass.DoR) // filter according to targetClasses specified
                        || !e.Group.Equals("DoR", StringComparison.InvariantCultureIgnoreCase))
                && (targetClassFlags.HasFlag(TargetClass.Handscoring)
                        || !e.Group.StartsWith("Handscoring", StringComparison.InvariantCultureIgnoreCase))
                && (targetClassFlags.HasFlag(TargetClass.General)
                        || e.Group.Equals("DoR", StringComparison.InvariantCultureIgnoreCase)
                        || e.Group.StartsWith("Handscoring", StringComparison.InvariantCultureIgnoreCase)));

            targetNames.Sort((e1, e2) =>
                (ServiceLocator.Resolve<ConfigurationHolder>().GetFromMetaData(projectID, e1.Group, Variables.Order.ToString()) == null
                        ? int.MaxValue
                        : ServiceLocator.Resolve<ConfigurationHolder>().GetFromMetaData(projectID, e1.Group, Variables.Order.ToString()).IntVal)
                .CompareTo(
                    ServiceLocator.Resolve<ConfigurationHolder>().GetFromMetaData(projectID, e2.Group, Variables.Order.ToString()) == null
                        ? int.MaxValue
                        : ServiceLocator.Resolve<ConfigurationHolder>().GetFromMetaData(projectID, e2.Group, Variables.Order.ToString()).IntVal));

            // special handling for legacy target config (for backward compat).  DoR first, then handscoring, then RB
            if (targetClassFlags.HasFlag(TargetClass.General))
                targetNames.InsertRange(0, ServiceLocator.Resolve<ConfigurationHolder>().GetFromMetaData(projectID, "UpdateRB").FindAll(e => e.IntVal == 1));
            if (targetClassFlags.HasFlag(TargetClass.Handscoring))
                targetNames.InsertRange(0, ServiceLocator.Resolve<ConfigurationHolder>().GetFromMetaData(projectID, "SendToHandScoring").FindAll(e => e.IntVal == 1));
            if(targetClassFlags.HasFlag(TargetClass.DoR))
                targetNames.InsertRange(0, ServiceLocator.Resolve<ConfigurationHolder>().GetFromMetaData(projectID, "UpdateDoR").FindAll(e => e.IntVal == 1));

            return targetNames.Select(e => e.Group).ToList();
        }
    }
}
