using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CMF {
    //This script handles and plays audio cues like footsteps, jump and land audio clips based on character movement speed and events; 
    public class AudioControl : MonoBehaviour {
        //References to components;
        private Controller _controller;
        private Animator _animator;
        private Mover _mover;
        private Transform _tr;
        public AudioSource audioSource;

        //Whether footsteps will be based on the currently playing animation or calculated based on walked distance (see further below);
        public bool useAnimationBasedFootsteps = true;

        //Velocity threshold for landing sound effect;
        //Sound effect will only be played if downward velocity exceeds this threshold;
        public float landVelocityThreshold = 5f;

        //Footsteps will be played every time the traveled distance reaches this value (if 'useAnimationBasedFootsteps' is set to 'true');
        public float footstepDistance = 0.2f;
        private float _currentFootstepDistance;

        private float _currentFootStepValue;

        //Volume of all audio clips;
        [Range(0f, 1f)]
        public float audioClipVolume = 0.1f;

        //Range of random volume deviation used for footsteps;
        //Footstep audio clips will be played at different volumes for a more "natural sounding" result;
        public float relativeRandomizedVolumeRange = 0.2f;

        //Audio clips;
        public AudioClip[] footStepClips;
        public AudioClip jumpClip;
        public AudioClip landClip;
        private static readonly int FootStep = Animator.StringToHash("FootStep");

        //Setup;
        private void Start() {
            //Get component references;
            _controller = GetComponent<Controller>();
            _animator = GetComponentInChildren<Animator>();
            _mover = GetComponent<Mover>();
            _tr = transform;

            //Connecting events to controller events;
            _controller.OnLand += OnLand;
            _controller.OnJump += OnJump;
            if (!_animator) useAnimationBasedFootsteps = false;
        }

        //Update;
        void Update() {
            //Get controller velocity;
            var velocity = _controller.GetVelocity();

            //Calculate horizontal velocity;
            var horizontalVelocity = VectorMath.RemoveDotVector(velocity, _tr.up);
            FootStepUpdate(horizontalVelocity.magnitude);
        }

        private void FootStepUpdate(float movementSpeed) {
            const float speedThreshold = 0.05f;
            if (useAnimationBasedFootsteps) {
                //Get current foot step value from animator;
                var newFootStepValue = _animator.GetFloat(FootStep);

                //Play a foot step audio clip whenever the foot step value changes its sign;
                if ((_currentFootStepValue <= 0f && newFootStepValue > 0f) || (_currentFootStepValue >= 0f && newFootStepValue < 0f)) {
                    //Only play footstep sound if mover is grounded and movement speed is above the threshold;
                    if (_mover.IsGrounded() && movementSpeed > speedThreshold) PlayFootstepSound(movementSpeed);
                }
                _currentFootStepValue = newFootStepValue;
            } else {
                _currentFootstepDistance += Time.deltaTime * movementSpeed;

                //Play foot step audio clip if a certain distance has been traveled;
                if (!(_currentFootstepDistance > footstepDistance)) return;
                //Only play footstep sound if mover is grounded and movement speed is above the threshold;
                if (_mover.IsGrounded() && movementSpeed > speedThreshold) PlayFootstepSound(movementSpeed);
                _currentFootstepDistance = 0f;
            }
        }

        private void PlayFootstepSound(float movementSpeed) {
            var footStepClipIndex = Random.Range(0, footStepClips.Length);
            audioSource.PlayOneShot(footStepClips[footStepClipIndex], audioClipVolume + audioClipVolume * Random.Range(-relativeRandomizedVolumeRange, relativeRandomizedVolumeRange));
        }

        private void OnLand(Vector3 v) {
            //Only trigger sound if downward velocity exceeds threshold;
            if (VectorMath.GetDotProduct(v, _tr.up) > -landVelocityThreshold) return;

            //Play land audio clip;
            audioSource.PlayOneShot(landClip, audioClipVolume);
        }

        private void OnJump(Vector3 v) {
            //Play jump audio clip;
            audioSource.PlayOneShot(jumpClip, audioClipVolume);
        }
    }
}