using UnityEngine;

namespace Data.Dialogs
{
    [CreateAssetMenu(menuName = "Dialog")]
    public class Dialog : ScriptableObject
    {
        public string title;
        [TextArea] public string text;
    }
}