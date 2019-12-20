using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public BodySourceView bsv;
    public GameObject ball;
    public Transform bar;
    public Text text;

    public int score = 0;

    private void Awake()
    {
       Physics.IgnoreLayerCollision(9, 10);
        Physics.IgnoreLayerCollision(11, 10);
        ResetGame();
        ActualiseScore();
    }

    public void ResetGame()
    {
        ball.transform.position = new Vector3(0.0f, ball.transform.localScale.y / 2.0f + 3.0f, bar.position.z);
        ball.GetComponent<Rigidbody>().velocity = new Vector3(0.0f, 0.0f, 0.0f);
    }


    public void ActualiseScore()
    {
        text.text = "Score : " + score;
    }

}
