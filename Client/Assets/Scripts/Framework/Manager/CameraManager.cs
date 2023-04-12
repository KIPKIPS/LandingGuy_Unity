// author:KIPKIPS
// date:2023.04.12 19:53
// describe:相机管理
using Framework.Singleton;
using UnityEngine;

namespace Framework.Manager {
    public class CameraManager:Singleton<CameraManager> {
        public Transform UICameraRoot => UICamera.transform;
        private Camera _uiCamera;
        public Camera UICamera => _uiCamera ??= Utils.Find<Camera>("[UICamera]");
        
        public Transform MainCameraRoot => MainCamera.transform;
        private Camera _mainCamera;
        public Camera MainCamera => _mainCamera ??= Utils.Find<Camera>("[MainCamera]");
    }
}