using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace TestFlow.Controllers
{
    public class TestPlansController : ApiController
    {
        private TestFlowManager serviceFacade;

        public TestPlansController()
        {
            
        }
        // GET: api/TestPlans/projectName/testPlanId
        [Route("api/TestPlans/{ProjectId}/{Id}")]
        public TestPlan Get(int ProjectId, int Id)
        {
            serviceFacade = new TestFlowManager(User, ProjectId);
            return serviceFacade.GetTestPlan(Id);
        }

        // GET: api/TestPlans/projectName
        [Route("api/TestPlans/{ProjectId}")]
        public IEnumerable<TestPlan> Get(int ProjectId)
        {
            serviceFacade = new TestFlowManager(User, ProjectId);
            return serviceFacade.GetTestPlans(ProjectId);
        }

        // POST: api/TestPlans/14
        [Route("api/TestPlans/create/{ProjectId}")]
        public void Post(int ProjectId, [FromBody]string value)
        {
            serviceFacade = new TestFlowManager(User, ProjectId);
            TestPlan tp = new TestPlan();
            tp.Name = value;
            tp.Project = new Project();
            tp.Project.Id = ProjectId;
            serviceFacade.CreateTestPlan(tp);
        }

        // PUT: api/TestPlans/5
        [Route("api/TestPlans/edit/{ProjectId}/{Id}")]
        public void Put(int ProjectId, int Id, [FromBody]string value)
        {
            serviceFacade = new TestFlowManager(User, ProjectId);
            TestPlan plan = new TestPlan();
            plan.Id = Id;
            plan.Name = value;
            plan.Project = new Project();
            plan.Project.Id = ProjectId;
            serviceFacade.EditTestPlan(plan);
        }

        // DELETE: api/TestPlans/5
        public void Delete(int id)
        {
        }
    }
}
