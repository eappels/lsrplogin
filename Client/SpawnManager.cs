using CitizenFX.Core;
using System;
using System.Threading.Tasks;
using static CitizenFX.Core.Native.API;

namespace lsrplogin.Client
{
    public class SpawnManager : BaseScript
    {

        private bool spawnLock = false;

        private static SpawnManager instance;
        public static SpawnManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new SpawnManager();
                }
                return instance;
            }
        }

        public SpawnManager()
        {
            instance = this;
        }

        public void SpawnPlayer(string modelName, Vector3 position, float heading)
        {
            SpawnPoint sp = new SpawnPoint();
            sp.x = position.X;
            sp.y = position.Y;
            sp.z = position.Z;
            sp.model = modelName;
            sp.heading = heading;
            sp.skipFade = true;

            _ = SpawnPlayerFromArg(sp);
        }

        private async Task SpawnPlayerFromArg(SpawnPoint spawn)
        {
            if (spawnLock)
                return;

            spawnLock = true;

            try
            {
                await Delay(1000);
                try
                {
                    if (!string.IsNullOrEmpty(spawn.model))
                    {
                        var hash = GetHashKey(spawn.model);
                        RequestModel((uint)hash);
                        int attempts = 0;
                        while (!HasModelLoaded((uint)hash) && attempts++ < 200)
                        {
                            await Delay(50);
                            Debug.WriteLine($"[SpawnManager] Loading model {spawn.model} ({hash}), attempt {attempts}/200");
                        }

                        if (HasModelLoaded((uint)hash))
                        {
                            await Game.Player.ChangeModel((int)hash);
                            try { SetPedDefaultComponentVariation(GetPlayerPed(-1)); } catch { }
                        }
                    }
                    var ped = GetPlayerPed(-1);
                    RequestCollisionAtCoord(spawn.x, spawn.y, spawn.z);
                    SetEntityCoordsNoOffset(ped, spawn.x, spawn.y, spawn.z, false, false, false);
                    NetworkResurrectLocalPlayer(spawn.x, spawn.y, spawn.z, spawn.heading, true, true);
                    SetEntityCoordsNoOffset(GetPlayerPed(-1), spawn.x, spawn.y, spawn.z, false, false, false);
                    SetEntityHeading(GetPlayerPed(-1), spawn.heading);
                    ClearPedTasksImmediately(GetPlayerPed(-1));
                    RemoveAllPedWeapons(GetPlayerPed(-1), false);
                    ClearPlayerWantedLevel(PlayerId());

                    int collisionAttempts = 0;
                    while (!HasCollisionLoadedAroundEntity(ped) && collisionAttempts++ < 500)
                    {
                        await Delay(10);
                    }

                    try { ShutdownLoadingScreen(); } catch { }
                    try { ShutdownLoadingScreenNui(); } catch { }

                    try { ResetEntityAlpha(ped); } catch { }
                    try { SetPedDefaultComponentVariation(ped); } catch { }
                    try { SetLocalPlayerVisibleLocally(true); } catch { }
                    try { SetEntityVisible(ped, true, false); } catch { }
                    try { SetEntityVisible(ped, true, true); } catch { }
                    try { SetEntityCollision(ped, true, true); } catch { }
                    try { SetFollowPedCamViewMode(1); } catch { }
                    try { RenderScriptCams(false, false, 0, true, false); } catch { }
                    try { ClearFocus(); } catch { }
                    try { DoScreenFadeIn(250); } catch { }

                    try
                    {
                        var pped = GetPlayerPed(-1);
                        try { if (!IsEntityVisible(pped)) SetEntityVisible(pped, true, false); } catch { }
                        try { if (!IsPedInAnyVehicle(pped, true)) SetEntityCollision(pped, true, true); } catch { }
                        try { FreezeEntityPosition(pped, false); } catch { }
                        try { SetPlayerInvincible(PlayerId(), false); } catch { }
                    }
                    catch { }
                    try { SetPlayerControl(PlayerId(), true, 0); } catch { }

                    try { RenderScriptCams(false, false, 0, true, false); ClearFocus(); DoScreenFadeIn(250); } catch { }
                    try { SetEntityVisible(GetPlayerPed(-1), true, false); SetEntityCollision(GetPlayerPed(-1), true, true); } catch { }
                    try
                    {
                        var ped2 = GetPlayerPed(-1);
                        var pos = GetEntityCoords(ped2, true);
                        CitizenFX.Core.Debug.WriteLine($"[spawnmgr] DirectSpawn completed: ped={ped2} pos=({pos[0]},{pos[1]},{pos[2]})");
                    }
                    catch { }
                }
                catch (Exception ex1)
                {
                    Debug.WriteLine($"[SpawnManager] Error spawning player: {ex1.Message}");
                    throw;
                }
            }
            catch (Exception ex0)
            {
                Debug.WriteLine($"[SpawnManager] Error spawning player: {ex0.Message}");
                throw;
            }
        }
    }
}