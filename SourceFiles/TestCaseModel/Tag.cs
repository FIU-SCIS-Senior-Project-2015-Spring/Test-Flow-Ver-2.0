using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Tag
{
	public virtual int Id
	{
		get;
		set;
	}

	public virtual string Name
	{
		get;
		set;
	}

	public virtual TestCase TestCase
	{
		get;
		set;
	}

}

