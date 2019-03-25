using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FileHandler : MonoBehaviour
{
    private string folderPath = "ExperimentData/";
    private string fileName = "participants";
    private readonly string suffix = ".txt";

    void Start()
    {
    }

    public void WriteToFile(ExperimentData data)
    {
        string filePath = folderPath + fileName + suffix;

        if (File.Exists(filePath))
        {
            print(filePath);
            using (var append = File.AppendText(filePath))
            {
                string csvFormattedOutput = string.Format(
                    "{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11}",
                    data.participantId,                 // 0
                    data.flatScreenParticipantPos.x,    // 1
                    data.flatScreenParticipantPos.y,    // 2
                    data.flatScreenParticipantPos.z,    // 3
                    data.flatScreenPos.x,               // 4
                    data.flatScreenPos.y,               // 5
                    data.flatScreenPos.z,               // 6
                    data.flatScreenScale.x,             // 7
                    data.flatScreenScale.y,             // 8
                    data.flatScreenScale.z,             // 9
                    data.flatScreenTextSizeComfortable, // 10
                    data.flatScreenPos.z - Mathf.Abs(data.flatScreenParticipantPos.z) // 11 Calculated distance to "screen"
                );
                append.WriteLine(csvFormattedOutput);
                append.Flush();
                append.Close();
            }

        }
        else // New participant, create new file with column identifiers on first line.
        {
            using (var writer = new StreamWriter(filePath))
            {
                string col1  = "participantId";
                string col2  = "flatScreenParticipantPosX";
                string col3  = "flatScreenParticipantPosY";
                string col4  = "flatScreenParticipantPosZ";
                string col5  = "flatScreenPosX";
                string col6  = "flatScreenPosY";
                string col7  = "flatScreenPosZ";
                string col8  = "flatScreenScaleX";
                string col9  = "flatScreenScaleY";
                string col10 = "flatScreenScaleZ";
                string col11 = "flatScreenTextSizeComfortable";
                string col12 = "flatScreenCalculatedDistanceComfortable";
                string col13 = "flatScreenTextSizeMinimum";
                string col14 = "flatScreenCalculatedDistanceMinimum";

                string csvFormattedOutput = string.Format(
                    "{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11}", 
                    col1, col2, col3, col4, col5, col6, col7, col8, col9, col10, col11, col12
                );

                writer.WriteLine(csvFormattedOutput);
                writer.Flush();
                writer.Close();
                WriteToFile(data);
            }
        }
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

    public string FormatForFile(Vector3 data)
    {
        string tab = "\t";
        return data.x + tab + data.y + tab + data.z;
    }
}


//FileHandler fh = new FileHandler();
//fh.WriteToFile(participantId, "Transform_flat_screen");
//fh.WriteToFile(participantId, fh.FormatForFile("X", "Y", "Z"));
//fh.WriteToFile(participantId, fh.FormatForFile(flatScreenPos.x, flatScreenPos.y, flatScreenPos.z));

//// Camera position
//fh.WriteToFile(participantId, "Camera head");
//fh.WriteToFile(participantId, fh.FormatForFile("X", "Y", "Z"));
//fh.WriteToFile(participantId, fh.FormatForFile(headPos.x, headPos.y, headPos.z));

//participantId++;
//fileHandler.WriteToFile(  experimentStage);
//fileHandler.WriteToFile("Experiment_stage: " + experimentStage);