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
using System.Configuration;
using ScoringEngine.MeasurementModels;
using ScoringEngine.ConfiguredTests;
using ScoringEngine.Scoring;
using System.Data.SqlClient;
using System.Data;
using System.Reflection;

namespace ScoringEngine
{
    public enum TestMode { Online = 0, Paper = 1, ScannedPaper = 2 };

    /// <summary>
    /// Contains information about one or more tests configured in the current item bank DB.
    /// Ctor takes the following two config settings: ConnectionStrings["ItemBankConnectionString"] and AppSettings["ScoringEnvironment"]
    /// </summary>
   
    public sealed class TestCollection
    {
        private string itemBankConnectionString;
        private string environment;
        private Dictionary<string, TestBlueprint> _tests = new Dictionary<string, TestBlueprint>();
        //private Dictionary<string,List<WindowMaxOpportunity>> _windowMaxOpp = new Dictionary<string, List<WindowMaxOpportunity>>();
        // Accommodations: For each TestID, valid accommodations by type
        Dictionary<string, Dictionary<string, List<TestAccomodation>>> accommodations = new Dictionary<string, Dictionary<string, List<TestAccomodation>>>();
        // Computation rules: For each TestID a list of computations to apply (in the order specified by ComputationOrder)
        private Dictionary<string, List<ComputationSpec>> testScoring = new Dictionary<string,List<ComputationSpec>>();
        // Conversion tables: First key is conversion table name, second key is value to be converted. Returns a double.
        private Dictionary<string, Dictionary<int, double>> conversionTables = new Dictionary<string, Dictionary<int, double>>();

        public Dictionary<string, List<ComputationSpec>> TestScoring
        {
            get
            {
                return testScoring;
            }
        }

      
        internal Dictionary<string, Dictionary<int, double>> ConversionTables
        {
            get
            {
                return conversionTables;
            }
        }

        public string ItemBankConnectionString
        {
            get
            {
                return this.itemBankConnectionString;
            }
        }

        public string Environment
        {
            get
            {
                return environment;
            }
        }

        // set this in order to extend the command timeout for all DB access
        public int? DbCommandTimeout { get; set; }

        /// <summary>
        /// Creates an empty TestCollection.  Call LoadTest one or more times to
        /// load configuration data for tests you want to score.  Also call LoadConversionTables 
        /// if they're needed.
        /// </summary>
        /// <param name="itemBankConnectionString"></param>
        /// <param name="environment">Currently QA or TDS</param>
        public TestCollection(string itemBankConnectionString, string environment)
        {
            this.itemBankConnectionString = itemBankConnectionString;
            this.environment = environment;
            int numQuadraturePoints = 5;
            if (!String.IsNullOrEmpty(ConfigurationManager.AppSettings["NumQuadPointsForExpectedInfo"]))
                numQuadraturePoints = Int32.Parse(ConfigurationManager.AppSettings["NumQuadPointsForExpectedInfo"]);
            if (!((numQuadraturePoints > 2 && numQuadraturePoints < 6) || numQuadraturePoints == 7 || numQuadraturePoints == 20 || numQuadraturePoints == 21))
            {
                throw new ScoringEngineException("Configured number of quadrature points (NumQuadPointsForExpectedInfo = " + numQuadraturePoints.ToString() + ") is not valid. Only 3, 4, 5, 7, 20, or 21 is allowed");
            }
        }

        /// <summary>
        /// Creates a TestCollection instance and, optionally, loads all tests and conversion tables (for all clients)
        /// </summary>
        /// <param name="itemBankConnectionString"></param>
        /// <param name="environment"></param>
        /// <param name="loadAllTests"></param>
        /// <param name="dbCommandTimeout"></param>
        public TestCollection(string itemBankConnectionString, string environment, bool loadAllTests, int? dbCommandTimeout)
            : this(itemBankConnectionString, environment)
        {
            this.DbCommandTimeout = dbCommandTimeout;
            if (loadAllTests)
            {
                LoadAllTests("");
            }
        }

        /// <summary>
        /// Creates a TestCollection instance and, optionally, loads all tests and conversion tables (for all clients)
        /// </summary>
        /// <param name="itemBankConnectionString"></param>
        /// <param name="environment"></param>
        /// <param name="loadAllTests"></param>
        public TestCollection(string itemBankConnectionString, string environment, bool loadAllTests) 
            : this(itemBankConnectionString, environment, loadAllTests, null)
        {
        }

        /// <summary>
        /// Creates a TestCollection instance and automatically loads all tests and conversion tables 
        /// for the specified client.
        /// </summary>
        /// <param name="itemBankConnectionString"></param>
        /// <param name="environment">Currently QA or TDS</param>
        /// <param name="clientName">from Item Bank</param>
        public TestCollection(string itemBankConnectionString, string environment, string clientName) 
            : this(itemBankConnectionString, environment, clientName, null)
        {
        }

        /// <summary>
        /// Creates a TestCollection instance and automatically loads all tests and conversion tables 
        /// for the specified client.
        /// </summary>
        /// <param name="itemBankConnectionString"></param>
        /// <param name="environment">Currently QA or TDS</param>
        /// <param name="clientName">from Item Bank</param>
        /// <param name="dbCommandTimeout">set the command timeout used for db reads</param>
        public TestCollection(string itemBankConnectionString, string environment, string clientName, int? dbCommandTimeout)
            : this(itemBankConnectionString, environment)
        {
            this.DbCommandTimeout = dbCommandTimeout;
            LoadAllTests(clientName);
        }

