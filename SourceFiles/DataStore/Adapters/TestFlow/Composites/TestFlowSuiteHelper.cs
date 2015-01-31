using DataStore.Adapters.Composites;
using DataStore.EntityData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataStore.Adapters.TestFlow.Composites
{
    class TestFlowSuiteHelper : TestFlowCompositeBase, ISuiteHelper
    {
        public TestFlowSuiteHelper(testflowEntities context)
            : base(context)
        { }
    
        public int Create(TestSuite item)
        {
            TF_Suites dbSuite = new TF_Suites();
            dbSuite.Name = item.Name;
            if (item.ParentExternalId > 0)
            {
                item.Parent = (from s in this.context.TF_Suites
                               where s.External_Id == item.ParentExternalId
                               select s.Suite_Id).FirstOrDefault();
            }
            dbSuite.Parent = item.Parent;
            dbSuite.Modified = DateTime.Now;
            dbSuite.TestPlan_Id = item.TestPlan;
            dbSuite.LastModifiedBy = item.LastModifiedBy;
            dbSuite.Created = DateTime.Now;
            dbSuite.CreatedBy = item.CreatedBy;
            dbSuite.External_Id = item.ExternalId;
            dbSuite.Parent_External_Id = item.ParentExternalId;

            this.context.TF_Suites.Add(dbSuite);

            try
            {
                this.context.SaveChanges();
                return dbSuite.Suite_Id;
            }
            catch(Exception e)
            {
                return -1;
            }
        }

        public bool Edit(TestSuite item)
        {
            TF_Suites dbSuite = this.context.TF_Suites.Find(item.Id);
            dbSuite.Name = item.Name;
            dbSuite.Parent = item.Parent;
            dbSuite.Modified = item.Modified;
            dbSuite.TestPlan_Id = item.TestPlan;
            dbSuite.LastModifiedBy = item.LastModifiedBy;

            try
            {
                this.context.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public TestSuite Get(int id)
        {
            return fillSuite(this.context.TF_Suites.Find(id));
        }
        /// <summary>
        /// Returns the testplans list of suites.
        /// </summary>
        /// <param name="testPlanId">Test plan id</param>
        /// <returns></returns>
        public List<TestSuite> GetFromParent(int testPlanId)
        {
            List<TestSuite> suites = new List<TestSuite>();
            var dbSuites = from s in this.context.TF_Suites
                         where s.TestPlan_Id == testPlanId && s.Parent == 0
                         select s;
            foreach (TF_Suites s in dbSuites)
            {
                suites.Add(fillSuite(s));
            }

            return suites;
        }

        private TestSuite fillSuite(TF_Suites dbSuite)
        {
            TestSuite suite = new TestSuite();
            suite.Id = dbSuite.Suite_Id;
            suite.Name = dbSuite.Name;
            suite.Description = dbSuite.Description;
            suite.Created = dbSuite.Created;
            suite.CreatedBy = dbSuite.CreatedBy;
            suite.Modified = dbSuite.Modified;
            suite.LastModifiedBy = dbSuite.LastModifiedBy;
            suite.ExternalId = dbSuite.External_Id;
            suite.Parent = dbSuite.Parent;
            suite.ParentExternalId = Convert.ToInt32(dbSuite.Parent_External_Id);
            suite.TestPlan = dbSuite.TestPlan_Id;
            suite.SubSuites = REC_getSuitesFromParent(suite.Id);
            return suite;
        }
        /// <summary>
        /// Recursive function which returns a list of suites from a parent suite's subsuites.
        /// </summary>
        /// <param name="parentId">The parent suite</param>
        /// <returns></returns>
        private List<TestSuite> REC_getSuitesFromParent(int parentId)
        {
            var dbSuites = from s in this.context.TF_Suites
                           where s.Parent == parentId
                           select s;

            List<TestSuite> suites = new List<TestSuite>();
            foreach (TF_Suites s in dbSuites)
            {
                suites.Add(fillSuite(s));
            }

            return suites;
        }
    }
}
