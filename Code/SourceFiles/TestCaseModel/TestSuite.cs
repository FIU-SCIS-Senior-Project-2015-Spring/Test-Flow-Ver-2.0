using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TestCaseModel;

public class TestSuite : IUserInformation
{
    public int Id
    {
        get;
        set;
    }

    public string Name
    {
        get;
        set;
    }

	public virtual List<TestCase> TestCases
	{
		get;
		set;
	}

	public virtual string Description
	{
		get;
		set;
	}

	public virtual int Parent
	{
		get;
		set;
	}

    public List<TestSuite> SubSuites
    {
        get;
        set;
    }

    public int TestPlan
    {
        get;
        set;
    }

    public int LastModifiedBy
    {
        get;
        set;
    }

    public int CreatedBy
    {
        get;
        set;
    }

    public DateTime Created
    {
        get;
        set;
    }

    public DateTime Modified
    {
        get;
        set;
    }

    public int ExternalId
    {
        get;
        set;
    }

    public int ParentExternalId
    {
        get;
        set;
    }
}

