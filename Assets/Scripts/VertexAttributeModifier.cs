using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class VertexAttributeModifier : MonoBehaviour
{
    private TextMeshPro m_TextMeshPro;
    private TMP_TextInfo m_textInfo;
    private int LOOP_MAGIC_NUMBER = 10000;

    private struct VertexAnim
    {
        public float angleRange;
        public float angle;
        public float speed;
    }

    public void Awake()
    {
        m_TextMeshPro = GetComponent<TextMeshPro>();
        m_TextMeshPro.text = "This is just plain old text..";
        m_TextMeshPro.enableWordWrapping = true;
        m_TextMeshPro.colorGradient = new VertexGradient(Color.white, Color.white, Color.blue, Color.cyan);
        m_TextMeshPro.enableVertexGradient = true;

        m_TextMeshPro.ForceMeshUpdate();
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(AnimateVertexPositions());
    }

    IEnumerator AnimateVertexPositions()
    {
        Matrix4x4 matrix;
        Vector3[] verticies;

        int loopCount = 0;

        VertexAnim[] vertexAnim = new VertexAnim[1024]; // Amount of letters are still unknown.
        for (int i = 0; i < 1024; i++)
        {
            vertexAnim[i].angleRange = Random.Range(10f, 25f);
            vertexAnim[i].speed = Random.Range(1f, 3f);
        }

        while(loopCount < LOOP_MAGIC_NUMBER)
        {
            m_TextMeshPro.ForceMeshUpdate();
            verticies = m_TextMeshPro.mesh.vertices;

            int characterCount = m_TextMeshPro.textInfo.characterCount;

            for(int i = 0; i < characterCount; i++)
            {
                // setup initial random values
                VertexAnim vertAnim = vertexAnim[i];
                TMP_CharacterInfo charInfo = m_TextMeshPro.textInfo.characterInfo[i];

                if (!charInfo.isVisible)
                {
                    continue;
                }

                // first vertex of each character to know which one to manpiulate
                int vertexIndex = charInfo.vertexIndex;

                // calculate where the middle top line is for each character. Where should the characters pivot from
                // bottomRight + bottomLeft divided by 2 to get the middle between them.
                float bottomMiddle = verticies[vertexIndex + 0].x + verticies[vertexIndex + 2].x;

                // Pivot from the top
                float topRight = charInfo.topRight.y;

                Vector2 charMidTopline = new Vector2(bottomMiddle, topRight);

                // Offset the mesh based on the pivot point that was calculated above (charMidTopline)
                // Need to translate all 4 verticies of each quad to aligned with middle of character/baseline
                Vector3 offset = charMidTopline;
                verticies[vertexIndex + 0] += -offset;
                verticies[vertexIndex + 1] += -offset;
                verticies[vertexIndex + 2] += -offset;
                verticies[vertexIndex + 3] += -offset;

                // Compute angle of the rotation
                vertAnim.angle = Mathf.SmoothStep(-vertAnim.angleRange, vertAnim.angleRange, Mathf.PingPong(loopCount / 25f * vertAnim.speed, 1f));

                // Setup the matrix: Transform, Rotate and Scale
                // Only care about rotation in this case.
                matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0, 0, vertexAnim[i].angle), Vector3.one);

                // Perform the rotation operation on the verticies for each given letter.,
                verticies[vertexIndex + 0] = matrix.MultiplyPoint3x4(verticies[vertexIndex + 0]);
                verticies[vertexIndex + 1] = matrix.MultiplyPoint3x4(verticies[vertexIndex + 1]);
                verticies[vertexIndex + 2] = matrix.MultiplyPoint3x4(verticies[vertexIndex + 2]);
                verticies[vertexIndex + 3] = matrix.MultiplyPoint3x4(verticies[vertexIndex + 3]);

                // Reset pivot back to where it was.
                verticies[vertexIndex + 0] += offset;
                verticies[vertexIndex + 1] += offset;
                verticies[vertexIndex + 2] += offset;
                verticies[vertexIndex + 3] += offset;

                vertexAnim[i] = vertAnim;
            }

            loopCount++;

            m_TextMeshPro.UpdateVertexData();
            yield return new WaitForSeconds(0.025f);
        }
    }

    //// Update is called once per frame
    //void Update()
    //{
        
    //}
}
