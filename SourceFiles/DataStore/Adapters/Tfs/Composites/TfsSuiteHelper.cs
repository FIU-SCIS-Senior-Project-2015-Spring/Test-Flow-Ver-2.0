using DataStore.Adapters.Composites;
using Microsoft.TeamFoundation.TestManagement.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataStore.Adapters.Tfs.Composites
{
    public class TfsSuiteHelper : TfsCompositeBase, ISuiteHelper
    {
        private ITestPlan testPlan;
        public TfsSuiteHelper(ITestManagementService testManagementService, string projectName, int testPlanId)
            : base(testManagementService, projectName)
        {
            Microsoft.TeamFoundation.TestManagement.Client.ITestPlanHelper planHelper = this.testManagementProject.TestPlans;
            testPlan = planHelper.Find(testPlanId);
        }
    
        public int Create(TestSuite item)
        {
            try
            {
                IStaticTestSuite suite = null;

                IStaticTestSuite newSuite = this.testManagementProject.TestSuites.CreateStatic();

                newSuite.Title = item.Name;
                newSuite.Description = item.Description;

                if (item.Parent > 0)
                    suite = FindSuite(testPlan.RootSuite.Entries, item.Parent);

                if (suite != null)
                    suite.Entries.Add(newSuite);
                else
                    testPlan.RootSuite.Entries.Add(newSuite);

                testPlan.Save();

                return newSuite.Id;
            }
            catch(Exception e)
            {
                return -1;
            }
        }

        public bool Edit(TestSuite item)
        {
            try
            {
                IStaticTestSuite suite = null;

                suite = FindSuite(testPlan.RootSuite.Entries, item.ExternalId);

                if(suite != null)
                {
                    suite.Title = item.Name;
                    suite.Description = (item.Description != null) ? item.Description : "";

                    testPlan.Save();
                    return true;
                }
                return false;
            }
            catch(Exception e)
            {
                return false;
            }
        }

        public TestSuite Get(int id)
        {
            
            IStaticTestSuite suite = FindSuite(testPlan.RootSuite.Entries, id);

            if (suite != null)
            {
                TestSuite result = new TestSuite();
                result.Name = suite.Title;
                result.Description = suite.Description;
                result.ExternalId = suite.Id;
                result.Parent = suite.Parent.Id;
                return result;
            }
            else
                return null;
        }

        public List<TestSuite> GetFromParent(int parentId)
        {
            Microsoft.TeamFoundation.TestManagement.Client.ITestPlanHelper planHelper = this.testManagementProject.TestPlans;
            ITestPlan testPlan = planHelper.Find(parentId);

            return getSuites(testPlan.RootSuite.Entries);
        }

        /// <summary>
        /// Retrieves all child suites form a given suite collection
        /// </summary>
        /// <param name="parentSuite">the parent suite</param>
        /// <param name="parentId">the parents id</param>
        /// <returns>all sub suites down 1 level.</returns>
        private List<TestSuite> getSuites(ITestSuiteEntryCollection parentSuite, int parent = 0)
        {
            List<TestSuite> suites = new List<TestSuite>();
            // iterate over the TFS Suite Sub Suite colleciton
            foreach (ITestSuiteEntry entry in parentSuite)
            {
                IStaticTestSuite tfsSuite = entry.TestSuite as IStaticTestSuite;
                if (tfsSuite == null)
                    continue;
                // import into our test model
                TestSuite suite = new TestSuite();
                suite.Name = tfsSuite.Title;
                suite.ExternalId = tfsSuite.Id;
                suite.Description = tfsSuite.Description;
                suite.ParentExternalId = parent;
                // recursively get children
                if (tfsSuite.Entries.Count > 0)
                    suite.SubSuites = getSuites(tfsSuite.Entries, suite.ExternalId);
                suites.Add(suite);
            }
            return suites;
        }

        private IStaticTestSuite FindSuite(ITestSuiteEntryCollection collection, int id)
        { 
            foreach (ITestSuiteEntry entry in collection) 
            { 
                IStaticTestSuite suite = entry.TestSuite as IStaticTestSuite; 

                if (suite != null) 
                { 
                    if (suite.Id == id) 
                        return suite; 
                    else if (suite.Entries.Count > 0) 
                        FindSuite(suite.Entries, id); 
                } 
            } 
            return null; 
        } 
    }
}


