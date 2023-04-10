// author:KIPKIPS
// date:2023.04.10 21:27
// describe:
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.UI.Pages {
    [CreateAssetMenu(fileName = "PagesConfig", menuName = "Framework/UI/PagesConfig")]
    public class PagesConfig : ScriptableObject {
        public List<PageConfig> configs = new();
    }
}