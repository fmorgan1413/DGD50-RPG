using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Enemy
{
    public string name;

    public float health;
    public float currentHealth;

    public float actionPoints;
    public float currtentAP;



    //*********************special enemy crap***************************

    //*******************enum go here if you need different enemy types********************
    //public Type enemyType;

    public float baseDEF;
    public float currentDEF;

    public float agility;

    public List<AttackHandler> attackHandlers = new List<AttackHandler>();
}
