using System;
using Cysharp.Threading.Tasks;
using IdleActionFarm.Runtime.Growing;
using UnityEngine;
using UnityEngine.InputSystem;

namespace IdleActionFarm.Runtime.Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] [Range(10f, 50f)] private float movementSpeed;
        [SerializeField] private Blade blade;

        private CharacterController _cc;
        private PlayerControls _playerControls;
        private Animator _animator;
        private Vector3 _curMovement;


        private void Awake()
        {
            _cc = GetComponent<CharacterController>();
            _animator = GetComponent<Animator>();
            _playerControls = new PlayerControls();
            _playerControls.Game.Move.performed += OnMove;
            _playerControls.Game.Harvest.performed += OnHarvest;

            blade.gameObject.SetActive(false);
        }


        private void Start()
        {
            _playerControls.Enable();
            _isHarvesting = false;
        }

        private bool _isHarvesting;

        private void OnHarvest(InputAction.CallbackContext obj)
        {
            if (_isHarvesting) return;

            _animator.Play("Harvesting");
            _isHarvesting = true;
            blade.gameObject.SetActive(true);
            blade.enabled = true;
            blade.HarvestingBegan();
        }

        // Called by animation events
        public void OnHarvestHalf() { blade.enabled = false; }

        public void OnHarvestEnded()
        {
            _isHarvesting = false;
            blade.gameObject.SetActive(false);
        }
        //

        private static readonly int Movement = Animator.StringToHash("movement");

        private void OnMove(InputAction.CallbackContext obj)
        {
            var raw = obj.ReadValue<Vector2>();
            _curMovement = new Vector3(raw.x, 0f, raw.y);

            _animator.SetFloat(Movement, _curMovement.magnitude);
        }

        [SerializeField] private float collisionForce = 50f;

        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            if (hit.rigidbody != null)
                hit.rigidbody.AddForce(_curMovement * collisionForce);
        }

        [SerializeField] private float gravity = 9.81f;

        private void Update()
        {
            _cc.Move(new Vector3(0f, -gravity, 0f) * Time.deltaTime);
            if (!_isHarvesting && _curMovement != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(_curMovement, Vector3.up);
                _cc.Move(_curMovement * (movementSpeed * Time.deltaTime));
            }
        }
    }
}