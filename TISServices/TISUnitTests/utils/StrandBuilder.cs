using System.Collections.Generic;
using System.Linq;
using TDSQASystemAPI.DAL.itembank.dtos;
using TDSQASystemAPI.TestPackage;

namespace TISUnitTests.utils
{
    public class StrandBuilder
    {
        public static IDictionary<string, StrandDTO> GetStrandDTODictionary(TestPackage testPackage)
        {
            var blueprintElements = from bp in testPackage.Blueprint
                                    select bp;
            var initialTreeLevel = 1;
            var initialParentKey = string.Empty;
            var newStrands = new List<StrandDTO>();
            var unitTestClient = new ClientDTO { Name = "UNIT-TEST", ClientKey = 99L };


            BuildStrandsWithHierarchyFromBlueprintElements(blueprintElements,
                newStrands,
                unitTestClient,
                initialParentKey,
                "UNIT_TEST-ELA",
                (long)testPackage.version,
                initialTreeLevel);

            return newStrands.ToDictionary(s => s.Name, s => s);
        }

        private static void BuildStrandsWithHierarchyFromBlueprintElements(IEnumerable<BlueprintElement> blueprintElements,
                                                                           IList<StrandDTO> strands,
                                                                           ClientDTO client,
                                                                           string parentKey,
                                                                           string subjectKey,
                                                                           long testVersion,
                                                                           int treeLevel)
        {
            var CLAIM_AND_TARGET_TYPES = new string[] { "strand", "contentlevel", "claim" };
            if (blueprintElements == null || !blueprintElements.Any())
            {
                return;
            }

            foreach (var blueprintElement in blueprintElements)
            {
                // For claims and targets, the convention is to prepend the client name to the id
                var key = CLAIM_AND_TARGET_TYPES.Contains(blueprintElement.type)
                    ? string.Format("{0}-{1}", client.Name, blueprintElement.id)
                    : blueprintElement.id;

                // If a blueprint element does not have any "child" blueprint elements, it should be marked as a
                // "leaf" node.  Otherwise, it should not be marked as a leaf node.  The HasLeafNode property is used
                // later in the loading process.
                // from https://github.com/SmarterApp/TDS_AssessmentService/blob/1dc03698a2a608437d63b491757d366b1c217abc/service/src/main/java/tds/assessment/services/impl/AssessmentItemBankGenericDataLoaderServiceImpl.java#L138
                var strand = new StrandDTO
                {
                    Name = blueprintElement.id,
                    ParentId = parentKey,
                    ClientKey = client.ClientKey,
                    TreeLevel = treeLevel,
                    TestVersion = testVersion,
                    BlueprintElementId = blueprintElement.id,
                    SubjectKey = subjectKey,
                    Key = key,
                    Type = blueprintElement.type,
                    IsLeafTarget = (blueprintElement.BlueprintElement1?.Length ?? 0) == 0
                };

                strands.Add(strand);

                // Build the blueprint hierarchy at the next tree level.
                BuildStrandsWithHierarchyFromBlueprintElements(blueprintElement.BlueprintElement1,
                    strands,
                    client,
                    key,
                    subjectKey,
                    testVersion,
                    treeLevel + 1);
            }
        }
    }
}
