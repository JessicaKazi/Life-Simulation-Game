using System;
using System.Collections.Generic;

namespace LifeGame.Systems.NPC;

public enum NPCAction { Sleep, Eat, Work, Socialize, Shop, Wander, Idle }

public class NPCDecisionMaker
{
    private NPCAction _current = NPCAction.Idle;

    // Utility scoring: return 0..1, higher = more urgent
    public NPCAction Decide(NPC npc)
    {
        var scores = new Dictionary<NPCAction, float>
        {
            [NPCAction.Sleep]     = 1f - npc.Character.Needs.Energy / 100f,
            [NPCAction.Eat]       = 1f - npc.Character.Needs.Hunger / 100f,
            [NPCAction.Work]      = npc.HasJob && npc.IsWorkHours() ? 0.8f : 0f,
            [NPCAction.Socialize] = 1f - npc.Character.Needs.Social / 100f,
            [NPCAction.Shop]      = npc.Character.Needs.Fun < 40f ? 0.5f : 0.1f,
            [NPCAction.Wander]    = 0.2f,
            [NPCAction.Idle]      = 0.05f
        };

        NPCAction best = NPCAction.Idle;
        float bestScore = 0f;
        foreach (var (action, score) in scores)
            if (score > bestScore) { bestScore = score; best = action; }

        _current = best;
        return best;
    }
}