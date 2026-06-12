// BlitzEntity.cs
// Represents the base "entity" concept from Blitz3D.
// Every object in the Blitz3D world (pivots, meshes, cameras, lights) is an entity
// with a shared transform, parent/child hierarchy, and visibility state.

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace SCPCB360.Engine
{
    public enum EntityType
    {
        Pivot,
        Mesh,
        Camera,
        Light,
        Sprite,
        Terrain
    }

    public class BlitzEntity
    {
        // ── Identity ─────────────────────────────────────────────────────────────
        public int Handle { get; }              // Unique integer handle (Blitz3D returns handles, not objects)
        public string Name { get; set; }
        public EntityType Type { get; }

        // ── Transform (local space) ───────────────────────────────────────────────
        public Vector3 LocalPosition { get; set; } = Vector3.Zero;
        public Quaternion LocalRotation { get; set; } = Quaternion.Identity;
        public Vector3 LocalScale { get; set; } = Vector3.One;

        // Blitz3D uses Euler angles (pitch/yaw/roll in degrees) as its primary interface.
        // We store them separately so RotateEntity / TurnEntity accumulate correctly
        // without gimbal lock surprises from round-tripping through quaternions.
        private float _pitch, _yaw, _roll; // degrees

        // ── Hierarchy ─────────────────────────────────────────────────────────────
        public BlitzEntity Parent { get; private set; }
        private readonly List<BlitzEntity> _children = new();
        public IReadOnlyList<BlitzEntity> Children => _children;

        // ── Render state ──────────────────────────────────────────────────────────
        public bool Visible { get; set; } = true;
        public float Alpha { get; set; } = 1f;          // EntityAlpha()
        public Color EntityColor { get; set; } = Color.White; // EntityColor()
        public int BlendMode { get; set; } = 1;         // 1=solid, 2=alpha, 3=add, 4=multiply

        // ── Mesh data (Mesh/Sprite entities only) ─────────────────────────────────
        public Model XnaModel { get; set; }             // Loaded from .xnb
        public RMeshRenderMesh RMeshRenderMesh { get; set; } // Optional runtime RMESH geometry
        public Texture2D PortalTexture { get; set; }     // Render-to-texture portals (DrawPortals.bb)
        public Texture2D Texture { get; set; }           // EntityTexture()
        public int BrushHandle { get; set; } = -1;      // Painted brush reference
        public int PickMode { get; set; }
        public int SpriteViewMode { get; set; }

        // ── Physics / collision ───────────────────────────────────────────────────
        // Blitz3D EntityType() values: 0=none,1=sphere,2=box,3=polygon,4=box-triggers
        public int CollisionType { get; set; } = 0;
        public float CollisionRadius { get; set; } = 1f;
        public BoundingBox BoundingBox { get; set; }
        public CollisionMesh CollisionMesh { get; set; }  // Optional triangle mesh collision

        // ── Cached world matrix (recomputed on demand) ────────────────────────────
        private Matrix _worldMatrix;
        private bool _worldDirty = true;

        private static int _nextHandle = 1;

        public BlitzEntity(EntityType type, string name = "")
        {
            Handle = _nextHandle++;
            Type = type;
            Name = name;
        }

        // ─────────────────────────────────────────────────────────────────────────
        // Transform helpers — mirror Blitz3D's PositionEntity / RotateEntity / etc.
        // ─────────────────────────────────────────────────────────────────────────

        /// <summary>PositionEntity e, x, y, z [,global]</summary>
        public void SetPosition(float x, float y, float z, bool global = false)
        {
            if (global && Parent != null)
            {
                // Convert world position to local space
                var invParent = Matrix.Invert(Parent.GetWorldMatrix());
                var local = Vector3.Transform(new Vector3(x, y, z), invParent);
                LocalPosition = local;
            }
            else
            {
                LocalPosition = new Vector3(x, y, z);
            }
            MarkDirty();
        }

        /// <summary>MoveEntity e, dx, dy, dz  (local-space translation)</summary>
        public void Move(float dx, float dy, float dz)
        {
            // Translate along entity's own axes
            var localDelta = Vector3.Transform(new Vector3(dx, dy, dz),
                Matrix.CreateFromQuaternion(LocalRotation));
            LocalPosition += localDelta;
            MarkDirty();
        }

        /// <summary>RotateEntity e, pitch, yaw, roll [,global]</summary>
        public void SetRotation(float pitch, float yaw, float roll, bool global = false)
        {
            _pitch = pitch;
            _yaw = yaw;
            _roll = roll;
            RebuildRotationQuaternion();
            MarkDirty();
        }

        /// <summary>TurnEntity e, dpitch, dyaw, droll</summary>
        public void Turn(float dp, float dy, float dr)
        {
            _pitch += dp;
            _yaw += dy;
            _roll += dr;
            RebuildRotationQuaternion();
            MarkDirty();
        }

        /// <summary>ScaleEntity e, sx, sy, sz</summary>
        public void SetScale(float sx, float sy, float sz)
        {
            LocalScale = new Vector3(sx, sy, sz);
            MarkDirty();
        }

        // Blitz3D pitch/yaw/roll → quaternion (ZYX convention to match original engine)
        private void RebuildRotationQuaternion()
        {
            LocalRotation =
                Quaternion.CreateFromAxisAngle(Vector3.UnitY, MathHelper.ToRadians(_yaw)) *
                Quaternion.CreateFromAxisAngle(Vector3.UnitX, MathHelper.ToRadians(_pitch)) *
                Quaternion.CreateFromAxisAngle(Vector3.UnitZ, MathHelper.ToRadians(_roll));
        }

        // ─────────────────────────────────────────────────────────────────────────
        // World matrix (cached, propagated through hierarchy)
        // ─────────────────────────────────────────────────────────────────────────

        public Matrix GetWorldMatrix()
        {
            if (!_worldDirty) return _worldMatrix;

            var local = Matrix.CreateScale(LocalScale)
                      * Matrix.CreateFromQuaternion(LocalRotation)
                      * Matrix.CreateTranslation(LocalPosition);

            _worldMatrix = Parent != null ? local * Parent.GetWorldMatrix() : local;
            _worldDirty = false;
            return _worldMatrix;
        }

        private void MarkDirty()
        {
            _worldDirty = true;
            foreach (var child in _children)
                child.MarkDirty();
        }

        // ─────────────────────────────────────────────────────────────────────────
        // Hierarchy
        // ─────────────────────────────────────────────────────────────────────────

        public void SetParent(BlitzEntity newParent)
        {
            Parent?._children.Remove(this);
            Parent = newParent;
            newParent?._children.Add(this);
            MarkDirty();
        }

        // ─────────────────────────────────────────────────────────────────────────
        // Utility
        // ─────────────────────────────────────────────────────────────────────────

        public Vector3 GetWorldPosition()
        {
            var m = GetWorldMatrix();
            return new Vector3(m.M41, m.M42, m.M43);
        }

        public float GetPitch() => _pitch;
        public float GetYaw()   => _yaw;
        public float GetRoll()  => _roll;

        public override string ToString() => $"[{Type}#{Handle} '{Name}']";
    }
}
