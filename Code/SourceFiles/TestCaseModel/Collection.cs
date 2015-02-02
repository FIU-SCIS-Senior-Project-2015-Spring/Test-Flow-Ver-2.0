using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Collection : ITestItemBase
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
	public string Host
	{
		get;
		set;
	}

    public int Type
    {
        get;
        set;
    }

	public virtual List<Project> Projects
	{
		get;
		set;
	}

}

