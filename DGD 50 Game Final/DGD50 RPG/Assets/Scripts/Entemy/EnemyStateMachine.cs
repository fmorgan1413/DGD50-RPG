using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;

public class EnemyStateMachine : MonoBehaviour
{
    public Enemy enemy;
    public States currentState;
    private GameManager GM;

    public GameObject selector;

    private float currentCool = 0.0f;

    //figure out math to make sure higher agility goes first
    // TODO: try multiplying instead of dividing
    private float maxCool
    {
        get { return enemy.agility / 5f; }
    }

    private Vector3 startPos;

    //IENumerator stuff
    private bool actionStarted = false;
    public GameObject PlayerToAttack;
    private float animSpeed = 5.0f;

    // Start is called before the first frame update
    void Start()
    {
        currentState = States.PROCESSING;
        GM = GameObject.Find("Manager").GetComponent<GameManager>();
        startPos = transform.position;
        selector.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case States.PROCESSING:
                ProgressBar();
                break;

            case States.ADD:
                ChooseAction();
                currentState = States.WAITING;
                break;

            case States.WAITING:
                break;

            case States.ACTION:
                StartCoroutine(TimeForAction());
                break;

            case States.DEAD:
                this.gameObject.tag = "DeadEnemy";
                GM.Enemies.Remove(this.gameObject);
                //GM.PlayerCharacters.Remove(this.gameObject);

                for (int i = 0; i < GM.turns.Count; i++)
                {
                    if (GM.turns[i].attacked == this.gameObject)
                    {
                        GM.turns.Remove(GM.turns[i]);
                    }
                }
                this.gameObject.SetActive(false);
                break;
        }
    }

    void ProgressBar()
    {
        currentCool = currentCool + Time.deltaTime;
        float calcCool = currentCool / maxCool;

        if (currentCool >= maxCool)
        {
            currentState = States.ADD;
        }
    }

    void ChooseAction()
    {
        HandleTurns attack = new HandleTurns();
        attack.AttackerName = enemy.name;
        attack.Type = "Enemy";
        attack.attacker = this.gameObject;

        attack.attacked = GM.PlayerCharacters[Random.Range(0, GM.PlayerCharacters.Count)];

        //enemy randomly chooses attack
        int num = Random.Range(0, enemy.attackHandlers.Count);
        attack.chosenAttack = enemy.attackHandlers[num];
        Debug.Log(this.gameObject.name + " has chosen " + attack.chosenAttack + " / damage: " + attack.chosenAttack.attackDamage); 

        GM.GetActions(attack);
    }

    private IEnumerator TimeForAction()
    {
        if(actionStarted)
        {
            yield break;
        }

        actionStarted = true;

        // animation/response stuff
        Vector3 playerPos = new Vector3(PlayerToAttack.transform.position.x - 1.5f, PlayerToAttack.transform.position.y);
        while(MoveTowardEnemy(playerPos))
        {
            yield return null;
        }

        PlayerToAttack.gameObject.GetComponent<SpriteRenderer>().color = new Color32(157, 71, 71,255);
        
        yield return new WaitForSeconds(0.15f);
        
        PlayerToAttack.gameObject.GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255,255);
        DoDamage();

        Vector3 frstPos = startPos;
        while (MoveTowardStart(frstPos))
        {
            yield return null;
        }

        GM.turns.RemoveAt(0);
        GM.actionStates = GameManager.DoActions.WAIT;

        actionStarted = false;
        currentCool = 0.0f;
        currentState = States.PROCESSING;
    }

    private bool MoveTowardEnemy(Vector3 target)
    {
        return target != (transform.position = Vector3.MoveTowards(transform.position, target, animSpeed * Time.deltaTime));
    }

    private bool MoveTowardStart(Vector3 target)
    {
        return target != (transform.position = Vector3.MoveTowards(transform.position, target, animSpeed * Time.deltaTime));
    }

    void DoDamage()
    {
        float damage = GM.turns[0].chosenAttack.attackDamage;
        PlayerToAttack.GetComponent<PlayerStateMachine>().TakeDamage(damage);
    }

    public void TakeDamage(float damage)
    {
        enemy.currentHealth -= damage;

        if(enemy.currentHealth <= 0)
        {
            enemy.currentHealth = 0;
            currentState = States.DEAD;
        }
    }
    public enum States
    {
        PROCESSING,
        ADD,
        WAITING,
        ACTION,
        DEAD
    }
}
