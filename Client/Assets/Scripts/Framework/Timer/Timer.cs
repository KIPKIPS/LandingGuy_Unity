// author:KIPKIPS
// date:2023.02.11 02:06
// describe:定时器
using Framework.Manager;
using TimerEntity = Framework.Manager.TimerManager.TimerEntity;
using System;
namespace Framework {
    public class Timer {
        private static TimerEntity entity;
        public static TimerEntity New(Action<TimerEntity> callback,int millisecond, int times = 1) {
            entity = TimerManager.Instance.CreateTimer(callback, millisecond, times);
            return entity;
        }
        public void Destroy() {
            entity.Destroy();
        }
        public void Start() {
            entity.Start();
        }
    }
}