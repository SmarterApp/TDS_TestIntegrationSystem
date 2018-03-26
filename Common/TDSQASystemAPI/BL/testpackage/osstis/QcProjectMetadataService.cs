using System;
using System.Linq;
using TDSQASystemAPI.DAL;
using TDSQASystemAPI.DAL.osstis.daos;
using TDSQASystemAPI.DAL.osstis.dtos;
using TDSQASystemAPI.TestPackage.utils;

namespace TDSQASystemAPI.BL.testpackage.osstis
{
    public class QcProjectMetadataService : IQcProjectMetadataService
    {
        private readonly ITestPackageDao<QcProjectMetadataDTO> qcProjectMetadataDao;
        private readonly ITestPackageDao<ProjectDTO> projectDao;

        public QcProjectMetadataService()
        {
            qcProjectMetadataDao = new QcProjectMetadataDAO();
            projectDao = new ProjectDAO();
        }

        public QcProjectMetadataService(ITestPackageDao<QcProjectMetadataDTO> qcProjectMetadataDao,
            ITestPackageDao<ProjectDTO> projectDao)
        {
            this.qcProjectMetadataDao = qcProjectMetadataDao;
            this.projectDao = projectDao;
        }

        public void CreateQcProjectMetadata(TestPackage.TestPackage testPackage)
        {
            // Only "combined" test packages need QC Project Metadata records 
            if (!testPackage.IsCombined())
            {
                return;
            }
            
            var projectList = projectDao.Find(QcProjectMetadataDefaults.TEST_PACKAGE_XSD_PROJECT_DESCRIPTION);
            if (projectList == null || !projectList.Any())
            {
                throw new InvalidOperationException(string.Format("Could not find a TIS project for '{0}'", QcProjectMetadataDefaults.TEST_PACKAGE_XSD_PROJECT_DESCRIPTION));
            }

            var project = projectList.First();
            var combinedTestName = testPackage.GetTestPackageKey();

            var qcProjectMetadataDtos = from status in QcProjectMetadataDefaults.Statuses
                                        select new QcProjectMetadataDTO
                                        {
                                            ProjectId = QcProjectMetadataDefaults.PROJECT_MAP_ID,
                                            GroupName = QcProjectMetadataDefaults.PROJECT_MAP_GROUP_NAME,
                                            VarName = string.Format("0-{0}-{1}", combinedTestName, status),
                                            IntValue = project.ProjectKey,
                                            FloatValue = null,
                                            TextValue = null,
                                            Comment = QcProjectMetadataDefaults.COMMENT
                                        };

            qcProjectMetadataDao.Insert(qcProjectMetadataDtos.ToList());
        }
    }
}
