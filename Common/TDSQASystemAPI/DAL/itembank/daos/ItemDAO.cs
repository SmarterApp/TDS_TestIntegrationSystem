using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using TDSQASystemAPI.DAL.itembank.dtos;

namespace TDSQASystemAPI.DAL.itembank.daos
{
    /// <summary>
    /// A class for saving <code>ItemDTO</code>s to the <code>OSS_Itembank..tblItem</code> table
    /// </summary>
    public class ItemDAO : TestPackageDaoBase<ItemDTO>
    {
        /// <summary>
        /// A class defining the ordinal position of the columns in the SELECT query that returns item data
        /// </summary>
        class ItemQueryColumnPositions
        {
            internal const int ITEM_BANK_KEY = 0;
            internal const int ITEM_KEY = 1;
            internal const int ITEM_TYPE = 2;
            internal const int SCORE_POINTS = 3;
            internal const int FILE_PATH = 4;
            internal const int FILE_NAME = 5;
            internal const int DATE_LAST_UPDATED = 6;
            internal const int KEY = 7;
            internal const int TEST_VERSION = 8;
        }

        public ItemDAO()
        {
            DbConnectionStringName = DatabaseConnectionStringNames.ITEMBANK;
            TvpType = "ItemTable";
            InsertSql =
                "INSERT \n" +
                "   dbo.tblItem (" +
                "       _efk_ItemBank, " +
                "       _efk_Item, " +
                "       ItemType, " +
                "       ScorePoint," +
                "       FilePath, " +
                "       FileName, " +
                "       DateLastUpdated, " +
                "       _Key, " +
                "       LoadConfig) \n" +
                "SELECT \n" +
                "   ItemBankKey, \n" +
                "   ItemKey, \n" +
                "   ItemType, \n" +
                "   ScorePoints, \n" +
                "   FilePath, \n" +
                "   [FileName], \n" +
                "   DateLastUpdated, \n" +
                "   [Key], \n" +
                "   TestVersion \n";
            SelectSql =
                "SELECT \n" +
                "   _efk_ItemBank AS ItemBankKey, \n" +
                "   _efk_Item AS ItemKey, \n" +
                "   ItemType, \n" +
                "   ScorePoint AS ScorePoints, \n" +
                "   FilePath, \n" +
                "   [FileName], \n" +
                "   DateLastUpdated, \n" +
                "   _Key AS [Key], \n" +
                "   LoadConfig AS TestVersion \n" +
                "FROM \n" +
                "   tblItem \n" +
                "WHERE \n" +
                "   _Key IN ({0})";
        }

        /// <summary>
        /// Override the <code>Find()</code> method to identify items in this <code>TestPackage</code> that already exist
        /// in the database.
        /// </summary>
        /// <remarks>
        /// In the event the <code>TestPackage</code> being loaded contains items that already exist in the 
        /// <code>OSS_Itembank..tblItem</code>, the existing items should _not_ be overwritten.
        /// </remarks>
        /// <param name="criteria">In this case, the criteria are a collection of all the <code>ItemDTO</code>s that
        /// are included in the <code>TestPackage</code> being loaded.</param>
        /// <returns>A collection of <code>ItemDTO</code>s that already exist in <code>OSS_Itembank..tblItem</code></returns>
        public override List<ItemDTO> Find(object criteria)
        {
            if (!(criteria is IEnumerable<ItemDTO>))
            {
                throw new ArgumentException(string.Format("This method expects a criteria type of IEnumerable<ItemDTO>; criteria passed in is {0}", criteria.GetType()));
            }

            var results = new List<ItemDTO>();
            var itemKeys = from item in criteria as IEnumerable<ItemDTO>
                           select string.Format("'{0}'", item.Key);

            using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings[DbConnectionStringName].ConnectionString))
            {
                using (var command = new SqlCommand(string.Format(SelectSql, string.Join(",", itemKeys)), connection))
                {
                    command.CommandType = CommandType.Text;

                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        results = GetItemDtoListFromDataReader(reader);
                    }
                }
            }

            return results;
        }

        /// <summary>
        /// Get a collection of <code>ItemDTO</code>s from the results of the query. 
        /// </summary>
        /// <param name="reader">An <code>IDataReader</code> implementation containing the query results.</param>
        /// <returns>A <code>List<ItemDTO></code> of items that exist in the <code>OSS_Itembank..tblItem</code> table.</returns>
        private List<ItemDTO> GetItemDtoListFromDataReader(IDataReader reader)
        {
            var itemDtos = new List<ItemDTO>();
            while (reader.Read())
            {
                itemDtos.Add(GetItemDtoFromDataReader(reader));
            }

            return itemDtos;
        }

        /// <summary>
        /// Get an <code>ItemDTO</code> from a record in the query results.
        /// </summary>
        /// <param name="reader">An <code>IDataReader</code> implementation containing the query results.</param>
        /// <returns>An <code>ItemDTO</code> representing a record from the database.</returns>
        private ItemDTO GetItemDtoFromDataReader(IDataReader reader)
        {
            return new ItemDTO
            {
                ItemBankKey = reader.GetInt64(ItemQueryColumnPositions.ITEM_BANK_KEY),
                ItemKey = reader.GetInt64(ItemQueryColumnPositions.ITEM_KEY),
                ItemType = reader.GetString(ItemQueryColumnPositions.ITEM_TYPE),
                ScorePoints = reader.GetInt32(ItemQueryColumnPositions.SCORE_POINTS),
                FilePath = reader.GetString(ItemQueryColumnPositions.FILE_PATH),
                FileName = reader.GetString(ItemQueryColumnPositions.FILE_NAME),
                DateLastUpdated = reader.GetDateTime(ItemQueryColumnPositions.DATE_LAST_UPDATED),
                Key = reader.GetString(ItemQueryColumnPositions.KEY),
                TestVersion = reader.GetInt64(ItemQueryColumnPositions.TEST_VERSION)
            };
        }
    }
}
