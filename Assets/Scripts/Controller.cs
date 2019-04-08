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
    public Transform screen;
    public Camera head;
    public Transform textContainer;
    public Transform curvedTextMeshColletion;
    public Transform curvedScreensCollection;

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
    private List<Transform> curvedScreenList;
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

        curvedScreenList = new List<Transform>();
        foreach (Transform curvedScreen in curvedScreensCollection)
        {
            curvedScreenList.Add(curvedScreen);
        }

        
        currentVisibleObject = (int) textMeshList.Count / 2;
        textMeshList[currentVisibleObject].gameObject.SetActive(true);
        ChangeVisibleCurvedScreen();

        Debug.LogWarning("Remember to update the participantId!");
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
            if (currentScreenType == (int)ScreenType.flatScreen && currentExperimentStage != (int)ExperimentStage.flatScreenLineWidth)
            {
                text.fontSize += TEXT_SIZING_INCREMENTS * (touchpadValue.x * TEXT_SIZE_CHANGE_MODIFIER);
                //ChangeObjectSize(flatScreen.transform, true, ScreenType.flatScreen);
                //ChangeObjectSize(text.rectTransform, true);
            }
            if (currentScreenType == (int)ScreenType.curvedScreen && currentExperimentStage != (int)ExperimentStage.curvedScreenLineWidth) ChangeObjectSize(textContainer, true, ScreenType.curvedScreen);

        }
        else if (touchpadValue.x < -TRIGGER_THRESHOLD_SMOOTH_SCALING) // LEFT
        {
            if (currentScreenType == (int)ScreenType.flatScreen && currentExperimentStage != (int)ExperimentStage.flatScreenLineWidth)
            {
                text.fontSize -= TEXT_SIZING_INCREMENTS * (-touchpadValue.x * TEXT_SIZE_CHANGE_MODIFIER);
                //ChangeObjectSize(flatScreen.transform, false, ScreenType.flatScreen);
                //ChangeObjectSize(text.rectTransform, false);
            }
            if (currentScreenType == (int)ScreenType.curvedScreen && currentExperimentStage != (int)ExperimentStage.curvedScreenLineWidth) ChangeObjectSize(textContainer, false, ScreenType.curvedScreen);
        }
        

        // Experiment stages where only line width changes should be available.
        if (SteamVR_Actions._default.InteractUI.GetStateDown(SteamVR_Input_Sources.Any))
        {
            // Where the touch pad was triggered
            if (touchpadValue.y > TRIGGER_THRESHOLD) // UP
            {
                if (currentScreenType == (int)ScreenType.flatScreen && currentExperimentStage == (int)ExperimentStage.flatScreenLineWidth) ChangeObjectSize(screen, text.rectTransform, true);
                if (currentScreenType == (int)ScreenType.curvedScreen && currentExperimentStage == (int)ExperimentStage.curvedScreenLineWidth) ChangeVisibleObjectInList(TextWidth.increase);
            }
            else if (touchpadValue.y < -TRIGGER_THRESHOLD) // DOWN
            {
                if (currentScreenType == (int)ScreenType.flatScreen && currentExperimentStage == (int)ExperimentStage.flatScreenLineWidth) ChangeObjectSize(screen, text.rectTransform, false);
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
                participantData.curvedScreenLineWidthComfortable = (currentVisibleObject * 0.2f) + 0.8f;
                participantData.curvedScreenLineHeightComfortable = INITIAL_CURVED_TEXT_HEIGHT * participantData.curvedScreenScaleComfortable.y;
                participantData.curvedScreenAngularSizeComfortable = CalculateAngularSize(participantData.curvedScreenLineHeightComfortable, participantData.curvedScreenDistanceToScreenComfortable);
                participantData.curvedscreenDmmComfortable = CalculateDMM(participantData.curvedScreenLineHeightComfortable, participantData.curvedScreenDistanceToScreenComfortable);
                print("Storing data - Curved screen comfortable");
            }
            else if (currentExperimentStage == (int)ExperimentStage.curvedScreenLineWidth)
            {
                participantData.curvedScreenParticipantPosLineWidth = head.transform.localPosition;
                participantData.curvedScreenDistanceToScreenLineWidth = (INITIAL_RADIUS * participantData.curvedScreenScaleComfortable.z + participantData.curvedScreenPosComfortable.z) - participantData.curvedScreenParticipantPosLineWidth.z;
                participantData.curvedScreenWidth = textMeshList[currentVisibleObject].GetComponent<MeshFilter>().mesh.bounds.extents.x;
                participantData.curvedScreenAngularSizeLineWidth = 2 * CalculateAngularSize((participantData.curvedScreenWidth / 2), participantData.curvedScreenDistanceToScreenLineWidth);
                participantData.curvedScreenDMMLineWidth = CalculateDMM(participantData.curvedScreenWidth, participantData.curvedScreenDistanceToScreenLineWidth);
                print("Storing data - Curved screen line width");
            }
            else if (currentExperimentStage == (int) ExperimentStage.curvedScreenMinimum)
            {
                participantData.curvedScreenParticipantPosMinimum = head.transform.localPosition;
                participantData.curvedScreenPosMinimum = textContainer.transform.localPosition;
                participantData.curvedScreenScaleMinimum = textContainer.transform.localScale;
                participantData.curvedScreenDistanceToScreenMinimum = (INITIAL_RADIUS * participantData.curvedScreenScaleMinimum.z + participantData.curvedScreenPosMinimum.z) - participantData.curvedScreenParticipantPosMinimum.z;
                participantData.curvedScreenLineWidthMinimum = (currentVisibleObject * 0.2f) + 0.8f;
                participantData.curvedScreenLineHeightMinimum = INITIAL_CURVED_TEXT_HEIGHT * participantData.curvedScreenScaleMinimum.y;
                participantData.curvedScreenAngularSizeMinimum = CalculateAngularSize(participantData.curvedScreenLineHeightMinimum, participantData.curvedScreenDistanceToScreenMinimum);
                participantData.curvedscreenDmmMinimum = CalculateDMM(participantData.curvedScreenLineHeightMinimum, participantData.curvedScreenDistanceToScreenMinimum);
                print("Storing data - Curved screen minimum");
                Debug.LogWarning("Remember to save data! Q - Quit and save.");
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
            //print(mesh.bounds.size.x);
            //print(mesh.bounds.size.y);
            //print(mesh.bounds.size.z);

            //print(mesh.bounds.min.x);
            //print(mesh.bounds.max.x);
            print(mesh.bounds.extents.x);
            print(mesh.bounds.extents.y);
            print(mesh.bounds.extents.z);

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
            textContainer.localPosition = new Vector3(textContainer.localPosition.x, head.transform.localPosition.y, textContainer.localPosition.z);

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
            textContainer.localPosition = new Vector3(textContainer.localPosition.x, head.transform.localPosition.y, textContainer.localPosition.z);


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

        // Update head position for the different screens.
        if (Input.GetKeyDown(KeyCode.H) || (SteamVR_Actions._default.GrabGrip.GetStateDown(SteamVR_Input_Sources.Any) && DEBUG))
        {
            flatScreen.localPosition = new Vector3(0f, head.transform.localPosition.y, 3.0f);
            textContainer.localPosition = new Vector3(textContainer.localPosition.x, head.transform.localPosition.y, textContainer.localPosition.z);
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
            position = new Vector3(position.x, position.y, position.z - increment * 3); // RECHECK THIS VALUE FOR INCREMENT * n
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
    { // 13 + 6 + 6 + 6 ----> max
        switch (action)
        {
            case TextWidth.increase:
                if (currentVisibleObject < textMeshList.Count - 1)
                {
                    textMeshList[currentVisibleObject++].gameObject.SetActive(false);
                    textMeshList[currentVisibleObject].gameObject.SetActive(true);
                    ChangeVisibleCurvedScreen();
                }
                break;
            case TextWidth.decrease:
                if (currentVisibleObject > 0)
                {
                    textMeshList[currentVisibleObject--].gameObject.SetActive(false);
                    textMeshList[currentVisibleObject].gameObject.SetActive(true);
                    ChangeVisibleCurvedScreen();
                }
                break;
            default:
                Debug.LogError("Invalid option provided.");
                break;
        }
    }

    /*
     * Decide which screen should appear behind the text. This entirely depends on the  range of text meshes
     * that has been generated from Blender and will not look proper if the meshes were to be changed.
     * Ideally this should be calculated from the bounds.extends of the text meshes and compared to the
     * bounds.extends of the curved screens, but this does not seem to be worth the time implementing as
     * of now.
     * TODO: Use bounds.extents instead of hardcoding the values for when a particular screen should be
     * displayed.
     */
    private void ChangeVisibleCurvedScreen()
    {
        int firstRange = 13;
        int screenRange = 6; // How long the screen should be visible before the next one should be displayed.

        // Hide all active curvedScreens
        foreach(Transform curvedScreen in curvedScreenList)
        {
            if (curvedScreen.gameObject.activeSelf) curvedScreen.gameObject.SetActive(false);
        }

        //13 + 6 + 6 + 6
        if (currentVisibleObject < firstRange) curvedScreenList[0].gameObject.SetActive(true);
        else if (currentVisibleObject < firstRange + screenRange) curvedScreenList[1].gameObject.SetActive(true);
        else if (currentVisibleObject < firstRange + screenRange * 2) curvedScreenList[2].gameObject.SetActive(true);
        else if (currentVisibleObject < firstRange + screenRange * 3) curvedScreenList[3].gameObject.SetActive(true);
        else if (currentVisibleObject >= firstRange + screenRange * 3) curvedScreenList[4].gameObject.SetActive(true);
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
