using System;
using System.Collections;
using UnityEngine;

namespace FTK.UIToolkit.Util
{
    public abstract class AnimatableBase : MonoBehaviour
    {
        private static readonly float epsilon = 0.000001f;

        private MappingList<string, AnimationData> currentAnimations = new MappingList<string, AnimationData>();
        private bool coroutineRunning = false;
        private bool startCoroutine = false;
        protected void Animate(string id, float from, float target, float time, Action<float> onAnimate)
        {
            if (IsSimilar(from, target)) return;
            time = Mathf.Max(time, 0f);
            float now = Time.time;
            AnimationData data = new FloatAnimationData(from, target, new Vector2(now, now + time), onAnimate);
            EnqueueAnimation(id, data);
        }

        protected void Animate(string id, Vector2 from, Vector2 target, float time, Action<Vector2> onAnimate)
        {
            if (from == null) from = Vector2.zero;
            if (target == null) target = Vector2.zero;
            if (IsSimilar(from, target)) return;
            time = Mathf.Max(time, 0f);
            float now = Time.time;
            AnimationData data = new Vector2AnimationData(from, target, new Vector2(now, now + time), onAnimate);
            EnqueueAnimation(id, data);
        }

        protected void Animate(string id, Vector3 from, Vector3 target, float time, Action<Vector3> onAnimate)
        {
            if (from == null) from = Vector3.zero;
            if (target == null) target = Vector3.zero;
            if (IsSimilar(from, target)) return;
            time = Mathf.Max(time, 0f);
            float now = Time.time;
            AnimationData data = new Vector3AnimationData(from, target, new Vector2(now, now + time), onAnimate);
            EnqueueAnimation(id, data);
        }

        protected void Animate(string id, Vector4 from, Vector4 target, float time, Action<Vector4> onAnimate)
        {
            if (from == null) from = Vector4.zero;
            if (target == null) target = Vector4.zero;
            if (IsSimilar(from, target)) return;
            time = Mathf.Max(time, 0f);
            float now = Time.time;
            AnimationData data = new Vector4AnimationData(from, target, new Vector2(now, now + time), onAnimate);
            EnqueueAnimation(id, data);
        }

        protected void Animate(string id, Color from, Color target, float time, Action<Color> onAnimate)
        {
            if (from == null) from = new Color(0, 0, 0, 0);
            if (target == null) target = new Color(0, 0, 0, 0);
            if (IsSimilar(from, target)) return;
            time = Mathf.Max(time, 0f);
            float now = Time.time;
            AnimationData data = new ColorAnimationData(from, target, new Vector2(now, now + time), onAnimate);
            EnqueueAnimation(id, data);
        }

        protected void Animate(string id, Quaternion from, Quaternion target, float time, Action<Quaternion> onAnimate)
        {
            if (from == null) from = Quaternion.identity;
            if (target == null) target = Quaternion.identity;
            if (IsSimilar(from, target)) return;
            time = Mathf.Max(time, 0f);
            float now = Time.time;
            AnimationData data = new QuaternionAnimationData(from, target, new Vector2(now, now + time), onAnimate);
            EnqueueAnimation(id, data);
        }

        protected bool IsAnimationRunning(string id) => currentAnimations.Contains(id);

        protected void StopAnimation(string id) => currentAnimations.Remove(id);

        protected static float GetInterpolationFactor(float a, float b, float t)
        {
            if (a + .000001f > b) a = b - 0.000001f;
            float fac = Mathf.Clamp((t - a) / (b - a), 0, 1); //linear interpolation factor
            return fac * fac * (3f - 2f * fac); //linear to cubic interpolation <- smoothstep-function
        }

        protected bool IsSimilar(float a, float b) => Mathf.Abs(a - b) <= epsilon;
        protected bool IsSimilar(Vector2 a, Vector2 b) => IsSimilar(a.x, b.x) && IsSimilar(a.y, b.y);
        protected bool IsSimilar(Vector3 a, Vector3 b) => IsSimilar(a.x, b.x) && IsSimilar(a.y, b.y) && IsSimilar(a.z, b.z);
        protected bool IsSimilar(Vector4 a, Vector4 b) => IsSimilar(a.x, b.x) && IsSimilar(a.y, b.y) && IsSimilar(a.z, b.z) && IsSimilar(a.w, b.w);
        protected bool IsSimilar(Quaternion a, Quaternion b) => IsSimilar(a.x, b.x) && IsSimilar(a.y, b.y) && IsSimilar(a.z, b.z) && IsSimilar(a.w, b.w);
        protected bool IsSimilar(Color a, Color b) => IsSimilar((Vector4)a, (Vector4)b);

