// author:KIPKIPS
// date:2023.04.10 21:19
// describe:ui配置编辑器
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Framework.UI {
    public class PagesEditorWindow : EditorWindow {
        private const string LOGTag = "PagesEditorWindow";
        
        public PagesConfig pagesConfig;
        [MenuItem("Tools/UI/PagesEditor")]
        public static void OpenWindow() => GetWindow<PagesEditorWindow>().titleContent = new GUIContent("PagesEditor");
        private TableView _tableView;
        private TreeViewState _treeViewState;
        private MultiColumnHeaderState _multiColHeaderState;
        private List<PageData> _allConfig = new();
        private void OnEnable() {
            Initialize();
        }
        private void Initialize() {
            _treeViewState ??= new TreeViewState();
            _multiColHeaderState = CreateStateWith3Cols();
            var multiColHeader = new MultiColumnHeader(_multiColHeaderState);
            multiColHeader.ResizeToFit();
            _tableView = new TableView(_treeViewState, multiColHeader);
            pagesConfig = AssetDatabase.LoadAssetAtPath<PagesConfig>(UIManager.ConfigAssetPath);
            _allConfig.Clear();
            _tableView.PageConfigDict.Clear();
            foreach (var config in pagesConfig.configs) {
                _allConfig.Add(new PageData(config.pageID, config.pageName, config.pageType, config.pageMode, config.assetPath));
            }
            _tableView.SetData(_allConfig);
        }
        private static MultiColumnHeaderState.Column CreateColumn(string title, float width, float minWidth, float maxWidth, TextAlignment headerTextAlignment = TextAlignment.Left, bool canSort = false, bool autoResize = false, bool allowToggleVisibility = false) =>new() {
            headerContent = new GUIContent(title),
            width = width,
            minWidth = minWidth,
            maxWidth = maxWidth,
            headerTextAlignment = headerTextAlignment,
            canSort = canSort,
            autoResize = autoResize,
            allowToggleVisibility = allowToggleVisibility
        };
        private static MultiColumnHeaderState CreateStateWith3Cols() => new(new[] {
            CreateColumn("ID", 20, 20, 20, TextAlignment.Center),
            CreateColumn("PageName", 150, 150, 300),
            CreateColumn("PageType", 80, 80, 80),
            CreateColumn("PageMode", 80, 80, 80),
            CreateColumn("AssetPath", 300, 300, 400)
        });
        //ContextualMenuPopulateEvent

        public void OnGUI() {
            if (_tableView == null) return;
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Save", GUILayout.Width(50))) {
                pagesConfig.configs.Clear();
                var idx = 0;
                foreach (var data in _tableView.PageConfigDict.Select(pageData => pageData.Value)) {
                    pagesConfig.configs.Add(new PageConfig {
                        pageID = idx,
                        pageName = data.PageName,
                        pageType = data.PageType,
                        pageMode = data.PageMode,
                        assetPath = data.AssetPath
                    });
                    idx++;
                }
                EditorUtility.SetDirty(pagesConfig);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                LUtil.Log(LOGTag, "page config save success");
                Initialize();
            }
            if (GUILayout.Button("Add", GUILayout.Width(50))) {
                var id = _tableView.GetNewId();
                var pd = new PageData(id, $"name{id}Page", PageType.Stack, PageMode.Coexist, $"UI/Page/path{id}.prefab");
                _tableView.AddPageConfig(pd);
                _allConfig.Add(pd);
            }
            DrawInputTextField();
            GUILayout.EndHorizontal();
            var evt = UnityEngine.Event.current;

            var contextRect = new Rect(0, 20, position.width, position.height - 20);
            if (evt.type == UnityEngine.EventType.ContextClick) {
                if (contextRect.Contains(evt.mousePosition)){
                    var menu = new GenericMenu();
                    menu.AddItem(new GUIContent ("Delete Select"), false, DeleteSelect);
                    menu.ShowAsContext ();
                    evt.Use();
                }
            }
            if (!_tableView.IsValid) return;
            _tableView.OnGUI(contextRect);
        }

        private void DeleteSelect() {
            HashSet<int> dict = new HashSet<int>();
            foreach (var idx in _tableView.GetSelection()) {
                dict.Add(idx);
            }
            var newAllConfig = new List<PageData>();
            foreach (var c in _allConfig) {
                if (!dict.Contains(c.ID)) {
                    newAllConfig.Add(c);
                }
            }
            _allConfig = newAllConfig;
            _tableView.PageConfigDict.Clear();
            _tableView.SetData(_allConfig);
            // DoFilter(_inputSearchText);
        }
        private GUIStyle _textFieldRoundEdge;
        private GUIStyle _textFieldRoundEdgeCancelButton;
        private GUIStyle _textFieldRoundEdgeCancelButtonEmpty;
        private GUIStyle _transparentTextField;
        private string _inputSearchText;
        private string _lastInputSearchText;
        private void DrawInputTextField() {
            if (_textFieldRoundEdge == null) {
                _textFieldRoundEdge = new GUIStyle("SearchTextField");
                _textFieldRoundEdgeCancelButton = new GUIStyle("SearchCancelButton");
                _textFieldRoundEdgeCancelButtonEmpty = new GUIStyle("SearchCancelButtonEmpty");
                _transparentTextField = new GUIStyle(EditorStyles.whiteLabel) {
                    alignment = TextAnchor.MiddleLeft,
                    normal = {
                        textColor = EditorStyles.textField.normal.textColor
                    }
                };
            }
            var controlRect = EditorGUILayout.GetControlRect();
            var textFieldRoundEdge = _textFieldRoundEdge;
            var transparentTextField = _transparentTextField;
            var gUIStyle = _inputSearchText != "" ? _textFieldRoundEdgeCancelButton : _textFieldRoundEdgeCancelButtonEmpty;
            controlRect.y += 2f;
            if (UnityEngine.Event.current.type == UnityEngine.EventType.Repaint) {
                textFieldRoundEdge.Draw(controlRect, new GUIContent(""), 0);
            }
            var rect = controlRect;
            const int searchWidth = 15;
            rect.width -= gUIStyle.fixedWidth + searchWidth;
            rect.x += searchWidth;
            _inputSearchText = EditorGUI.TextField(rect, _inputSearchText, transparentTextField);
            if (_inputSearchText != _lastInputSearchText) {
                DoFilter(_inputSearchText);
            }
            _lastInputSearchText = _inputSearchText;
            controlRect.x += controlRect.width - gUIStyle.fixedWidth;
            controlRect.width = gUIStyle.fixedWidth;
            controlRect.height = gUIStyle.fixedHeight;
            if (string.IsNullOrEmpty(_inputSearchText)) return;
            if (!GUI.Button(controlRect, GUIContent.none, gUIStyle)) return;
            _inputSearchText = "";
            DoFilter(_inputSearchText);
            GUI.changed = true; //用户是否做了输入
            GUIUtility.keyboardControl = 0; //把焦点移开输入框
        }
        private void DoFilter(string filterStr) {
            if (string.IsNullOrEmpty(filterStr)) {
                _tableView.PageConfigDict.Clear();
                _tableView.SetData(_allConfig);
            } else {
                _tableView.DoFilter(filterStr);
            }
        }
        private void OnDestroy() {
            _tableView.PageConfigDict.Clear();
        }

        private class PageData {
            public readonly int ID;
            public string PageName;
            public PageType PageType;
            public PageMode PageMode;
            public string AssetPath;
            public PageData(int id, string pageName, PageType pageType, PageMode pageMode, string assetPath) {
                ID = id;
                PageName = pageName;
                PageType = pageType;
                PageMode = pageMode;
                AssetPath = assetPath;
            }
        }

        private class TableView : TreeView {
            public void SetData(List<PageData> pageData) {
                foreach (var data in pageData) {
                    AddPageConfig(data);
                }
            }
            private static bool Contains(object a, object b, bool ignoreCase = true) {
                var convertA = ignoreCase ? a.ToString().ToLower() : a.ToString();
                var convertB = ignoreCase ? b.ToString().ToLower() : b.ToString();
                return convertA.Contains(convertB);
            }
            public void DoFilter(string filterStr) {
                filterStr = filterStr.ToLower();
                Dictionary<int, PageData> newDict = new();
                foreach (var pcd in PageConfigDict) {
                    var d = pcd.Value;
                    if (Contains(d.ID, filterStr) || Contains(d.PageName, filterStr) || Contains(d.PageType, filterStr) || Contains(d.PageMode, filterStr) || Contains(d.AssetPath, filterStr)) {
                        newDict.Add(d.ID, d);
                    }
                }
                PageConfigDict.Clear();
                PageConfigDict = newDict;
                Reload();
            }
            public Dictionary<int, PageData> PageConfigDict { get; private set; } = new();
            public void AddPageConfig(PageData config) {
                PageConfigDict.Add(PageConfigDict.Count, config);
                Reload();
            }
            public TableView(TreeViewState state, MultiColumnHeader multiColumnHeader) : base(state, multiColumnHeader) {
                rowHeight = 24;
                showAlternatingRowBackgrounds = true; //隔行显示颜色
                showBorder = false; //表格边框
                // Reload();
            }
            public bool IsValid => PageConfigDict.Count > 0;

            public int GetNewId() => PageConfigDict.Count;
            // protected override IList<TreeViewItem> BuildRows(TreeViewItem root) {
            //     return base.BuildRows(root);
            // }

            protected override TreeViewItem BuildRoot() {
                var rootTreeViewItem = new TreeViewItem(-1, -1); //root的depth必须为-1
                var idx = 0;
                foreach (var pcd in PageConfigDict) {
                    var rowData = pcd.Value;
                    var childTreeViewItem = new TreeViewItem(idx, 0, rowData.ID.ToString());
                    rootTreeViewItem.AddChild(childTreeViewItem);
                    idx++;
                }
                return rootTreeViewItem;
            }

            protected override void RowGUI(RowGUIArgs args) {
                for (var i = 0; i < args.GetNumVisibleColumns(); ++i) {
                    CellGUI(args.GetCellRect(i), args.item, args.GetColumn(i));
                }
            }

            private void CellGUI(Rect cellRect, TreeViewItem item, int col) {
                //var kv = _pageConfigDict[item.id];
                if (!PageConfigDict.TryGetValue(item.id, out var cellRowData)) return;
                CenterRectUsingSingleLineHeight(ref cellRect); //控件的区域居中
                switch (col) {
                    case 0:
                        var style = new GUIStyle {
                            alignment = TextAnchor.MiddleCenter,
                            normal = {
                                textColor = Color.white
                            }
                        };
                        EditorGUI.LabelField(cellRect, $"{cellRowData.ID}", style);
                        break;
                    case 1:
                        cellRowData.PageName = EditorGUI.TextField(cellRect, cellRowData.PageName); //预制体选择框
                        break;
                    case 2:
                        cellRowData.PageType = (PageType)EditorGUI.EnumPopup(cellRect, cellRowData.PageType);
                        break;
                    case 3:
                        cellRowData.PageMode = (PageMode)EditorGUI.EnumPopup(cellRect, cellRowData.PageMode);
                        break;
                    case 4:
                        cellRowData.AssetPath = EditorGUI.TextField(cellRect, cellRowData.AssetPath);
                        break;
                }
            }
        }
    }
}