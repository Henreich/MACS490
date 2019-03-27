using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using TMPro;

public class Controller : MonoBehaviour
{
    public SteamVR_Action_Single squeezeAction;
    public SteamVR_Action_Vector2 touchpadAction;

    public TextMeshPro text;
    public Transform screen;
    public Camera head;
    public Transform textContainer;
    public Transform textCollection;

    private DataController dataController;
    //private readonly double MINIMUM_FONT_SIZE = 1.0f;
    //private readonly double MAXIMUM_FONT_SIZE = 7.0f;
    private readonly double TRIGGER_THRESHOLD = 0.0f;
    private readonly double TRIGGER_THRESHOLD_SMOOTH_SCALING = 0.3f;
    private readonly float TEXT_SIZING_INCREMENTS = 0.0001f;
    private readonly float TEXT_SIZE_CHANGE_MODIFIER = 5;

    private int currentTextShown = 0;

    private enum ScreenType { flatScreen, curvedScreen };
    private int currentScreenType = -1;
    private enum ExperimentStage {
        flatScreenComfortable,
        flatScreenMinimum,
        curvedScreenComfortable,
        curvedScreenMinimum
    };
    private int currentExperimentStage = -1;

    // Stage - Curved text.
    private int currentVisibleObject = 0;
    private enum TextWidth { increase, decrease };
    private List<Transform> textList;

    private int participantId = 0;
    private FileHandler fh;
    private ExperimentData participantData;

    // Start is called before the first frame update
    void Start()
    {
        dataController = ScriptableObject.CreateInstance<DataController>();
        participantData = new ExperimentData();
        fh = ScriptableObject.CreateInstance<FileHandler>();

        /*
         * Set up the list of text object transforms
         */
        textList = new List<Transform>();
        foreach (Transform text in textCollection)
        {
            textList.Add(text);
        }
        currentVisibleObject = (int) textList.Count / 2;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 touchpadValue = touchpadAction.GetAxis(SteamVR_Input_Sources.Any);

        // Where the touch pad was triggered
        if (touchpadValue.x > TRIGGER_THRESHOLD_SMOOTH_SCALING) // RIGHT
        {
            if (currentScreenType == (int) ScreenType.flatScreen)   text.fontSize += TEXT_SIZING_INCREMENTS * (touchpadValue.x * TEXT_SIZE_CHANGE_MODIFIER);
            if (currentScreenType == (int) ScreenType.curvedScreen) ChangeObjectSize(textContainer, true);

        }
        else if (touchpadValue.x < -TRIGGER_THRESHOLD_SMOOTH_SCALING) // LEFT
        {
            if (currentScreenType == (int) ScreenType.flatScreen) text.fontSize -= TEXT_SIZING_INCREMENTS * (-touchpadValue.x * TEXT_SIZE_CHANGE_MODIFIER);
            if (currentScreenType == (int) ScreenType.curvedScreen) ChangeObjectSize(textContainer, false);

        }

        if (SteamVR_Actions._default.InteractUI.GetStateDown(SteamVR_Input_Sources.Any))
        {
            // Where the touch pad was triggered
            if (touchpadValue.y > TRIGGER_THRESHOLD) // UP
            {
                if (currentScreenType == (int) ScreenType.flatScreen) screen.localPosition = new Vector3(screen.localPosition.x, screen.localPosition.y, screen.localPosition.z + 1.0f);
                if (currentScreenType == (int) ScreenType.curvedScreen) ChangeVisibleObjectInList(TextWidth.increase);
            }
            else if (touchpadValue.y < -TRIGGER_THRESHOLD) // DOWN
            {
                if (currentScreenType == (int) ScreenType.flatScreen) screen.localPosition = new Vector3(screen.localPosition.x, screen.localPosition.y, screen.localPosition.z - 1.0f);
                if (currentScreenType == (int) ScreenType.curvedScreen) ChangeVisibleObjectInList(TextWidth.decrease);
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
        if (SteamVR_Actions._default.GrabPinch.GetStateDown(SteamVR_Input_Sources.Any) || Input.GetKeyDown(KeyCode.S))
        {
            participantData.participantId = participantId;

            if (currentExperimentStage == (int) ExperimentStage.flatScreenComfortable)
            {
                participantData.flatScreenParticipantPosComfortable = head.transform.localPosition;
                participantData.flatScreenPosComfortable = screen.transform.localPosition;
                participantData.flatScreenScaleComfortable = screen.transform.localScale;
                participantData.flatScreenTextSizeComfortable = text.fontSize;
                participantData.flatScreenDistanceToScreenComfortable = participantData.flatScreenPosComfortable.z - Mathf.Abs(participantData.flatScreenParticipantPosComfortable.z);
                participantData.currentTextShownComfortable = currentTextShown;
            }
            else if (currentExperimentStage == (int) ExperimentStage.flatScreenMinimum)
            {
                participantData.flatScreenParticipantPosMinimum = head.transform.localPosition;
                participantData.flatScreenPosMinimum = screen.transform.localPosition;
                participantData.flatScreenScaleMinimum = screen.transform.localScale;
                participantData.flatScreenTextSizeMinimum = text.fontSize;
                participantData.flatScreenDistanceToScreenMinimum = participantData.flatScreenPosMinimum.z - Mathf.Abs(participantData.flatScreenParticipantPosMinimum.z);
                participantData.currentTextShownMinimum = currentTextShown;
            }
            else if (currentExperimentStage == (int) ExperimentStage.curvedScreenComfortable)
            {
                participantData.curvedScreenParticipantPosComfortable = head.transform.localPosition;
                participantData.curvedScreenPosComfortable = textCollection.transform.localPosition;
                participantData.curvedScreenScaleComfortable = textCollection.transform.localScale;
                participantData.curvedScreenDistanceToScreenComfortable = participantData.curvedScreenPosComfortable.z - Mathf.Abs(participantData.curvedScreenParticipantPosComfortable.z);
                participantData.currentlyVisibleObjectComfortable = currentVisibleObject;
            }
            else if (currentExperimentStage == (int) ExperimentStage.curvedScreenMinimum)
            {
                participantData.curvedScreenParticipantPosMinimum = head.transform.localPosition;
                participantData.curvedScreenPosMinimum = textCollection.transform.localPosition;
                participantData.curvedScreenScaleMinimum = textCollection.transform.localScale;
                participantData.curvedScreenDistanceToScreenMinimum = participantData.curvedScreenPosMinimum.z - Mathf.Abs(participantData.curvedScreenParticipantPosMinimum.z);
                participantData.currentlyVisibleObjectMinimum = currentVisibleObject;
            }
        }


        /*
         * Cancel experiment and write current data to file.
         */
        if (Input.GetKeyDown(KeyCode.C))
        {
            fh.WriteToFile(participantData);
        }

        /*
         * Print values for debugging.
         */
        if (Input.GetKeyDown(KeyCode.P))
        {
            print(string.Format("ExperimentStage: {0}", currentExperimentStage));
            print(string.Format("ScreenType: {0}", currentScreenType));
        }
        /*
         * Increment currentExperimentStage
         */
        if (Input.GetKeyDown(KeyCode.I))
        {
            currentExperimentStage++;
            print(string.Format("Changing to experiment stage: {0}", currentExperimentStage));
        }

        /**
         * Change the text that is being displayed
         */
        if (Input.GetKeyDown(KeyCode.A))
        {
            if (currentTextShown == dataController.AllTextData.Length) currentTextShown = 0;
            text.text = dataController.AllTextData[currentTextShown++].Text;
        }
        // Change between the screentype, either reading from a flat or a curved screen.
        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            ChangeVisibleScreen(ScreenType.flatScreen);
            currentExperimentStage = 0;
            currentScreenType = (int) ScreenType.flatScreen;
        }
        if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            ChangeVisibleScreen(ScreenType.curvedScreen);
            currentScreenType = (int) ScreenType.curvedScreen;
        }
    }

