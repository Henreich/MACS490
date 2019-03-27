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
    public float flatScreenTextSizeComfortable { get; set; }
    public float flatScreenDistanceToScreenComfortable { get; set; }
    public int currentTextShownComfortable { get; set; }

    // Minimum text size
    public Vector3 flatScreenParticipantPosMinimum { get; set; }
    public Vector3 flatScreenPosMinimum { get; set; }
    public Vector3 flatScreenScaleMinimum { get; set; }
    public float flatScreenTextSizeMinimum { get; set; }
    public float flatScreenDistanceToScreenMinimum { get; set; }
    public int currentTextShownMinimum { get; set; }

    // Curved screen
    // Comfortable text size
    public Vector3 curvedScreenParticipantPosComfortable { get; set; }
    public Vector3 curvedScreenPosComfortable { get; set; }
    public Vector3 curvedScreenScaleComfortable { get; set; }
    public float curvedScreenDistanceToScreenComfortable { get; set; }
    public float currentlyVisibleObjectComfortable { get; set; }

    // Minimum text size
    public Vector3 curvedScreenParticipantPosMinimum { get; set; }
    public Vector3 curvedScreenPosMinimum { get; set; }
    public Vector3 curvedScreenScaleMinimum { get; set; }
    public float curvedScreenDistanceToScreenMinimum { get; set; }
    public float currentlyVisibleObjectMinimum { get; set; }
}