using DataStore.Adapters.Composites;
using Microsoft.TeamFoundation.TestManagement.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataStore.Adapters.Tfs.Composites
{
    public class TfsStepHelper : TfsCompositeBase, IStepHelper
    {
        private ITestCase testCase;
        public TfsStepHelper(ITestManagementService testManagementService, string projectName, int testCaseId)
            : base(testManagementService, projectName)
        {
            Microsoft.TeamFoundation.TestManagement.Client.ITestPlanHelper planHelper = this.testManagementProject.TestPlans;
            testCase = this.testManagementProject.TestCases.Find(testCaseId);
        }
    
        public int Create(TestStep item)
        {
            ITestStep step = testCase.CreateTestStep();

            step.Title = item.Name;
            step.ExpectedResult = item.Result;

            testCase.Actions.Add(step);

            try
            {
                testCase.Save();    // objects created from the project have their own save
                return step.Id;
            }
            catch(Exception e)
            {
                return -1;
            }
           
        }

        public bool Edit(TestStep item)
        {
            ITestStep step = testCase.FindAction(item.ExternalId) as ITestStep;

            if (step == null)
                return false;

            step.Title = item.Name;
            step.ExpectedResult = item.Result;

            testCase.Save();
            return true;
        }

        public TestStep Get(int id)
        {
            ITestStep tfsStep = testCase.FindAction(id) as ITestStep;

            if (tfsStep == null)
                return null;

            TestStep step = new TestStep();
            step.Name = tfsStep.Title;
            step.ExternalId = tfsStep.Id;
            step.Result = tfsStep.ExpectedResult.ToString();

            return step;
        }

        public List<TestStep> GetFromParent(int parentId)
        {
            List<TestStep> testSteps = new List<TestStep>();

            foreach(ITestAction ta in testCase.Actions)
            {
                Type hello = ta.GetType();
                if (!ta.GetType().Name.Equals("TestStep"))
                    continue;
                ITestStep tfsStep = ta as ITestStep;
                TestStep step = new TestStep();
                step.Name = tfsStep.Title;
                step.Result = tfsStep.ExpectedResult.ToString();
                step.ExternalId = tfsStep.Id;

                testSteps.Add(step);
            }

            return testSteps;
        }
    }
}
