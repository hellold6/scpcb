// NPCSystem.cs — ports NPCs.bb Type + CreateNPC/UpdateNPCs

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;
using SCPCB360.Engine;
using static SCPCB360.Engine.B3D;

namespace SCPCB360.GameLogic
{
    public enum NPCState
    {
        Idle = 0, Wander = 1, Alert = 2, Chase = 3, Attack = 4, Stunned = 5, Dead = 6,
    }

    public class NPC
    {
        public int Obj = -1;
        public int Obj2 = -1;
        public int Collider = -1;
        public int NpcType;
        public int Id;
        public float DropSpeed;
        public bool Gravity = true;
        public float State;
        public float State2;
        public float State3;
        public int PrevState;
        public bool Idle;
        public bool MakingNoise;
        public float Frame;
        public float Angle;
        public int Sound = -1;
        public int Sound2 = -1;
        public SoundEffect Sfx;
        public SoundEffect Sfx2;
        public int SoundChn = -1;
        public int SoundChn2 = -1;
        public int PathStatus;
        public float PathX, PathY, PathZ;
        public float SoundTimer;
        public float Speed;
        public int CurrSpeed;
        public string Texture = "";
        public int Reload;
        public int LastSeen;
        public float LastDist;
        public float PrevX, PrevY, PrevZ;
        public NPC Target;
        public int TargetId;
        public float EnemyX, EnemyY, EnemyZ;
        public float GravityMult = 1f;
        public float MaxGravity = 0.2f;
        public bool IsDead;
        public float BlinkTimer = 1f;
        public bool IgnorePlayer;
        public bool InFacility = true;
        public float CollRadius = 0.2f;
        public float IdleTimer;
        public string NvName = "";
        public int HP = 100;
        public string Model = "";
        public float ModelScaleX = 1f, ModelScaleY = 1f, ModelScaleZ = 1f;
        public int TextureId;
        public bool CanUseElevator;

        public int Ent => Collider;
        public int SpawnEnt = -1;
        public int CamEnt = -1;
        public float Health = 100f;
        public NPCState StateEnum = NPCState.Idle;
        public int AngerLvl;
        public float WaitTimer;
        public float AtkTimer;
        public int TargetEnt = -1;
        public float PathTimer;
        public bool BeingWatched;
        public int BlinkTarget = -1;
        public NPCKind Kind = NPCKind.Guard;

        internal static int NextId = 1;
        public NPC() { Id = NextId++; }
    }

    public enum NPCKind
    {
        Guard = 0, SCP_173 = 173, SCP_106 = 106, SCP_096 = 96,
        SCP_049 = 49, SCP_049_2 = 492, SCP_939 = 939, Scientist = 1000,
    }

    public static class NPCSystem
    {
        // NPC type constants from NPCs.bb
        public const int NpcType173 = 1;
        public const int NpcTypeOldMan = 2;
        public const int NpcTypeGuard = 3;
        public const int NpcTypeD = 4;
        public const int NpcType372 = 6;
        public const int NpcTypeApache = 7;
        public const int NpcTypeMtf = 8;
        public const int NpcType096 = 9;
        public const int NpcType049 = 10;
        public const int NpcTypeZombie = 11;
        public const int NpcType5131 = 12;
        public const int NpcTypeTentacle = 13;
        public const int NpcType860 = 14;
        public const int NpcType939 = 15;
        public const int NpcType066 = 16;
        public const int NpcTypePdPlane = 17;
        public const int NpcType966 = 18;
        public const int NpcType1048a = 19;
        public const int NpcType1499 = 20;
        public const int NpcType008 = 21;
        public const int NpcTypeClerk = 22;

        private static readonly List<NPC> _npcs = new();
        private static readonly List<NPC> _pendingRemove = new();

        public static NPC Curr173;
        public static NPC Curr106;
        public static NPC Curr096;
        public static NPC Curr5131;

        public static IReadOnlyList<NPC> All => _npcs;
        public static int Count => _npcs.Count;

