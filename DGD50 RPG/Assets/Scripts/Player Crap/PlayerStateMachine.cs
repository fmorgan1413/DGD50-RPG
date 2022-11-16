using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStateMachine : MonoBehaviour
{
    private GameManager GM;
    public Player player;
    public States currentState;
    private bool alive = true;

    //progress bar
    private float currentCool = 0.0f;

    //figure out math to make sure higher agility goes first
    // TODO: try multiplying instead of dividing
    private float maxCool{
        get{ return player.agility / 5f; }
    }
    public Image progressBar;
    public GameObject selector;

    //IENumertor crap
    public GameObject enemyToAttack;
    private bool actionStarted = false;
    private Vector3 startPos;
    private float animSpeed = 5.0f;

    public TextMeshProUGUI HPText;
    public TextMeshProUGUI APText;

    // Start is called before the first frame update
    void Start()
    {
        GM = GameObject.Find("Manager").GetComponent<GameManager>();
        currentState = States.PROCESSING;
        // maxCool = player.agility / 5.0f;
        selector.SetActive(false);
        startPos = transform.position;

        HPText.text = "HP: " + player.currentHealth + "/" + player.health;
        APText.text = "AP: " + player.currentAP + "/" + player.actionPoints;
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
                GM.PlayersToManage.Add(this.gameObject);
                currentState = States.WAITING;
                break;

            case States.WAITING:
                break;

            case States.SELECTING:
                break;

            case States.ACTION:
                StartCoroutine(TimeForAction());
                break;

            case States.DEAD:
                if (alive)
                {
                    return;
                }
                else
                {
                    this.gameObject.tag = "DeadPlayer";
                    GM.PlayersToManage.Remove(this.gameObject);
                    GM.PlayerCharacters.Remove(this.gameObject);
                    selector.SetActive(false);

                    GM.selectEnemyPanel.SetActive(false);

                    for (int i = 0; i < GM.turns.Count; i++)
                    {
                        if (GM.turns[i].attacked == this.gameObject)
                        {
                            GM.turns.Remove(GM.turns[i]);
                        }
                    }

                    //TODO: figure out why coloring changing not working
                    this.gameObject.GetComponent<SpriteRenderer>().color = new Color32(105, 105, 105, 255);

                    GM.playerInputs = GameManager.PlayerInputs.ACTIVATE;
                    alive = false;
                }

                break;
        }
    }

    void ProgressBar()
    {
        currentCool = currentCool + Time.deltaTime;
        float calcCool = currentCool / maxCool;

        progressBar.transform.localScale = new Vector3(Mathf.Clamp(calcCool, 0, 1), progressBar.transform.localScale.y);

        if (currentCool >= maxCool)
        {
            currentState = States.ADD;
        }
    }

    private IEnumerator TimeForAction()
    {
        if (actionStarted)
        {
            yield break;
        }

        actionStarted = true;

        // animation/response stuff
        Vector3 enemyPos = new Vector3(enemyToAttack.transform.position.x + 1.5f, enemyToAttack.transform.position.y);
        while (MoveTowardEnemy(enemyPos))
        {
            yield return null;
        }

        yield return new WaitForSeconds(0.25f);

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

    public void TakeDamage(float damageAmt)
    {
        player.currentHealth -= damageAmt;
        HPText.text = "HP: " + player.currentHealth + "/" + player.health;
        if (player.currentHealth <= 0)
        {
            currentState = States.DEAD;
        }
    }

    void DoDamage()
    {
        float damage = GM.turns[0].chosenAttack.attackDamage;
        enemyToAttack.GetComponent<EnemyStateMachine>().TakeDamage(damage);
    }

    public enum States
    {
        PROCESSING,
        ADD,
        WAITING,
        SELECTING,
        ACTION,
        DEAD
    }
}