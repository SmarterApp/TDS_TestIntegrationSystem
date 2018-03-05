using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TDSQASystemAPI.BL.testpackage.administration
{
    public interface IItembankConfigurationDataService
    {
        /// <summary>
        /// Create a new subject.
        /// </summary>
        /// <param name="testPackage">The <code>TestPackage.TestPackage</code> containing the subject information.</param>
        void CreateSubject(TestPackage.TestPackage testPackage);
    }
}
