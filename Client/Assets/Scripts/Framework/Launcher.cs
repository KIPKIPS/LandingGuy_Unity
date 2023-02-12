// author:KIPKIPS
// date:2023.02.02 22:37
// describe:框架启动器
using System;
using UnityEngine;
using Framework.Manager;
using SceneManager = Framework.Manager.SceneManager;

namespace Framework {
    public class Launcher : MonoBehaviour {
        
        void Awake() {
            DontDestroyOnLoad(this);
            MonoManager.Instance.Launch();
            TimerManager.Instance.Launch();
            EventManager.Instance.Launch();
            ConfigManager.Instance.Launch();
            StorageManager.Instance.Launch();
            AudioManager.Instance.Launch();
            LocalizationManager.Instance.Launch();
            SceneManager.Instance.Launch();
            #region Test
            Event.Register(EventType.SCENE_LOAD_FINISHED,d => {
                Utils.Log(d);
            });
            #endregion
        }
        void Update() {
            #region Test

            if (Input.GetMouseButtonDown(0)) {
                System.Random r = new System.Random();
                AudioManager.Instance.PlayAudio(Resources.Load<AudioClip>("aud_button"),new Vector3(r.Next(3),r.Next(3),r.Next(3)));
            }
            if (Input.GetKeyDown(KeyCode.M)) {
                AudioManager.Instance.Mute = !AudioManager.Instance.Mute;
            }
            if (Input.GetKeyDown(KeyCode.Equals)) {
                AudioManager.Instance.GlobalVolume += 0.1f;
            }
            if (Input.GetKeyDown(KeyCode.Minus)) {
                AudioManager.Instance.GlobalVolume -= 0.1f;
            }
            if (Input.GetKeyDown(KeyCode.S)) {
                TimerManager.Instance.Dispose();
            }
            if (Input.GetKeyDown(KeyCode.L)) {
                SceneManager.Instance.LoadSceneAsync("TestScene", () => {
                    Utils.Log("TestScene scene load finished");
                });
            }
            

            #endregion
        }
    }
}
