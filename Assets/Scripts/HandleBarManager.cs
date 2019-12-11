using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleBarManager : MonoBehaviour
{

    private GameObject selectedObject;
    private Rigidbody rbSelected;
    public BodySourceView bsv;
    private List<Vector3> lastPos;

    private void Awake()
    {
        lastPos = new List<Vector3>();
    }

    // Update is called once per frame
    void Update()
    {
        if (selectedObject != null)
        {
            if (bsv.CheckLeftHandClosed())
            {
                if (bsv.CheckRightHandClosed())
                {
                    Vector3 trans = bsv.getTranslation() - transform.position;
                    selectedObject.transform.position += trans;


                    Vector3 rot = bsv.getRotation() - transform.eulerAngles;
                    Quaternion rotation = Quaternion.Euler(rot);
                    selectedObject.transform.rotation *= rotation;

                    lastPos.Add(selectedObject.transform.position);
                    if(lastPos.Count == 6)
                    {
                        lastPos.RemoveAt(0);

                    }
                }
                else
                {
                    float scaler = bsv.getScaler() - transform.localScale.x;
                    selectedObject.transform.localScale += new Vector3(scaler, scaler, scaler);
                    rbSelected.mass = selectedObject.transform.localScale.x / 3.0f;
                }
            }
            

            if (!bsv.CheckLeftHandClosed() && !bsv.CheckRightHandClosed())
            {

                Vector3 launch = Vector3.zero;

                for (int i = 1; i < lastPos.Count; ++i)
                {
                    launch += (lastPos[i] - lastPos[i - 1]);
                }

                launch = new Vector3(launch.x, launch.y, -launch.z);
                rbSelected.AddForce(launch * 500.0f);

                rbSelected.useGravity = true;
                rbSelected = null;
                selectedObject = null;
                if(lastPos.Count > 0)
                    lastPos.Clear();
            }
        }
        
        transform.position = bsv.getTranslation();
        transform.rotation = Quaternion.Euler(bsv.getRotation());
        transform.localScale = new Vector3(bsv.getScaler(), 0.5f, 0.5f);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Object")
        {
            if (bsv.CheckLeftHandClosed())
            {
                selectedObject = other.gameObject;
                rbSelected = selectedObject.GetComponent<Rigidbody>();
                rbSelected.useGravity = false;
            }
        }
    }

}
