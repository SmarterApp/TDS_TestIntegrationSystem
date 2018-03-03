﻿using System.Xml.Serialization;

namespace TDSQASystemAPI.TestPackage
{
    /// <summary>
    /// A partial class to augment the <code>AssessmentSegmentSegmentFormsSegmentForm</code> with additional 
    /// data.
    /// </summary>
    public partial class AssessmentSegmentSegmentFormsSegmentForm
    {
        /// <summary>
        /// The <code>AssessmentSegment</code> that owns this <code>AssessmentSegmentSegmentFormsSegmentForm</code>.
        /// </summary>
        [XmlIgnore]
        public AssessmentSegment AssessmentSegment { get; set; }
    }
}
