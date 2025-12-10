using CitizenFX.Core;
using System;
using static CitizenFX.Core.Native.API;

namespace lsrplogin.Client
{
    public class ClientEvents : BaseScript
    {
        public ClientEvents()
        {
            EventHandlers["lsrp:client:spawnplayer"] += new Action(OnSpawnPlayer);
        }

        private void OnSpawnPlayer()
        {
            string modelName = "mp_m_freemode_01";
            Vector3 position = new Vector3(154.0f, -977.0f, 30.5f);
            float heading = 205.0f;
            SpawnManager.Instance.SpawnPlayer(modelName, position, heading);
        }
    }
}