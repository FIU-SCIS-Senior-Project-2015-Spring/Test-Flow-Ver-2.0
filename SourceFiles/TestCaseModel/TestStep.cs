using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TestCaseModel;

public class TestStep : IStepBase
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

    public string Result
    {
        get;
        set;
    }
	public int TestCase
	{
		get;
		set;
	}

    public int Parent
    {
        get;
        set;
    }

    public List<TestStep> Children
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

