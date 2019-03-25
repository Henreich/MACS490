using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/**
 * Class to hold experiment data
 */
public class ExperimentData
{
    public int participantId { get; set; }

    public Vector3 flatScreenParticipantPos { get; set; }
    public Vector3 flatScreenPos { get; set; }
    public Vector3 flatScreenScale { get; set; }
    public float flatScreenTextSizeComfortable { get; set; }
    public float flatScreenTextSizeMinimum { get; set; }
    public int currentTextShown { get; set; }

    //public float flatScreenTextSizeComfortableDistance2 { get; set; }
    //public float flatScreenTextSizeMinimumDistance2 { get; set; }

    public Vector3 curvedScreenPos { get; set; }
    public Vector3 curvedScreenScale { get; set; }
    public float currentlyVisibleObject { get; set; }
}









//public float headPosX { get; set; }
//public float headPosY { get; set; } // Needed? Could be tracked back to the participant.
//public float headPosZ { get; set; }>
//public float flatScreenPosX { get; set; }
//public float flatScreenPosY { get; set; }
//public float flatScreenPosZ { get; set; }
//public float flatScreenScaleX { get; set; }
//public float flatScreenScaleY { get; set; }
//public float flatScreenScaleZ { get; set; }
//public float curvedScreenPosX { get; set; }
//public float curvedScreenPosY { get; set; }
//public float curvedScreenPosZ { get; set; }
//public float curvedScreenScaleX { get; set; }
//public float curvedScreenScaleY { get; set; }
//public float curvedScreenScaleZ { get; set; }