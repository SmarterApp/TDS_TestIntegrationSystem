using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Xml;
using TDSQASystemAPI.BL.testpackage.osstis;
using TDSQASystemAPI.DAL;
using TDSQASystemAPI.DAL.osstis.dtos;
using TDSQASystemAPI.TestPackage;
using TDSQASystemAPI.TestPackage.utils;

namespace TISUnitTests.services
{
    [TestClass]
    public class QcProjectMetadataServiceUnitTest
    {
        private const string TEST_PACKAGE_XML_FILE = @"..\..\resources\test-packages\new-xsd\V2-(SBAC_PT)IRP-GRADE-11-MATH-EXAMPLE.xml";

        private readonly Mock<ITestPackageDao<QcProjectMetadataDTO>> mockQcProjectMetadataDao = new Mock<ITestPackageDao<QcProjectMetadataDTO>>();
        private readonly Mock<ITestPackageDao<ProjectDTO>> mockProjectDao = new Mock<ITestPackageDao<ProjectDTO>>();

        private IQcProjectMetadataService qcProjectMetadataService;

        [TestInitialize]
        public void Setup()
        {
            qcProjectMetadataService = new QcProjectMetadataService(mockQcProjectMetadataDao.Object,
                mockProjectDao.Object);
        }

        [TestMethod]
        public void QcProjectMetadata_ShouldCreateACollectionOfQcProjectMetadataRecords()
        {
            var testPackage = TestPackageMapper.FromXml(new XmlTextReader(TEST_PACKAGE_XML_FILE));
            
            var mockProject = new ProjectDTO
            {
                ProjectKey = 42,
                Description = QcProjectMetadataDefaults.TEST_PACKAGE_XSD_PROJECT_DESCRIPTION
            };

            mockProjectDao.Setup(dao => dao.Find(It.IsAny<string>()))
                .Returns(new List<ProjectDTO> { mockProject });
            mockQcProjectMetadataDao.Setup(dao => dao.Insert(It.IsAny<List<QcProjectMetadataDTO>>()))
                .Verifiable();

            qcProjectMetadataService.CreateQcProjectMetadata(testPackage);

            mockProjectDao.Verify(dao => dao.Find(QcProjectMetadataDefaults.TEST_PACKAGE_XSD_PROJECT_DESCRIPTION));
            mockQcProjectMetadataDao.Verify(dao => dao.Insert(It.Is<IList<QcProjectMetadataDTO>>(result =>
                EvaluateShouldCreateACollectionOfQcProjectMetadataRecords(result))));
        }

        [TestMethod]
        public void QcProjectMetadata_ShouldNotCreateQcProjectMetadataRecordsForATestThatIsNotCombined()
        {
            var testPackage = new TestPackage
            {
                publisher = "SBAC_PT",
                academicYear = "2017-2018",
                Blueprint = new BlueprintElement[]
                {
                    new BlueprintElement
                    {
                        id = "ICA-UNIT-TEST-GRADE-11-FIXED",
                        type = "foo"
                    }
                }
            };

            mockProjectDao.Setup(dao => dao.Find(It.IsAny<string>()))
                .Returns(new List<ProjectDTO>());
            mockQcProjectMetadataDao.Setup(dao => dao.Insert(It.IsAny<List<QcProjectMetadataDTO>>()))
                .Verifiable();

            qcProjectMetadataService.CreateQcProjectMetadata(testPackage);

            mockProjectDao.Verify(dao => dao.Find(QcProjectMetadataDefaults.TEST_PACKAGE_XSD_PROJECT_DESCRIPTION), Times.Never);
            mockQcProjectMetadataDao.Verify(dao => dao.Insert(It.IsAny<List<QcProjectMetadataDTO>>()), Times.Never);
        }

        [TestMethod]
        public void QcProjectMetadata_ShouldThrowExceptionWhenProjectCannotBeFound()
        {
            var testPackage = new TestPackage
            {
                publisher = "SBAC_PT",
                academicYear = "2017-2018",
                Blueprint = new BlueprintElement[]
                {
                    new BlueprintElement
                    {
                        id = "ICA-UNIT-TEST-GRADE-11-FIXED",
                        type = "combined"
                    }
                }
            };

            mockProjectDao.Setup(dao => dao.Find(It.IsAny<string>()))
                .Returns(new List<ProjectDTO>());
            mockQcProjectMetadataDao.Setup(dao => dao.Insert(It.IsAny<List<QcProjectMetadataDTO>>()))
                .Verifiable();

            var exception = 
                Assert.ThrowsException<InvalidOperationException>(() => qcProjectMetadataService.CreateQcProjectMetadata(testPackage));
            Assert.AreEqual("Could not find a TIS project for 'SBAC Test Package XSD Project'", exception.Message);
        }

        private bool EvaluateShouldCreateACollectionOfQcProjectMetadataRecords(IList<QcProjectMetadataDTO> qcProjectMetadataDTOs)
        {
            Assert.AreEqual(QcProjectMetadataDefaults.Statuses.Length, qcProjectMetadataDTOs.Count);

            for (var i = 0; i < qcProjectMetadataDTOs.Count; i++)
            {
                var qcMetaDataDto = qcProjectMetadataDTOs[i];
                var status = QcProjectMetadataDefaults.Statuses[i];

                Assert.AreEqual(QcProjectMetadataDefaults.PROJECT_MAP_ID, qcMetaDataDto.ProjectId);
                Assert.AreEqual(QcProjectMetadataDefaults.PROJECT_MAP_GROUP_NAME, qcMetaDataDto.GroupName);
                Assert.AreEqual(string.Format("0-(SBAC_PT)SBAC-IRP-COMBINED-MATH-11-2017-2018-{0}", status), qcMetaDataDto.VarName);
                Assert.AreEqual(42, qcMetaDataDto.IntValue);
                Assert.IsNull(qcMetaDataDto.FloatValue);
                Assert.IsNull(qcMetaDataDto.TextValue);
            }

            // If all the Assertions pass, this method passes.  Otherwise, the Assert will throw an
            // exception before this line is hit.
            return true;
        }
    }
}
