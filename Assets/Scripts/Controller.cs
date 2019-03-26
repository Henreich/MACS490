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

    private enum ExperimentStage { flatScreen, curvedScreen };
    private int experimentStage = -1;

    // Stage - Curved text.
    private int currentVisibleObject = 0;
    private enum TextWidth { increase, decrease };
    private List<Transform> textList;

    private int participantId = 0;
    private FileHandler fileHandler;
    private ExperimentData participantData;

    // Start is called before the first frame update
    void Start()
    {
        dataController = ScriptableObject.CreateInstance<DataController>();
        participantData = new ExperimentData();
        fileHandler = ScriptableObject.CreateInstance<FileHandler>();

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
            if (experimentStage == (int) ExperimentStage.flatScreen)   text.fontSize += TEXT_SIZING_INCREMENTS * (touchpadValue.x * TEXT_SIZE_CHANGE_MODIFIER);
            if (experimentStage == (int) ExperimentStage.curvedScreen) ChangeObjectSize(textContainer, true);

        }
        else if (touchpadValue.x < -TRIGGER_THRESHOLD_SMOOTH_SCALING) // LEFT
        {
            if (experimentStage == (int) ExperimentStage.flatScreen) text.fontSize -= TEXT_SIZING_INCREMENTS * (-touchpadValue.x * TEXT_SIZE_CHANGE_MODIFIER);
            if (experimentStage == (int) ExperimentStage.curvedScreen) ChangeObjectSize(textContainer, false);

        }

        if (SteamVR_Actions._default.InteractUI.GetStateDown(SteamVR_Input_Sources.Any))
        {
            // Where the touch pad was triggered
            if (touchpadValue.y > TRIGGER_THRESHOLD) // UP
            {
                if (experimentStage == (int) ExperimentStage.flatScreen) screen.localPosition = new Vector3(screen.localPosition.x, screen.localPosition.y, screen.localPosition.z + 1.0f);
                if (experimentStage == (int) ExperimentStage.curvedScreen) ChangeVisibleObjectInList(TextWidth.increase);
            }
            else if (touchpadValue.y < -TRIGGER_THRESHOLD) // DOWN
            {
                if (experimentStage == (int) ExperimentStage.flatScreen) screen.localPosition = new Vector3(screen.localPosition.x, screen.localPosition.y, screen.localPosition.z - 1.0f);
                if (experimentStage == (int) ExperimentStage.curvedScreen) ChangeVisibleObjectInList(TextWidth.decrease);
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
            print("Saving data.");
            participantData.participantId = participantId;

            if (experimentStage == (int) ExperimentStage.flatScreen)
            {
                participantData.flatScreenParticipantPos = head.transform.localPosition;
                participantData.flatScreenPos = screen.transform.localPosition;
                participantData.flatScreenScale = screen.transform.localScale;
                participantData.flatScreenTextSizeComfortable = text.fontSize;
                participantData.currentTextShown = currentTextShown;
                fileHandler.WriteToFile(participantData);
            }


            if (experimentStage == (int) ExperimentStage.curvedScreen)
            {
                participantData.curvedScreenPos = textCollection.transform.localPosition;
                participantData.curvedScreenScale = textCollection.transform.localScale;
                participantData.currentlyVisibleObject = currentVisibleObject;
            }
            //participantId++;
            //filehandler fh = new filehandler();
            //fh.writetofile(participantid, "transform_flat_screen");
            //fh.writetofile(participantid, fh.formatforfile("x", "y", "z"));
            //fh.writetofile(participantid, fh.formatforfile(flatscreenpos.x, flatscreenpos.y, flatscreenpos.z));

                //// camera position
                //fh.writetofile(participantid, "camera head");
                //fh.writetofile(participantid, fh.formatforfile("x", "y", "z"));
                //fh.writetofile(participantid, fh.formatforfile(headpos.x, headpos.y, headpos.z));

                //participantid++;
                //filehandler.writetofile(  experimentstage);
                //filehandler.writetofile("experiment_stage: " + experimentstage);
                //filehandler.writetofile("experiment_stage: " + experimentstage);

                // temp to control everything inside VR
                //if (experimentStage == -1)
                //{
                //    ChangeExperimentStage((int) ExperimentStage.flatScreen);
                //} else
                //{
                //    if (experimentStage == (int) ExperimentStage.flatScreen)
                //    {
                //        ChangeExperimentStage((int) ExperimentStage.curvedScreen);
                //    } else
                //    {
                //        ChangeExperimentStage((int) ExperimentStage.flatScreen);
                //    }
                //}
                //print(head.transform.localPosition);
                //screen.transform.localPosition = new Vector3(screen.transform.localPosition.x, head.transform.localPosition.y, screen.localPosition.z);
        }

        /**
         * Change the text that is being displayed
         */
        if (Input.GetKeyDown(KeyCode.A))
        {
            if (currentTextShown == dataController.AllTextData.Length) currentTextShown = 0;
            text.text = dataController.AllTextData[currentTextShown++].Text;
        }

        // Change between the experiment stage, either reading from a flat or a curved screen.
        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            ChangeExperimentStage((int) ExperimentStage.flatScreen);
        }
        if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            ChangeExperimentStage((int) ExperimentStage.curvedScreen);
        }
        if (Input.GetKeyDown(KeyCode.Keypad5)) // TEMPORARY
        {
            fileHandler.WriteToFile(participantData);
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

    private void ChangeExperimentStage(int stageNumber)
    {
        experimentStage = stageNumber;
        bool isStageOne = experimentStage == 0 ? true : false;

        screen.gameObject.SetActive(isStageOne);
        textContainer.gameObject.SetActive(!isStageOne);
        print("Experiment stage: " + experimentStage);
    }
}
