using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallDetector : MonoBehaviour
{
    public GameManager Manager;
    public PlayerMovementManager PlayerManager;
    
    // Start is called before the first frame update
    void Start()
    {
        gameObject.tag = "Win Door Closed";
    }

    // Update is called once per frame
    void Update()
    {
        if (Manager.isFinished)
        {
            gameObject.tag = "Win Door Opened";
        }
        else
        {
            gameObject.tag = "Win Door Closed";
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && gameObject.tag == "Win Door Opened")
        {
           PlayerManager.GameWin();
        }
    }
}
