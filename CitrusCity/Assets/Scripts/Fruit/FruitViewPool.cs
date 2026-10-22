// ============================================================
//  FruitViewPool.cs — Citrus City
//  MonoBehaviour: object pool for FruitView GameObjects.
//  Keeps pool in the scene, reuses GameObjects to avoid GC.
// ============================================================
using System.Collections.Generic;
using UnityEngine;
using CitrusCity.Fruit;

namespace CitrusCity.Fruit
{
    public class FruitViewPool : MonoBehaviour
    {
        [SerializeField] private GameObject fruitPrefab;
        [SerializeField] private int initialPoolSize = 100;

        private readonly Queue<FruitView> _pool = new();

        private void Awake()
        {
            if (fruitPrefab == null)
                fruitPrefab = CreateDefaultFruitPrefab();

            for (int i = 0; i < initialPoolSize; i++)
                _pool.Enqueue(CreateInstance());
        }

        // ── API ───────────────────────────────────────────────

        public FruitView Rent(Vector3 worldPos, Transform parent)
        {
            FruitView view;
            if (_pool.Count > 0)
                view = _pool.Dequeue();
            else
                view = CreateInstance();

            view.transform.SetParent(parent, false);
            view.transform.position  = worldPos;
            view.transform.localScale = Vector3.one;
            view.gameObject.SetActive(true);
            return view;
        }

        public void Return(FruitView view)
        {
            view.ResetForPool();
            view.gameObject.SetActive(false);
            view.transform.SetParent(transform, false);
            _pool.Enqueue(view);
        }

        // ── Private helpers ───────────────────────────────────

        private FruitView CreateInstance()
        {
            var go   = Instantiate(fruitPrefab, transform);
            go.SetActive(false);
            var view = go.GetComponent<FruitView>();
            if (view == null) view = go.AddComponent<FruitView>();
            return view;
        }

        private static GameObject CreateDefaultFruitPrefab()
        {
            var go = new GameObject("FruitCell");
            go.AddComponent<SpriteRenderer>();
            go.AddComponent<FruitView>();
            return go;
        }
    }
}
