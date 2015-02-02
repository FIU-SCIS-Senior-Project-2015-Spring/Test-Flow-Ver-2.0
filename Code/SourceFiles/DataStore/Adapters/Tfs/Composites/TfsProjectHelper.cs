using DataStore.Adapters.Composites;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.TestManagement.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataStore.Adapters.Tfs.Composites
{
    public class TfsProjectHelper : TfsCompositeBase, IProjectHelper
    {
        private TfsTeamProjectCollection tpc;
        public TfsProjectHelper(ITestManagementService testManagementService, TfsTeamProjectCollection tpc, string projectName)
            : base(testManagementService, projectName)
        {
            this.tpc = tpc;
        }
    
        public int Create(Project item)
        {
            throw new NotImplementedException("Test Flow does not offer TFS Project Creation.");
        }

        public bool Edit(Project item)
        {
            throw new NotImplementedException("Test Flow does not offer TFS Project Editing.");
        }

        public Project Get(int id)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Retrieves all of the projects avaliable to this user for a given TFS Collection
        /// </summary>
        /// <returns>List of serializable projects</returns>
        public List<Project> GetFromParent(int parentId)
        {
            var workItemStore = new WorkItemStore(tpc);
            // returns list of tfs projects
            var tfsProjectList = (from Microsoft.TeamFoundation.WorkItemTracking.Client.Project pr in workItemStore.Projects select pr).ToList();

            // convert list into test flow project list
            List<Project> projectList = new List<Project>();
            foreach (Microsoft.TeamFoundation.WorkItemTracking.Client.Project tfsProj in tfsProjectList)
            {
                Project proj = new Project();
                proj.Name = tfsProj.Name;
                proj.ExternalId = tfsProj.Id;
                projectList.Add(proj);
            }

            return projectList;
        }
    }
}
