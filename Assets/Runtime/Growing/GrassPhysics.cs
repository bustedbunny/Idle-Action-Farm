using UnityEngine;

namespace IdleActionFarm.Runtime.Growing
{
    public class GrassPhysics : MonoBehaviour
    {
        private Rigidbody _rigidbody;
        private void Awake() { _rigidbody = GetComponent<Joint>().connectedBody; }

        private void FixedUpdate() { _rigidbody.AddForce(new Vector3(0f, 10f, 0f), ForceMode.Impulse); }
    }
}