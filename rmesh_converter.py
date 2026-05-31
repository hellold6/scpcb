import struct
import os
import glob

def read_blitz_string(f):
    """Reads a BlitzBasic length-prefixed string safely."""
    try:
        length_bytes = f.read(4)
        if not length_bytes or len(length_bytes) < 4:
            return ""
        length = struct.unpack('<i', length_bytes)[0]
        if length <= 0 or length > 1024:
            return ""
        return f.read(length).decode('utf-8', errors='ignore')
    except Exception:
        return ""

def convert_rmesh_to_obj(rmesh_path, obj_path):
    if not os.path.exists(rmesh_path):
        print(f"[-] Error: Source file {rmesh_path} not found.")
        return False

    print(f"[*] Processing: {os.path.basename(rmesh_path)}")
    
    with open(rmesh_path, 'rb') as f:
        # 1. Parse the primary RoomMesh Magic Header String
        header = read_blitz_string(f)
        
        # If it's a trigger room variant, the engine loops past the trailing trigger data blocks
        if header == "RoomMesh.HasTrigger":
            trigger_count = struct.unpack('<i', f.read(4))[0]
            for _ in range(trigger_count):
                # Clear out trigger type names and spatial boundary floats
                read_blitz_string(f) 
                f.read(24) # X, Y, Z coordinates + dimensions (6 floats * 4 bytes)
        elif header != "RoomMesh":
            # Fallback check for standard or slight variant text string layouts
            if not header.startswith("RoomM"):
                print(f"[-] Invalid file signature format: '{header}'")
                return False

        # 2. Extract the actual synchronized mesh iteration bounds
        try:
            mesh_count_bytes = f.read(4)
            if not mesh_count_bytes or len(mesh_count_bytes) < 4:
                return False
            mesh_count = struct.unpack('<i', mesh_count_bytes)[0]
        except Exception:
            print("[-] Failed to safely resolve sub-mesh header bounds.")
            return False

        if mesh_count <= 0 or mesh_count > 5000:
            print(f"[-] Unrealistic sub-mesh bounding iteration matrix array: {mesh_count}")
            return False

        all_vertices = []
        all_uvs = []
        all_faces = []
        vertex_offset = 1

        for i in range(mesh_count):
            # Texture Map Asset Slots
            for j in range(2):
                tex_flag = struct.unpack('<B', f.read(1))[0]
                if tex_flag != 0:
                    read_blitz_string(f)

            # Vertices Node Array
            vertex_count = struct.unpack('<i', f.read(4))[0]
            local_vertices = []
            local_uvs = []

            for _ in range(vertex_count):
                x, y, z = struct.unpack('<fff', f.read(12))
                local_vertices.append((x, y, z))
                
                u0, v0 = struct.unpack('<ff', f.read(8))
                u1, v1 = struct.unpack('<ff', f.read(8))
                local_uvs.append((u0, 1.0 - v0))

                f.read(3) # Vertex color values

            # Triangles Layout Map
            face_count = struct.unpack('<i', f.read(4))[0]
            local_faces = []
            for _ in range(face_count):
                idx1, idx2, idx3 = struct.unpack('<iii', f.read(12))
                local_faces.append((idx1 + vertex_offset, idx2 + vertex_offset, idx3 + vertex_offset))

            all_vertices.extend(local_vertices)
            all_uvs.extend(local_uvs)
            all_faces.extend(local_faces)
            
            vertex_offset += vertex_count

    # Write out unified Wavefront OBJ asset
    with open(obj_path, 'w') as out:
        out.write(f"# SCP CB Converted Room Mesh\n\n")
        for v in all_vertices:
            out.write(f"v {v[0]} {v[1]} {v[2]}\n")
        for vt in all_uvs:
            out.write(f"vt {vt[0]} {vt[1]}\n")
        out.write("\ng RoomMesh_Geometry\n")
        for face in all_faces:
            out.write(f"f {face[0]}/{face[0]} {face[1]}/{face[1]} {face[2]}/{face[2]}\n")

    print(f"[+] Successfully exported: {obj_path}")
    return True

if __name__ == "__main__":
    target_dir = os.path.join("GFX", "map")
    if not os.path.exists(target_dir):
        target_dir = "."
        print(f"[*] Scanning current local path alternative...")
    else:
        print(f"[*] Targeted asset directory found: {target_dir}")

    rmesh_files = glob.glob(os.path.join(target_dir, "*.rmesh"))
    if not rmesh_files:
        print("[-] Absolute zero files located.")
    else:
        print(f"[*] Unpacking {len(rmesh_files)} source components...")
        print("-" * 50)
        
        converted_count = 0
        for rmesh_path in rmesh_files:
            obj_path = rmesh_path.replace(".rmesh", ".obj")
            if convert_rmesh_to_obj(rmesh_path, obj_path):
                converted_count += 1
                
        print("-" * 50)
        print(f"[+] Complete. Extracted {converted_count}/{len(rmesh_files)} assets.")