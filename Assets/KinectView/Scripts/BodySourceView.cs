using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Kinect = Windows.Kinect;

public class BodySourceView : MonoBehaviour 
{
    public GameObject bar;
    public Material BoneMaterial;
    public GameObject BodySourceManager;
    
    private Dictionary<ulong, GameObject> _Bodies = new Dictionary<ulong, GameObject>();
    private BodySourceManager _BodyManager;

    private List<Kinect.JointType> jt_list;

    private Vector3 translation;
    private Vector3 rotation;
    private float scaler;
    private Quaternion rotate;
    private bool leftHandClosed;
    private bool rightHandClosed;
    private bool moveForward;

    public bool chamber = false;

    private Dictionary<Kinect.JointType, Kinect.JointType> _BoneMap = new Dictionary<Kinect.JointType, Kinect.JointType>()
    {        
        { Kinect.JointType.HandTipLeft, Kinect.JointType.HandLeft },
        { Kinect.JointType.ThumbLeft, Kinect.JointType.HandLeft },
        { Kinect.JointType.HandLeft, Kinect.JointType.WristLeft },
        { Kinect.JointType.WristLeft, Kinect.JointType.ElbowLeft },
        { Kinect.JointType.ElbowLeft, Kinect.JointType.ShoulderLeft },
        { Kinect.JointType.ShoulderLeft, Kinect.JointType.SpineShoulder },
        
        { Kinect.JointType.HandTipRight, Kinect.JointType.HandRight },
        { Kinect.JointType.ThumbRight, Kinect.JointType.HandRight },
        { Kinect.JointType.HandRight, Kinect.JointType.WristRight },
        { Kinect.JointType.WristRight, Kinect.JointType.ElbowRight },
        { Kinect.JointType.ElbowRight, Kinect.JointType.ShoulderRight },
        { Kinect.JointType.ShoulderRight, Kinect.JointType.SpineShoulder },
        
        { Kinect.JointType.SpineBase, Kinect.JointType.SpineMid },
        { Kinect.JointType.SpineMid, Kinect.JointType.SpineShoulder },
        { Kinect.JointType.SpineShoulder, Kinect.JointType.Neck },
        { Kinect.JointType.Neck, Kinect.JointType.Head },
    };
    
    void Update () 
    {
        if (BodySourceManager == null)
        {
            return;
        }
        
        _BodyManager = BodySourceManager.GetComponent<BodySourceManager>();
        if (_BodyManager == null)
        {
            return;
        }
        
        Kinect.Body[] data = _BodyManager.GetData();
        if (data == null)
        {
            return;
        }
        
        List<ulong> trackedIds = new List<ulong>();
        foreach(var body in data)
        {
            if (body == null)
            {
                continue;
              }
                
            if(body.IsTracked)
            {
                trackedIds.Add (body.TrackingId);
            }
        }
        
        List<ulong> knownIds = new List<ulong>(_Bodies.Keys);
        
        // First delete untracked bodies
        foreach(ulong trackingId in knownIds)
        {
            if(!trackedIds.Contains(trackingId))
            {
                Destroy(_Bodies[trackingId]);
                _Bodies.Remove(trackingId);
            }
        }

        foreach(var body in data)
        {
            if (body == null)
            {
                continue;
            }
            
            if(body.IsTracked)
            {
                if(!_Bodies.ContainsKey(body.TrackingId))
                {
                    _Bodies[body.TrackingId] = CreateBodyObject(body.TrackingId);
                }
                
                RefreshBodyObject(body, _Bodies[body.TrackingId]);
                RefreshBarTransform(_Bodies[body.TrackingId]);
                refreshCloseStatus(body);
            }
        }
    }
    
    private GameObject CreateBodyObject(ulong id)
    {
        GameObject body = new GameObject("Body:" + id);
        body.tag = "Player";
        jt_list = new List<Kinect.JointType>();
        jt_list.Add(Kinect.JointType.HandLeft);
        jt_list.Add(Kinect.JointType.HandRight);

        foreach (Kinect.JointType jt in jt_list)
        {
            GameObject jointObj;
            jointObj = GameObject.CreatePrimitive(PrimitiveType.Cube);

            jointObj.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
            jointObj.name = jt.ToString();
            jointObj.tag = "Player";
            jointObj.transform.parent = body.transform;
            jointObj.AddComponent<Rigidbody>();
            jointObj.GetComponent<Rigidbody>().useGravity = false;
            jointObj.GetComponent<BoxCollider>().isTrigger = true;
        }

        return body;
    }
    
    private void RefreshBarTransform(GameObject bodyObject)
    {
        Transform leftHand = bodyObject.transform.Find(Kinect.JointType.HandLeft.ToString());
        Transform rightHand = bodyObject.transform.Find(Kinect.JointType.HandRight.ToString());

        Vector3 hand_vec = rightHand.position - leftHand.position;

        //Set size of handle bar
        scaler = hand_vec.magnitude;

        //Set position of handle bar
        translation = leftHand.transform.position + hand_vec / 2.0f;


        //Set rotation of handle bar
        hand_vec.Normalize();

        float angle = Vector3.Angle(Vector3.right, new Vector3(hand_vec.x, hand_vec.y, 0.0f));
        float angle2 = Vector3.Angle(Vector3.forward, new Vector3(hand_vec.x, 0.0f, hand_vec.z)) - 90.0f;

        if (leftHand.position.y > rightHand.position.y)
        {
            angle = 360.0f - angle;
        }
        if (leftHand.position.x > rightHand.position.x)
        {
            angle2 = 360.0f - angle2;
        }
        
        rotation = new Vector3(0.0f, angle2, angle);
    }

    public Vector3 getTranslation()
    {
        return translation;
    }

    public Vector3 getRotation()
    {
        return rotation;
    }

    public float getScaler()
    {
        return scaler;
    }
    

    //Check if left and right hands are closed or not
    private void refreshCloseStatus(Kinect.Body body)
    {
        leftHandClosed = body.HandLeftState == Kinect.HandState.Closed;
        rightHandClosed = body.HandRightState == Kinect.HandState.Closed;
    }

    public bool CheckLeftHandClosed()
    {
        return leftHandClosed;
    }

    public bool CheckRightHandClosed()
    {
        return rightHandClosed;
    }

    private void RefreshBodyObject(Kinect.Body body, GameObject bodyObject)
    {

        foreach (Kinect.JointType jt in jt_list)
        {
            
            Kinect.Joint sourceJoint = body.Joints[jt];
            Kinect.Joint? targetJoint = null;

            if (_BoneMap.ContainsKey(jt))
            {
                targetJoint = body.Joints[_BoneMap[jt]];
            }

            Transform jointObj = bodyObject.transform.Find(jt.ToString());
            Vector3 pos = GetVector3FromJoint(sourceJoint) ;
            jointObj.localPosition = new Vector3(-pos.x * 3.0f, pos.y * 5.0f, pos.z * 2.0f);
            if (chamber)
            {
                jointObj.localPosition = new Vector3(-pos.x, pos.y, pos.z);
            }
        }
    }
    
    private static Color GetColorForState(Kinect.TrackingState state)
    {
        switch (state)
        {
        case Kinect.TrackingState.Tracked:
            return Color.green;

        case Kinect.TrackingState.Inferred:
            return Color.red;

        default:
            return Color.black;
        }
    }
    
    private static Vector3 GetVector3FromJoint(Kinect.Joint joint)
    {
        return new Vector3(joint.Position.X * 10, joint.Position.Y * 10, joint.Position.Z * 10);
    }
}
