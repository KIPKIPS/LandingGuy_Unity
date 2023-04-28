// author:KIPKIPS
// date:2023.04.28 10:37
// describe:模型挂载器
using System;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

namespace Framework.UI {
    [AddComponentMenu("LUI/LModelContainer", 10),RequireComponent(typeof(LRawImage))]
    public class LModelContainer:MonoBehaviour {
        private string _modePath;
        public Vector2Int size;
        private const int DepthBits = 0;
        private RenderTexture _renderTexture;
        private RenderTexture RenderTexture {
            get {
                if (_renderTexture == null) {
                    _renderTexture = new RenderTexture(size.x,size.y,DepthBits,RenderTextureFormat.ARGB32,RenderTextureReadWrite.Default) {
                        antiAliasing = (int)AntialiasingQuality.Low,
                        depthStencilFormat = GraphicsFormat.D24_UNorm,
                        useMipMap = false
                    };
                }
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
                t.localPosition = Vector3.zero;
                t.localRotation = quaternion.identity;
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
        public void LoadModel(string modelPath) {
            if (_modePath == modelPath) return;
            _modePath = modelPath;
            PrefabMounter.SetModelPath(modelPath);
        }
    }
}