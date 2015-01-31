using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.TestManagement.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DataStore.Adapters.Tfs.Composites
{
    /// <summary>
    /// Base composite class for TFS extened composites, provides TFS service accessiblity.
    /// </summary>
    public class TfsCompositeBase
    {
        protected ITestManagementService testManagementService;
        protected ITestManagementTeamProject testManagementProject = null;

        /// <summary>
        /// Base constructor for the TFS Composite extended classes, initializes the Management Service for TFS
        /// </summary>
        /// <param name="testManagementService"> ITestManagementService from TFS manager</param>
        public TfsCompositeBase(ITestManagementService testManagementService, string projectName)
        {
            this.testManagementService = testManagementService;
            try
            {
                this.testManagementProject = this.testManagementService.GetTeamProject(projectName);
            }
            catch(Exception e)
            {

            }
        }
    }
}
