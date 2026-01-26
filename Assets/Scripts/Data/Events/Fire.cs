using Core;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Android;
using Utils;

namespace Data.Events
{
    [CreateAssetMenu(menuName = "Events/Fire")]
    public class Fire : GameEvent
    {
        public override bool IsThresholdEvent => false;
        public override bool GetsWorsOverTime => true;
        
    }
}