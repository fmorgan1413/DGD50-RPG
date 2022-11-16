using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HandleTurns 
{
    public string AttackerName;
    public string Type;

    public GameObject attacker;
    public GameObject attacked;

    public AttackHandler chosenAttack;
}
