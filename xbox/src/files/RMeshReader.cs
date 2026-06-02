// RMeshReader.cs
// Parses SCP:CB's .rmesh binary format to extract collision mesh geometry.
// RMESH structure (from Converter.bb SaveRoomMesh):
//   - Header string ("RoomMesh" or "RoomMesh.HasTriggerBox")
//   - Visible mesh: surfaces with vertices (pos, uv0, uv1, color) and triangles
//   - Collision mesh (field_hit): surfaces with vertices (pos only) and triangles
//   - Trigger boxes (optional): geometry with names
//   - Point entities: screens, lights, waypoints, etc.

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;

namespace SCPCB360.Engine
{
    public class CollisionMesh
    {
        public Vector3[] Vertices;
        public int[] TriangleIndices;  // flat array: [i0, i1, i2, i0, i1, i2, ...]
        public int TriangleCount => TriangleIndices.Length / 3;
    }

    public class RMeshRenderMesh
    {
        public List<RMeshRenderSurface> Surfaces { get; } = new();
    }

    public class RMeshRenderSurface
    {
        public VertexPositionColorTexture[] Vertices;
        public int[] Indices;
        public string TextureName;
        public string TexturePath;
        public Texture2D Texture;
    }

    public static class RMeshReader
    {
        public static RMeshRenderMesh LoadRenderMesh(string rmeshPath)
        {
            if (!File.Exists(rmeshPath))
            {
                System.Diagnostics.Debug.WriteLine($"[RMesh] File not found: {rmeshPath}");
                return null;
            }

            try
            {
                using (var f = new BinaryReader(File.OpenRead(rmeshPath)))
                {
                    string header = ReadString(f);
                    if (!header.StartsWith("RoomMesh"))
                    {
                        System.Diagnostics.Debug.WriteLine($"[RMesh] Invalid header: {header}");
                        return null;
                    }

                    int drawnMeshSurfaceCount = f.ReadInt32();
                    var mesh = ReadRenderSurfaces(f, drawnMeshSurfaceCount, Path.GetDirectoryName(rmeshPath));
                    return mesh.Surfaces.Count > 0 ? mesh : null;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[RMesh] Error reading render mesh {rmeshPath}: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Reads a .rmesh file and extracts the collision mesh (hidden geometry).
        /// Returns null if file is invalid or collision mesh is empty.
        /// </summary>
        public static CollisionMesh LoadCollisionMesh(string rmeshPath)
        {
            if (!File.Exists(rmeshPath))
            {
                System.Diagnostics.Debug.WriteLine($"[RMesh] File not found: {rmeshPath}");
                return null;
            }

            try
            {
                using (var f = new BinaryReader(File.OpenRead(rmeshPath)))
                {
                    // 1. Read header
                    string header = ReadString(f);
                    if (!header.StartsWith("RoomMesh"))
                    {
                        System.Diagnostics.Debug.WriteLine($"[RMesh] Invalid header: {header}");
                        return null;
                    }

                    // 2. Skip visible mesh (drawnmesh)
                    // Trigger boxes, when present, are written after the hidden collision mesh.
                    int drawnMeshSurfaceCount = f.ReadInt32();
                    for (int i = 0; i < drawnMeshSurfaceCount; i++)
                        SkipSurfaceWithTextures(f);

                    // 3. Read collision mesh (hidden/field_hit)
                    int collisionMeshSurfaceCount = f.ReadInt32();
                    return ReadCollisionSurfaces(f, collisionMeshSurfaceCount);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[RMesh] Error reading {rmeshPath}: {ex.Message}");
                return null;
            }
        }

        private static string ReadString(BinaryReader f)
        {
            try
            {
                int length = f.ReadInt32();
                if (length <= 0 || length > 1024)
                    return "";
                byte[] buf = f.ReadBytes(length);
                return System.Text.Encoding.UTF8.GetString(buf);
            }
            catch
            {
                return "";
            }
        }

        private static void SkipSurfaceWithTextures(BinaryReader f)
        {
            // Texture 0
            byte hasTexture0 = f.ReadByte();
            if (hasTexture0 != 0)
                ReadString(f);

            // Texture 1
            byte hasTexture1 = f.ReadByte();
            if (hasTexture1 != 0)
                ReadString(f);

            // Skip vertices
            int vertexCount = f.ReadInt32();
            for (int j = 0; j < vertexCount; j++)
            {
                f.ReadSingle(); // X
                f.ReadSingle(); // Y
                f.ReadSingle(); // Z
                f.ReadSingle(); // U0
                f.ReadSingle(); // V0
                f.ReadSingle(); // U1
                f.ReadSingle(); // V1
                f.ReadByte();   // R
                f.ReadByte();   // G
                f.ReadByte();   // B
            }

            // Skip triangles
            int triangleCount = f.ReadInt32();
            f.Read(new byte[triangleCount * 12], 0, triangleCount * 12); // 3 ints * 4 bytes each
        }

        private static RMeshRenderMesh ReadRenderSurfaces(BinaryReader f, int surfaceCount, string textureDirectory)
        {
            var mesh = new RMeshRenderMesh();

            for (int i = 0; i < surfaceCount; i++)
            {
                string texture0 = null;
                string texture1 = null;

                if (f.ReadByte() != 0)
                    texture0 = ReadString(f);
                if (f.ReadByte() != 0)
                    texture1 = ReadString(f);

                int vertexCount = f.ReadInt32();
                var vertices = new VertexPositionColorTexture[vertexCount];

                string textureName = !string.IsNullOrWhiteSpace(texture1) ? texture1 : texture0;

                for (int j = 0; j < vertexCount; j++)
                {
                    float x = f.ReadSingle();
                    float y = f.ReadSingle();
                    float z = f.ReadSingle();
                    float u0 = f.ReadSingle();
                    float v0 = f.ReadSingle();
                    float u1 = f.ReadSingle();
                    float v1 = f.ReadSingle();
                    byte r = f.ReadByte();
                    byte g = f.ReadByte();
                    byte b = f.ReadByte();

                    vertices[j] = new VertexPositionColorTexture(
                        new Vector3(x, y, z),
                        new Color(r, g, b),
                        new Vector2(u0, v0));
                }

                int triangleCount = f.ReadInt32();
                var indices = new int[triangleCount * 3];
                for (int j = 0; j < indices.Length; j++)
                    indices[j] = f.ReadInt32();

                if (vertexCount > 0 && indices.Length > 0)
                {
                    mesh.Surfaces.Add(new RMeshRenderSurface
                    {
                        Vertices = vertices,
                        Indices = indices,
                        TextureName = textureName,
                        TexturePath = ResolveTexturePath(textureDirectory, textureName)
                    });
                }
            }

            return mesh;
        }

        public static CollisionMesh BuildVisibleCollisionMesh(RMeshRenderMesh renderMesh)
        {
            if (renderMesh == null)
                return null;

            var allVertices = new List<Vector3>();
            var allIndices = new List<int>();

            foreach (var surface in renderMesh.Surfaces)
            {
                if (ShouldSkipVisibleCollisionSurface(surface.TextureName))
                    continue;

                int vertexOffset = allVertices.Count;
                foreach (var vertex in surface.Vertices)
                    allVertices.Add(vertex.Position);

                foreach (int index in surface.Indices)
                    allIndices.Add(index + vertexOffset);
            }

            if (allVertices.Count == 0 || allIndices.Count == 0)
                return null;

            return new CollisionMesh
            {
                Vertices = allVertices.ToArray(),
                TriangleIndices = allIndices.ToArray()
            };
        }

        private static CollisionMesh ReadCollisionSurfaces(BinaryReader f, int surfaceCount)
        {
            var allVertices = new List<Vector3>();
            var allIndices = new List<int>();

            for (int i = 0; i < surfaceCount; i++)
            {
                int vertexCount = f.ReadInt32();
                int vertexOffset = allVertices.Count;

                // Read vertices (position only, no UV or color)
                for (int j = 0; j < vertexCount; j++)
                {
                    float x = f.ReadSingle();
                    float y = f.ReadSingle();
                    float z = f.ReadSingle();
                    allVertices.Add(new Vector3(x, y, z));
                }

                // Read triangles
                int triangleCount = f.ReadInt32();
                for (int j = 0; j < triangleCount; j++)
                {
                    int i0 = f.ReadInt32() + vertexOffset;
                    int i1 = f.ReadInt32() + vertexOffset;
                    int i2 = f.ReadInt32() + vertexOffset;
                    allIndices.Add(i0);
                    allIndices.Add(i1);
                    allIndices.Add(i2);
                }
            }

            if (allVertices.Count == 0)
                return null;

            return new CollisionMesh
            {
                Vertices = allVertices.ToArray(),
                TriangleIndices = allIndices.ToArray()
            };
        }

        private static string ResolveTexturePath(string textureDirectory, string textureName)
        {
            if (string.IsNullOrWhiteSpace(textureDirectory) || string.IsNullOrWhiteSpace(textureName))
                return null;

            string directPath = Path.Combine(textureDirectory, textureName);
            if (File.Exists(directPath))
                return directPath;

            foreach (var path in Directory.EnumerateFiles(textureDirectory, "*", SearchOption.AllDirectories))
            {
                if (string.Equals(Path.GetFileName(path), textureName, StringComparison.OrdinalIgnoreCase))
                    return path;
            }

            System.Diagnostics.Debug.WriteLine($"[RMesh] Texture not found: {textureName}");
            return null;
        }

        private static bool ShouldSkipVisibleCollisionSurface(string textureName)
        {
            if (string.IsNullOrWhiteSpace(textureName))
                return true;

            string name = Path.GetFileName(textureName).ToLowerInvariant();
            return name.Contains("door")
                || name.Contains("glass")
                || name.Contains("label")
                || name.Contains("paper")
                || name.Contains("logo")
                || name.Contains("controlpanel")
                || name.Contains("misc");
        }
    }
}
