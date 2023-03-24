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
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FTK.UIToolkit.Regions
{
    public class MultiRegionIntersect : MonoBehaviour, IRegion
    {
        [Range(1, 8)]
        [SerializeField]
        private int approxIterations = 2;
        [SerializeField]
        private bool inverted = false;
        [SerializeField]
        private List<InspectorRegion> childRegions = new List<InspectorRegion>();

        private List<IRegion> regions = new List<IRegion>();
        public bool Inverted
        {
            get => inverted;
            set { inverted = value; }
        }

        public Vector3 ClosestPoint(Vector3 position)
        {
            if (inverted)
            {
                Vector3 closestPoint = position;
                float minDist = float.PositiveInfinity;
                foreach (IRegion region in regions)
                {
                    if (!region.isActiveAndEnabled) continue;
                    region.Inverted ^= true;
                    Vector3 candidatePoint = region.ClosestPoint(position);
                    region.Inverted ^= true;
                    float candidateDistance = Vector3.Distance(candidatePoint, position);
                    if (candidateDistance < minDist)
                    {
                        closestPoint = candidatePoint;
                        minDist = candidateDistance;
                    }
                }
                return closestPoint;
            }
            else
            {
                for (int i = 0; i < approxIterations; i++)
                    position = ClostestPointIteration(position);
                return position;
            }
        }
        public bool IsInsideOfRegion(Vector3 position)
        {
            if (inverted) return regions.Any(regions => !regions.IsInsideOfRegion(position));
            else return regions.All(region => region.IsInsideOfRegion(position));
        }

        private Vector3 ClostestPointIteration(Vector3 position)
        {
            int c = 0;
            Vector3 clostestPoint = Vector3.zero;
            foreach (IRegion region in regions)
            {
                if (!region.isActiveAndEnabled) continue;
                if (!region.IsInsideOfRegion(position))
                {
                    c++;
                    clostestPoint += region.ClosestPoint(position);
                }
            }
            if (c > 0) return clostestPoint / c;
            else return position;
        }
        private void OnEnable() { }

        private void OnValidate()
        {
            regions.Clear();
            for (var index = 0; index < childRegions.Count; index++)
            {
                var entry = childRegions[index];
                if (entry.IsEmpty()) continue;
                if (entry.IsValid() && !ReferenceEquals(entry.region, this)) regions.Add(entry.region as IRegion);
                else
                {
                    Debug.LogWarning("Removing entry '" + entry.name + "':'" + entry.region + "', since it is not a valid region!");
                    entry.region = null;
                }
            }
        }

        [Serializable]
        private class InspectorRegion
        {
            [SerializeField]
            internal string name;
            [SerializeField]
            internal Component region;

            internal bool IsValid() => typeof(IRegion).IsAssignableFrom(region?.GetType());

            internal bool IsEmpty() => region == null;
        }
    }
}

