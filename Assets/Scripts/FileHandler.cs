using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FileHandler : MonoBehaviour
{
    private string path = "Assets/Resources/";
    private string fileName = "participant";
    private int lastParticipantId = 0;
    private readonly string suffix = ".txt";

    void Start()
    {
        WriteToFile("Participant: " + lastParticipantId + "\n");
    }

    private void WriteToFile(string text)
    {
        string filePath = path + fileName + lastParticipantId + suffix;

        if (!File.Exists(filePath))
        {
            StreamWriter sw = new StreamWriter(filePath, true);
            sw.WriteLine(text);
            sw.Close();   
        } else
        {
            lastParticipantId++;
            WriteToFile("Participant: " + lastParticipantId + "\n");
        }
    }

    private void FileIsComplete()
    {
        lastParticipantId++;
    }
}
