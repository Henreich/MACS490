using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DataController : MonoBehaviour
{
    private string TEXT_DATA_FILE_NAME = "text_data.json";

    //private TextData[] allTextData;

    // Start is called before the first frame update
    void Start()
    {
        LoadTextData();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A)) {
            //print(allTextData);
        }
    }

    void LoadTextData()
    {
  
        TextData textData = new TextData();
        textData.Text = "Lorem ipsum";

        string json = JsonUtility.ToJson(textData);

        //TextData loadedData = JsonUtility.FromJson<TextData>(json);
        //print(loadedData.Text);

        string filePath = Path.Combine(Application.streamingAssetsPath, TEXT_DATA_FILE_NAME);
        string dataFromFile = File.ReadAllText(filePath);
        TextData loadedData = JsonUtility.FromJson<TextData>(dataFromFile);

        print(loadedData.Text);
        
        //print(Application.dataPath + "/Scripts/test.json");
        //File.WriteAllText(Application.dataPath + "/Scripts/test.json", json);


        //if (File.Exists(filePath))
        //{
        //    string fileData = File.ReadAllText(filePath);
        //    TextCollection collection = JsonUtility.FromJson<TextCollection>(fileData);
        //    allTextData = collection.allTextData;

        //    print(allTextData.Length);
        //    // alltextData[0].Text == Null even though allTextData.Length == 2...
        //    print(allTextData[0]);
        //    print(allTextData[0].Text);
        //}
        //else
        //{
        //    Debug.LogError("Cannot load text data from " + filePath);
        //}
    }

    private class TextData
    {
        public string Text;
    }
}
