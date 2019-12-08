using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinTheGame : MonoBehaviour
{
    public bool EnteredTrigger;

    public void OnTriggerEnter2D(Collider2D other)
    {
        EnteredTrigger |= other.CompareTag("Ball");
    }
}
