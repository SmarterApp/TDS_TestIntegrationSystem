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

namespace ScoringEngine.MeasurementModels
{
    public class IRTModelFactory
    {
        public enum Model { Unknown = 0, IRT3PL = 1, IRTPCL = 2, raw = 3, IRT3PLn = 4, IRTGPC = 5, IRTGRL = 6 };

        public static IRTModel CreateModel(Model type)
        {
            switch (type)
            {
                case Model.IRT3PL:
                    return new IRTModel3pl();
                case Model.IRT3PLn:
                    return new IRTModel3pln();
                case Model.IRTPCL:
                    return new IRTModelPCL();
                case Model.IRTGPC:
                    return new IRTModelGPC();
                case Model.IRTGRL:
                    return new IRTModelGRL();
                case Model.raw:
                    return new RawModel();
                default:
                    throw new Exception("Undefined model type");
            }
        }


    }
}
