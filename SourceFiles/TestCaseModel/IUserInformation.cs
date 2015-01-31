using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestCaseModel
{
    public interface IUserInformation : ITestItemBase
    {
        int LastModifiedBy { get; set; }

        int CreatedBy { get; set; }

        DateTime Created { get; set; }

        DateTime Modified { get; set; }
    }
}
