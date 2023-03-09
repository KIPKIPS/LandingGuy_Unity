// author:KIPKIPS
// date:2023.02.03 21:59
// describe:配置表数据
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Framework.Container;
using Framework.Singleton;
namespace Framework.Manager {
    // ReSharper disable ClassNeverInstantiated.Global
    // ReSharper disable UnusedMember.Global
    public class ConfigManager :Singleton<ConfigManager> {
        private const string LOGTag = "ConfigManager";
        private const string ConfigPath = "Config/"; //配置表路径
        //配置表
        private readonly RestrictedDictionary<string, List<dynamic>> _configDict = new ();
        private readonly RestrictedDictionary<string, RestrictedDictionary<string, string>> _typeDict = new ();
        public void Launch() {
            AnalyticsConfig();
            Utils.Log(LOGTag, "config data load finished");
        }
        // private ConfigManager() {
        //     
        // }
        /// <summary>
        /// 解析配置表
        /// </summary>
        private void AnalyticsConfig() {
            _configDict.EnableWrite();
            _typeDict.EnableWrite();
            // _configDict = new Dictionary<string, List<dynamic>>();
            //获取所有配置表
            // UIUtils.LoadJsonByPath<List<JObject>>("Data/" + tabName + ".json");
            var dir = new DirectoryInfo(ConfigPath);
            var files = dir.GetFileSystemInfos();
            // string fullName = "";
            foreach (var t in files) {
                var configName = t.Name.Replace(".json", "");
                // fullName = configPath + files[i].Name;
                if (_configDict.ContainsKey(configName)) continue;
                _configDict.Add(configName, new List<dynamic>());
                _configDict[configName].Add(null); //预留一个位置
                // _configDict[configName].Add();
                // Utils.Log(configPath + files[i].Name);
                try {
                    var jObjList = Utils.LoadJsonByPath<List<JObject>>(ConfigPath + t.Name);
                    var metatable = jObjList.Last();
                    if (metatable.ContainsKey("__metatable")) {
                        var metatableProperties = metatable.Properties();
                        if (!_typeDict.ContainsKey(configName)) {
                            _typeDict.Add(configName, new RestrictedDictionary<string, string>());
                        }
                        foreach (var metatableProp in metatableProperties) {
                            if (metatableProp.Name == "__metatable") continue;
                            _typeDict[configName].EnableWrite();
                            _typeDict[configName].Add(metatableProp.Name, metatableProp.Value.ToString());
                            _typeDict[configName].ForbidWrite();
                        }
                        for (var j = 0; j < jObjList.Count - 1; j++) {
                            RestrictedDictionary<string, dynamic> table = new ();
                            table.EnableWrite();
                            var properties = jObjList[j].Properties();
                            foreach (var prop in properties) {
                                table[prop.Name] = prop.Value.Type.ToString() switch {
                                    "Integer" => prop.Value,
                                    "Float" => prop.Value,
                                    "Boolean" => prop.Value,
                                    "String" => prop.Value.ToString(),
                                    "Array" => HandleArray(prop.Value.ToArray()),
                                    "Object" => HandleDict(prop.Value.ToObject<JObject>(), prop.Name, configName),
                                    _ => table[prop.Name]
                                };
                            }
                            table.ForbidWrite();
                            _configDict[configName].Add(table);
                        }
                    }
                } catch (Exception ex) {
                    Utils.Log(LOGTag, configName);
                    Utils.LogError(LOGTag, ex.ToString());
                }
            }
            _configDict.ForbidWrite();
            _typeDict.ForbidWrite();
        }
        
        /// <summary>
        /// 处理字典类型的配置表
        /// </summary>
        /// <param name="jObj">解析的Json对象</param>
        /// <param name="filedName">文件名称</param>
        /// <param name="cfName">配置表名称</param>
        /// <returns></returns>
        private dynamic HandleDict(JObject jObj, string filedName, string cfName) {
            dynamic table = new RestrictedDictionary<dynamic, dynamic>();
            table.EnableWrite();
            var valueTypeDict = _typeDict[cfName];
            var properties = jObj.Properties();
            dynamic key = null;
            foreach (var prop in properties) {
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
        private static dynamic HandleArray(IReadOnlyList<JToken> array) {
            dynamic table = new RestrictedDictionary<int, dynamic>();
            table.EnableWrite();
            for (var i = 1; i <= array.Count; i++) {
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
        public List<dynamic> GetConfig(string configName) {
            dynamic result = null;
            try {
                if (_configDict.ContainsKey(configName)) {
                    result = _configDict[configName];
                }
            } catch (Exception) {
                result = null;
            }
            return result;
        }

        /// <summary>
        /// 获取配置表的指定id的Hashtable
        /// </summary>
        /// <param name="configName">配置表名称</param>
        /// <param name="id">索引</param>
        /// <returns>配置表索引数据对象</returns>
        public dynamic GetConfig(string configName, int id) {
            dynamic result = null;
            try {
                if (_configDict.ContainsKey(configName)) {
                    if (_configDict[configName] != null && _configDict[configName][id] != null) {
                        result= _configDict[configName][id];
                    }
                }
            } catch (Exception) {
                result = null;
            }
            return result;
        }
    }
}