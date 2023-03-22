using FaPra.Data;
using UnityEngine;

namespace FaPra.ScriptableObjects {
    public class MasterSettingsProvider : MonoBehaviourSingleton<MasterSettingsProvider> {
        [SerializeField]
        private BackendSettings settings;

        public BackendSettings SelectedSetting { get { return settings; } }
    }
}