        private void EnqueueAnimation(string id, AnimationData data)
        {
            if (id == null) id = "";
            if (currentAnimations.Contains(id))
                data = currentAnimations[id].CombineWith(data);
            currentAnimations[id] = data;
            if (!coroutineRunning)
                if (isActiveAndEnabled && gameObject.activeInHierarchy) StartCoroutine(AnimationCoroutine());
                else startCoroutine = true;
        }

        protected virtual void OnEnable() { if (startCoroutine) StartCoroutine(AnimationCoroutine()); }

        protected virtual void OnDisable()
        {
            if (coroutineRunning)
            {
                StopCoroutine(AnimationCoroutine());
                startCoroutine = true;
            }
        }
        private IEnumerator AnimationCoroutine()
        {
            startCoroutine = false;
            coroutineRunning = true;
            while (currentAnimations.Count > 0)
            {
                float time = Time.time;
                for (int i = 0; i < currentAnimations.Count; i++)
                {
                    AnimationData data = currentAnimations.GetAt(i);
                    data.Animate(time);
                    if (data.IsOver(time))
                    {
                        currentAnimations.RemoveAt(i);
                        i--;
                    }
                }
                yield return null;
            }
            coroutineRunning = false;
        }

        private abstract class AnimationData
        {
            public abstract bool IsOver(float time);
            public abstract void Animate(float time);
            public abstract AnimationData CombineWith(AnimationData newAnimation);
        }

        private class FloatAnimationData : AnimationData
        {
            public FloatAnimationData(float from, float target, Vector2 timeStartStop, Action<float> callback)
            {
                From = from;
                Target = target;
                TimeStartStop = timeStartStop;
                Callback = callback;
            }
            public readonly float From;
            public readonly float Target;
            public readonly Vector2 TimeStartStop;
            public readonly Action<float> Callback;
            public override bool IsOver(float time) { return time > TimeStartStop.y; }
            public override void Animate(float time) { Callback?.Invoke(AnimateNoInvoke(time)); }
            private float AnimateNoInvoke(float time)
            {
                float factor = GetInterpolationFactor(TimeStartStop.x, TimeStartStop.y, time);
                return Mathf.Lerp(From, Target, factor);
            }
            public override AnimationData CombineWith(AnimationData newAnimation)
            {
                if (newAnimation.GetType() == typeof(FloatAnimationData))
                {
                    FloatAnimationData newFloatAnimation = (FloatAnimationData) newAnimation;
                    return new FloatAnimationData(
                        AnimateNoInvoke(newFloatAnimation.TimeStartStop.x),
                        newFloatAnimation.Target,
                        newFloatAnimation.TimeStartStop,
                        newFloatAnimation.Callback);
                }
                else return this;
            }
        }

        private class Vector2AnimationData : AnimationData
        {
            public Vector2AnimationData(Vector2 from, Vector2 target, Vector2 timeStartStop, Action<Vector2> callback)
            {
                From = from;
                Target = target;
                TimeStartStop = timeStartStop;
                Callback = callback;
            }
            public readonly Vector2 From;
            public readonly Vector2 Target;
            public readonly Vector2 TimeStartStop;
            public readonly Action<Vector2> Callback;
            public override bool IsOver(float time)
            {
                return time > TimeStartStop.y;
            }
            public override void Animate(float time) { Callback?.Invoke(AnimateNoInvoke(time)); }
            private Vector2 AnimateNoInvoke(float time)
            {
                float factor = GetInterpolationFactor(TimeStartStop.x, TimeStartStop.y, time);
                return Vector2.Lerp(From, Target, factor);
            }
            public override AnimationData CombineWith(AnimationData newAnimation)
            {
                if (newAnimation.GetType() == typeof(Vector2AnimationData))
                {
                    Vector2AnimationData newVector2Animation = (Vector2AnimationData)newAnimation;
                    return new Vector2AnimationData(
                        AnimateNoInvoke(newVector2Animation.TimeStartStop.x),
                        newVector2Animation.Target,
                        newVector2Animation.TimeStartStop,
                        newVector2Animation.Callback);
                }
                else return this;
            }
        }

        private class Vector3AnimationData : AnimationData
        {
            public Vector3AnimationData(Vector3 from, Vector3 target, Vector2 timeStartStop, Action<Vector3> callback)
            {
                From = from;
                Target = target;
                TimeStartStop = timeStartStop;
                Callback = callback;
            }
            public readonly Vector3 From;
            public readonly Vector3 Target;
            public readonly Vector2 TimeStartStop;
            public readonly Action<Vector3> Callback;
            public override bool IsOver(float time)
            {
                return time > TimeStartStop.y;
            }
            public override void Animate(float time) { Callback?.Invoke(AnimateNoInvoke(time)); }
            private Vector3 AnimateNoInvoke(float time)
            {
                float factor = GetInterpolationFactor(TimeStartStop.x, TimeStartStop.y, time);
                return Vector3.Lerp(From, Target, factor);
            }
            public override AnimationData CombineWith(AnimationData newAnimation)
            {
                if (newAnimation.GetType() == typeof(Vector3AnimationData))
                {
                    Vector3AnimationData newVector3Animation = (Vector3AnimationData)newAnimation;
                    return new Vector3AnimationData(
                        AnimateNoInvoke(newVector3Animation.TimeStartStop.x),
                        newVector3Animation.Target,
                        newVector3Animation.TimeStartStop,
                        newVector3Animation.Callback);
                }
                else return this;
            }
        }

