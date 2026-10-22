// ============================================================
//  BoosterActivateFX.cs — Citrus City
//  MonoBehaviour: visual effects for booster activation.
//  Rocket: sweeps a flash across the row/column.
//  Bomb: radial burst.
//  Vortex: swirl + color flash.
// ============================================================
using System.Collections;
using UnityEngine;
using CitrusCity.Core;
using CitrusCity.Boosters;

namespace CitrusCity.Effects
{
    public class BoosterActivateFX : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private ParticleSystem rocketParticles;
        [SerializeField] private ParticleSystem bombParticles;
        [SerializeField] private ParticleSystem vortexParticles;

        [Header("Screen Flash")]
        [SerializeField] private UnityEngine.UI.Image screenFlash;

        private void OnEnable()
        {
            EventBus<BoosterActivatedEvent>.Subscribe(OnActivate);
        }

        private void OnDisable()
        {
            EventBus<BoosterActivatedEvent>.Unsubscribe(OnActivate);
        }

        private void OnActivate(BoosterActivatedEvent e)
        {
            StartCoroutine(PlayEffect(e));
        }

        private IEnumerator PlayEffect(BoosterActivatedEvent e)
        {
            switch (e.Type)
            {
                case BoosterType.Rocket:
                    rocketParticles?.Play();
                    yield return ScreenFlash(new Color(0.3f, 0.6f, 1f, 0.3f));
                    break;

                case BoosterType.Bomb:
                    bombParticles?.Play();
                    yield return ScreenFlash(new Color(0.9f, 0.5f, 0.1f, 0.4f));
                    break;

                case BoosterType.Vortex:
                    vortexParticles?.Play();
                    yield return ScreenFlash(new Color(1f, 0.4f, 0.9f, 0.5f));
                    break;
            }
        }

        private IEnumerator ScreenFlash(Color color)
        {
            if (screenFlash == null) yield break;
            screenFlash.color = color;
            float t = 0;
            while (t < 0.3f)
            {
                t += Time.deltaTime;
                var c = color;
                c.a = Mathf.Lerp(color.a, 0f, t / 0.3f);
                screenFlash.color = c;
                yield return null;
            }
            screenFlash.color = Color.clear;
        }
    }
}
