using UnityEngine;
[CreateAssetMenu(fileName = "NewWeights", menuName = "WFC/Weights")]
public class WeightsSO : ScriptableObject
{
    [Range(0, 10)] public int intersectionWeight;
    [Range(0, 10)] public int roadStraightWeight;
    [Range(0, 10)] public int grassWeight;
    [Range(0, 10)] public int roadTurnWeight;

    public int GetWeight(Attribute a)
    {
        return a switch
        {
            Attribute.Intersection => intersectionWeight,
            Attribute.RoadStraight => roadStraightWeight,
            Attribute.Grass => grassWeight,
            Attribute.RoadTurn => roadTurnWeight,
            _ => 0
        };
    }
}
