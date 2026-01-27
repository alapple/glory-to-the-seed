using UnityEngine;

namespace Data.Events
{
    [CreateAssetMenu(menuName = "Events/BrokenTractor")]
    public class BrokenTractor : GameEvent
    {
        public override bool IsThresholdEvent => false;
        public override bool GetsWorsOverTime => false;
    }
}