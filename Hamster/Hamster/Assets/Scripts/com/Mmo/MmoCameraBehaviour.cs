using System.Collections;
using UnityEngine;

namespace com
{
    public class MmoCameraBehaviour : MonoBehaviour
    {
        public Transform target;

        [Header("摄像机参数，目前支持2个")]
        [SerializeField] private MmoCameraParameters[] _params;
        private int _paramIndex = 0;
        public MmoCameraParameters _param;
        public float switchDuration = 2f;
        private void Start()
        {
            SetParam(0);
        }


        public bool testToggle;

        void LateUpdate()
        {
            if (testToggle)
            {
                testToggle = false;
                ToggleParam();
            }
            Sync();
        }

        private void OnEnable()
        {
            Sync();
        }

        public void ToggleParam()
        {
            var newIndex = (_paramIndex == 0) ? 1 : 0;


            StopAllCoroutines();
            StartCoroutine(TweenParam(_param, _params[newIndex], switchDuration, newIndex));
        }

        /// <summary>
        /// 切换摄像机机位
        /// </summary>
        /// <param name="p1">原本的机位参数</param>
        /// <param name="p2">新的机位参数</param>
        /// <param name="duration">切换时间长度</param>
        /// <param name="i">新的机位参数对应的_params中的索引</param>
        /// <returns></returns>
        IEnumerator TweenParam(MmoCameraParameters p1, MmoCameraParameters p2, float duration, int i)
        {

            float t = duration;
            while (t > 0)
            {
                yield return null;
                t -= Time.deltaTime;
                if (t < 0)
                    t = 0;
                var r = 1 - t / duration;

                //创建一个新的机位参数，并用插值算法赋值
                MmoCameraParameters p = new MmoCameraParameters();
                p.offset = Vector3.Lerp(p1.offset, p2.offset, r);
                p.pitch = Mathf.Lerp(p1.pitch, p2.pitch, r);
                p.yaw = Mathf.Lerp(p1.yaw, p2.yaw, r);
                p.distance = Mathf.Lerp(p1.distance, p2.distance, r);
                _param = p;

            }

            //切换结束，重设机位参数
            SetParam(i);
        }

        void SetParam(int i)
        {
            _paramIndex = i;
            _param = _params[_paramIndex];
        }

        public void Sync()
        {
            var backward = -Vector3.forward;
            var yawed = backward * Mathf.Cos(_param.yaw) + Vector3.right * Mathf.Sin(_param.yaw);
            var ideaPos = target.position + (yawed * Mathf.Cos(_param.pitch) + Vector3.up * Mathf.Sin(_param.pitch)) * _param.distance;
            transform.position = ideaPos;
            transform.rotation = Quaternion.LookRotation(target.position - transform.position);
            transform.position += _param.offset;
        }

        public void SetPosAndRot(ref Vector3 pos, ref Quaternion rot)
        {
            var backward = -Vector3.forward;
            var yawed = backward * Mathf.Cos(_param.yaw) + Vector3.right * Mathf.Sin(_param.yaw);
            var ideaPos = target.position + (yawed * Mathf.Cos(_param.pitch) + Vector3.up * Mathf.Sin(_param.pitch)) * _param.distance;
            pos = ideaPos;
            rot = Quaternion.LookRotation(target.position - pos);
            pos = pos + _param.offset;
        }
    }
}