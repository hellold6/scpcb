// NPCSystem.cs
// Ports BlitzBasic's Type / Each iteration idiom to C# generic lists.
//
// Original BlitzBasic pattern:
//   Type NPCs
//     Field ent, spawnEnt
//     Field health#, state, angerLvl
//     Field waitTimer#, soundTimer#
//   End Type
//
//   Global n.NPCs
//   For n.NPCs = Each NPCs
//     ... update logic ...
//   Next
//
// C# equivalent: a plain class with the same fields + a static registry.
// We keep field names identical to the .bb source to make diffing easy.

using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using static SCPCB360.Engine.B3D;

namespace SCPCB360.GameLogic
{
    // ─────────────────────────────────────────────────────────────────────────
    // NPC state machine states (match CB's internal state constants)
    // ─────────────────────────────────────────────────────────────────────────
    public enum NPCState
    {
        Idle     = 0,
        Wander   = 1,
        Alert    = 2,
        Chase    = 3,
        Attack   = 4,
        Stunned  = 5,
        Dead     = 6,
    }

    // ─────────────────────────────────────────────────────────────────────────
    // NPC type definition — mirrors the BlitzBasic Type fields exactly
    // ─────────────────────────────────────────────────────────────────────────
    public class NPC
    {
        // Entity handles (integers, matching Blitz3D handle idiom)
        public int ent       = -1;   // main entity
        public int spawnEnt  = -1;   // spawn point pivot
        public int camEnt    = -1;   // per-NPC camera (SCP-173 uses this for freeze detection)

        // Stats
        public float health    = 100f;
        public NPCState state  = NPCState.Idle;
        public int angerLvl   = 0;    // 0-100

        // Timers (seconds, matching CB's millisecond timers ÷ 1000)
        public float waitTimer  = 0f;
        public float soundTimer = 0f;
        public float atkTimer   = 0f;

        // Pathfinding
        public int   targetEnt  = -1;   // current nav target entity
        public float pathTimer  = 0f;

        // SCP-173 specific
        public bool beingWatched = false;
        public int  blinkTarget  = -1;

        // Type discriminator (maps to CB's individual NPC type globals)
        public NPCKind kind = NPCKind.Guard;

        // Unique id
        public int id;
        private static int _nextId = 0;
        public NPC() { id = _nextId++; }
    }

    public enum NPCKind
    {
        Guard       = 0,   // MTF / Security Guard
        SCP_173     = 173,
        SCP_106     = 106,
        SCP_096     = 96,
        SCP_049     = 49,
        SCP_049_2   = 492, // 049-2 zombie
        SCP_939     = 939,
        Scientist   = 1000,
    }

    // ─────────────────────────────────────────────────────────────────────────
    // NPC registry + update system
    // ─────────────────────────────────────────────────────────────────────────
    public static class NPCSystem
    {
        // The equivalent of Blitz3D's global linked list for Each NPCs
        private static readonly List<NPC> _npcs = new();
        private static readonly List<NPC> _pendingRemove = new();

        public static IReadOnlyList<NPC> All => _npcs;

        public static NPC Spawn(NPCKind kind, int spawnPivotHandle)
        {
            var n = new NPC
            {
                kind     = kind,
                spawnEnt = spawnPivotHandle,
                state    = NPCState.Idle,
            };

            // Duplicate the appropriate base mesh
            string meshPath = GetMeshPath(kind);
            n.ent = LoadMesh(meshPath);
            PositionEntity(n.ent,
                EntityX(spawnPivotHandle, true),
                EntityY(spawnPivotHandle, true),
                EntityZ(spawnPivotHandle, true));

            _npcs.Add(n);
            return n;
        }

        public static void Remove(NPC n)
        {
            _pendingRemove.Add(n);
        }

        /// <summary>
        /// Call each game tick. Equivalent to CB's "For n.NPCs = Each NPCs" loop.
        /// </summary>
        public static void Update(float delta, int playerEntity)
        {
            foreach (var n in _npcs)
                UpdateNPC(n, delta, playerEntity);

            // Flush removals after iteration (avoids modifying list mid-loop)
            foreach (var n in _pendingRemove)
            {
                FreeEntity(n.ent);
                _npcs.Remove(n);
            }
            _pendingRemove.Clear();
        }

        private static void UpdateNPC(NPC n, float delta, int playerEnt)
        {
            n.waitTimer  = Math.Max(0f, n.waitTimer  - delta);
            n.soundTimer = Math.Max(0f, n.soundTimer - delta);
            n.atkTimer   = Math.Max(0f, n.atkTimer   - delta);

            float distToPlayer = EntityDistance(n.ent, playerEnt);

            switch (n.kind)
            {
                case NPCKind.SCP_173: Update173(n, delta, playerEnt, distToPlayer); break;
                case NPCKind.SCP_106: Update106(n, delta, playerEnt, distToPlayer); break;
                case NPCKind.Guard:   UpdateGuard(n, delta, playerEnt, distToPlayer); break;
                default:              UpdateGeneric(n, delta, playerEnt, distToPlayer); break;
            }
        }

