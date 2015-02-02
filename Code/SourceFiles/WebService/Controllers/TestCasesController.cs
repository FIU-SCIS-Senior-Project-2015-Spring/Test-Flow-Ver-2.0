using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace TestFlow.Controllers
{
    public class TestCasesController : ApiController
    {
        // GET: api/TestCase
        [Route("api/TestCases/{ProjectId}/{TestPlanId}/{SuiteId}")]
        public IEnumerable<TestCase> Get(int ProjectId, int TestPlanId, int SuiteId)
        {
            TestFlowManager tfManager = new TestFlowManager(User, ProjectId, TestPlanId);
            return tfManager.GetTestCases(SuiteId);
        }

        [Route("api/TestCases/{ProjectId}/{TestPlanId}/{SuiteId}/{TestCaseId}")]
        public string Get(int ProjectId, int TestPlanId, int SuiteId, int TestCaseId)
        {
            return CredentialCache.DefaultNetworkCredentials.UserName;
        }

        // POST: api/TestCase
        [Route("api/TestCases/create/{ProjectId}/{TestPlanId}/{SuiteId}")]
        public string Post(int ProjectId, int TestPlanId, int SuiteId, [FromBody]string value)
        {
            try
            {
                TestFlowManager tfManager = new TestFlowManager(User, ProjectId, TestPlanId);
                TestCase testCase = new TestCase();
                JObject result = JObject.Parse(value);
                testCase.Name = (string)result["name"];
                testCase.Description = (string)result["summary"];
                testCase.TestSuite = new TestSuite();
                testCase.TestSuite.Id = SuiteId;
                return tfManager.CreateTestCase(testCase).ToString();
            }
            catch (Exception e)
            {
                return "-1";
            }
        }

        // PUT: api/TestCase/5
        [Route("api/TestCases/edit/{ProjectId}/{TestPlanId}/{TestCaseId}")]
        public string Put(int ProjectId, int TestPlanId, int TestCaseId, [FromBody]string value)
        {
            try
            {
                TestFlowManager tfManager = new TestFlowManager(User, ProjectId, TestPlanId);
                TestCase testCase = tfManager.GetTestCase(TestCaseId);
                JObject result = JObject.Parse(value);
                testCase.Name = (string)result["name"];
                testCase.Description = (string)result["summary"];
                if (tfManager.EditTestCase(testCase))
                    return "1";
                else
                    return "-1";
            }
            catch (Exception e)
            {
                return "-1";
            }
        }

        // DELETE: api/TestCases/5
        public void Delete(int id)
        {
        }
    }
}
