using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TDSQASystemAPI.DAL;
using TDSQASystemAPI.DAL.configs.dtos;

namespace TDSQASystemAPI.BL.testpackage.administration
{
    class ConfigsDataService
    {
        private readonly ITestPackageDao<ClientDTO> clientDAO;
        private readonly ITestPackageDao<TimeWindowDTO> timeWindowDAO;
        private readonly ITestPackageDao<SubjectDTO> subjectDAO;

        public ConfigsDataService(
            ITestPackageDao<ClientDTO> clientDAO,
            ITestPackageDao<TimeWindowDTO> timeWindowDAO,
            ITestPackageDao<SubjectDTO> subjectDAO)
        {
            this.clientDAO = clientDAO;
            this.timeWindowDAO = timeWindowDAO;
            this.subjectDAO = subjectDAO;
        }

        public void LoadSeedData(TestPackage.TestPackage testPackage)
        {
            string clientName = testPackage.publisher;

            CreateTesteeAttribute(clientName);
            CreateTesteeRelationshipAttribute(clientName);

            var clientDTO = new List<ClientDTO>();
            clientDTO.Add(new ClientDTO()
            {
                Name = clientName
            });
            clientDAO.Insert(clientDTO);

            CreateTimeWindow(clientName);
            CreateFieldtestPriority(clientName);
            InsertClientGradeData(testPackage);
            InsertClientSubject(clientName, testPackage.subject);
        }

