using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
[ExecuteInEditMode]
public class RaymarchCam : MonoBehaviour
{
    [SerializeField]
    public Shader shader;
    public Material raymarchMaterial{
        get{
            if(!_raymarchMaterial && shader){
                _raymarchMaterial = new Material(shader);
                _raymarchMaterial.hideFlags = HideFlags.HideAndDontSave;
            }
            return _raymarchMaterial;
        }
    }
    private Material _raymarchMaterial;
    public Camera camera{
        get{
            if(!_camera){
                _camera = GetComponent<Camera>();
            }
            return _camera;
        }
    }
    private Camera _camera;

    private void OnRenderImage(RenderTexture src, RenderTexture dest) {
        if(!_raymarchMaterial){
            Graphics.Blit(src, dest);
            return;
        }
        
        raymarchMaterial.SetMatrix("_CameraFrustum", CameraFrustum(camera));
        raymarchMaterial.SetMatrix("_CamToworld", camera.cameraToWorldMatrix);
        raymarchMaterial.SetVector("_CameraWorldSpace", camera.transform.position);

        RenderTexture.active = dest;
        GL.PushMatrix();
        GL.LoadOrtho();
        _raymarchMaterial.SetPass(0);
        GL.Begin(GL.QUADS);

        // Bottom Left
        GL.MultiTexCoord2(0, 0.0f, 0.0f);
        GL.Vertex3(0.0f, 0.0f, 3.0f);
        // Bottom Right
        GL.MultiTexCoord2(0, 1.0f, 0.0f);
        GL.Vertex3(1.0f, 0.0f, 2.0f);
        // Top Right
        GL.MultiTexCoord2(0, 1.0f, 1.0f);
        GL.Vertex3(1.0f, 1.0f, 1.0f);
        // Top Left
        GL.MultiTexCoord2(0, 0.0f, 1.0f);
        GL.Vertex3(0.0f, 1.0f, 0.0f);

        GL.End();
        GL.PopMatrix();
    }

    private Matrix4x4 CameraFrustum(Camera camera){
        Matrix4x4 frustum = Matrix4x4.identity;
        float fov = Mathf.Tan((camera.fieldOfView * 0.5f) * Mathf.Deg2Rad);
        
        Vector3 goUp = Vector3.up * fov;
        Vector3 goRight = Vector2.right * fov * camera.aspect;

        Vector3 topLeft = (-Vector3.forward - goRight + goUp);
        Vector3 topRight = (-Vector3.forward + goRight + goUp);
        Vector3 bottomRight = (-Vector3.forward + goRight - goUp);
        Vector3 bottomLeft = (-Vector3.forward - goRight - goUp);

        frustum.SetRow(0, topLeft);
        frustum.SetRow(1, topRight);
        frustum.SetRow(2, bottomRight);
        frustum.SetRow(3, bottomLeft);

        return frustum;
    }
}
