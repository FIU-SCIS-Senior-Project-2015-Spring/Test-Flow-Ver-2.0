using DataStore.Adapters.Composites;
using DataStore.EntityData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DataStore.Adapters.TestFlow.Composites
{
    class TestFlowStepHelper : TestFlowCompositeBase, IStepHelper
    {
        public TestFlowStepHelper(testflowEntities context)
            : base(context)
        { }
    
        public int Create(TestStep item)
        {
            TF_Step dbStep = new TF_Step();
            dbStep.Name = item.Name;
            dbStep.Results = item.Result;
            dbStep.Parent = item.Parent;
            dbStep.LastModifiedBy = item.LastModifiedBy;
            dbStep.Modified = DateTime.Now;
            dbStep.Created = DateTime.Now;
            dbStep.CreatedBy = item.CreatedBy;
            dbStep.TestCase_Id = item.TestCase;
            dbStep.External_Id = item.ExternalId;
            dbStep.Parent_External_Id = item.ParentExternalId;

            this.context.TF_Step.Add(dbStep);

            try
            {
                this.context.SaveChanges();
                return dbStep.Step_Id;
            }
            catch(Exception e)
            {
                return -1;
            }
        }

        public bool Edit(TestStep item)
        {
            TF_Step dbStep = this.context.TF_Step.Find(item.Id);
            dbStep.Name = item.Name;
            dbStep.Results = item.Result;
            dbStep.Parent = item.Parent;
            dbStep.LastModifiedBy = item.LastModifiedBy;
            dbStep.Modified = item.Modified;

            try
            {
                this.context.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public TestStep Get(int id)
        {
            return fillStep(this.context.TF_Step.Find(id));
        }

        public List<TestStep> GetFromParent(int parentId)
        {
            TF_TestCases testCase = this.context.TF_TestCases.Find(parentId);

            List<TestStep> steps = new List<TestStep>();
            foreach(TF_Step s in testCase.TF_Step)
            {
                steps.Add(fillStep(s));
            }

            return steps;
        }

        private TestStep fillStep(TF_Step dbStep)
        {
            TestStep step = new TestStep();
            step.Id = dbStep.Step_Id;
            step.Name = ScrubHtml(dbStep.Name);
            step.Result = ScrubHtml(dbStep.Results);
            step.Created = dbStep.Created;
            step.CreatedBy = Convert.ToInt32(dbStep.CreatedBy); // weird
            step.LastModifiedBy = dbStep.LastModifiedBy;
            step.Modified = dbStep.Modified;
            step.Parent = dbStep.Parent;
            step.TestCase = dbStep.TestCase_Id;
            step.ExternalId = dbStep.External_Id;
            step.ParentExternalId = Convert.ToInt32(dbStep.Parent_External_Id);

            step.Children = new List<TestStep>();

            var dbSteps = from s in this.context.TF_Step
                          where s.Parent == step.Id
                          select s;
            foreach (TF_Step ts in dbSteps)
                step.Children.Add(fillStep(ts));

            return step;
            
        }

        private string ScrubHtml(string value)
        {
            var step1 = Regex.Replace(value, @"<[^>]+>|&nbsp;", "").Trim();
            var step2 = Regex.Replace(step1, @"\s{2,}", " ");
            return step2;
        }
    }
}
