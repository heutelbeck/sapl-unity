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

public interface IBreakingTheGlas
{
    bool PermitExecution { get; set; }

    /* Regard: This method needs to be copied into the Class implementing this interface
     * if you want to call this methods via the unity editor
     * (editor is not able to find interface methods).
    */
    public sealed void SecuredExecute()
    {
        if (PermitExecution)
        {
            UserExecute();
        }
    }

    protected void UserExecute();

    public sealed void ExecuteWithPermission()
    {
        PermitExecution = true;
        SecuredExecute();
    }

    public sealed void ResetState()
    {
        PermitExecution = true;
    }
}
