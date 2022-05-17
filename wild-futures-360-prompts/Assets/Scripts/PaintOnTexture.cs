using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEditor;

public class PaintOnTexture : MonoBehaviour
{
    public StreetViewCamera streetViewCamera;

    public RenderTexture paintTex;
    public Renderer paintRend;

    public RenderTexture backgroundTex;
    public Renderer backgroundRend;


    [Range(1.1f, 10.0f)]
    public float brushRadius = 1.1f;
    public Color brushColor;


    private Camera cam;
    private Material _paintMaterial;
    private Material _backgroundMaterial;


    private Vector3 _gizmoRayStart = Vector3.zero;
    private Vector3 _gizmoRayTarget = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        // get all the private variables
        // paintRend = GetComponent<Renderer>();

        cam = streetViewCamera.cam;

        brushColor = Color.clear;

        _paintMaterial = paintRend.material;
        _paintMaterial.SetVector("_Color", brushColor);
        _paintMaterial.SetFloat("_BrushRadius", brushRadius);

        _backgroundMaterial = backgroundRend.material;


        // Create a new render texture
        // https://www.youtube.com/watch?v=-yaqhzX-7qo
        // https://docs.unity3d.com/ScriptReference/RenderTexture-ctor.html
        // R channel -> semantic index
        // G channel -> instance index
        // B channel -> ???
        // A channel -> mask
        // _renderTex = new RenderTexture(renderTex){
        //     filterMode = FilterMode.Point
        // };

        // Asign the textures to the mesh renderers
        _paintMaterial.SetTexture("_MainTex", paintTex);
        _backgroundMaterial.SetTexture("_MainTex", backgroundTex);
    }

    // Update is called once per frame
    void Update()
    {
        bool isShiftKeyDown = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

        if (!Input.GetMouseButton(0) || !isShiftKeyDown) {
            _gizmoRayTarget = Vector3.zero;
            return;
        }

        if (EventSystem.current.IsPointerOverGameObject()) {
            return;
        }

        if(cam == null) {
            Debug.Log("wheres the camera?");
            return;
        } 

        _paintMaterial.SetVector("_Color", brushColor);
        _paintMaterial.SetFloat("_BrushRadius", brushRadius);

        Vector3 mousePos = Input.mousePosition;

        Ray ray = cam.ScreenPointToRay(mousePos);
        RaycastHit hit;

        ray.origin = ray.GetPoint(100);
        ray.direction = -ray.direction;

        if (!Physics.Raycast(ray, out hit)) {
            return;
        }

        Vector2 pixelUV = sphereRayCast(cam, mousePos);
        drawOnRenderTexture(pixelUV);
    }

    Vector2 sphereRayCast(Camera cam, Vector3 target) {
        Ray ray = cam.ScreenPointToRay(target);
        RaycastHit hit;

        ray.origin = ray.GetPoint(100);
        ray.direction = -ray.direction;

        if (!Physics.Raycast(ray, out hit)) {
            return new Vector2(0,0);
        }
        _gizmoRayTarget = hit.point;

        Vector2 pixelUV = hit.textureCoord;
        return pixelUV;
    }

    Vector2Int uvToPixelCoordinates(Vector2 uv, Texture2D tex) {
        return new Vector2Int((int)(uv.x * tex.width), (int)(uv.y * tex.height));
    }

    void drawOnTexture2D(Vector2Int centerCoord, int radius, Texture2D tex) {
        for (int i = -radius; i < brushRadius; i++) {
            for (int j = -radius; j < brushRadius; j++) {
                tex.SetPixel(centerCoord.x+i, centerCoord.y+j, Color.white);
            }
        }
        tex.Apply();
    }

    void drawOnRenderTexture(Vector2 pixelUV) {
        _paintMaterial.SetVector("_Coordinate", new Vector4(pixelUV.x, pixelUV.y, 0, 0));
        RenderTexture temp = RenderTexture.GetTemporary(paintTex.width, paintTex.height, 0, paintTex.format);
        Graphics.Blit(paintTex, temp);
        Graphics.Blit(temp, paintTex, _paintMaterial);
        RenderTexture.ReleaseTemporary(temp);
    }

    public void addPaintToBackgroundRenderTexture() {
        // _paintMaterial.SetVector("_Coordinate", new Vector4(pixelUV.x, pixelUV.y, 0, 0));
        RenderTexture temp = RenderTexture.GetTemporary(paintTex.width, paintTex.height, 0, paintTex.format);
        Graphics.Blit(paintTex, temp);
        Graphics.Blit(temp, backgroundTex, _backgroundMaterial);
        RenderTexture.ReleaseTemporary(temp);

        clearRenderTex(paintTex);
    }

    public void clearRenderTex(RenderTexture tex) {
        _paintMaterial.SetVector("_Coordinate", new Vector4(-1, -1, 0, 0));
        tex.Release();
        Debug.Log(tex.width);
    }

    public void selectColor(Color color) {
        brushColor = color;
    }

    public void setBrushRadius(float radius) {
        brushRadius = radius;
    }

    // public void SaveTexture () {
    //     Debug.Log("Saving Texture!");
    //     byte[] bytes = toTexture2D(_renderTex).EncodeToPNG();
    //     string fileName = System.DateTime.Now.ToString("yyyy-MM-dd\\THH-mm-ss\\Z");
    //     File.WriteAllBytes(Application.dataPath + "/Textures/Generated/" + fileName + ".png", bytes);
    // }
    // Texture2D toTexture2D(RenderTexture rTex)
    // {
    //     Texture2D tex = new Texture2D(_renderTex.width, _renderTex.height, TextureFormat.RGBA32, false);
    //     RenderTexture.active = rTex;
    //     tex.ReadPixels(new Rect(0, 0, rTex.width, rTex.height), 0, 0);
    //     tex.Apply();
    //     return tex;
    // }

    void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Vector3 source = ( cam != null ) ? cam.transform.position : Vector3.zero;
        Vector3 direction = (_gizmoRayTarget - source) * 5;
        Gizmos.DrawRay(source, direction);
        Gizmos.DrawSphere(_gizmoRayTarget, 0.01f);
    }

    private void OnGUI() {
        GUI.DrawTexture(new Rect(0, 0, 192, 192/2), paintTex, ScaleMode.ScaleToFit, false);
        GUI.DrawTexture(new Rect(0, 192/2, 192, 192/2), backgroundTex, ScaleMode.ScaleToFit, false);
        DrawCircleBrush(Color.black, brushRadius*6f);
        
    }

    private static void DrawCircleBrush(Color _color, float _size)
    {
        Handles.color = _color;
        // Circle
        Handles.CircleHandleCap(0, Event.current.mousePosition, Quaternion.identity, _size, EventType.Repaint);
        // Cross Center
        Handles.DrawLine(Event.current.mousePosition + Vector2.left, Event.current.mousePosition + Vector2.right);
        Handles.DrawLine(Event.current.mousePosition + Vector2.up, Event.current.mousePosition + Vector2.down);
    }
}
