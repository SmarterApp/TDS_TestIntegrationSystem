﻿using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;

namespace TDSQASystemAPI.TestPackage.utils
{
    /// <summary>
    /// 
    /// </summary>
    public class TestPackageAssembler
    {
        private static readonly XmlSerializer xmlSerializer = new XmlSerializer(typeof(TestPackage));

        /// <summary>
        /// Create a <code>TestPackage</code> instance from 
        /// </summary>
        /// <param name="reader">An <code>XmlReader</code> containing the test package's XML.</param>
        /// <returns></returns>
        public static TestPackage FromXml(XmlReader reader)
        {
            var testPackage = xmlSerializer.Deserialize(reader) as TestPackage;

            // Wire up parent -> child relationships for:
            // 1.  test package -> assessment(s)
            // 2.  assesmment -> segment(s)
            testPackage.Assessment.ForEach(a => a.TestPackage = testPackage);
            testPackage.Assessment.SelectMany(a => a.Segments, (a, s) => s.Assessment = a);

            var allSegments = from a in testPackage.Assessment
                              from s in a.Segments
                              select s;
                           
            // The segment's Item property can be a form (for fixed-form assessments) or a pool of items (for an 
            // adaptive assessment).  Set the appropriate properties based on the type that's stored in the
            // AssessmentSegment's Item property.
            foreach (var segment in allSegments)
            {
                if (segment.Item is AssessmentSegmentSegmentForms)
                {
                    var form = segment.Item as AssessmentSegmentSegmentFormsSegmentForm;
                    form.AssessmentSegment = segment;
                    form.ItemGroup.ForEach(ig => AssembleItemGroup(ig, testPackage, segment, form));
                }
                else
                {
                    var pool = segment.Item as AssessmentSegmentPool;
                    pool.ItemGroup.ForEach(ig => AssembleItemGroup(ig, testPackage, segment));
                }
            }

            return testPackage;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemGroup">The <code>ItemGroup</code> to assemble</param>
        /// <param name="testPackage">The <code>TestPackage</code> that ultimately owns this <code>ItemGroup</code></param>
        /// <param name="segment">The <code>AssessmentSegment</code> that owns this <code>ItemGroup</code></param>
        /// <param name="form">The <code>AssessmentSegmentSegmentFormsSegmentForm</code> to which the item is associated.
        /// Will be null for items associated to adaptive assessments; adaptive assessments do not have forms.</param>
        private static void AssembleItemGroup(ItemGroup itemGroup, 
            TestPackage testPackage, 
            AssessmentSegment segment, 
            AssessmentSegmentSegmentFormsSegmentForm form = null)
        {
            itemGroup.AssessmentSegment = segment;
            foreach (var item in itemGroup.Item)
            {
                item.TestPackage = testPackage;
                item.SegmentForm = form;
                item.ItemGroup = itemGroup;
                if (item.TeacherHandScoring != null)
                {
                    item.TeacherHandScoring.TestPackage = testPackage;
                    item.TeacherHandScoring.ItemGroupItem = item;
                }
            }

            if (itemGroup.Stimulus != null)
            {
                itemGroup.Stimulus.TestPackage = testPackage;
            }
        }
    }
}