using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public BodySourceView bsv;
    public GameObject ball;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!bsv.CheckLeftHandClosed() && bsv.CheckRightHandClosed() && ball.GetComponent<Rigidbody>().velocity == Vector3.zero &&
            Vector3.Distance(ball.transform.position, new Vector3(0.0f, ball.transform.localScale.y / 2.0f, 20.0f)) >= 1.0f)
        {
            ResetGame();
        }
    }

    void ResetGame()
    {
        ball.transform.position = new Vector3(0.0f, ball.transform.localScale.y / 2.0f, 20.0f);
    }
}
