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
    
    public partial class TF_Projects
    {
        public TF_Projects()
        {
            this.TF_TestPlan = new HashSet<TF_TestPlan>();
        }
    
        public int Project_Id { get; set; }
        public string Name { get; set; }
        public int Collection_Id { get; set; }
        public int External_Id { get; set; }
    
        public virtual TF_Collections TF_Collections { get; set; }
        public virtual ICollection<TF_TestPlan> TF_TestPlan { get; set; }
    }
}
