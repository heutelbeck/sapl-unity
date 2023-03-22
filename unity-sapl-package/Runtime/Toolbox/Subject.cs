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
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace Sapl.Components
{
    [AddComponentMenu("Sapl/Subject")]
    public class Subject : MonoBehaviour
    {
        [SerializeField]
        public string subjectName;

        [SerializeField]
        public string email;

        [SerializeField]
        public string company;
        



        public override string ToString()
        {
            return subjectName;
        }
    } 
}
