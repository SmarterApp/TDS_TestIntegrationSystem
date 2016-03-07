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
using TDSQASystemAPI;
using TDSQASystemAPI.TestResults;
using TDSQASystemAPI.Config;
using TDSQASystemAPI.Data;
using TDSQASystemAPI.Routing;

namespace TDSQASystemAPI
{
    public interface ITISExtender
    {
        ITISExtenderState CreateStateContainer();
        void PreScore(QASystem tis, TestResult tr, XmlRepositoryItem xmlRepoItem, ProjectMetaData projectMetaData, ITISExtenderState state);
        bool ShouldScore(QASystem tis, XMLAdapter adapter, TestResult tr, ProjectMetaData projectMetaData, ITISExtenderState state);
        void PostScore(QASystem tis, TestResult tr, XmlRepositoryItem xmlRepoItem, ProjectMetaData projectMetaData, ITISExtenderState state);
        List<ValidationRecord> Validate(QASystem tis, TestResult tr, XmlRepositoryItem xmlRepoItem, ProjectMetaData projectMetaData, ITISExtenderState state, out bool isFatal, out SendToModifiers sendToModifiers);
        void PreRoute(QASystem tis, XMLAdapter adapter, TestResult tr, XmlRepositoryItem xmlRepoItem, ProjectMetaData projectMetaData, SendToModifiers sendToModifiers, ITISExtenderState state);
        void PostRoute(QASystem tis, TestResult tr, XmlRepositoryItem xmlRepoItem, ProjectMetaData projectMetaData, ITISExtenderState state);
        void PostSave(QASystem tis, TestResult tr, XmlRepositoryItem xmlRepoItem, ProjectMetaData projectMetaData, ITISExtenderState state);
    }

    public interface ITISExtenderState { }

    public class NullTISExtender : ITISExtender
    {
        #region ITISExtender Members

        public ITISExtenderState CreateStateContainer()
        {
            return new NoTISExtenderState();
        }

        public void PreScore(QASystem tis, TestResult tr, XmlRepositoryItem xmlRepoItem, ProjectMetaData projectMetaData, ITISExtenderState state)
        {
            // do nothing
        }

        public bool ShouldScore(QASystem tis, XMLAdapter adapter, TestResult tr, ProjectMetaData projectMetaData, ITISExtenderState state)
        {
            return true;
        }

        public void PostScore(QASystem tis, TestResult tr, XmlRepositoryItem xmlRepoItem, ProjectMetaData projectMetaData, ITISExtenderState state)
        {
            // do nothing
        }

        public List<ValidationRecord> Validate(QASystem tis, TestResult tr, XmlRepositoryItem xmlRepoItem, ProjectMetaData projectMetaData, ITISExtenderState state, out bool isFatal, out SendToModifiers sendToModifiers)
        {
            isFatal = false;
            sendToModifiers = new SendToModifiers();
            return new List<ValidationRecord>();
        }

        public void PreRoute(QASystem tis, XMLAdapter adapter, TestResult tr, XmlRepositoryItem xmlRepoItem, ProjectMetaData projectMetaData, SendToModifiers sendToModifiers, ITISExtenderState state)
        {
            // do nothing
        }

        public void PostRoute(QASystem tis, TestResult tr, XmlRepositoryItem xmlRepoItem, ProjectMetaData projectMetaData, ITISExtenderState state)
        {
            // do nothing
        }

        public void PostSave(QASystem tis, TestResult tr, XmlRepositoryItem xmlRepoItem, ProjectMetaData projectMetaData, ITISExtenderState state)
        {
            // do nothing
        }

        #endregion
    }

    public class NoTISExtenderState : ITISExtenderState
    {

    }
}
