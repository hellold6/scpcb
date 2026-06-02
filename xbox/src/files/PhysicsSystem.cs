// PhysicsSystem.cs
// Tracks Blitz3D Collisions() pairs and resolves simple player-vs-world collisions.

using Microsoft.Xna.Framework;
using System;
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
            Console.WriteLine($"[Physics] Registered collision pair {src} -> {dst} method={method} response={response}");
        }

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
                {
                    foreach (var tgt in targets)
                        ResolveCollision(src, tgt, pair.Method);
                }
            }
        }

        private static void ResolveCollision(BlitzEntity src, BlitzEntity tgt, int method)
        {
            if (src == tgt || method <= 0)
                return;

            if (tgt.CollisionMesh != null)
            {
                ResolveTriangleMeshCollision(src, tgt);
                return;
            }

            ResolveAabbCollision(src, tgt, method);
        }

        private static void ResolveAabbCollision(BlitzEntity src, BlitzEntity tgt, int method)
        {
            var sphere = new BoundingSphere(src.GetWorldPosition(), src.CollisionRadius);
            var world = tgt.GetWorldMatrix();
            var box = new BoundingBox(
                Vector3.Transform(tgt.BoundingBox.Min, world),
                Vector3.Transform(tgt.BoundingBox.Max, world));

            if (!sphere.Intersects(box))
                return;

            var closest = Vector3.Clamp(src.GetWorldPosition(), box.Min, box.Max);
            var delta = src.GetWorldPosition() - closest;
            if (delta.LengthSquared() < 0.0001f)
                delta = src.GetWorldPosition() - ((box.Min + box.Max) * 0.5f);
            if (delta.LengthSquared() < 0.0001f)
                delta = Vector3.UnitZ;

            delta.Normalize();
            var newWorldPos = closest + delta * (src.CollisionRadius + 0.001f);
            var push = newWorldPos - src.GetWorldPosition();
            src.SetPosition(newWorldPos.X, newWorldPos.Y, newWorldPos.Z, true);
            Console.WriteLine($"[Physics] Collision: {src} hit {tgt}; AABB push={push}");
        }

        private static void ResolveTriangleMeshCollision(BlitzEntity src, BlitzEntity target)
        {
            var world = target.GetWorldMatrix();
            var invWorld = Matrix.Invert(world);
            var sphereLocalPos = Vector3.Transform(src.GetWorldPosition(), invWorld);
            float localRadius = src.CollisionRadius / Math.Max(GetMaxScale(world), 0.0001f);

            var verts = target.CollisionMesh.Vertices;
            var indices = target.CollisionMesh.TriangleIndices;

            Vector3 pushDirection = Vector3.Zero;
            float maxPenetration = 0f;

            for (int i = 0; i < indices.Length; i += 3)
            {
                var v0 = verts[indices[i]];
                var v1 = verts[indices[i + 1]];
                var v2 = verts[indices[i + 2]];

                var closest = ClosestPointOnTriangle(sphereLocalPos, v0, v1, v2);
                var delta = sphereLocalPos - closest;
                float distanceSquared = delta.LengthSquared();

                if (distanceSquared >= localRadius * localRadius)
                    continue;

                float distance = (float)Math.Sqrt(distanceSquared);
                Vector3 direction = distance > 0.0001f ? delta / distance : GetTriangleNormal(v0, v1, v2);
                float penetration = localRadius - distance;

                if (penetration > maxPenetration)
                {
                    maxPenetration = penetration;
                    pushDirection = direction;
                }
            }

            if (maxPenetration <= 0f)
                return;

            var localPush = pushDirection * (maxPenetration + 0.001f);
            var worldPush = Vector3.TransformNormal(localPush, world);
            var oldWorldPos = src.GetWorldPosition();
            var newWorldPos = oldWorldPos + worldPush;
            src.SetPosition(newWorldPos.X, newWorldPos.Y, newWorldPos.Z, true);
            Console.WriteLine($"[Physics] Collision: {src} hit {target}; triangle push={worldPush}");
        }

        private static Vector3 ClosestPointOnTriangle(Vector3 p, Vector3 a, Vector3 b, Vector3 c)
        {
            var ab = b - a;
            var ac = c - a;
            var ap = p - a;

            float d1 = Vector3.Dot(ab, ap);
            float d2 = Vector3.Dot(ac, ap);
            if (d1 <= 0f && d2 <= 0f) return a;

            var bp = p - b;
            float d3 = Vector3.Dot(ab, bp);
            float d4 = Vector3.Dot(ac, bp);
            if (d3 >= 0f && d4 <= d3) return b;

            float vc = d1 * d4 - d3 * d2;
            if (vc <= 0f && d1 >= 0f && d3 <= 0f)
            {
                float v = d1 / (d1 - d3);
                return a + ab * v;
            }

            var cp = p - c;
            float d5 = Vector3.Dot(ab, cp);
            float d6 = Vector3.Dot(ac, cp);
            if (d6 >= 0f && d5 <= d6) return c;

            float vb = d5 * d2 - d1 * d6;
            if (vb <= 0f && d2 >= 0f && d6 <= 0f)
            {
                float w = d2 / (d2 - d6);
                return a + ac * w;
            }

            float va = d3 * d6 - d5 * d4;
            if (va <= 0f && (d4 - d3) >= 0f && (d5 - d6) >= 0f)
            {
                float w = (d4 - d3) / ((d4 - d3) + (d5 - d6));
                return b + (c - b) * w;
            }

            float denom = 1f / (va + vb + vc);
            float vInside = vb * denom;
            float wInside = vc * denom;
            return a + ab * vInside + ac * wInside;
        }

        private static Vector3 GetTriangleNormal(Vector3 a, Vector3 b, Vector3 c)
        {
            var normal = Vector3.Cross(b - a, c - a);
            if (normal.LengthSquared() < 0.0001f)
                return Vector3.UnitY;
            normal.Normalize();
            return normal;
        }

        private static float GetMaxScale(Matrix matrix)
        {
            float x = new Vector3(matrix.M11, matrix.M12, matrix.M13).Length();
            float y = new Vector3(matrix.M21, matrix.M22, matrix.M23).Length();
            float z = new Vector3(matrix.M31, matrix.M32, matrix.M33).Length();
            return Math.Max(x, Math.Max(y, z));
        }
    }
}
