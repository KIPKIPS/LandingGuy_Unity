// author:KIPKIPS
// date:2023.03.18 15:33
// describe:行为树黑板
using System;
using UnityEngine;

namespace Framework.AI {
    [Serializable]
    public class Blackboard {
        public Vector3 moveToPosition;
        public GameObject moveToObject;
    }
}