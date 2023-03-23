﻿/*
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
namespace Sapl.Internal.Structs
{
    /// <summary>
    /// Used to serialize a logged-on User for <see cref="Sapl.Internal.Registry.SaplRegistry"/>
    /// </summary>
    struct UserSubjectStruct
    {
        public UserSubjectStruct(string name, string domain, string machine)
        {
            Name = name;
            Domain = domain;
            Machine = machine;
        }
#nullable enable
        public string? Name { get; set; }
        public string? Domain { get; set; }
        public string? Machine { get; set; }
    }
}
