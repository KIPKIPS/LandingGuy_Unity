using UnityEngine;

namespace CMF {
    //This script calculates the average framerate and displays it in the upper right corner of the screen;
    public class FPSCounter : MonoBehaviour {
        //Framerate is calculated using this interval;
        public float checkInterval = 1f;

        //Variables to keep track of passed time and frames;
        private int _currentPassedFrames;
        private float _currentPassedTime;

        //Current framerate;
        public float currentFrameRate;
        private string _currentFrameRateString = "";

        // Update;
        private void Update() {
            //Increment passed frames;
            _currentPassedFrames++;

            //Increment passed time;
            _currentPassedTime += Time.deltaTime;

            //If passed time has reached 'checkInterval', recalculate framerate;
            if (!(_currentPassedTime >= checkInterval)) return;
            //Calculate frame rate;
            currentFrameRate = _currentPassedFrames / _currentPassedTime;

            //Reset counters;
            _currentPassedTime = 0f;
            _currentPassedFrames = 0;

            //Clamp to two digits behind comma;
            currentFrameRate *= 100f;
            currentFrameRate = (int)currentFrameRate;
            currentFrameRate /= 100f;

            //Calculate framerate string to display later;
            _currentFrameRateString = $"{currentFrameRate}";
        }

        //Render framerate in the upper right corner of the screen;
        private void OnGUI() {
            GUI.contentColor = Color.black;
            const float labelSize = 40f;
            const float offset = 2f;
            GUI.Label(new Rect(Screen.width - labelSize + offset, offset, labelSize, 30f), _currentFrameRateString);
            GUI.contentColor = Color.white;
            GUI.Label(new Rect(Screen.width - labelSize, 0f, labelSize, 30f), _currentFrameRateString);
        }
    }
}