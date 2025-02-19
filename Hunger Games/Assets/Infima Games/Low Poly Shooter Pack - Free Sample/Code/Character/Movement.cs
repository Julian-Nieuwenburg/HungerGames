// Copyright 2021, Infima Games. All Rights Reserved.

using System.Linq;
using UnityEngine;

namespace InfimaGames.LowPolyShooterPack
{
    [RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
    public class Movement : MovementBehaviour
    {
        #region FIELDS SERIALIZED

        [Header("Audio Clips")]
        [Tooltip("The audio clip that is played while walking.")]
        [SerializeField] private AudioClip audioClipWalking;

        [Tooltip("The audio clip that is played while running.")]
        [SerializeField] private AudioClip audioClipRunning;

        [Header("Speeds")]
        [SerializeField] private float speedWalking = 5.0f;
        [Tooltip("How fast the player moves while running."), SerializeField]
        private float speedRunning = 9.0f;

        #endregion

        #region FIELDS

        private Rigidbody rigidBody;
        private CapsuleCollider capsule;
        private AudioSource audioSource;

        private bool grounded;
        private CharacterBehaviour playerCharacter;
        private WeaponBehaviour equippedWeapon;
        private readonly RaycastHit[] groundHits = new RaycastHit[8];

        #endregion

        #region UNITY FUNCTIONS

        /// <summary>
        /// Awake: Ensures necessary components are assigned.
        /// </summary>
        protected override void Awake()
        {
            // ✅ Assign Player Character Safely
            playerCharacter = ServiceLocator.Current?.Get<IGameModeService>()?.GetPlayerCharacter();

            if (playerCharacter == null)
            {
                Debug.LogError("❌ playerCharacter is NULL in Movement.cs! Make sure the Player has a CharacterBehaviour component.");
                return;
            }

            // ✅ Check if Player Has Inventory
            if (playerCharacter.GetInventory() == null)
            {
                Debug.LogError("❌ Inventory is NULL in Movement.cs! Make sure the Player has an Inventory component attached.");
            }

            // ✅ Assign Rigidbody
            rigidBody = GetComponent<Rigidbody>();
            if (rigidBody == null)
            {       
                Debug.LogError("❌ Rigidbody is missing on Player!");
            }
            else
            {
                rigidBody.constraints = RigidbodyConstraints.FreezeRotation;
            }

            // ✅ Assign CapsuleCollider
            capsule = GetComponent<CapsuleCollider>();
            if (capsule == null)
            {
                Debug.LogError("❌ CapsuleCollider is missing on Player!");
            }


        }


        /// <summary>
        /// Checks if the player is grounded.
        /// </summary>
        private void OnCollisionStay()
        {
            if (capsule == null) return;

            Bounds bounds = capsule.bounds;
            Vector3 extents = bounds.extents;
            float radius = extents.x - 0.01f;

            // ✅ Check if Player is Touching the Ground
            if (Physics.SphereCastNonAlloc(bounds.center, radius, Vector3.down,
                groundHits, extents.y - radius * 0.5f, ~0, QueryTriggerInteraction.Ignore) > 0)
            {
                grounded = true;
            }
        }

        #endregion

        #region MOVEMENT METHODS

        /// <summary>
        /// Moves the character based on input.
        /// </summary>
        private void MoveCharacter()
        {
            if (playerCharacter == null) return;

            // ✅ Get Movement Input
            Vector2 frameInput = playerCharacter.GetInputMovement();
            Vector3 movement = new Vector3(frameInput.x, 0.0f, frameInput.y);

            // ✅ Adjust Speed for Running
            movement *= playerCharacter.IsRunning() ? speedRunning : speedWalking;

            // ✅ Convert Local to World Space Movement
            movement = transform.TransformDirection(movement);

            // ✅ Update Rigidbody Velocity
            if (rigidBody != null)
            {
                rigidBody.linearVelocity = new Vector3(movement.x, rigidBody.linearVelocity.y, movement.z);
            }
        }

        /// <summary>
        /// Handles footstep sounds based on movement.
        /// </summary>
        private void PlayFootstepSounds()
        {
            if (audioSource == null) return;

            if (grounded && rigidBody.linearVelocity.sqrMagnitude > 0.1f)
            {
                // ✅ Select Correct Footstep Sound
                audioSource.clip = playerCharacter.IsRunning() ? audioClipRunning : audioClipWalking;

                // ✅ Play Footstep Sound if Not Already Playing
                if (!audioSource.isPlaying)
                    audioSource.Play();
            }
            else
            {
                // ✅ Pause Footstep Sound When Not Moving
                audioSource.Pause();
            }
        }

        #endregion
    }
}
