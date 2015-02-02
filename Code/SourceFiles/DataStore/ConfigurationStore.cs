using DataStore.Adapters;
using DataStore.Adapters.TestFlow;
using DataStore.Adapters.Tfs;
using DataStore.EntityData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;

public struct UserCollections
{
    public IDataStoreAdapter collection;
    public TestFlowManager tfStore;
}
/// <summary>
/// This class collects all of the users collections as adapters
/// </summary>
public class ConfigurationStore
{
	public static UserCollections getUserConfiguration(IPrincipal user, int projectId, int testPlanId = -1, int testCaseId = -1)
	{
        // get the local model
        UserCollections Configuration = new UserCollections();
        Configuration.tfStore = new TestFlowManager(user.Identity.Name);

        // get the users adapters
        using (var context = new testflowEntities())
        {
            // query the user's collections
            TF_Projects project = context.TF_Projects.Find(projectId);
            var permissions = from p in context.TF_User_Permissions
                              where p.User_Id == Configuration.tfStore.User.User_Id 
                              select p.Collection_Id;
            TF_Collections collection;
            if (projectId > 0)
                collection = (from c in context.TF_Collections
                              where permissions.Contains(c.Collection_Id) && c.Collection_Id == project.Collection_Id
                              select c).FirstOrDefault();
            else
            {
                collection = (from c in context.TF_Collections
                              where permissions.Contains(c.Collection_Id)
                              select c).FirstOrDefault();
                // user either doesn't have any collections or permissions so do not give access to data.
                if (collection == null)
                    return new UserCollections();

                project = (from p in context.TF_Projects 
                                   where p.Collection_Id == collection.Collection_Id
                                   select p).FirstOrDefault();
            }

            if (testPlanId > 0)
            {
                testPlanId = (from tp in context.TF_TestPlan
                              where tp.TestPlan_Id == testPlanId
                              select tp.External_Id).FirstOrDefault();
            }

            if (testCaseId > 0)
            {
                testCaseId = (from tp in context.TF_TestCases
                              where tp.TestCase_Id == testCaseId
                              select tp.External_Id).FirstOrDefault();
            }

            // this supports other future adapters
            switch(collection.Type_Id)
            {
                case 1:
                    TfsManager tfsStore;
                    if (project == null)
                        tfsStore = new TfsManager(new Uri(collection.Host));
                    else if (testPlanId < 0)
                        tfsStore = new TfsManager(new Uri(collection.Host), project.Name);
                    else if (testCaseId < 0)
                        tfsStore = new TfsManager(new Uri(collection.Host), project.Name, testPlanId);
                    else
                        tfsStore = new TfsManager(new Uri(collection.Host), project.Name, testPlanId, testCaseId);

                    tfsStore.Name = collection.Name;
                    tfsStore.Id = collection.Collection_Id;
                    Configuration.collection = tfsStore;
                    break;
            }
        }
        
        return Configuration;
	}

    public static List<IDataStoreAdapter> GetAllUserStores(int userId)
    {
        List<IDataStoreAdapter> dStores = new List<IDataStoreAdapter>();

        // get the users adapters
        using (var context = new testflowEntities())
        {
            // query the user's collections
            var permissions = from p in context.TF_User_Permissions
                              where p.User_Id == userId
                              select p.Collection_Id;
            var collections = from c in context.TF_Collections
                                where permissions.Contains(c.Collection_Id)
                                select c;

            // user either doesn't have any collections or permissions so do not give access to data.
            if (collections == null)
                return null;

            // this supports other future adapters
            foreach(TF_Collections c in collections)
            {
                IDataStoreAdapter dStore;
                if (c.Type_Id == 1)
                {
                    // need to provide a method for optionally providing project name.
                    dStore = new TfsManager(new Uri(c.Host));
                    dStore.Name = c.Name;
                    dStore.Id = c.Collection_Id;
                    dStores.Add(dStore);

                }
            }
        }

        return dStores;
    }
}

