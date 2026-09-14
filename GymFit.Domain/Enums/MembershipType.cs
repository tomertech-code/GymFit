using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymFit.Domain.Enums
{

    public enum MembershipType
    {
        Basic = 1,          // Single branch
        Standard = 2,       // Single branch + guest access
        Premium = 3,        // 2-3 branches
        VIP = 4,           // All branches
        Corporate = 5       // Custom multi-branch
    }
}
