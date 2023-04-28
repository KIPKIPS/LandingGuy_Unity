// --[[
//     author:{wkp}
//     time:14:08
// ]]
using System;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

namespace Framework.UI {
    public class PrefabMounter:MonoBehaviour {
        private Camera _camera;
        private GameObject _cameraGo;
        private GameObject CameraGo {
            get {
                if (_cameraGo != null) return _cameraGo;
                _cameraGo = new GameObject("Camera");
                var t = _cameraGo.transform;
                t.SetParent(transform);
                t.localPosition = Vector3.zero;
                t.localRotation = quaternion.identity;
                t.localScale = Vector3.one;
                return _cameraGo;
            }
        }
        public Camera Camera {
            get {
                if (_camera != null) return _camera;
                _camera = CameraGo.AddComponent<Camera>();
                _camera.clearFlags = CameraClearFlags.SolidColor;
                _camera.cullingMask = LayerMask.NameToLayer("Default") | LayerMask.NameToLayer("3DUI");
                // _camera.targetTexture = RenderTexture;
                return _camera;
            }
        }
        public void SetRenderTexture(RenderTexture rt) {
            Camera.targetTexture = rt;
        }
        private void OnDestroy() {
            if (_prefabGo != null) {
                DestroyImmediate(_prefabGo);
            }
        }
        private GameObject _prefabGo;
        public void SetModelPath(string modelPath) {
            if (_prefabGo != null) {
                DestroyImmediate(_prefabGo);
            }
            var go = AssetDatabase.LoadAssetAtPath<GameObject>(modelPath);
            _prefabGo = Instantiate(go, Vector3.zero, quaternion.identity, transform);
            _prefabGo.transform.localScale = Vector3.one;
        }
    }
}