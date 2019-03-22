using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FileHandler : MonoBehaviour
{
    private string folderPath = "ExperimentData/";
    private string fileName = "participant";
    private readonly string suffix = ".txt";

    void Start()
    {
    }

    public void WriteToFile(int id, string text)
    {
        string filePath = folderPath + fileName + id + suffix;

        if (File.Exists(filePath))
        {
            print(filePath);
            using (var append = File.AppendText(filePath))
            {
                append.WriteLine(text);
                append.Flush();
                append.Close();
            }

        }
        else // New participant, create new file with identifier on first line.
        {
            using (var writer = new StreamWriter(filePath))
            {
                writer.WriteLine("Participant: " + id);
                writer.WriteLine(text);
                writer.Flush();
                writer.Close();
            }
        }
    }
    /*
     * Utilities for formatting.
     */
    public string FormatForFile(float x, float y, float z)
    {
        string tab = "\t";
        return x + tab + y + tab + z;
    }

    public string FormatForFile(string x, string y, string z)
    {
        string tab = "\t";
        return x + tab + y + tab + z;
    }
}
