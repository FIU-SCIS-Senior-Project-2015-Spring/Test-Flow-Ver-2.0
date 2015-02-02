using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
[DataContract]
public class Project : ITestItemBase
{
    [DataMember]
    public int Id
    {
        get;
        set;
    }
    [DataMember]
    public string Name
    {
        get;
        set;
    }
    [DataMember]
    public string Store
    {
        get;
        set;
    }
	public int CollectionId
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

