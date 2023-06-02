using UnityEngine;
using UnityEditor;

namespace CMF {
    //This editor script displays some additional information in the mover inspector, like a preview of the current raycast array;
    [CustomEditor(typeof(Mover))]
    public class MoverInspector : Editor {
        private Mover _mover;

        private void Reset() {
            Setup();
        }

        private void OnEnable() {
            Setup();
        }

        private void Setup() {
            //Get reference to mover component;
            _mover = (Mover)target;
        }

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            DrawRaycastArrayPreview();
        }

        //Draw preview of raycast array in inspector;
        private void DrawRaycastArrayPreview() {
            if (_mover.sensorType != Sensor.CastType.RaycastArray) return;
            GUILayout.Space(5);
            var space = GUILayoutUtility.GetRect(GUIContent.none, GUIStyle.none, GUILayout.Height(100));
            var background = new Rect(space.x + (space.width - space.height) / 2f, space.y, space.height, space.height);
            EditorGUI.DrawRect(background, Color.grey);
            const float pointSize = 3f;
            var previewPositions = _mover.raycastArrayPreviewPositions;
            var center = new Vector2(background.x + background.width / 2f, background.y + background.height / 2f);
            if (previewPositions != null && previewPositions.Length != 0) {
                for (var i = 0; i < previewPositions.Length; i++) {
                    var position = center + new Vector2(previewPositions[i].x, previewPositions[i].z) * background.width / 2f * 0.9f;
                    EditorGUI.DrawRect(new Rect(position.x - pointSize / 2f, position.y - pointSize / 2f, pointSize, pointSize), Color.white);
                }
            }
            if (previewPositions != null && previewPositions.Length != 0) GUILayout.Label("Number of rays = " + previewPositions.Length, EditorStyles.centeredGreyMiniLabel);
        }
    }
}