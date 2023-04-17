// author:KIPKIPS
// date:2023.04.08 20:01
// describe:UI框架管理器
using System;
using System.Collections;
using System.Collections.Generic;
using Framework.Manager;
using Framework.Singleton;
using UnityEditor;
using UnityEngine;

namespace Framework.UI {
    public class UIManager :MonoSingleton<UIManager> {
        private const string LOGTag = "UI";
        public const string ConfigAssetPath = "Assets/ResourcesAssets/UI/Config/pages.asset";//配置资源路径
        private PagesConfig _pagesConfigAsset;
        public void Launch() {
            UIBinding.Register();
            InitUICamera();
            LoadConfig();
        }
        private void LoadConfig() {
            _pagesConfigAsset = AssetDatabase.LoadAssetAtPath<PagesConfig>(ConfigAssetPath);
            foreach (var t in _pagesConfigAsset.configs) { 
                _pageName2IdMap.Add(t.pageName,t.pageID);
            }
        }
        private PageConfig GetConfig(string pageName) {
            _pageName2IdMap.TryGetValue(pageName, out var id);
            return _pagesConfigAsset.configs.Count > id ? _pagesConfigAsset.configs[id] : null;
        }
        private readonly Stack<BasePage> _pageStack = new();
        private readonly Dictionary<string, int> _pageName2IdMap = new ();
        private readonly Dictionary<int, BasePage> _pageDict = new ();
        
        private void InitUICamera() {
            DontDestroyOnLoad(CameraProxy.GetCameraRoot(CameraType.UI));
        }

        public void Open(string pageName, dynamic options = null) {
            Utils.Log(LOGTag,$"Open Page === {pageName}");
            var config = GetConfig(pageName);
            if (config == null) {
                Utils.LogError(LOGTag,$"Page name [{pageName}] dont have config");
                return;
            }
            PushPage(pageName, options);
        }
        private BasePage GetPage(string pageName) {
            var config = GetConfig(pageName);
            if (config == null) return null;
            _pageDict.TryGetValue(config.pageID, out var page);
            if (page) {
                return page;
            }
            page = Instantiate(AssetDatabase.LoadAssetAtPath<BasePage>($"Assets/ResourcesAssets/{config.assetPath}"),CameraProxy.GetCameraRoot(CameraType.UI));
            page.gameObject.name = config.pageName;
            page.Config = config;
            _pageDict.Add(config.pageID,page);
            return page;
        }
        private void PushPage(string pageName, dynamic options = null) {
            var page = GetPage(pageName);
            if (page.IsShow) {
                return;
            }
            if (_pageStack.Count > 0) {
                _pageStack.Peek().OnPause();
            }
            //每次入栈,触发page的OnEnter方法
            page.OnEnter(options);
            _pageStack.Push(page);
        }
        public void Close(string pageName) {
            Utils.Log(LOGTag,$"Close Page === {pageName}");
            var page = GetPage(pageName);
            page.OnExit();
            _pageDict.Remove(page.Config.pageID);
            //对各种类型界面做判断
            switch (page.Config.pageType) {
                case PageType.Stack:
                    break;
                case PageType.Freedom:
                    break;
            }
            PopPage();
        }
        private void PopPage() {
            if (_pageStack.Count <= 0) return;
            var go = _pageStack.Pop().gameObject;
            Destroy(go);
            if (_pageStack.Count > 0) {
                _pageStack.Peek().OnResume(); //恢复原先的界面
            }
        }
    }
}