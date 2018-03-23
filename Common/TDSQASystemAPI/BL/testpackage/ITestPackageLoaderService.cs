using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TDSQASystemAPI.BL.testpackage
{
    public interface ITestPackageLoaderService
    {
        LoadTestPackageResponse LoadTestPackage(Stream testPackageXmlStream);
    }
}
