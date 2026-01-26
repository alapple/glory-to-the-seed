using Core;
using UnityEngine;
using Utils;

namespace Data.Events    
{
    [CreateAssetMenu(menuName = "Events/Revolt")]
    public class Revolt : GameEvent
    {
        public override bool IsThresholdEvent => true;        
        public override bool GetsWorsOverTime => false;
    }
}