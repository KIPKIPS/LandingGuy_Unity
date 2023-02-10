// author:KIPKIPS
// date:2023.02.02 22:37
// describe:框架启动器
using System;
using UnityEngine;
using Framework.Manager;
namespace Framework {
    public class Launcher : MonoBehaviour {
        
        void Awake() {
            StartCoroutine(Timer.TriggerTimer());
            Config.Instance.Initialize();
            StorageManager.Instance.Initialize();
            AudioManager.Instance.Initialize();
        }
        void Update() {
            if (Input.GetMouseButtonDown(0)) {
                System.Random r = new System.Random();
                AudioManager.Instance.PlayAudio(Resources.Load<AudioClip>("aud_button"),new Vector3(r.Next(2),r.Next(2),r.Next(2)));
            }
            if (Input.GetKeyDown(KeyCode.M)) {
                AudioManager.Instance.Mute = !AudioManager.Instance.Mute;
            }
            if (Input.GetKeyDown(KeyCode.KeypadPlus)) {
                AudioManager.Instance.GlobalVolume += 0.1f;
            }
            if (Input.GetKeyDown(KeyCode.Minus)) {
                AudioManager.Instance.GlobalVolume -= 0.1f;
            }
        }
    }
}
