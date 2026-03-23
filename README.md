<img width="1920" height="941" alt="image" src="https://github.com/user-attachments/assets/d8ec7470-3786-4af8-a7ab-aa41bbaf97c7" />









# Unity Custom Camera Gizmo & Speed Controller
A simple Editor extension that replaces the default Unity Camera gizmo with a custom 3D mesh, adds a persistent frustum visualization, and includes an interactive Scene View overlay for real time control.

## Features
Custom Mesh Support : Uses a custom `.fbx` (designed in Blender) as the camera icon.
Interactive Overlay : Toggle the gizmo, change colors, and adjust camera speed directly in the Scene View.
Integrated Speed Control : Built in slider and field to control the Scene View movement speed.
Auto-Scaling : Uses `HandleUtility.GetHandleSize` so the gizmo remains readable regardless of zoom level.
Persistent Settings : Automatically saves your colors, speed, and toggle state across Unity sessions.
Persistent Frustum : Draws the camera frustum even when the camera is not selected.

## Installation
1. Import this project into your Unity Project.
2. Ensure the script and the `CAMERA GIZMO.fbx` are in the same folder.
3. Open the Overlays menu in the Scene View (three-dot menu) and enable the Camera Gizmo Controller.

## Customization
Most settings are now adjusted via the Scene View overlay. However, you can still adjust the following static variables in the script:
GizmoScale: Change the base scale of the 3D Mesh.
GizmoRotationOffset: Adjust if your Blender model is imported with different axes.
