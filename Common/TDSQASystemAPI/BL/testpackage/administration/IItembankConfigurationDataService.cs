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
        /// Create a new collection of Items
        /// </summary>
        /// <param name="testPackage">The <code>TestPackage.TestPackage</code> containing the items.</param>
        void CreateItems(TestPackage.TestPackage testPackage);

        /// <summary>
        /// Create a new collection of Stimuli
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
        /// 
        /// </summary>
        /// <param name="strandMap"></param>
        void LinkItemToStrands(TestPackage.TestPackage testPackage, IDictionary<string, StrandDTO> strandMap);

        /// <summary>
        /// 
        /// </summary>
        void LinkItemsToStimuli(TestPackage.TestPackage testPackage, IDictionary<string, StrandDTO> strandMap);
    }
}
