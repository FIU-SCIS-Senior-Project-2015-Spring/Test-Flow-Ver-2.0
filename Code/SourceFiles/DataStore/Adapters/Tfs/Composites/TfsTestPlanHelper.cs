using DataStore.Adapters.Composites;
using DataStore.EntityData;
using Microsoft.TeamFoundation.TestManagement.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataStore.Adapters.Tfs.Composites
{
    public class TfsTestPlanHelper : TfsCompositeBase, DataStore.Adapters.Composites.ITestPlanHelper
    {
        public TfsTestPlanHelper(ITestManagementService testManagementService, string projectName)
            : base(testManagementService, projectName)
        { }
        public int Create(TestPlan item)
        {
            Microsoft.TeamFoundation.TestManagement.Client.ITestPlanHelper planHelper = this.testManagementProject.TestPlans;

            ITestPlan testPlan = planHelper.Create();
            testPlan.Name = item.Name;

            try
            {
                testPlan.Save();
                return testPlan.Id;
            }
            catch (Exception e)
            {
                return -1;
            }
        }

        public bool Edit(TestPlan item)
        {
            // get the project and plan helper
            Microsoft.TeamFoundation.TestManagement.Client.ITestPlanHelper planHelper = this.testManagementProject.TestPlans;

            // find the right plan
            ITestPlan tfsPlan = planHelper.Find(item.ExternalId);

            tfsPlan.Name = item.Name;
            try
            {
                tfsPlan.Save();
                return true;
            }
            catch(Exception e)
            {
                return false;
            }
        }

        public TestPlan Get(int id)
        {
            // get the project and plan helper
            Microsoft.TeamFoundation.TestManagement.Client.ITestPlanHelper planHelper = this.testManagementProject.TestPlans;

            // find the right plan
            ITestPlan tfsPlan = planHelper.Find(id);

            // import to our test model
            TestPlan plan = new TestPlan();
            plan.Name = tfsPlan.Name;
            plan.ExternalId = tfsPlan.Id;
            plan.Project = new Project();
            plan.Project.Name = this.testManagementProject.TeamProjectName;

            return plan;
        }
        /// <summary>
        /// Retrieves all of the test plans under a given project
        /// </summary>
        /// <param name="parentId">Id of project</param>
        /// <returns>list of test plans under a given project</returns>
        public List<TestPlan> GetFromParent(int parentId)
        {
            // setup project
            Microsoft.TeamFoundation.TestManagement.Client.ITestPlanHelper planHelper = this.testManagementProject.TestPlans;

            // query and import all test plans found
            List<TestPlan> planList = new List<TestPlan>();
            foreach (ITestPlan p in planHelper.Query("Select * From TestPlan"))
            {
                TestPlan tp = new TestPlan();
                tp.ExternalId = p.Id;
                tp.Name = p.Name;
                planList.Add(tp);
            }

            return planList;
        }
        
    }
}
