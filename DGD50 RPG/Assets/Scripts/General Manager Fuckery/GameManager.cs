using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    // TODO: Fix selector activator to let player know which player has the turn
    // or find a different solution to that problem (change player character?)

    // TODO: connect magic attacks to AP
    // (make sure cant do those attacks with no AP and take AP away)

    public DoActions actionStates;

    public List<HandleTurns> turns = new List<HandleTurns>();
    public List<GameObject> PlayerCharacters = new List<GameObject>();
    public List<GameObject> Enemies = new List<GameObject>();

    public PlayerInputs playerInputs;
    public List<GameObject> PlayersToManage = new List<GameObject>();
    private HandleTurns playerChoice;

    public GameObject enemyButton;
    public Transform Spacer;

    //panels crap
    public GameObject actionsPanel;
    public GameObject selectEnemyPanel;
    public GameObject specialsPanel;

    // Start is called before the first frame update
    void Start()
    {
        actionStates = DoActions.WAIT;

        Enemies.AddRange(GameObject.FindGameObjectsWithTag("Enemy"));
        PlayerCharacters.AddRange(GameObject.FindGameObjectsWithTag("Player"));

        playerInputs = PlayerInputs.ACTIVATE;
        selectEnemyPanel.SetActive(false);

        EnemyButtons();
    }

    // Update is called once per frame
    void Update()
    {
        switch (actionStates)
        {
            case DoActions.WAIT: 
                if (turns.Count > 0)
                {
                    actionStates = DoActions.TAKE;
                }
                break;

            case DoActions.TAKE:
                GameObject actionTaker = GameObject.Find(turns[0].AttackerName);
                if (turns[0].Type == "Enemy")
                {
                    EnemyStateMachine EM = actionTaker.GetComponent<EnemyStateMachine>();
                    EM.PlayerToAttack = turns[0].attacked;
                    EM.currentState = EnemyStateMachine.States.ACTION;
                }

                if (turns[0].Type == "Player")
                {
                    PlayerStateMachine PM = actionTaker.GetComponent<PlayerStateMachine>();
                    PM.enemyToAttack = turns[0].attacked;
                    PM.currentState = PlayerStateMachine.States.ACTION;
                }

                actionStates = DoActions.PERFORM; 
                break;

            case DoActions.PERFORM:
                break;
        }

        switch (playerInputs)
        {
            case PlayerInputs.ACTIVATE:
                if (PlayersToManage.Count > 0)
                {
                    PlayersToManage[0].transform.Find("Selector").gameObject.SetActive(true);
                    playerChoice = new HandleTurns();

                    actionsPanel.SetActive(true);
                    playerInputs = PlayerInputs.WAIT;
                }
                break;

            case PlayerInputs.WAIT:
                break;

            case PlayerInputs.ACTION:
                break;

            case PlayerInputs.ENEMY_SELECT:
                break;

            case PlayerInputs.DONE:
                FinishPlayerInput();
                break;
        }    
    }

    public void GetActions(HandleTurns input)
    {
        turns.Add(input);
    }

    void EnemyButtons()
    {
        foreach (GameObject enemy in Enemies)
        {
            GameObject newButton = Instantiate(enemyButton) as GameObject;
            EnemySelect button = newButton.GetComponent<EnemySelect>();

            EnemyStateMachine currentEnemy = enemy.GetComponent<EnemyStateMachine>();

            TextMeshProUGUI buttonText = newButton.transform.Find("Text (TMP)").gameObject.GetComponent<TextMeshProUGUI>();
            buttonText.text = currentEnemy.enemy.name;

            button.enemy = enemy;

            newButton.transform.SetParent(Spacer,false);
        }
    }

    public void AttackButton()
    {
        playerChoice.AttackerName = PlayersToManage[0].name;
        playerChoice.attacker = PlayersToManage[0];
        playerChoice.Type = "Player";

        playerChoice.chosenAttack = PlayersToManage[0].GetComponent<PlayerStateMachine>().player.attackHandlers[0];

        //actionsPanel.SetActive(false);
        selectEnemyPanel.SetActive(true);
    }

    public void MagicButton() 
    {
        playerChoice.AttackerName = PlayersToManage[0].name;
        playerChoice.attacker = PlayersToManage[0];
        playerChoice.Type = "Player";

        playerChoice.chosenAttack = PlayersToManage[0].GetComponent<PlayerStateMachine>().player.attackHandlers[1];

        PlayersToManage[0].GetComponent<PlayerStateMachine>().player.currentAP -= PlayersToManage[0].GetComponent<PlayerStateMachine>().player.attackHandlers[1].attackAP;
        selectEnemyPanel.SetActive(true);
    }

    public void SpecialsButton()
    {
        specialsPanel.SetActive(true); 
    }

    public void BackToActions()
    {
        specialsPanel.SetActive(false);
    }

    public void EnemySelect(GameObject chosenEnemy)
    {
        playerChoice.attacked = chosenEnemy;
        playerInputs = PlayerInputs.DONE;
    }

    void FinishPlayerInput()
    {
        turns.Add(playerChoice);
        selectEnemyPanel.SetActive(false);
        specialsPanel.SetActive(false);
        PlayersToManage[0].transform.Find("Selector").gameObject.SetActive(false);

        PlayersToManage.RemoveAt(0);
        playerInputs = PlayerInputs.WAIT;
    }

    public enum DoActions
    {
        WAIT,
        TAKE,
        PERFORM
    }

    public enum PlayerInputs
    {
        ACTIVATE,
        WAIT,
        ACTION,
        ENEMY_SELECT,
        DONE
    }
}
