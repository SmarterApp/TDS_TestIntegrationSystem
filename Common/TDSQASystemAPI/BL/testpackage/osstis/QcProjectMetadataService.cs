using System;
using System.Linq;
using TDSQASystemAPI.DAL;
using TDSQASystemAPI.DAL.osstis.daos;
using TDSQASystemAPI.DAL.osstis.dtos;
using TDSQASystemAPI.TestPackage;
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

        private QcProjectMetadataDTO CreateQcProjectMetadata(int projectKey, string testPackageName, string status)
        {
            return new QcProjectMetadataDTO
            {
                ProjectId = QcProjectMetadataDefaults.PROJECT_MAP_ID,
                GroupName = QcProjectMetadataDefaults.PROJECT_MAP_GROUP_NAME,
                VarName = string.Format("0-{0}-{1}", testPackageName, status),
                IntValue = projectKey,
                FloatValue = null,
                TextValue = null,
                Comment = QcProjectMetadataDefaults.COMMENT
            };
        }

        private void CreateQcProjectMetadata(int projectKey, string testPackageName)
        {
            var qcProjectMetadataDtos = from status in QcProjectMetadataDefaults.Statuses
                                        select CreateQcProjectMetadata(projectKey, testPackageName, status);

            qcProjectMetadataDao.Insert(qcProjectMetadataDtos.ToList());
        }

        private int ProjectKey(string projectDescription)
        {
            var projectList = projectDao.Find(projectDescription);
            if (projectList == null || !projectList.Any())
            {
                throw new InvalidOperationException(string.Format("Could not find a TIS project for '{0}'", projectDescription));
            }

            var project = projectList.First();

            return project.ProjectKey;
        }

        public void CreateQcProjectMetadata(TestPackage.TestPackage testPackage)
        {
            var projectKey = ProjectKey(QcProjectMetadataDefaults.TEST_PACKAGE_XSD_PROJECT_DESCRIPTION);
            
            if (testPackage.IsCombined())
            {
                var combinedTestName = testPackage.GetTestPackageKey();
                CreateQcProjectMetadata(projectKey, combinedTestName);

                qcProjectMetadataDao.Insert(CreateQcProjectMetadata(projectKey, combinedTestName, "pending"));

                projectKey = ProjectKey(QcProjectMetadataDefaults.TEST_PACKAGE_XSD_PROJECT_COMBINED_PENDING_DESCRIPTION);
                var qcProjectMetadataDtos = from test in testPackage.Test
                                            select CreateQcProjectMetadata(projectKey, test.Key, "pending");

                qcProjectMetadataDao.Insert(qcProjectMetadataDtos.ToList());

                projectKey = ProjectKey(QcProjectMetadataDefaults.TEST_PACKAGE_XSD_PROJECT_COMBINED_DESCRIPTION);
            }

            foreach (Test t in testPackage.Test)
            {
                CreateQcProjectMetadata(projectKey, t.Key);
            }

        }
    }
}
