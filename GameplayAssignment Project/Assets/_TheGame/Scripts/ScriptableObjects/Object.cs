using UnityEngine;

namespace Custom
{
    [CreateAssetMenu(fileName = "ObjectSO", menuName = "Scriptable Objects/ObjectSO")]
    public class Object : ScriptableObject
    {
        public int id;
        public string ObjectId;
        public Texture itemImage;
        public int stackSize;
        public GameObject InGroundPrefab;
        public GameObject InHandPrefab;
        public bool Deployable;
        public bool Holdable;
    }
}