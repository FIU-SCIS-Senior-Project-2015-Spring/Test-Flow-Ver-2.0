using DataStore.Adapters.Composites;
using DataStore.Adapters.TestFlow.Composites;
using DataStore.EntityData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;

namespace DataStore.Adapters.TestFlow
{
    public class TestFlowManager : IDataStoreAdapter
    {
        private testflowEntities context;
        private TestFlowProjectHelper projectsHelper;
        private TestFlowTestPlanHelper testPlanHelper;
        private TestFlowSuiteHelper suiteHelper;
        private TestFlowTestCaseHelper testCaseHelper;
        private TestFlowStepHelper stepHelper;

        public TF_User User
        {
            get;
            set;
        }
        public IProjectHelper Projects
        {
            get { return projectsHelper; }
        }

        public ITestPlanHelper TestPlans
        {
            get { return testPlanHelper; }
        }

        public ISuiteHelper Suites
        {
            get { return suiteHelper; }
        }

        public ITestCaseHelper TestCases
        {
            get { return testCaseHelper; }
        }

        public IStepHelper Steps
        {
            get { return stepHelper; }
        }

        public TestFlowManager(string Username)
        {
            // get user information
            context = new testflowEntities();
            setupUser(Username);
            projectsHelper = new TestFlowProjectHelper(context);
            testPlanHelper = new TestFlowTestPlanHelper(context);
            suiteHelper = new TestFlowSuiteHelper(context);
            testCaseHelper = new TestFlowTestCaseHelper(context);
            stepHelper = new TestFlowStepHelper(context);
        }

        private void setupUser(string Username)
        {
            var user = (from u in context.TF_User where u.Username.Equals(Username) select u).FirstOrDefault();
            if (user == null)
            {
                User = new TF_User();
                User.Username = Username;
                context.TF_User.Add(User);
                try
                {
                    context.SaveChanges();
                }
                catch (Exception e)
                {
                    // this probably happend because of multiple ajax calls and the unique constraint.
                }
                User = (from u in context.TF_User where u.Username == Username select u).FirstOrDefault();
            }
            else
                User = user;
        }


        // not needed since this manager is ALWAYS used.
        public Uri Host { get; set; }

        public int Id { get; set; }

        public string Name { get; set; }


        public void CreateStepHelper(int testCaseId)
        {
            throw new NotImplementedException();
        }
    }
}