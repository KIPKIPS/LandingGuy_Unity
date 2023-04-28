// author:KIPKIPS
// date:2023.04.08 20:01
// describe:UI框架管理器
using System;
using System.Collections.Generic;
using Framework.Singleton;
// using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;
// using UnityEngine.Profiling;

namespace Framework.UI {
    public class UIManager : Singleton<UIManager> {
        private const string LOGTag = "UI";
        public const string ConfigAssetPath = "Assets/ResourcesAssets/UI/Config/pages.asset"; //配置资源路径
        private PagesConfig _pagesConfigAsset;
        public void Launch() {
            _pageDict.Clear();
            UIBinding.Register();
            InitUICamera();
            LoadConfig();
        }
        private void LoadConfig() {
            _pagesConfigAsset = AssetDatabase.LoadAssetAtPath<PagesConfig>(ConfigAssetPath);
            foreach (var t in _pagesConfigAsset.configs) {
                _pageName2IdMap.Add(t.pageName, t.pageID);
            }
        }
        private PageConfig GetConfig(string pageName) {
            return _pageName2IdMap.ContainsKey(pageName) ? _pagesConfigAsset.configs[_pageName2IdMap[pageName]] : null;
        }
        private readonly Stack<UIBinding> _pageStack = new();
        
        private readonly Dictionary<string, int> _pageName2IdMap = new();
        private readonly Dictionary<int, UIBinding> _pageDict = new();
        private void InitUICamera() {
            Object.DontDestroyOnLoad(LCamera.GetCameraRoot(CameraType.UI));
        }
        public void Open(string pageName, dynamic options = null) {
            LUtil.Log(LOGTag, $"Open Page === {pageName}");
            var config = GetConfig(pageName);
            if (config == null) {
                LUtil.LogError(LOGTag, $"Page name [{pageName}] dont have config");
                return;
            }
            PushPage(pageName, options);
        }
        private UIBinding GetPageUIBinding(string pageName) {
            var config = GetConfig(pageName);
            if (config == null) {
                LUtil.LogError(LOGTag, $"Page name [{pageName}] dont have config");
                return null;
            }
            _pageDict.TryGetValue(config.pageID, out var uiBinding);
            if (uiBinding) {
                return uiBinding;
            }
            var go = Object.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>($"Assets/ResourcesAssets/{config.assetPath}"), LCamera.GetCameraRoot(CameraType.UI));
            uiBinding = go.GetComponent<UIBinding>();
            var page = Activator.CreateInstance(UIBinding.GetPageType(uiBinding.PageType)) as BasePage;
            uiBinding.Page = page;
            page.UIBinding = uiBinding;
            page.Config = config;
            page.Canvas = go.GetComponent<Canvas>();
            page.Canvas.sortingOrder = CalculateSortingOrder(config.pageType);
            // page.Canvas.additionalShaderChannels = AdditionalCanvasShaderChannels.Tangent | AdditionalCanvasShaderChannels.TexCoord1 | AdditionalCanvasShaderChannels.Normal;
            page.OnBind();
            go.name = config.pageName;
            _pageDict.Add(config.pageID, uiBinding);
            return uiBinding;
        }
        private const int BaseStackSortingOrder = 0;
        private const int BaseFreedomSortingOrder = 1000;
        private int CalculateSortingOrder(PageType pageType) {
            switch (pageType) {
                case PageType.Stack:
                    return BaseStackSortingOrder + _pageStack.Count * 2;
                case PageType.Freedom:
                    return BaseFreedomSortingOrder + _pageStack.Count * 2;
            }
            return 0;
        }

        private void PushPage(string pageName, dynamic options = null) {
            var uiBinding = GetPageUIBinding(pageName);
            var page = uiBinding.Page;
            if (page.IsShow) {
                LUtil.LogError("Repeat Open",pageName);
                return;
            }
            if (_pageStack.Count > 0) {
                _pageStack.Peek().Page.OnPause();
            }
            //每次入栈,触发page的OnEnter方法
            page.OnEnter(options);
            page.
            Canvas.worldCamera = LCamera.GetCamera(CameraType.UI);
            page.IsShow = true;
            _pageStack.Push(uiBinding);
        }
        public void Close(string pageName) {
            LUtil.Log(LOGTag, $"Close Page === {pageName}");
            var uiBinding = GetPageUIBinding(pageName);
            var page = uiBinding.Page;
            if (page == null) return;
            page.OnExit();
            page.IsShow = false;
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
            Object.Destroy(go);
            if (_pageStack.Count > 0) {
                _pageStack.Peek().Page.OnResume(); //恢复原先的界面
            }
        }
        
    }
}