        /// <summary>
        /// Loads configuration data for the specified test
        /// </summary>
        /// <param name="testName"></param>
        public void LoadTest(string testName)
        {
            if (String.IsNullOrEmpty(testName))
            {
                throw new ScoringEngineException("testName cannot be null or empty.");
            }

            if (HasTest(testName))
            {
                throw new ScoringEngineException(String.Format("Test '{0}' has already been loaded.", testName));
            }

            try
            {
                using (SqlConnection itemBankConnection = new SqlConnection(itemBankConnectionString))
                {
                    itemBankConnection.Open();

                    LoadBlueprint(itemBankConnection, testName);
                    LoadTestItems(itemBankConnection, testName);
                    LoadForms(itemBankConnection, testName);
                    LoadSegments(itemBankConnection, testName);
                    LoadAccomodations(itemBankConnection, testName);
                    LoadComputationRules(itemBankConnection, environment, testName);
                    //LoadWindowMaxOpportunity(itemBankConnection, testName);
                    //LoadScoringSpec(itemBankConnection, environment, testName);   AM: not loading conversion tables implicitly
                }
            }
            catch (ScoringEngineException se)
            {
                throw se;
            }
            catch (SqlException sqlEx)
            {
                
                throw new ScoringEngineException(String.Format("A SQL exception occured while attempting to access the item bank for test '{0}'.  Message: {1}", testName, sqlEx.Message), sqlEx);
            }
            catch (Exception e)
            {
                throw new ScoringEngineException(String.Format("An exception occured while loading test: {0}.  Message: {1}", testName, e.Message), e);
            }
        }

        public bool HasTest(string testName)
        {
            if (_tests.ContainsKey(testName))
                return true;
            // check for bad casing
            foreach (string knownTest in _tests.Keys)
                if (testName.Equals(knownTest, StringComparison.InvariantCultureIgnoreCase))
                    throw new ScoringEngineException("Problem with testname case: looking for testName " + testName + ", and we have " + knownTest + ", but we can only deal with this case sensitively");
            return false;
        }

        /// <summary>
        /// Original signature so as not to break existing clients; will load conversion tables for all clients.
        /// </summary>
        [Obsolete("Use overload that takes clientName.  This version will load for all clients.", false)]
        public void LoadConversionTables()
        {
            LoadConversionTables(String.Empty);
        }

        /// <summary>
        /// Loads conversion tables on demand
        /// TODO: this may go away if we add a mapping table between conversion tables
        /// and tests.  Then we can load the conversion tables when the test is loaded.
        /// </summary>
        public void LoadConversionTables(string clientName)
        {
            try
            {
                using (SqlConnection itemBankConnection = new SqlConnection(itemBankConnectionString))
                {
                    itemBankConnection.Open();
                    LoadConversionTables(itemBankConnection, clientName);
                }
            }
            catch (ScoringEngineException se)
            {
                throw se;
            }
            catch (SqlException sqlEx)
            {
                throw new ScoringEngineException(String.Format("A SQL exception occured while attempting to load the conversion tables with connection string '{0}'.  Message: {1}", itemBankConnectionString, sqlEx.Message), sqlEx);
            }
            catch (Exception e)
            {
                throw new ScoringEngineException(String.Format("An exception occured while loading the conversion tables.  Message: {0}", e.Message), e);
            }
        }

        /// <summary>
        /// Loads configuration data for all tests in the item bank
        /// </summary>
        private void LoadAllTests(string clientName)
        {
            DataTable testNames = new DataTable();

            try
            {
                // get all test names to load
                using (SqlConnection itemBankConnection = new SqlConnection(itemBankConnectionString))
                {
                    string query = @"select distinct t.TestName from qa.Test t";
                    if (!String.IsNullOrEmpty(clientName))
                    {
                        query = String.Format("{0} inner join TDSCONFIGS_Client_TestMode m on m.testkey = t.testName and m.ClientName='{1}'", query, clientName);
                    }
                    query = String.Format("{0} order by t.TestName", query);
                    
                    SqlDataAdapter da = new SqlDataAdapter();
                    da.SelectCommand = new SqlCommand(query, itemBankConnection);
                    if(DbCommandTimeout != null)
                        da.SelectCommand.CommandTimeout = DbCommandTimeout.Value;
                    da.Fill(testNames);
                }

                // load each test one by one
                foreach (DataRow dr in testNames.Rows)
                {
                    LoadTest(dr["TestName"].ToString());
                }

                // also load the conversion tables
                LoadConversionTables(clientName);
            }
            catch (ScoringEngineException se)
            {
                throw se;
            }
            catch (SqlException sqlEx)
            {
                throw new ScoringEngineException(String.Format("A SQL exception occured while attempting to load all tests with connection string '{0}'.  Message: {1}", itemBankConnectionString, sqlEx.Message), sqlEx);
            }
            catch (Exception e)
            {
                throw new ScoringEngineException(String.Format("An exception occured while loading all tests.  Message: {0}", e.Message), e);
            }
        }

        public void DeleteTest(string testName)
        {
            if (String.IsNullOrEmpty(testName))
            {
                throw new ScoringEngineException("DeleteTest: testName cannot be null or empty.");
            }

            if (!HasTest(testName))
            {
                throw new ScoringEngineException(String.Format("Test '{0}' hasn't been loaded and therefore can't be dropped.", testName));
            }
            
            if(TestScoring.ContainsKey(testName))
                TestScoring.Remove(testName);
            if (accommodations.ContainsKey(testName))
                accommodations.Remove(testName);
            _tests.Remove(testName);
        }

