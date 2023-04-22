// --[[
//     author:{wkp}
//     time:19:55
// ]]
using Framework.Manager;
using UnityEngine;

namespace Framework {
    public enum CameraType {
        UI,Main,
    }
    public static class LCamera {
        public static Transform GetCameraRoot(CameraType cameraType) {
            switch (cameraType) {
                case CameraType.UI:
                    return CameraManager.Instance.UICameraRoot;
                case CameraType.Main:
                    return CameraManager.Instance.MainCameraRoot;
            }
            return null;
        }
        public static Camera GetCamera(CameraType cameraType) {
            switch (cameraType) {
                case CameraType.UI:
                    return CameraManager.Instance.UICamera;
                case CameraType.Main:
                    return CameraManager.Instance.MainCamera;
            }
            return null;
        }
    }
}