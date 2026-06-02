// Blitz3D.cs
// Static API that mirrors Blitz3D / BlitzBasic command syntax as closely as possible.
// The original .bb source files call things like:
//   n\ent = CreatePivot()
//   PositionEntity n\ent, x#, y#, z#
//   d# = EntityDistance(a, b)
// We replicate that call surface here so porting is a near-mechanical text transformation.
//
// Design notes:
//   • All functions work with integer handles, matching Blitz3D's pointer-as-integer idiom.
//   • EntityWorld holds the global entity registry.
//   • XNA resources (models, textures) are loaded through ContentManager and cached.
//   • Methods are grouped to match the Blitz3D manual chapter structure.

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;

namespace SCPCB360.Engine
{
    public static class B3D
    {
        // ── Global registries ─────────────────────────────────────────────────────

        private static readonly Dictionary<int, BlitzEntity> _entities = new();
        private static ContentManager _content;
        private static GraphicsDevice _gfx;

        // Cache: content path → loaded Model/Texture2D (avoids redundant XNB reads)
        private static readonly Dictionary<string, Model>      _modelCache   = new();
        private static readonly Dictionary<string, Texture2D>  _textureCache = new();

        // Active camera handle
        public static int ActiveCamera { get; private set; } = -1;

        // ── Initialisation (call once from Game.Initialize) ───────────────────────

        public static void Initialize(GraphicsDevice gfx, ContentManager content)
        {
            _gfx = gfx;
            _content = content;
        }

        // ─────────────────────────────────────────────────────────────────────────
        // SECTION 1 — Entity creation
        // ─────────────────────────────────────────────────────────────────────────

        /// <summary>
        /// CreatePivot([parent]) → handle
        /// Pivot = invisible transform node. Used everywhere in CB as a logical parent.
        /// </summary>
        public static int CreatePivot(int parent = -1)
        {
            var e = Register(new BlitzEntity(SCPCB360.Engine.EntityType.Pivot));
            if (parent != -1) EntityParent(e.Handle, parent);
            return e.Handle;
        }

        /// <summary>
        /// LoadMesh(path$, [parent]) → handle
        /// Loads a cooked .xnb model. The path maps CB's GFX\ prefix to the XNB content root.
        /// </summary>
        public static int LoadMesh(string path, int parent = -1)
        {
            var e = Register(new BlitzEntity(SCPCB360.Engine.EntityType.Mesh, Path.GetFileNameWithoutExtension(path)));
            e.XnaModel = LoadModelCached(NormalizePath(path));
            if (e.XnaModel != null)
                e.BoundingBox = ComputeBoundingBox(e.XnaModel);
            if (parent != -1) EntityParent(e.Handle, parent);
            return e.Handle;
        }

        /// <summary>
        /// CreateCamera([parent]) → handle
        /// </summary>
        public static int CreateCamera(int parent = -1)
        {
            var e = Register(new BlitzEntity(SCPCB360.Engine.EntityType.Camera));
            ActiveCamera = e.Handle;
            if (parent != -1) EntityParent(e.Handle, parent);
            return e.Handle;
        }

        /// <summary>
        /// CreateLight([type, parent]) → handle
        /// type: 1=directional, 2=point, 3=spot (matches Blitz3D)
        /// </summary>
        public static int CreateLight(int type = 1, int parent = -1)
        {
            var e = Register(new BlitzEntity(SCPCB360.Engine.EntityType.Light));
            e.CollisionType = type; // repurpose field to store light type
            if (parent != -1) EntityParent(e.Handle, parent);
            return e.Handle;
        }

        /// <summary>
        /// CopyEntity(src, [parent]) → handle
        /// Shallow copy — shares the same XnaModel reference (shared GPU buffer, no extra VRAM).
        /// </summary>
        public static int CopyEntity(int src, int parent = -1)
        {
            if (!TryGet(src, out var s)) return -1;
            var e = Register(new BlitzEntity(s.Type, s.Name + "_copy"));
            e.XnaModel   = s.XnaModel;
            e.BoundingBox = s.BoundingBox;
            e.BrushHandle = s.BrushHandle;
            if (parent != -1) EntityParent(e.Handle, parent);
            return e.Handle;
        }

        // ─────────────────────────────────────────────────────────────────────────
        // SECTION 2 — Transform commands
        // ─────────────────────────────────────────────────────────────────────────

        public static void PositionEntity(int h, float x, float y, float z, bool global = false)
            => Get(h)?.SetPosition(x, y, z, global);

        public static void MoveEntity(int h, float x, float y, float z)
            => Get(h)?.Move(x, y, z);

        public static void RotateEntity(int h, float pitch, float yaw, float roll, bool global = false)
            => Get(h)?.SetRotation(pitch, yaw, roll, global);

