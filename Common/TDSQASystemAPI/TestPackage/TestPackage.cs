using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace TDSQASystemAPI.TestPackage
{
    public partial class TestPackage
    {
        /// <summary>
        /// Get a collection of all <code>ItemGroupItem</code>s from this <code>TestPackage</code>, regardless of their source (e.g. an item pool
        /// vs. a form).  This collection of <code>ItemGroupStimulus</code> objects comes from every <code>AssessmentSegment</code> in the
        /// <code>TestPackage</code>.
        /// </summary>
        /// <returns>A collection of all <code>ItemGroupItem</code>s from every <code>AssessmentSegment</code> in this <code>TestPackage</code>.</returns>
        public IReadOnlyCollection<ItemGroupItem> GetAllItems()
        {
            var allSegments = from a in Assessment
                              from s in a.Segments
                              select s;

            // Collect all the items, regardless of their source (i.e. either a form or an item pool).
            List<ItemGroupItem> allItems = new List<ItemGroupItem>();
            foreach (var segment in allSegments)
            {
                if (segment.Item is AssessmentSegmentSegmentForms)
                {
                    foreach (var form in (segment.Item as AssessmentSegmentSegmentForms).SegmentForm)
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
                    var pool = segment.Item as AssessmentSegmentPool;
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
        /// vs. a form).  This collection of <code>ItemGroupStimulus</code> objects comes from every <code>AssessmentSegment</code> in the
        /// <code>TestPackage</code>.
        /// </summary>
        /// <returns>A collection of all <code>ItemGroupItem</code>s from every <code>AssessmentSegment</code> in this <code>TestPackage</code>.</returns>
        public IReadOnlyCollection<ItemGroupStimulus> GetAllStimuli()
        {
            var allSegments = from a in Assessment
                              from s in a.Segments
                              select s;

            // Collect all the stimuli, regardless of their source (i.e. either a form or an item pool).
            List<ItemGroupStimulus> allStimuli = new List<ItemGroupStimulus>();
            foreach (var segment in allSegments)
            {
                if (segment.Item is AssessmentSegmentSegmentForms)
                {
                    foreach (var form in (segment.Item as AssessmentSegmentSegmentForms).SegmentForm)
                    {
                        var stimuli = from ig in form.ItemGroup
                                      select ig.Stimulus;
                        if (stimuli.Any())
                        {
                            allStimuli.AddRange(stimuli);
                        }
                    }
                }
                else
                {
                    var pool = segment.Item as AssessmentSegmentPool;
                    var stimuli = from ig in pool.ItemGroup
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
        /// 
        /// </summary>
        /// <returns></returns>
        public IReadOnlyDictionary<string, BlueprintElement> GetAllTestPackageBlueprintElements()
        {
            var flattenedBlueprintDictionary = new Dictionary<string, BlueprintElement>();

            BuildFlatBlueprintElementList(Blueprint, flattenedBlueprintDictionary);

            return new ReadOnlyDictionary<string, BlueprintElement>(flattenedBlueprintDictionary);
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
