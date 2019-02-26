using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;

public class Text : MonoBehaviour {
    public TextMeshPro text;
    private double MINIMUM_FONT_SIZE = 1.0f;
    private double MAXIMUM_FONT_SIZE = 7.0f;

    // Use this for initialization
    void Start () {
        //text = GetComponent<TextMeshPro>();
	}
	
	// Update is called once per frame
	void Update () {
       
        if (Input.GetKey(KeyCode.DownArrow) && text.fontSize > MINIMUM_FONT_SIZE)
        {
            text.fontSize -= 0.01f;
        }
      

        if (Input.GetKey(KeyCode.UpArrow) && text.fontSize < MAXIMUM_FONT_SIZE)
        {
            text.fontSize += 0.01f;
        }
        
        // Return the result
        if (Input.GetKey(KeyCode.R))
        {
            Debug.Log("Current font size: " + text.fontSize);
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            Debug.Log("Writing to file...");
            StreamWriter writer = new StreamWriter("Assets/Resources/test.txt", true);
            writer.WriteLine(text.fontSize);
            writer.Close();
        }
    }
}