        //AM: computation rules are loaded per test, but conversion tables will be loaded
        //  on demand by the client.  See comment with LoadConversionTables method for
        //  more info.
        //private void LoadScoringSpec(SqlConnection itemBankConnection, string environment, string testName)
        //{
        //    LoadComputationRules(itemBankConnection, environment, testName);
        //    LoadConversionTables(itemBankConnection);
        //}

        private void LoadComputationRules(SqlConnection itemBankConnection, string environment, string testName)
        {
            SqlDataAdapter adapter = new SqlDataAdapter();
            SqlCommand cmd = new SqlCommand("qa.ComputationRulesByTest", itemBankConnection);
            if (DbCommandTimeout != null)
                cmd.CommandTimeout = DbCommandTimeout.Value;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@ComputationLocation", SqlDbType.VarChar, 32).Value = environment;
            cmd.Parameters.Add("@TestName", SqlDbType.VarChar, 255).Value = testName;
            adapter.SelectCommand = cmd;
            DataTable scoringTable = new DataTable();
            adapter.Fill(scoringTable);
            int lastComputationOrder = -1;
            foreach (DataRow dr in scoringTable.Rows)
            {
                string testID = Util.Value(dr["TestID"],"");
                int computationOrder = Util.Value(dr["ComputationOrder"],-1);
                string measureLabel = Util.Value(dr["MeasureLabel"],"");
                string measureOf = Util.Value(dr["MeasureOf"],"");
                string computationRule = Util.Value(dr["ComputationRule"],"");

                // I don't think this can happen, but...
                if (testID != testName)
                    throw new ScoringEngineException("Case problem in LoadComputationRules! " + testID + " != " + testName);
 
                if (!testScoring.ContainsKey(testID))
                {
                    testScoring[testID] = new List<ComputationSpec>();
                    lastComputationOrder = -1;
                }
                ComputationSpec spec = null;
                if (computationOrder == lastComputationOrder)
                {
                    spec = testScoring[testID][testScoring[testID].Count-1];
                    if (spec.MeasureOf != measureOf || spec.MeasureLabel != measureLabel || spec.ComputationRule != computationRule)
                        throw new ScoringEngineException("All 'MeasureOf', 'MeasureLabel', and 'ComputationRule' values must be the same for all records with the same 'ComputationOrder', this is not the case for TestKey = " + testID + " and computation order " + computationOrder.ToString());
                }
                else
                {
                    spec = new ComputationSpec(measureLabel, measureOf, computationRule);
                    testScoring[testID].Add(spec);
                }
                spec.AddParameter(
                    Util.Value(dr["ParameterPosition"], -1),
                    Util.Value(dr["IndexType"], ""),
                    Util.Value(dr["Index"], ""),
                    Util.Value(dr["Type"], ""),
                    Util.Value(dr["Value"], ""));
                lastComputationOrder = computationOrder;
            }

            adapter = new SqlDataAdapter();
            cmd = new SqlCommand("qa.ComputationLocationsByTest", itemBankConnection);
            if (DbCommandTimeout != null)
                cmd.CommandTimeout = DbCommandTimeout.Value;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@TestName", SqlDbType.VarChar, 255).Value = testName;
            adapter.SelectCommand = cmd;
            DataTable computationLocationTable = new DataTable();
            adapter.Fill(computationLocationTable);
            foreach (DataRow dr in computationLocationTable.Rows)
            {
                string measureLabel = Util.Value(dr["MeasureLabel"], "");
                string measureOf = Util.Value(dr["MeasureOf"], "");
                string computationLocation = Util.Value(dr["ComputationLocation"], "");
                if (testScoring.ContainsKey(testName))
                {
                    foreach (ComputationSpec spec in testScoring[testName])
                    {
                        if (spec.MeasureOf == measureOf && spec.MeasureLabel == measureLabel)
                            spec.AddComputationLocation(computationLocation);
                    }
                }
            }

            CheckScoringSpecs(testScoring);
        }

