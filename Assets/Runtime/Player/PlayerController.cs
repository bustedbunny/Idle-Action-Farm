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


        private CharacterController _characterController;
        private PlayerControls _playerControls;
        private Animator _animator;
        private Vector3 _curMovement;
        private static readonly int Movement = Animator.StringToHash("movement");

        private void Awake()
        {
            _characterController = GetComponent<CharacterController>();
            _animator = GetComponent<Animator>();
            _playerControls = new PlayerControls();
            _playerControls.Game.Move.performed += OnMove;
            _playerControls.Game.Harvest.performed += OnHarvest;

            blade.gameObject.SetActive(false);
        }


        private void Start()
        {
            _playerControls.Enable();
            isHarvesting = false;
        }

        private bool isHarvesting;

        private void OnHarvest(InputAction.CallbackContext obj)
        {
            _animator.Play("Harvesting");
            isHarvesting = true;
            blade.gameObject.SetActive(true);
            blade.enabled = true;
            blade.HarvestingBegan();
        }

        public void OnHarvestHalf() { blade.enabled = false; }

        public void OnHarvestEnded()
        {
            isHarvesting = false;
            blade.gameObject.SetActive(false);
        }


        private void OnMove(InputAction.CallbackContext obj)
        {
            var raw = obj.ReadValue<Vector2>();
            _curMovement = new Vector3(raw.x, 0f, raw.y);

            _animator.SetFloat(Movement, _curMovement != Vector3.zero ? 1f : 0f);
        }

        private void Update()
        {
            if (!isHarvesting && _curMovement != Vector3.zero)
            {
                transform.rotation =
                    Quaternion.LookRotation(_curMovement, Vector3.up);

                _characterController.Move(_curMovement * (movementSpeed * Time.deltaTime));
            }
        }
    }
}