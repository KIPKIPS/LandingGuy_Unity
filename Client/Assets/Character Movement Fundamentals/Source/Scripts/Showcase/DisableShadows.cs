using UnityEngine;

namespace CMF {
    //This script controls whether a scene light casts shadows;
    //It is used by the 'DemoMenu' script to toggle the shadows;
    public class DisableShadows : MonoBehaviour {
        //Whether shadows are cast;
        private bool _shadowsAreActive = true;

        //Reference to light component;
        public Light sceneLight;

        //Start;
        private void Start() {
            sceneLight = GetComponent<Light>();
        }

        //This function is called by an external script to disable/enable shadows in the scene;
        public void SetShadows(bool isActivated) {
            _shadowsAreActive = isActivated;
            sceneLight.shadows = !_shadowsAreActive ? LightShadows.None : LightShadows.Hard;
        }
    }
}