        public static NPC CreateNpc(int npcType, float x, float y, float z)
        {
            var n = new NPC { NpcType = npcType };
            n.Collider = CreatePivot();
            PositionEntity(n.Collider, x, y, z);
            EntityRadius(n.Collider, 0.23f, 0.32f);
            EntityType(n.Collider, 1);

            switch (npcType)
            {
                case NpcType173:
                    Setup173(n);
                    break;
                case NpcTypeOldMan:
                    Setup106(n);
                    break;
                case NpcTypeGuard:
                    SetupGuard(n);
                    break;
                case NpcTypeMtf:
                    SetupMtf(n);
                    break;
                case NpcTypeD:
                    SetupClassD(n);
                    break;
                case NpcType372:
                    Setup372(n);
                    break;
                case NpcType5131:
                    Setup5131(n);
                    break;
                case NpcType096:
                    Setup096(n);
                    break;
                case NpcType049:
                    Setup049(n);
                    break;
                case NpcTypeZombie:
                    SetupZombie(n);
                    break;
                case NpcTypeApache:
                    SetupApache(n);
                    break;
                case NpcTypeTentacle:
                    SetupTentacle(n);
                    break;
                case NpcType860:
                    Setup860(n);
                    break;
                case NpcType939:
                    Setup939(n);
                    break;
                case NpcType066:
                    Setup066(n);
                    break;
                case NpcType966:
                    Setup966(n);
                    break;
                case NpcType1048a:
                    Setup1048a(n);
                    break;
                case NpcType1499:
                    Setup1499(n);
                    break;
                case NpcType008:
                    Setup008(n);
                    break;
                case NpcTypeClerk:
                    SetupClerk(n);
                    break;
                default:
                    SetupGuard(n);
                    break;
            }

            if (n.Obj != -1)
            {
                EntityParent(n.Obj, n.Collider);
                ShowEntity(n.Obj);
            }

            _npcs.Add(n);
            return n;
        }

        public static NPC Spawn(NPCKind kind, int spawnPivotHandle)
        {
            int type = kind switch
            {
                NPCKind.SCP_173 => NpcType173,
                NPCKind.SCP_106 => NpcTypeOldMan,
                NPCKind.SCP_096 => NpcType096,
                NPCKind.SCP_049 => NpcType049,
                NPCKind.SCP_049_2 => NpcTypeZombie,
                NPCKind.SCP_939 => NpcType939,
                NPCKind.Guard => NpcTypeGuard,
                _ => NpcTypeGuard,
            };

            return CreateNpc(type,
                EntityX(spawnPivotHandle, true),
                EntityY(spawnPivotHandle, true),
                EntityZ(spawnPivotHandle, true));
        }

        public static void Spawn173()
        {
            if (Curr173 != null) return;
            Curr173 = CreateNpc(NpcType173, 0, 1f, 0);
        }

        public static void ForceSetNpcId(NPC n, int id)
        {
            if (n == null) return;
            n.Id = id;
            if (id >= NPC.NextId) NPC.NextId = id + 1;
        }

        public static void ResolveTargets(IEnumerable<NPC> npcs)
        {
            var lookup = new Dictionary<int, NPC>();
            foreach (var n in npcs)
                lookup[n.Id] = n;

            foreach (var n in npcs)
            {
                if (n.TargetId != 0 && lookup.TryGetValue(n.TargetId, out var target))
                    n.Target = target;
            }
        }

        public static void Remove(NPC n) => _pendingRemove.Add(n);

        public static void Update(float delta, int playerEntity)
        {
            GameState.FpsFactor = delta * 60f;

            foreach (var n in _npcs)
                UpdateNpc(n, delta, playerEntity);

            foreach (var n in _pendingRemove)
            {
                FreeEntity(n.Obj);
                if (n.Obj2 != -1) FreeEntity(n.Obj2);
                FreeEntity(n.Collider);
                _npcs.Remove(n);
            }
            _pendingRemove.Clear();
        }

        public static void UpdateNPCs()
            => Update(GameState.FpsFactor / 60f, GameState.Collider);

