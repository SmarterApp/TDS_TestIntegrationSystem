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
using System.IO;


namespace TDSQASystemAPI.Config
{
    public class TestCollection
    {
        private Dictionary<string, TestBlueprint> _tests = new Dictionary<string, TestBlueprint>();
        private TestEnvironment _testEnv = null;

        internal void AddTestEnvironment(TestEnvironment testEnv)
        {
            _testEnv = testEnv;
        }

        internal TestEnvironment GetTestEnvironment()
        {
            return _testEnv;
        }

        internal void AddBlueprint(TestBlueprint blueprint)
        {
            if (_tests.ContainsKey(blueprint.TestName))
                _tests.Remove(blueprint.TestName);

            _tests.Add(blueprint.TestName, blueprint);
        }

        internal void AddItem(string testName, TestItem ti)
        {
            TestBlueprint blueprint = GetBlueprint(testName);
            blueprint.AddItem(ti);
        }

        internal void AddFormItem(string testName, string formName, long formKey, int position, TestItem ti)
        {
            TestBlueprint blueprint = GetBlueprint(testName);
            blueprint.AddFormItem(formName, formKey, position, ti);
        }

        internal TestBlueprint GetBlueprint(string testName)
        {
            if (_tests.ContainsKey(testName))
            {
                return _tests[testName];
            }
            return null;
        }

        public void PrintCheck(TextWriter tw)
        {
            foreach (TestBlueprint blueprint in _tests.Values)
            {
                blueprint.PrintBlueprintSummary(tw);
            }
        }
    }
}
