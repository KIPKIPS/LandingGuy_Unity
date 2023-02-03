// author:KIPKIPS
// date:2023.02.02 22:37
// describe:框架启动器
using Framework.Core.Pool;
using UnityEngine;

public class Launcher : MonoBehaviour {
    
    void Awake() {
        test test = Resources.Load<test>("cube");
        Debug.Log(test == null);
        PrefabPool<test> gopool = new PrefabPool<test>(test);
        gopool.Allocate();
        gopool.Allocate();
    }
}