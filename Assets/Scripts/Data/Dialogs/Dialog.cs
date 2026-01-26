using UnityEngine;

namespace Data.Dialogs
{
    [CreateAssetMenu(fileName = "New Dialog", menuName = "ScriptableObject/Dialog")]
    public class Dialog : ScriptableObject
    {
        public string title;
        [TextArea]
        public string text;
    }
}