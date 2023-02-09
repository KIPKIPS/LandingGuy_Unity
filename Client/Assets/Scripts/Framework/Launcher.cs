// author:KIPKIPS
// date:2023.02.02 22:37
// describe:框架启动器
using UnityEngine;

namespace Framework {
    public class Launcher : MonoSingleton<Launcher> {
        
        void Awake() {
            // AudioManager.Instance.PlayAudio(Resources.Load<AudioClip>("aud_button"));
        }
        void Update() {
            if (Input.GetMouseButtonDown(0)) {
                AudioManager.Instance.PlayAudio(Resources.Load<AudioClip>("aud_button"),transform);
            }
        }
    }
}
