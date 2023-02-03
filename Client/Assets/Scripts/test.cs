// --[[
//     author:{wkp}
//     time:15:55
// ]]
using Framework.Core.Pool;
using UnityEngine;
public class test :MonoBehaviour, IPoolAble {
    public bool IsRecycled { get; set; }
    public void OnRecycled() {
            
    }
}