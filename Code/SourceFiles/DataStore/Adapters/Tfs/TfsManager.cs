using DataStore.Adapters.Tfs.Composites;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.TestManagement.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Principal;
using System.Text;

namespace DataStore.Adapters.Tfs
{
    public class TfsManager : IDataStoreAdapter
    {
        private ICredentials credentials;
        private ITestManagementService testManagementService;
        private TfsTeamProjectCollection teamProjectCollection;
        private string projectName;

        private TfsProjectHelper projectsHelper;
        private TfsTestPlanHelper testPlanHelper;
        private TfsSuiteHelper suiteHelper;
        private TfsTestCaseHelper testCaseHelper;
        private TfsStepHelper stepHelper;

        public DataStore.Adapters.Composites.IProjectHelper Projects
        {
            get { return projectsHelper; }
        }

        public DataStore.Adapters.Composites.ITestPlanHelper TestPlans
        {
            get { return testPlanHelper; }
        }

        public DataStore.Adapters.Composites.ISuiteHelper Suites
        {
            get { return suiteHelper; }
        }

        public DataStore.Adapters.Composites.ITestCaseHelper TestCases
        {
            get { return testCaseHelper; }
        }

        public DataStore.Adapters.Composites.IStepHelper Steps
        {
            get { return stepHelper; }
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public Uri Host { get; set; }
        /// <summary>
        /// Constructor which provides access to management services.
        /// </summary>
        public TfsManager(Uri host)
        {
            Host = host;
            credentials = new NetworkCredential("TFS", "test123"); //CredentialCache.DefaultNetworkCredentials only works using digest;

            teamProjectCollection = TfsTeamProjectCollectionFactory.GetTeamProjectCollection(Host);
            //ICredentials crds = new NetworkCredential("TFS", "test123");
            teamProjectCollection.Credentials = credentials;

            TfsTeamProjectCollection tfsCollection = new TfsTeamProjectCollection(Host, credentials);
            testManagementService = teamProjectCollection.GetService<ITestManagementService>();

            projectsHelper = new TfsProjectHelper(testManagementService, teamProjectCollection, "");
        }

        /// <summary>
        /// This constructor only provides testplan and project helpers, you must provide testplan Id for suites, testcases, and step helpers.
        /// </summary>
        /// <param name="projectName"></param>
        public TfsManager(Uri host, string projectName) : this(host)
        {
            projectsHelper = new TfsProjectHelper(testManagementService, teamProjectCollection, projectName);

            testPlanHelper = new TfsTestPlanHelper(testManagementService, projectName);

            this.projectName = projectName;
        }
        /// <summary>
        /// This constructor provides access to every helper but test steps, which needs a test case id
        /// </summary>
        /// <param name="projectName"></param>
        /// <param name="testPlanId"></param>
        public TfsManager(Uri host, string projectName, int testPlanId)
            : this(host)
        {

            projectsHelper = new TfsProjectHelper(testManagementService, teamProjectCollection, projectName);

            testPlanHelper = new TfsTestPlanHelper(testManagementService, projectName);

            suiteHelper = new TfsSuiteHelper(testManagementService, projectName, testPlanId);

            testCaseHelper = new TfsTestCaseHelper(testManagementService, projectName, testPlanId);

            this.projectName = projectName;
        }

        /// <summary>
        /// Final constructor which provides access to all helpers.
        /// </summary>
        /// <param name="projectName"></param>
        /// <param name="testPlanId"></param>
        /// <param name="testCaseId"></param>
        public TfsManager(Uri host, string projectName, int testPlanId, int testCaseId)
            : this(host)
        {

            projectsHelper = new TfsProjectHelper(testManagementService, teamProjectCollection, projectName);

            testPlanHelper = new TfsTestPlanHelper(testManagementService, projectName);

            suiteHelper = new TfsSuiteHelper(testManagementService, projectName, testPlanId);

            testCaseHelper = new TfsTestCaseHelper(testManagementService, projectName, testPlanId);

            stepHelper = new TfsStepHelper(testManagementService, projectName, testCaseId);

            this.projectName = projectName;
        }


        public void CreateStepHelper(int testCaseId)
        {
            stepHelper = new TfsStepHelper(testManagementService, projectName, testCaseId);
        }
    }
}