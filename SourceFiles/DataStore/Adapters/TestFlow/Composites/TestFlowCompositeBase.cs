using DataStore.EntityData;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.TestManagement.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DataStore.Adapters.TestFlow.Composites
{
    /// <summary>
    /// Base composite class for Test Flow composite extended classes.
    /// </summary>
    public class TestFlowCompositeBase
    {
        protected testflowEntities context;

        /// <summary>
        /// Base constructor for the Test Flow composite classes, initilizes the database context
        /// </summary>
        /// <param name="context"> database context</param>
        public TestFlowCompositeBase(testflowEntities context)
        {
            this.context = context;
        }
    }
}
