using System;
using System.Collections.Generic;
using System.Linq;
using TDSQASystemAPI.DAL;
using TDSQASystemAPI.DAL.itembank.daos;
using TDSQASystemAPI.DAL.itembank.dtos;
using TDSQASystemAPI.TestPackage;
using TDSQASystemAPI.TestPackage.utils;

namespace TDSQASystemAPI.BL.testpackage.administration
{
    public class ItembankConfigurationDataService : IItembankConfigurationDataService
    {
        private const string ITEM_FILE_NAME_PATTERN = "item-{0}-{1}.xml";
        private const string ITEM_FILE_PATH_PATTERN = "item-{0}-{1}/";
        private const string STIMULUS_FILE_NAME_PATTERN = "stim-{0}-{1}.xml";
        private const string STIMULUS_FILE_PATH_PATTERN = "stim-{0}-{1}/";

        private const string ITEM_TYPE_PROP_NAME = "--ITEMTYPE--";
        private const string LANGUAGE_PROP_NAME = "Language";
        private const string GRADE_PROP_NAME = "Grade";

        private readonly IItembankConfigurationDataQueryService itembankConfigurationDataQueryService;
        private readonly ITestPackageDao<ClientDTO> clientDao;
        private readonly ITestPackageDao<SubjectDTO> subjectDao;
        private readonly ITestPackageDao<StrandDTO> strandDAO;
        private readonly ITestPackageDao<ItemDTO> itemDAO;
        private readonly ITestPackageDao<StimulusDTO> stimuliDAO;
        private readonly ITestPackageDao<AaItemClDTO> aaItemClDao;
        private readonly ITestPackageDao<SetOfItemStrandDTO> setOfItemStrandDao;
        private readonly ITestPackageDao<SetOfItemStimuliDTO> setOfItemStimuliDao;
        private readonly ITestPackageDao<ItemPropertyDTO> itemPropertyDao;

        /// <summary>
        /// Default constructor to create new <code>ITestPackageDao<SubjectDTO></code> and
        /// <code>ITestPackageDao<ClientDTO></code> since a dependency injection container has not been 
        /// implemented.
        /// </summary>
        public ItembankConfigurationDataService()
        {
            itembankConfigurationDataQueryService = new ItembankConfigurationDataQueryService();
            clientDao = new ClientDAO();
            subjectDao = new SubjectDAO();
            strandDAO = new StrandDAO();
            itemDAO = new ItemDAO();
            stimuliDAO = new StimulusDAO();
            aaItemClDao = new AaItemClDAO();
            setOfItemStrandDao = new SetOfItemStrandDAO();
            setOfItemStimuliDao = new SetOfItemStimuliDAO();
            itemPropertyDao = new ItemPropertyDAO();
        }

        public ItembankConfigurationDataService(IItembankConfigurationDataQueryService itembankConfigurationDataQueryService,
                                                ITestPackageDao<ClientDTO> clientDao,
                                                ITestPackageDao<SubjectDTO> subjectDao,
                                                ITestPackageDao<StrandDTO> strandDAO,
                                                ITestPackageDao<ItemDTO> itemDAO,
                                                ITestPackageDao<StimulusDTO> stimuliDAO,
                                                ITestPackageDao<AaItemClDTO> aaItemClDao,
                                                ITestPackageDao<SetOfItemStrandDTO> setOfItemStrandDao,
                                                ITestPackageDao<SetOfItemStimuliDTO> setOfItemStimuliDao,
                                                ITestPackageDao<ItemPropertyDTO> itemPropertyDao)
        {
            this.itembankConfigurationDataQueryService = itembankConfigurationDataQueryService;
            this.clientDao = clientDao;
            this.subjectDao = subjectDao;
            this.strandDAO = strandDAO;
            this.itemDAO = itemDAO;
            this.stimuliDAO = stimuliDAO;
            this.aaItemClDao = aaItemClDao;
            this.setOfItemStrandDao = setOfItemStrandDao;
            this.setOfItemStimuliDao = setOfItemStimuliDao;
            this.itemPropertyDao = itemPropertyDao;
        }