        private void CheckScoringSpecs(Dictionary<string,List<ComputationSpec>> testScoring)
        {
            foreach (List<ComputationSpec> specs in testScoring.Values)
            {
                foreach (ComputationSpec spec in specs)
                {
                    Type classType = typeof(SpecificComputationRules);
                    MethodInfo computationMethod = classType.GetMethod(spec.ComputationRule);
                    if (computationMethod == null)
                        throw new ScoringEngineException("No method called " + spec.ComputationRule + " in the SpecificComputationRules class");
                    int maxParameterPosition = 0;
                    foreach (int parameterPosition in spec.ParameterValues.Keys)
                        if (parameterPosition > maxParameterPosition) maxParameterPosition = parameterPosition;

                    ParameterInfo[] pi = computationMethod.GetParameters();
                    if (pi.Length - 2 != maxParameterPosition)
                        throw new ScoringEngineException("Computation rule " + spec.ComputationRule + " needs " + Convert.ToString(pi.Length - 2) + " parameter values to be specified, but there is a ParameterPosition = " + maxParameterPosition);
                    for (int i = 2; i < pi.Length; i++)
                    {
                        if (!spec.ParameterValues.ContainsKey(i-1))
                            throw new ScoringEngineException("Computation rule " + spec.ComputationRule + " has no ParameterPosition = " + Convert.ToString(i-1));
                        List<ParameterValue> par = spec.ParameterValues[i-1];
                        if (par[0].IndexType == "")
                        {
                            if (par.Count != 1)
                                throw new ScoringEngineException("Parameter " + Convert.ToString(i-1) + " looks like a simple type so only one value can be given. Computation rule " + spec.ComputationRule);
                            if (par[0].Index != "")
                                throw new ScoringEngineException("Parameter " + Convert.ToString(i-1) + " looks like a simple type so 'Index' value must be the empty string. Computation rule " + spec.ComputationRule);
                            switch (par[0].Type.ToLower())
                            {
                                case "int":
                                    if (pi[i].ParameterType != typeof(int))
                                        throw new ScoringEngineException("Parameter " + Convert.ToString(i-1) + ", type int, does not match the argument of computation rule " + spec.ComputationRule);
                                    int dum;
                                    if (!Int32.TryParse(par[0].Value, out dum))
                                        throw new ScoringEngineException("Can't convert " + par[0].Value + " to an integer. Parameter " + Convert.ToString(i-2) + ", computation rule " + spec.ComputationRule);
                                    break;
                                case "double":
                                    if (pi[i].ParameterType != typeof(double))
                                        throw new ScoringEngineException("Parameter " + Convert.ToString(i-1) + ", type double, does not match the argument of computation rule " + spec.ComputationRule);
                                    double dum2;
                                    if (!double.TryParse(par[0].Value, out dum2))
                                        throw new ScoringEngineException("Can't convert " + par[0].Value + " to a double. Parameter " + Convert.ToString(i-2) + ", computation rule " + spec.ComputationRule);
                                    break;
                                case "string":
                                    if (pi[i].ParameterType != typeof(string))
                                        throw new ScoringEngineException("Parameter " + Convert.ToString(i-1) + ", type string, does not match the argument of computation rule " + spec.ComputationRule);
                                    break;
                                case "measurelabel":
                                    if (pi[i].ParameterType != typeof(string))
                                        throw new ScoringEngineException("Parameter " + Convert.ToString(i-1) + ", has type 'MeasureLabel', but the corresponding argument of computation rule " + spec.ComputationRule + " does not have type 'string'");
                                    if (par[0].Value != "")
                                        throw new ScoringEngineException("Parameter " + Convert.ToString(i-1) + ", has type 'MeasureLabel', so the value must be the empty string but it is '" + par[0].Value + "'. Computation rule " + spec.ComputationRule);
                                    break;
                                default:
                                    throw new ScoringEngineException("Parameter " + Convert.ToString(i-1) + ", must have type 'int', 'double', 'string' or 'MeasureType'. Type '" + par[0].Type + " is not valid. Parameter " + par[0].ParameterPosition + ", computation rule " + spec.ComputationRule);
                            }
                        }
                        else if (par[0].IndexType.ToLower() == "int")
                        {
                            string type = par[0].Type.ToLower();
                            switch (type)
                            {
                                case "int":
                                    if (pi[i].ParameterType != typeof(Dictionary<int, int>))
                                        throw new ScoringEngineException("Argument " + i.ToString() + " does not have type Dictionary<int, int>, computation rule " + spec.ComputationRule);
                                    break;
                                case "double":
                                    if (pi[i].ParameterType != typeof(Dictionary<int, double>))
                                        throw new ScoringEngineException("Argument " + i.ToString() + " does not have type Dictionary<int, double>, computation rule " + spec.ComputationRule);
                                    break;
                                case "string":
                                    if (pi[i].ParameterType != typeof(Dictionary<int, string>))
                                        throw new ScoringEngineException("Argument " + i.ToString() + " does not have type Dictionary<int, string>, computation rule " + spec.ComputationRule);
                                    break;
                                default:
                                    throw new ScoringEngineException("Parameter " + Convert.ToString(i-1) + " dictionary values can only be of type 'int', 'double' or 'string', type: " + type + " is not valid. Computation rule " + spec.ComputationRule);
                            }
                            foreach (ParameterValue pr in par)
                            {
                                if (pr.IndexType.ToLower() != "int")
                                    throw new ScoringEngineException("All IndexTypes for parameter " + Convert.ToString(i-1) + " must be 'int'. Computation rule " + spec.ComputationRule);
                                int dum;
                                if (!int.TryParse(pr.Index, out dum))
                                    throw new ScoringEngineException("Can't parse " + pr.Index + " as an integer. ParameterPosition " + pr.ParameterPosition + ", computation rule " + spec.ComputationRule);
                                if (pr.Type.ToLower() != type)
                                    throw new ScoringEngineException("All Types for parameter " + Convert.ToString(i-1) + " must be the same. Computation rule " + spec.ComputationRule);
                                switch (pr.Type.ToLower())
                                {
                                    case "int":
                                        if (!Int32.TryParse(pr.Value, out dum))
                                            throw new ScoringEngineException("Can't convert " + pr.Value + " to an integer. Parameter " + Convert.ToString(i-2) + ", computation rule " + spec.ComputationRule);
                                        break;
                                    case "double":
                                        double dum2;
                                        if (!double.TryParse(pr.Value, out dum2))
                                            throw new ScoringEngineException("Can't convert " + pr.Value + " to a double. Parameter " + Convert.ToString(i-2) + ", computation rule " + spec.ComputationRule);
                                        break;
                                    case "string":
                                        break;
                                    default:
                                        throw new ScoringEngineException("Parameter " + Convert.ToString(i-1) + ", must have type 'int', 'double', 'string' or 'MeasureType'. Type '" + pr.Type + " is not valid. Parameter " + pr.ParameterPosition + ", computation rule " + spec.ComputationRule);
                                }
                            }
                        }
                        else if (par[0].IndexType.ToLower() == "string")
                        {
                            string type = par[0].Type.ToLower();
                            switch (type)
                            {
                                case "int":
                                    if (pi[i].ParameterType != typeof(Dictionary<string, int>))
                                        throw new ScoringEngineException("Argument " + i.ToString() + " does not have type Dictionary<string, int>, computation rule " + spec.ComputationRule);
                                    break;
                                case "double":
                                    if (pi[i].ParameterType != typeof(Dictionary<string, double>))
                                        throw new ScoringEngineException("Argument " + i.ToString() + " does not have type Dictionary<string, double>, computation rule " + spec.ComputationRule);
                                    break;
                                case "string":
                                    if (pi[i].ParameterType != typeof(Dictionary<string, string>))
                                        throw new ScoringEngineException("Argument " + i.ToString() + " does not have type Dictionary<string, string>, computation rule " + spec.ComputationRule);
                                    break;
                                default:
                                    throw new ScoringEngineException("Parameter " + Convert.ToString(i-1) + " dictionary values can only be of type 'int', 'double' or 'string', type: " + type + " is not valid. Computation rule " + spec.ComputationRule);
                            }
                            foreach (ParameterValue pr in par)
                            {
                                if (pr.IndexType.ToLower() != "string")
                                    throw new ScoringEngineException("All IndexTypes for parameter " + Convert.ToString(i-1) + " must be 'int'. Computation rule " + spec.ComputationRule);
                                if (pr.Type.ToLower() != type)
                                    throw new ScoringEngineException("All Types for parameter " + Convert.ToString(i-1) + " must be the same. Computation rule " + spec.ComputationRule);
                                switch (pr.Type.ToLower())
                                {
                                    case "int":
                                        int dum;
                                        if (!Int32.TryParse(pr.Value, out dum))
                                            throw new ScoringEngineException("Can't convert " + pr.Value + " to an integer. Parameter " + Convert.ToString(i-2) + ", computation rule " + spec.ComputationRule);
                                        break;
                                    case "double":
                                        double dum2;
                                        if (!double.TryParse(pr.Value, out dum2))
                                            throw new ScoringEngineException("Can't convert " + pr.Value + " to a double. Parameter " + Convert.ToString(i-2) + ", computation rule " + spec.ComputationRule);
                                        break;
                                    case "string":
                                        break;
                                    default:
                                        throw new ScoringEngineException("Parameter " + Convert.ToString(i-1) + ", must have type 'int', 'double', 'string' or 'MeasureType'. Type '" + pr.Type + " is not valid. Parameter " + pr.ParameterPosition + ", computation rule " + spec.ComputationRule);
                                }
                            }
                        }
                        else
                        {
                            throw new ScoringEngineException("Value " + par[0].IndexType + " for IndexType is not valid, it must be the empty string, 'int' or 'string'. Parameter " + Convert.ToString(i-2) + ", must have type 'int', 'double', 'string' or 'MeasureType'. Type '" + par[0].Type + " is not valid. Parameter " + par[0].ParameterPosition + ", computation rule " + spec.ComputationRule);
                        }
                    }
                }
            }
        }

