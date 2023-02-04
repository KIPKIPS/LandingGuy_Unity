// author:KIPKIPS
// date:2023.02.03 21:59
// describe:配置表数据
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
namespace Framework.Core {
    public class Config :Singleton<Config> {
        private static readonly string logTag = "Config";
        private static readonly string configPath = "Config/"; //配置表路径
        //配置表
        private static readonly RestrictedDictionary<string, List<dynamic>> _configDict = new ();
        private static readonly RestrictedDictionary<string, RestrictedDictionary<string, string>> _typeDict = new ();
        
        /// <summary>
        /// 构造器
        /// </summary>
        private Config() => AnalyticsConfig();
        
        /// <summary>
        /// 解析配置表
        /// </summary>
        private static void AnalyticsConfig() {
            _configDict.EnableWrite();
            _typeDict.EnableWrite();
            // _configDict = new Dictionary<string, List<dynamic>>();
            //获取所有配置表
            // UIUtils.LoadJsonByPath<List<JObject>>("Data/" + tabName + ".json");
            DirectoryInfo dir = new DirectoryInfo(configPath);
            FileSystemInfo[] files = dir.GetFileSystemInfos();
            string configName = "";
            string fullName = "";
            for (int i = 0; i < files.Length; i++) {
                configName = files[i].Name.Replace(".json", "");
                // fullName = configPath + files[i].Name;
                if (!_configDict.ContainsKey(configName)) {
                    _configDict.Add(configName, new List<dynamic>());
                    _configDict[configName].Add(null); //预留一个位置
                    // _configDict[configName].Add();
                    // Utils.Log(configPath + files[i].Name);
                    try {
                        List<JObject> jObjList = Utils.LoadJsonByPath<List<JObject>>(configPath + files[i].Name);
                        JObject metatable = jObjList.Last();
                        if (metatable.ContainsKey("__metatable")) {
                            IEnumerable<JProperty> metatableProperties = metatable.Properties();
                            if (!_typeDict.ContainsKey(configName)) {
                                _typeDict.Add(configName, new RestrictedDictionary<string, string>());
                            }
                            foreach (JProperty metatableProp in metatableProperties) {
                                if (metatableProp.Name != "__metatable") {
                                    _typeDict[configName].EnableWrite();
                                    _typeDict[configName].Add(metatableProp.Name, metatableProp.Value.ToString());
                                    _typeDict[configName].ForbidWrite();
                                }
                            }
                            for (int j = 0; j < jObjList.Count - 1; j++) {
                                RestrictedDictionary<string, dynamic> table = new ();
                                table.EnableWrite();
                                IEnumerable<JProperty> properties = jObjList[j].Properties();
                                foreach (JProperty prop in properties) {
                                    switch (prop.Value.Type.ToString()) {
                                        case "Integer":
                                            table[prop.Name] = (int)prop.Value;
                                            break;
                                        case "Float":
                                            table[prop.Name] = (float)prop.Value;
                                            break;
                                        case "Boolean":
                                            table[prop.Name] = (bool)prop.Value;
                                            break;
                                        case "String":
                                            table[prop.Name] = prop.Value.ToString();
                                            break;
                                        case "Array":
                                            table[prop.Name] = HandleArray(prop.Value.ToArray());
                                            break;
                                        case "Object":
                                            table[prop.Name] = HandleDict(prop.Value.ToObject<JObject>(), prop.Name, configName);
                                            break;
                                    }
                                }
                                table.ForbidWrite();
                                _configDict[configName].Add(table);
                            }
                        }
                    } catch (Exception ex) {
                        Utils.Log(logTag, configName);
                        Utils.LogError(logTag, ex.ToString());
                    }
                }
            }
            _configDict.ForbidWrite();
            _typeDict.ForbidWrite();
            Utils.Log(logTag, "Config table data is parsed");
        }
        
        /// <summary>
        /// 处理字典类型的配置表
        /// </summary>
        /// <param name="jObj">解析的Json对象</param>
        /// <param name="filedName">文件名称</param>
        /// <param name="cfName">配置表名称</param>
        /// <returns></returns>
        private static dynamic HandleDict(JObject jObj, string filedName, string cfName) {
            dynamic table = new RestrictedDictionary<dynamic, dynamic>();
            table.EnableWrite();
            RestrictedDictionary<string, string> valueTypeDict = _typeDict[cfName];
            IEnumerable<JProperty> properties = jObj.Properties();
            dynamic key = null;
            foreach (JProperty prop in properties) {
                if (valueTypeDict.ContainsKey(filedName)) {
                    if (valueTypeDict[filedName].StartsWith("dict<int")) {
                        key = int.Parse(prop.Name);
                    } else if (valueTypeDict[filedName].StartsWith("dict<float")) {
                        key = float.Parse(prop.Name);
                    } else if (valueTypeDict[filedName].StartsWith("dict<string")) {
                        key = prop.Name;
                    }
                    switch (prop.Value.Type.ToString()) {
                        case "Integer":
                            table.Add(key, (int)prop.Value);
                            break;
                        case "Float":
                            table.Add(key, (float)prop.Value);
                            break;
                        case "Boolean":
                            table.Add(key, (bool)prop.Value);
                            break;
                        case "String":
                            table.Add(key, prop.Value.ToString());
                            break;
                    }
                }
            }
            table.ForbidWrite();
            return table;
        }
        
        /// <summary>
        /// 递归处理数组类型
        /// </summary>
        /// <param name="array">配置数组</param>
        /// <returns></returns>
        private static dynamic HandleArray(JToken[] array) {
            dynamic table = new RestrictedDictionary<int, dynamic>();
            table.EnableWrite();
            for (int i = 1; i <= array.Length; i++) {
                switch (array[i - 1].Type.ToString()) {
                    case "Integer":
                        table.Add(i, (int)array[i - 1]);
                        break;
                    case "Float":
                        table.Add(i, (float)array[i - 1]);
                        break;
                    case "Boolean":
                        table.Add(i, (bool)array[i - 1]);
                        break;
                    case "String":
                        table.Add(i, array[i - 1].ToString());
                        break;
                    case "Array":
                        table.Add(i, HandleArray(array[i - 1].ToArray()));
                        break;
                }
            }
            table.ForbidWrite();
            return table;
        }

        /// <summary>
        /// 获取配置表
        /// </summary>
        /// <param name="configName">配置表名称</param>
        /// <returns>配置表对象</returns>
        public static List<dynamic> GetConfig(string configName) {
            try {
                if (_configDict.ContainsKey(configName)) {
                    return _configDict[configName];
                }
            } catch (Exception e) {
                return null;
            }
            return null;
        }

        /// <summary>
        /// 获取配置表的指定id的Hashtable
        /// </summary>
        /// <param name="configName">配置表名称</param>
        /// <param name="id">索引</param>
        /// <returns>配置表索引数据对象</returns>
        public static dynamic GetConfig(string configName, int id) {
            try {
                if (_configDict.ContainsKey(configName)) {
                    if (_configDict[configName] != null && _configDict[configName][id] != null) {
                        return _configDict[configName][id];
                    }
                }
            } catch (Exception e) {
                return null;
            }
            return null;
        }
    }
}