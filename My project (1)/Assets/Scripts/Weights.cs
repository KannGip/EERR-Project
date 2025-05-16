using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable] public struct Weights
{
    [Range(0,10)] public int intersectionWeight;
    [Range(0,10)] public int roadStraightWeight;
    [Range(0,10)] public int grassWeight;
    [Range(0,10)] public int roadTurnWeight;
    public int GetWeight(Attribute a)
    {
        if(a==Attribute.Intersection)
            return intersectionWeight;
        else if(a==Attribute.RoadStraight)
            return roadStraightWeight;
        else if(a==Attribute.Grass)
            return grassWeight;
        else if(a==Attribute.RoadTurn)
            return roadTurnWeight;

        return 0;
    }
}
public enum Attribute {Intersection, RoadStraight, Grass, RoadTurn};