        // ─── SCP-173 ──────────────────────────────────────────────────────────────
        // Core rule: moves only when not in any observer's FOV.

        private static void Update173(NPC n, float delta, int playerEnt, float dist)
        {
            // Check if player (or any guard) has 173 in their view frustum
            n.beingWatched = EntityVisible(playerEnt, n.ent);

            if (n.beingWatched)
            {
                // Freeze — do nothing. (CB blink timer is handled in PlayerSystem)
                n.state = NPCState.Idle;
                return;
            }

            // Not watched → teleport-sprint toward nearest target
            n.state = NPCState.Chase;

            float speed = 12f * delta; // 173 moves very fast
            MoveToward(n.ent, playerEnt, speed);

            if (dist < 1.5f && n.atkTimer <= 0f)
            {
                // Snap neck — instant kill in CB
                n.atkTimer = 2f;
                // PlayerSystem.Kill() would be called here
            }
        }

        // ─── SCP-106 ──────────────────────────────────────────────────────────────

        private static void Update106(NPC n, float delta, int playerEnt, float dist)
        {
            switch (n.state)
            {
                case NPCState.Wander:
                    n.waitTimer -= delta;
                    if (n.waitTimer <= 0f)
                    {
                        n.state = NPCState.Chase;
                    }
                    break;

                case NPCState.Chase:
                    // 106 is slow but phases through walls (no collision response)
                    float speed = 2.5f * delta;
                    MoveToward(n.ent, playerEnt, speed);

                    if (dist < 2f && n.atkTimer <= 0f)
                    {
                        n.atkTimer = 3f;
                        // Pocket dimension teleport — PlayerSystem.TriggerPocketDimension()
                    }
                    break;
            }
        }

        // ─── Security Guard ───────────────────────────────────────────────────────

        private static void UpdateGuard(NPC n, float delta, int playerEnt, float dist)
        {
            switch (n.state)
            {
                case NPCState.Idle:
                    if (dist < 15f) n.state = NPCState.Alert;
                    break;

                case NPCState.Alert:
                    n.angerLvl = (int)MathHelper.Clamp(n.angerLvl + delta * 5, 0, 100);
                    if (n.angerLvl >= 50) n.state = NPCState.Chase;
                    break;

                case NPCState.Chase:
                    MoveToward(n.ent, playerEnt, 4f * delta);
                    if (dist < 2f && n.atkTimer <= 0f)
                    {
                        n.atkTimer = 1f;
                        // DamagePlayer(15)
                    }
                    break;
            }
        }

        private static void UpdateGeneric(NPC n, float delta, int playerEnt, float dist)
        {
            if (dist < 20f && n.state == NPCState.Idle)
                n.state = NPCState.Chase;

            if (n.state == NPCState.Chase)
                MoveToward(n.ent, playerEnt, 3f * delta);
        }

        // ─────────────────────────────────────────────────────────────────────────
        // Pathfinding stub — CB uses direct entity movement, not navmesh.
        // Replace with a proper A* grid if room-aware navigation is needed.
        // ─────────────────────────────────────────────────────────────────────────

        private static void MoveToward(int mover, int target, float speed)
        {
            var mPos = Get(mover)?.GetWorldPosition() ?? Vector3.Zero;
            var tPos = Get(target)?.GetWorldPosition() ?? Vector3.Zero;
            var dir  = tPos - mPos;
            if (dir.LengthSquared() < 0.001f) return;
            dir.Normalize();
            // Keep Y locked — NPCs don't fly
            dir.Y = 0f;
            PositionEntity(mover,
                mPos.X + dir.X * speed,
                mPos.Y,
                mPos.Z + dir.Z * speed,
                true);
            // Face direction of travel
            TurnEntity(mover, 0f, (float)Math.Atan2(dir.X, dir.Z) * (180f / (float)Math.PI) - EntityYaw(mover), 0f);
        }

        private static string GetMeshPath(NPCKind kind) => kind switch
        {
            NPCKind.SCP_173   => "GFX/npc/scp173",
            NPCKind.SCP_106   => "GFX/npc/scp106",
            NPCKind.SCP_096   => "GFX/npc/scp096",
            NPCKind.SCP_049   => "GFX/npc/scp049",
            NPCKind.SCP_049_2 => "GFX/npc/scp049zombie",
            NPCKind.Guard     => "GFX/npc/guard",
            NPCKind.Scientist => "GFX/npc/scientist",
            _                 => "GFX/npc/guard",
        };
    }
}
