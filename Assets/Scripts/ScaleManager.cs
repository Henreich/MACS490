using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleManager : MonoBehaviour
{
    private Transform screen;
    private readonly float WORLD_CENTER = 0.0f;
    private readonly float SCALE_Z = 0.001f;
    private readonly float ANGLE_COMFORTABLE_LEFTRIGHT = 30;
    private readonly float ANGLE_COMFORTABLE_UP = 20;
    private readonly float ANGLE_COMFORTABLE_DOWN = 12;

    private readonly float ANGLE_MAX_LEFTRIGHT = 55;
    private readonly float ANGLE_MAX_UP = 60;
    private readonly float ANGLE_MAX_DOWN = 40;

    // Start is called before the first frame update
    void Start()
    {
        screen = GetComponent<Transform>();
        Scale();

    }

    // Update is called once per frame
    void Update()
    {
        Scale();
    }

    private void Scale()
    {
        float ScaleX    = 2 * (GetZDistanceFromZero() / Mathf.Cos(ANGLE_COMFORTABLE_LEFTRIGHT * Mathf.Deg2Rad));
        float up        = GetZDistanceFromZero() / Mathf.Cos(ANGLE_COMFORTABLE_UP * Mathf.Deg2Rad);
        float down      = GetZDistanceFromZero() / Mathf.Cos(ANGLE_COMFORTABLE_DOWN * Mathf.Deg2Rad);
        float ScaleY    = up + down;
        screen.transform.localScale = new Vector3(ScaleX, ScaleY, SCALE_Z);
    }

    /**
     * Get amount of distance units between the object and the center (0, 0, 0)
     **/
    private float GetZDistanceFromZero()
    {
        return System.Math.Abs(screen.transform.localPosition.z - WORLD_CENTER);
    }
}
