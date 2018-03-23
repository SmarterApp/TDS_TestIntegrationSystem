using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace TDSQASystemAPI.TestPackage
{
    public partial class TestPackage
    {
        /// <summary>
        /// Get the combined test package identifier for this <code>TestPackage</code>.
        /// </summary>
        public string GetCombinationTestPackageKey()
        {
            var combinedBlueprint = Blueprint.FirstOrDefault(bpe => 
                bpe.type.Equals("combined", StringComparison.InvariantCultureIgnoreCase));

            return combinedBlueprint == null 
                ? string.Empty
                : string.Format("({0}){1}-{2}", publisher, combinedBlueprint.id, academicYear);
        }

        /// <summary>
        /// Determine if this <code>TestPackage</code> represents a "combined" test package.
        /// </summary>
        /// <remarks>
        /// A "combined" test package contains multiple assessments that are intended to be scored as a single unit.
        /// </remarks>
        /// <returns>True if the <code>TestPackage</code> is a "combined" test package; otherwise false.</returns>
        public bool IsCombined()
        {
            return GetAllTestPackageBlueprintElements().Any(bpe => 
                bpe.Value.type.Equals("combined", StringComparison.InvariantCultureIgnoreCase));
        }

        /// <summary>
        /// Get a collection of all <code>ItemGroupItem</code>s from this <code>TestPackage</code>, regardless of their source (e.g. an item pool
        /// vs. a form).  This collection of <code>ItemGroupStimulus</code> objects comes from every <code>TestSegment</code> in the
        /// <code>TestPackage</code>.
        /// </summary>
        /// <returns>A collection of all <code>ItemGroupItem</code>s from every <code>TestSegment</code> in this <code>TestPackage</code>.</returns>
        public IReadOnlyCollection<ItemGroupItem> GetAllItems()
        {
            var allSegments = from a in Test
                              from s in a.Segments
                              select s;

            // Collect all the items, regardless of their source (i.e. either a form or an item pool).
            List<ItemGroupItem> allItems = new List<ItemGroupItem>();
            foreach (var segment in allSegments)
            {
                if (segment.Item is TestSegmentSegmentForms)
                {
                    foreach (var form in (segment.Item as TestSegmentSegmentForms).SegmentForm)
                    {
                        var items = from ig in form.ItemGroup
                                    from item in ig.Item
                                    select item;
                        if (items.Any())
                        {
                            allItems.AddRange(items);
                        }
                    }
                }
                else
                {
                    var pool = segment.Item as TestSegmentPool;
                    var items = from ig in pool.ItemGroup
                                from item in ig.Item
                                select item;
                    if (items.Any())
                    {
                        allItems.AddRange(items);
                    }
                }
            }

            return new ReadOnlyCollection<ItemGroupItem>(allItems);
        }

        /// Get a collection of all <code>ItemGroupStimulus</code>s from this <code>TestPackage</code>, regardless of their source (e.g. an item pool
        /// vs. a form).  This collection of <code>ItemGroupStimulus</code> objects comes from every <code>TestSegment</code> in the
        /// <code>TestPackage</code>.
        /// </summary>
        /// <returns>A collection of all <code>ItemGroupItem</code>s from every <code>TestSegment</code> in this <code>TestPackage</code>.</returns>
        public IReadOnlyCollection<ItemGroupStimulus> GetAllStimuli()
        {
            var allSegments = from a in Test
                              from s in a.Segments
                              select s;

            // Collect all the stimuli, regardless of their source (i.e. either a form or an item pool).
            List<ItemGroupStimulus> allStimuli = new List<ItemGroupStimulus>();
            foreach (var segment in allSegments)
            {
                if (segment.Item is TestSegmentSegmentForms)
                {
                    foreach (var form in (segment.Item as TestSegmentSegmentForms).SegmentForm)
                    {
                        var stimuli = from ig in form.ItemGroup
                                      where ig.Stimulus != null
                                      select ig.Stimulus;
                        if (stimuli.Any())
                        {
                            allStimuli.AddRange(stimuli);
                        }
                    }
                }
                else
                {
                    var pool = segment.Item as TestSegmentPool;
                    var stimuli = from ig in pool.ItemGroup
                                  where ig.Stimulus != null
                                  select ig.Stimulus;
                    if (stimuli.Any())
                    {
                        allStimuli.AddRange(stimuli);
                    }
                }
            }

            return new ReadOnlyCollection<ItemGroupStimulus>(allStimuli);
        }

        /// <summary>
        /// Collect all of the <code>BlueprintElement</code>s from this <code>TestPackage</code> as a flattened map.
        /// <remarks>
        /// Since <code>BlueprintElement</code>s can be nested n-levels deep, "flattening" the hierarchy makes looking up a 
        /// <code>BlueprintElement</code> by its identifier simpler.
        /// </remarks>
        /// </summary>
        /// <returns>A <code>IReadOnlyDictionary</code> of <code>BlueprintElement</code>s.  The key is the <code>BlueprintElement</code>'s identifier
        /// and the value is the <code>BlueprintElement</code>.</returns>
        public IReadOnlyDictionary<string, BlueprintElement> GetAllTestPackageBlueprintElements()
        {
            var flattenedBlueprintDictionary = new Dictionary<string, BlueprintElement>();

            BuildFlatBlueprintElementList(Blueprint, flattenedBlueprintDictionary);

            return new ReadOnlyDictionary<string, BlueprintElement>(flattenedBlueprintDictionary);
        }

        /// <summary>
        /// Get this <code>TestPackage</code>'s subject key.
        /// </summary>
        /// <returns>This <code>TestPackage</code>'s subject ke in the correct format.</returns>
        public string GetSubjectKey()
        {
            return string.Format("{0}-{1}", publisher, subject);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="blueprintElement">The current <code>BlueprintElement</code> being "flattened".</param>
        /// <param name="blueprintElementsSoFar">The collection of <code>BlueprintElement</code>s that have been collected and "flattened" so far.</param>
        private void BuildFlatBlueprintElementList(BlueprintElement[] blueprintElement, IDictionary<string, BlueprintElement> blueprintElementsSoFar)
        {
            if (blueprintElement == null || !blueprintElement.Any())
            {
                return;
            }

            foreach (var bp in blueprintElement)
            {
                blueprintElementsSoFar.Add(bp.id, bp);
                
                BuildFlatBlueprintElementList(bp.BlueprintElement1, blueprintElementsSoFar);
            }
        }
    }
}
