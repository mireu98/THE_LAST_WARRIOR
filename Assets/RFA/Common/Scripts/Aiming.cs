using UnityEngine;
using Cinemachine;

namespace Retro.ThirdPersonCharacter
{
    public class Aiming : MonoBehaviour
    {
        public float turnspeed = 15;
        private Transform _camTf;
        private Combat _combat;

        private void Start()
        {
            _camTf = Camera.main != null ? Camera.main.transform : null;
            _combat = GetComponent<Combat>();
        }

        void LateUpdate()
        {
            if (_camTf == null) return;
            float yaw = _camTf.rotation.eulerAngles.y;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, yaw, 0), turnspeed * Time.deltaTime);
        }
    }
}