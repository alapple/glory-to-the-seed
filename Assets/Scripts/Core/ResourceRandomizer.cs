using System.Collections.Generic;

public class ResourceRandomizer
{
    //TODO: Make actual randomizer. Has to return the SortedDictionary<string, int>
    public Dictionary<string, int> ReturnResources()
    {
        Dictionary<string, int> resources = new Dictionary<string, int>();

        resources.Add("Potato", 10);
        resources.Add("Vodka", 10);
        resources.Add("Megafon", 10);
        resources.Add("Seal", 10);
        resources.Add("Repair Materials", 10);
        resources.Add("Workers", 10);

        return resources;
    }
}