using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace TestFlow.Controllers
{
    public class ProjectsController : ApiController
    {
        private TestFlowManager serviceFacade;

        public ProjectsController()
        {
            serviceFacade = new TestFlowManager(User);
        }
        // GET: api/Projects
        public IEnumerable<Project> Get()
        {
            return serviceFacade.getProjects();
        }

        // GET: api/Project/5
        public Project Get(int id)
        {
            return new Project();
        }
    }
}
