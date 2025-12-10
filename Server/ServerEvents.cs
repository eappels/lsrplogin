using CitizenFX.Core;
using System;
using System.Threading.Tasks;
using static CitizenFX.Core.Native.API;

namespace lsrplogin.Server
{
    public class ServerEvents : BaseScript
    {
        public ServerEvents()
        {
            EventHandlers["playerConnecting"] += new Action<Player, string, dynamic, dynamic>(OnPlayerConnecting);
            EventHandlers["playerJoining"] += new Action<Player>(OnPlayerJoining);
            EventHandlers["playerDropped"] += new Action<Player, string, string, uint>(OnPlayerDropped);
        }

        private async void OnPlayerConnecting([FromSource] Player player, string playerName, dynamic setKickReason, dynamic deferrals)
        {
            deferrals.defer();
            await Delay(0);
            var licenseIdentifier = player.Identifiers["license"];
            Debug.WriteLine($"A player with the name {playerName} (Identifier: [{licenseIdentifier}]) is connecting to the server.");
            deferrals.update($"Hello {playerName}, your license [{licenseIdentifier}] is being checked");
            if (await IsBanned(licenseIdentifier))
            {
                deferrals.done($"You have been kicked (Reason: [Banned])! Please contact the server administration (Identifier: [{licenseIdentifier}]).");
            }

            deferrals.done();
        }

        private async Task<bool> IsBanned(string licenseIdentifier)
        {
            // TODO: Implement ban check logic here (e.g., database lookup)
            await Delay(0);
            return false;
        }

        private void OnPlayerJoining([FromSource]Player player)
        {
            Debug.WriteLine($"[lsrp] Player joined: {player.Name} (StateID: {player.Handle})");
            try
            {
                TriggerClientEvent(player, "lsrp:client:spawnplayer");
            }
            catch { }
        }

        private void OnPlayerDropped([FromSource] Player player, string reason, string resourceName, uint clientDropReason)
        {
            Debug.WriteLine($"[lsrp] Player dropped: {player.Name} (StateID: {player.Handle}) reason: {reason} drop reason: {clientDropReason}");
        }

    }
}