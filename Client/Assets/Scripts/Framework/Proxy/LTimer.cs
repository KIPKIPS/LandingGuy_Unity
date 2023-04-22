// author:KIPKIPS
// date:2023.02.11 02:06
// describe:定时器
using Framework.Manager;
using TimerEntity = Framework.Manager.TimerManager.TimerEntity;
using System;
namespace Framework {
    // ReSharper disable ClassNeverInstantiated.Global
    // ReSharper disable UnusedMember.Global
    public class LTimer {
        // ReSharper disable Unity.PerformanceAnalysis
        public static TimerEntity New(Action<TimerEntity> callback,int millisecond, int times = 0) =>TimerManager.Instance.CreateTimer(callback, millisecond, times);
        public static void Destroy(TimerEntity entity) => entity.Destroy();
        public static void Start(TimerEntity entity) => entity.Start();
    }
}