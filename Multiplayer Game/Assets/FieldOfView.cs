using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    private Mesh mesh;
    private float fov;
    private Vector3 origin;
    private float startingAngle;

    [SerializeField] private LayerMask layerMask;

    private void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        fov = 70f;
        origin = Vector3.zero;
    }

    private void LateUpdate() {
        int rayCount = 50;
        float angle = startingAngle;
        float angleIncrease = fov / rayCount;
        float viewDistance = 20f;

        Vector3[] vertices = new Vector3[rayCount + 1 + 1];
        Color[] colors = new Color[vertices.Length];
        Vector2[] uv = new Vector2[vertices.Length];
        int[] triangles = new int[rayCount * 3];

        vertices[0] = origin;
        colors[0] = new Color(0f, 0f, 0f, 1f);

        int vertexIndex = 1;
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
            vertices[vertexIndex] = vertex;
            colors[vertexIndex] = new Color(0f, 0f, 0f, 1f);

            if (i > 0) {
                triangles[triangleIndex + 0] = 0;
                triangles[triangleIndex + 1] = vertexIndex - 1;
                triangles[triangleIndex + 2] = vertexIndex;

                triangleIndex += 3;
            }
            
            vertexIndex ++;
            angle -= angleIncrease;
        }

        mesh.vertices = vertices;
        mesh.colors = colors;
        mesh.uv = uv;
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

}
