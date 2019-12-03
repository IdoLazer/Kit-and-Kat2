using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ManagerController : MonoBehaviour
{
    public GameObject startGame;
    public GameObject gameOn;
    public GameObject gameOnCanvas;
    public GameObject endGame;
    public GameObject endGameCanvas;
    public GameObject ball;

    public float gameTimeSeconds;
    public TextMeshProUGUI scoreRecordMsg;
    public TextMeshProUGUI currScoredMsg;
    public TextMeshProUGUI currTimerMsg;

    private static ManagerController _instance;

    private readonly int timerCountDown = 100;
    private int mode;
    private float startTime;
    private static float scoreRecord = 0f;
    private static float currScore = 0f;
 

    public static ManagerController Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
        mode = 1;
        startGame.SetActive(true);
        gameOn.SetActive(false);
        gameOnCanvas.SetActive(false);
        endGame.SetActive(false);
        endGameCanvas.SetActive(false);

    }

    void Update()
    {
        switch (mode)
        {
            case 1:
                StartMode();
                break;
            case 2:
                PlayMode();
                break;
            case 3:
                EndMode();
                break;
        }

    }

    void StartMode()
    {
        if (Input.anyKey)
        {
            StartPlayTheGame();
        }
    }

    void PlayMode()
    {
        float timeProgress = TimerUpdate();

        if (timeProgress >= gameTimeSeconds)
        {
            EndPlayTheGame();
        }

        //if(win)
    }

    void EndMode()
    {

    }

    public void StartPlayTheGame()
    {
        mode = 2;

        startGame.SetActive(false);

        gameOn.SetActive(true);
        gameOnCanvas.SetActive(true);

        endGame.SetActive(false);
        endGameCanvas.SetActive(false);

        startTime = Time.time;
    }

    private void EndPlayTheGame()
    {
        mode = 3;
        startGame.SetActive(false);

        gameOn.SetActive(false);
        gameOnCanvas.SetActive(false);

        endGame.SetActive(true);
        endGameCanvas.SetActive(true);

        if (currScore > scoreRecord) { scoreRecord = currScore; }
        scoreRecordMsg.text = scoreRecord.ToString("f2");
        currScoredMsg.text = currScore.ToString("f2");

    }

    private float TimerUpdate()
    {
        float t = Time.time - startTime;
        int precentTimePassed = (int) ((t / gameTimeSeconds) * 100);
        //string minutes = (timerCountDown - Mathf.Ceil(t / 60)).ToString();
        //string seconds = (60 - (t % 60)).ToString("f1");
        currScore = (timerCountDown - precentTimePassed);
        currTimerMsg.text = currScore.ToString("f2");

        t = BallController.GetHealth();
        return t;
    }
}
