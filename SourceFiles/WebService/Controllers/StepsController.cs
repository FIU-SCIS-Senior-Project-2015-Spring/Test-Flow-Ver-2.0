using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace TestFlow.Controllers
{
    public class StepsController : ApiController
    {
        // GET: api/Steps
        [Route("api/Steps/{ProjectId}/{TestPlanId}/{TestCaseId}")]
        public IEnumerable<TestStep> Get(int ProjectId, int TestPlanId, int TestCaseId)
        {
            try
            {
                TestFlowManager tfManager = new TestFlowManager(User, ProjectId, TestPlanId, TestCaseId);
                return tfManager.GetTestSteps(TestCaseId);
            }
            catch(Exception e)
            {
                return null;
            }
            
        }

        // GET: api/Steps/5
        [Route("api/Steps/{ProjectId}/{TestPlanId}/{TestCaseId}/{TestStepId}")]
        public TestStep Get(int ProjectId, int TestPlanId, int TestCaseId, int TestStepId)
        {
            try
            {
                TestFlowManager tfManager = new TestFlowManager(User, ProjectId, TestPlanId, TestCaseId);
                return tfManager.GetTestStep(TestStepId);
            }
            catch (Exception e)
            {
                return null;
            }
        }

        // POST: api/Steps
        [Route("api/Steps/create/{ProjectId}/{TestPlanId}/{TestCaseId}/")]
        public string Post(int ProjectId, int TestPlanId, int TestCaseId, [FromBody]string value)
        {
            try
            {
                TestFlowManager tfManager = new TestFlowManager(User, ProjectId, TestPlanId, TestCaseId);
                TestStep testStep = new TestStep();
                JObject result = JObject.Parse(value);
                testStep.Name = (string)result["name"];
                testStep.Result = (string)result["result"];
                testStep.TestCase = TestCaseId;

                return tfManager.CreateTestStep(testStep).ToString();
            }
            catch(Exception e)
            {
                return "-1";
            }
        }

        // PUT: api/Steps/5
        [Route("api/Steps/edit/{ProjectId}/{TestPlanId}/{TestCaseId}/")]
        public string Put(int ProjectId, int TestPlanId, int TestCaseId, [FromBody]string value)
        {
            try
            {
                TestFlowManager tfManager = new TestFlowManager(User, ProjectId, TestPlanId, TestCaseId);
                TestStep testStep = new TestStep();
                JObject result = JObject.Parse(value);
                testStep.Id = Convert.ToInt32(result["id"]);
                testStep.Name = (string)result["name"];
                testStep.Result = (string)result["result"];

                if (tfManager.EditTestStep(testStep))
                    return "1";
                else
                    return "-1";
            }
            catch (Exception e)
            {
                return "-1";
            }
        }

        // DELETE: api/Steps/5
        public void Delete(int id)
        {
        }
    }
}