        private void CreateTesteeAttribute(string clientName)
        {
            // copies rows from TDSCONFIGS_TDS_TesteeAttribute 
            // to TDSCONFIGS_Client_TesteeAttribute
            const string SQL =
                "insert into TDSCONFIGS_Client_TesteeAttribute (clientname, TDS_ID, RTSName, type, Label, reportName, atLogin, SortOrder)" +
                "select cname, TDS_ID, RTSName, type, label, reportName, atLogin, SortOrder " +
                "from TDSCONFIGS_TDS_TesteeAttribute " +
                "where not exists(select * from TDSCONFIGS_Client_TesteeAttribute where clientname = :clientName);";

            using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings[DatabaseConnectionStringNames.CONFIGS].ConnectionString))
            {
                using (var command = new SqlCommand(SQL, connection))
                {
                    command.CommandType = CommandType.Text;
                    var testeeAttributeParam = command.Parameters.AddWithValue("clientName", clientName);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        private void CreateTesteeRelationshipAttribute(string clientName)
        {
            // copies rows from TDSCONFIGS_TDS_TesteeRelationshipAttribute 
            // to TDSCONFIGS_Client_TesteeRelationshipAttribute
            const string SQL =
                "insert into TDSCONFIGS_Client_TesteeRelationshipAttribute(clientname, TDS_ID, RTSName, Label, reportName, atLogin, SortOrder, relationshipType) " +
                "select cname, TDS_ID, RTSName, label, reportName, atLogin, SortOrder, relationshipType " +
                "from TDSCONFIGS_TDS_TesteeRelationshipAttribute " +
                "where not exists(select * from TDSCONFIGS_Client_TesteeRelationshipAttribute where clientname = :clientName);";

            using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings[DatabaseConnectionStringNames.CONFIGS].ConnectionString))
            {
                using (var command = new SqlCommand(SQL, connection))
                {
                    command.CommandType = CommandType.Text;
                    var testeeAttributeParam = command.Parameters.AddWithValue("clientName", clientName);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        private void CreateTimeWindow(string clientName)
        {
            var now = DateTime.Now;
            TimeWindowDTO clientTimeWindow = new TimeWindowDTO()
            {
                ClientName = clientName,
                WindowId = "ANNUAL",
                StartDate = now,
                EndDate = now.AddYears(10)
            };

            timeWindowDAO.Insert(clientTimeWindow);
        }

        private void CreateFieldtestPriority(string clientName)
        {
            // copies rows from TDSCONFIGS_TDS_FieldtestPriority
            // to TDSCONFIGS_Client_FieldtestPriority
            const string SQL =
                "insert into TDSCONFIGS_Client_FieldtestPriority (clientname, TDS_ID, priority, TestID) " +
                "select cname, P.TDS_ID, priority, '*' " +
                "from TDSCONFIGS_TDS_FieldtestPriority P, TDSCONFIGS_Client_TesteeAttribute A " +
                "where A.clientname = :clientName and A.TDS_ID = P.TDS_ID " +
                "and :clientName not in " +
                "(select clientname from TDSCONFIGS_Client_FieldtestPriority); ";

            using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings[DatabaseConnectionStringNames.CONFIGS].ConnectionString))
            {
                using (var command = new SqlCommand(SQL, connection))
                {
                    command.CommandType = CommandType.Text;
                    var testeeAttributeParam = command.Parameters.AddWithValue("clientName", clientName);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        private void InsertClientGradeData(TestPackage.TestPackage testPackage)
        {
            /*
            Set<Grade> grades = testPackage.getAssessments().stream()
                       .flatMap(assessment->assessment.getGrades().stream())
                       .map(grade->
                           //TODO: Update this once label is part of <Grade>
                           new Grade(grade.getValue(),
                               StringUtils.isNumeric(grade.getValue()) ? "Grade " + grade.getValue() : grade.getValue(),
                               testPackage.getPublisher()))
                       .collect(Collectors.toSet());

            gradeRepository.save(grades);

            List<AssessmentGrade> assessmentGrades = testPackage.getAssessments().stream()
                .flatMap(assessment->assessment.getGrades().stream()
                    .map(grade-> new AssessmentGrade(testPackage.getPublisher(), assessment.getId(), grade.getValue())))
                .collect(Collectors.toList());

            assessmentGradeRepository.save(assessmentGrades);
            */
            HashSet<GradeDTO> grades = new HashSet<GradeDTO>(testPackage.Assessment.
                SelectMany(assessment => assessment.Grades).
                Select(grade => new GradeDTO()
                {
                    ClientName = testPackage.publisher,
                    GradeCode = grade.value,
                    Grade = grade.label
                }));
            // where T.testkey = S._fk_AdminSubject and T.testkey not like '%student help%'
        }


        private void InsertClientSubject(string client, string testkey)
        {
            string SQL = "insert into TDSCONFIGS_Client_Subject(clientname, Subject, SubjectCode)\n" +
                "    select distinct :client, S.Name\n" +
                "        , (select SubjectCode from SubjectCodes C where C.subject = S.Name)\n" +
                "    from tblSubject S, tblsetofadminsubjects A\n" +
                "    where A._Key = :testkey and A._fk_Subject = S._Key and :testkey not like '%student help%'\n" +
                "    and not exists (select * from TDSCONFIGS_Client_Subject C where c.clientname = :client and C.Subject = S.Name);\n";
        }

        private void InsertClientAccommodationFamily()
        {
            string SQL = "insert into TDSCONFIGS_Client_AccommodationFamily (clientname, family, label)\n" +
                "    select distinct T.client, SubjectCode, T.Subject\n" +
                "    from @tests T, SubjectCodes S\n" +
                "    where T.Subject = S.Subject \n" +
                "        and not exists (select * from TDSCONFIGS_Client_AccommodationFamily F where F.clientname = T.client and F.family = S.SubjectCode);\n";

            // alternatively

            var accommodationFamilyDTO = new List<AccommodationFamilyDTO>();
            accommodationFamilyDTO.Add(new AccommodationFamilyDTO()
            {
                Name = clientName
            });
            //accommodationFamilyDAO.Insert(accommodationFamilyDTO);

        }

        private void InsertClientTestProperties()
        {
            string SQL = "    insert into TDSCONFIGS_Client_TestProperties \n" +
            "        (ClientName, TestID, IsSelectable, Label, SubjectName, MaxOpportunities, ScoreByTDS, AccommodationFamily,  ReportingInstrument, TIDE_ID, gradeText)\n" +
            "    select distinct client, test, dbo.IsSelectable(testKey), dbo._MakeTestLabel(client, test) as Label, T.subject, 3, dbo.IsSelectable(testKey), \n" +
            "        (select Family from TDSCONFIGS_Client_AccommodationFamily F where T.client = F.clientname and T.subject = F.Label)\n" +
            "        , instrument, case when instrument is not null then instrument + '-' + subject else null end\n" +
            "        , dbo._MakeTestGradeLabel(client, test)\n" +
            "    from @tests T\n" +
            "    where not exists (select * from TDSCONFIGS_Client_TestProperties where ClientName = client and TestID = test);\n";
        }

        private void InsertClientTestMode()
        {
            /*
            --add physical tests where none exist for the logical test(-FX - tests are BOTH online AND paper)

   insert into TDSCONFIGS_Client_TestMode(clientname, testID, testkey, mode, sessionType)

   select client, test, testkey, 'online', 0

   from @tests T where testkey not like '%paper%' and testkey not like '%-fx-%'-- TODO: add FX tests as both online and paper

   and not exists(select * from TDSCONFIGS_Client_TestMode where clientname = client and testID = test and mode = 'online');

            insert into TDSCONFIGS_Client_TestMode(clientname, testID, testkey, mode, sessionType)
    select client, test, testkey, 'paper', 1
    from @tests T where (testkey like '%paper%' or testkey like '%-FX-%')
    and not exists(select * from TDSCONFIGS_Client_TestMode where clientname = client and testID = test and mode = 'paper');

            --insert the physical tests in a 'dormant' state(sessionType = 99) if they are not in the Test Mode table
    insert into TDSCONFIGS_Client_TestMode(clientname, testID, testkey, mode, sessionType)
    select client, test, testkey, 'online', 99
    from @tests T where testkey not like '%paper%'
    and not exists
    (select * from TDSCONFIGS_Client_testMode where testkey = T.testKey);

            insert into TDSCONFIGS_Client_TestMode(clientname, testID, testkey, mode, sessionType)
    select client, test, testkey, 'paper', 99
    from @tests T where (testkey like '%paper%' or testkey like '%-FX-%')
    and not exists
    (select * from TDSCONFIGS_Client_testMode where testkey = T.testKey);
            --

                update TDSCONFIGS_Client_TestMode set isSegmented = T.IsSegmented
    from @tests T where TDSCONFIGS_Client_TestMode.testkey = T.testkey;

            update TDSCONFIGS_Client_testMode set algorithm = selectionAlgorithm
    from tblSetofAdminSubjects S, @tests T
    where S._Key = T.testkey and S._Key = TDSCONFIGS_Client_TestMode.testkey;
    */
        }
    }


}
