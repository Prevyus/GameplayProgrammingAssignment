using Newtonsoft.Json.Linq;
using System.Xml;
using UnityEngine;

namespace Toolbox
{
    public static class tb
    {
        public static void deb(params object[] inputs)
        {
            string output = string.Join(" | ", inputs);
            Debug.Log(output);
        }

        public static Vector3 GetBreathingPositionOffset(float breathPosFrequency, float breathPosAmplitude)
        {
            float t = Time.time * breathPosFrequency;
            float vertical = Mathf.Sin(t) * breathPosAmplitude;
            float forward = Mathf.Cos(t * 0.5f) * breathPosAmplitude * 0.3f;
            return new Vector3(0f, vertical, forward);
        }

        public static Quaternion GetBreathingRotationOffset(float breathRotFrequency, float breathRotAmplitude)
        {
            float t = Time.time * breathRotFrequency;
            float pitch = Mathf.Sin(t) * breathRotAmplitude;
            float roll = Mathf.Cos(t) * breathRotAmplitude * 0.4f;
            return Quaternion.Euler(pitch, 0f, roll);
        }

        public static bool searchForTransformInChildren(Transform parent, Transform value)
        {
            if (parent == value) { return true; }

            for (int i = 0; i < parent.childCount; i++)
            {
                if (searchForTransformInChildren(parent.GetChild(i), value)) return true;
            }

            return false;
        }

        public static Vector3 IgnoreY(Vector3 value, float defaultY)
        {
            return new Vector3(value.x, defaultY, value.z);
        }

        public static int RandomRangeNoRepeat(int value, int min, int max)
        {
            int result = UnityEngine.Random.Range(min, max);
            if (result != value) return result;
            else return RandomRangeNoRepeat(value, min, max);
        }

        public static bool CheckForTagByRaycast(string tagName, Vector3 origin, Vector3 direction, float maxDistance, out GameObject hit, bool draw)
        {
            hit = null;
            RaycastHit[] hits = Physics.RaycastAll(origin, direction, maxDistance);
            if (draw) Debug.DrawLine(origin, direction * maxDistance, Color.red, .5f);
            if (hits.Length > 0)
            {
                for (int i = 0; i < hits.Length; i++)
                {
                    if (hits[i].transform.CompareTag(tagName))
                    {
                        hit = hits[i].transform.gameObject;
                        return true;
                    }
                }
            }
            return false;
        }

        public static bool CheckForComponentAndTagByRaycast<T>(string tagName, Vector3 origin, Vector3 direction, float maxDistance, out T hitComponent, out GameObject hitGameObject, bool draw)
        {
            hitComponent = default;
            hitGameObject = null;
            RaycastHit[] hits = Physics.RaycastAll(origin, direction, maxDistance);
            if (draw) Debug.DrawLine(origin, direction * maxDistance, Color.red, .5f);
            if (hits.Length > 0)
            {
                for (int i = 0; i < hits.Length; i++)
                {
                    T componentHit = hits[i].transform.GetComponent<T>();
                    GameObject gameObjectHit = hits[i].transform.gameObject;
                    if (hits[i].transform.CompareTag(tagName) && componentHit == null)
                    {
                        componentHit = hits[i].transform.GetComponentInParent<T>();
                    }
                    if (componentHit != null)
                    {
                        hitComponent = componentHit;
                        hitGameObject = gameObjectHit;
                        return true;
                    }
                }
            }
            return false;
        }

        public static bool CheckForLayerByRaycast(LayerMask layerMask, Vector3 origin, Vector3 direction, float maxDistance, out Collider hit, bool draw)
        {
            hit = null;
            RaycastHit[] hits = Physics.RaycastAll(origin, direction, maxDistance);
            if (draw) Debug.DrawLine(origin, direction * maxDistance, Color.red, .5f);
            if (hits.Length > 0)
            {
                for (int i = 0; i < hits.Length; i++)
                {
                    if (((1 << hits[i].collider.gameObject.layer) & layerMask) != 0)
                    {
                        hit = hits[i].collider;
                        return true;
                    }
                }
            }
            return false;
        }

        public static bool CheckForComponentByRaycast<T>(Vector3 origin, Vector3 direction, float maxDistance, out T hit, bool draw)
        {
            hit = default;
            RaycastHit[] hits = Physics.RaycastAll(origin, direction, maxDistance);
            if (draw) Debug.DrawLine(origin, direction * maxDistance, Color.red, .5f);
            if (hits.Length > 0)
            {
                for (int i = 0; i < hits.Length; i++)
                {
                    T interfaceHit = hits[i].collider.GetComponent<T>();
                    if (interfaceHit != null)
                    {
                        hit = interfaceHit;
                        return true;
                    }
                }
            }
            return false;
        }
        public static bool CheckForInterfaceByRaycast<T>(Vector3 origin, Vector3 direction, float maxDistance, out T hit, out GameObject hitGameObject, bool draw)
        {
            hit = default;
            hitGameObject = null;
            RaycastHit[] hits = Physics.RaycastAll(origin, direction, maxDistance);
            if (draw) Debug.DrawLine(origin, direction * maxDistance, Color.red, .5f);
            if (hits.Length > 0)
            {
                for (int i = 0; i < hits.Length; i++)
                {
                    T interfaceHit = hits[i].collider.GetComponent<T>();
                    GameObject gameObjectHit = hits[i].collider.gameObject;
                    if (interfaceHit != null)
                    {
                        hit = interfaceHit;
                        hitGameObject = gameObjectHit;
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