        private void LoadConversionTables(SqlConnection itemBankConnection, string clientName)
        {
            SqlDataAdapter adapter = new SqlDataAdapter();
            string query = @"select * from qa.ConversionTables";
            if (!String.IsNullOrEmpty(clientName))
            {
                query = String.Format("{0} where ClientName='{1}'", query, clientName);
            }
            SqlCommand cmd = new SqlCommand(query, itemBankConnection);
            if (DbCommandTimeout != null)
                cmd.CommandTimeout = DbCommandTimeout.Value;
            adapter.SelectCommand = cmd;
            DataTable ctTable = new DataTable();
            adapter.Fill(ctTable);
            foreach (DataRow dr in ctTable.Rows)
            {
                string ctName = Util.Value(dr["TableName"],"");
                int inValue = Util.Value(dr["InValue"],-1);
                double outValue = Util.Value(dr["OutValue"],0.0);
                if (!conversionTables.ContainsKey(ctName))
                    conversionTables[ctName] = new Dictionary<int,double>();
                if (conversionTables[ctName].ContainsKey(inValue))
                    throw new ScoringEngineException("Conversion table " + ctName + " has multiple values for 'in' value " + inValue.ToString());
                conversionTables[ctName][inValue] = outValue;
            }
        }


//        private void LoadWindowMaxOpportunity(SqlConnection itemBankConnection, string testname)
//        {
//            SqlDataAdapter windowOpportunityAdapter = new SqlDataAdapter();
//            DataTable windopOpportunityTable = new DataTable();
//            //Loads the WindowMaxOpportunity for the given test
//            string query = string.Format(@"select CT.numopps as [WindowMaxOpportunityNo], CT.TestId as [TestID], CT.WindowID as [WindowID] 
//                                            from TDSCONFIGS_Client_TestWindow CT 
//                                            join dbo.tblSetofAdminSubjects SA 
//                                            on CT.TestID=SA.TestID where SA.[_Key]='{0}'", testname);
//            windowOpportunityAdapter.SelectCommand = new SqlCommand(query, itemBankConnection);
//            windowOpportunityAdapter.Fill(windopOpportunityTable);
//            foreach (DataRow dr in windopOpportunityTable.Rows)
//            {
//                if (!_windowMaxOpp.ContainsKey(testname))
//                {
//                    _windowMaxOpp[testname] = new List<WindowMaxOpportunity>();

//                }
//                WindowMaxOpportunity opportunity = null;
//                opportunity = new WindowMaxOpportunity(Util.Value(dr["TestId"], "Unnamed"),
//                                                       Util.Value(dr["WindowMaxOpportunityNo"], 0),
//                                                       Util.Value(dr["WindowID"], "Unnamed"));
//                _windowMaxOpp[testname].Add(opportunity);
//            }

//        }


