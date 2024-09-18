using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eTickets.Data
{
    public enum MovieCategory
    {
        Action = 1,  // Assigning explicit integer value 1 to the Action category
        Comedy,      // Default value is 2 (following the previous value)
        Drama,       // Default value is 3
        Documentary, // Default value is 4
        Cartoon,     // Default value is 5
        Horror       // Default value is 6
    }
}
