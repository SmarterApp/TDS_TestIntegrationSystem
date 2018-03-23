using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TDSQASystemAPI.DAL.utils
{
    /// <summary>
    /// A class that uses reflection to create a list of objects from a <code>IDataReader</code> implementation.
    /// </summary>
    /// <seealso cref="https://www.codeproject.com/Articles/1009908/Generic-ListHelper-Class-NET"/>
    /// <typeparam name="T">The type to be created from the results contained in the <code>IDataReader</code></typeparam>
    internal class ReflectionObjectPopulator<T>
    {
        /// <summary>
        /// Iterate through a <code>IDataReader</code> implementation and get back a list of objects.
        /// </summary>
        /// <remarks>
        /// The property names in the <code>IDataReader</code> must match the object's property names exactly.
        /// </remarks>
        /// <param name="reader">The <code>IDataReader</code> to iterate through.</param>
        /// <returns>A collection of <typeparamref name="T"/> read from the database</returns>
        public virtual List<T> GetListFromDataReader(IDataReader reader)
        {
            var results = new List<T>();
            var properties = typeof(T).GetProperties();

            while (reader.Read())
            {
                var item = Activator.CreateInstance<T>();
                foreach (var property in typeof(T).GetProperties())
                {
                    if (!reader.IsDBNull(reader.GetOrdinal(property.Name)))
                    {
                        Type convertTo = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
                        property.SetValue(item, Convert.ChangeType(reader[property.Name], convertTo), null);
                    }
                }

                results.Add(item);
            }

            return results;
        }
    }
}
