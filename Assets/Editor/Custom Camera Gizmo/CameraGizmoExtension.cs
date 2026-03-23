using UnityEditor;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.IO;

public class CameraGizmoExtension
{
    public static Mesh GizmoMesh;
    public static bool IsGizmoEnabled = true;
    public static Color GizmoColor = Color.black;
    public static Color FrustumColor = Color.black;
    public static Vector3 GizmoScale = new Vector3(1.0f, 1.0f, 1.0f);
    public static Vector3 GizmoRotationOffset = new Vector3(0.0f, -90f, 0.0f);

    [DrawGizmo(GizmoType.NonSelected | GizmoType.Selected | GizmoType.Pickable)]
    public static void RenderCustomCameraGizmo(Camera CameraTarget, GizmoType CurrentGizmoType)
    {
        if (!IsGizmoEnabled)
        {
            return;
        }
        if (GizmoMesh == null)
        {
            string[] Guids = AssetDatabase.FindAssets("CameraGizmoExtension t:Script");
            if (Guids.Length > 0)
            {
                string ScriptPath = AssetDatabase.GUIDToAssetPath(Guids[0]);
                string FolderPath = Path.GetDirectoryName(ScriptPath);
                string FullMeshPath = Path.Combine(FolderPath, "CAMERA GIZMO.fbx").Replace("\\", "/");
                GizmoMesh = AssetDatabase.LoadAssetAtPath<Mesh>(FullMeshPath);
            }
        }
        if (GizmoMesh == null)
        {
            return;
        }
        Gizmos.color = GizmoColor;
        Vector3 CameraPosition = CameraTarget.transform.position;
        Quaternion CameraRotation = CameraTarget.transform.rotation;
        Quaternion CorrectedRotation = CameraRotation * Quaternion.Euler(GizmoRotationOffset);
        float SizeMultiplier = HandleUtility.GetHandleSize(CameraPosition);
        Vector3 FinalScale = new Vector3(
            GizmoScale.x * SizeMultiplier,
            GizmoScale.y * SizeMultiplier,
            GizmoScale.z * SizeMultiplier
        );
        Gizmos.DrawMesh(
            GizmoMesh,
            CameraPosition,
            CorrectedRotation,
            FinalScale
        );
        DrawCameraFrustum(CameraTarget);
    }
    public static void DrawCameraFrustum(Camera CameraTarget)
    {
        Matrix4x4 OriginalMatrix = Gizmos.matrix;
        Gizmos.matrix = CameraTarget.transform.localToWorldMatrix;
        Gizmos.color = FrustumColor;
        Gizmos.DrawFrustum(
            Vector3.zero,
            CameraTarget.fieldOfView,
            CameraTarget.farClipPlane,
            CameraTarget.nearClipPlane,
            CameraTarget.aspect
        );
        Gizmos.matrix = OriginalMatrix;
    }
}
[Overlay(typeof(SceneView), "Camera Gizmo Controller", true)]
public class CameraGizmoOverlay : Overlay
{
    public Slider SpeedSlider;
    public FloatField SpeedField;
    public Toggle GizmoToggle;
    public ColorField CameraColorField;
    public ColorField CameraFrustrumColorField;
    public bool IsMouseOver;
    public VisualElement Root;
    public override VisualElement CreatePanelContent()
    {
        Root = new VisualElement();
        Root.style.width = 320;
        Root.style.paddingTop = 5;
        Root.style.paddingBottom = 5;
        Root.style.paddingLeft = 5;
        Root.style.paddingRight = 5;
        LoadSettings();    
        GizmoToggle = new Toggle("Enable Custom Gizmo");
        GizmoToggle.value = CameraGizmoExtension.IsGizmoEnabled;
        GizmoToggle.RegisterValueChangedCallback(Evt =>
        {
            CameraGizmoExtension.IsGizmoEnabled = Evt.newValue;
            EditorPrefs.SetBool("CamGizmo_Enabled", Evt.newValue);
            SceneView.RepaintAll();
        });
        Root.Add(GizmoToggle);
        CameraColorField = new ColorField("Camera Color");
        CameraColorField.value = CameraGizmoExtension.GizmoColor;
        CameraColorField.RegisterValueChangedCallback(Evt =>
        {
            CameraGizmoExtension.GizmoColor = Evt.newValue;
            SaveColor("CamGizmo_Color", Evt.newValue);
            SceneView.RepaintAll();
        });
        Root.Add(CameraColorField);
        CameraFrustrumColorField = new ColorField("Camera Frustrum Color");
        CameraFrustrumColorField.value = CameraGizmoExtension.FrustumColor;
        CameraFrustrumColorField.RegisterValueChangedCallback(Evt =>
        {
            CameraGizmoExtension.FrustumColor = Evt.newValue;
            SaveColor("CamGizmo_FrustumColor", Evt.newValue);
            SceneView.RepaintAll();
        });
        Root.Add(CameraFrustrumColorField);
        VisualElement SpeedRow = new VisualElement();
        SpeedRow.style.flexDirection = FlexDirection.Row;
        SpeedRow.style.alignItems = Align.Center;
        SpeedRow.style.marginTop = 5;
        SpeedField = new FloatField("Speed");
        SpeedField.style.width = 80;
        SpeedField.style.marginRight = 5;
        SpeedSlider = new Slider(0.01f, 1000.0f);
        SpeedSlider.style.flexGrow = 1;
        if (SceneView.lastActiveSceneView != null)
        {
            float CurrentSpeed = SceneView.lastActiveSceneView.cameraSettings.speed;
            SpeedSlider.SetValueWithoutNotify(CurrentSpeed);
            SpeedField.SetValueWithoutNotify(CurrentSpeed);
        }
        SpeedSlider.RegisterValueChangedCallback(Evt =>
        {
            UpdateCameraSpeed(Evt.newValue);
            SpeedField.SetValueWithoutNotify(Evt.newValue);
        });
        SpeedField.RegisterValueChangedCallback(Evt =>
        {
            UpdateCameraSpeed(Evt.newValue);
            SpeedSlider.SetValueWithoutNotify(Evt.newValue);
        });
        SpeedRow.Add(SpeedField);
        SpeedRow.Add(SpeedSlider);
        Root.Add(SpeedRow);
        Root.RegisterCallback<MouseEnterEvent>(Evt => { IsMouseOver = true; });
        Root.RegisterCallback<MouseLeaveEvent>(Evt => { IsMouseOver = false; });
        Root.schedule.Execute(UpdateUIFromCamera).Every(50);
        return Root;
    }
    public void UpdateCameraSpeed(float NewSpeed)
    {
        if (SceneView.lastActiveSceneView != null)
        {
            SceneView.lastActiveSceneView.cameraSettings.speedMin = 0.1f;
            SceneView.lastActiveSceneView.cameraSettings.speedMax = 10000.0f;
            SceneView.lastActiveSceneView.cameraSettings.speed = NewSpeed;
        }
    }
    public void UpdateUIFromCamera()
    {
        if (IsMouseOver) return;
        if (SceneView.lastActiveSceneView != null && SpeedSlider != null && SpeedField != null)
        {
            float CameraSpeed = SceneView.lastActiveSceneView.cameraSettings.speed;
            if (!Mathf.Approximately(SpeedField.value, CameraSpeed))
            {
                SpeedField.SetValueWithoutNotify(CameraSpeed);
                SpeedSlider.SetValueWithoutNotify(CameraSpeed);
            }
        }
    }
    public void SaveColor(string Key, Color TargetColor)
    {
        EditorPrefs.SetString(Key, "#" + ColorUtility.ToHtmlStringRGBA(TargetColor));
    }
    public void LoadSettings()
    {
        CameraGizmoExtension.IsGizmoEnabled = EditorPrefs.GetBool("CamGizmo_Enabled", true);
        string CamHex = EditorPrefs.GetString("CamGizmo_Color", "#000000");
        if (ColorUtility.TryParseHtmlString(CamHex, out Color CamCol))
        {
            CameraGizmoExtension.GizmoColor = CamCol;
        }
        string FrustumHex = EditorPrefs.GetString("CamGizmo_FrustumColor", "#000000");
        if (ColorUtility.TryParseHtmlString(FrustumHex, out Color FrusCol))
        {
            CameraGizmoExtension.FrustumColor = FrusCol;
        }
    }
}
