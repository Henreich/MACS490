using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using TMPro;

public class Controller : MonoBehaviour { 
    public SteamVR_Action_Single squeezeAction;
    public SteamVR_Action_Vector2 touchpadAction;

    private DataController dataController;

    public TextMeshPro text;
    public TMP_CharacterInfo characterInfo;

    private double MINIMUM_FONT_SIZE = 1.0f;
    private double MAXIMUM_FONT_SIZE = 7.0f;
    private double TRIGGER_THRESHOLD = 0.85f;
    private float TEXT_SIZING_INCREMENTS = 0.01f;

    private int currentTextShown = 0;

    // Start is called before the first frame update
    void Start()
    {
        //text = GetComponent<TextMeshPro>();
        dataController = ScriptableObject.CreateInstance<DataController>();
        print(dataController.AllTextData.Length);

        //print(text.characterInfo.baseLine);
        text.ForceMeshUpdate(true);
        print("character:" + text.GetTextInfo(text.text).characterInfo[0].character);
        print("baseLine:" + text.GetTextInfo(text.text).characterInfo[0].baseLine);
        print("lineNumber:" + text.GetTextInfo(text.text).characterInfo[0].lineNumber);
        print("scale:" + text.GetTextInfo(text.text).characterInfo[0].scale);
        print("pointSize:" + text.GetTextInfo(text.text).characterInfo[0].pointSize); // Font size
        print("vertexIndex:" + text.GetTextInfo(text.text).characterInfo[0].vertexIndex);

        print("word count:" + text.GetTextInfo(text.text).wordCount);
        print("lineHeight:" + text.GetTextInfo(text.text).lineInfo[0].lineHeight);


    }

    // Update is called once per frame
    void Update()
    {
        if (SteamVR_Actions._default.InteractUI.GetStateDown(SteamVR_Input_Sources.Any))
        {
            Vector2 touchpadValue = touchpadAction.GetAxis(SteamVR_Input_Sources.Any);
            print(touchpadValue);

            // Where the touch pad was triggered
            if (touchpadValue.x > TRIGGER_THRESHOLD)
            {
                print("right pad triggered");
                text.fontSize += TEXT_SIZING_INCREMENTS;
                
            }
            else if (touchpadValue.x < -TRIGGER_THRESHOLD)
            {
                print("left pad triggered");
                text.fontSize -= TEXT_SIZING_INCREMENTS;

            } else if (touchpadValue.y > TRIGGER_THRESHOLD)
            {
                print("up pad triggered");
            } else if (touchpadValue.y < -TRIGGER_THRESHOLD)
            {
                print("down pad triggered");
            }
        }

        //float triggerValue = squeezeAction.GetAxis(SteamVR_Input_Sources.Any);

        //if (triggerValue > 0.0f)
        //{
        //    print(triggerValue);
        //}

        if(SteamVR_Actions._default.GrabPinch.GetStateDown(SteamVR_Input_Sources.Any))
        {
            //print("Trigger down.");
            print(text.fontSize);
        }


        if (Input.GetKeyDown(KeyCode.A))
        {
            if (currentTextShown == dataController.AllTextData.Length) currentTextShown = 0;
            text.text = dataController.AllTextData[currentTextShown++].Text;
        }

    }
}
