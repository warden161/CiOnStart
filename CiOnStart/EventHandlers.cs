// -----------------------------------------------------------------------
// <copyright file="EventHandlers.cs" company="Build">
// Copyright (c) Build. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace CiOnStart
{
    using System.Collections.Generic;
    using Exiled.Events.EventArgs.Player;
    using InventorySystem;
    using MonoMod.Utils;
    using PlayerRoles;
    using Respawning;

    /// <summary>
    /// Handles events derived from <see cref="Exiled.Events.Handlers"/>.
    /// </summary>
    public class EventHandlers
    {
        private readonly Plugin plugin;
        private readonly Queue<RoleTypeId> spawnQueue = new Queue<RoleTypeId>();
        private bool isChi;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventHandlers"/> class.
        /// </summary>
        /// <param name="plugin">An instance of the <see cref="Plugin"/> class.</param>
        public EventHandlers(Plugin plugin) => this.plugin = plugin;

        /// <inheritdoc cref="Exiled.Events.Handlers.Player.OnChangingRole(ChangingRoleEventArgs)"/>
        public void OnChangingRole(ChangingRoleEventArgs ev)
        {
            if (!isChi || ev.NewRole != RoleTypeId.FacilityGuard)
                return;

            ev.NewRole = spawnQueue.Dequeue();
            if (!InventorySystem.Configs.StartingInventories.DefinedInventories.TryGetValue(ev.NewRole, out InventoryRoleInfo inventoryRoleInfo))
                return;

            ev.Items.Clear();
            ev.Ammo.Clear();

            ev.Items.AddRange(inventoryRoleInfo.Items);
            ev.Ammo.AddRange(inventoryRoleInfo.Ammo);
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Server.OnWaitingForPlayers()"/>
        public void OnWaitingForPlayers()
        {
            isChi = Exiled.Loader.Loader.Random.Next(100) < plugin.Config.CiChance;
            if (isChi)
            {
                SpawnableTeamHandlerBase chaosSpawnHandler = RespawnManager.SpawnableTeams[SpawnableTeamType.ChaosInsurgency];
                chaosSpawnHandler.GenerateQueue(spawnQueue, chaosSpawnHandler.MaxWaveSize);
            }
        }
    }
}