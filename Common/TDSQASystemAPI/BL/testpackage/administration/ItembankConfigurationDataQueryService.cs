using System.Linq;
using TDSQASystemAPI.DAL;
using TDSQASystemAPI.DAL.itembank.daos;
using TDSQASystemAPI.DAL.itembank.dtos;

namespace TDSQASystemAPI.BL.testpackage.administration
{
    public class ItembankConfigurationDataQueryService : IItembankConfigurationDataQueryService
    {
        private readonly ITestPackageDao<SubjectDTO> subjectDAO;
        private readonly ITestPackageDao<ClientDTO> clientDAO;
        private readonly ITestPackageDao<TestAdminDTO> testAdminDAO;

        public ItembankConfigurationDataQueryService()
        {
            subjectDAO = new SubjectDAO();
            clientDAO = new ClientDAO();
            testAdminDAO = new TestAdminDAO();
        }

        public ItembankConfigurationDataQueryService(ITestPackageDao<SubjectDTO> subjectDAO, 
                                                     ITestPackageDao<ClientDTO> clientDAO,
                                                     ITestPackageDao<TestAdminDTO> testAdminDAO)
        {
            this.subjectDAO = subjectDAO;
            this.clientDAO = clientDAO;
            this.testAdminDAO = testAdminDAO;
        }

        public ClientDTO FindClientByName(string clientName)
        {
            var clientList = clientDAO.Find(clientName);
            
            return (clientList == null || !clientList.Any())
                ? null
                : clientList.First();
        }

        public SubjectDTO FindSubject(string subjectKey)
        {
            var subjectList = subjectDAO.Find(subjectKey);

            return (subjectList == null || !subjectList.Any())
                ? null
                : subjectList.First();
        }

        public TestAdminDTO FindTestAdmin(string clientName)
        {
            var testAdminList = testAdminDAO.Find(clientName);

            return (testAdminList == null || !testAdminList.Any())
                ? null
                : testAdminList.First();
        }
    }
}
