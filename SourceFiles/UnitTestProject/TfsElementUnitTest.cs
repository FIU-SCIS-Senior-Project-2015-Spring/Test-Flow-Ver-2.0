using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DataStore.Adapters.Tfs;

namespace UnitTestProject
{
    [TestClass]
    public class TfsElementUnitTest
    {
        // these are dependant on your database
        string username = @"JUSTIN-DESKTOP\Justin";
        string projectId = "HelloWorld";
        int testPlanId = 6;
        int testCaseId = 52;
        int suiteId = 51;

        [TestMethod]
        public void TestPlanTestMethod()
        {
            TfsManager tfManager = new TfsManager(new Uri("http://tc-dev.cis.fiu.edu:8080/tfs/DefaultCollection"), projectId);
            TestPlan createdPlan = new TestPlan();
            createdPlan.Name = "Unit Test " + DateTime.Now.ToString();
            createdPlan.ExternalId = 0;

            // attempt to create, -1 if failed
            Assert.AreNotEqual(tfManager.TestPlans.Create(createdPlan), -1);

            // get what was created
            TestPlan check = tfManager.TestPlans.Get(createdPlan.Id);

            // check that it was the right name
            Assert.AreEqual(createdPlan.Name, check.Name);

            // edit the name
            createdPlan.Name.Replace("Unit Test ", "");

            // attempt to make edit
            Assert.IsTrue(tfManager.TestPlans.Edit(createdPlan));

            // get the edit from db
            check = tfManager.TestPlans.Get(createdPlan.Id);

            // check if they are equal
            Assert.AreEqual(createdPlan.Name, check.Name);
        }

        [TestMethod]
        public void SuiteTestMethod()
        {
            TfsManager tfManager = new TfsManager(new Uri("http://tc-dev.cis.fiu.edu:8080/DefaultCollection"), projectId, testPlanId);
            TestSuite createdPlan = new TestSuite();
            createdPlan.Name = "Unit Test " + DateTime.Now.ToString();
            createdPlan.TestPlan = testPlanId;
            createdPlan.ParentExternalId = 0;
            createdPlan.Modified = DateTime.Now;
            createdPlan.Created = DateTime.Now;
            createdPlan.ExternalId = 0;
            createdPlan.Parent = 0;
            createdPlan.ParentExternalId = 0;
            createdPlan.Description = "This is a unit test generated element.";

            // attempt to create, -1 if failed
            Assert.AreNotEqual(tfManager.Suites.Create(createdPlan), -1);

            // get what was created
            TestSuite check = tfManager.Suites.Get(createdPlan.Id);

            // check that it was the right name
            Assert.AreEqual(createdPlan.Name, check.Name);

            // edit the name
            createdPlan.Name.Replace("Unit Test ", "");

            // attempt to make edit
            Assert.IsTrue(tfManager.Suites.Edit(createdPlan));

            // get the edit from db
            check = tfManager.Suites.Get(createdPlan.Id);

            // check if they are equal
            Assert.AreEqual(createdPlan.Name, check.Name);
        }

        [TestMethod]
        public void TestCaseTestMethod()
        {
            TfsManager tfManager = new TfsManager(new Uri("http://tc-dev.cis.fiu.edu:8080/DefaultCollection"), projectId, testPlanId);
            TestCase createdCase = new TestCase();
            createdCase.Name = "Unit Test " + DateTime.Now.ToString();
            createdCase.TestSuite = new TestSuite();
            createdCase.TestSuite.Id = suiteId;
            createdCase.Modified = DateTime.Now;
            createdCase.Created = DateTime.Now;
            createdCase.ExternalId = 0;
            createdCase.Description = "This is a unit test generated element.";

            // attempt to create, -1 if failed
            Assert.AreNotEqual(tfManager.TestCases.Create(createdCase), -1);

            // get what was created
            TestCase check = tfManager.TestCases.Get(createdCase.Id);

            // check that it was the right name
            Assert.AreEqual(createdCase.Name, check.Name);

            // edit the name
            createdCase.Name.Replace("Unit Test ", "");

            // attempt to make edit
            Assert.IsTrue(tfManager.TestCases.Edit(createdCase));

            // get the edit from db
            check = tfManager.TestCases.Get(createdCase.Id);

            // check if they are equal
            Assert.AreEqual(createdCase.Name, check.Name);
        }

        [TestMethod]
        public void TestStepTestMethod()
        {
            TfsManager tfManager = new TfsManager(new Uri("http://tc-dev.cis.fiu.edu:8080/DefaultCollection"), projectId, testPlanId, testCaseId);
            TestStep createdStep = new TestStep();
            createdStep.Name = "Unit Test " + DateTime.Now.ToString();
            createdStep.TestCase = testCaseId;
            createdStep.Modified = DateTime.Now;
            createdStep.Created = DateTime.Now;
            createdStep.ExternalId = 0;
            createdStep.Result = "This is a unit test generated element.";

            // attempt to create, -1 if failed
            Assert.AreNotEqual(tfManager.Steps.Create(createdStep), -1);

            // get what was created
            TestStep check = tfManager.Steps.Get(createdStep.Id);

            // check that it was the right name
            Assert.AreEqual(createdStep.Name, check.Name);

            // edit the name
            createdStep.Name.Replace("Unit Test ", "");

            // attempt to make edit
            Assert.IsTrue(tfManager.Steps.Edit(createdStep));

            // get the edit from db
            check = tfManager.Steps.Get(createdStep.Id);

            // check if they are equal
            Assert.AreEqual(createdStep.Name, check.Name);
        }
    }
}
