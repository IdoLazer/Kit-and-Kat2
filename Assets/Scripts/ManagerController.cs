using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class ManagerController : MonoBehaviour
{
    public GameObject startGame;
    public GameObject startGameCanvas;
    public GameObject gameOn;
    public GameObject gameOnCanvas;
    public GameObject endGame;
    public GameObject endGameCanvas;
    public GameObject ball;
    public GameObject backgroundBallInAir;
    public GameObject backgroundBallInCat;

    public TextMeshProUGUI scoreRecordMsg;
    public TextMeshProUGUI currScoredMsg;
    public TextMeshProUGUI currTimerMsg;

    private static ManagerController _instance;

    private int mode;
    private static float scoreRecord = 0f;
    private static float currScore = 0f;
    private bool backgroundModeHeldByCat = false; //First state is not held by cat
 

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
        startGameCanvas.SetActive(true);
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
        float healthProgress = HealthUpdate();
        currScore = healthProgress;

        if (healthProgress <= 0)
        {
            EndPlayTheGame();
        }


        if (backgroundModeHeldByCat != ball.GetComponent<BallController>().isHeldByCat)
        {
            backgroundModeHeldByCat = ball.GetComponent<BallController>().isHeldByCat;
            if (backgroundModeHeldByCat)
            {
                backgroundBallInCat.SetActive(true);
                backgroundBallInAir.SetActive(false);
            }
            else
            {
                backgroundBallInCat.SetActive(false);
                backgroundBallInAir.SetActive(true);
            }

        }
        //if(win)
    }

    void EndMode()
    {

    }

    public void RestartGame()
    {
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);

    }

    public void StartPlayTheGame()
    {
        mode = 2;

        startGame.SetActive(false);
        startGameCanvas.SetActive(false);

        gameOn.SetActive(true);
        gameOnCanvas.SetActive(true);

        endGame.SetActive(false);
        endGameCanvas.SetActive(false);

    }

    private void EndPlayTheGame()
    {
        mode = 3;
        startGame.SetActive(false);
        startGameCanvas.SetActive(false);

        gameOn.SetActive(false);
        gameOnCanvas.SetActive(false);

        endGame.SetActive(true);
        endGameCanvas.SetActive(true);

        if (currScore > scoreRecord) { scoreRecord = currScore; }
        scoreRecordMsg.text = scoreRecord.ToString("f2");
        currScoredMsg.text = currScore.ToString("f2");

    }

    private float HealthUpdate()
    {

        float t = ball.GetComponent<BallController>().mHealth;
        return t;
    }
}
