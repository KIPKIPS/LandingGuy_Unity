// author:KIPKIPS
// date:2023.02.08 23:15
// describe:枚举定义类

namespace Framework {
    // ReSharper disable InconsistentNaming
    public static class LDefine {
        #region 系统默认值
        public const int SYSTEM_STANDARD_DPI = 96; //系统默认dpi
        #endregion
        public static readonly string DataStoragePath = $"{System.Environment.CurrentDirectory}\\Storage\\";
        public static int BIND_ENUM_GAP = 10000;
    }
    #region AUDIO
    public enum AudioType {
        BGM = 0,
        EFFECT = 1,
    }
    #endregion

}
