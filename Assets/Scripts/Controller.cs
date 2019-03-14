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
    public GameObject screen;
    public Camera head;
    private double MINIMUM_FONT_SIZE = 1.0f;
    private double MAXIMUM_FONT_SIZE = 7.0f;
    private double TRIGGER_THRESHOLD = 0.0f;
    private float TEXT_SIZING_INCREMENTS = 0.0001f;
    private float TEXT_SIZE_CHANGE_MODIFIER = 5;
    private readonly float WORLD_CENTER = 0.0f;
    private readonly float ANGLE_COMFORTABLE_DOWN = 6.0f;

    private int currentTextShown = 0;

    // Start is called before the first frame update
    void Start()
    {
        dataController = ScriptableObject.CreateInstance<DataController>();
        print(dataController.AllTextData.Length);
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 touchpadValue = touchpadAction.GetAxis(SteamVR_Input_Sources.Any);

        // Where the touch pad was triggered
        if (touchpadValue.x > 0.0f) // RIGHT
        {
            text.fontSize += TEXT_SIZING_INCREMENTS * (touchpadValue.x * TEXT_SIZE_CHANGE_MODIFIER);

        }
        else if (touchpadValue.x < -0.0f) // LEFT
        {
            text.fontSize -= TEXT_SIZING_INCREMENTS * (-touchpadValue.x * TEXT_SIZE_CHANGE_MODIFIER);

        }

        if (SteamVR_Actions._default.InteractUI.GetStateDown(SteamVR_Input_Sources.Any))
        {
            // Where the touch pad was triggered
            if (touchpadValue.y > TRIGGER_THRESHOLD) // UP
            {
                screen.transform.localPosition = new Vector3(screen.transform.localPosition.x, screen.transform.localPosition.y, screen.transform.localPosition.z + 1.0f);
            }
            else if (touchpadValue.y < -TRIGGER_THRESHOLD) // DOWN
            {
                screen.transform.localPosition = new Vector3(screen.transform.localPosition.x, screen.transform.localPosition.y, screen.transform.localPosition.z - 1.0f);
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
            SetScreenPosition(screen.transform);
        }

        /**
         * Change the text that is being displayed
         */
        if (Input.GetKeyDown(KeyCode.A))
        {
            if (currentTextShown == dataController.AllTextData.Length) currentTextShown = 0;
            text.text = dataController.AllTextData[currentTextShown++].Text;
        }

        /*
         * Move screen down 6 degrees from the horizon line
         */
        if (Input.GetKeyDown(KeyCode.H))
        {
            SetScreenPosition(screen.transform);
        }

        /*
         * Start experiment
         */
        if (Input.GetKeyDown(KeyCode.S))
        {
            screen.SetActive(true);
            Vector3 StartPos = screen.transform.localPosition;
            screen.transform.localPosition = new Vector3(StartPos.x, head.transform.localPosition.y, StartPos.z);
        }
    }

    /**
     * Get amount of distance units between the object and the center (0, 0, 0)
     **/
    private float GetZDistanceFromZero(Transform obj)
    {
        return System.Math.Abs(obj.localPosition.z - WORLD_CENTER);
    }

    /*
     * Sets the screens position to be ANGLE_COMFORTABLE_DOWN degrees down from the y
     * position of the head camera.
     */
    private void SetScreenPosition(Transform screen)
    {
        //float distanceToScreen = GetZDistanceFromZero(screen);
        //float offset = distanceToScreen / Mathf.Cos(ANGLE_COMFORTABLE_DOWN * Mathf.Deg2Rad);
        //float CalculatedYPos = head.transform.localPosition.y - offset;

        //screen.transform.localPosition = new Vector3(screen.transform.localPosition.x, CalculatedYPos, screen.localPosition.z);
    }
}