        public void CreateClient(TestPackage.TestPackage testPackage)
        {
            // If the client record already exists, there's nothing to do.
            var existingClient = itembankConfigurationDataQueryService.FindClientByName(testPackage.publisher);
            if (existingClient != null)
            {
                return;
            }

            clientDao.Insert(new ClientDTO { Name = testPackage.publisher });
        }

        public void CreateItemProperties(TestPackage.TestPackage testPackage)
        {
            var allTestPackageItems = testPackage.GetAllItems();

            var allItemProperties = new List<ItemPropertyDTO>();
            foreach (var item in allTestPackageItems)
            {
                // Build --ITEMTYPE-- property
                allItemProperties.Add(new ItemPropertyDTO()
                {
                    ItemKey = item.Key,
                    PropertyName = ITEM_TYPE_PROP_NAME,
                    PropertyValue = item.type,
                    SegmentKey = item.TestSegment.Key,
                    IsActive = true
                });

                // Build language item property for each presentation included in the item.
                item.Presentations.ForEach(language => allItemProperties.Add(new ItemPropertyDTO()
                {
                    ItemKey = item.Key,
                    PropertyName = LANGUAGE_PROP_NAME,
                    PropertyValue = language.code,
                    SegmentKey = item.TestSegment.Key,
                    IsActive = true
                }));
            }

            itemPropertyDao.Insert(allItemProperties);
        }
            
        public List<ItemDTO> CreateItems(TestPackage.TestPackage testPackage)
        {
            var allItems = testPackage.GetAllItems();

            // Map all items to ItemDTOs for persistence.
            var allItemDtos = from item in allItems
                              select new ItemDTO
                              {
                                  ItemBankKey = testPackage.bankKey,
                                  ItemKey = long.Parse(item.id),
                                  ScorePoints = item.ItemScoreDimension.scorePoints,
                                  DateLastUpdated = DateTime.Now,
                                  FileName = string.Format(ITEM_FILE_NAME_PATTERN, testPackage.bankKey, item.id),
                                  FilePath = string.Format(ITEM_FILE_PATH_PATTERN, testPackage.bankKey, item.id),
                                  ItemType = item.type,
                                  Key = item.Key,
                                  TestVersion = (long)testPackage.version
                              };

            // Exclude items that already exist...
            var existingItems = itemDAO.Find(allItemDtos);
            var itemsToInsert = allItemDtos.Where(all => existingItems.All(exists => !exists.Key.Equals(all.Key)));

            // ... and insert the remaining items (if there are any)
            if (itemsToInsert.Any()) { 
                itemDAO.Insert(itemsToInsert.ToList());
            }

            return existingItems;
        }

        public void CreateStimuli(TestPackage.TestPackage testPackage)
        {
            var allStimuli = testPackage.GetAllStimuli();

            // Map all stimuli to StimulusDTOs for persistences
            var allStimuliDtos = from s in allStimuli
                                 select new StimulusDTO
                                 {
                                     ItemBankKey = testPackage.bankKey,
                                     ItsKey = long.Parse(s.id),
                                     TestVersion = (long)testPackage.version,
                                     FileName = string.Format(STIMULUS_FILE_NAME_PATTERN, testPackage.bankKey, s.id),
                                     FilePath = string.Format(STIMULUS_FILE_PATH_PATTERN, testPackage.bankKey, s.id),
                                     DateLastUpdated = DateTime.Now,
                                     StimulusKey = s.Key
                                 };

            stimuliDAO.Insert(allStimuliDtos.ToList());
        }

