using UnityEngine;

public class QuestAmountRandomizer
{
    //TODO: implement real quest amount randomizer
    public int GetQuestAmount()
    {
        return Random.Range(1, 5);
    }
}