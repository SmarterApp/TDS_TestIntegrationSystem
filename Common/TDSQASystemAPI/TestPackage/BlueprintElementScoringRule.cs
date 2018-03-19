using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TDSQASystemAPI.TestPackage
{
    public partial class BlueprintElementScoringRule
    {
    
        private Guid id;

        /// <summary>
        /// The unique identifier for this <code>BlueprintElementScoringRule</code>.
        /// </summary>
        public Guid Id
        {
            get
            {
                if (id == null)
                {
                    id = Guid.NewGuid();
                }
                return id;
            }
        }

    }
}