        public IDictionary<string, StrandDTO> CreateStrands(TestPackage.TestPackage testPackage)
        {
            var subjectKey = testPackage.GetSubjectKey();

            var existingSubject = itembankConfigurationDataQueryService.FindSubject(subjectKey);
            if (existingSubject == null)
            {
                throw new InvalidOperationException(string.Format("Could not find a subject for '{0}'", subjectKey));
            }

            var existingClient = itembankConfigurationDataQueryService.FindClientByName(testPackage.publisher);
            if (existingClient == null)
            {
                throw new InvalidOperationException(string.Format("Could not find a client record with name '{0}'", testPackage.publisher));
            }

            var blueprintElements = from bp in testPackage.Blueprint
                          select bp;

            var initialTreeLevel = 1;
            var initialParentKey = string.Empty;
            var newStrands = new List<StrandDTO>();

            BuildStrandsWithHierarchyFromBlueprintElements(blueprintElements,
                newStrands,
                existingClient,
                initialParentKey,
                subjectKey,
                (long)testPackage.version,
                initialTreeLevel);

            strandDAO.Insert(newStrands);

            return newStrands.ToDictionary(s => s.Name, s => s);
        }

        public void CreateSubject(TestPackage.TestPackage testPackage)
        {
            var subjectKey = testPackage.GetSubjectKey();

            // If a subject already exists, exit; there's nothing to do
            var existingSubject = itembankConfigurationDataQueryService.FindSubject(subjectKey);
            if (existingSubject != null)
            {
                return;
            }

            var client = itembankConfigurationDataQueryService.FindClientByName(testPackage.publisher);
            if (client == null)
            {
                throw new InvalidOperationException(string.Format("Could not find a client record with name '{0}'", testPackage.publisher));
            }

            var newSubjectDto = new SubjectDTO
            {
                Name = testPackage.subject,
                Grade = string.Empty, // from line 25 of tp.spLoad_Subject
                SubjectKey = subjectKey,
                ClientKey = client.ClientKey, 
                TestVersion = (long)testPackage.version
            };

            subjectDao.Insert(new List<SubjectDTO> { newSubjectDto });
        }

        public void LinkItemsToStimuli(TestPackage.TestPackage testPackage)
        {
            var allTestPackageStimuli = testPackage.GetAllStimuli();

            var itemStimuliDtos = from stimulus in allTestPackageStimuli
                                  from item in stimulus.ItemGroup.Item
                                  select new SetOfItemStimuliDTO
                                  {
                                      ItemKey = item.Key,
                                      StimulusKey = stimulus.Key,
                                      SegmentKey = stimulus.TestSegment.Key
                                  };

            setOfItemStimuliDao.Insert(itemStimuliDtos.ToList());
        }

        public void LinkItemToStrands(TestPackage.TestPackage testPackage, IDictionary<string, StrandDTO> strandMap)
        {
            var items = testPackage.GetAllItems();
            var allTestPackageBlueprintElements = testPackage.GetAllTestPackageBlueprintElements();

            // Create all the item -> content level links
            CreateItemLevelContent(allTestPackageBlueprintElements, items, strandMap);

            // Create the item -> strand links
            var strandDtos = from item in items
                             from bpRef in item.BlueprintReferences
                             join blueprint in allTestPackageBlueprintElements
                                 on bpRef.idRef equals blueprint.Value.id
                             where blueprint.Value.IsClaimOrTarget()
                             select new SetOfItemStrandDTO
                             {
                                 ItemKey = item.Key,
                                 SegmentKey = item.TestSegment.Key,
                                 StrandKey = strandMap[bpRef.idRef].Key,
                                 TestVersion = (long)testPackage.version
                             };

            setOfItemStrandDao.Insert(strandDtos.ToList());
        }

