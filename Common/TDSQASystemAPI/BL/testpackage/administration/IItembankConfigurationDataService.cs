using System.Collections.Generic;
using TDSQASystemAPI.DAL.itembank.dtos;

namespace TDSQASystemAPI.BL.testpackage.administration
{
    /// <summary>
    /// An interface for interacting with the static/"lookup" Test Package data.
    /// </summary>
    public interface IItembankConfigurationDataService
    {
        /// <summary>
        /// Create a new client from the publisher information included in the <code>TestPackage</code>.
        /// </summary>
        /// <param name="testPackage">The <code>TestPackage</code> being loaded.</param>
        void CreateClient(TestPackage.TestPackage testPackage);

        /// <summary>
        /// Create a collection of item properties for each item included in the teset package.
        /// </summary>
        /// <param name="testPackage">The <code>TestPackage</code> being loaded.</param>
        void CreateItemProperties(TestPackage.TestPackage testPackage);

        /// <summary>
        /// Create a new collection of Items for each item included in the test package.
        /// </summary>
        /// <param name="testPackage">The <code>TestPackage.TestPackage</code> containing the items.</param>
        /// <returns>A collection of <code>ItemDTO</code>s representing items that already exist in the database.</returns>
        List<ItemDTO> CreateItems(TestPackage.TestPackage testPackage);

        /// <summary>
        /// Create a new collection of Stimuli for each stimulus included in the test package.
        /// </summary>
        /// <param name="testPackage">The <code>TestPackage.TestPackage</code> containing the stimuli.</param>
        void CreateStimuli(TestPackage.TestPackage testPackage);

        /// <summary>
        /// Create a new subject.
        /// </summary>
        /// <param name="testPackage">The <code>TestPackage.TestPackage</code> containing the subject information.</param>
        void CreateSubject(TestPackage.TestPackage testPackage);

        /// <summary>
        /// Create a new set of <code>StrandDTO</code>s
        /// </summary>
        /// <param name="testPackage">The <code>TestPackage.TestPackage</code> containing the strand information.</param>
        /// <returns>A <code>IDictionary<string, StrandDTO></code> where the key is the strand's name and the value is the 
        /// <code>StrandDTO</code> the name points to.</returns>
        IDictionary<string, StrandDTO> CreateStrands(TestPackage.TestPackage testPackage);

        /// <summary>
        /// Insert records that will establish links between items and strand blueprints
        /// </summary>
        /// /// <param name="testPackage">The <code>TestPackage.TestPackage</code> containing the subject information.</param>
        /// <param name="strandMap"></param>
        void LinkItemToStrands(TestPackage.TestPackage testPackage, IDictionary<string, StrandDTO> strandMap);

        /// <summary>
        /// Insert records that will establish links between items and stimuli.
        /// </summary>
        /// <param name="testPackage"></param>
        void LinkItemsToStimuli(TestPackage.TestPackage testPackage);
    }
}