        public static void TurnEntity(int h, float dp, float dy, float dr)
            => Get(h)?.Turn(dp, dy, dr);

        public static void ScaleEntity(int h, float sx, float sy, float sz)
            => Get(h)?.SetScale(sx, sy, sz);

        public static void AlignToVector(int h, float vx, float vy, float vz, int axis = 2)
        {
            // axis: 1=X, 2=Y, 3=Z  — most common is axis=2 (face entity along its Y)
            if (!TryGet(h, out var e)) return;
            var target = new Vector3(vx, vy, vz);
            if (target == Vector3.Zero) return;
            target.Normalize();
            var from   = axis == 1 ? Vector3.UnitX : axis == 2 ? Vector3.UnitY : Vector3.UnitZ;
            var rot    = QuaternionFromTo(from, target);
            e.LocalRotation = rot;
        }

        public static void PointEntity(int h, int target, float roll = 0f)
        {
            if (!TryGet(h, out var ent) || !TryGet(target, out var tgt)) return;
            var dir = Vector3.Normalize(tgt.GetWorldPosition() - ent.GetWorldPosition());
            AlignToVector(h, dir.X, dir.Y, dir.Z, 3);
        }

        // ─────────────────────────────────────────────────────────────────────────
        // SECTION 3 — Entity queries
        // ─────────────────────────────────────────────────────────────────────────

        public static float EntityX(int h, bool global = false)
            => global ? Get(h)?.GetWorldPosition().X ?? 0f : Get(h)?.LocalPosition.X ?? 0f;

        public static float EntityY(int h, bool global = false)
            => global ? Get(h)?.GetWorldPosition().Y ?? 0f : Get(h)?.LocalPosition.Y ?? 0f;

        public static float EntityZ(int h, bool global = false)
            => global ? Get(h)?.GetWorldPosition().Z ?? 0f : Get(h)?.LocalPosition.Z ?? 0f;

        public static float EntityPitch(int h) => Get(h)?.GetPitch() ?? 0f;
        public static float EntityYaw(int h)   => Get(h)?.GetYaw()   ?? 0f;
        public static float EntityRoll(int h)  => Get(h)?.GetRoll()  ?? 0f;

        /// <summary>EntityDistance(a, b) → float</summary>
        public static float EntityDistance(int a, int b)
        {
            if (!TryGet(a, out var ea) || !TryGet(b, out var eb)) return 0f;
            return Vector3.Distance(ea.GetWorldPosition(), eb.GetWorldPosition());
        }

        /// <summary>EntityVisible(a, b) — approximate LOS check via BoundingFrustum</summary>
        public static bool EntityVisible(int a, int b)
        {
            if (!TryGet(a, out _) || !TryGet(b, out var eb)) return false;
            if (ActiveCamera == -1) return false;
            var cam = BuildCameraFrustum();
            return cam.Contains(eb.GetWorldPosition()) != ContainmentType.Disjoint;
        }

        // ─────────────────────────────────────────────────────────────────────────
        // SECTION 4 — Entity appearance
        // ─────────────────────────────────────────────────────────────────────────

        public static void EntityAlpha(int h, float a)
        {
            if (TryGet(h, out var e)) e.Alpha = MathHelper.Clamp(a, 0f, 1f);
        }

        public static void EntityColor(int h, float r, float g, float b)
        {
            if (TryGet(h, out var e))
                e.EntityColor = new Color(
                    (int)MathHelper.Clamp(r, 0, 255),
                    (int)MathHelper.Clamp(g, 0, 255),
                    (int)MathHelper.Clamp(b, 0, 255));
        }

        public static void EntityBlend(int h, int mode) { if (TryGet(h, out var e)) e.BlendMode = mode; }

        public static void HideEntity(int h) { if (TryGet(h, out var e)) e.Visible = false; }
        public static void ShowEntity(int h) { if (TryGet(h, out var e)) e.Visible = true; }

        public static void EntityParent(int child, int parent)
        {
            if (TryGet(child, out var c))
                c.SetParent(parent >= 0 && TryGet(parent, out var p) ? p : null);
        }

        // ─────────────────────────────────────────────────────────────────────────
        // SECTION 5 — Entity removal
        // ─────────────────────────────────────────────────────────────────────────

        /// <summary>FreeEntity e</summary>
        public static void FreeEntity(int h)
        {
            if (!TryGet(h, out var e)) return;
            // Detach children before removal so they don't become orphaned handles
            foreach (var child in new List<BlitzEntity>(e.Children))
                child.SetParent(null);
            e.SetParent(null);
            _entities.Remove(h);
        }

        // ─────────────────────────────────────────────────────────────────────────
        // SECTION 6 — Texture loading
        // ─────────────────────────────────────────────────────────────────────────

