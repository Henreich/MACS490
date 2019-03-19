using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using TMPro;

public class Controller : MonoBehaviour
{
    public SteamVR_Action_Single squeezeAction;
    public SteamVR_Action_Vector2 touchpadAction;

    private DataController dataController;

    public TextMeshPro text;
    public Transform screen;
    public Camera head;
    public Transform textCollection;

    private double MINIMUM_FONT_SIZE = 1.0f;
    private double MAXIMUM_FONT_SIZE = 7.0f;
    private double TRIGGER_THRESHOLD = 0.0f;
    private float TEXT_SIZING_INCREMENTS = 0.0001f;
    private float TEXT_SIZE_CHANGE_MODIFIER = 5;

    private int currentTextShown = 0;

    private int currentVisibleObject = 0;
    private List<Transform> textList;

    // Start is called before the first frame update
    void Start()
    {
        dataController = ScriptableObject.CreateInstance<DataController>();
        
        /*
         * Set up the list of text object transforms
         */
        textList = new List<Transform>();
        foreach (Transform text in textCollection)
        {
            textList.Add(text);
        }
        //foreach (Transform radius in textCollection)
        //{
        //    foreach (Transform text in radius)
        //    {
        //        textList.Add(text);
        //    }
        //}
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 touchpadValue = touchpadAction.GetAxis(SteamVR_Input_Sources.Any);

        // Where the touch pad was triggered
        if (touchpadValue.x > 0.0f) // RIGHT
        {
            //text.fontSize += TEXT_SIZING_INCREMENTS * (touchpadValue.x * TEXT_SIZE_CHANGE_MODIFIER);
            changeObjectSize(textCollection, true);

        }
        else if (touchpadValue.x < -0.0f) // LEFT
        {
            //text.fontSize -= TEXT_SIZING_INCREMENTS * (-touchpadValue.x * TEXT_SIZE_CHANGE_MODIFIER);
            changeObjectSize(textCollection, false);

        }

        if (SteamVR_Actions._default.InteractUI.GetStateDown(SteamVR_Input_Sources.Any))
        {
            // Where the touch pad was triggered
            if (touchpadValue.y > TRIGGER_THRESHOLD) // UP
            {
                screen.localPosition = new Vector3(screen.localPosition.x, screen.localPosition.y, screen.localPosition.z + 1.0f);
            }
            else if (touchpadValue.y < -TRIGGER_THRESHOLD) // DOWN
            {
                screen.localPosition = new Vector3(screen.localPosition.x, screen.localPosition.y, screen.localPosition.z - 1.0f);
            }

        }

        //float triggerValue = squeezeAction.GetAxis(SteamVR_Input_Sources.Any);

        //if (triggerValue > 0.0f)
        //{

        //}


        /**
         * Set "screen" height to match the height of the user's head.
         * TODO Maybe position it  a bit lower than the user's head. Example 6 degrees down (Google I/0 17)
         */
        if (SteamVR_Actions._default.GrabPinch.GetStateDown(SteamVR_Input_Sources.Any))
        {

            //print(head.transform.localPosition);
            //screen.transform.localPosition = new Vector3(screen.transform.localPosition.x, head.transform.localPosition.y, screen.localPosition.z);
            if (currentVisibleObject == 0) textList[textList.Count - 1].gameObject.SetActive(false);

            textList[currentVisibleObject].gameObject.SetActive(false);
            textList[currentVisibleObject + 1].gameObject.SetActive(true);
            currentVisibleObject++;

            if (currentVisibleObject == textList.Count - 1) currentVisibleObject = 0;
        }

        /**
         * Change the text that is being displayed
         */
        if (Input.GetKeyDown(KeyCode.A))
        {
            if (currentTextShown == dataController.AllTextData.Length) currentTextShown = 0;
            text.text = dataController.AllTextData[currentTextShown++].Text;
        }

        // WIP-DEV - Cycle through game objects
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (currentVisibleObject == 0) textList[textList.Count - 1].gameObject.SetActive(false);

            textList[currentVisibleObject].gameObject.SetActive(false);
            textList[currentVisibleObject + 1].gameObject.SetActive(true);
            currentVisibleObject++;

            if (currentVisibleObject == textList.Count - 1) currentVisibleObject = 0;
        }
    }

    private void changeObjectSize(Transform obj, bool increase)
    {
        float increment = 0.01f;
        if (!increase) increment = -increment;

        Vector3 position = obj.localScale;
        position = new Vector3(position.x + increment, position.y + increment, position.z + increment);
        obj.transform.localScale = position;
    }
}
