using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using TMPro;
using System.Linq;

public class Controller : MonoBehaviour
{
    public bool DEBUG = true;

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
        flatScreenLineWidth,
        flatScreenMinimum,
        curvedScreenComfortable,
        curvedScreenLineWidth,
        curvedScreenMinimum
    };
    [SerializeField]
    private int currentExperimentStage = -1;

    // Stage - Curved text.
    private int currentVisibleObject = 0;
    private enum TextWidth { increase, decrease };
    private List<Transform> textMeshList;
    private readonly float INITIAL_RADIUS = 3.0f; // When generated in Blender.
    private readonly float INITIAL_CURVED_TEXT_HEIGHT = 0.1f; // The y-dimension one line of the Blender generated text.

    [SerializeField]
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
        // TODO: Rewrite control scheme to be easier to maintain.

        // Experiment stages where only the text scaling should be available.
        // Where the touch pad was triggered
        if (touchpadValue.x > TRIGGER_THRESHOLD_SMOOTH_SCALING) // RIGHT
        {
            if (currentScreenType == (int)ScreenType.flatScreen && currentExperimentStage != (int)ExperimentStage.flatScreenLineWidth) text.fontSize += TEXT_SIZING_INCREMENTS * (touchpadValue.x * TEXT_SIZE_CHANGE_MODIFIER);
            if (currentScreenType == (int)ScreenType.curvedScreen && currentExperimentStage != (int)ExperimentStage.curvedScreenLineWidth) ChangeObjectSize(textContainer, true);

        }
        else if (touchpadValue.x < -TRIGGER_THRESHOLD_SMOOTH_SCALING) // LEFT
        {
            if (currentScreenType == (int)ScreenType.flatScreen && currentExperimentStage != (int)ExperimentStage.flatScreenLineWidth) text.fontSize -= TEXT_SIZING_INCREMENTS * (-touchpadValue.x * TEXT_SIZE_CHANGE_MODIFIER);
            if (currentScreenType == (int)ScreenType.curvedScreen && currentExperimentStage != (int)ExperimentStage.curvedScreenLineWidth) ChangeObjectSize(textContainer, false);

        }
        

        // Experiment stages where only line width changes should be available.
        if (SteamVR_Actions._default.InteractUI.GetStateDown(SteamVR_Input_Sources.Any))
        {
            // Where the touch pad was triggered
            if (touchpadValue.y > TRIGGER_THRESHOLD) // UP
            {
                if (currentScreenType == (int)ScreenType.flatScreen && currentExperimentStage == (int)ExperimentStage.flatScreenLineWidth) flatScreen.localPosition = new Vector3(flatScreen.localPosition.x, flatScreen.localPosition.y, flatScreen.localPosition.z + 1.0f);
                if (currentScreenType == (int)ScreenType.curvedScreen && currentExperimentStage == (int)ExperimentStage.curvedScreenLineWidth) ChangeVisibleObjectInList(TextWidth.increase);
            }
            else if (touchpadValue.y < -TRIGGER_THRESHOLD) // DOWN
            {
                if (currentScreenType == (int)ScreenType.flatScreen && currentExperimentStage == (int)ExperimentStage.flatScreenLineWidth) flatScreen.localPosition = new Vector3(flatScreen.localPosition.x, flatScreen.localPosition.y, flatScreen.localPosition.z - 1.0f);
                if (currentScreenType == (int)ScreenType.curvedScreen && currentExperimentStage == (int)ExperimentStage.curvedScreenLineWidth) ChangeVisibleObjectInList(TextWidth.decrease);
            }

        }
        


        //float triggerValue = squeezeAction.GetAxis(SteamVR_Input_Sources.Any);

        //if (triggerValue > 0.0f)
        //{

        //}


        /**
         * Store data that is relevant to the currentExperimentStage
         */
        if (SteamVR_Actions._default.GrabPinch.GetStateDown(SteamVR_Input_Sources.Any) || Input.GetKeyDown(KeyCode.S))
        {

            if (currentExperimentStage == (int) ExperimentStage.flatScreenComfortable)
            {
                text.ForceMeshUpdate();

                participantData.participantId = participantId;
                participantData.flatScreenParticipantPosComfortable = head.transform.localPosition;
                participantData.flatScreenPosComfortable = flatScreen.transform.localPosition;
                participantData.flatScreenScaleComfortable = flatScreen.transform.localScale;
                participantData.flatScreenFontSizeComfortable = text.fontSize;
                participantData.flatScreenDistanceToScreenComfortable = participantData.flatScreenPosComfortable.z - participantData.flatScreenParticipantPosComfortable.z;
                participantData.currentTextShownComfortable = currentTextShown;
                participantData.flatScreenLineHeightComfortable = (float) System.Math.Round(text.textInfo.lineInfo[0].lineHeight, 4);
                participantData.flatScreenAngularSizeComfortable = CalculateAngularSize(participantData.flatScreenLineHeightComfortable, participantData.flatScreenDistanceToScreenComfortable);
                participantData.flatscreenDmmComfortable = CalculateDMM(participantData.flatScreenLineHeightComfortable, participantData.flatScreenDistanceToScreenComfortable);
                print("Storing data - Flat screen comfortable");
            }
            else if (currentExperimentStage == (int) ExperimentStage.flatScreenLineWidth)
            {
                text.ForceMeshUpdate();

                participantData.flatScreenParticipantPosLineWidth = head.transform.localPosition;
                participantData.flatScreenDistanceToScreenLineWidth = participantData.flatScreenPosComfortable.z - participantData.flatScreenParticipantPosLineWidth.z;
                participantData.flatScreenLineWidth = text.rectTransform.rect.width;
                participantData.flatScreenAngularSizeLineWidth = 2 * CalculateAngularSize((participantData.flatScreenLineWidth / 2), participantData.flatScreenDistanceToScreenLineWidth);
                participantData.flatScreenDMMLineWidth = CalculateDMM(participantData.flatScreenLineWidth, participantData.flatScreenDistanceToScreenLineWidth);
                print("Storing data - Flat screen line width");
            }
            else if (currentExperimentStage == (int) ExperimentStage.flatScreenMinimum)
            {
                text.ForceMeshUpdate();

                participantData.flatScreenParticipantPosMinimum = head.transform.localPosition;
                participantData.flatScreenPosMinimum = flatScreen.transform.localPosition;
                participantData.flatScreenScaleMinimum = flatScreen.transform.localScale;
                participantData.flatScreenFontSizeMinimum = text.fontSize;
                participantData.flatScreenDistanceToScreenMinimum = participantData.flatScreenPosMinimum.z - participantData.flatScreenParticipantPosMinimum.z;
                participantData.currentTextShownMinimum = currentTextShown;
                participantData.flatScreenLineHeightMinimum = (float)System.Math.Round(text.textInfo.lineInfo[0].lineHeight, 4);
                participantData.flatScreenAngularSizeMinimum = CalculateAngularSize(participantData.flatScreenLineHeightMinimum, participantData.flatScreenDistanceToScreenMinimum);
                participantData.flatscreenDmmMinimum = CalculateDMM(participantData.flatScreenLineHeightMinimum, participantData.flatScreenDistanceToScreenMinimum);
                print("Storing data - Flat screen minimum");
            }
            else if (currentExperimentStage == (int) ExperimentStage.curvedScreenComfortable)
            {
                participantData.curvedScreenParticipantPosComfortable = head.transform.localPosition;
                participantData.curvedScreenPosComfortable = textContainer.transform.localPosition;
                participantData.curvedScreenScaleComfortable = textContainer.transform.localScale;
                participantData.curvedScreenDistanceToScreenComfortable = (INITIAL_RADIUS * participantData.curvedScreenScaleComfortable.z + participantData.curvedScreenPosComfortable.z) - participantData.curvedScreenParticipantPosComfortable.z;
                participantData.currentlyVisibleObjectComfortable = currentVisibleObject;
                participantData.curvedScreenLineHeightComfortable = INITIAL_CURVED_TEXT_HEIGHT * participantData.curvedScreenScaleComfortable.y;
                participantData.curvedScreenAngularSizeComfortable = CalculateAngularSize(participantData.curvedScreenLineHeightComfortable, participantData.curvedScreenDistanceToScreenComfortable);
                participantData.curvedscreenDmmComfortable = CalculateDMM(participantData.curvedScreenLineHeightComfortable, participantData.curvedScreenDistanceToScreenComfortable);
                print("Storing data - Curved screen comfortable");
            }
            else if (currentExperimentStage == (int)ExperimentStage.curvedScreenLineWidth)
            {
                participantData.curvedScreenParticipantPosLineWidth = head.transform.localPosition;
                participantData.curvedScreenDistanceToScreenLineWidth = participantData.curvedScreenPosComfortable.z - participantData.curvedScreenParticipantPosLineWidth.z;
                participantData.curvedScreenLineWidth = text.rectTransform.rect.width;
                participantData.curvedScreenAngularSizeLineWidth = 2 * CalculateAngularSize((participantData.curvedScreenLineWidth / 2), participantData.curvedScreenDistanceToScreenLineWidth);
                participantData.curvedScreenDMMLineWidth = CalculateDMM(participantData.curvedScreenLineWidth, participantData.curvedScreenDistanceToScreenLineWidth);
                print("Storing data - Curved screen line width");
            }
            else if (currentExperimentStage == (int) ExperimentStage.curvedScreenMinimum)
            {
                participantData.curvedScreenParticipantPosMinimum = head.transform.localPosition;
                participantData.curvedScreenPosMinimum = textContainer.transform.localPosition;
                participantData.curvedScreenScaleMinimum = textContainer.transform.localScale;
                participantData.curvedScreenDistanceToScreenMinimum = (INITIAL_RADIUS * participantData.curvedScreenScaleMinimum.z + participantData.curvedScreenPosMinimum.z) - participantData.curvedScreenParticipantPosMinimum.z;
                participantData.currentlyVisibleObjectMinimum = currentVisibleObject;
                participantData.curvedScreenLineHeightMinimum = INITIAL_CURVED_TEXT_HEIGHT * participantData.curvedScreenScaleMinimum.y;
                participantData.curvedScreenAngularSizeMinimum = CalculateAngularSize(participantData.curvedScreenLineHeightMinimum, participantData.curvedScreenDistanceToScreenMinimum);
                participantData.curvedscreenDmmMinimum = CalculateDMM(participantData.curvedScreenLineHeightMinimum, participantData.curvedScreenDistanceToScreenMinimum);
                print("Storing data - Curved screen minimum");
            }
        }

        /*
         * Cancel experiment and write current data to file.
         */
        if (Input.GetKeyDown(KeyCode.Q))
        {
            fh.WriteToFile(participantData);
        }

        /*
         * Print values for debugging.
         */
        if (Input.GetKeyDown(KeyCode.P))
        {
            Mesh mesh = textMeshList[currentVisibleObject].GetComponent<MeshFilter>().mesh; // Bounding box, but not actual text size.
            print(mesh.bounds.size.x);
            print(mesh.bounds.size.y);
            print(mesh.bounds.size.z);

            // At the exported size from Blender the first line is 0.069 high
            // The other lines are 0.1 units high, which is what we divide by to get the lineCount
            print(Mathf.Round(mesh.bounds.size.y / 0.1f));
        }


        /*
         * Increment currentExperimentStage and make sure the screentype changed accordingly.
         */
        if (Input.GetKeyDown(KeyCode.I) || (SteamVR_Actions._default.GrabGrip.GetStateDown(SteamVR_Input_Sources.Any) && DEBUG))
        {
            flatScreen.localPosition = new Vector3(0f, head.transform.localPosition.y, 3.0f);

            currentExperimentStage++;
            if (currentExperimentStage == (int) ExperimentStage.flatScreenComfortable) ChangeVisibleScreen(ScreenType.flatScreen);
            if (currentExperimentStage == (int) ExperimentStage.curvedScreenComfortable) ChangeVisibleScreen(ScreenType.curvedScreen);
            print(string.Format("Changing to experiment stage: {0}", currentExperimentStage));
        }        
        /*
         * Increment currentExperimentStage and make sure the screentype changed accordingly.
         */
        if (Input.GetKeyDown(KeyCode.O))
        {
            flatScreen.localPosition = new Vector3(0f, head.transform.localPosition.y, 3.0f);

            currentExperimentStage--;
            if (currentExperimentStage == (int) ExperimentStage.flatScreenComfortable) ChangeVisibleScreen(ScreenType.flatScreen);
            if (currentExperimentStage == (int) ExperimentStage.curvedScreenComfortable) ChangeVisibleScreen(ScreenType.curvedScreen);
            print(string.Format("Changing to experiment stage: {0}", currentExperimentStage));
        }

        /**
         * Change the text that is being displayed
         */
        if (Input.GetKeyDown(KeyCode.A))
        {
            currentTextShown++;
            if (currentTextShown == dataController.AllTextData.Length) currentTextShown = 0;
            text.text = dataController.AllTextData[currentTextShown].Text;
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
        position = new Vector3(position.x, position.y, position.z - increment * 3);

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

    /*
     * Returns angular size in degrees
     */
    private float CalculateAngularSize(float opposite, float adjacent)
    {
        return Mathf.Atan(opposite / adjacent) * Mathf.Rad2Deg;
    }

    /*
     * Returns size in distance independent milimeters (i.e x mm at y meters away)
     * Size: In meters
     * Distance: In meters
     */
    private float CalculateDMM(float size, float distance)
    {
        return size / distance * 1000; // M to mm
    }
}
