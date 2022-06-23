using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.OnScreen;
using UnityEngine.Serialization;

namespace Runtime.Input
{
    public class MovableJoystick : OnScreenControl, IPointerDownHandler, IPointerUpHandler
    {
        [InputControl(layout = "Vector2")] [SerializeField]
        private string mControlPath;

        [SerializeField] private RectTransform circle;
        [SerializeField] private RectTransform stick;

        protected override string controlPathInternal { get => mControlPath; set => mControlPath = value; }

        private Vector2 _pointerDownPos;

        private RectTransform _parentRect;
        private void Awake() { _parentRect = transform.parent.GetComponentInParent<RectTransform>(); }

        private void Start() { SetState(false); }

        private void SetState(bool state)
        {
            circle.gameObject.SetActive(state);
            stick.gameObject.SetActive(state);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData == null)
                throw new ArgumentNullException(nameof(eventData));

            RectTransformUtility.ScreenPointToLocalPointInRectangle(_parentRect,
                eventData.position,
                eventData.pressEventCamera,
                out _pointerDownPos);

            circle.anchoredPosition = _pointerDownPos;
            stick.anchoredPosition = _pointerDownPos;

            SetState(true);
        }

        public void OnPointerUp(PointerEventData eventData) { SetState(false); }
    }
}