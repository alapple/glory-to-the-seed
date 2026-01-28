using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public class WorldManager : MonoBehaviour
    {
        public List<RegionController> regions;

        public int GetTotalProduction()
        {
            int counter = 0;

            foreach (var region in regions)
            {
                counter += region.production;
            }
            return counter;
        }

        public int GetTotalHappiness()
        {
            return 0;
        }
    }
}