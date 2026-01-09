using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AdvancedCommands.Components.Features;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Server;
using JetBrains.Annotations;
using MEC;
using PlayerRoles;
using RueI.API;
using RueI.API.Elements;
using UnityEngine;

namespace AdvancedCommands.Commands.IWantScp;

public class IwsHandler
{
    public const int HintHeight = 150;
    public const float HintDuration = 1.05f;
    
    public HashSet<RoleTypeId> LeavedScp = new();
    public HashSet<Player> LotteryMembers = new();

    public static float LotteryProcessingTime = 25;
    public static float TimeForReplacement = 120;
    
    public bool IsLotteryProcessing = false;
    public bool IsLotteryClosed = false;

    [CanBeNull] public Coroutine LotteryCoroutine;

    public void OnRoundEnded(RoundEndedEventArgs ev)
    {
        if (LotteryCoroutine != null)
            CoroutineRunner.Stop(LotteryCoroutine);
        IsLotteryClosed = false;
        IsLotteryProcessing = false;
        LeavedScp.Clear();
        LotteryMembers.Clear();
        LotteryCoroutine = null;
    }
    
    public void LeftPlayer(LeftEventArgs ev)
    {
        if (Round.ElapsedTime.TotalSeconds > TimeForReplacement && Round.IsStarted && !Round.IsEnded)
            return;
        
        if (ev.Player.IsScp && ev.Player.Role.Type != RoleTypeId.Scp0492)
        {
            if (!IsLotteryProcessing)
            {
                IsLotteryProcessing = true;
                LotteryCoroutine = CoroutineRunner.Run(ProcessingLottery());
            }
            LeavedScp.Add(ev.Player.Role);
        }
    }

    private IEnumerator ProcessingLottery()
    {
        IsLotteryClosed = false;
        
        for (int i = (int)LotteryProcessingTime; i != 0; i--)
        {
            if (Round.IsEnded)
                yield break;
            
            foreach (var player in Player.List)
            {
                if (!player.IsScp)
                {
                    Plugin.RueHint(player, "<b><color=#FA4343>Один или несколько SCP досрочно покинули игру.\n " +
                                           "Все желающие заменить их могут принять участие в лотерее.\n" +
                                           $"<color=#FFB31A>Лотерея закончиться через <color=#FF1A25>{i}</color> сек.</color>\n" +
                                           "<color=#FF9C1A>Для участия напишите команду -</color> </b><size=130%><color=#F5367E>.iws</color></size>",
                        HintHeight, HintDuration);
                }
            }
            yield return new WaitForSeconds(1f);
        }

        try
        {
            IsLotteryClosed = true;

            for (int i = 0; i < LeavedScp.Count; i++)
            {
                foreach (var member in LotteryMembers)
                {
                    if (member == null || Player.Get(member?.UserId) == null || member.IsOverwatchEnabled || member.IsScp)
                        LotteryMembers.Remove(member);
                }

                if (LeavedScp.Count == 0 || LotteryMembers.Count == 0)
                {
                    IsLotteryClosed = false;
                    IsLotteryProcessing = false;
                    LotteryCoroutine = null;
                    yield break;
                }
            
                var winner = LotteryMembers.GetRandomValue();
                var replacedScp = LeavedScp.GetRandomValue();

                Ragdoll.List.FirstOrDefault(ragdoll => ragdoll.Role == replacedScp)?.Destroy();
                
                winner.DropItems();
                winner.DisableAllEffects();
                winner.Role.Set(replacedScp);
                
                LeavedScp.Remove(replacedScp);
                LotteryMembers.Remove(winner);
                
                if (LeavedScp.IsEmpty())
                    LotteryMembers.Clear();
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex);
        }
        
        IsLotteryClosed = false;
        IsLotteryProcessing = false;
        LotteryCoroutine = null;
    }
}