// author:KIPKIPS
// date:2023.04.08 20:01
// describe:UI框架管理器
using System.Collections;
using System.Collections.Generic;
using Framework.Singleton;
using UnityEditor;
using UnityEngine;

namespace Framework.UI {
    public class UIManager :MonoSingleton<UIManager> {
        public void Launch() {
            AnalysisWindow();
            InitUICamera();
            LoadConfig();
        }
        private void LoadConfig() {
            
        }
        private const string LOGTag = "UIManager";
        private readonly Stack<BaseWindow> _windowStack = new();
        private readonly Dictionary<int, BaseWindow> _baseWindowDict = new();
        private readonly Dictionary<int, PageConfig> _windowDict = new ();
        private static Transform UICameraRoot => UICamera.transform;
        private Camera _uiCamera;
        private static Camera UICamera => Utils.Find<Camera>("[UICamera]");
        private void InitUICamera() {
            DontDestroyOnLoad(UICameraRoot);
        }

        public void Open(string windowName, dynamic options = null) {
            PushWindow(windowName, options);
        }
        private BaseWindow GetWindow(string pageName) {
            BaseWindow window;
            _baseWindowDict.TryGetValue(id, out window);
            if (window != null) {
                return window;
            } else {
                string path = _windowDict[id].path;
                GameObject windowObj = null;
                windowObj = Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/ResourcesAssets/" + _windowDict[id].path + ".prefab"), UICameraRoot);
                windowObj.name = _windowDict[id].name;
                window = windowObj.transform.GetComponent<BaseWindow>();
                window.WindowId = (UIWindow)id;
                _baseWindowDict.Add(id, window);
                return window;
            }
        }
        private void PushWindow(string pageName, dynamic options = null) {
            BaseWindow window = GetWindow(pageName);
            Utils.Log("Open Window === ", window.name);
            window.Canvas.sortingOrder = _windowDict[windowId].layer;
            window.Canvas.worldCamera = UICamera;
            if (window.IsShow) {
                return;
            }

            //显示当前界面时,应该先去判断当前栈是否为空,不为空说明当前有界面显示,把当前的界面OnPause掉
            if (_windowStack.Count > 0) {
                _windowStack.Peek().OnPause();
            }

            //每次入栈(显示页面的时候),触发window的OnEnter方法
            window.OnEnter(options);
            _windowStack.Push(window);
        }
        public void Close(string pageName) {
            BaseWindow window = GetWindow(pageName);
            Utils.Log("Close Window === ", window.name);
            window.OnExit();
        }
        public void PopWindow() {
            if (_windowStack.Count > 0) {
                _windowStack.Pop(); //关闭栈顶界面
                // Destroy(window.gameObject);
                if (_windowStack.Count > 0) {
                    _windowStack.Peek().OnResume(); //恢复原先的界面
                }
            }
        }
        private void AnalysisWindow() {
            WindowMap windowMap = null;
#if UNITY_EDITOR
            windowMap = AssetDatabase.LoadAssetAtPath<WindowMap>("Assets/ResourcesAssets/RuntimeDepend/UIFramework/WindowMap.asset");
#else
            windowMap = ResourcesLoadManager.Instance.LoadFromFile<WindowMap>("RuntimeDepend","WindowMap".ToLower());
#endif
            foreach (WindowData windowData in windowMap.windowDataList) {
                windowData.AssetTag = windowData.path.Replace("/" + windowData.name, "");
                _windowDict.Add((int)windowData.id, windowData);
            }
            Utils.Log(LOGTag, "Window data is parsed");
        }
    }
}