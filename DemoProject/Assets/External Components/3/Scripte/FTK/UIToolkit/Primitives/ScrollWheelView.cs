/*
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *      http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
using FTK.UIToolkit.Util;
using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;
using UnityEngine.Events;

namespace FTK.UIToolkit.Primitives
{
    public class ScrollWheelView : AnimatableBase
    {
        private static readonly string COLOR_PROPERTY = "_PrimaryColor";

        [SerializeField]
        private Transform wheelTransform;
        [SerializeField]
        private Transform visualsTransform;
        [SerializeField]
        private AnimatableMaterialInstance material;
        [SerializeField]
        private AudioSource audioSource;
        [Space(10)]
        [Min(1)]
        [SerializeField]
        private int stepsPerRevolution = 24;
        [SerializeField]
        private Vector2Int minMaxSteps = new Vector2Int(0, 10);
        [Space(20)]
        [SerializeField]
        private UnityEvent<int> discreteValueUpdate;
        [SerializeField]
        private UnityEvent<float> continuousValueUpdate;
        [SerializeField]
        private AudioClip onStartMove;
        [SerializeField]
        private AudioClip onIncrement;
        [SerializeField]
        private AudioClip onEndMove;

        private float startAngle = 0f;
        private float prevAngle = 0f;
        private float accAngle = 0f;
        private float totalSteps = 0f;
        private bool isMoving = false;

        public void BeginMove(HandTrackingInputEventData data) => BeginMove(data.InputData);

        public void BeginMove(MixedRealityPointerEventData data) => BeginMove(data.Pointer.Result.Details.Point);

        private void BeginMove(Vector3 startPos)
        {
            if (isMoving) return;
            isMoving = true;
            startAngle = Project(startPos);
            prevAngle = startAngle;
            audioSource.PlayOneShot(onStartMove);
        }

        public void Move(HandTrackingInputEventData data) => Move(data.InputData);

        public void Move(MixedRealityPointerEventData data) => Move(data.Pointer.Position);

        private void Move(Vector3 currentPos)
        {
            if (!isMoving) return;
            var currentAngle = Project(currentPos);

            var newAccAngle = accAngle + prevAngle - currentAngle;
            prevAngle = currentAngle;

            if (ComputeTotalSteps(newAccAngle))
            {
                accAngle = newAccAngle;

                var delta = Mathf.Repeat(startAngle - currentAngle, 360f);
                if (IsAnimationRunning("ROT"))
                    StopAnimation("ROT");
                wheelTransform.localEulerAngles = new Vector3(delta, 0f, 0f);
                material.AnimateMaterialProperty(COLOR_PROPERTY, new Color(0.5f, 0.5f, 0.5f), 0.1f);
            }
            else material.AnimateMaterialProperty(COLOR_PROPERTY, new Color(1f, 0f, 0f), 0.1f);
        }

        public void EndMove()
        {
            if (!isMoving) return;
            isMoving = false;
            Animate("VISUAL", visualsTransform.localRotation, Quaternion.Euler(accAngle, 0f, 0f), 0.1f, value => visualsTransform.localRotation = value);
            Animate("ROT", wheelTransform.localRotation, Quaternion.identity, 0.1f, value => wheelTransform.localRotation = value);
            //visualsTransform.localRotation = Quaternion.Euler(accAngle, 0f, 0f);
            //wheelTransform.localRotation = Quaternion.identity;
            material.AnimateMaterialProperty(COLOR_PROPERTY, new Color(0.5f, 0.5f, 0.5f), 0.1f);
            audioSource.PlayOneShot(onEndMove);
        }

        private bool ComputeTotalSteps(float angle)
        {
            float steps = angle / 360f * stepsPerRevolution;
            bool isInRange = steps >= minMaxSteps.x && steps <= minMaxSteps.y;
            steps = Mathf.Clamp(steps, minMaxSteps.x, minMaxSteps.y);
            int discreteSteps = Mathf.RoundToInt(steps);
            int discreteTotalSteps = Mathf.RoundToInt(totalSteps);
            if (discreteSteps != discreteTotalSteps)
            {
                discreteValueUpdate.Invoke(discreteSteps);
                audioSource.PlayOneShot(onIncrement);
            }
            if (steps != totalSteps)
                continuousValueUpdate.Invoke(steps);
            totalSteps = steps;
            return isInRange;
        }

        private float Project(Vector3 pos)
        {
            var localPos = transform.InverseTransformPoint(pos);
            return Mathf.Atan2(localPos.y, localPos.z) * Mathf.Rad2Deg + (-0.5f * Mathf.Sign(localPos.y) + 0.5f) * 360f;
        }
    }
}