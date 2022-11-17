using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySelect : MonoBehaviour
{
    public GameObject enemy;

    public void SelectEnemy() 
    {
        GameObject.Find("Manager").GetComponent<GameManager>().EnemySelect(enemy);
    }

    public void ToggleSelectorOff()
    {
        enemy.transform.Find("Selector").gameObject.SetActive(false);
    }

    public void ToggleSelectorOn()
    {
        enemy.transform.Find("Selector").gameObject.SetActive(true);
    }
}
