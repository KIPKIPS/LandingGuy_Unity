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

            var t= Timer.New(_ => {
                UIProxy.Open("TempPage");
            },2000,1);
            t.Start();
            int idx = 0;
            var t2= Timer.New(_ => {
                Utils.Log("tick");
                idx++;
                if (idx ==5) {
                    t.Destroy();
                    _.Destroy();
                    var t3= Timer.New(x => {
                        Utils.Log("t3");
                        x.Destroy();
                    },2000,1);
                    t3.Start();
                }
            },500);
            t2.Start();
            
        }
    }
}