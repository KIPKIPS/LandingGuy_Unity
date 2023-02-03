// author:KIPKIPS
// date:2023.02.02 22:37
// describe:框架启动器
using Framework.Core.Config;
using UnityEngine;

namespace Framework {
    public class Launcher : MonoBehaviour {
        
        void Awake() {
            Config.Instance.Initialize();
            Utils.Log(Config.GetConfig("color", 1));
        }
    }

}
