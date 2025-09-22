using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
[Serializable]
public class Individual: IComparable<Individual>
{
    public float xDegree;
    public float yDegree;
    public float strength;

    public float fitness;

    public Individual(float xD, float yD, float s)
    {
        fitness = +1000f;
        xDegree = xD;
        yDegree = yD;
        strength = s;
    }

    public int CompareTo(Individual other) => fitness.CompareTo(other.fitness);
}
