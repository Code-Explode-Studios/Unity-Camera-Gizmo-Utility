# Unity Custom Camera Gizmo
A simple Editor extension that replaces the default Unity Camera gizmo with a custom 3D mesh and adds a persistent frustum visualization.

## Features
Custom Mesh Support: Uses a custom `.fbx` (designed in Blender) as the camera icon.
Auto-Scaling: Uses `HandleUtility.GetHandleSize` so the gizmo remains readable regardless of zoom level.
Persistent Frustum:** Draws the camera frustum even when the camera is not selected.

## Installation
1. Import this project in your Unity Project.
2. The script will automatically find the mesh and apply it to all cameras in the scene.

## Customization
You can adjust the following static variables in the script:
GizmoColor: Change the color of the 3D Mesh.
FrustumColor: Change the color of the frustum lines.
GizmoRotationOffset: Adjust if your Blender model is imported with different axes.
