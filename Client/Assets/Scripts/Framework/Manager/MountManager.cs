// --[[
//     author:{wkp}
//     time:12:12
// ]]
using Framework.Singleton;
using Unity.Mathematics;
using UnityEngine;

namespace Framework.Manager {
    public class MountManager: Singleton<MountManager> {
        private Transform _modelMountRoot;
        public Transform ModelMountRoot {
            get {
                if (_modelMountRoot != null) return _modelMountRoot;
                var go = new GameObject {
                    name = "[ModelMountRoot]"
                };
                _modelMountRoot = go.transform;
                _modelMountRoot.localPosition = Vector3.zero;
                _modelMountRoot.localRotation = quaternion.identity;
                _modelMountRoot.localScale = Vector3.one;
                return _modelMountRoot;
            }
        }
    }
}