        private void LoadBlueprint(SqlConnection itemBankConnection, string testName)
        {
            string query = String.Format("select * from qa.Test where TestName = '{0}'", testName);
            SqlDataAdapter blueprintsAdapter = new SqlDataAdapter();
            blueprintsAdapter.SelectCommand = new SqlCommand(query, itemBankConnection);
            if (DbCommandTimeout != null)
                blueprintsAdapter.SelectCommand.CommandTimeout = DbCommandTimeout.Value;
            DataTable blueprintsTable = new DataTable();
            blueprintsAdapter.Fill(blueprintsTable);
            foreach (DataRow dr in blueprintsTable.Rows)
            {
                TestBlueprint blueprint = new TestBlueprint(Util.Value(dr["TestName"], "Unnamed"),
                                                            Util.Value(dr["TestID"], "Unnamed"),
                                                            Util.Value(dr["Subject"], ""),
                                                            Util.Value(dr["GradeCode"], ""),
                                                            Util.Value(dr["GradeSpan"], ""),                                                
                                                            Util.Value(dr["MaxItems"], 0),
                                                            Util.Value(dr["MinItems"], 0),
                                                            Util.Value(dr["StartDifficultyMax"], 0.0),
                                                            Util.Value(dr["StartDifficultyMin"], 0.0),
                                                            Util.Value(dr["FieldTestMinNum"], 0),
                                                            Util.Value(dr["FieldTestMaxNum"], 0),
                                                            Util.Value(dr["FieldTestStartsAfter"], 0),
                                                            Util.Value(dr["FieldTestEndsBefore"], 0),
                                                            Util.Value(dr["FTStartDate"], DateTime.MaxValue),
                                                            Util.Value(dr["FTEndDate"], DateTime.MaxValue),
                                                            Util.Value(dr["Slope"], 1.0),
                                                            Util.Value(dr["Intercept"], 0.0),
                                                            Util.Value(dr["MaxOpportunities"], 0),
                                                            Util.Value(dr["SelectionAlgorithm"], TestBlueprint.SelectionAlgorithmType.Adaptive));

                if (HasTest(blueprint.TestName))
                    throw new ScoringEngineException("Already loaded a blueprint for this test!");
                _tests[blueprint.TestName] = blueprint;

                LoadBlueprintConstraints(blueprint, itemBankConnection);
            }
        }

        private void LoadBlueprintConstraints(TestBlueprint blueprint, SqlConnection itemBankConnection)
        {
            DataTable testFeatureTable = GetTableForTest(@"qa.TestFeature_Constraints", blueprint.TestName, itemBankConnection);
            foreach (DataRow dr in testFeatureTable.Rows)
            {
                blueprint.AddFeatureSpecification(Util.Value(dr["_fk_Feature"], ""),
                                                  Util.Value(dr["_fk_FeatureValue"], ""),
                                                  Util.Value(dr["MinNum"], 0),
                                                  Util.Value(dr["MaxNum"], 0),
                                                  Util.Value(dr["StartAbility"], 0.0),
                                                  Util.Value(dr["StartInformation"], 0.0),
                                                  Util.Value(dr["CutAbility"], 0.0),
                                                  Util.Value(dr["LambdaMultiplier"], 0.0),
                                                  Util.Value(dr["ScaledCutScore"], Double.NaN),
                                                  Util.Value(dr["TestSegment"], ""));
            }

            DataTable performanceLevelsTable = GetTableForTest(@"qa.Test_PerformanceLevels", blueprint.TestName, itemBankConnection);
            foreach (DataRow dr in performanceLevelsTable.Rows)
            {
                blueprint.AddTestPerformanceLevels(
                    Util.Value(dr["Domain"], ""),
                    Util.Value(dr["ScaledLo"], 0.0),
                    Util.Value(dr["ScaledHi"], 0.0),
                    Util.Value(dr["TestName"], ""),
                    Util.Value(dr["PLevel"], "")
                    );
            }
        }

