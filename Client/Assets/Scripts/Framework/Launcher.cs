// author:KIPKIPS
// date:2023.02.02 22:37
// describe:框架启动器
using UnityEngine;
using Framework.Manager;
using Framework.UI;
using SceneManager = Framework.Manager.SceneManager;

namespace Framework {
    public class Launcher : MonoBehaviour {
        private void Awake() {
            DontDestroyOnLoad(this);
            MonoManager.Instance.Launch();
            EventManager.Instance.Launch();
            ConfigManager.Instance.Launch();
            StorageManager.Instance.Launch();
            AudioManager.Instance.Launch();
            LocalizationManager.Instance.Launch();
            SceneManager.Instance.Launch();
            UIManager.Instance.Launch();

            UIProxy.Open("CreateRolePage");
        }
    }
}