using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TDSQASystemAPI.DAL.itembank.dtos;

namespace TDSQASystemAPI.BL.testpackage.administration
{
    public interface IItembankConfigurationDataService
    {
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
    }
}
