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
using System.Text;

namespace ScoringEngine.MeasurementModels
{
    public abstract class IRTModel
    {
        // Note valid values are checked in the TestCollection constructor.
        private int numQuadraturePoints = String.IsNullOrEmpty(ConfigurationManager.AppSettings["NumQuadPointsForExpectedInfo"]) ? 5 : Int32.Parse(ConfigurationManager.AppSettings["NumQuadPointsForExpectedInfo"]);
        protected int _parameterCount = 0;
        protected double[] _parameters = null;
        protected IRTModelFactory.Model _measurementModel = IRTModelFactory.Model.Unknown;

        public IRTModel()
        {
            _measurementModel = IRTModelFactory.Model.Unknown;
        }


        public IRTModel(IRTModelFactory.Model model)
        {
            _measurementModel = model;
        }

        #region properties

        public int ParameterCount
        {
            get
            {
                return _parameterCount;
            }
        }

        public double[] Parameters
        {
            get 
            { 
                return _parameters;
            }
        }

        public IRTModelFactory.Model MeasurementModel
        {
            get
            {
                return _measurementModel;
            }
        }

        #endregion properties

        #region quadrature data
        private static readonly double[] abscissae3 = { -1.7320508075688772935, 0, 1.7320508075688772935 };
        private static readonly double[] weights3 = { 0.16666666666666666667, 0.66666666666666666667, 0.16666666666666666667 };

        private static readonly double[] abscissae4 = { -2.3344142183389772393, -0.74196378430272585765, 0.74196378430272585765, 2.3344142183389772393 };
        private static readonly double[] weights4 = { 0.045875854768068491817, 0.45412414523193150818, 0.45412414523193150818, 0.045875854768068491817 };

        private static readonly double[] abscissae5 = { -2.8569700138728056542, -1.3556261799742658658, 0, 1.3556261799742658658, 2.8569700138728056542 };
        private static readonly double[] weights5 = { 0.011257411327720688933, 0.22207592200561264440, 0.53333333333333333333, 0.22207592200561264440, 0.011257411327720688933 };

        private static readonly double[] abscissae7 =
            {
                -3.7504397177257422563, -2.3667594107345412886,
                -1.1544053947399681272, 0, 1.1544053947399681272, 2.3667594107345412886, 3.7504397177257422563
            };

        private static readonly double[] weights7 =
            {
                0.00054826885597221779162, 0.030757123967586497040,
                0.24012317860501271374, 0.45714285714285714286, 0.24012317860501271374, 0.030757123967586497040,
                0.00054826885597221779162
            };

        private static readonly double[] abscissae20 =
            {
                -7.6190485416797582914, -6.5105901570136544864,
                -5.5787388058932011527, -4.7345813340460553439, -3.9439673506573162603, -3.1890148165533894149,
                -2.4586636111723677513, -1.7452473208141267149, -1.0429453488027510315, -0.34696415708135592797,
                0.34696415708135592797, 1.0429453488027510315, 1.7452473208141267149, 2.4586636111723677513,
                3.1890148165533894149, 3.9439673506573162603, 4.7345813340460553439, 5.5787388058932011527,
                6.5105901570136544864, 7.6190485416797582914
            };

        private static readonly double[] weights20 =
            {
                1.2578006724379270154E-13, 2.4820623623151786456E-10,
                6.1274902599829475405E-8, 4.4021210902308528331E-6, 0.00012882627996192944940, 0.0018301031310804927956,
                0.013997837447101003350, 0.061506372063976906552, 0.16173933398399996172, 0.26079306344955485915,
                0.26079306344955485915, 0.16173933398399996172, 0.061506372063976906552, 0.013997837447101003350,
                0.0018301031310804927956, 0.00012882627996192944940, 4.4021210902308528331E-6, 6.1274902599829475405E-8,
                2.4820623623151786456E-10, 1.2578006724379270154E-13
            };

        private static readonly double[] abscissae21 =
            {
                -7.8493828951138219930, -6.7514447187174607668,
                -5.8293820073044713717, -4.9949639447820251929, -4.2143439816884213500, -3.4698466904753762952,
                -2.7505929810523730936, -2.0491024682571626618, -1.3597658232112302657, -0.67804569244064402621, 0,
                0.67804569244064402621, 1.3597658232112302657, 2.0491024682571626618, 2.7505929810523730936,
                3.4698466904753762952, 4.2143439816884213500, 4.9949639447820251929, 5.8293820073044713717,
                6.7514447187174607668, 7.8493828951138219930
            };

        private static readonly double[] weights21 =
            {
                2.0989912195656767434E-14, 4.9753686041217240051E-11,
                1.4506612844930866107E-8, 1.2253548361482535298E-6, 0.000042192347425516757386, 0.00070804779548153646931,
                0.0064396970514087768692, 0.033952729786542835053, 0.10839228562641944345, 0.21533371569505968660,
                0.27026018357287707133, 0.21533371569505968660, 0.10839228562641944345, 0.033952729786542835053,
                0.0064396970514087768692, 0.00070804779548153646931, 0.000042192347425516757386, 1.2253548361482535298E-6,
                1.4506612844930866107E-8, 4.9753686041217240051E-11, 2.0989912195656767434E-14
            };

        private static readonly Dictionary<int, double[]> abscissae = new Dictionary<int, double[]>()
            {
                {3,abscissae3},
                {4, abscissae4},
                {5, abscissae5},
                {7, abscissae7},
                {20, abscissae20},
                {21, abscissae21}
            };
        private static readonly Dictionary<int, double[]> weights = new Dictionary<int, double[]>()         
            {
                {3,weights3},
                {4, weights4},
                {5, weights5},
                {7, weights7},
                {20, weights20},
                {21, weights21}
            };
        #endregion quadrature data

        abstract public void SetParameterCount(int parameterCount, int scorePoints);

        virtual public void SetParameter(int position, double value)
        {
            if ((_parameterCount < position) || (_parameters == null) || (position < 0))
                throw new Exception("ScoringEngine.MeasurementModels.IRTModel.SetParameter. Parameter position out of bounds. position = " + position.ToString() + ", parameter count = " + _parameterCount.ToString());

            _parameters[position] = value;
        }

        abstract public double D1LnlWrtTheta(double p, double theta);
        abstract public double ComputeProbability(double p, double theta);
        abstract public double D2LnlWrtTheta(double p, double theta);
        abstract public void RescaleParameters(double slope, double intercept);
        abstract public double GetDifficulty();
        abstract public double GetSlope();
        abstract public double ExpectedScore(double theta);
        abstract public double Information(double theta);
        abstract public void PrintDebugInfo(string itemName, double score, double theta, string filename);
        abstract public IRTModel DeepCopy();
        public abstract double GuessProbability();

        /// Compute the information if the student's theta isn't known exactly, but is assumed normaly distributed with mean theta and standard deviation se.
        public double ExpectedInformation(double theta, double se)
        {
            double sum = 0;
            for (int i = 0; i < numQuadraturePoints; i++)
            {
                sum += weights[numQuadraturePoints][i] * Information(theta + se * abscissae[numQuadraturePoints][i]);
            }
            return sum;
        }
    }
}
