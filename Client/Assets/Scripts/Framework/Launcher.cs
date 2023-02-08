// author:KIPKIPS
// date:2023.02.02 22:37
// describe:框架启动器

namespace Framework {
    public class Launcher : MonoSingleton<Launcher> {
        
        void Awake() {
            AudioManager.Instance.BgmVolume = 0.5f;
        }
    }
}
