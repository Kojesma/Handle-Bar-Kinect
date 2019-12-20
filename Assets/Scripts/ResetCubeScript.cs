using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetCubeScript : MonoBehaviour
{
    bool canReset = false;
    bool step2 = false;

    public BodySourceView bsv;
    public GameManager gm;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && !bsv.CheckRightHandClosed())
        {
            canReset = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if ( other.tag == "Player" && canReset && bsv.CheckRightHandClosed())
        {
            canReset = false;
            gm.ResetGame();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            canReset = false;
        }
    }

}