        /// <summary>LoadTexture(path$) → texture handle (stored as entity handle convention)</summary>
        public static Texture2D LoadTexture(string path)
        {
            var key = NormalizePath(path);
            if (_textureCache.TryGetValue(key, out var cached)) return cached;
            try
            {
                var tex = _content.Load<Texture2D>(key);
                _textureCache[key] = tex;
                return tex;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[B3D] LoadTexture failed: {path} → {ex.Message}");
                return null;
            }
        }

        // ─────────────────────────────────────────────────────────────────────────
        // SECTION 7 — Collision (simplified Blitz3D interface)
        // ─────────────────────────────────────────────────────────────────────────

        public static void EntityType(int h, int colType, bool recursive = false)
        {
            if (!TryGet(h, out var e)) return;
            e.CollisionType = colType;
            if (recursive)
                foreach (var child in e.Children)
                    EntityType(child.Handle, colType, true);
        }

        public static void EntityRadius(int h, float xRadius, float yRadius = 0f)
        {
            if (TryGet(h, out var e))
                e.CollisionRadius = xRadius;
        }

        public static void Collisions(int srcType, int dstType, int method, int response)
        {
            // Blitz3D's Collisions() sets up pairs.
            // We defer to PhysicsSystem for actual resolution — record intent here.
            PhysicsSystem.RegisterCollisionPair(srcType, dstType, method, response);
        }

        // ─────────────────────────────────────────────────────────────────────────
        // SECTION 8 — Math helpers (used directly in .bb logic)
        // ─────────────────────────────────────────────────────────────────────────

        public static float ATan2(float y, float x) => MathHelper.ToDegrees((float)Math.Atan2(y, x));
        public static float Abs(float v) => Math.Abs(v);
        public static float Sqr(float v) => (float)Math.Sqrt(v);

        // Blitz3D's Rand(min, max) is inclusive on both ends
        private static readonly Random _rng = new();
        public static int Rand(int min, int max) => _rng.Next(min, max + 1);
        public static float Rnd(float min = 0f, float max = 1f) => (float)(_rng.NextDouble() * (max - min) + min);

        // ─────────────────────────────────────────────────────────────────────────
        // Internal helpers
        // ─────────────────────────────────────────────────────────────────────────

        private static BlitzEntity Register(BlitzEntity e)
        {
            _entities[e.Handle] = e;
            return e;
        }

        public static BlitzEntity Get(int h)
        {
            _entities.TryGetValue(h, out var e);
            return e;
        }

        public static bool TryGet(int h, out BlitzEntity e) => _entities.TryGetValue(h, out e);

        public static IEnumerable<BlitzEntity> AllEntities() => _entities.Values;

        private static string NormalizePath(string path)
        {
            return path
                .Replace('\\', '/')
                .Replace(".b3d", "")
                .Replace(".fbx", "")
                .Replace(".x", "")
                .Replace(".obj", "");
        }

        private static Model LoadModelCached(string key)
        {
            if (_modelCache.TryGetValue(key, out var m)) return m;
            try
            {
                m = _content.Load<Model>(key);
                _modelCache[key] = m;
                return m;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[B3D] LoadMesh failed: {key} → {ex.Message}");
                return null;
            }
        }

        private static BoundingBox ComputeBoundingBox(Model model)
        {
            var min = new Vector3(float.MaxValue);
            var max = new Vector3(float.MinValue);
            foreach (var mesh in model.Meshes)
            {
                foreach (var part in mesh.MeshParts)
                {
                    var verts = new VertexPositionNormalTexture[part.NumVertices];
                    part.VertexBuffer.GetData(verts);
                    foreach (var v in verts)
                    {
                        min = Vector3.Min(min, v.Position);
                        max = Vector3.Max(max, v.Position);
                    }
                }
            }
            return new BoundingBox(min, max);
        }

        private static BoundingFrustum BuildCameraFrustum()
        {
            // Pull camera world matrix and build view/projection for frustum check
            var cam = Get(ActiveCamera);
            var view = Matrix.Invert(cam.GetWorldMatrix());
            var proj = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.ToRadians(60f), _gfx.Viewport.AspectRatio, 0.1f, 1000f);
            return new BoundingFrustum(view * proj);
        }

        private static Quaternion QuaternionFromTo(Vector3 from, Vector3 to)
        {
            var cross = Vector3.Cross(from, to);
            var dot   = Vector3.Dot(from, to);
            if (cross.LengthSquared() < 1e-6f) return Quaternion.Identity;
            cross.Normalize();
            var angle = (float)Math.Acos(MathHelper.Clamp(dot, -1f, 1f));
            return Quaternion.CreateFromAxisAngle(cross, angle);
        }
    }
}