        private static void UpdateNpc(NPC n, float delta, int playerEnt)
        {
            n.SoundTimer = Math.Max(0f, n.SoundTimer - delta);
            n.AtkTimer = Math.Max(0f, n.AtkTimer - delta);
            n.WaitTimer = Math.Max(0f, n.WaitTimer - delta);

            float dist = EntityDistance(n.Collider, playerEnt);

            switch (n.NpcType)
            {
                case NpcType173: Update173(n, delta, playerEnt, dist); break;
                case NpcTypeOldMan: Update106(n, delta, playerEnt, dist); break;
                case NpcTypeGuard: UpdateGuard(n, delta, playerEnt, dist); break;
                case NpcTypeMtf: UpdateMtf(n, delta, playerEnt, dist); break;
                case NpcType096: Update096(n, delta, playerEnt, dist); break;
                case NpcType049: Update049(n, delta, playerEnt, dist); break;
                case NpcTypeZombie: UpdateZombie(n, delta, playerEnt, dist); break;
                case NpcType372: Update372(n, delta, playerEnt, dist); break;
                case NpcType5131: Update5131(n, delta, playerEnt, dist); break;
                case NpcType939: Update939(n, delta, playerEnt, dist); break;
                case NpcType966: Update966(n, delta, playerEnt, dist); break;
                case NpcType860: Update860(n, delta, playerEnt, dist); break;
                case NpcType066: Update066(n, delta, playerEnt, dist); break;
                case NpcType1499: Update1499(n, delta, playerEnt, dist); break;
                case NpcType008: Update008(n, delta, playerEnt, dist); break;
                case NpcTypeD:
                case NpcTypeClerk: UpdateClassD(n, delta, playerEnt, dist); break;
                case NpcTypeApache: UpdateApache(n, delta, playerEnt, dist); break;
                case NpcTypeTentacle: UpdateTentacle(n, delta, playerEnt, dist); break;
                case NpcType1048a: Update1048a(n, delta, playerEnt, dist); break;
                default: UpdateGeneric(n, delta, playerEnt, dist); break;
            }

            SyncVisual(n);
        }

        private static void SyncVisual(NPC n)
        {
            if (n.Obj == -1) return;
            PositionEntity(n.Obj,
                EntityX(n.Collider, true),
                EntityY(n.Collider, true) - 0.32f,
                EntityZ(n.Collider, true),
                true);
            RotateEntity(n.Obj, 0, EntityYaw(n.Collider) - 180f, 0);
        }

        // ── CreateNPC setups ──────────────────────────────────────────────────────

        private static void Setup173(NPC n)
        {
            n.NvName = "SCP-173";
            n.Obj = LoadMesh("GFX/npcs/173_2");
            n.Obj2 = LoadMesh("GFX/173box");
            HideEntity(n.Obj2);
            n.Speed = 0.12f;
            n.CollRadius = 0.32f;
            n.Kind = NPCKind.SCP_173;
            Curr173 = n;
        }

        private static void Setup106(NPC n)
        {
            n.NvName = "SCP-106";
            n.Obj = LoadMesh("GFX/npcs/oldman");
            n.GravityMult = 0f;
            n.MaxGravity = 0f;
            n.Speed = 0.025f;
            n.Kind = NPCKind.SCP_106;
            Curr106 = n;
        }

        private static void SetupGuard(NPC n)
        {
            n.NvName = "Human";
            n.Obj = LoadMesh("GFX/npc/guard");
            n.Speed = 0.03f;
            n.Kind = NPCKind.Guard;
        }

        private static void SetupMtf(NPC n)
        {
            n.NvName = "Human";
            n.Obj = LoadMesh("GFX/npcs/mtf");
            n.Speed = 0.035f;
            n.Kind = NPCKind.Guard;
        }

        private static void SetupClassD(NPC n)
        {
            n.NvName = "Human";
            n.Obj = LoadMesh("GFX/npcs/classd");
            n.Speed = 0.02f;
            n.CollRadius = 0.32f;
        }

