using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TestCaseModel;

public class TestCase : IUserInformation
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
	public virtual List<TestStep> Steps
	{
		get;
		set;
	}

	public virtual string Description
	{
		get;
		set;
	}

	public virtual List<Attachment> Attachments
	{
		get;
		set;
	}

	public virtual List<Tag> Tags
	{
		get;
		set;
	}

	public virtual TestSuite TestSuite
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
}

