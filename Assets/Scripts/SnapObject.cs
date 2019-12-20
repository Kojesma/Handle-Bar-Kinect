using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SnapObject : MonoBehaviour
{
    public Transform dest;

    public GameManager gm;
    bool win = false;
    public Text sizeText;

  
    private void OnTriggerStay(Collider other)
    {
        if(other.tag == "Object" )
        {
            if (Mathf.Abs(other.transform.localScale.x - transform.localScale.x) < 0.5f)
            {
                other.transform.position = new Vector3(dest.position.x, dest.position.y + other.transform.localScale.y / 2.0f, dest.position.z);
                other.GetComponent<Rigidbody>().velocity = new Vector3(0.0f, 0.0f, 0.0f);
                if (!win)
                {
                    StartCoroutine("NextPole");
                    win = true;
                    gm.score += 1;
                    gm.ActualiseScore();
                }
            }else if(other.transform.localScale.x - transform.localScale.x < 0.0f)
            {
                StartCoroutine("GiveInformation", "Your ball is too small !");
            }
            else if (other.transform.localScale.x - transform.localScale.x > 0.0f)
            {
                StartCoroutine("GiveInformation", "Your ball is too big !");
            }

        }
    }

    IEnumerator NextPole()
    {
        yield return new WaitForSeconds(1.0f);
        transform.position = new Vector3(Random.Range(-15.0f, 15.0f) , 4.0f, Random.Range(-30.0f, -5.0f));
        float newScale = Random.Range(1, 10);
        transform.localScale = new Vector3(newScale, transform.localScale.y, newScale);
        gm.ResetGame();
        win = false;
    }

    IEnumerator GiveInformation(string message)
    {
        sizeText.gameObject.SetActive(true);
        sizeText.text = message;
        yield return new WaitForSeconds(2.0f);
        sizeText.gameObject.SetActive(false);        
    }
}
