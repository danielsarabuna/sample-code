using System;
using System.Collections.Generic;

namespace View
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "EasePresets", menuName = "Animation/EasePresets")]
    public class EasePresets : ScriptableObject
    {
        //https://d3kjluh73b9h9o.cloudfront.net/original/3X/a/a/aa952478c253cabf54fcc29fd8a88f86b3a83b8b.jpeg
        [SerializeField] private AnimationCurve linear = AnimationCurve.Linear(0, 0, 1, 1);
        [SerializeField] private AnimationCurve inSine = AnimationCurve.Linear(0, 0, 1, 1);
        [SerializeField] private AnimationCurve outSine = AnimationCurve.Linear(0, 0, 1, 1);
        [SerializeField] private AnimationCurve inOutSine = AnimationCurve.Linear(0, 0, 1, 1);
        [SerializeField] private AnimationCurve inQuad = AnimationCurve.Linear(0, 0, 1, 1);
        [SerializeField] private AnimationCurve outQuad = AnimationCurve.Linear(0, 0, 1, 1);
        [SerializeField] private AnimationCurve inOutQuad = AnimationCurve.Linear(0, 0, 1, 1);
        [SerializeField] private AnimationCurve inCubic = AnimationCurve.Linear(0, 0, 1, 1);
        [SerializeField] private AnimationCurve outCubic = AnimationCurve.Linear(0, 0, 1, 1);
        [SerializeField] private AnimationCurve inOutCubic = AnimationCurve.Linear(0, 0, 1, 1);
        [SerializeField] private AnimationCurve inQuart = AnimationCurve.Linear(0, 0, 1, 1);
        [SerializeField] private AnimationCurve outQuart = AnimationCurve.Linear(0, 0, 1, 1);
        [SerializeField] private AnimationCurve inOutQuart = AnimationCurve.Linear(0, 0, 1, 1);
        [SerializeField] private AnimationCurve inExpo = AnimationCurve.Linear(0, 0, 1, 1);
        [SerializeField] private AnimationCurve outExpo = AnimationCurve.Linear(0, 0, 1, 1);
        [SerializeField] private AnimationCurve inOutExpo = AnimationCurve.Linear(0, 0, 1, 1);
        [SerializeField] private AnimationCurve inElastic = AnimationCurve.Linear(0, 0, 1, 1);
        [SerializeField] private AnimationCurve outElastic = AnimationCurve.Linear(0, 0, 1, 1);
        [SerializeField] private AnimationCurve inOutElastic = AnimationCurve.Linear(0, 0, 1, 1);
        [SerializeField] private AnimationCurve inBounce = AnimationCurve.Linear(0, 0, 1, 1);
        [SerializeField] private AnimationCurve outBounce = AnimationCurve.Linear(0, 0, 1, 1);
        [SerializeField] private AnimationCurve inOutBounce = AnimationCurve.Linear(0, 0, 1, 1);
        [SerializeField] private AnimationCurve inBack = AnimationCurve.Linear(0, 0, 1, 1);
        [SerializeField] private AnimationCurve outBack = AnimationCurve.Linear(0, 0, 1, 1);
        [SerializeField] private AnimationCurve inOutBack = AnimationCurve.Linear(0, 0, 1, 1);
        private Dictionary<string, AnimationCurve> _animationCurves;

        public void Bind()
        {
            _animationCurves = new Dictionary<string, AnimationCurve>
            {
                { nameof(linear), linear },
                { nameof(inSine), inSine },
                { nameof(outSine), outSine },
                { nameof(inOutSine), inOutSine },
                { nameof(inQuad), inQuad },
                { nameof(outQuad), outQuad },
                { nameof(inOutQuad), inOutQuad },
                { nameof(inCubic), inCubic },
                { nameof(outCubic), outCubic },
                { nameof(inOutCubic), inOutCubic },
                { nameof(inQuart), inQuart },
                { nameof(outQuart), outQuart },
                { nameof(inOutQuart), inOutQuart },
                { nameof(inExpo), inExpo },
                { nameof(outExpo), outExpo },
                { nameof(inOutExpo), inOutExpo },
                { nameof(inElastic), inElastic },
                { nameof(outElastic), outElastic },
                { nameof(inOutElastic), inOutElastic },
                { nameof(inBounce), inBounce },
                { nameof(outBounce), outBounce },
                { nameof(inOutBounce), inOutBounce },
                { nameof(inBack), inBack },
                { nameof(outBack), outBack },
                { nameof(inOutBack), inOutBack }
            };
        }

        public Func<float, float> Get(string key)
        {
            if (_animationCurves != null && _animationCurves.TryGetValue(key, out var curve))
                return value => curve.Evaluate(value);

            throw new ArgumentException($"No curve found for key: {key}");
        }
    }
}