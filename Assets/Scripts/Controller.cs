using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using TMPro;
using System.Linq;

public class Controller : MonoBehaviour
{
    public SteamVR_Action_Single squeezeAction;
    public SteamVR_Action_Vector2 touchpadAction;

    public TextMeshPro text;
    public Transform flatScreen;
    public Camera head;
    public Transform textContainer;
    public Transform curvedTextMeshColletion;

    public TMP_CharacterInfo characterInfo;

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
                if (currentScreenType == (int) ScreenType.flatScreen) flatScreen.localPosition = new Vector3(flatScreen.localPosition.x, flatScreen.localPosition.y, flatScreen.localPosition.z + 1.0f);
                if (currentScreenType == (int) ScreenType.curvedScreen) ChangeVisibleObjectInList(TextWidth.increase);
            }
            else if (touchpadValue.y < -TRIGGER_THRESHOLD) // DOWN
            {
                if (currentScreenType == (int) ScreenType.flatScreen) flatScreen.localPosition = new Vector3(flatScreen.localPosition.x, flatScreen.localPosition.y, flatScreen.localPosition.z - 1.0f);
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
            text.ForceMeshUpdate();
            // Identical.
            //print(text.textInfo.lineInfo[0].baseline);

            //float total = 0;
            //for (int i = 0; i < text.textInfo.lineCount; i++)
            //{
            //    print(string.Format("{0}: {1}", i, text.textInfo.lineInfo[i].lineHeight));
            //    total += text.textInfo.lineInfo[i].lineHeight;
            //    fh.WriteToFile(text.textInfo.lineInfo[i].lineHeight);
            //}
            //print(total / text.textInfo.lineCount);
            //print(text.GetRenderedValues().x / text.textInfo.lineCount);


            // Average lineHeight of visible characters
            float averageLineHeight = text.GetRenderedValues(true).x / text.textInfo.lineCount;
            print(averageLineHeight);
            //float lineHeightStandardDeviation = averageLineHeight;

            //print(total); //1.035

            //print(text.renderedWidth + " " + text.renderedHeight);                        // 1.000119, 1.033352
            //print(text.GetRenderedValues(true).x + " " + text.GetRenderedValues(true).y); // 1.0001, 1.033352
            //print(total - text.renderedWidth);                                            // 0.03488064
            //print(total - text.GetRenderedValues(true).x);                                // 0.03489959


            //string infoToFile = "";
            //for (int i = 0; i < text.text.Length; i++)
            //{
            //    TMP_CharacterInfo characterInfo = text.GetTextInfo(text.text).characterInfo[i];


            //    print(string.Format("{0} - Base line: {1}, lineNumber: {2}, Scale: {3}, Point size: {4}, Vertex index: {5}",
            //         characterInfo.character, characterInfo.baseLine, characterInfo.lineNumber, characterInfo.scale, characterInfo.pointSize, characterInfo.vertexIndex
            //    ));
            //    infoToFile += string.Format("{0} - Base line: {1}, lineNumber: {2}, Scale: {3}, Point size: {4}, Vertex index: {5}",
            //         characterInfo.character, characterInfo.baseLine, characterInfo.lineNumber, characterInfo.scale, characterInfo.pointSize, characterInfo.vertexIndex);
            //    infoToFile += "\n";

            //}
            //fh.WriteToFile(infoToFile);
            //print(text.bounds);

            //print("word count:" + text.GetTextInfo(text.text).wordCount);
            //print("lineHeight:" + text.GetTextInfo(text.text).lineInfo[0].lineHeight);
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
