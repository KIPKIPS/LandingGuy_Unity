// author:KIPKIPS
// date:2023.04.10 21:29
// describe:
using System;

namespace Framework.UI {
    [Serializable]
    public class PageConfig {
        public int pageID;
        public string pageName;
        public PageType pageType;
        public PageMode pageMode;
        public string assetPath;
    }
}