    /*
     * Change the size of the object in increments and keep it in the same place on the z-axis.
     */
    private void ChangeObjectSize(Transform obj, bool increase)
    {
        float increment = (increase) ? 0.001f : -0.001f;

        Vector3 scale = obj.localScale;
        Vector3 position = obj.localPosition;

        scale = new Vector3(scale.x + increment, scale.y + increment, scale.z + increment);
        position = new Vector3(position.x, position.y, position.z - increment * 2);

        obj.transform.localScale = scale;
        obj.transform.localPosition = position;
    }

    /*
     * Decides which object in the list of objects should be visible when going up
     * or down in the list. Only one object should be visible at a time.
     */
    private void ChangeVisibleObjectInList(TextWidth action)
    {
        switch (action)
        {
            case TextWidth.increase:
                if (currentVisibleObject < textList.Count - 1)
                {
                    textList[currentVisibleObject++].gameObject.SetActive(false);
                    textList[currentVisibleObject].gameObject.SetActive(true);
                }
                break;
            case TextWidth.decrease:
                if (currentVisibleObject > 0)
                {
                    textList[currentVisibleObject--].gameObject.SetActive(false);
                    textList[currentVisibleObject].gameObject.SetActive(true);
                }
                break;
            default:
                Debug.LogError("Invalid option provided.");
                break;
        }
    }

    /*
     * Decide which screen should be displayed.
     */
    private void ChangeVisibleScreen(ScreenType screenType)
    { 
        bool displayFlatScreen = screenType == ScreenType.flatScreen ? true : false;

        screen.gameObject.SetActive(displayFlatScreen);
        textContainer.gameObject.SetActive(!displayFlatScreen);
    }
}
