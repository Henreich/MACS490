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
                    "{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20},{21},{22},{23},{24},{25},{26},{27},{28},{29},{30},{31},{32},{33},{34},{35},{36},{37},{38},{39},{40},{41},{42},{43},{44}",
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
                    data.flatScreenFontSizeComfortable,
                    data.flatScreenDistanceToScreenComfortable,
                    data.currentTextShownComfortable,
                    data.flatScreenLineHeightComfortable,
                    data.flatScreenAngularSizeComfortable,
                    data.flatscreenDmmComfortable,

                    // Minimum text size
                    data.flatScreenParticipantPosMinimum.x,
                    data.flatScreenParticipantPosMinimum.z,
                    data.flatScreenPosMinimum.x,
                    data.flatScreenPosMinimum.z,
                    data.flatScreenScaleMinimum.x,
                    data.flatScreenScaleMinimum.y,
                    data.flatScreenScaleMinimum.z,
                    data.flatScreenFontSizeMinimum,
                    data.flatScreenDistanceToScreenMinimum,
                    data.currentTextShownMinimum,
                    data.flatScreenLineHeightMinimum,
                    data.flatScreenAngularSizeMinimum,
                    data.flatscreenDmmMinimum,

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
                string col9   = "flatScreenFontSizeComfortable";
                string col10  = "flatScreenDistanceToScreenComfortable";
                string col11  = "currentTextShownComfortable";
                string col12  = "flatScreenLineHeightComfortable";
                string col13  = "flatScreenAngularSizeComfortable";
                string col14  = "flatscreenDmmComfortable";

                // Minimum text size
                string col15  = "flatScreenParticipantPosMinimumX";
                string col16  = "flatScreenParticipantPosMinimumZ";
                string col17  = "flatScreenPosMinimumX";
                string col18  = "flatScreenPosMinimumZ";
                string col19  = "flatScreenScaleMinimumX";
                string col20  = "flatScreenScaleMinimumY";
                string col21  = "flatScreenScaleMinimumZ";
                string col22  = "flatScreenFontSizeMinimum";
                string col23  = "flatScreenDistanceToScreenMinimum";
                string col24  = "currentTextShownMinimum";
                string col25  = "flatScreenLineHeightMinimum";
                string col26  = "flatScreenAngularSizeMinimum";
                string col27  = "flatscreenDmmMinimum";

                // Curved screen
                // Comfortable text size
                string col28  = "curvedScreenParticipantPosComfortableX";
                string col29  = "curvedScreenParticipantPosComfortableZ";
                string col30  = "curvedScreenPosComfortableX";
                string col31  = "curvedScreenPosComfortableZ";
                string col32  = "curvedScreenScaleComfortableX";
                string col33  = "curvedScreenScaleComfortableY";
                string col34  = "curvedScreenScaleComfortableZ";
                string col35  = "curvedScreenDistanceToScreenComfortable";
                string col36  = "currentlyVisibleObjectComfortable";

                // Minimum text size
                string col37  = "curvedScreenParticipantPosMinimumX";
                string col38  = "curvedScreenParticipantPosMinimumZ";
                string col39  = "curvedScreenPosMinimumX";
                string col40  = "curvedScreenPosMinimumZ";
                string col41  = "curvedScreenScaleMinimumX";
                string col42  = "curvedScreenScaleMinimumY";
                string col43  = "curvedScreenScaleMinimumZ";
                string col44  = "curvedScreenDistanceToScreenMinimum";
                string col45  = "currentlyVisibleObjectMinimum";
                //string col  = "";

                // ,{}
                string csvFormattedOutput = string.Format(
                    "{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20},{21},{22},{23},{24},{25},{26},{27},{28},{29},{30},{31},{32},{33},{34},{35},{36},{37},{38},{39},{40},{41},{42},{43},{44}",
                    col1, col2, col3, col4, col5, col6, col7, col8, col9, col10, col11, col12, col13, col14, col15, col16, col17, col18, col19, col20, col21, col22, col23, col24, col25, col26, col27, col28, col29, col30, col31, col32, col33, col34, col35, col36, col37, col38, col39, col40, col41, col42, col43, col44, col45
                );

                writer.WriteLine(csvFormattedOutput);
                writer.Flush();
                writer.Close();
                WriteToFile(data);
            }
        }
    }

    /*
     * Writing to file functions only used for visualising a bunch of data easier during development.
     */
    public void WriteToFile(string data)
    {
        if (File.Exists("ExperimentData/lineInfo.txt"))
        {
            using (var append = File.AppendText("ExperimentData/lineInfo.txt"))
            {
                append.WriteLine(data);
                append.Flush();
                append.Close();
            }

        }
        else // New participant, create new file with column identifiers on first line.
        {
            using (var writer = new StreamWriter("ExperimentData/lineInfo.txt"))
            {

                writer.WriteLine(data);
                writer.Flush();
                writer.Close();
            }
        }
    }

    public void WriteToFile(float data)
    {
        if (File.Exists("ExperimentData/characterinfo.txt"))
        {
            using (var append = File.AppendText("ExperimentData/characterinfo.txt"))
            {
                append.WriteLine(data);
                append.Flush();
                append.Close();
            }

        }
        else // New participant, create new file with column identifiers on first line.
        {
            using (var writer = new StreamWriter("ExperimentData/characterinfo.txt"))
            {

                writer.WriteLine(data);
                writer.Flush();
                writer.Close();
            }
        }
    }
}