        private class Vector4AnimationData : AnimationData
        {
            public Vector4AnimationData(Vector4 from, Vector4 target, Vector2 timeStartStop, Action<Vector4> callback)
            {
                From = from;
                Target = target;
                TimeStartStop = timeStartStop;
                Callback = callback;
            }
            public readonly Vector4 From;
            public readonly Vector4 Target;
            public readonly Vector2 TimeStartStop;
            public readonly Action<Vector4> Callback;
            public override bool IsOver(float time)
            {
                return time > TimeStartStop.y;
            }
            public override void Animate(float time) { Callback?.Invoke(AnimateNoInvoke(time)); }
            private Vector4 AnimateNoInvoke(float time)
            {
                float factor = GetInterpolationFactor(TimeStartStop.x, TimeStartStop.y, time);
                return Vector4.Lerp(From, Target, factor);
            }
            public override AnimationData CombineWith(AnimationData newAnimation)
            {
                if (newAnimation.GetType() == typeof(Vector4AnimationData))
                {
                    Vector4AnimationData newVector4Animation = (Vector4AnimationData)newAnimation;
                    return new Vector4AnimationData(
                        AnimateNoInvoke(newVector4Animation.TimeStartStop.x),
                        newVector4Animation.Target,
                        newVector4Animation.TimeStartStop,
                        newVector4Animation.Callback);
                }
                else return this;
            }
        }

        private class ColorAnimationData : AnimationData
        {
            public ColorAnimationData(Color from, Color target, Vector2 timeStartStop, Action<Color> callback)
            {
                From = from;
                Target = target;
                TimeStartStop = timeStartStop;
                Callback = callback;
            }
            public readonly Color From;
            public readonly Color Target;
            public readonly Vector2 TimeStartStop;
            public readonly Action<Color> Callback;
            public override bool IsOver(float time)
            {
                return time > TimeStartStop.y;
            }
            public override void Animate(float time) { Callback?.Invoke(AnimateNoInvoke(time)); }
            private Color AnimateNoInvoke(float time)
            {
                float factor = GetInterpolationFactor(TimeStartStop.x, TimeStartStop.y, time);
                return Color.Lerp(From, Target, factor);
            }
            public override AnimationData CombineWith(AnimationData newAnimation)
            {
                if (newAnimation.GetType() == typeof(ColorAnimationData))
                {
                    ColorAnimationData newColorAnimation = (ColorAnimationData)newAnimation;
                    return new ColorAnimationData(
                        AnimateNoInvoke(newColorAnimation.TimeStartStop.x),
                        newColorAnimation.Target,
                        newColorAnimation.TimeStartStop,
                        newColorAnimation.Callback);
                }
                else return this;
            }
        }

        private class QuaternionAnimationData : AnimationData
        {
            public QuaternionAnimationData(Quaternion from, Quaternion target, Vector2 timeStartStop, Action<Quaternion> callback)
            {
                From = from;
                Target = target;
                TimeStartStop = timeStartStop;
                Callback = callback;
            }
            public readonly Quaternion From;
            public readonly Quaternion Target;
            public readonly Vector2 TimeStartStop;
            public readonly Action<Quaternion> Callback;
            public override bool IsOver(float time)
            {
                return time > TimeStartStop.y;
            }
            public override void Animate(float time) { Callback?.Invoke(AnimateNoInvoke(time)); }
            private Quaternion AnimateNoInvoke(float time)
            {
                float factor = GetInterpolationFactor(TimeStartStop.x, TimeStartStop.y, time);
                return Quaternion.Lerp(From, Target, factor);
            }
            public override AnimationData CombineWith(AnimationData newAnimation)
            {
                if (newAnimation.GetType() == typeof(QuaternionAnimationData))
                {
                    QuaternionAnimationData newQuaternionAnimation = (QuaternionAnimationData)newAnimation;
                    return new QuaternionAnimationData(
                        AnimateNoInvoke(newQuaternionAnimation.TimeStartStop.x),
                        newQuaternionAnimation.Target,
                        newQuaternionAnimation.TimeStartStop,
                        newQuaternionAnimation.Callback);
                }
                else return this;
            }
        }
    }
}

