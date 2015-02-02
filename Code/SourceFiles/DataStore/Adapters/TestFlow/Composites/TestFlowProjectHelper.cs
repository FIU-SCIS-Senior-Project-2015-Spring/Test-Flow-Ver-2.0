using DataStore.Adapters.Composites;
using DataStore.EntityData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataStore.Adapters.TestFlow.Composites
{
    public class TestFlowProjectHelper : TestFlowCompositeBase, IProjectHelper
    {
        public TestFlowProjectHelper(testflowEntities context)
            : base(context)
        { }
    
        public int Create(Project item)
        {
            TF_Projects proj = new TF_Projects();
            proj.Name = item.Name;
            proj.Collection_Id = item.CollectionId;
            proj.External_Id = item.ExternalId;

            this.context.TF_Projects.Add(proj);

            try
            {
                context.SaveChanges();
                return proj.Project_Id;
            }
            catch (Exception e)
            {
                return -1;
            }
        }

        public bool Edit(Project item)
        {
            TF_Projects proj = this.context.TF_Projects.Find(item.Id);
            proj.Name = item.Name;

            try
            {
                context.SaveChanges();
                return true;
            }
            catch(Exception e)
            {
                return false;
            }
        }

        public Project Get(int id)
        {
            return fillProject(this.context.TF_Projects.Find(id));
        }

        public List<Project> GetFromParent(int parentId)
        {
            TF_Collections collection = this.context.TF_Collections.Find(parentId);

            List<Project> projects = new List<Project>();

            foreach(TF_Projects p in collection.TF_Projects)
            {
                projects.Add(fillProject(p));
            }

            return projects;
        }

        public List<Project> GetAll(int userId)
        {
            var collectionIds = from c in this.context.TF_Collections
                                join p in this.context.TF_User_Permissions on c.Collection_Id equals p.Collection_Id
                                select c.Collection_Id;
            var dbProjects = from p in this.context.TF_Projects
                           where collectionIds.Contains(p.Collection_Id)
                           select p;

            List<Project> projects = new List<Project>();
            foreach (TF_Projects p in dbProjects)
                projects.Add(fillProject(p));

            return projects;
        }

        private Project fillProject(TF_Projects dbProject)
        {
            Project project = new Project();
            project.Id = dbProject.Project_Id;
            project.Name = dbProject.Name;
            project.CollectionId = dbProject.Collection_Id;
            project.ExternalId = dbProject.External_Id;
            return project;
        }
    }
}
