using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Player
{
    public string name;

    public float health;
    public float currentHealth;

    public float actionPoints;
    public float currentAP;

    //*********extra stats and stuff can go here****************
    public float agility;

    public List<AttackHandler> attackHandlers = new List<AttackHandler>();
}
