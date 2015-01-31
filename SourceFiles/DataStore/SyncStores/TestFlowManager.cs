﻿using DataStore.EntityData;
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool
//     Changes to this file will be lost if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;

public class TestFlowDataStorezs
{
/*

    private void MergeExternalSuites(List<TestSuite> suites, int parentId, int testPlanId, testflowEntities context)
    {
        int suiteItemType = Convert.ToInt32(ItemTypes.Suite);
        foreach (TestSuite ts in suites)
        {
            var suite = (from s in context.TF_Suites
                         join e in context.TF_ExternalIds on s.Suite_Id equals e.Internal_Id
                         where e.External_Id == ts.Id
                         select s).FirstOrDefault();
            if (suite == null)
            {
                TF_Suites dbSuite = new TF_Suites();
                dbSuite.Name = ts.Name;
                dbSuite.CreatedBy = User.User_Id;
                dbSuite.Created = DateTime.Now;
                dbSuite.LastModifiedBy = User.User_Id;
                dbSuite.Modified = DateTime.Now;
                dbSuite.Parent = parentId;
                dbSuite.TestPlan_Id = testPlanId;
                context.TF_Suites.Add(dbSuite);
                try
                {
                    context.SaveChanges();
                    bindExternalId(dbSuite.Suite_Id, ts.Id, suiteItemType);
                }
                catch(Exception e)
                {
                    return;
                }
                if(ts.SubSuites != null)
                    MergeExternalSuites(ts.SubSuites, dbSuite.Suite_Id, testPlanId, context);

            }
            else
            {
                if (!suite.Name.Equals(ts.Name))
                    suite.Name = ts.Name;
                try
                {
                    context.SaveChanges();
                }
                catch(Exception e)
                {

                }
            }
        }
    }

    public void SyncPlans(List<TestPlan> externalPlans, int projectId)
    {
        using (var context = new testflowEntities())
        {
            int planItemType = Convert.ToInt32(ItemTypes.TestPlan);
            var dbTestPlan = from tp in context.TF_TestPlan
                             join ip in context.TF_ExternalIds on tp.TestPlan_Id equals ip.Internal_Id
                             where tp.Project_Id == projectId && ip.Type == planItemType
                             select new { TestPlan = tp, ExternalId = ip };

            // N^2 but not expecting too many test plans.
            foreach(TestPlan tp in externalPlans)
            {
                bool exist = false;
                foreach (var ip in dbTestPlan)
                {
                    if (ip.ExternalId.Id == tp.Id)
                    {
                        exist = true;
                    }
                }

                if (!exist)
                {
                    TF_TestPlan testPlan = new TF_TestPlan();
                    testPlan.Name = tp.Name;
                    testPlan.Project_Id = projectId;
                    context.TF_TestPlan.Add(testPlan);
                    try
                    {
                        context.SaveChanges();
                        bindExternalId(testPlan.TestPlan_Id, tp.Id, planItemType);
                    }
                    catch (Exception e)
                    {

                    }


                }
            }
        }
    }
    /// <summary>
    /// Syncronizes the projects from adapters and local entity framework.
    /// </summary>
    /// <param name="externalProjects">external projects</param>
    /// <param name="collectionId">local collection ID where projects are from</param>
    public void SyncProjects(List<Project> externalProjects, int collectionId)
    {
        using(var context = new testflowEntities())
        {
            int projItemType = Convert.ToInt32(ItemTypes.Project);
            
            // get all projects and external ids that relate to this collection
            var projects = from e in context.TF_ExternalIds 
                           join p in context.TF_Projects on e.Internal_Id equals p.Project_Id 
                           where e.Type == projItemType && p.Collection_Id == collectionId
                           select new { Project = p, ExternalId = e};

            // N^2 but not expecting many projects per collection
            foreach(Project p in externalProjects)
            {
                bool exist = false;
                foreach(var ip in projects)
                {
                    if(ip.ExternalId.Id == p.Id)
                    {
                        exist = true;
                        if(!ip.Project.Name.Equals(p.Name))
                        {
                            ip.Project.Name = p.Name;  // only occasion where data flows in reverse besides data that doesn't exist
                        }
                    }
                }

                if(!exist)
                {
                    TF_Projects proj = new TF_Projects();
                    proj.Name = p.Name;
                    proj.Collection_Id = collectionId;
                    context.TF_Projects.Add(proj);
                    try
                    {
                        context.SaveChanges();
                        bindExternalId(proj.Project_Id, p.Id, projItemType);
                    }
                    catch(Exception e)
                    {

                    }
                    
                    
                }
            }
            try
            {
                context.SaveChanges();
            }
            catch(Exception e)
            {

            }
        }
        
    }

    public void bindExternalId(int internalId, int externalId, int type)
    {
        using (var context = new testflowEntities())
        {
            TF_ExternalIds externId = new TF_ExternalIds();
            externId.Internal_Id = internalId;
            externId.Id = externalId;
            externId.Type = type;
            context.TF_ExternalIds.Add(externId);
            context.SaveChanges();
        }
    }

   
    public void CreateCollection(Collection collection, int type)
    {
        TF_Collections dbCollection = new TF_Collections();
        dbCollection.Host = collection.Host.ToString();
        dbCollection.Name = collection.Name;
        dbCollection.Type_Id = type;

        using(var context = new testflowEntities())
        {
            context.TF_Collections.Add(dbCollection);
            try
            {
                context.SaveChanges();
                CreatePermission(dbCollection.Collection_Id);
            }
            catch(Exception e)
            {

            }
        }
    }

    public void EditCollection(Collection collection)
    {
        using (var context = new testflowEntities())
        {
            var dbCollection = context.TF_Collections.Find(collection.Id);
            dbCollection.Name = collection.Name;
            dbCollection.Host = collection.Host;
            try
            {
                context.SaveChanges();
            }
            catch (Exception e)
            {

            }
        }
    }

    public Collection GetCollection(int id)
    {
        using (var context = new testflowEntities())
        {
            Collection collection = new Collection();
            var dbCollection = context.TF_Collections.Find(id);
            collection.Name = dbCollection.Name;
            collection.Host = dbCollection.Host;
            collection.Id = dbCollection.Collection_Id;

            return collection;
        }
    }

    public List<Collection> GetCollections()
    {
        using (var context = new testflowEntities())
        {
            var permissions = from p in context.TF_User_Permissions where p.User_Id == User.User_Id select p.Collection_Id;
            var dbCollections = context.TF_Collections.Where(p => permissions.Contains(p.Collection_Id));
            List<Collection> collections = new List<Collection>();

            foreach(TF_Collections c in dbCollections)
            {
                Collection collection = new Collection();
                collection.Id = c.Collection_Id;
                collection.Name = c.Name;
                collection.Host = c.Host;
                collections.Add(collection);
            }

            return collections;
        }
    }

    private void CreatePermission(int CollectionId)
    {
        using(var context = new testflowEntities())
        {
            var permissions = from p in context.TF_User_Permissions where p.Collection_Id == CollectionId select p;
            if(permissions.Count() <= 0)
            {
                TF_User_Permissions permission = new TF_User_Permissions();
                permission.Collection_Id = CollectionId;
                permission.User_Id = User.User_Id;
                context.TF_User_Permissions.Add(permission);
                try
                {
                    context.SaveChanges();
                }
                catch(Exception e)
                {

                }
                
            }
        }
    }

    public Collection getCollectionFromProject(int projectId)
    {
        using (var context = new testflowEntities())
        {
            int colId = (from p in context.TF_Projects where p.Project_Id == projectId select p.Collection_Id).FirstOrDefault();
            TF_Collections dbCollection = context.TF_Collections.Find(colId);
            Collection collection = new Collection();
            collection.Id = dbCollection.Collection_Id;
            collection.Name = dbCollection.Name;
            return collection;
        }
    }*/
}