        private void LoadTestItems(SqlConnection itemBankConnection, string testName)
        {
            DataTable          itemsTable = GetTableForTest("qa.Load_ItemsByTest", testName, itemBankConnection);
            DataTable      dimensionsTable = GetTableForTest("qa.Item_DimensionsByTest", testName, itemBankConnection);
            DataTable itemFeaturesTable = GetTableForTest("qa.ItemFeatureValuesByTest", testName, itemBankConnection);
            DataTable itemParametersTable = GetTableForTest("qa.ItemMeasurementParametersByTest", testName, itemBankConnection);
            DataTable itemStimuliTable = GetTableForTest("qa.StimuliByTest", testName, itemBankConnection);

            foreach (DataRow dr in itemsTable.Rows)
            {
                int bank = Util.Value(dr["_fk_AIRBank"], 0);
                int itsID = Util.Value(dr["_fk_ItemName"], -1);
                string stimulusCompositeKey = Util.Value(dr["StimulusID"], string.Empty);

                TestItem ti = new TestItem(bank,
                                           itsID,
                                           Util.Value(dr["Strand"], ""),
                                           Util.Value(dr["Active"], true),
                                           Util.Value(dr["Required"], true),
                                           Util.Value(dr["Scored"], true),
                                           Util.Value(dr["notForScoring"], false),
                                           Util.Value(dr["FieldTest"], false),
                                           Util.Value(dr["ParametersOnTestScale"], true),
                                           Util.Value(dr["Answer"], ""),
                                           -1,
                                           Util.Value(dr["TestSegment"], ""));

                LoadItemDimensions(testName, ti, dimensionsTable);
                LoadItemFeatures(testName, ti, itemFeaturesTable);
                LoadItemParameters(testName, ti, itemParametersTable);
                LoadItemStimuli(testName, stimulusCompositeKey, ti, itemStimuliTable);

                if (HasTest(testName))
                {
                    TestBlueprint blueprint = _tests[testName];
                    ti.RescaleParameters(blueprint.Slope, blueprint.Intercept);
                    _tests[testName].AddItem(ti);
                }
                else
                    throw new ScoringEngineException("Don't know test " + testName + " for item " + ti.ItemName);
            }
        }

        private void LoadItemDimensions(string testName, TestItem ti, DataTable dimensionsTable)
        {
            DataRow[] drItemDimensions = dimensionsTable.Select(
                string.Format("_fk_testName = '{0}' AND _fk_AIRBank = {1} AND _fk_ItemName = '{2}'",
                    testName, ti.ItemBank, ti.ItemID));
            foreach (DataRow dr in drItemDimensions)
            {
                string recodeRule = Util.Value(dr["RecodeRule"], "");
                // Don't apply recode rules when in the simulator: already recoded scores are simulated.
                if (environment == "SIM") recodeRule = "";
                ti.SetDimension(
                    Util.Value(dr["Dimension"], ""),
                    (IRTModelFactory.Model)Util.Value(dr["_fk_MeasurementModel"], 0),
                    recodeRule,
                    Util.Value(dr["Weight"], 0.0),
                    Util.Value(dr["ScorePoints"], 0),
                    Util.Value(dr["parameterCount"], -1)
                );
            }
        }

        private void LoadItemFeatures(string testName, TestItem ti, DataTable itemFeaturesTable)
        {
            DataRow[] drItemFeatures = itemFeaturesTable.Select(
                string.Format("_fk_testName = '{0}' AND _fk_AIRBank = {1} AND _fk_ItemName = '{2}'",
                    testName, ti.ItemBank, ti.ItemID));
            foreach (DataRow dr in drItemFeatures)
            {
                ti.SetFeature(
                        Util.Value(dr["Dimension"], ""),
                        (IRTModelFactory.Model)Util.Value(dr["_fk_MeasurementModel"], 0),
                        Util.Value(dr["FeatureName"], "Unknown"),
                        Util.Value(dr["FeatureValue"], "UnknownValue")
                    );
            }
        }

        private void LoadItemParameters(string testName, TestItem ti, DataTable itemParametersTable)
        {
            ti.SetParameters(testName, itemParametersTable);
        }

        private void LoadItemStimuli(string testName, string stimulusKey, TestItem ti, DataTable itemStimuliTable)
        {
            if (stimulusKey.Length > 0)
            {
                DataRow[] drItemStimuli = itemStimuliTable.Select(
                    string.Format("TestID = '{0}' AND TestSegment = '{1}' AND StimulusID = '{2}'", testName, ti.TestSegment, stimulusKey));
                int cnt = 0;
                foreach (DataRow dr in drItemStimuli)
                {
                    ti.StimulusInfo = new Stimulus(stimulusKey, ti.ItemBank, Util.Value(dr["numItemsRequired"], -1));
                    cnt++;
                }
                if (cnt != 1)
                    throw new ScoringEngineException("Did not find stimuli information for item " + ti.ItemName + " and ID " + stimulusKey + " (on segment " + ti.TestSegment + ")");
            }
        }

        private void LoadForms(SqlConnection itemBankConnection, string testName)
        {
            DataTable itemsTable = GetTableForTest("qa.TestFormItemsByTest", testName, itemBankConnection);
            foreach (DataRow dr in itemsTable.Rows)
            {
                long itemBank = Util.Value(dr["_fk_AIRBank"], 0);
                long itemKey = Util.Value(dr["_fk_ItemName"], -1L);
                string formName = Util.Value(dr["FormID"], "Unnamed");
                long formKey = Util.Value(dr["_efk_ITSFormKey"], -1L);
                int position = Util.Value(dr["FormPosition"], -1);
                DateTime startDate = Util.Value(dr["startDate"], DateTime.MinValue);
                DateTime endDate = Util.Value(dr["endDate"], DateTime.MaxValue);

                string itemName = itemBank.ToString() + "-" + itemKey.ToString();
                if (HasTest(testName))
                    _tests[testName].AddFormItem(itemBank, formName, formKey, startDate, endDate, position, _tests[testName].GetItem(itemName));
                else
                    throw new Exception("Don't know test " + testName + " for item " + itemKey + " on form " + formName);
            }
        }

