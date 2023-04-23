// author:KIPKIPS
// date:2023.04.08 19:55
// describe:Camera代理
using Framework.Manager;
using UnityEngine;

namespace Framework {
    public enum CameraType {
        UI,
        Main
    }
    public static class LCamera {
        public static Transform GetCameraRoot(CameraType cameraType) {
            return cameraType switch {
                CameraType.UI => CameraManager.Instance.UICameraRoot,
                CameraType.Main => CameraManager.Instance.MainCameraRoot,
                _ => null
            };
        }
        public static Camera GetCamera(CameraType cameraType) {
            return cameraType switch {
                CameraType.UI => CameraManager.Instance.UICamera,
                CameraType.Main => CameraManager.Instance.MainCamera,
                _ => null
            };
        }
    }
}