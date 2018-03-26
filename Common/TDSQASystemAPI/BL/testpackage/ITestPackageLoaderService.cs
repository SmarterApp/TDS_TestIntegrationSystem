using System.Collections.Generic;
using System.IO;
using TDSQASystemAPI.Utilities;

namespace TDSQASystemAPI.BL.testpackage
{
    public interface ITestPackageLoaderService
    {
        IList<ValidationError> LoadTestPackage(Stream testPackageXmlStream);
    }
}
