using System;
using AdvancedCommands.Commands.DisableScp2536;
using AdvancedCommands.Commands.IWantScp;
using AdvancedCommands.Commands.JoinWave;
using AdvancedCommands.Commands.OwnerMode;
using AdvancedCommands.Commands.ReservedSlotsManagement;
using AdvancedCommands.Components.Features;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using HarmonyLib;
using MEC;
using PlayerRoles;
using RueI.API;
using RueI.API.Elements;
using events = Exiled.Events.Handlers;
using lab = LabApi.Events.Handlers;

namespace AdvancedCommands
{
    public class Plugin : Plugin<Config>
    {
        public override string Name => "AdvancedCommands";
        public override string Prefix => Name;
        public override string Author => "Morkamo";
        public override Version RequiredExiledVersion => new(9, 1, 0);
        public override Version Version => new(2, 4, 0);

        public static Plugin Instance;
        public static Harmony Harmony;
        
        public JoinWaveHandler JoinWaveHandler;
        public IwsHandler IwsHandler;
        
        public DateTime? LastSpawnTime { get; set; }
        public SquadTypes? LastSpawnedSquad { get; set; }
        public bool IsWaveBlockedAnotherTeam = false;
        
        public RsmHandler RsmHandler;
        public RsmHeader RsmHeader;
        public Disable2536Handler Disable2536Handler;
        public OwnerModeHandler OwnerModeHandler;
        
        private void InitClasses()
        {
            JoinWaveHandler = Config.JoinWave;
            IwsHandler = new IwsHandler();
            RsmHandler = new RsmHandler();
            RsmHeader = Config.ReservedSlotManagement;
            Disable2536Handler = new Disable2536Handler();
            OwnerModeHandler = new OwnerModeHandler();
        }

        private void DeInitClasses()
        {
            JoinWaveHandler = null;
            IwsHandler = null;
            RsmHeader = null;
            RsmHandler = null;
            Disable2536Handler = null;
            OwnerModeHandler = null;
        }
        
        private void RegisterEvents()
        {
            events.Player.Verified += OnVerifiedPlayer;
            events.Server.RespawnedTeam += JoinWaveHandler.OnRespawnedTeam;
            events.Player.Left += IwsHandler.LeftPlayer;
            events.Server.RoundEnded += IwsHandler.OnRoundEnded;
            events.Player.ReservedSlot += RsmHandler.OnCheckReservedSlot;
            events.Scp2536.FindingPosition += Disable2536Handler.OnFindingSpawnPoint;
            events.Player.Handcuffing += OwnerModeHandler.OnHandcuffing;
            events.Player.UsingMicroHIDEnergy += OwnerModeHandler.OnUsingMicroHID;
            events.Player.Dying += OwnerModeHandler.OnDying;
            events.Player.ItemAdded += OwnerModeHandler.OnItemAdded;
            events.Player.ExplodingMicroHID += OwnerModeHandler.OnMicroHidExploding;
            events.Player.ChangingMicroHIDState += OwnerModeHandler.OnChangingMicroHIDState;
            events.Player.Hurting += OwnerModeHandler.OnHurting;
            events.Player.ChangingRole += OwnerModeHandler.OnChangingRole;
            LabApi.Events.Handlers.PlayerEvents.ShootingWeapon += OwnerModeHandler.OnShooting;
        }

        private void UnregisterEvents()
        {
            events.Player.Verified -= OnVerifiedPlayer;
            events.Server.RespawnedTeam -= JoinWaveHandler.OnRespawnedTeam;
            events.Player.Left -= IwsHandler.LeftPlayer;
            events.Server.RoundEnded -= IwsHandler.OnRoundEnded;
            events.Player.ReservedSlot -= RsmHandler.OnCheckReservedSlot;
            events.Scp2536.FindingPosition -= Disable2536Handler.OnFindingSpawnPoint;
            events.Player.Handcuffing -= OwnerModeHandler.OnHandcuffing;
            events.Player.UsingMicroHIDEnergy -= OwnerModeHandler.OnUsingMicroHID;
            events.Player.Dying -= OwnerModeHandler.OnDying;
            events.Player.ItemAdded -= OwnerModeHandler.OnItemAdded;
            events.Player.ExplodingMicroHID -= OwnerModeHandler.OnMicroHidExploding;
            events.Player.ChangingMicroHIDState -= OwnerModeHandler.OnChangingMicroHIDState;
            events.Player.Hurting -= OwnerModeHandler.OnHurting;
            events.Player.ChangingRole -= OwnerModeHandler.OnChangingRole;
            LabApi.Events.Handlers.PlayerEvents.ShootingWeapon -= OwnerModeHandler.OnShooting;
        }
        
        public override void OnEnabled()
        {
            Instance = this;
            
            Harmony = new Harmony("ru.morkamo.advancedCommands.patches");
            Harmony.PatchAll();
            
            InitClasses();
            RegisterEvents();
            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            UnregisterEvents();
            DeInitClasses();
            
            Harmony.UnpatchAll();
            
            Instance = null;
            base.OnDisabled();
        }
        
        private void OnVerifiedPlayer(VerifiedEventArgs ev)
        {
            if (ev.Player.ReferenceHub.gameObject.GetComponent<PlayerAdvancedCommands>() != null)
                return;

            ev.Player.ReferenceHub.gameObject.AddComponent<PlayerAdvancedCommands>();
        }
        
        public static void RueHint(Player player, string text, float hintWidth, float hintDuration)
        {
            if (text.IsEmpty())
                return;
            
            RueDisplay
                .Get(player)
                .Show(
                    new Tag(),
                    new BasicElement(hintWidth, $"<b>{text}</b>"),
                    hintDuration);

            Timing.CallDelayed(hintDuration, () =>
            {
                RueDisplay.Get(player).Update();
            });
        }
    }
}