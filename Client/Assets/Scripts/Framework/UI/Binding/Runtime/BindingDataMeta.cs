// --[[
//     author:{wkp}
//     time:18:51
// ]]
using System.Collections.Generic;
namespace Framework.UI {
    public class BindingDataMeta {
        public class PageDataContainer {
            
        }

        private Dictionary<int, PageDataContainer> _pageContainerDict;
        public Dictionary<int, PageDataContainer> PageContainerDict => _pageContainerDict??= new Dictionary<int, PageDataContainer>();
    }
}