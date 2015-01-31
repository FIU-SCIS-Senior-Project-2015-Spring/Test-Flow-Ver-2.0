using DataStore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;

public class TestFlowManager
{
    private DataManagerFacade dataManager
	{
		get;
		set;
	}

    private IPrincipal User
    {
        get;
        set;
    }

    public TestFlowManager(IPrincipal user, bool collectionManagement)
    {
        User = user;
        if (!collectionManagement)
            dataManager = new DataManagerFacade(user, -1);
    }

    public TestFlowManager(IPrincipal user, int projectId = -1)
    {
        User = user;
        dataManager = new DataManagerFacade(user, projectId);
    }

    public TestFlowManager(IPrincipal user, int projectId, int testPlanId)
    {
        User = user;
        dataManager = new DataManagerFacade(user, projectId, testPlanId);
    }

    public TestFlowManager(IPrincipal user, int projectId, int testPlanId, int testCaseId)
    {
        User = user;
        dataManager = new DataManagerFacade(user, projectId, testPlanId, testCaseId);
    }

	

    // Project Stuff
    // *********************************************************************************
	public List<Project> getProjects()
	{
        return dataManager.GetProjects();
	}
    // *********************************************************************************
    // end Project Stuff

    // TestPlan Stuff
    // *********************************************************************************
	public TestPlan GetTestPlan(int testPlanId)
	{
        return dataManager.GetTestPlan(testPlanId);
	}

    public List<TestPlan> GetTestPlans(int projectId)
    {
        return dataManager.GetTestPlans(projectId);
    }

    public void EditTestPlan(TestPlan testPlan)
    {
        dataManager.EditTestPlan(testPlan);
    }

    public void CreateTestPlan(TestPlan testPlan)
    {
        dataManager.CreateTestPlan(testPlan);
    }
    // *********************************************************************************
    // end TestPlan Stuff

    // Suite Stuff
    // *********************************************************************************
    public TestSuite GetSuite(int suiteId)
    {
        return dataManager.GetSuite(suiteId);
    }

    public List<TestSuite> GetSuites(int testPlanId)
    {
        return dataManager.GetSuites(testPlanId);
    }

    public bool EditSuite(TestSuite suite)
    {
        return dataManager.EditSuite(suite);
    }

    public int CreateSuite(TestSuite suite)
    {
        return dataManager.CreateSuite(suite);
    }
    // *********************************************************************************
    // end Suite Stuff

    // TestCase Stuff
    // *********************************************************************************
    public TestCase GetTestCase(int testCaseId)
    {
        return dataManager.GetTestCase(testCaseId);
    }

    public List<TestCase> GetTestCases(int suiteId)
    {
        return dataManager.GetTestCases(suiteId);
    }

    public bool EditTestCase(TestCase testCase)
    {
        return dataManager.EditTestCase(testCase);
    }

    public int CreateTestCase(TestCase testCase)
    {
        return dataManager.CreateTestCase(testCase);
    }
    // *********************************************************************************
    // end TestCase Stuff

    // Step Stuff
    // *********************************************************************************
    public TestStep GetTestStep(int testStepId)
    {
        return dataManager.GetStep(testStepId);
    }

    public List<TestStep> GetTestSteps(int projectId)
    {
        return dataManager.GetSteps(projectId);
    }

    public bool EditTestStep(TestStep step)
    {
        return dataManager.EditStep(step);
    }

    public int CreateTestStep(TestStep step)
    {
        return dataManager.CreateStep(step);
    }
    // *********************************************************************************
    // end Step Stuff

    // collection management metods

    // Collections stuff
    public void EditCollection(Collection collection)
    {
        CollectionHelper cHelper = new CollectionHelper(User.Identity.Name);
        cHelper.EditCollection(collection);
    }

    public Collection GetCollection(int id)
    {
        CollectionHelper cHelper = new CollectionHelper(User.Identity.Name);
        return cHelper.GetCollection(id);
    }

    public List<Collection> GetCollections()
    {
        CollectionHelper cHelper = new CollectionHelper(User.Identity.Name);
        return cHelper.GetCollections();
    }

    public void CreateCollection(Collection collection, int type)
    {
        CollectionHelper cHelper = new CollectionHelper(User.Identity.Name);
        cHelper.CreateCollection(collection, type);
    }
}

