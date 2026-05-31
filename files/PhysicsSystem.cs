// PhysicsSystem.cs
// Tracks Blitz3D Collisions() pairs and resolves sphere/box collisions each frame.
// Blitz3D collision model: source type vs destination type, resolution method (1=stop, 2=slide, 3=slide2).

using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace SCPCB360.Engine
{
    public static class PhysicsSystem
    {
        private record CollisionPair(int SrcType, int DstType, int Method, int Response);
        private static readonly List<CollisionPair> _pairs = new();

        public static void RegisterCollisionPair(int src, int dst, int method, int response)
        {
            _pairs.Add(new CollisionPair(src, dst, method, response));
        }

        /// <summary>
        /// Call each Update() tick. Resolves registered collision pairs
        /// using simple sphere vs AABB tests — sufficient for CB's NPC/room geometry.
        /// </summary>
        public static void Update()
        {
            foreach (var pair in _pairs)
            {
                var sources = new List<BlitzEntity>();
                var targets = new List<BlitzEntity>();

                foreach (var e in B3D.AllEntities())
                {
                    if (e.CollisionType == pair.SrcType) sources.Add(e);
                    if (e.CollisionType == pair.DstType) targets.Add(e);
                }

                foreach (var src in sources)
                    foreach (var tgt in targets)
                        ResolveCollision(src, tgt, pair.Method);
            }
        }

        private static void ResolveCollision(BlitzEntity src, BlitzEntity tgt, int method)
        {
            var sphere = new BoundingSphere(src.GetWorldPosition(), src.CollisionRadius);

            // Transform target bounding box to world space
            var world = tgt.GetWorldMatrix();
            var box = new BoundingBox(
                Vector3.Transform(tgt.BoundingBox.Min, world),
                Vector3.Transform(tgt.BoundingBox.Max, world));

            if (sphere.Intersects(box))
            {
                if (method == 1) // Stop
                {
                    // Push source out of target along shortest axis
                    var center = (box.Min + box.Max) * 0.5f;
                    var diff   = src.GetWorldPosition() - center;
                    if (diff == Vector3.Zero) diff = Vector3.UnitY;
                    diff.Normalize();
                    src.SetPosition(
                        center.X + diff.X * src.CollisionRadius,
                        center.Y + diff.Y * src.CollisionRadius,
                        center.Z + diff.Z * src.CollisionRadius,
                        true);
                }
                // method 2/3 (slide) — full implementation requires surface normal from mesh;
                // deferring to a more complete phase 2 collision system.
            }
        }
    }
}
