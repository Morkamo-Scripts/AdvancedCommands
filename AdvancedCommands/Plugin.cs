using System;
using AdvancedCommands.Commands.IWantScp;
using AdvancedCommands.Commands.JoinWave;
using AdvancedCommands.Components.Features;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using HarmonyLib;
using MEC;
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
        public override Version Version => new(2, 0, 0);

        public static Plugin Instance;
        public static Harmony Harmony;
        
        public JoinWaveHandler JoinWaveHandler;
        public IwsHandler IwsHandler;
        
        private void InitClasses()
        {
            JoinWaveHandler = Config.JoinWave;
            IwsHandler = Config.IWantScp;
        }

        private void DeInitClasses()
        {
            JoinWaveHandler = null;
            IwsHandler = null;
        }
        
        private void RegisterEvents()
        {
            events.Player.Verified += OnVerifiedPlayer;
            events.Map.AnnouncingChaosEntrance += JoinWaveHandler.OnChaosEntrance;
            events.Map.AnnouncingNtfEntrance += JoinWaveHandler.OnNtfEntrance;
            events.Player.Left += IwsHandler.LeftPlayer;
        }

        private void UnregisterEvents()
        {
            events.Player.Verified -= OnVerifiedPlayer;
            events.Map.AnnouncingChaosEntrance -= JoinWaveHandler.OnChaosEntrance;
            events.Map.AnnouncingNtfEntrance -= JoinWaveHandler.OnNtfEntrance;
            events.Player.Left -= IwsHandler.LeftPlayer;
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