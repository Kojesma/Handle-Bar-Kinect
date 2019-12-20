using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HandleBarManager : MonoBehaviour
{

    private GameObject selectedObject;
    private Rigidbody rbSelected;
    public BodySourceView bsv;
    private List<Vector3> lastPos;

    private bool useGravaty;

    public bool chamber = false;

    private void Awake()
    {
        lastPos = new List<Vector3>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            Rigidbody[] rbsObjects = GameObject.FindObjectsOfType<Rigidbody>();
            foreach (Rigidbody rb in rbsObjects)
            {
                if (rb.gameObject.tag == "Object")
                {
                    rb.useGravity = !rb.useGravity;
                    rb.isKinematic = !rb.isKinematic;
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("Menu", LoadSceneMode.Single);
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
        }
        if (selectedObject != null)
        {
            rbSelected.velocity = new Vector3(0.0f, 0.0f, 0.0f);
            if (bsv.CheckLeftHandClosed())
            {
                if (bsv.CheckRightHandClosed())
                {
                    Vector3 trans = bsv.getTranslation() - transform.position;
                    selectedObject.transform.position += trans;


                    Vector3 rot = bsv.getRotation() - transform.eulerAngles;
                    Quaternion rotation = Quaternion.Euler(rot);
                    selectedObject.transform.rotation *= rotation;

                    if (!chamber)
                    {
                        lastPos.Add(selectedObject.transform.position);
                        if (lastPos.Count == 51)
                        {
                            lastPos.RemoveAt(0);

                        }
                    }
                }
                else
                {
                    float scaler = bsv.getScaler() - transform.localScale.x;
                    selectedObject.transform.localScale += new Vector3(scaler, scaler, scaler);
                    if (!chamber)
                    {
                        rbSelected.mass = selectedObject.transform.localScale.x;
                    }
                }
            }
            

            if (!bsv.CheckLeftHandClosed() && !bsv.CheckRightHandClosed())
            {

                if (!chamber)
                {
                    Vector3 launch = Vector3.zero;

                    for (int i = 1; i < lastPos.Count; ++i)
                    {
                        launch += (lastPos[i] - lastPos[i - 1]);
                    }

                    if (lastPos.Count != 0)
                    {
                        launch = new Vector3(launch.x / 3.0f, launch.y / 5.0f, launch.z) / lastPos.Count * 150.0f;
                    }
                    
                    rbSelected.velocity = launch;
                    //rbSelected.AddForce(launch * 700.0f);
                    if (lastPos.Count > 0)
                        lastPos.Clear();
                }
                rbSelected.maxAngularVelocity = 7;
                rbSelected.useGravity = useGravaty;
                rbSelected = null;
                selectedObject = null;                
            }
        }
        
        transform.position = bsv.getTranslation();
        transform.rotation = Quaternion.Euler(bsv.getRotation());
        transform.localScale = new Vector3(bsv.getScaler(), 0.5f, 0.5f);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Object" && selectedObject == null)
        {
            if (bsv.CheckLeftHandClosed())
            {
                selectedObject = other.gameObject;
                rbSelected = selectedObject.GetComponent<Rigidbody>();
                useGravaty = rbSelected.useGravity;
                rbSelected.useGravity = false;
                rbSelected.velocity = new Vector3(0.0f, 0.0f, 0.0f);
                rbSelected.maxAngularVelocity = 0;
            }
        }
    }

}
