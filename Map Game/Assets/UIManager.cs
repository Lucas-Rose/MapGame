using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private TMP_Text countDownText;
    [SerializeField] private TMP_Text killfeedText;
    [SerializeField] private int maxKillFeedActions;
    [SerializeField] private GameObject pauseMenu;

    private List<string> killfeedStack;

    // Start is called before the first frame update
    void Start()
    {
        killfeedStack = new List<string>();
        pauseMenu.SetActive(false);
    }

    public void UpdateRoundTimer(float time)
    {
        timerText.text = time.ToString("f1");
    }
    public void UpdateCountDown(float time)
    {
        countDownText.text = (time + 1).ToString("f0"); //Don't want to see zero;
    }
    public void DeactivateCountdown()
    {
        countDownText.gameObject.SetActive(false);
    }
    public void TogglePause(bool state)
    {
        pauseMenu.SetActive(state);
    }

    public void UpdateKillFeed(string action, string player, string victim)
    {
        switch (action) {
            case ("Kill"):
                killfeedStack.Add(player + " killed: " + victim + "\n");
                break;
            case ("Died"):
                killfeedStack.Add(player + " died..." + "\n");
                break;
            case ("Respawned"):
                killfeedStack.Add(player + " respawned..." + "\n");
                break; 
            case ("Moving"):
                break;
            case ("Update"):
                break;
        }
        if(killfeedStack.Count > maxKillFeedActions)
        {
            killfeedStack.RemoveAt(0);
        }
        LoadKillfeed();
    }
    private void LoadKillfeed()
    {
        killfeedText.text = "";
        for(int i = 0; i < killfeedStack.Count; i++)
        {
            killfeedText.text += killfeedStack[i];
        }
    }
}
