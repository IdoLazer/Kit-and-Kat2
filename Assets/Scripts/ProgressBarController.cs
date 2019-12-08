using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBarController : MonoBehaviour
{
    public GameObject ball;

    private Vector3 offSet = new Vector3(0, -25, 0);
    public Slider progressBar;


    void Update()
    {
        progressBar.value = ball.GetComponent<BallController>().mHealth / 100;
        Vector3 newPos = Camera.main.WorldToScreenPoint(ball.transform.position);
        progressBar.transform.position = newPos + offSet;

    }
}
