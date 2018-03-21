using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TDSQASystemAPI.DAL.itembank.dtos;

namespace TDSQASystemAPI.BL.testpackage.osstis
{
    public interface ICombinationTestMapService
    {
        /// <summary>
        /// Create a collection of combination test form records, mapping the components of a "combined" <code>TestPackage</code>.
        /// </summary>
        /// <remarks>
        /// This method should only be called when loading a "combined" <code>TestPackage</code>.
        /// </remarks>
        /// <param name="testPackage">The <code>TestPackage</code> being loaded.</param>
        void CreateCombinationTestMap(TestPackage.TestPackage testPackage);

        /// <summary>
        /// Create a collection of test package form records, mapping the test forms to the "combined" <code>TestPackage</code>.
        /// </summary>
        /// <remarks>
        /// This method should only be called when loading a "combined" <code>TestPackage</code>.
        /// </remarks>
        /// <param name="testPackage">The <code>TestPackage</code> being loaded.</param>
        void CreateCombinationTestFormMap(TestPackage.TestPackage testPackage, IList<TestFormDTO> testForms);
    }
}
