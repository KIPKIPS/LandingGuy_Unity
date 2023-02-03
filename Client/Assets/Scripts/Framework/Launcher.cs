// author:KIPKIPS
// date:2023.02.02 22:37
// describe:框架启动器

using Framework.Core.Config;
using UnityEngine;
using Timer = Framework.Core.Timer.Timer;

namespace Framework {
    public class Launcher : MonoBehaviour {
        
        void Awake() {
            Config.Instance.Initialize();
            Timer.Instance.Initialize();
        }
    }

}
