using UnityEngine;
using System.Collections;
using TMPro;


/**
 * Initially from this forum thread.
 * http://digitalnativestudios.com/forum/index.php?topic=998.0
 */
public class WarpTextExample : MonoBehaviour
{

    private TextMeshPro m_TextComponent;

    public AnimationCurve VertexCurve = new AnimationCurve( new Keyframe(0.0f, 0.0f),
                                                            new Keyframe(1.0f, 1.0f),
                                                            new Keyframe(2.0f, 0.0f)
                                                            );
    public float AngleMultiplier = 1.0f;
    public float SpeedMultiplier = 1.0f;
    public float CurveScale = 1.0f;

    void Awake()
    {
        m_TextComponent = gameObject.GetComponent<TextMeshPro>();
    }


    void Start()
    {
        StartCoroutine(WarpText());
    }


    private AnimationCurve CopyAnimationCurve(AnimationCurve curve)
    {
        AnimationCurve newCurve = new AnimationCurve
        {
            keys = curve.keys
        };

        return newCurve;
    }


    /// <summary>
    ///  Method to curve text along a Unity animation curve.
    /// </summary>
    /// <param name="textComponent"></param>
    /// <returns></returns>
    IEnumerator WarpText()
    {
        VertexCurve.preWrapMode = WrapMode.Clamp;
        VertexCurve.postWrapMode = WrapMode.Clamp;

        //Mesh mesh = m_TextComponent.textInfo.meshInfo[0].mesh;

        Vector3[] vertices;
        Matrix4x4 matrix;

        m_TextComponent.havePropertiesChanged = true; // Need to force the TextMeshPro Object to be updated.
        CurveScale *= 10;
        float old_CurveScale = CurveScale;
        AnimationCurve old_curve = CopyAnimationCurve(VertexCurve);

        while (true)
        {
            if (!m_TextComponent.havePropertiesChanged && old_CurveScale == CurveScale && old_curve.keys[1].value == VertexCurve.keys[1].value)
            {
                yield return null;
                continue;
            }

            old_CurveScale = CurveScale;
            old_curve = CopyAnimationCurve(VertexCurve);

            m_TextComponent.ForceMeshUpdate(); // Generate the mesh and populate the textInfo with data we can use and manipulate.

            TMP_TextInfo textInfo = m_TextComponent.textInfo;
            int characterCount = textInfo.characterCount;


            if (characterCount == 0) continue;

            //vertices = textInfo.meshInfo[0].vertices;
            //int lastVertexIndex = textInfo.characterInfo[characterCount - 1].vertexIndex;

            float boundsMinX = m_TextComponent.textInfo.meshInfo[0].mesh.bounds.min.x;
            float boundsMaxX = m_TextComponent.textInfo.meshInfo[0].mesh.bounds.max.x;
            //print("boundsMinX | maxX ==>" + boundsMinX + " | " + boundsMaxX);
           
            for (int i = 0; i < characterCount; i++)
            {
                if (!textInfo.characterInfo[i].isVisible)
                    continue;

                int vertexIndex = textInfo.characterInfo[i].vertexIndex;

                // Get the index of the mesh used by this character.
                int meshIndex = textInfo.characterInfo[i].materialReferenceIndex;

                vertices = textInfo.meshInfo[meshIndex].vertices;

                // Compute the baseline mid point for each character
                Vector3 offsetToMidBaseline = new Vector2((vertices[vertexIndex + 0].x + vertices[vertexIndex + 2].x) / 2, textInfo.characterInfo[i].baseLine);
                //float offsetY = VertexCurve.Evaluate((float)i / characterCount + loopCount / 50f); // Random.Range(-0.25f, 0.25f);

                // Apply offset to adjust our pivot point.
                vertices[vertexIndex + 0] += -offsetToMidBaseline;
                vertices[vertexIndex + 1] += -offsetToMidBaseline;
                vertices[vertexIndex + 2] += -offsetToMidBaseline;
                vertices[vertexIndex + 3] += -offsetToMidBaseline;

                // Compute the angle of rotation for each character based on the animation curve

                // x = TIME   = X-axis
                // y = Height = Y-axis
                float x0 = (offsetToMidBaseline.x - boundsMinX) / (boundsMaxX - boundsMinX); // Character's position relative to the bounds of the mesh.
                float x1 = x0 + 0.0001f; // Magic number..?
                float y0 = VertexCurve.Evaluate(x0) * CurveScale; // Evaluate the VertexCurve at the time (float) x0
                float y1 = VertexCurve.Evaluate(x1) * CurveScale;

                print(VertexCurve.Evaluate(1.0f));

                Vector3 horizontal = new Vector3(1, 0, 0);
                //Vector3 normal = new Vector3(-(y1 - y0), (x1 * (boundsMaxX - boundsMinX) + boundsMinX) - offsetToMidBaseline.x, 0);
                // Tangent should be indicating just how much a letter needs to be rotated in order to look good on the curve.
                // Tangent line that intercepts curve in a single spot..?
                Vector3 tangent = new Vector3(x1 * (boundsMaxX - boundsMinX) + boundsMinX, y1) - new Vector3(offsetToMidBaseline.x, y0);
                //print("Tangent " + tangent.x + " | " + tangent.y + " | " + tangent.z);

                // Magic number here comes from hardcoding a value that makes the letter make sense with the curve displayed in the
                // unmodified example. In this case we need to make it work with a round curve in the Z-axis.
                float dot = Mathf.Acos(Vector3.Dot(horizontal, tangent.normalized)) * 57.2957795f;
                Vector3 cross = Vector3.Cross(horizontal, tangent);

                //if (i == 0)
                //{
                //    //print("cross " + cross + " | " + horizontal + " | " + tangent);
                //    print("cross " + i + "(" + cross.x + " | " + cross.y + " | " + cross.z + ")");
                //    print("tangent" + i + "(" + tangent.x + " | " + tangent.y + " | " + tangent.z +")");
                //}
                // Angle: The angle of which each individual character should be rotated along the Y-axis
                float angle = cross.z > 0 ? dot : 360 - dot;

                //print(angle + " | " + cross.z + " | " + dot);
                //matrix = Matrix4x4.TRS(new Vector3(0, y0, 0), Quaternion.Euler(0, 0, angle), Vector3.one);
                //print(angle);
                matrix = Matrix4x4.TRS(new Vector3(0, 0, y0), Quaternion.Euler(0, tangent.z, 0), Vector3.one); // Z "rotation"
                // TRANSLATION/POSITION = Placement along the Z-axis when (0, 0, VALUE)
                // ROTATION = Rotation along the Y-axis (0, Value, 0)
                // SCALE = No change needed.


                vertices[vertexIndex + 0] = matrix.MultiplyPoint3x4(vertices[vertexIndex + 0]);
                vertices[vertexIndex + 1] = matrix.MultiplyPoint3x4(vertices[vertexIndex + 1]);
                vertices[vertexIndex + 2] = matrix.MultiplyPoint3x4(vertices[vertexIndex + 2]);
                vertices[vertexIndex + 3] = matrix.MultiplyPoint3x4(vertices[vertexIndex + 3]);

                vertices[vertexIndex + 0] += offsetToMidBaseline;
                vertices[vertexIndex + 1] += offsetToMidBaseline;
                vertices[vertexIndex + 2] += offsetToMidBaseline;
                vertices[vertexIndex + 3] += offsetToMidBaseline;
            }


            // Upload the mesh with the revised information
            m_TextComponent.UpdateVertexData();

            //yield return new WaitForSeconds(0.025f);
            yield return null;
        }
    }

}
