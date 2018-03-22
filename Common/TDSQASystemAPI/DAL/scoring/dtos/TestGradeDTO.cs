namespace TDSQASystemAPI.DAL.scoring.dtos
{
    /// <summary>
    /// Represents a single record in the <code>OSS_TestScoringConfigs..TestGrades</code> table
    /// </summary>
    public class TestGradeDTO
    {
        public string ClientName { get; set; }
        public string TestId { get; set; }
        public string ReportingGrade { get; set; }
        public override bool Equals(object obj)
        {
            var testGradeDTO = obj as TestGradeDTO;

            if (testGradeDTO == null)
            {
                return false;
            }

            return this.ClientName.Equals(testGradeDTO.ClientName) && 
                this.TestId.Equals(testGradeDTO.TestId) && this.
                ReportingGrade.Equals(testGradeDTO.ReportingGrade);
        }

        public override int GetHashCode()
        {
            unchecked 
            {
                int hash = 17;
                if (ClientName != null)
                {
                    hash = hash * 23 + ClientName.GetHashCode();
                }
                if (TestId != null)
                {
                    hash = hash * 23 + TestId.GetHashCode();
                }
                if (ReportingGrade != null)
                {
                    hash = hash * 23 + ReportingGrade.GetHashCode();
                }
                return hash;
            }
        }
    }
}
