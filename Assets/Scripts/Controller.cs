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
    public Transform flatScreen;
    public Camera head;
    public Transform textContainer;
    public Transform curvedTextMeshColletion;

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
    private List<Transform> textMeshList;

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
        textMeshList = new List<Transform>();
        foreach (Transform text in curvedTextMeshColletion)
        {
            textMeshList.Add(text);
        }
        currentVisibleObject = (int) textMeshList.Count / 2;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 touchpadValue = touchpadAction.GetAxis(SteamVR_Input_Sources.Any);

        // Where the touch pad was triggered
        if (touchpadValue.x > TRIGGER_THRESHOLD_SMOOTH_SCALING) // RIGHT
        {
            if (currentScreenType == (int)ScreenType.flatScreen)
            {
                text.fontSize += TEXT_SIZING_INCREMENTS * (touchpadValue.x * TEXT_SIZE_CHANGE_MODIFIER);
                ChangeObjectSize(flatScreen.transform, true, ScreenType.flatScreen);
                ChangeObjectSize(text.rectTransform, true);
            }
            if (currentScreenType == (int) ScreenType.curvedScreen) ChangeObjectSize(textContainer, true, ScreenType.curvedScreen);

        }
        else if (touchpadValue.x < -TRIGGER_THRESHOLD_SMOOTH_SCALING) // LEFT
        {
            if (currentScreenType == (int)ScreenType.flatScreen)
            {
                text.fontSize -= TEXT_SIZING_INCREMENTS * (-touchpadValue.x * TEXT_SIZE_CHANGE_MODIFIER);
                ChangeObjectSize(flatScreen.transform, false, ScreenType.flatScreen);
                ChangeObjectSize(text.rectTransform, false);
            }
            if (currentScreenType == (int) ScreenType.curvedScreen) ChangeObjectSize(textContainer, false, ScreenType.curvedScreen);

        }

        if (SteamVR_Actions._default.InteractUI.GetStateDown(SteamVR_Input_Sources.Any))
        {
            // Where the touch pad was triggered
            if (touchpadValue.y > TRIGGER_THRESHOLD) // UP
            {
                if (currentScreenType == (int) ScreenType.flatScreen) ChangeObjectSize(flatScreen, text.rectTransform, true);
                if (currentScreenType == (int) ScreenType.curvedScreen) ChangeVisibleObjectInList(TextWidth.increase);
            }
            else if (touchpadValue.y < -TRIGGER_THRESHOLD) // DOWN
            {
                if (currentScreenType == (int) ScreenType.flatScreen) ChangeObjectSize(flatScreen, text.rectTransform, false);
                if (currentScreenType == (int) ScreenType.curvedScreen) ChangeVisibleObjectInList(TextWidth.decrease);
            }

        }

        //float triggerValue = squeezeAction.GetAxis(SteamVR_Input_Sources.Any);

        //if (triggerValue > 0.0f)
        //{

        //}


        /**
         * Set "flatScreen" height to match the height of the user's head.
         * TODO Maybe position it  a bit lower than the user's head. Example 6 degrees down (Google I/0 17)
         */
        if (SteamVR_Actions._default.GrabPinch.GetStateDown(SteamVR_Input_Sources.Any) || Input.GetKeyDown(KeyCode.S))
        {
            participantData.participantId = participantId;

            if (currentExperimentStage == (int) ExperimentStage.flatScreenComfortable)
            {
                participantData.flatScreenParticipantPosComfortable = head.transform.localPosition;
                participantData.flatScreenPosComfortable = flatScreen.transform.localPosition;
                participantData.flatScreenScaleComfortable = flatScreen.transform.localScale;
                participantData.flatScreenTextSizeComfortable = text.fontSize;
                participantData.flatScreenDistanceToScreenComfortable = participantData.flatScreenPosComfortable.z - Mathf.Abs(participantData.flatScreenParticipantPosComfortable.z);
                participantData.currentTextShownComfortable = currentTextShown;
            }
            else if (currentExperimentStage == (int) ExperimentStage.flatScreenMinimum)
            {
                participantData.flatScreenParticipantPosMinimum = head.transform.localPosition;
                participantData.flatScreenPosMinimum = flatScreen.transform.localPosition;
                participantData.flatScreenScaleMinimum = flatScreen.transform.localScale;
                participantData.flatScreenTextSizeMinimum = text.fontSize;
                participantData.flatScreenDistanceToScreenMinimum = participantData.flatScreenPosMinimum.z - Mathf.Abs(participantData.flatScreenParticipantPosMinimum.z);
                participantData.currentTextShownMinimum = currentTextShown;
            }
            else if (currentExperimentStage == (int) ExperimentStage.curvedScreenComfortable)
            {
                participantData.curvedScreenParticipantPosComfortable = head.transform.localPosition;
                participantData.curvedScreenPosComfortable = curvedTextMeshColletion.transform.localPosition;
                participantData.curvedScreenScaleComfortable = curvedTextMeshColletion.transform.localScale;
                participantData.curvedScreenDistanceToScreenComfortable = participantData.curvedScreenPosComfortable.z - Mathf.Abs(participantData.curvedScreenParticipantPosComfortable.z);
                participantData.currentlyVisibleObjectComfortable = currentVisibleObject;
            }
            else if (currentExperimentStage == (int) ExperimentStage.curvedScreenMinimum)
            {
                participantData.curvedScreenParticipantPosMinimum = head.transform.localPosition;
                participantData.curvedScreenPosMinimum = curvedTextMeshColletion.transform.localPosition;
                participantData.curvedScreenScaleMinimum = curvedTextMeshColletion.transform.localScale;
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
         * Increment currentExperimentStage and make sure the screentype changed accordingly.
         */
        if (Input.GetKeyDown(KeyCode.I))
        {
            currentExperimentStage++;
            if (currentExperimentStage == (int) ExperimentStage.flatScreenComfortable) ChangeVisibleScreen(ScreenType.flatScreen);
            if (currentExperimentStage == (int) ExperimentStage.curvedScreenComfortable) ChangeVisibleScreen(ScreenType.curvedScreen);
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
    }

    /*
     * Change the size of the object in increments and keep it in the same place on the z-axis.
     */
    private void ChangeObjectSize(Transform obj, bool increase, ScreenType screenType)
    {
        float increment = (increase) ? 0.001f : -0.001f;

        Vector3 scale = obj.localScale;
        Vector3 position = obj.localPosition;

        scale = new Vector3(scale.x + increment, scale.y + increment, scale.z + increment);
        obj.transform.localScale = scale;


        if (screenType == ScreenType.curvedScreen)
        { 
            position = new Vector3(position.x, position.y, position.z - increment * 2);
            obj.transform.localPosition = position;
        }
    }

    private void ChangeObjectSize(RectTransform obj, bool increase)
    {
        float increment = (increase) ? 0.001f : -0.001f;
        obj.sizeDelta = new Vector2(obj.rect.width + increment, obj.rect.height + increment);
    }

    /*
     * Change the size of both the obj and the rectObj accordingly.
     */
    private void ChangeObjectSize(Transform screenObj, RectTransform textObj, bool increase)
    {
        float increment = 0.1f;
        float sizeModifier = (increase) ? increment : -increment;

        textObj.sizeDelta = new Vector2(text.rectTransform.rect.width + sizeModifier, text.rectTransform.rect.height);
        screenObj.localScale = new Vector3(textObj.rect.width + increment, textObj.rect.height + increment, screenObj.localScale.z);
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
                if (currentVisibleObject < textMeshList.Count - 1)
                {
                    textMeshList[currentVisibleObject++].gameObject.SetActive(false);
                    textMeshList[currentVisibleObject].gameObject.SetActive(true);
                }
                break;
            case TextWidth.decrease:
                if (currentVisibleObject > 0)
                {
                    textMeshList[currentVisibleObject--].gameObject.SetActive(false);
                    textMeshList[currentVisibleObject].gameObject.SetActive(true);
                }
                break;
            default:
                Debug.LogError("Invalid option provided.");
                break;
        }
    }

    /*
     * Decide which flatScreen should be displayed.
     */
    private void ChangeVisibleScreen(ScreenType screenType)
    { 
        bool displayFlatScreen = screenType == ScreenType.flatScreen ? true : false;

        currentScreenType = (int) screenType;

        flatScreen.gameObject.SetActive(displayFlatScreen);
        textContainer.gameObject.SetActive(!displayFlatScreen);
    }
}
