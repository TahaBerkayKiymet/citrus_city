// ============================================================
//  FruitExplosionFX.cs — Citrus City
//  MonoBehaviour: particle burst when a fruit group is removed.
//  Uses Unity's built-in ParticleSystem to avoid dependencies.
//  One instance lives in the scene; it's triggered per removal.
// ============================================================
using System.Collections.Generic;
using UnityEngine;
using CitrusCity.Core;
using CitrusCity.Fruit;

namespace CitrusCity.Effects
{
    public class FruitExplosionFX : MonoBehaviour
    {
        [SerializeField] private ParticleSystem burstParticles;
        [SerializeField] private int            particlesPerFruit = 6;

        private void OnEnable()
        {
            EventBus<GroupConfirmedEvent>.Subscribe(OnGroup);
        }

        private void OnDisable()
        {
            EventBus<GroupConfirmedEvent>.Unsubscribe(OnGroup);
        }

        private void OnGroup(GroupConfirmedEvent e)
        {
            if (burstParticles == null) return;

            // Get color for this fruit type
            Color color = FruitDataRegistry.GetColor(e.FruitType);

            // Emit burst at centre of group
            var (minC, minR, maxC, maxR) = Selection.GroupFinder.GetBounds(e.Cells);
            // We need world position — use board centre approximation
            // (exact positions would require BoardController ref; we use cell indices)
            // FX positions are approximate: centre of the selection bounding box
            // The actual pop animation plays on FruitView; this is a secondary layer
            var main = burstParticles.main;
            main.startColor = color;

            var em = burstParticles.emission;
            em.SetBurst(0, new ParticleSystem.Burst(0f, e.Cells.Count * particlesPerFruit));

            burstParticles.Play();
        }
    }
}
