//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DataStore.EntityData
{
    using System;
    using System.Collections.Generic;
    
    public partial class TF_Case_Metrics
    {
        public int Metrics_Id { get; set; }
        public int TestCase_Id { get; set; }
        public string Metrics_Desc { get; set; }
        public int Failed { get; set; }
        public string Fail_Cause { get; set; }
        public System.DateTime Update_Date { get; set; }
    
        public virtual TF_TestCases TF_TestCases { get; set; }
    }
}