// author:KIPKIPS
// date:2023.04.10 21:19
// describe:
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
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
        private readonly List<PageData> _allConfig = new();
        private void OnEnable() {
            _treeViewState ??= new TreeViewState();
            _multiColHeaderState = CreateStateWith3Cols();
            var multiColHeader = new MultiColumnHeader(_multiColHeaderState);
            multiColHeader.ResizeToFit();
            _tableView = new TableView(_treeViewState, multiColHeader);
            pagesConfig = AssetDatabase.LoadAssetAtPath<PagesConfig>(UIManager.ConfigAssetPath);
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
            CreateColumn("AssetPath", 200, 200, 300)
        });

        public void OnGUI() {
            if (_tableView == null) return;
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Save", GUILayout.Width(50))) {
                pagesConfig.configs.Clear();
                foreach (var data in _tableView.PageConfigDict.Select(pageData => pageData.Value)) {
                    pagesConfig.configs.Add(new PageConfig {
                        pageID = data.ID,
                        pageName = data.PageName,
                        pageType = data.PageType,
                        pageMode = data.PageMode,
                        assetPath = data.AssetPath
                    });
                }
                AssetDatabase.SaveAssets();
                Utils.Log(LOGTag, "page config save success");
            }
            if (GUILayout.Button("Add", GUILayout.Width(50))) {
                _tableView.AddPageConfig(new PageData(_tableView.GetNewId(), "$name", PageType.Stack, PageMode.Coexist, "$path"));
            }
            DrawInputTextField();
            GUILayout.EndHorizontal();
            if (!_tableView.IsValid) return;
            _tableView.OnGUI(new Rect(0, 20, position.width, position.height - 20));
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
                for (var id = 0; id < PageConfigDict.Count; ++id) {
                    var d = PageConfigDict[id];
                    if (Contains(d.ID, filterStr) || Contains(d.PageName, filterStr) || Contains(d.PageType, filterStr) || Contains(d.PageMode, filterStr) || Contains(d.AssetPath, filterStr)) {
                        newDict.Add(newDict.Count, d);
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
                for (var id = 0; id < PageConfigDict.Count; ++id) {
                    var rowData = PageConfigDict[id];
                    var childTreeViewItem = new TreeViewItem(rowData.ID, 0, id.ToString());
                    rootTreeViewItem.AddChild(childTreeViewItem);
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
                        EditorGUI.LabelField(cellRect, $"{item.id}", style);
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