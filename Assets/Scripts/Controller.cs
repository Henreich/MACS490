using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class Controller : MonoBehaviour { 
    public XRNode NodeType;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.localPosition = InputTracking.GetLocalPosition(NodeType);
        transform.localRotation = InputTracking.GetLocalRotation(NodeType);

        Debug.Log(InputTracking.GetLocalPosition(NodeType));
    }
}
