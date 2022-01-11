using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonValheim
{
    class PickableDV
    {
        public void SetRespawnTime(Pickable instance, int time)
        {
            instance.m_respawnTimeMinutes = 60;
        }
    }
}
