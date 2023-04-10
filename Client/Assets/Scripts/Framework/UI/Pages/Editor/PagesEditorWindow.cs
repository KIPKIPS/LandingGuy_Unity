// author:KIPKIPS
// date:2023.04.10 21:19
// describe:
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Framework.UI.Pages {
    public class PagesEditorWindow : EditorWindow {
        private const string LOGTag = "PagesEditorWindow";
        private const string assetPath = "Assets/ResourcesAssets/UI/Config/pages.asset";
        [MenuItem("Tools/UI/PagesEditor")]
        public static void OpenWindow() {
            GetWindow<PagesEditorWindow>().titleContent = new GUIContent("PagesEditor");
        }
        
        public void OnGUI() {

        }
        private class PageData {
            public int ID;
            public string PageName;
            public PageType PageType;
            public PageMode PageMode;
            public string AssetPath;
        }
        
    }
}