        private static void Setup372(NPC n)
        {
            n.NvName = "SCP-372";
            n.Obj = LoadMesh("GFX/npcs/372");
            n.Speed = 0f;
        }

        private static void Setup5131(NPC n)
        {
            n.NvName = "SCP-513-1";
            n.Obj = LoadMesh("GFX/npcs/bll");
            n.Obj2 = CopyEntity(n.Obj);
            Curr5131 = n;
        }

        private static void Setup096(NPC n)
        {
            n.NvName = "SCP-096";
            n.Obj = LoadMesh("GFX/npcs/scp096");
            n.Speed = 0.15f;
            n.CollRadius = 0.26f;
            n.Kind = NPCKind.SCP_096;
            Curr096 = n;
        }

        private static void Setup049(NPC n)
        {
            n.NvName = "SCP-049";
            n.Obj = LoadMesh("GFX/npcs/scp-049");
            n.Speed = 0.03f;
            n.CanUseElevator = true;
            n.Kind = NPCKind.SCP_049;
        }

        private static void SetupZombie(NPC n)
        {
            n.NvName = "Human";
            n.Obj = LoadMesh("GFX/npcs/zombie1");
            n.Speed = 0.025f;
            n.HP = 100;
            n.Kind = NPCKind.SCP_049_2;
        }

        private static void SetupApache(NPC n)
        {
            n.NvName = "Human";
            n.Obj = LoadMesh("GFX/apache");
            n.GravityMult = 0f;
            n.MaxGravity = 0f;
        }

        private static void SetupTentacle(NPC n)
        {
            n.NvName = "Unidentified";
            n.Obj = LoadMesh("GFX/NPCs/035tentacle");
        }

        private static void Setup860(NPC n)
        {
            n.NvName = "Unidentified";
            n.Obj = LoadMesh("GFX/npcs/forestmonster");
            n.Speed = 0.04f;
            n.CollRadius = 0.25f;
        }

        private static void Setup939(NPC n)
        {
            int count = 0;
            foreach (var other in _npcs)
                if (other.NpcType == NpcType939) count++;

            int id = count switch { 0 => 53, 1 => 89, _ => 96 };
            n.NvName = $"SCP-939-{id}";
            n.Obj = LoadMesh("GFX/NPCs/scp-939");
            n.Speed = 0.04f;
            n.CollRadius = 0.3f;
            n.Kind = NPCKind.SCP_939;
        }

        private static void Setup066(NPC n)
        {
            n.NvName = "SCP-066";
            n.Obj = LoadMesh("GFX/NPCs/scp-066");
            n.Speed = 0.03f;
        }

        private static void Setup966(NPC n)
        {
            int count = 1;
            foreach (var other in _npcs)
                if (other.NpcType == NpcType966) count++;

            n.NvName = $"SCP-966-{count}";
            n.Obj = LoadMesh("GFX/npcs/scp-966");
            n.Speed = 0.05f;
        }

        private static void Setup1048a(NPC n)
        {
            n.NvName = "SCP-1048-A";
            n.Obj = LoadMesh("GFX/npcs/scp-1048a");
        }

        private static void Setup1499(NPC n)
        {
            n.NvName = "Unidentified";
            n.Obj = LoadMesh("GFX/npcs/1499-1");
            n.Speed = 0.035f;
        }

        private static void Setup008(NPC n)
        {
            n.NvName = "Human";
            n.Obj = LoadMesh("GFX/npcs/zombiesurgeon");
            n.Speed = 0.02f;
            n.HP = 120;
        }

        private static void SetupClerk(NPC n)
        {
            n.NvName = "Human";
            n.Obj = LoadMesh("GFX/npcs/clerk");
            n.Speed = 0.02f;
            n.CollRadius = 0.32f;
        }

        // ── Update stubs ────────────────────────────────────────────────────────

