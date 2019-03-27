using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FileHandler : ScriptableObject
{
    private string folderPath = "ExperimentData/";
    private string fileName = "participant_data";
    private readonly string suffix = ".txt";

    /*
     * Writes the ExperimentData to file in .csv format. Each participant
     * has one row of data, meaning there's a lot of columns. This is because
     * the data will be interpreted by SPSS. 
     * TODO: Will check if one row is actually
     * beneficial for SPSS, since the amount of columns is getting out of hand.
     */
    public void WriteToFile(ExperimentData data)
    {
        string filePath = folderPath + fileName + suffix;

        if (File.Exists(filePath))
        {
            Debug.Log(filePath);
            using (var append = File.AppendText(filePath))
            {
                string csvFormattedOutput = string.Format(
                    "{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20},{21},{22},{23},{24},{25},{26},{27},{28},{29},{30},{31},{32},{33},{34},{35},{36},{37},{38}",
                    // Flat Screen
                    // Comfortable text size
                    data.participantId,
                    data.flatScreenParticipantPosComfortable.x,
                    data.flatScreenParticipantPosComfortable.z,
                    data.flatScreenPosComfortable.x,
                    data.flatScreenPosComfortable.z,
                    data.flatScreenScaleComfortable.x,
                    data.flatScreenScaleComfortable.y,
                    data.flatScreenScaleComfortable.z,
                    data.flatScreenTextSizeComfortable,
                    data.flatScreenDistanceToScreenComfortable,
                    data.currentTextShownComfortable,

                    // Minimum text size
                    data.flatScreenParticipantPosMinimum.x,
                    data.flatScreenParticipantPosMinimum.z,
                    data.flatScreenPosMinimum.x,
                    data.flatScreenPosMinimum.z,
                    data.flatScreenScaleMinimum.x,
                    data.flatScreenScaleMinimum.y,
                    data.flatScreenScaleMinimum.z,
                    data.flatScreenTextSizeMinimum,
                    data.flatScreenDistanceToScreenMinimum,
                    data.currentTextShownMinimum,
                    
                    // Curved screen
                    // Comfortable text size
                    data.curvedScreenParticipantPosComfortable.x,
                    data.curvedScreenParticipantPosComfortable.z,
                    data.curvedScreenPosComfortable.x,
                    data.curvedScreenPosComfortable.z,
                    data.curvedScreenScaleComfortable.x,
                    data.curvedScreenScaleComfortable.y,
                    data.curvedScreenScaleComfortable.z,
                    data.curvedScreenDistanceToScreenComfortable,
                    data.currentlyVisibleObjectComfortable,

                    // Minimum text size
                    data.curvedScreenParticipantPosMinimum.x,
                    data.curvedScreenParticipantPosMinimum.z,
                    data.curvedScreenPosMinimum.x,
                    data.curvedScreenPosMinimum.z,
                    data.curvedScreenScaleMinimum.x,
                    data.curvedScreenScaleMinimum.y,
                    data.curvedScreenScaleMinimum.z,
                    data.curvedScreenDistanceToScreenMinimum,
                    data.currentlyVisibleObjectMinimum
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
                string col1   = "participantId";

                // Flat screen
                // Comfortable text size
                string col2   = "flatScreenParticipantPosComfortableX";
                string col3   = "flatScreenParticipantPosComfortableZ";
                string col4   = "flatScreenPosComfortableX";
                string col5   = "flatScreenPosComfortableZ";
                string col6   = "flatScreenScaleComfortableX";
                string col7   = "flatScreenScaleComfortableY";
                string col8   = "flatScreenScaleComfortableZ";
                string col9   = "flatScreenTextSizeComfortable";
                string col10  = "flatScreenDistanceToScreenComfortable";
                string col11  = "currentTextShownComfortable";

                // Minimum text size
                string col12  = "flatScreenParticipantPosMinimumX";
                string col13  = "flatScreenParticipantPosMinimumZ";
                string col14  = "flatScreenPosMinimumX";
                string col15  = "flatScreenPosMinimumZ";
                string col16  = "flatScreenScaleMinimumX";
                string col17  = "flatScreenScaleMinimumY";
                string col18  = "flatScreenScaleMinimumZ";
                string col19  = "flatScreenTextSizeMinimum";
                string col20  = "flatScreenDistanceToScreenMinimum";
                string col21  = "currentTextShownMinimum";
                
                // Curved screen
                // Comfortable text size
                string col22  = "curvedScreenParticipantPosComfortableX";
                string col23  = "curvedScreenParticipantPosComfortableZ";
                string col24  = "curvedScreenPosComfortableX";
                string col25  = "curvedScreenPosComfortableZ";
                string col26  = "curvedScreenScaleComfortableX";
                string col27  = "curvedScreenScaleComfortableY";
                string col28  = "curvedScreenScaleComfortableZ";
                string col29  = "curvedScreenDistanceToScreenComfortable";
                string col30  = "currentlyVisibleObjectComfortable";

                // Minimum text size
                string col31  = "curvedScreenParticipantPosMinimumX";
                string col32  = "curvedScreenParticipantPosMinimumZ";
                string col33  = "curvedScreenPosMinimumX";
                string col34  = "curvedScreenPosMinimumZ";
                string col35  = "curvedScreenScaleMinimumX";
                string col36  = "curvedScreenScaleMinimumY";
                string col37  = "curvedScreenScaleMinimumZ";
                string col38  = "curvedScreenDistanceToScreenMinimum";
                string col39  = "currentlyVisibleObjectMinimum";
                //string col  = "";

                // ,{}
                string csvFormattedOutput = string.Format(
                     "{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20},{21},{22},{23},{24},{25},{26},{27},{28},{29},{30},{31},{32},{33},{34},{35},{36},{37},{38}", 
                    col1, col2, col3, col4, col5, col6, col7, col8, col9, col10, col11, col12, col13, col14, col15, col16, col17, col18, col19, col20, col21, col22, col23, col24, col25, col26, col27, col28, col29, col30, col31, col32, col33, col34, col35, col36, col37, col38, col39
                );

                writer.WriteLine(csvFormattedOutput);
                writer.Flush();
                writer.Close();
                WriteToFile(data);
            }
        }
    }
}
