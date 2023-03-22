using FTK.UIToolkit.Util;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace FTK.UIToolkit.Containers
{
    [ExecuteInEditMode]
    public class AnchoredObjectContainer : MonoBehaviour
    {
        [SerializeField]
        private List<AnchoredGameObject> objects = new List<AnchoredGameObject>();

        private Queue<GameObject> toDestroy = new Queue<GameObject>();

        public List<AnchoredGameObject> Objects
        {
            get => objects;
            set
            {
                objects = value;
                Apply();
            }
        }

        public void Apply()
        {
            foreach (AnchoredGameObject obj in objects)
                if (obj.GameObject != null)
                    obj.GameObject.transform.parent = null;
            foreach (Transform child in transform)
                toDestroy.Enqueue(child.gameObject);
            for (int i = 0; i < objects.Count; i++)
            {
                AnchoredGameObject obj = objects[i];
                if (obj.GameObject != null)
                {
                    if (obj.GameObject.transform.parent?.parent == transform)
                    {
                        objects.RemoveAt(i);
                        i--;
                        continue;
                    }
                    GameObject anchor = new GameObject("Anchor[" + obj.GameObject.name + "]");
                    anchor.transform.parent = transform;
                    anchor.transform.localPosition = obj.Position + obj.GameObject.transform.localPosition;
                    anchor.transform.localRotation = obj.Rotation * obj.GameObject.transform.localRotation;
                    anchor.transform.localScale = Mult(obj.Scale, obj.GameObject.transform.localScale);
                    obj.GameObject.transform.parent = anchor.transform;
                    obj.GameObject.transform.localPosition = Vector3.zero;
                    obj.GameObject.transform.localRotation = Quaternion.identity;
                    obj.GameObject.transform.localScale = Vector3.one;
                }
            }

            
        }

        private static Vector3 Mult(Vector3 a, Vector3 b) => new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
        private void OnValidate()
        {
            foreach (AnchoredGameObject obj in objects)
                if (obj != null)
                    obj.EditorUpdate();
            Apply();
        }

        private void Update()
        {
            while (toDestroy.Count > 0)
            {
                GameObject obj = toDestroy.Dequeue();
                if (obj.name.StartsWith("Anchor[") && obj.name.EndsWith("]"))
                    obj.transform.DetachChildren();
                DestroyImmediate(obj);
            }
        }

    }

    [System.Serializable]
    public class AnchoredGameObject
    {
        static AnchoredGameObject() => new AnchoredGameObject();
        public AnchoredGameObject() { }
        public AnchoredGameObject(GameObject gameObject)
        {
            name = gameObject.name;
            this.gameObject = gameObject;
            initialized = true;
            gameObjectPresent = true;
        }
        [HideInInspector]
        public string name;
        [SerializeField]
        private GameObject gameObject = null;
        [SerializeField]
        [HideInInspector]
        private bool gameObjectPresent = false;
        [SerializeField]
        [ConditionalField("gameObjectPresent")]
        private Vector3 position = Vector3.zero;
        [SerializeField]
        [ConditionalField("gameObjectPresent")]
        private Vector3 rotation = Vector3.zero;
        [SerializeField]
        [ConditionalField("gameObjectPresent")]
        private Vector3 scale = Vector3.one;
        private bool initialized = false;
        public void EditorUpdate()
        {
            if (!initialized)
            {
                scale = Vector3.one;
                initialized = true;
            }
            gameObjectPresent = gameObject != null;
            if (gameObjectPresent)
                name = gameObject.name;
            else name = null;
        }
        public GameObject GameObject
        {
            get => gameObject;
            set
            {
                gameObjectPresent = value != null;
                gameObject = value;
                if (gameObjectPresent)
                    name = gameObject.name;
            }
        }
        public Vector3 Position
        {
            get => position;
            set { position = value; }
        }
        public Quaternion Rotation
        {
            get => Quaternion.Euler(rotation);
            set { rotation = value.eulerAngles; }
        }
        public Vector3 Scale
        {
            get => scale;
            set { scale = value; }
        }
    }
#if UNITY_EDITOR
    [CanEditMultipleObjects]
    [CustomEditor(typeof(AnchoredObjectContainer))]
    public class AnchoredObjectContainerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            AnchoredObjectContainer aoc = (AnchoredObjectContainer)target;
            GUILayout.BeginHorizontal();
            GUILayout.Space(5);
            if (GUILayout.Button("Add"))
                aoc.Objects.Add(new AnchoredGameObject());
            if (GUILayout.Button("Remove") && aoc.Objects.Count > 0)
            {
                aoc.Objects.RemoveAt(aoc.Objects.Count - 1);
                aoc.Apply();
            }
            GUILayout.EndHorizontal();
        }

    }
#endif
}
