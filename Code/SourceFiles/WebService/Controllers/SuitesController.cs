using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace TestFlow.Controllers
{
    public class SuitesController : ApiController
    {
        // GET: api/Suites/projid/testplanid
        [Route("api/Suites/{ProjectId}/{TestPlanId}")]
        public IEnumerable<TestSuite> Get(int ProjectId, int TestPlanId)
        {
            try
            {
                TestFlowManager tfManager = new TestFlowManager(User, ProjectId, TestPlanId);
                return tfManager.GetSuites(TestPlanId);
            }
            catch(Exception e)
            {
                return null;
            }
        }

        [Route("api/Suites/{ProjectId}/{TestPlanId}/{SuiteId}")]
        public string Get(int ProjectId, int TestPlanId, int SuiteId)
        {
            return "Not implemented";
        }

        // POST: api/Suites
        [Route("api/Suites/create/{ProjectId}/{TestPlanId}")]
        public string Post(int ProjectId, int TestPlanId, [FromBody]string value)
        {
            try
            {
                TestFlowManager tfManager = new TestFlowManager(User, ProjectId, TestPlanId);
                TestSuite suite = new TestSuite();
                JObject result = JObject.Parse(value);
                suite.Name = (string)result["name"];
                suite.Description = (string)result["summary"];
                suite.Parent = Convert.ToInt32(result["parent"]);
                suite.TestPlan = TestPlanId;
                return tfManager.CreateSuite(suite).ToString();
            }
            catch(Exception e)
            {
                return "-1";
            }
            
        }

        // PUT: api/Suites/5
        [Route("api/Suites/edit/{ProjectId}/{TestPlanId}")]
        public string Put(int ProjectId, int TestPlanId, [FromBody]string value)
        {
            try
            {
                TestFlowManager tfManager = new TestFlowManager(User, ProjectId, TestPlanId);
                JObject result = JObject.Parse(value);
                int suiteId = Convert.ToInt32(result["id"]);
                TestSuite suite = tfManager.GetSuite(suiteId);
                
                suite.Id = suiteId;
                suite.Name = (string)result["name"];
                suite.Description = (string)result["summary"];
                suite.TestPlan = TestPlanId;
                if (tfManager.EditSuite(suite))
                    return "1";
                else
                    return "-1";
            }
            catch(Exception e)
            {
                return "-1";
            }
        }

        // DELETE: api/Suites/5
        public void Delete(int id)
        {
        }
    }
}
