using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FileHandler : ScriptableObject
{
    private string folderPath = "ExperimentData/";
    private string fileName = "participant_data";
    private readonly string suffix = ".csv";

    /*
     * Writes the ExperimentData to file in .csv format. Each participant
     * has one row of data, meaning there's a lot of columns. This is because
     * the data will be interpreted by SPSS. 
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
                    "{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20},{21},{22},{23},{24},{25},{26},{27},{28},{29},{30},{31},{32},{33},{34},{35},{36},{37},{38},{39},{40},{41},{42},{43},{44},{45},{46},{47},{48},{49},{50},{51},{52},{53},{54},{55},{56},{57},{58},{59},{60},{61},{62},{63}",
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
                    
                    // Line width
                    data.flatScreenParticipantPosLineWidth.x,
                    data.flatScreenParticipantPosLineWidth.z,
                    data.flatScreenDistanceToScreenLineWidth,
                    data.flatScreenLineWidth,
                    data.flatScreenAngularSizeLineWidth,
                    data.flatScreenDMMLineWidth,

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
                    data.curvedScreenLineWidthComfortable,
                    data.curvedScreenLineHeightComfortable,
                    data.curvedScreenAngularSizeComfortable,
                    data.curvedscreenDmmComfortable,
                    
                    //Line width
                    data.curvedScreenParticipantPosLineWidth.x,
                    data.curvedScreenParticipantPosLineWidth.z,
                    data.curvedScreenDistanceToScreenLineWidth,
                    data.curvedScreenLineWidth,
                    data.curvedScreenWidth,
                    data.curvedScreenAngularSizeLineWidth,
                    data.curvedScreenDMMLineWidth,

                    // Minimum text size
                    data.curvedScreenParticipantPosMinimum.x,
                    data.curvedScreenParticipantPosMinimum.z,
                    data.curvedScreenPosMinimum.x,
                    data.curvedScreenPosMinimum.z,
                    data.curvedScreenScaleMinimum.x,
                    data.curvedScreenScaleMinimum.y,
                    data.curvedScreenScaleMinimum.z,
                    data.curvedScreenDistanceToScreenMinimum,
                    data.curvedScreenLineWidthMinimum,
                    data.curvedScreenLineHeightMinimum,
                    data.curvedScreenAngularSizeMinimum,
                    data.curvedscreenDmmMinimum
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

                //Line width
                string col15 = "flatScreenParticipantPosLineWidthX";
                string col16 = "flatScreenParticipantPosLineWidthZ";
                string col17 = "flatScreenDistanceToScreenLineWidth";
                string col18 = "flatScreenLineWidth";
                string col19 = "flatScreenAngularSizeLineWidth";
                string col20 = "flatScreenDMMLineWidth";

                // Minimum text size
                string col21 = "flatScreenParticipantPosMinimumX";
                string col22 = "flatScreenParticipantPosMinimumZ";
                string col23 = "flatScreenPosMinimumX";
                string col24  = "flatScreenPosMinimumZ";
                string col25  = "flatScreenScaleMinimumX";
                string col26  = "flatScreenScaleMinimumY";
                string col27  = "flatScreenScaleMinimumZ";
                string col28  = "flatScreenFontSizeMinimum";
                string col29  = "flatScreenDistanceToScreenMinimum";
                string col30  = "currentTextShownMinimum";
                string col31  = "flatScreenLineHeightMinimum";
                string col32  = "flatScreenAngularSizeMinimum";
                string col33  = "flatscreenDmmMinimum";

                // Curved screen
                // Comfortable text size
                string col34  = "curvedScreenParticipantPosComfortableX";
                string col35  = "curvedScreenParticipantPosComfortableZ";
                string col36  = "curvedScreenPosComfortableX";
                string col37  = "curvedScreenPosComfortableZ";
                string col38  = "curvedScreenScaleComfortableX";
                string col39  = "curvedScreenScaleComfortableY";
                string col40  = "curvedScreenScaleComfortableZ";
                string col41  = "curvedScreenDistanceToScreenComfortable";
                string col42  = "curvedScreenLineWidthComfortable";
                string col43  = "curvedScreenLineHeightComfortable";
                string col44  = "curvedScreenAngularSizeComfortable";
                string col45  = "curvedscreenDmmComfortable";

                // Line width
                string col46 = "curvedScreenParticipantPosLineWidthX";
                string col47 = "curvedScreenParticipantPosLineWidthY";
                string col48 = "curvedScreenDistanceToScreenLineWidth";
                string col49 = "curvedScreenLineWidth";
                string col50 = "curvedScreenWidth";
                string col51 = "curvedScreenAngularSizeLineWidth";
                string col52 = "curvedScreenDMMLineWidth";

                // Minimum text size
                string col53  = "curvedScreenParticipantPosMinimumX";
                string col54  = "curvedScreenParticipantPosMinimumZ";
                string col55  = "curvedScreenPosMinimumX";
                string col56  = "curvedScreenPosMinimumZ";
                string col57  = "curvedScreenScaleMinimumX";
                string col58  = "curvedScreenScaleMinimumY";
                string col59  = "curvedScreenScaleMinimumZ";
                string col60  = "curvedScreenDistanceToScreenMinimum";
                string col61  = "curvedScreenLineWidthMinimum";
                string col62  = "curvedScreenLineHeightMinimum";
                string col63  = "curvedScreenAngularSizeMinimum";
                string col64  = "curvedscreenDmmMinimum";

                string csvFormattedOutput = string.Format(
                    "{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20},{21},{22},{23},{24},{25},{26},{27},{28},{29},{30},{31},{32},{33},{34},{35},{36},{37},{38},{39},{40},{41},{42},{43},{44},{45},{46},{47},{48},{49},{50},{51},{52},{53},{54},{55},{56},{57},{58},{59},{60},{61},{62},{63}",
                    col1, col2, col3, col4, col5, col6, col7, col8, col9, col10, 
                    col11, col12, col13, col14, col15, col16, col17, col18, col19, col20, 
                    col21, col22, col23, col24, col25, col26, col27, col28, col29, col30, 
                    col31, col32, col33, col34, col35, col36, col37, col38, col39, col40, 
                    col41, col42, col43, col44, col45, col46, col47, col48, col49, col50, 
                    col51, col52, col53, col54, col55, col56, col57, col58, col59, col60, 
                    col61, col62, col63, col64
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
        string path = "ExperimentData/post_hoc_CPL.csv";

        if (File.Exists(path))
        {
            using (var append = File.AppendText(path))
            {
                append.WriteLine(data);
                append.Flush();
                append.Close();
            }

        }
        else // New participant, create new file with column identifiers on first line.
        {
            using (var writer = new StreamWriter(path))
            {

                writer.WriteLine(data);
                writer.Flush();
                writer.Close();
            }
        }
    }
}
