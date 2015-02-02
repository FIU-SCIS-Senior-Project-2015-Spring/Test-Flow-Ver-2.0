using DataStore.Adapters.Composites;
using DataStore.EntityData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataStore.Adapters.TestFlow.Composites
{
    class TestFlowTestPlanHelper : TestFlowCompositeBase, ITestPlanHelper
    {
        public TestFlowTestPlanHelper(testflowEntities context)
            : base(context)
        { }
    
        public int Create(TestPlan item)
        {
            TF_TestPlan dbPlan = new TF_TestPlan();
            dbPlan.Name = item.Name;
            dbPlan.Project_Id = item.Project.Id;
            dbPlan.External_Id = item.ExternalId;
            this.context.TF_TestPlan.Add(dbPlan);
            try
            {
                this.context.SaveChanges();
                return dbPlan.TestPlan_Id;
            }
            catch(Exception e)
            {
                return -1;
            }
        }

        public bool Edit(TestPlan item)
        {
            TF_TestPlan dbPlan = this.context.TF_TestPlan.Find(item.Id);
            dbPlan.Name = item.Name;
            try
            {
                this.context.SaveChanges();
                return true;
            }
            catch(Exception e)
            {
                return false;
            }
        }

        public TestPlan Get(int id)
        {
            return fillPlan(this.context.TF_TestPlan.Find(id));
        }

        public List<TestPlan> GetFromParent(int parentId)
        {
            var dbProject = this.context.TF_Projects.Find(parentId);
            List<TestPlan> testPlans = new List<TestPlan>();

            foreach (TF_TestPlan tp in dbProject.TF_TestPlan)
            {
                testPlans.Add(fillPlan(tp));
            }

            return testPlans;
        }

        private TestPlan fillPlan(TF_TestPlan dbPlan)
        {
            TestPlan testPlan = new TestPlan();
            testPlan.Id = dbPlan.TestPlan_Id;
            testPlan.Name = dbPlan.Name;
            testPlan.Project = new Project();
            testPlan.Project.Id = dbPlan.TF_Projects.Project_Id;
            testPlan.Project.Name = dbPlan.TF_Projects.Name;
            testPlan.Project.CollectionId = dbPlan.TF_Projects.Collection_Id;
            testPlan.ExternalId = dbPlan.External_Id;
            return testPlan;
        }
    }
}
