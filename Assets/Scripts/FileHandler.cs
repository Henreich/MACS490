using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FileHandler : ScriptableObject
{
    private string folderPath = "ExperimentData/";
    private string fileName = "participant_data";
    private readonly string suffix = ".txt";

    void Start()
    {
    }

    public void WriteToFile(ExperimentData data)
    {
        string filePath = folderPath + fileName + suffix;

        if (File.Exists(filePath))
        {
            using (var append = File.AppendText(filePath))
            {
                Debug.Log(string.Format("Appending to file: {0}", filePath));
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
                Debug.Log(string.Format("Creating new file: {0}", filePath));
                //string col1  = "participantId";
                //string col2  = "flatScreenParticipantPosX";
                //string col3  = "flatScreenParticipantPosY";
                //string col4  = "flatScreenParticipantPosZ";
                //string col5  = "flatScreenPosX";
                //string col6  = "flatScreenPosY";
                //string col7  = "flatScreenPosZ";
                //string col8  = "flatScreenScaleX";
                //string col9  = "flatScreenScaleY";
                //string col10 = "flatScreenScaleZ";
                //string col11 = "flatScreenTextSizeComfortable";
                //string col12 = "flatScreenCalculatedDistanceComfortable";
                //string col13 = "flatScreenTextSizeMinimum";
                //string col14 = "flatScreenCalculatedDistanceMinimum";

                string col1  = "participantId";
                string col2  = "experimentId";
                string col3  = "screenPosX";
                string col4  = "screenPosY";
                string col5  = "screenPosZ";
                string col6  = "screenScaleX";
                string col7  = "screenScaleY";
                string col8  = "screenScaleZ";
                string col9  = "textSize";
                string col10 = "displayedTextIndex";
                string col11 = "calculatedDistanceToScreen";

                string csvFormattedOutput = string.Format(
                    "{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10}",
                    col1, col2, col3, col4, col5, col6, col7, col8, col9, col10, col11
                );
                //string csvFormattedOutput = string.Format(
                //    "{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11}", 
                //    col1, col2, col3, col4, col5, col6, col7, col8, col9, col10, col11, col12
                //);

                writer.WriteLine(csvFormattedOutput);
                writer.Flush();
                writer.Close();
                //WriteToFile(data);
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