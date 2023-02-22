using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    private Mesh mesh;
    private float fov;
    [SerializeField] private Vector3 origin;
    [SerializeField] private float startingAngle;

    [SerializeField] private LayerMask layerMask;

    private void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        fov = 90f;
        origin = Vector3.zero;
    }

    private void LateUpdate() {
        int rayCount = 100;
        float angle = startingAngle;
        float angleIncrease = fov / rayCount;
        float viewDistance = 20f;

        Vector3[] vertices = new Vector3[2 * rayCount + 1];
        Color[] colors = new Color[vertices.Length];
        //Vector2[] uv = new Vector2[vertices.Length];
        int[] triangles = new int[rayCount * 3];

        //vertices[0] = origin;
        //colors[0] = new Color(1f, 0f, 0f, 1f);

        int vertexIndex = 0;
        int triangleIndex = 0;
        for (int i = 0; i <= rayCount; i++) {
            Vector3 vertex;

            RaycastHit2D raycastHit2D = Physics2D.Raycast(origin, AngleToVector(angle), viewDistance, layerMask);
            if (raycastHit2D.collider == null){
                // No hit
                vertex = origin + AngleToVector(angle) * viewDistance;
            } else {
                // Hit object
                vertex = raycastHit2D.point;
            }
            vertices[i] = vertex;
            colors[i] = GetColor(i, rayCount);

            if (i > 0) {
                Vector3 vertexOrigin = origin;
                vertices[rayCount + i] = vertexOrigin;
                colors[rayCount + i] = GetColor(i, rayCount);

                triangles[triangleIndex + 0] = rayCount + i;
                triangles[triangleIndex + 1] = i - 1;
                triangles[triangleIndex + 2] = i;

                triangleIndex += 3;
            }
            
            vertexIndex ++;
            angle -= angleIncrease;
        }

        mesh.vertices = vertices;
        mesh.colors = colors;
        //mesh.uv = uv;
        mesh.triangles = triangles;

        mesh.RecalculateBounds();
    }

    public void SetOrigin(Vector3 origin){
        this.origin = origin;
    }

    public void SetAimDirection(float aimAngle){
        //startingAngle = VectorToAngle(aimDirection) - fov / 2f;
        startingAngle = aimAngle;
    }

    Vector3 AngleToVector(float angle)
    {
        return new Vector3(Mathf.Cos(Mathf.Deg2Rad * angle), Mathf.Sin(Mathf.Deg2Rad * angle));
    }

    /*Vector3 VectorToAngle(Vector3 dir){
        float n = Mathf.Atan2(dir.y, dir.x) * Math.Rad2Deg;
        return n < 0 ? n + 360 : n;
    }*/

    Color GetColor(int i, int rayCount){
        float scale = 0.5f;
        float a = Mathf.Pow(Mathf.Abs(1f - (float) (2 * i) / rayCount), 5f);
        float neg = scale - scale * a;
        float alpha = 0.31f + a * 0.4f;
        return new Color(neg, neg, 0.8f * neg, alpha);
    }

}
