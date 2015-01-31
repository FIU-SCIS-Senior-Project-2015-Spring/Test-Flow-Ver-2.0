using DataStore.Adapters;
using DataStore.Adapters.TestFlow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataStore.SyncStores
{
    class SyncManager
    {
        public static void SyncProjects(List<IDataStoreAdapter> dStores, TestFlowManager tfStore)
        {
            // n^3, however relative to this context n should not be greater than 10
            foreach (IDataStoreAdapter da in dStores)
            {
                List<Project> tfProjects = tfStore.Projects.GetFromParent(da.Id);
                List<Project> daProjects = da.Projects.GetFromParent(0);
                foreach (Project p in daProjects)
                {
                    bool exist = false;
                    foreach (var ip in tfProjects)
                    {
                        if (ip.ExternalId == p.ExternalId)
                        {
                            exist = true;
                            if (!ip.Name.Equals(p.Name))
                            {
                                ip.Name = p.Name;  // only occasion where data flows in reverse besides data that doesn't exist
                                tfStore.Projects.Edit(ip);
                            }
                        }
                    }
                    if (!exist)
                    {
                        p.CollectionId = da.Id;
                        tfStore.Projects.Create(p);
                    }
                }
            }
        }

        public static void SyncTestPlans(List<TestPlan> testFlowPlans, List<TestPlan> externalPlans, TestFlowManager tfStore, int projectId)
        {
            foreach(TestPlan etp in externalPlans)
            {
                bool exist = false;
                foreach (TestPlan tp in testFlowPlans)
                {
                    if(etp.ExternalId == tp.ExternalId)
                    {
                        exist = true;
                        if (!etp.Name.Equals(tp.Name))
                        {
                            tp.Name = etp.Name;
                            tfStore.TestPlans.Edit(tp);
                        }
                    }
                }

                if(!exist)
                {
                    etp.Project = new Project();
                    etp.Project.Id = projectId;
                    tfStore.TestPlans.Create(etp);
                }
            }
        }

        public static void SyncSuites(List<TestSuite> testFlowSuites, List<TestSuite> externalSuites, TestFlowManager tfStore, int testPlanId)
        {
            foreach (TestSuite etp in externalSuites)
            {
                bool exist = false;
                foreach (TestSuite tp in testFlowSuites)
                {
                    if (etp.ExternalId == tp.ExternalId)
                    {
                        exist = true;
                        if (!etp.Name.Equals(tp.Name))
                        {
                            tp.Name = etp.Name;
                            tfStore.Suites.Edit(tp);
                        }

                        if (etp.SubSuites != null && etp.SubSuites.Count > 0)
                            SyncSuites(tp.SubSuites, etp.SubSuites, tfStore, testPlanId);
                    }
                }

                if (!exist)
                {
                    etp.Created = DateTime.Now;
                    etp.TestPlan = testPlanId;
                    etp.CreatedBy = tfStore.User.User_Id;
                    etp.LastModifiedBy = tfStore.User.User_Id;
                    etp.Modified = DateTime.Now;
                    tfStore.Suites.Create(etp);
                    if (etp.SubSuites != null && etp.SubSuites.Count > 0)
                        SyncSuites(new List<TestSuite>(), etp.SubSuites, tfStore, testPlanId);
                }
            }
        }

        public static void SyncTestCases(List<TestCase> testFlowTestCases, List<TestCase> externalTestCases, TestFlowManager tfStore, int suiteId)
        {
            foreach(TestCase tc in externalTestCases)
            {
                bool exist = false;
                foreach(TestCase iTc in testFlowTestCases)
                {
                    if(tc.ExternalId == iTc.ExternalId)
                    {
                        exist = true;
                        bool hasChanges = false;
                        if (!tc.Name.Equals(iTc.Name))
                        {
                            hasChanges = true;
                            iTc.Name = tc.Name;
                        }
                            
                        if (!tc.Description.Equals(iTc.Name))
                        {
                            hasChanges = true;
                            iTc.Description = tc.Description;
                        }

                        if(hasChanges)
                            tfStore.TestCases.Edit(iTc);
                    }
                }

                if(!exist)
                {
                    tc.TestSuite = new TestSuite();
                    tc.TestSuite.Id = suiteId;
                    tfStore.TestCases.Create(tc);
                }
            }
        }

        public static void SyncTestSteps(List<TestStep> testFlowSteps, List<TestStep> externalSteps, TestFlowManager tfStore, int testCaseId)
        {
            foreach(TestStep ts in externalSteps)
            {
                bool exist = false;
                foreach(TestStep iTs in testFlowSteps)
                {
                    if(ts.ExternalId == iTs.ExternalId)
                    {
                        exist = true;
                        bool hasChanged = false;
                        if(!ts.Name.Equals(iTs.Name))
                        {
                            iTs.Name = ts.Name;
                            hasChanged = true;
                        }

                        if(!ts.Result.Equals(iTs.Result))
                        {
                            iTs.Result = ts.Result;
                            hasChanged = true;
                        }

                        if (hasChanged)
                            tfStore.Steps.Edit(iTs);
                    }
                }

                if(!exist)
                {
                    ts.TestCase = testCaseId;
                    tfStore.Steps.Create(ts);
                }
            }
        }
    }
}
