using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class TestPlan : ITestItemBase
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
	public virtual List<TestSuite> Suites
	{
		get;
		set;
	}

	public Project Project
	{
		get;
		set;
	}

    public int ExternalId
    {
        get;
        set;
    }

}

