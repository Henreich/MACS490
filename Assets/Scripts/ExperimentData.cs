using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/**
 * Class to hold experiment data
 * TODO Reduce the amount of information stored by only storing
 * relevant values? E.g. calculated distances instead of the
 * user position AND the screen position.
 */
public class ExperimentData
{
    public int participantId { get; set; }

    // Flat screen
    // Comfortable text size
    public Vector3 flatScreenParticipantPosComfortable { get; set; }
    public Vector3 flatScreenPosComfortable { get; set; }
    public Vector3 flatScreenScaleComfortable { get; set; }
    public float flatScreenFontSizeComfortable { get; set; }
    public float flatScreenDistanceToScreenComfortable { get; set; }
    public int currentTextShownComfortable { get; set; }
    public float flatScreenLineHeightComfortable { get; set; }
    public float flatScreenAngularSizeComfortable { get; set; }
    public float flatscreenDmmComfortable { get; set; }


    // Line width
    public Vector3 flatScreenParticipantPosLineWidth { get; set; }
    public float flatScreenDistanceToScreenLineWidth { get; set; }
    public float flatScreenLineWidth { get; set; }
    public float flatScreenAngularSizeLineWidth { get; set; }
    public float flatScreenDMMLineWidth { get; set; }


    // Minimum text size
    public Vector3 flatScreenParticipantPosMinimum { get; set; }
    public Vector3 flatScreenPosMinimum { get; set; }
    public Vector3 flatScreenScaleMinimum { get; set; }
    public float flatScreenFontSizeMinimum { get; set; }
    public float flatScreenDistanceToScreenMinimum { get; set; }
    public int currentTextShownMinimum { get; set; }
    public float flatScreenLineHeightMinimum { get; set; }
    public float flatScreenAngularSizeMinimum { get; set; }
    public float flatscreenDmmMinimum { get; set; }

    // Curved screen
    // Comfortable text size
    public Vector3 curvedScreenParticipantPosComfortable { get; set; }
    public Vector3 curvedScreenPosComfortable { get; set; }
    public Vector3 curvedScreenScaleComfortable { get; set; }
    public float curvedScreenDistanceToScreenComfortable { get; set; }
    public float currentlyVisibleObjectComfortable { get; set; }
    public float curvedScreenLineHeightComfortable { get; set; }
    public float curvedScreenAngularSizeComfortable { get; set; }
    public float curvedscreenDmmComfortable { get; set; }
     
    // Line width
    public Vector3 curvedScreenParticipantPosLineWidth { get; set; }
    public float curvedScreenDistanceToScreenLineWidth { get; set; }
    public float curvedScreenLineWidth { get; set; }
    public float curvedScreenAngularSizeLineWidth { get; set; }
    public float curvedScreenDMMLineWidth { get; set; }

    // Minimum text size
    public Vector3 curvedScreenParticipantPosMinimum { get; set; }
    public Vector3 curvedScreenPosMinimum { get; set; }
    public Vector3 curvedScreenScaleMinimum { get; set; }
    public float curvedScreenDistanceToScreenMinimum { get; set; }
    public float currentlyVisibleObjectMinimum { get; set; }
    public float curvedScreenLineHeightMinimum { get; set; }
    public float curvedScreenAngularSizeMinimum { get; set; }
    public float curvedscreenDmmMinimum { get; set; }
}