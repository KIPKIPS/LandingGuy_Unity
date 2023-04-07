// --[[
//     author:{wkp}
//     time:15:33
// ]]
using System;
using UnityEngine;

namespace Framework.AI.BehaviorTree {
    [Serializable]
    public class Blackboard {
        public Vector3 moveToPosition;
        public GameObject moveToObject;
    }
}