using System;
using System.Collections.Generic;
using System.Linq;
using TDSQASystemAPI.DAL;
using TDSQASystemAPI.DAL.itembank.daos;
using TDSQASystemAPI.DAL.itembank.dtos;

namespace TDSQASystemAPI.BL.testpackage.administration
{
    public class ItembankConfigurationDataService : IItembankConfigurationDataService
    {
        private readonly ITestPackageDao<SubjectDTO> subjectDAO;
        private readonly ITestPackageDao<ClientDTO> clientDAO;

        /// <summary>
        /// Default constructor to create new <code>ITestPackageDao<SubjectDTO></code> and
        /// <code>ITestPackageDao<ClientDTO></code> since a dependency injection container has not been 
        /// implemented.
        /// </summary>
        public ItembankConfigurationDataService()
        {
            subjectDAO = new SubjectDAO();
            clientDAO = new ClientDAO();
        }

        public ItembankConfigurationDataService(ITestPackageDao<SubjectDTO> subjectDAO, 
                                                ITestPackageDao<ClientDTO> clientDAO)
        {
            this.subjectDAO = subjectDAO;
            this.clientDAO = clientDAO;
        }

        public void CreateSubject(TestPackage.TestPackage testPackage)
        {
            var subjectKey = string.Format("{0}-{1}", testPackage.publisher, testPackage.subject);

            // If a subject already exists, exit; there's nothing to do
            var existingSubject = subjectDAO.Find(subjectKey);
            if (existingSubject != null && existingSubject.Any())
            {
                return;
            }

            var clientList = clientDAO.Find(testPackage.publisher);
            if (clientList == null || !clientList.Any())
            {
                throw new InvalidOperationException(string.Format("Could not find a client record with name '{0}'", testPackage.publisher));
            }

            var newSubjectDto = new SubjectDTO
            {
                Name = testPackage.subject,
                Grade = string.Empty, // from line 25 of tp.spLoad_Subject
                SubjectKey = subjectKey,
                ClientKey = clientList[0].ClientKey, 
                TestVersion = (long)testPackage.version
            };

            subjectDAO.Insert(new List<SubjectDTO> { newSubjectDto });
        }
    }
}
