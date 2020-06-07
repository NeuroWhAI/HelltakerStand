using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelltakerStand
{
    interface IStand
    {
        event Action StandRemoved;
        event Action StandChanged;

        string StandName { get; }
        int StandX { get; set; }
        int StandY { get; set; }
        bool StandTopmost { get; set; }

        void ResetStandTime();
    }
}
