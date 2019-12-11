using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnapObject : MonoBehaviour
{
    public Transform dest;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

  
    private void OnTriggerStay(Collider other)
    {
        if(other.tag == "Object")
        {
            Debug.Log("collision");
            other.transform.position = new Vector3(dest.position.x, dest.position.y + other.transform.localScale.y/2.0f, dest.position.z);
            other.GetComponent<Rigidbody>().velocity = new Vector3(0.0f, 0.0f, 0.0f);
        }
    }
}
