using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private UIManager ui;
    private BehaviourDispensor bd;

    //COUNTDOWN INFO
    [SerializeField] private float countDownLength;
    private float countDownTime;

    //ROUND INFO
    [SerializeField] private float roundLength;
    private bool roundPlaying;
    private float roundTime;

    private enum GameState
    {
        countdown,
        playing,
        finished
    }
    private GameState gameState;
    // Start is called before the first frame update
    void Start()
    {
        gameState = GameState.countdown;
        ui = FindObjectOfType<UIManager>();
        bd = FindObjectOfType<BehaviourDispensor>();
        roundTime = roundLength;
        countDownTime = countDownLength;
        ui.UpdateCountDown(roundTime);
        StartRound();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            SendFeedRequest("Kill", "Alpha", "Beta");
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            SendFeedRequest("Died", "Alpha", null);
        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            SendFeedRequest("Respawned", "Alpha", null);
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (roundPlaying)
            {
                Pause(!roundPlaying);
            }
            else
            {
                Pause(roundPlaying);
            }
        }
    }

    private void FixedUpdate()
    {
        switch (gameState)
        {
            case (GameState.countdown):
                countDownTime -= Time.deltaTime;
                ui.UpdateCountDown(countDownTime);
                if(countDownTime < 0)
                {
                    countDownTime = countDownLength;
                    ui.DeactivateCountdown();
                    gameState = GameState.playing;
                    bd.ToggleMove(true);
                }
                break;
            case (GameState.playing):
                if (roundPlaying)
                {
                    roundTime -= Time.deltaTime;
                    ui.UpdateRoundTimer(roundTime);
                }
                break;
            case (GameState.finished):
                break;
        }
    }
    public void StartRound()
    {
        roundTime = roundLength;
        roundPlaying = true;
    }

    public void SendFeedRequest(string action, string player, string victim)
    {
        ui.UpdateKillFeed(action, player, victim);
    }
    public void Pause(bool state)
    {
        roundPlaying = state;
        if (!roundPlaying)
        {
            bd.ToggleMove(false);
        }
        else
        {
            bd.ToggleMove(true);
        }
    }
}
