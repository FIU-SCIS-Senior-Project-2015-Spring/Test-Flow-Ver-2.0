using DataStore.Adapters.Composites;
using DataStore.EntityData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DataStore.Adapters.TestFlow.Composites
{
    class TestFlowTestCaseHelper : TestFlowCompositeBase, ITestCaseHelper
    {
        public TestFlowTestCaseHelper(testflowEntities context)
            : base(context)
        { }
    
        public int Create(TestCase item)
        {
            if (item.TestSuite.Id <= 0)
                item.TestSuite.Id = (from ts in context.TF_Suites
                                    where ts.External_Id == item.TestSuite.ExternalId
                                    select ts.Suite_Id).FirstOrDefault()
                                    ;
            TF_TestCases testCase = new TF_TestCases();
            testCase.Name = item.Name;
            testCase.Suite_Id = item.TestSuite.Id;
            testCase.Description = item.Description;
            testCase.External_Id = item.ExternalId;
            testCase.LastModifiedBy = item.LastModifiedBy;
            testCase.Modified = DateTime.Now;
            testCase.Created = DateTime.Now;
            testCase.CreatedBy = item.CreatedBy;
            this.context.TF_TestCases.Add(testCase);
            try
            {
                this.context.SaveChanges();
                return testCase.TestCase_Id;
            }
            catch(Exception e)
            {
                return -1;
            }
        }

        public bool Edit(TestCase item)
        {
            TF_TestCases testCase = this.context.TF_TestCases.Find(item.Id);
            testCase.Name = item.Name;
            testCase.Suite_Id = item.TestSuite.Id;
            testCase.Description = item.Description;
            testCase.LastModifiedBy = item.LastModifiedBy;
            testCase.Modified = item.Modified;
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

        public TestCase Get(int id)
        {
            return fillCase(this.context.TF_TestCases.Find(id));
        }

        public List<TestCase> GetFromParent(int parentId)
        {
            TF_Suites suite = this.context.TF_Suites.Find(parentId);
            List<TestCase> testCases = new List<TestCase>();
            foreach (TF_TestCases dbCase in suite.TF_TestCases)
            {
                testCases.Add(fillCase(dbCase));
            }

            return testCases;
        }

        private TestCase fillCase(TF_TestCases dbCase)
        {
            TestCase testCase = new TestCase();
            testCase.Id = dbCase.TestCase_Id;
            testCase.Name = dbCase.Name;
            testCase.LastModifiedBy = dbCase.LastModifiedBy;
            testCase.TestSuite = new TestSuite();
            testCase.TestSuite.Id = dbCase.Suite_Id;
            testCase.Modified = dbCase.Modified;
            testCase.Created = dbCase.Created;
            testCase.CreatedBy = dbCase.CreatedBy;
            testCase.ExternalId = dbCase.External_Id;

            return testCase;
        }
    }
}
