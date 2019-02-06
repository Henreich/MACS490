using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Angle : MonoBehaviour {

    public Transform target;
    private double angle;
    
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        angle = Vector3.Angle(transform.position, target.position);
        Debug.Log(angle);
	}
}
