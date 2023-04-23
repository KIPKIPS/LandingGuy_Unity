﻿// author:KIPKIPS
// date:2023.04.10 21:31
// describe:界面类型枚举
namespace Framework.UI {
    /// <summary>
    /// 界面模式
    /// </summary>
    public enum PageMode {
        Coexist,//共存
        Exclusion,//互斥
    }
    /// <summary>
    /// 界面类型
    /// </summary>
    public enum PageType {
        Stack,
        Freedom,
    }
}