        private void LoadSegments(SqlConnection itemBankConnection, string testName)
        {
            // We should know about the test
            if (!HasTest(testName))
                throw new ApplicationException("Don't know test " + testName + " to get segments"); 

            // Get the blueprint
            TestBlueprint testBlueprint = _tests[testName];

            // Get the segments/forms for the test
            DataTable segmentsTable = GetTableForTest("qa.SegmentsByTest ", testName, itemBankConnection);
            foreach (DataRow dr in segmentsTable.Rows)
            {
                string segmentName = Util.Value(dr["SegmentName"], "");
                if (string.IsNullOrEmpty(segmentName))
                    throw new ApplicationException("Load Segments: Segment name should be defined");

                // Form related info are unique in records
                string formId = Util.Value(dr["FormId"], ""); 

                // If the segment is already loaded - we just need to add the form information
                SegmentBlueprint segmentBlueprint = testBlueprint.GetSegment(segmentName); 
                if (segmentBlueprint == null)
                {
                    string segmentID = Util.Value(dr["SegmentID"], "");
                    int maxItems = Util.Value(dr["MaxItems"], 0);
                    int minItems = Util.Value(dr["MinItems"], 0);
                    int maxNumFieldTest = Util.Value(dr["FTMaxItems"], 0);
                    int minNumFieldTest = Util.Value(dr["FTMinItems"], 0);
                    int fieldTestStartPosition = Util.Value(dr["FTStartPos"], 0);
                    int fieldTestEndPosition = Util.Value(dr["FTEndPos"], 0);
                    int testPosition = Util.Value(dr["TestPosition"], 1);
                    string selectionAlgorithm = Util.Value(dr["selectionAlgorithm"], "");                                
                    // Construct a new Segment blueprint object and add it the repository
                    segmentBlueprint = new SegmentBlueprint(segmentName, segmentID, maxItems, minItems, maxNumFieldTest, minNumFieldTest,
                                                fieldTestStartPosition, fieldTestEndPosition, testPosition, selectionAlgorithm);
                    testBlueprint.AddSegment(segmentBlueprint);
                }

                // Link-up forms of the segments, if they are defined
                // All forms related to tests are already loaded
                if (!string.IsNullOrEmpty(formId))
                {
                    TestForm form = testBlueprint.GetForm(formId);
                    if (form != null)
                        segmentBlueprint.AddTestForm(form);
                    else
                        throw new ApplicationException("Form " + formId + " associated with segment " + segmentName + " does not exist");
                }
            }
        }

        private void LoadAccomodations(SqlConnection itemBankConnection, string testName)
        {
            DataTable accommodationTable = GetTableForTest("qa.GetTestAccommodations", testName, itemBankConnection);

            // get all accommodations to load for this test
			// string query = @"select AccCode as Code, AccType as Type, AccValue as Description, AllowCombine as MultiSelection, IsDefault from dbo.TestAccommodations('" + testName + "')";
            /*
            string query = @"exec dbo.TestAccommodations '" + testName + "'";
            SqlDataAdapter da = new SqlDataAdapter();
            da.SelectCommand = new SqlCommand(query, itemBankConnection);
            da.Fill(accommodationTable);
            */

            TestAccomodation testAcc = null;
            foreach (DataRow drAcc in accommodationTable.Rows)
            {
                if (!accommodations.ContainsKey(testName))
                    accommodations.Add(testName, new Dictionary<string, List<TestAccomodation>>());
                testAcc = new TestAccomodation(drAcc["Type"].ToString(), drAcc["Description"].ToString(), drAcc["Code"].ToString(), Convert.ToInt32(drAcc["Segment"]), string.Empty, Util.Value(drAcc["MultiSelection"], -1));
                if (accommodations[testName].ContainsKey(testAcc.Type))
                {
                    accommodations[testName][testAcc.Type].Add(testAcc);
                }
                else
                {
                    List<TestAccomodation> lstacc = new List<TestAccomodation>();
                    lstacc.Add(testAcc);
                    accommodations[testName].Add(testAcc.Type, lstacc);
                }
            }
        }

        private DataTable GetTableForTest(string storedProcedure, string testName, SqlConnection itemBankConnection)
        {
            SqlDataAdapter adapter = new SqlDataAdapter();
            SqlCommand cmd = new SqlCommand(storedProcedure, itemBankConnection);
            if (DbCommandTimeout != null)
                cmd.CommandTimeout = DbCommandTimeout.Value;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@TestName", SqlDbType.VarChar, 255).Value = testName;
            adapter.SelectCommand = cmd;
            DataTable table = new DataTable();
            adapter.Fill(table);
            return table;
        }

        private DataTable GetTable(string storedProcedure, SqlConnection itemBankConnection)
        {
            SqlDataAdapter adapter = new SqlDataAdapter();
            SqlCommand cmd = new SqlCommand(storedProcedure, itemBankConnection);
            if (DbCommandTimeout != null)
                cmd.CommandTimeout = DbCommandTimeout.Value;
            cmd.CommandType = CommandType.StoredProcedure;
            adapter.SelectCommand = cmd;
            DataTable table = new DataTable();
            adapter.Fill(table);
            return table;
        }

        public TestItem FindItem(string testID, string itemName)
        {
            if (!HasTest(testID))
                return null;
            return _tests[testID].GetItem(itemName);
        }

        public TestBlueprint GetBlueprint(string testID)
        {
            if (HasTest(testID))
                return _tests[testID];
            else
                return null;
        }

        //public List<WindowMaxOpportunity> GetMaxWindowOpportunity(string testID)
        //{
        //    if (!HasTest(testID))
        //        return null;
        //    return _windowMaxOpp[testID];
        //}

        public Dictionary<string, List<TestAccomodation>> GetTestAccommodations(string testID)
        {
            if (!accommodations.ContainsKey(testID))
                return null;
            return accommodations[testID];
        }
      
    }
}
