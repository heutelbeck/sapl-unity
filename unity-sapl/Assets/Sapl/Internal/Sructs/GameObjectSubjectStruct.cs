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
using Newtonsoft.Json;

namespace Sapl.Internal.Structs
{
    ///<summary>
    ///Used to serialize <see cref="UnityEngine.GameObject"/> for <see cref="Sapl.Internal.Registry.SaplRegistry"/>
    ///</summary>
    struct GameObjectSubjectStruct
    {
        /// <summary>Initializes a new instance of the <see cref="GameObjectSubjectStruct" /> struct.</summary>
        /// <param name="name">The name of the game object.</param>
        /// <param name="tag">The tag of the game object.</param>
        public GameObjectSubjectStruct(string name, string tag)//, int hash)
        {
            Name = name;
            Tag = tag;
            //Hash = hash;
        }
#nullable enable
        /// <summary>Gets or sets the name.</summary>
        /// <value>The name.</value>
        [JsonProperty("name")]
        public string? Name { get; set; }
        /// <summary>Gets or sets the tag.</summary>
        /// <value>The tag.</value>
        [JsonProperty("tag")]
        public string? Tag { get; set; }
        //public int? Hash { get; set; } nicht praktikabel
    }
}
