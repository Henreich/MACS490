using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DataController : MonoBehaviour
{
    private readonly string TEXT_DATA_FILE_NAME = "text_data.json";

    private TextData[] allTextData;

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

    /**
     * Load JSON from file and store it in the allTextData array for later access.
     ***/
    void LoadTextData()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, TEXT_DATA_FILE_NAME);
        
        if (File.Exists(filePath))
        {
            string fileData = File.ReadAllText(filePath);
            allTextData = JsonHelper.FromJson<TextData>(fileData);
        }
        else
        {
            Debug.LogError("Cannot load text data from " + filePath);
        }
    }

    /**
     * Creates dummy JSON formatted file with 4 array objects.
     **/
    public void CreateDummyData()
    {
        //TextCollection collection = new TextCollection();
        //collection.allTextData.Add(new TextData(0, "Lorem ipsum"));
        //collection.allTextData.Add(new TextData(1, "I can't believe it's not lorem ipsum!"));
        //collection.allTextData.Add(new TextData(2, "Superhot is pretty fun"));
        //collection.allTextData.Add(new TextData(3, "All good things are three"));

        //TextData[] arrayToJson = collection.allTextData.ToArray();

        //string json = JsonHelper.ToJson(arrayToJson, true);
        //File.WriteAllText(Application.dataPath + "/Scripts/test.json", json);
    }

    [System.Serializable]
    private class TextCollection
    {
        public List<TextData> allTextData = new List<TextData>();
    }
    [System.Serializable]
    private class TextData
    {
        public int Id;
        public string Text;

        public TextData(int id, string text)
        {
            this.Id = id;
            this.Text = text;
        }
    }

    /**
     * Helper class to convert to and from arrays of objects to JSON arrays, 
     * since this does not seem to be supportedby Jsonutilities yet. 
     * https://stackoverflow.com/questions/36239705/serialize-and-deserialize-json-and-json-array-in-unity
     * http://www.boxheadproductions.com.au/deserializing-top-level-arrays-in-json-with-unity/
     * **/
    public static class JsonHelper
    {
        public static T[] FromJson<T>(string json)
        {
            Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
            return wrapper.Items;
        }

        public static string ToJson<T>(T[] array)
        {
            Wrapper<T> wrapper = new Wrapper<T>();
            wrapper.Items = array;
            return JsonUtility.ToJson(wrapper);
        }

        public static string ToJson<T>(T[] array, bool prettyPrint)
        {
            Wrapper<T> wrapper = new Wrapper<T>();
            wrapper.Items = array;
            return JsonUtility.ToJson(wrapper, prettyPrint);
        }

        [System.Serializable]
        private class Wrapper<T>
        {
            public T[] Items;
        }
    }
}
