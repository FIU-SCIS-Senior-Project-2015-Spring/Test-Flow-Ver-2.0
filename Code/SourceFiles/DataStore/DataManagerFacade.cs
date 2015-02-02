using DataStore;
using DataStore.Adapters;
using DataStore.Adapters.TestFlow.Composites;
using DataStore.SyncStores;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;

public class DataManagerFacade
{
    private UserCollections dataStores;

    private IPrincipal User;

    public DataManagerFacade(IPrincipal user, int projectId)
    {
        User = user;
        dataStores = ConfigurationStore.getUserConfiguration(user, projectId);
    }

    public DataManagerFacade(IPrincipal user, int projectId, int testPlanId)
    {
        User = user;
        dataStores = ConfigurationStore.getUserConfiguration(user, projectId, testPlanId);
    }

    public DataManagerFacade(IPrincipal user, int projectId, int testPlanId, int testCaseId)
    {
        User = user;
        dataStores = ConfigurationStore.getUserConfiguration(user, projectId, testPlanId, testCaseId);
    }

    // Projects Stuff
    //*****************************************************************************

    public Project GetProject(int id)
    {
        return dataStores.tfStore.Projects.Get(id);
    }

    public List<Project> GetProjects()
    {
        if (dataStores.tfStore == null)
            return null;
        List<IDataStoreAdapter> dStores = ConfigurationStore.GetAllUserStores(dataStores.tfStore.User.User_Id);
        if (dStores.Count <= 0)
            return null;
        SyncManager.SyncProjects(dStores, dataStores.tfStore);
        TestFlowProjectHelper tfph = (TestFlowProjectHelper)dataStores.tfStore.Projects;
        return tfph.GetAll(dataStores.tfStore.User.User_Id);
    }

    //*****************************************************************************
    // End Projects Stuff

    // TestPlan Stuff
    //*****************************************************************************

	public void CreateTestPlan(TestPlan testPlan)
    {
        testPlan.ExternalId = dataStores.collection.TestPlans.Create(testPlan);
        dataStores.tfStore.TestPlans.Create(testPlan);
    }

    public void EditTestPlan(TestPlan testPlan)
    {
        if (dataStores.collection.TestPlans.Edit(testPlan))
            dataStores.tfStore.TestPlans.Edit(testPlan);
    }

    public TestPlan GetTestPlan(int id)
    {
        return dataStores.tfStore.TestPlans.Get(id);
    }

    public List<TestPlan> GetTestPlans(int projectId)
    {
        List<TestPlan> externalPlans = dataStores.collection.TestPlans.GetFromParent(projectId); // this id doesn't matter because project is being set elsewhere
        List<TestPlan> internalPlans = dataStores.tfStore.TestPlans.GetFromParent(projectId);
        SyncManager.SyncTestPlans(internalPlans, externalPlans, dataStores.tfStore, projectId);
        return dataStores.tfStore.TestPlans.GetFromParent(projectId);
    }

    //*****************************************************************************
    // End TestPlan Stuff

    // Suite Stuff
    //*****************************************************************************

    public int CreateSuite(TestSuite Suite)
    {
        Suite.ExternalId = dataStores.collection.Suites.Create(Suite);
        return dataStores.tfStore.Suites.Create(Suite);
    }

    public bool EditSuite(TestSuite Suite)
    {
        if (dataStores.collection.Suites.Edit(Suite))
            return dataStores.tfStore.Suites.Edit(Suite);
        else
            return false;
    }

    public TestSuite GetSuite(int id)
    {
        return dataStores.tfStore.Suites.Get(id);
    }

    public List<TestSuite> GetSuites(int testPlanId)
    {
        TestPlan testPlan = GetTestPlan(testPlanId);
        List<TestSuite> externalSuites = dataStores.collection.Suites.GetFromParent(testPlan.ExternalId);
        List<TestSuite> internalSuites = dataStores.tfStore.Suites.GetFromParent(testPlanId);
        SyncManager.SyncSuites(internalSuites, externalSuites, dataStores.tfStore, testPlanId);
        return dataStores.tfStore.Suites.GetFromParent(testPlanId);
    }

    //*****************************************************************************
    // End Suite Stuff

    // TestCase Stuff
    //*****************************************************************************

    public int CreateTestCase(TestCase TestCase)
    {
        TestCase.TestSuite = GetSuite(TestCase.TestSuite.Id);
        TestCase.ExternalId = dataStores.collection.TestCases.Create(TestCase);
        return dataStores.tfStore.TestCases.Create(TestCase);
    }

    public bool EditTestCase(TestCase TestCase)
    {
        if (dataStores.collection.TestCases.Edit(TestCase))
            return dataStores.tfStore.TestCases.Edit(TestCase);
        else
            return false;
    }

    public TestCase GetTestCase(int id)
    {
        return dataStores.tfStore.TestCases.Get(id);
    }

    public List<TestCase> GetTestCases(int suiteId)
    {
        TestSuite suite = GetSuite(suiteId);
        List<TestCase> externalTestCases = dataStores.collection.TestCases.GetFromParent(suite.ExternalId);
        List<TestCase> internalTestCases = dataStores.tfStore.TestCases.GetFromParent(suite.Id);
        SyncManager.SyncTestCases(internalTestCases, externalTestCases, dataStores.tfStore, suiteId);
        List<TestCase> results = dataStores.tfStore.TestCases.GetFromParent(suiteId);
        
        foreach(TestCase tc in results)
        {
            dataStores.collection.CreateStepHelper(tc.ExternalId);
            tc.Steps = GetSteps(tc.Id);
        }
        return results;
    }

    //*****************************************************************************
    // End TestCase Stuff

    // Step Stuff
    //*****************************************************************************

    public int CreateStep(TestStep Step)
    {
        TestCase tc = GetTestCase(Step.TestCase);
        Step.ParentExternalId = tc.ExternalId;
        Step.ExternalId = dataStores.collection.Steps.Create(Step);
        return dataStores.tfStore.Steps.Create(Step);
    }

    public bool EditStep(TestStep Step)
    {
        TestStep ts = GetStep(Step.Id);
        Step.ExternalId = ts.ExternalId;
        if (dataStores.collection.Steps.Edit(Step))
            return dataStores.tfStore.Steps.Edit(Step);
        else
            return false;
    }

    public TestStep GetStep(int id)
    {
        return dataStores.tfStore.Steps.Get(id);
    }

    public List<TestStep> GetSteps(int testCaseId)
    {
        TestCase testCase = GetTestCase(testCaseId);
        List<TestStep> externalSteps = dataStores.collection.Steps.GetFromParent(testCase.ExternalId);
        List<TestStep> internalSteps = dataStores.tfStore.Steps.GetFromParent(testCase.Id);
        SyncManager.SyncTestSteps(internalSteps, externalSteps, dataStores.tfStore, testCase.Id);
        return dataStores.tfStore.Steps.GetFromParent( testCaseId);
    }

    //*****************************************************************************
    // End Step Stuff

}