        private static void Update173(NPC n, float delta, int playerEnt, float dist)
        {
            bool watched = EntityVisible(playerEnt, n.Collider);
            if (GameState.BlinkTimer < -16f || GameState.BlinkTimer > -6f)
            {
                if (EntityVisible(n.Obj, GameState.Camera))
                    watched = true;
            }

            n.BeingWatched = watched;
            if (watched)
            {
                n.StateEnum = NPCState.Idle;
                return;
            }

            n.StateEnum = NPCState.Chase;
            MoveToward(n.Collider, playerEnt, n.Speed * 100f * delta);

            if (dist < 1.5f && n.AtkTimer <= 0f)
            {
                n.AtkTimer = 2f;
                PlayerSystem.Kill();
            }
        }

        private static void Update106(NPC n, float delta, int playerEnt, float dist)
        {
            if (n.State > 0f)
            {
                n.State -= delta;
                return;
            }

            n.StateEnum = NPCState.Chase;
            MoveToward(n.Collider, playerEnt, 2.5f * delta);

            if (dist < 2f && n.AtkTimer <= 0f)
                n.AtkTimer = 3f;
        }

        private static void Update096(NPC n, float delta, int playerEnt, float dist)
        {
            if (n.State < 1f)
            {
                n.StateEnum = NPCState.Idle;
                return;
            }

            n.StateEnum = NPCState.Chase;
            MoveToward(n.Collider, playerEnt, n.Speed * 120f * delta);

            if (dist < 2f && n.AtkTimer <= 0f)
            {
                n.AtkTimer = 2f;
                PlayerSystem.Kill();
            }
        }

        private static void Update049(NPC n, float delta, int playerEnt, float dist)
        {
            if (n.StateEnum == NPCState.Idle && dist < 12f)
                n.StateEnum = NPCState.Chase;

            if (n.StateEnum == NPCState.Chase)
                MoveToward(n.Collider, playerEnt, n.Speed * 80f * delta);

            if (dist < 2f && n.AtkTimer <= 0f)
                n.AtkTimer = 4f;
        }

        private static void UpdateZombie(NPC n, float delta, int playerEnt, float dist)
        {
            if (dist < 18f)
                n.StateEnum = NPCState.Chase;

            if (n.StateEnum == NPCState.Chase)
                MoveToward(n.Collider, playerEnt, n.Speed * 90f * delta);

            if (dist < 1.8f && n.AtkTimer <= 0f)
            {
                n.AtkTimer = 1.5f;
                GameState.Injuries += 0.5f;
            }
        }

        private static void UpdateGuard(NPC n, float delta, int playerEnt, float dist)
        {
            switch (n.StateEnum)
            {
                case NPCState.Idle:
                    if (dist < 15f) n.StateEnum = NPCState.Alert;
                    break;
                case NPCState.Alert:
                    n.AngerLvl = (int)MathHelper.Clamp(n.AngerLvl + delta * 5, 0, 100);
                    if (n.AngerLvl >= 50) n.StateEnum = NPCState.Chase;
                    break;
                case NPCState.Chase:
                    MoveToward(n.Collider, playerEnt, 4f * delta);
                    break;
            }
        }

        private static void UpdateMtf(NPC n, float delta, int playerEnt, float dist)
        {
            if (dist < 25f)
                n.StateEnum = NPCState.Chase;
            if (n.StateEnum == NPCState.Chase)
                MoveToward(n.Collider, playerEnt, 5f * delta);
        }

        private static void Update372(NPC n, float delta, int playerEnt, float dist)
        {
            if (dist < 8f && EntityVisible(playerEnt, n.Collider))
                GameState.Sanity = Math.Max(0f, GameState.Sanity - delta * 2f);
        }

        private static void Update5131(NPC n, float delta, int playerEnt, float dist)
        {
            n.State2 = Math.Max(0f, n.State2 - delta);
            if (n.State > 0f && dist < 30f)
                GameState.Sanity = Math.Max(0f, GameState.Sanity - delta * 0.5f);
        }

        private static void Update939(NPC n, float delta, int playerEnt, float dist)
        {
            bool heard = dist < 10f && GameState.CurrSpeed > 0.05f;
            if (heard || dist < 6f)
                n.StateEnum = NPCState.Chase;
            if (n.StateEnum == NPCState.Chase)
                MoveToward(n.Collider, playerEnt, n.Speed * 100f * delta);
        }

