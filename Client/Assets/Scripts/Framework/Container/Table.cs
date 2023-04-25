// author:KIPKIPS
// date:2023.04.25 16:25
// describe:table容器
using System.Collections.Generic;
using System.Dynamic;

namespace Framework.Container {
    public class Table {
        private Dictionary<string, dynamic> _strTable;
        private Dictionary<int, dynamic> _intTable;
        private Dictionary<string, dynamic> StrTable => _strTable ??= new Dictionary<string, dynamic>();
        private Dictionary<int, dynamic> IntTable => _intTable ??= new Dictionary<int, dynamic>();
        public dynamic this[string key] {
            get {
                StrTable.TryGetValue(key, out var data);
                return data != null ? data : null;
            }
            set {
                if (StrTable.ContainsKey(key)) {
                    StrTable[key] = value;
                } else {
                    StrTable.Add(key,value);
                }
            }
        }
        public dynamic this[int index] {
            get {
                IntTable.TryGetValue(index, out var data);
                return data != null ? data : null;
            }
            set {
                if (IntTable.ContainsKey(index)) {
                    IntTable[index] = value;
                } else {
                    IntTable.Add(index,value);
                }
            }
        }
    }
}