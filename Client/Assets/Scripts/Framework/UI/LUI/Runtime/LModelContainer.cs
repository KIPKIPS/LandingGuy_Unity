// author:KIPKIPS
// date:2023.04.28 10:37
// describe:模型挂载器
using Framework.Manager;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

namespace Framework.UI {
    [AddComponentMenu("LUI/LModelContainer", 10),RequireComponent(typeof(LRawImage))]
    public class LModelContainer:MonoBehaviour {
        private string _modePath;
        private RectTransform _rectTransform;
        private RectTransform RectTransform => _rectTransform ??= GetComponent<RectTransform>();
        public Vector2Int size;
        private const int DepthBits = 0;
        private RenderTexture _renderTexture;
        private RenderTexture RenderTexture {
            get {
                if (_renderTexture != null) return _renderTexture;
                size = new Vector2Int(size.x == 0 ? (int)RectTransform.rect.width : size.x, size.y == 0 ? (int)RectTransform.rect.height : size.y);
                _renderTexture = new RenderTexture(size.x,size.y,DepthBits,RenderTextureFormat.ARGB32,RenderTextureReadWrite.Default) {
                    antiAliasing = 1,
                    depthStencilFormat = GraphicsFormat.D24_UNorm,
                    useMipMap = false,
                    name = "TempBuffer"
                };
                return _renderTexture;
            }
        }
        private PrefabMounter _prefabMounter;
        private PrefabMounter PrefabMounter {
            get {
                if (_prefabMounter != null) return _prefabMounter;
                var go = new GameObject("Mounter");
                _prefabMounter = go.AddComponent<PrefabMounter>();
                _prefabMounter.SetRenderTexture(RenderTexture);
                var t = go.transform;
                t.SetParent(MountManager.Instance.ModelMountRoot);
                t.localPosition = Vector3.zero;
                t.localRotation = Quaternion.identity;
                t.localScale = Vector3.one;
                return _prefabMounter;
            }
        }
        private LRawImage _rawImage;
        private LRawImage RawImage => _rawImage ??= GetComponent<LRawImage>();
        private void OnEnable() {
            if (RawImage) {
                RawImage.texture = RenderTexture;
            }
        }
        private void OnDestroy() {
            DestroyImmediate(_prefabMounter.gameObject);
            DestroyImmediate(_renderTexture);
        }
        public void LoadModel(string modelPath) {
            if (_modePath == modelPath) return;
            _modePath = modelPath;
            PrefabMounter.SetModelPath(modelPath);
        }
        public void SetModelLocalRotation(Vector3 v) {
            PrefabMounter.SetModelLocalRotation(v);
        }
        public void SetModelLocalPosition(Vector3 v) {
            PrefabMounter.SetModelLocalPosition(v);
        }
        public void SetModelLocalScale(Vector3 v) {
            PrefabMounter.SetModelLocalScale(v);
        }
        
    }
}