        private static void Update966(NPC n, float delta, int playerEnt, float dist)
        {
            if (dist < 12f && !GameState.Crouch)
            {
                GameState.Stamina = Math.Max(0f, GameState.Stamina - delta * 3f);
                n.StateEnum = NPCState.Alert;
            }
            else
            {
                n.StateEnum = NPCState.Idle;
            }
        }

        private static void Update860(NPC n, float delta, int playerEnt, float dist)
        {
            if (dist < 20f)
            {
                n.StateEnum = NPCState.Chase;
                MoveToward(n.Collider, playerEnt, n.Speed * 100f * delta);
            }
            if (dist < 2f && n.AtkTimer <= 0f)
            {
                n.AtkTimer = 2f;
                PlayerSystem.Kill();
            }
        }

        private static void Update066(NPC n, float delta, int playerEnt, float dist)
        {
            if (dist < 15f)
                n.State += delta;
        }

        private static void Update1499(NPC n, float delta, int playerEnt, float dist)
        {
            if (dist < 16f)
                MoveToward(n.Collider, playerEnt, n.Speed * 80f * delta);
        }

        private static void Update008(NPC n, float delta, int playerEnt, float dist)
        {
            UpdateZombie(n, delta, playerEnt, dist);
        }

        private static void UpdateClassD(NPC n, float delta, int playerEnt, float dist)
        {
            if (dist < 10f)
                MoveAway(n.Collider, playerEnt, 3f * delta);
        }

        private static void UpdateApache(NPC n, float delta, int playerEnt, float dist)
        {
            n.State2 += delta * 20f;
            if (n.Obj2 != -1)
                TurnEntity(n.Obj2, n.State2, 0, 0);
        }

        private static void UpdateTentacle(NPC n, float delta, int playerEnt, float dist)
        {
            n.Frame += delta * 30f;
        }

        private static void Update1048a(NPC n, float delta, int playerEnt, float dist)
        {
            if (dist < 5f && n.AtkTimer <= 0f)
            {
                n.AtkTimer = 5f;
                GameState.Injuries += 1f;
            }
        }

        private static void UpdateGeneric(NPC n, float delta, int playerEnt, float dist)
        {
            if (dist < 20f && n.StateEnum == NPCState.Idle)
                n.StateEnum = NPCState.Chase;
            if (n.StateEnum == NPCState.Chase)
                MoveToward(n.Collider, playerEnt, 3f * delta);
        }

        private static void MoveToward(int mover, int target, float speed)
        {
            var mPos = Get(mover)?.GetWorldPosition() ?? Vector3.Zero;
            var tPos = Get(target)?.GetWorldPosition() ?? Vector3.Zero;
            var dir = tPos - mPos;
            if (dir.LengthSquared() < 0.001f) return;
            dir.Normalize();
            dir.Y = 0f;
            PositionEntity(mover, mPos.X + dir.X * speed, mPos.Y, mPos.Z + dir.Z * speed, true);
        }

        private static void MoveAway(int mover, int target, float speed)
        {
            var mPos = Get(mover)?.GetWorldPosition() ?? Vector3.Zero;
            var tPos = Get(target)?.GetWorldPosition() ?? Vector3.Zero;
            var dir = mPos - tPos;
            if (dir.LengthSquared() < 0.001f) return;
            dir.Normalize();
            dir.Y = 0f;
            PositionEntity(mover, mPos.X + dir.X * speed, mPos.Y, mPos.Z + dir.Z * speed, true);
        }

        public static void FreeAll()
        {
            foreach (var n in _npcs.ToArray())
            {
                FreeEntity(n.Obj);
                if (n.Obj2 != -1) FreeEntity(n.Obj2);
                FreeEntity(n.Collider);
            }
            _npcs.Clear();
            Curr173 = null;
            Curr106 = null;
            Curr096 = null;
            Curr5131 = null;
            NPC.NextId = 1;
        }
    }
}