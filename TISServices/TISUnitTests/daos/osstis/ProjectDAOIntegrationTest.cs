using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using TDSQASystemAPI.DAL;
using TDSQASystemAPI.DAL.osstis.daos;
using TDSQASystemAPI.DAL.osstis.dtos;
using TISUnitTests.utils;

namespace TISUnitTests.daos.osstis
{
    /// <summary>
    /// For these integration tests to run successfully, the 
    /// <code></code>TDS_TestIntegrationSystem\TDSQAService\OSS.TIS\SQL\TISDB\7_oss_tis_add_new_project_and_metadata.sql</summary>code> script must
    /// be run against the <code>OSS_TIS</code>.
    /// </summary>
    [TestClass]
    public class ProjectDAOIntegrationTest : TestPackageDaoIntegrationTestBase<ProjectDTO>
    {
        private readonly ITestPackageDao<ProjectDTO> testPackageDao = new ProjectDAO();

        [TestMethod]
        public void ShouldGetAProjectDTORecordByDescription()
        {
            var projectName = "SBAC Test Package XSD Project";

            var result = testPackageDao.Find(projectName);

            Assert.AreEqual(1, result.Count);

            var project = result.First();
            Assert.AreEqual(projectName, project.Description);
            Assert.IsTrue(project.ProjectKey > 0);
        }

        [TestMethod]
        public void ShouldNotBeAbleToInsertANewProjectDTO()
        {
            var project = new ProjectDTO
            {
                Description = "unit-test"
            };

            Assert.ThrowsException<NotImplementedException>(() => testPackageDao.Insert(new List<ProjectDTO> { project }));
        }
    }
}