        /// <summary>
        /// Create a collection of <code>StrandDTO</code>s from the <code>TestPackage</code>'s collection of <code>BlueprintElement</code>s.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method emulates the <code>AssessmentItemBankGenericDataLoaderServiceImpl#loadBlueprintElementsHelper</code> method
        /// in TDS.
        /// </para>
        /// </remarks>
        /// <see cref="https://github.com/SmarterApp/TDS_AssessmentService/blob/1dc03698a2a608437d63b491757d366b1c217abc/service/src/main/java/tds/assessment/services/impl/AssessmentItemBankGenericDataLoaderServiceImpl.java#L124"/>
        /// <param name="blueprintElements">The collection of <code>BlueprintElement</code>s from the test package</param>
        /// <param name="strands">The collection of <code>StrandDTO</code>s that will be built by this method</param>
        /// <param name="client">The <code>ClientDTO</code> representing the client that "owns" this test package</param>
        /// <param name="parentKey">The identifier of the parent strand</param>
        /// <param name="subjectKey">The identifier of the subject to which these <code>StrandDTO</code>s are assigned.  e.g. SBAC_PT-ELA or SBAC_PT-MATH</param>
        /// <param name="testVersion">The version number of the test package being loaded</param>
        /// <param name="treeLevel"></param>
        private void BuildStrandsWithHierarchyFromBlueprintElements(IEnumerable<BlueprintElement> blueprintElements, 
                                                                    IList<StrandDTO> strands, 
                                                                    ClientDTO client, 
                                                                    string parentKey, 
                                                                    string subjectKey, 
                                                                    long testVersion, 
                                                                    int treeLevel)
        {
            if (blueprintElements == null || !blueprintElements.Any())
            {
                return;
            }

            foreach (var blueprintElement in blueprintElements)
            {
                // For claims and targets, the convention is to prepend the client name to the id
                var key = BlueprintElementTypes.CLAIM_AND_TARGET_TYPES.Contains(blueprintElement.type)
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

        /// <summary>
        /// Create records to describe the content levels for each <code>ItemGroupItem</code> contained within the 
        /// <code>TestPackage</code>.
        /// </summary>
        /// <param name="allTestPackageBlueprintElements">A <code>IReadOnlyDictionary</code> of all the <code>BlueprintElements</code> contained
        /// within the <code>TestPackage</code>.  The key is the <code>BlueprintElement</code>'s id, and the value is the
        /// <code>BlueprintElement</code>.</param>
        /// <param name="items">A collection of all the <code>ItemGroupItem</code>s contained within the <code>TestPackage</code></param>
        /// <param name="strandMap">A <code>IDictionary</code> of all the <code>StrandDTO</code>s that were built earlier in 
        /// the load process.</param>
        private void CreateItemLevelContent(IReadOnlyDictionary<string, BlueprintElement> allTestPackageBlueprintElements, 
                                            IReadOnlyCollection<ItemGroupItem> items, 
                                            IDictionary<string, StrandDTO> strandMap)
        {
            var allAaItemClDtos = new HashSet<AaItemClDTO>();
            foreach (var item in items)
            {
                var allAaItemClDtosForItem = new List<AaItemClDTO>();
                var bluePrintElements = from itemBp in item.BlueprintReferences
                                        select itemBp;
                
                foreach (var bp in bluePrintElements)
                {
                        var aaItemClDtos = from testPkgBp in allTestPackageBlueprintElements
                                            where testPkgBp.Key.Equals(bp.idRef)
                                                && (testPkgBp.Value.IsClaimOrTarget()
                                                    || testPkgBp.Value.type.Equals(BlueprintElementTypes.AFFINITY_GROUP, StringComparison.InvariantCultureIgnoreCase))
                                            select new AaItemClDTO
                                            {
                                                ContentLevel = testPkgBp.Value.IsClaimOrTarget() ? string.Format("{0}-{1}", item.TestPackage.publisher, bp.idRef) : bp.idRef,
                                                ItemKey = item.Key,
                                                SegmentKey = item.TestSegment.Key
                                            };

                        allAaItemClDtosForItem.AddRange(aaItemClDtos);
                    
                }

                if (allAaItemClDtosForItem.Any())
                {
                    allAaItemClDtosForItem.ForEach(aaItem => allAaItemClDtos.Add(aaItem));
                }
            }

            aaItemClDao.Insert(allAaItemClDtos.ToList());
        }
    }
}
