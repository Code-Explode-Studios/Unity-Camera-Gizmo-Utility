using UnityEditor;
using UnityEngine;
using System.IO;

public class CameraGizmoExtension
{
    public static Mesh GizmoMesh;
    public static Color GizmoColor = Color.black;
    public static Color FrustumColor = Color.black;
    public static Vector3 GizmoScale = new Vector3(1.0f, 1.0f, 1.0f);
    public static Vector3 GizmoRotationOffset = new Vector3(0.0f, -90f, 0.0f);

    [DrawGizmo(GizmoType.NonSelected | GizmoType.Selected | GizmoType.Pickable)]
    public static void RenderCustomCameraGizmo(Camera CameraTarget, GizmoType CurrentGizmoType)
    {        
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