#include <xtl.h>
#include <xboxmath.h>
#include <xaudio2.h>
#include <vector>
#include <string>
#include <cstdio>
#include <cstdarg>
#include <cmath>
#include <cstring>

struct Vertex
{
    FLOAT x, y, z;
    DWORD color;
    FLOAT u, v;
};

struct RMeshSurface
{
    std::vector<Vertex> verts;
    IDirect3DTexture9* texture;
    std::string textureName;
    FLOAT minX, maxX;
    FLOAT minZ, maxZ;
};

struct NativeModelVertex
{
    FLOAT x, y, z;
    FLOAT nx, ny, nz;
    FLOAT u, v;
};

struct NativeModelSurface
{
    std::vector<NativeModelVertex> verts;
    IDirect3DTexture9* texture;
    std::string textureName;
};

struct NativeModel
{
    std::vector<NativeModelSurface> surfaces;
    bool loaded;
};

struct IntroModelInstance
{
    const NativeModel* model;
    FLOAT x, y, z;
    FLOAT sx, sy, sz;
    FLOAT yaw;
    DWORD tint;
};

struct TextureCacheEntry
{
    std::string name;
    IDirect3DTexture9* texture;
};

struct CollisionBatch
{
    std::vector<Vertex> verts;
    FLOAT minX, maxX;
    FLOAT minY, maxY;
    FLOAT minZ, maxZ;
};

struct NativeDoor
{
    FLOAT x, y, z;
    FLOAT angle;
    INT dir;
    INT keycard;
    INT fastOpen;
    FLOAT openState;
    FLOAT timer;
    FLOAT timerState;
    bool locked;
    bool open;
    bool autoClose;
    char code[16];
};

static IDirect3D9* g_d3d = NULL;
static IDirect3DDevice9* g_device = NULL;
static IDirect3DVertexShader9* g_vs = NULL;
static IDirect3DPixelShader9* g_ps = NULL;
static IDirect3DVertexDeclaration9* g_decl = NULL;
static IDirect3DTexture9* g_whiteTexture = NULL;

static IXAudio2* g_audio = NULL;
static IXAudio2MasteringVoice* g_masterVoice = NULL;
static IXAudio2SourceVoice* g_beepVoice = NULL;
static std::vector<BYTE> g_beepPcm;

static std::vector<Vertex> g_roomVerts;
static std::vector<RMeshSurface> g_roomSurfaces;
static std::vector<TextureCacheEntry> g_textureCache;
static std::vector<CollisionBatch> g_collisionBatches;
static std::vector<Vertex> g_scriptedVerts;
static std::vector<RMeshSurface> g_scriptedSurfaces;
static std::vector<IntroModelInstance> g_introModelInstances;
static std::vector<Vertex> g_modelDrawScratch;
static std::vector<Vertex> g_screenQuadScratch;
static std::vector<Vertex> g_doorVerts;
static std::vector<RMeshSurface> g_doorSurfaces;
static std::vector<NativeDoor> g_doors;
static NativeModel g_modelDoorPanel;
static NativeModel g_modelDoorFrame;
static NativeModel g_modelButton;
static NativeModel g_modelButtonKeycard;
static NativeModel g_modelButtonCode;
static NativeModel g_modelButtonScanner;
static NativeModel g_modelHeavyDoor1;
static NativeModel g_modelHeavyDoor2;
static NativeModel g_modelContDoorLeft;
static NativeModel g_modelContDoorRight;
static NativeModel g_modelNpc173;
static NativeModel g_modelNpcGuard;
static NativeModel g_modelNpcClassD;
static NativeModel g_modelNpcClerk;
static INT g_texturesLoaded = 0;
static INT g_texturesFailed = 0;
static INT g_modelAssetsLoaded = 0;
static INT g_modelAssetsFailed = 0;
static INT g_facilityRoomCount = 0;
static INT g_facilityLoadFailures = 0;
static INT g_doorToggleCount = 0;
static char g_status[256] = "Booting native tech demo";

static XMVECTOR g_playerPos = XMVectorSet(0.0f, 1.7f, -12.0f, 0.0f);
static FLOAT g_yaw = 0.0f;
static FLOAT g_pitch = 0.0f;
static FLOAT g_fps = 0.0f;
static FLOAT g_frameMs = 0.0f;
static FLOAT g_collisionFlash = 0.0f;
static INT g_collisionCount = 0;
static INT g_surfacesDrawn = 0;
static INT g_surfacesCulled = 0;
static INT g_modelInstancesDrawn = 0;
static INT g_modelInstancesCulled = 0;
static UINT g_worldTrisSubmitted = 0;
static FLOAT g_renderDistance = 26.0f;
static FLOAT g_blinkFrequency = 7.2f;
static FLOAT g_blinkTimer = 7.2f;
static FLOAT g_blinkAlpha = 0.0f;
static FLOAT g_stamina = 100.0f;
static FLOAT g_staminaEffect = 1.0f;
static bool g_crouch = false;
static FLOAT g_crouchState = 0.0f;
static bool g_playerSprinting = false;
static FLOAT g_stepCycle = 0.0f;
static INT g_stepCueCount = 0;

static DWORD g_lastButtons = 0;
static LARGE_INTEGER g_lastTime;
static LARGE_INTEGER g_freq;
static FLOAT g_fpsTimer = 0.0f;
static INT g_fpsFrames = 0;

enum DemoState
{
    STATE_MENU,
    STATE_LOADING,
    STATE_INTRO,
    STATE_PLAYING
};

static DemoState g_state = STATE_MENU;
static INT g_loadingPercent = 0;
static char g_loadingText[128] = "PRESS A TO START";
static DWORD g_mapSeed = 173360;
static IDirect3DTexture9* g_menuBackTexture = NULL;
static IDirect3DTexture9* g_menu173Texture = NULL;
static IDirect3DTexture9* g_loadingBackTexture = NULL;
static IDirect3DTexture9* g_loadingImageTexture = NULL;
static FLOAT g_introTimer = 0.0f;
static FLOAT g_introBlackout = 0.0f;
static bool g_introTransferred = false;
static bool g_introNpcAssetsAttempted = false;
static char g_introSubtitle[128] = "";
static char g_introSubtitle2[128] = "";

static void RenderLoadingScreen(INT percent, const char* detail);
static void PlayBeep();
static void ResetPlayerVitals();
static void InitModelAssets();
static void LoadIntroNpcAssets();

static const CHAR* g_vsCode =
    "float4x4 matWVP : register(c0);"
    "struct VS_IN { float4 Pos : POSITION; float4 Color : COLOR; float2 Tex : TEXCOORD; };"
    "struct VS_OUT { float4 Pos : POSITION; float4 Color : COLOR; float2 Tex : TEXCOORD; };"
    "VS_OUT main(VS_IN In) { VS_OUT Out; Out.Pos = mul(matWVP, In.Pos); Out.Color = In.Color; Out.Tex = In.Tex; return Out; }";

static const CHAR* g_psCode =
    "struct PS_IN { float4 Color : COLOR; float2 Tex : TEXCOORD; };"
    "sampler detail : register(s0);"
    "float4 main(PS_IN In) : COLOR { return tex2D(detail, In.Tex) * In.Color; }";

static void SetStatus(const char* fmt, ...)
{
    va_list args;
    va_start(args, fmt);
    _vsnprintf(g_status, sizeof(g_status) - 1, fmt, args);
    g_status[sizeof(g_status) - 1] = 0;
    va_end(args);
    OutputDebugStringA(g_status);
    OutputDebugStringA("\n");
}

static FLOAT Clamp(FLOAT v, FLOAT lo, FLOAT hi)
{
    return v < lo ? lo : (v > hi ? hi : v);
}

static FLOAT ApplyDeadZone(SHORT value, SHORT deadZone)
{
    if (value > -deadZone && value < deadZone)
        return 0.0f;

    FLOAT sign = value < 0 ? -1.0f : 1.0f;
    FLOAT magnitude = (fabsf((FLOAT)value) - (FLOAT)deadZone) / (32767.0f - (FLOAT)deadZone);
    return Clamp(magnitude, 0.0f, 1.0f) * sign;
}

static bool ReadExact(FILE* f, void* dst, size_t bytes)
{
    return fread(dst, 1, bytes, f) == bytes;
}

static bool ReadInt32LE(FILE* f, INT* out)
{
    BYTE b[4];
    if (!ReadExact(f, b, 4))
        return false;
    DWORD v = ((DWORD)b[0]) | ((DWORD)b[1] << 8) | ((DWORD)b[2] << 16) | ((DWORD)b[3] << 24);
    *out = (INT)v;
    return true;
}

static bool ReadFloatLE(FILE* f, FLOAT* out)
{
    BYTE b[4];
    if (!ReadExact(f, b, 4))
        return false;
    DWORD bits = ((DWORD)b[0]) | ((DWORD)b[1] << 8) | ((DWORD)b[2] << 16) | ((DWORD)b[3] << 24);
    memcpy(out, &bits, sizeof(FLOAT));
    return true;
}

static bool ReadStringLE(FILE* f, std::string& out)
{
    INT len = 0;
    if (!ReadInt32LE(f, &len) || len < 0 || len > 4096)
        return false;

    out.assign((size_t)len, '\0');
    return len == 0 || ReadExact(f, &out[0], (size_t)len);
}

static void AddTri(std::vector<Vertex>& v, const Vertex& a, const Vertex& b, const Vertex& c)
{
    v.push_back(a);
    v.push_back(b);
    v.push_back(c);
}

static void AddQuad(std::vector<Vertex>& v, XMVECTOR a, XMVECTOR b, XMVECTOR c, XMVECTOR d, DWORD color)
{
    Vertex va = { XMVectorGetX(a), XMVectorGetY(a), XMVectorGetZ(a), color, 0.0f, 0.0f };
    Vertex vb = { XMVectorGetX(b), XMVectorGetY(b), XMVectorGetZ(b), color, 1.0f, 0.0f };
    Vertex vc = { XMVectorGetX(c), XMVectorGetY(c), XMVectorGetZ(c), color, 1.0f, 1.0f };
    Vertex vd = { XMVectorGetX(d), XMVectorGetY(d), XMVectorGetZ(d), color, 0.0f, 1.0f };
    AddTri(v, va, vb, vc);
    AddTri(v, va, vc, vd);
}

static void AddBox(std::vector<Vertex>& v, FLOAT minX, FLOAT minY, FLOAT minZ, FLOAT maxX, FLOAT maxY, FLOAT maxZ, DWORD color)
{
    XMVECTOR p000 = XMVectorSet(minX, minY, minZ, 0);
    XMVECTOR p001 = XMVectorSet(minX, minY, maxZ, 0);
    XMVECTOR p010 = XMVectorSet(minX, maxY, minZ, 0);
    XMVECTOR p011 = XMVectorSet(minX, maxY, maxZ, 0);
    XMVECTOR p100 = XMVectorSet(maxX, minY, minZ, 0);
    XMVECTOR p101 = XMVectorSet(maxX, minY, maxZ, 0);
    XMVECTOR p110 = XMVectorSet(maxX, maxY, minZ, 0);
    XMVECTOR p111 = XMVectorSet(maxX, maxY, maxZ, 0);

    AddQuad(v, p000, p100, p110, p010, color);
    AddQuad(v, p101, p001, p011, p111, color);
    AddQuad(v, p001, p000, p010, p011, color);
    AddQuad(v, p100, p101, p111, p110, color);
    AddQuad(v, p010, p110, p111, p011, color);
    AddQuad(v, p001, p101, p100, p000, color);
}

static void InitSurfaceBounds(RMeshSurface& surface)
{
    surface.minX = surface.minZ = 1000000.0f;
    surface.maxX = surface.maxZ = -1000000.0f;
}

static void ExpandSurfaceBounds(RMeshSurface& surface, const Vertex& v)
{
    surface.minX = min(surface.minX, v.x);
    surface.maxX = max(surface.maxX, v.x);
    surface.minZ = min(surface.minZ, v.z);
    surface.maxZ = max(surface.maxZ, v.z);
}

static Vertex TransformVertex(const Vertex& v, FLOAT x, FLOAT y, FLOAT z, FLOAT yawDegrees)
{
    FLOAT radians = yawDegrees * (XM_PI / 180.0f);
    FLOAT c = cosf(radians);
    FLOAT s = sinf(radians);

    Vertex out = v;
    out.x = v.x * c + v.z * s + x;
    out.y = v.y + y;
    out.z = -v.x * s + v.z * c + z;
    return out;
}

static void ClearFacilityGeometry()
{
    g_roomVerts.clear();
    g_roomSurfaces.clear();
    g_collisionBatches.clear();
    g_textureCache.clear();
    g_scriptedVerts.clear();
    g_scriptedSurfaces.clear();
    g_introModelInstances.clear();
    g_doorVerts.clear();
    g_doorSurfaces.clear();
    g_doors.clear();
    g_texturesLoaded = 0;
    g_texturesFailed = 0;
    g_facilityRoomCount = 0;
    g_facilityLoadFailures = 0;
    g_doorToggleCount = 0;
}

static void AddCollisionBatch(const std::vector<Vertex>& verts)
{
    if (verts.empty())
        return;

    CollisionBatch batch;
    batch.verts = verts;
    batch.minX = batch.minY = batch.minZ = 1000000.0f;
    batch.maxX = batch.maxY = batch.maxZ = -1000000.0f;

    for (size_t i = 0; i < verts.size(); ++i)
    {
        const Vertex& v = verts[i];
        batch.minX = min(batch.minX, v.x);
        batch.maxX = max(batch.maxX, v.x);
        batch.minY = min(batch.minY, v.y);
        batch.maxY = max(batch.maxY, v.y);
        batch.minZ = min(batch.minZ, v.z);
        batch.maxZ = max(batch.maxZ, v.z);
    }

    g_collisionBatches.push_back(batch);
}

static std::string BaseName(const std::string& path)
{
    size_t slash = path.find_last_of("\\/");
    if (slash == std::string::npos)
        return path;
    return path.substr(slash + 1);
}

static std::string StemName(const std::string& name)
{
    size_t dot = name.find_last_of('.');
    if (dot == std::string::npos)
        return name;
    return name.substr(0, dot);
}

static IDirect3DTexture9* LoadTextureCached(const std::string& textureName)
{
    std::string name = BaseName(textureName);
    if (name.empty())
        return g_whiteTexture;

    for (size_t i = 0; i < g_textureCache.size(); ++i)
    {
        if (_stricmp(g_textureCache[i].name.c_str(), name.c_str()) == 0)
            return g_textureCache[i].texture ? g_textureCache[i].texture : g_whiteTexture;
    }

    const char* prefixes[] =
    {
        "game:\\GFX\\map\\",
        "game:\\GFX\\npcs\\",
        "game:\\",
        ".\\GFX\\map\\",
        ".\\GFX\\npcs\\",
        "..\\..\\GFX\\map\\",
        "..\\..\\GFX\\npcs\\",
    };

    IDirect3DTexture9* texture = NULL;
    char fullPath[512];
    const char* extensions[] = { "", ".png", ".jpg", ".jpeg", ".bmp", ".dds", ".tga" };
    std::string stem = StemName(name);

    for (int e = 0; e < (int)(sizeof(extensions) / sizeof(extensions[0])) && !texture; ++e)
    {
        std::string candidate = (e == 0) ? name : (stem + extensions[e]);
        for (int i = 0; i < (int)(sizeof(prefixes) / sizeof(prefixes[0])); ++i)
        {
            _snprintf(fullPath, sizeof(fullPath), "%s%s", prefixes[i], candidate.c_str());
            fullPath[sizeof(fullPath) - 1] = 0;
            if (SUCCEEDED(D3DXCreateTextureFromFile(g_device, fullPath, &texture)))
                break;
        }
    }

    TextureCacheEntry entry;
    entry.name = name;
    entry.texture = texture;
    g_textureCache.push_back(entry);

    if (texture)
    {
        ++g_texturesLoaded;
        return texture;
    }

    ++g_texturesFailed;
    return g_whiteTexture;
}

struct XModelFVFVertex
{
    FLOAT x, y, z;
    FLOAT nx, ny, nz;
    FLOAT u, v;
};

static DWORD ScaleColor(DWORD tint, FLOAT lighting)
{
    lighting = Clamp(lighting, 0.0f, 1.25f);
    INT r = (INT)(((tint >> 16) & 0xff) * lighting);
    INT g = (INT)(((tint >> 8) & 0xff) * lighting);
    INT b = (INT)((tint & 0xff) * lighting);
    r = r < 0 ? 0 : (r > 255 ? 255 : r);
    g = g < 0 ? 0 : (g > 255 ? 255 : g);
    b = b < 0 ? 0 : (b > 255 ? 255 : b);
    return D3DCOLOR_XRGB(r, g, b);
}

static DWORD LitModelColor(DWORD tint, FLOAT nx, FLOAT ny, FLOAT nz)
{
    XMVECTOR n = XMVector3Normalize(XMVectorSet(nx, ny, nz, 0.0f));
    XMVECTOR lightDir = XMVector3Normalize(XMVectorSet(-0.30f, 0.88f, -0.36f, 0.0f));
    FLOAT ndotl = max(0.0f, XMVectorGetX(XMVector3Dot(n, lightDir)));
    FLOAT lighting = 0.52f + ndotl * 0.50f;
    return ScaleColor(tint, lighting);
}

static bool LoadXModel(NativeModel& model, const char* fileName)
{
    model.surfaces.clear();
    model.loaded = false;

    const char* prefixes[] =
    {
        "game:\\GFX\\map\\",
        ".\\GFX\\map\\",
        "..\\..\\GFX\\map\\",
    };

    ID3DXMesh* rawMesh = NULL;
    ID3DXBuffer* materialsBuffer = NULL;
    DWORD materialCount = 0;
    char fullPath[512];
    HRESULT hr = E_FAIL;

    for (int i = 0; i < (int)(sizeof(prefixes) / sizeof(prefixes[0])); ++i)
    {
        _snprintf(fullPath, sizeof(fullPath), "%s%s", prefixes[i], fileName);
        fullPath[sizeof(fullPath) - 1] = 0;
        hr = D3DXLoadMeshFromX(fullPath, D3DXMESH_SYSTEMMEM, g_device, NULL, &materialsBuffer, NULL, &materialCount, &rawMesh);
        if (SUCCEEDED(hr) && rawMesh)
            break;
    }

    if (!rawMesh)
    {
        ++g_modelAssetsFailed;
        return false;
    }

    ID3DXMesh* mesh = NULL;
    DWORD fvf = D3DFVF_XYZ | D3DFVF_NORMAL | D3DFVF_TEX1;
    hr = rawMesh->CloneMeshFVF(D3DXMESH_SYSTEMMEM, fvf, g_device, &mesh);
    rawMesh->Release();
    if (FAILED(hr) || !mesh)
    {
        if (materialsBuffer) materialsBuffer->Release();
        ++g_modelAssetsFailed;
        return false;
    }

    DWORD surfaceCount = materialCount > 1 ? materialCount : 1;
    model.surfaces.resize(surfaceCount);
    D3DXMATERIAL* materials = materialsBuffer ? (D3DXMATERIAL*)materialsBuffer->GetBufferPointer() : NULL;
    for (DWORD i = 0; i < surfaceCount; ++i)
    {
        const char* textureName = (materials && i < materialCount) ? materials[i].pTextureFilename : NULL;
        model.surfaces[i].textureName = textureName ? BaseName(textureName) : "";
        model.surfaces[i].texture = textureName ? LoadTextureCached(model.surfaces[i].textureName) : g_whiteTexture;
    }

    XModelFVFVertex* vertices = NULL;
    void* indices = NULL;
    DWORD* attributes = NULL;
    bool ok = SUCCEEDED(mesh->LockVertexBuffer(D3DLOCK_READONLY, (void**)&vertices)) &&
        SUCCEEDED(mesh->LockIndexBuffer(D3DLOCK_READONLY, &indices));

    if (ok)
    {
        if (FAILED(mesh->LockAttributeBuffer(D3DLOCK_READONLY, &attributes)))
            attributes = NULL;

        DWORD stride = mesh->GetNumBytesPerVertex();
        DWORD faceCount = mesh->GetNumFaces();
        bool index32 = (mesh->GetOptions() & D3DXMESH_32BIT) != 0;

        for (DWORD face = 0; face < faceCount; ++face)
        {
            DWORD subset = attributes ? attributes[face] : 0;
            if (subset >= surfaceCount)
                subset = 0;

            for (DWORD corner = 0; corner < 3; ++corner)
            {
                DWORD index = index32 ? ((DWORD*)indices)[face * 3 + corner] : ((WORD*)indices)[face * 3 + corner];
                XModelFVFVertex* src = (XModelFVFVertex*)((BYTE*)vertices + index * stride);
                NativeModelVertex dst;
                dst.x = src->x;
                dst.y = src->y;
                dst.z = src->z;
                dst.nx = src->nx;
                dst.ny = src->ny;
                dst.nz = src->nz;
                dst.u = src->u;
                dst.v = src->v;
                model.surfaces[subset].verts.push_back(dst);
            }
        }
    }

    if (attributes)
        mesh->UnlockAttributeBuffer();
    if (indices)
        mesh->UnlockIndexBuffer();
    if (vertices)
        mesh->UnlockVertexBuffer();
    mesh->Release();
    if (materialsBuffer)
        materialsBuffer->Release();

    if (!ok)
    {
        model.surfaces.clear();
        ++g_modelAssetsFailed;
        return false;
    }

    bool any = false;
    for (size_t i = 0; i < model.surfaces.size(); ++i)
    {
        if (!model.surfaces[i].verts.empty())
        {
            any = true;
            break;
        }
    }

    if (any)
    {
        model.loaded = true;
        ++g_modelAssetsLoaded;
        return true;
    }

    ++g_modelAssetsFailed;
    return false;
}

struct B3DTextureRef
{
    std::string name;
};

struct B3DBrushRef
{
    IDirect3DTexture9* texture;
    std::string textureName;
};

struct B3DTempVertex
{
    FLOAT x, y, z;
    FLOAT nx, ny, nz;
    FLOAT u, v;
};

static DWORD ReadU32LEMem(const std::vector<BYTE>& data, size_t pos)
{
    return ((DWORD)data[pos]) | ((DWORD)data[pos + 1] << 8) | ((DWORD)data[pos + 2] << 16) | ((DWORD)data[pos + 3] << 24);
}

static INT ReadI32LEMem(const std::vector<BYTE>& data, size_t pos)
{
    return (INT)ReadU32LEMem(data, pos);
}

static FLOAT ReadF32LEMem(const std::vector<BYTE>& data, size_t pos)
{
    DWORD bits = ReadU32LEMem(data, pos);
    FLOAT out;
    memcpy(&out, &bits, sizeof(FLOAT));
    return out;
}

static bool ReadB3DString(const std::vector<BYTE>& data, size_t& pos, size_t end, std::string& out)
{
    size_t start = pos;
    while (pos < end && data[pos] != 0)
        ++pos;
    if (pos >= end)
        return false;
    out.assign((const char*)&data[start], pos - start);
    ++pos;
    return true;
}

static bool ReadB3DChunk(const std::vector<BYTE>& data, size_t& pos, size_t end, char id[5], size_t& chunkStart, size_t& chunkEnd)
{
    if (pos + 8 > end)
        return false;
    id[0] = (char)data[pos + 0];
    id[1] = (char)data[pos + 1];
    id[2] = (char)data[pos + 2];
    id[3] = (char)data[pos + 3];
    id[4] = 0;
    DWORD size = ReadU32LEMem(data, pos + 4);
    chunkStart = pos + 8;
    chunkEnd = chunkStart + (size_t)size;
    if (chunkEnd > end || chunkEnd < chunkStart)
        return false;
    pos = chunkStart;
    return true;
}

static void ParseB3DTextures(const std::vector<BYTE>& data, size_t pos, size_t end, std::vector<B3DTextureRef>& textures)
{
    while (pos < end)
    {
        B3DTextureRef tex;
        if (!ReadB3DString(data, pos, end, tex.name))
            return;
        if (pos + 28 > end)
            return;
        pos += 28;
        textures.push_back(tex);
    }
}

static void ParseB3DBrushes(const std::vector<BYTE>& data, size_t pos, size_t end,
    const std::vector<B3DTextureRef>& textures, std::vector<B3DBrushRef>& brushes)
{
    if (pos + 4 > end)
        return;
    INT textureSlots = ReadI32LEMem(data, pos);
    pos += 4;
    if (textureSlots < 0 || textureSlots > 8)
        return;

    while (pos < end)
    {
        std::string name;
        if (!ReadB3DString(data, pos, end, name))
            return;
        if (pos + 4 * 4 + 4 + 4 + 4 + (size_t)textureSlots * 4 > end)
            return;

        pos += 4 * 4; // rgba
        pos += 4;     // shininess
        pos += 4;     // blend
        pos += 4;     // fx

        INT textureIndex = -1;
        for (INT i = 0; i < textureSlots; ++i)
        {
            INT candidate = ReadI32LEMem(data, pos);
            pos += 4;
            if (textureIndex < 0 && candidate >= 0 && candidate < (INT)textures.size())
                textureIndex = candidate;
        }

        B3DBrushRef brush;
        if (textureIndex >= 0)
        {
            brush.textureName = BaseName(textures[(size_t)textureIndex].name);
            brush.texture = LoadTextureCached(brush.textureName);
        }
        else
        {
            brush.textureName = "";
            brush.texture = g_whiteTexture;
        }
        brushes.push_back(brush);
    }
}

static void PrepareB3DModelSurfaces(NativeModel& model, const std::vector<B3DBrushRef>& brushes)
{
    size_t count = brushes.empty() ? 1 : brushes.size();
    model.surfaces.clear();
    model.surfaces.resize(count);
    for (size_t i = 0; i < count; ++i)
    {
        model.surfaces[i].textureName = brushes.empty() ? "" : brushes[i].textureName;
        model.surfaces[i].texture = brushes.empty() ? g_whiteTexture : (brushes[i].texture ? brushes[i].texture : g_whiteTexture);
    }
}

static void AppendB3DVertex(NativeModelSurface& surface, const B3DTempVertex& src, const XMMATRIX& world)
{
    XMVECTOR p = XMVector3TransformCoord(XMVectorSet(src.x, src.y, src.z, 0.0f), world);
    XMVECTOR n = XMVector3Normalize(XMVector3TransformNormal(XMVectorSet(src.nx, src.ny, src.nz, 0.0f), world));
    NativeModelVertex dst;
    dst.x = XMVectorGetX(p);
    dst.y = XMVectorGetY(p);
    dst.z = XMVectorGetZ(p);
    dst.nx = XMVectorGetX(n);
    dst.ny = XMVectorGetY(n);
    dst.nz = XMVectorGetZ(n);
    dst.u = src.u;
    dst.v = src.v;
    surface.verts.push_back(dst);
}

static void ParseB3DNode(NativeModel& model, const std::vector<BYTE>& data, size_t pos, size_t end,
    const XMMATRIX& parent, const std::vector<B3DBrushRef>& brushes);

static void ParseB3DMesh(NativeModel& model, const std::vector<BYTE>& data, size_t pos, size_t end,
    const XMMATRIX& world, const std::vector<B3DBrushRef>& brushes)
{
    if (pos + 4 > end)
        return;
    INT meshBrush = ReadI32LEMem(data, pos);
    pos += 4;
    std::vector<B3DTempVertex> verts;

    while (pos + 8 <= end)
    {
        char id[5];
        size_t chunkStart, chunkEnd;
        if (!ReadB3DChunk(data, pos, end, id, chunkStart, chunkEnd))
            return;

        if (strcmp(id, "VRTS") == 0)
        {
            size_t q = chunkStart;
            if (q + 12 > chunkEnd)
            {
                pos = chunkEnd;
                continue;
            }

            INT flags = ReadI32LEMem(data, q); q += 4;
            INT texCoordSets = ReadI32LEMem(data, q); q += 4;
            INT texCoordSize = ReadI32LEMem(data, q); q += 4;
            if (texCoordSets < 0 || texCoordSets > 8 || texCoordSize < 0 || texCoordSize > 4)
            {
                pos = chunkEnd;
                continue;
            }

            while (q + 12 <= chunkEnd)
            {
                B3DTempVertex v;
                v.x = ReadF32LEMem(data, q); q += 4;
                v.y = ReadF32LEMem(data, q); q += 4;
                v.z = ReadF32LEMem(data, q); q += 4;
                v.nx = 0.0f; v.ny = 1.0f; v.nz = 0.0f;
                v.u = 0.0f; v.v = 0.0f;

                if (flags & 1)
                {
                    if (q + 12 > chunkEnd) break;
                    v.nx = ReadF32LEMem(data, q); q += 4;
                    v.ny = ReadF32LEMem(data, q); q += 4;
                    v.nz = ReadF32LEMem(data, q); q += 4;
                }
                if (flags & 2)
                {
                    if (q + 16 > chunkEnd) break;
                    q += 16;
                }
                for (INT set = 0; set < texCoordSets; ++set)
                {
                    for (INT component = 0; component < texCoordSize; ++component)
                    {
                        if (q + 4 > chunkEnd) break;
                        FLOAT value = ReadF32LEMem(data, q);
                        q += 4;
                        if (set == 0 && component == 0) v.u = value;
                        if (set == 0 && component == 1) v.v = value;
                    }
                }

                verts.push_back(v);
            }
        }
        else if (strcmp(id, "TRIS") == 0)
        {
            size_t q = chunkStart;
            if (q + 4 > chunkEnd || model.surfaces.empty())
            {
                pos = chunkEnd;
                continue;
            }
            INT brush = ReadI32LEMem(data, q);
            q += 4;
            if (brush < 0)
                brush = meshBrush;
            if (brush < 0 || brush >= (INT)model.surfaces.size())
                brush = 0;
            NativeModelSurface& surface = model.surfaces[(size_t)brush];

            while (q + 12 <= chunkEnd)
            {
                INT i0 = ReadI32LEMem(data, q); q += 4;
                INT i1 = ReadI32LEMem(data, q); q += 4;
                INT i2 = ReadI32LEMem(data, q); q += 4;
                if (i0 >= 0 && i1 >= 0 && i2 >= 0 &&
                    i0 < (INT)verts.size() && i1 < (INT)verts.size() && i2 < (INT)verts.size())
                {
                    AppendB3DVertex(surface, verts[(size_t)i0], world);
                    AppendB3DVertex(surface, verts[(size_t)i1], world);
                    AppendB3DVertex(surface, verts[(size_t)i2], world);
                }
            }
        }
        else if (strcmp(id, "NODE") == 0)
        {
            ParseB3DNode(model, data, chunkStart, chunkEnd, world, brushes);
        }

        pos = chunkEnd;
    }
}

static void ParseB3DNode(NativeModel& model, const std::vector<BYTE>& data, size_t pos, size_t end,
    const XMMATRIX& parent, const std::vector<B3DBrushRef>& brushes)
{
    std::string name;
    if (!ReadB3DString(data, pos, end, name))
        return;
    if (pos + 40 > end)
        return;

    FLOAT px = ReadF32LEMem(data, pos); pos += 4;
    FLOAT py = ReadF32LEMem(data, pos); pos += 4;
    FLOAT pz = ReadF32LEMem(data, pos); pos += 4;
    FLOAT sx = ReadF32LEMem(data, pos); pos += 4;
    FLOAT sy = ReadF32LEMem(data, pos); pos += 4;
    FLOAT sz = ReadF32LEMem(data, pos); pos += 4;
    FLOAT qw = ReadF32LEMem(data, pos); pos += 4;
    FLOAT qx = ReadF32LEMem(data, pos); pos += 4;
    FLOAT qy = ReadF32LEMem(data, pos); pos += 4;
    FLOAT qz = ReadF32LEMem(data, pos); pos += 4;

    XMVECTOR quat = XMVector4Normalize(XMVectorSet(qx, qy, qz, qw));
    XMMATRIX local = XMMatrixScaling(sx, sy, sz) *
        XMMatrixRotationQuaternion(quat) *
        XMMatrixTranslation(px, py, pz);
    XMMATRIX world = local * parent;

    while (pos + 8 <= end)
    {
        char id[5];
        size_t chunkStart, chunkEnd;
        if (!ReadB3DChunk(data, pos, end, id, chunkStart, chunkEnd))
            return;

        if (strcmp(id, "MESH") == 0)
            ParseB3DMesh(model, data, chunkStart, chunkEnd, world, brushes);
        else if (strcmp(id, "NODE") == 0)
            ParseB3DNode(model, data, chunkStart, chunkEnd, world, brushes);

        pos = chunkEnd;
    }
}

static bool LoadB3DModel(NativeModel& model, const char* fileName)
{
    model.surfaces.clear();
    model.loaded = false;

    const char* prefixes[] =
    {
        "game:\\GFX\\npcs\\",
        ".\\GFX\\npcs\\",
        "..\\..\\GFX\\npcs\\",
    };

    FILE* f = NULL;
    char path[512];
    for (int i = 0; i < (int)(sizeof(prefixes) / sizeof(prefixes[0])); ++i)
    {
        _snprintf(path, sizeof(path), "%s%s", prefixes[i], fileName);
        path[sizeof(path) - 1] = 0;
        f = fopen(path, "rb");
        if (f)
            break;
    }

    if (!f)
    {
        ++g_modelAssetsFailed;
        return false;
    }

    fseek(f, 0, SEEK_END);
    long fileSize = ftell(f);
    fseek(f, 0, SEEK_SET);
    if (fileSize <= 0 || fileSize > 16 * 1024 * 1024)
    {
        fclose(f);
        ++g_modelAssetsFailed;
        return false;
    }

    std::vector<BYTE> data((size_t)fileSize);
    if (fread(&data[0], 1, (size_t)fileSize, f) != (size_t)fileSize)
    {
        fclose(f);
        ++g_modelAssetsFailed;
        return false;
    }
    fclose(f);

    size_t pos = 0;
    char rootId[5];
    size_t rootStart, rootEnd;
    if (!ReadB3DChunk(data, pos, data.size(), rootId, rootStart, rootEnd) || strcmp(rootId, "BB3D") != 0)
    {
        ++g_modelAssetsFailed;
        return false;
    }

    if (rootStart + 4 > rootEnd)
    {
        ++g_modelAssetsFailed;
        return false;
    }

    pos = rootStart + 4; // version
    std::vector<B3DTextureRef> textures;
    std::vector<B3DBrushRef> brushes;

    while (pos + 8 <= rootEnd)
    {
        char id[5];
        size_t chunkStart, chunkEnd;
        if (!ReadB3DChunk(data, pos, rootEnd, id, chunkStart, chunkEnd))
            break;

        if (strcmp(id, "TEXS") == 0)
        {
            ParseB3DTextures(data, chunkStart, chunkEnd, textures);
        }
        else if (strcmp(id, "BRUS") == 0)
        {
            ParseB3DBrushes(data, chunkStart, chunkEnd, textures, brushes);
            PrepareB3DModelSurfaces(model, brushes);
        }
        else if (strcmp(id, "NODE") == 0)
        {
            if (model.surfaces.empty())
                PrepareB3DModelSurfaces(model, brushes);
            ParseB3DNode(model, data, chunkStart, chunkEnd, XMMatrixIdentity(), brushes);
        }

        pos = chunkEnd;
    }

    bool any = false;
    for (size_t i = 0; i < model.surfaces.size(); ++i)
    {
        if (!model.surfaces[i].verts.empty())
        {
            any = true;
            break;
        }
    }

    if (any)
    {
        model.loaded = true;
        ++g_modelAssetsLoaded;
        return true;
    }

    ++g_modelAssetsFailed;
    return false;
}

static void AppendModel(std::vector<RMeshSurface>& dest, const NativeModel& model,
    FLOAT x, FLOAT y, FLOAT z, FLOAT sx, FLOAT sy, FLOAT sz, FLOAT yawDegrees, DWORD tint)
{
    if (!model.loaded)
        return;

    FLOAT radians = yawDegrees * (XM_PI / 180.0f);
    FLOAT c = cosf(radians);
    FLOAT s = sinf(radians);

    for (size_t surfaceIndex = 0; surfaceIndex < model.surfaces.size(); ++surfaceIndex)
    {
        const NativeModelSurface& source = model.surfaces[surfaceIndex];
        if (source.verts.empty())
            continue;

        RMeshSurface surface;
        surface.texture = source.texture ? source.texture : g_whiteTexture;
        surface.textureName = source.textureName;
        InitSurfaceBounds(surface);
        surface.verts.reserve(source.verts.size());

        for (size_t i = 0; i < source.verts.size(); ++i)
        {
            const NativeModelVertex& mv = source.verts[i];
            FLOAT lx = mv.x * sx;
            FLOAT ly = mv.y * sy;
            FLOAT lz = mv.z * sz;
            FLOAT nx = mv.nx * c + mv.nz * s;
            FLOAT nz = -mv.nx * s + mv.nz * c;
            Vertex v;
            v.x = lx * c + lz * s + x;
            v.y = ly + y;
            v.z = -lx * s + lz * c + z;
            v.color = LitModelColor(tint, nx, mv.ny, nz);
            v.u = mv.u;
            v.v = mv.v;
            surface.verts.push_back(v);
            ExpandSurfaceBounds(surface, v);
        }

        dest.push_back(surface);
    }
}

static XMVECTOR VertexPos(const Vertex& v)
{
    return XMVectorSet(v.x, v.y, v.z, 0.0f);
}

static XMVECTOR ClosestPointOnTriangle(XMVECTOR p, XMVECTOR a, XMVECTOR b, XMVECTOR c)
{
    XMVECTOR ab = b - a;
    XMVECTOR ac = c - a;
    XMVECTOR ap = p - a;

    FLOAT d1 = XMVectorGetX(XMVector3Dot(ab, ap));
    FLOAT d2 = XMVectorGetX(XMVector3Dot(ac, ap));
    if (d1 <= 0.0f && d2 <= 0.0f) return a;

    XMVECTOR bp = p - b;
    FLOAT d3 = XMVectorGetX(XMVector3Dot(ab, bp));
    FLOAT d4 = XMVectorGetX(XMVector3Dot(ac, bp));
    if (d3 >= 0.0f && d4 <= d3) return b;

    FLOAT vc = d1 * d4 - d3 * d2;
    if (vc <= 0.0f && d1 >= 0.0f && d3 <= 0.0f)
    {
        FLOAT v = d1 / (d1 - d3);
        return a + ab * v;
    }

    XMVECTOR cp = p - c;
    FLOAT d5 = XMVectorGetX(XMVector3Dot(ab, cp));
    FLOAT d6 = XMVectorGetX(XMVector3Dot(ac, cp));
    if (d6 >= 0.0f && d5 <= d6) return c;

    FLOAT vb = d5 * d2 - d1 * d6;
    if (vb <= 0.0f && d2 >= 0.0f && d6 <= 0.0f)
    {
        FLOAT w = d2 / (d2 - d6);
        return a + ac * w;
    }

    FLOAT va = d3 * d6 - d5 * d4;
    if (va <= 0.0f && (d4 - d3) >= 0.0f && (d5 - d6) >= 0.0f)
    {
        FLOAT w = (d4 - d3) / ((d4 - d3) + (d5 - d6));
        return b + (c - b) * w;
    }

    FLOAT denom = 1.0f / (va + vb + vc);
    FLOAT v = vb * denom;
    FLOAT w = vc * denom;
    return a + ab * v + ac * w;
}

static bool ResolvePlayerCollision()
{
    const FLOAT radius = 0.38f;
    const FLOAT centerY = 1.05f;
    bool collided = false;

    for (int pass = 0; pass < 3; ++pass)
    {
        FLOAT playerX = XMVectorGetX(g_playerPos);
        FLOAT playerZ = XMVectorGetZ(g_playerPos);
        XMVECTOR center = XMVectorSet(playerX, centerY, playerZ, 0.0f);

        FLOAT bestPenetration = 0.0f;
        XMVECTOR bestPush = XMVectorZero();

        for (size_t batchIndex = 0; batchIndex < g_collisionBatches.size(); ++batchIndex)
        {
            const CollisionBatch& batch = g_collisionBatches[batchIndex];
            if (playerX < batch.minX - radius || playerX > batch.maxX + radius ||
                playerZ < batch.minZ - radius || playerZ > batch.maxZ + radius)
                continue;
            if (centerY + 0.9f < batch.minY || centerY - 0.9f > batch.maxY)
                continue;

            for (size_t i = 0; i + 2 < batch.verts.size(); i += 3)
            {
                const Vertex& va = batch.verts[i + 0];
                const Vertex& vb = batch.verts[i + 1];
                const Vertex& vc = batch.verts[i + 2];

                FLOAT minX = min(va.x, min(vb.x, vc.x)) - radius;
                FLOAT maxX = max(va.x, max(vb.x, vc.x)) + radius;
                FLOAT minZ = min(va.z, min(vb.z, vc.z)) - radius;
                FLOAT maxZ = max(va.z, max(vb.z, vc.z)) + radius;
                FLOAT minY = min(va.y, min(vb.y, vc.y));
                FLOAT maxY = max(va.y, max(vb.y, vc.y));

                if (playerX < minX || playerX > maxX || playerZ < minZ || playerZ > maxZ)
                    continue;
                if (centerY + 0.9f < minY || centerY - 0.9f > maxY)
                    continue;

                XMVECTOR a = VertexPos(va);
                XMVECTOR b = VertexPos(vb);
                XMVECTOR c = VertexPos(vc);
                XMVECTOR normal = XMVector3Normalize(XMVector3Cross(b - a, c - a));
                FLOAT normalY = fabsf(XMVectorGetY(normal));

                if (normalY > 0.72f)
                    continue;

                XMVECTOR closest = ClosestPointOnTriangle(center, a, b, c);
                XMVECTOR delta = center - closest;
                delta = XMVectorSetY(delta, 0.0f);
                FLOAT distSq = XMVectorGetX(XMVector3LengthSq(delta));

                if (distSq >= radius * radius)
                    continue;

                FLOAT dist = sqrtf(max(distSq, 0.000001f));
                FLOAT penetration = radius - dist;
                XMVECTOR direction = delta / dist;

                if (distSq < 0.0001f)
                {
                    FLOAT side = XMVectorGetX(XMVector3Dot(center - a, normal));
                    direction = side >= 0.0f ? normal : -normal;
                    direction = XMVectorSetY(direction, 0.0f);
                    direction = XMVector3Normalize(direction);
                }

                if (penetration > bestPenetration)
                {
                    bestPenetration = penetration;
                    bestPush = direction * (penetration + 0.003f);
                }
            }
        }

        if (bestPenetration <= 0.0f)
            break;

        g_playerPos += bestPush;
        collided = true;
    }

    if (collided)
    {
        g_collisionFlash = 0.25f;
        ++g_collisionCount;
    }

    return collided;
}

static void BuildFallbackRoom()
{
    ClearFacilityGeometry();
    DWORD floorColor = D3DCOLOR_XRGB(90, 92, 88);
    DWORD wallColor = D3DCOLOR_XRGB(145, 142, 130);
    DWORD railColor = D3DCOLOR_XRGB(70, 70, 76);

    AddQuad(g_roomVerts, XMVectorSet(-10, 0, -12, 0), XMVectorSet(10, 0, -12, 0),
        XMVectorSet(10, 0, 16, 0), XMVectorSet(-10, 0, 16, 0), floorColor);
    AddQuad(g_roomVerts, XMVectorSet(-10, 5, -12, 0), XMVectorSet(-10, 0, -12, 0),
        XMVectorSet(-10, 0, 16, 0), XMVectorSet(-10, 5, 16, 0), wallColor);
    AddQuad(g_roomVerts, XMVectorSet(10, 0, -12, 0), XMVectorSet(10, 5, -12, 0),
        XMVectorSet(10, 5, 16, 0), XMVectorSet(10, 0, 16, 0), wallColor);
    AddQuad(g_roomVerts, XMVectorSet(-10, 5, 16, 0), XMVectorSet(10, 5, 16, 0),
        XMVectorSet(10, 0, 16, 0), XMVectorSet(-10, 0, 16, 0), wallColor);
    AddQuad(g_roomVerts, XMVectorSet(-10, 5, -12, 0), XMVectorSet(10, 5, -12, 0),
        XMVectorSet(10, 5, 16, 0), XMVectorSet(-10, 5, 16, 0), D3DCOLOR_XRGB(70, 70, 72));

    for (int i = 0; i < 12; ++i)
    {
        FLOAT z = -10.0f + i * 2.0f;
        AddQuad(g_roomVerts, XMVectorSet(-6.0f, 2.0f, z, 0), XMVectorSet(6.0f, 2.0f, z, 0),
            XMVectorSet(6.0f, 2.15f, z + 0.15f, 0), XMVectorSet(-6.0f, 2.15f, z + 0.15f, 0), railColor);
    }

    RMeshSurface surface;
    surface.verts = g_roomVerts;
    surface.texture = g_whiteTexture;
    surface.textureName = "fallback_white";
    InitSurfaceBounds(surface);
    for (size_t i = 0; i < surface.verts.size(); ++i)
        ExpandSurfaceBounds(surface, surface.verts[i]);
    g_roomSurfaces.push_back(surface);
    AddCollisionBatch(g_roomVerts);

    SetStatus("Fallback room active: %u tris", (unsigned)(g_roomVerts.size() / 3));
}

static bool AppendRMeshVisible(const char* path, FLOAT roomX, FLOAT roomY, FLOAT roomZ, FLOAT yawDegrees)
{
    FILE* f = fopen(path, "rb");
    if (!f)
        return false;

    std::string header;
    if (!ReadStringLE(f, header) || header.find("RoomMesh") != 0)
    {
        fclose(f);
        return false;
    }

    INT surfaceCount = 0;
    if (!ReadInt32LE(f, &surfaceCount) || surfaceCount <= 0 || surfaceCount > 4096)
    {
        fclose(f);
        return false;
    }

    std::vector<Vertex> collisionVerts;
    for (INT s = 0; s < surfaceCount; ++s)
    {
        std::string textureNames[2];
        for (int t = 0; t < 2; ++t)
        {
            BYTE hasTexture = 0;
            if (!ReadExact(f, &hasTexture, 1))
            {
                fclose(f);
                return false;
            }
            if (hasTexture)
            {
                if (!ReadStringLE(f, textureNames[t]))
                {
                    fclose(f);
                    return false;
                }
            }
        }

        INT vertexCount = 0;
        if (!ReadInt32LE(f, &vertexCount) || vertexCount < 0 || vertexCount > 100000)
        {
            fclose(f);
            return false;
        }

        std::vector<Vertex> surfaceVerts;
        surfaceVerts.reserve((size_t)vertexCount);
        for (INT i = 0; i < vertexCount; ++i)
        {
            FLOAT x, y, z, u0, v0, u1, v1;
            BYTE r, g, b;
            if (!ReadFloatLE(f, &x) || !ReadFloatLE(f, &y) || !ReadFloatLE(f, &z) ||
                !ReadFloatLE(f, &u0) || !ReadFloatLE(f, &v0) || !ReadFloatLE(f, &u1) || !ReadFloatLE(f, &v1) ||
                !ReadExact(f, &r, 1) || !ReadExact(f, &g, 1) || !ReadExact(f, &b, 1))
            {
                fclose(f);
                return false;
            }

            r = (BYTE)Clamp((FLOAT)r * 1.35f + 16.0f, 0.0f, 255.0f);
            g = (BYTE)Clamp((FLOAT)g * 1.35f + 16.0f, 0.0f, 255.0f);
            b = (BYTE)Clamp((FLOAT)b * 1.35f + 16.0f, 0.0f, 255.0f);

            Vertex v = { x * 0.01f, y * 0.01f, z * 0.01f, D3DCOLOR_XRGB(r, g, b), u0, v0 };
            surfaceVerts.push_back(TransformVertex(v, roomX, roomY, roomZ, yawDegrees));
        }

        INT triangleCount = 0;
        if (!ReadInt32LE(f, &triangleCount) || triangleCount < 0 || triangleCount > 100000)
        {
            fclose(f);
            return false;
        }

        RMeshSurface drawSurface;
        drawSurface.textureName = textureNames[1].empty() ? textureNames[0] : textureNames[1];
        drawSurface.texture = LoadTextureCached(drawSurface.textureName);
        drawSurface.verts.reserve((size_t)triangleCount * 3);
        InitSurfaceBounds(drawSurface);
        for (INT i = 0; i < triangleCount; ++i)
        {
            INT i0, i1, i2;
            if (!ReadInt32LE(f, &i0) || !ReadInt32LE(f, &i1) || !ReadInt32LE(f, &i2))
            {
                fclose(f);
                return false;
            }
            if (i0 >= 0 && i1 >= 0 && i2 >= 0 &&
                i0 < vertexCount && i1 < vertexCount && i2 < vertexCount)
            {
                const Vertex& a = surfaceVerts[(size_t)i0];
                const Vertex& b = surfaceVerts[(size_t)i1];
                const Vertex& c = surfaceVerts[(size_t)i2];
                AddTri(g_roomVerts, a, b, c);
                AddTri(collisionVerts, a, b, c);
                AddTri(drawSurface.verts, a, b, c);
                ExpandSurfaceBounds(drawSurface, a);
                ExpandSurfaceBounds(drawSurface, b);
                ExpandSurfaceBounds(drawSurface, c);
            }
        }

        if (!drawSurface.verts.empty())
            g_roomSurfaces.push_back(drawSurface);
    }

    fclose(f);
    AddCollisionBatch(collisionVerts);
    return !collisionVerts.empty();
}

static bool LoadRMeshVisible(const char* path)
{
    ClearFacilityGeometry();
    return AppendRMeshVisible(path, 0.0f, 0.0f, 0.0f, 0.0f);
}

static const INT MAP_WIDTH = 20;
static const INT MAP_HEIGHT = 20;
static const INT ZONE_AMOUNT = 3;
static const INT MAX_MAP_ROOMS = 96;
static const INT ROOM1 = 1;
static const INT ROOM2 = 2;
static const INT ROOM2C = 3;
static const INT ROOM3 = 4;
static const INT ROOM4 = 5;
static const FLOAT NATIVE_ROOM_SCALE = 8.0f / 2048.0f;

struct NativeMapState
{
    INT temp[MAP_WIDTH + 2][MAP_HEIGHT + 2];
    INT shape[MAP_WIDTH + 2][MAP_HEIGHT + 2];
    FLOAT yaw[MAP_WIDTH + 2][MAP_HEIGHT + 2];
    char name[MAP_WIDTH + 2][MAP_HEIGHT + 2][32];
    char roomSlots[ROOM4 + 1][MAX_MAP_ROOMS][32];
    INT roomId[ROOM4 + 1];
    INT roomAmount[ROOM4 + 1][ZONE_AMOUNT];
    FLOAT startX;
    FLOAT startZ;
    bool foundStart;
};

static NativeMapState g_map;
static DWORD g_rngState = 1;

static INT ClampInt(INT v, INT lo, INT hi)
{
    return v < lo ? lo : (v > hi ? hi : v);
}

static void SeedNativeMap(DWORD seed)
{
    g_rngState = seed ? seed : 1;
}

static INT RandInt(INT lo, INT hi)
{
    if (hi < lo)
        return lo;
    g_rngState = g_rngState * 1664525u + 1013904223u;
    return lo + (INT)((g_rngState >> 16) % (DWORD)(hi - lo + 1));
}

static INT GetZoneIndex(INT y)
{
    INT zone = (INT)floorf(((FLOAT)(MAP_WIDTH - y) / (FLOAT)MAP_WIDTH) * (FLOAT)ZONE_AMOUNT);
    return ClampInt(zone, 0, ZONE_AMOUNT - 1);
}

static INT MinCell(INT v)
{
    return v > 0 ? 1 : 0;
}

static void CopyRoomName(char* dst, const char* src)
{
    if (!src)
        src = "";
    _snprintf(dst, 32, "%s", src);
    dst[31] = 0;
}

static void SetRoomSlot(INT shape, INT pos, const char* name)
{
    if (shape < ROOM1 || shape > ROOM4 || pos < 0 || pos >= MAX_MAP_ROOMS)
        return;
    CopyRoomName(g_map.roomSlots[shape][pos], name);
}

static bool SetRoomLikeBlitz(const char* roomName, INT shape, INT pos, INT minPos, INT maxPos)
{
    if (shape < ROOM1 || shape > ROOM4)
        return false;
    if (maxPos < minPos)
        return false;

    minPos = ClampInt(minPos, 0, MAX_MAP_ROOMS - 1);
    maxPos = ClampInt(maxPos, 0, MAX_MAP_ROOMS - 1);
    pos = ClampInt(pos, minPos, maxPos);

    bool looped = false;
    while (g_map.roomSlots[shape][pos][0] != 0)
    {
        ++pos;
        if (pos > maxPos)
        {
            if (!looped)
            {
                pos = minPos;
                looped = true;
            }
            else
            {
                return false;
            }
        }
    }

    SetRoomSlot(shape, pos, roomName);
    return true;
}

static const char* PickFacilityRoom(INT zone, INT shape, INT ordinal);

static const char* ResolveRoomMeshName(const char* roomName, INT zoneIndex, INT shape, INT ordinal)
{
    if (!roomName || roomName[0] == 0)
        return PickFacilityRoom(zoneIndex + 1, shape, ordinal);

    struct Alias
    {
        const char* room;
        const char* file;
    };

    static const Alias aliases[] =
    {
        { "start", "173_opt.rmesh" },
        { "173", "173bright_opt.rmesh" },
        { "914", "machineroom_opt.rmesh" },
        { "checkpoint1", "checkpoint1_opt.rmesh" },
        { "checkpoint2", "checkpoint2_opt.rmesh" },
        { "room4tunnels", "4tunnels_opt.rmesh" },
        { "gatea", "gatea_opt.rmesh" },
        { "gateaentrance", "gateaentrance_opt.rmesh" },
        { "pocketdimension", "pocketdimension1_opt.rmesh" },
        { "dimension1499", "dimension1499_opt.rmesh" },
    };

    for (int i = 0; i < (int)(sizeof(aliases) / sizeof(aliases[0])); ++i)
    {
        if (_stricmp(roomName, aliases[i].room) == 0)
            return aliases[i].file;
    }

    static char fileName[64];
    _snprintf(fileName, sizeof(fileName), "%s_opt.rmesh", roomName);
    fileName[sizeof(fileName) - 1] = 0;
    return fileName;
}

static const char* PickFrom(const char* const* items, INT count, INT ordinal)
{
    if (count <= 0)
        return "room2_opt.rmesh";
    return items[ordinal % count];
}

static const char* PickFacilityRoom(INT zone, INT shape, INT ordinal)
{
    static const char* z1Room1[] = { "endroom_opt.rmesh", "room1archive_opt.rmesh" };
    static const char* z1Room2[] = { "room2_opt.rmesh", "room2_2_opt.rmesh", "room2_4_opt.rmesh", "room2_5_opt.rmesh", "room2tesla_lcz_opt.rmesh" };
    static const char* z1Room2C[] = { "room2C_opt.rmesh", "room2c2_opt.rmesh", "lockroom_opt.rmesh" };
    static const char* z1Room3[] = { "room3_opt.rmesh", "room3_2_opt.rmesh", "room3_3_opt.rmesh" };
    static const char* z1Room4[] = { "room4_opt.rmesh", "room4_2_opt.rmesh", "room4info_opt.rmesh" };

    static const char* z2Room1[] = { "endroom2_opt.rmesh", "room513_opt.rmesh", "coffin_opt.rmesh" };
    static const char* z2Room2[] = { "room2tunnel_opt.rmesh", "room2pipes_opt.rmesh", "room2pit_opt.rmesh", "room2tesla_hcz_opt.rmesh", "room2servers_opt.rmesh", "room2nuke_opt.rmesh" };
    static const char* z2Room2C[] = { "room2Ctunnel_opt.rmesh", "room2cpit_opt.rmesh" };
    static const char* z2Room3[] = { "room3tunnel_opt.rmesh", "room3pit_opt.rmesh", "room3z2_opt.rmesh" };
    static const char* z2Room4[] = { "4tunnels_opt.rmesh", "room4pit_opt.rmesh" };

    static const char* z3Room1[] = { "room1lifts_opt.rmesh", "medibay_opt.rmesh", "gateaentrance_opt.rmesh" };
    static const char* z3Room2[] = { "room2z3_opt.rmesh", "room2offices_opt.rmesh", "room2cafeteria_opt.rmesh", "room2toilets_opt.rmesh", "room2tesla_opt.rmesh", "room2servers2_opt.rmesh" };
    static const char* z3Room2C[] = { "room2Cz3_opt.rmesh", "room2ccont_opt.rmesh", "lockroom2_opt.rmesh" };
    static const char* z3Room3[] = { "room3servers_opt.rmesh", "room3servers2_opt.rmesh", "room3offices_opt.rmesh", "room3z3_opt.rmesh" };
    static const char* z3Room4[] = { "room4z3_opt.rmesh" };

    const char* const* list = z1Room2;
    INT count = (INT)(sizeof(z1Room2) / sizeof(z1Room2[0]));

    if (zone == 1)
    {
        if (shape == 1) { list = z1Room1; count = (INT)(sizeof(z1Room1) / sizeof(z1Room1[0])); }
        else if (shape == 2) { list = z1Room2; count = (INT)(sizeof(z1Room2) / sizeof(z1Room2[0])); }
        else if (shape == 3) { list = z1Room2C; count = (INT)(sizeof(z1Room2C) / sizeof(z1Room2C[0])); }
        else if (shape == 4) { list = z1Room3; count = (INT)(sizeof(z1Room3) / sizeof(z1Room3[0])); }
        else { list = z1Room4; count = (INT)(sizeof(z1Room4) / sizeof(z1Room4[0])); }
    }
    else if (zone == 2)
    {
        if (shape == 1) { list = z2Room1; count = (INT)(sizeof(z2Room1) / sizeof(z2Room1[0])); }
        else if (shape == 2) { list = z2Room2; count = (INT)(sizeof(z2Room2) / sizeof(z2Room2[0])); }
        else if (shape == 3) { list = z2Room2C; count = (INT)(sizeof(z2Room2C) / sizeof(z2Room2C[0])); }
        else if (shape == 4) { list = z2Room3; count = (INT)(sizeof(z2Room3) / sizeof(z2Room3[0])); }
        else { list = z2Room4; count = (INT)(sizeof(z2Room4) / sizeof(z2Room4[0])); }
    }
    else
    {
        if (shape == 1) { list = z3Room1; count = (INT)(sizeof(z3Room1) / sizeof(z3Room1[0])); }
        else if (shape == 2) { list = z3Room2; count = (INT)(sizeof(z3Room2) / sizeof(z3Room2[0])); }
        else if (shape == 3) { list = z3Room2C; count = (INT)(sizeof(z3Room2C) / sizeof(z3Room2C[0])); }
        else if (shape == 4) { list = z3Room3; count = (INT)(sizeof(z3Room3) / sizeof(z3Room3[0])); }
        else { list = z3Room4; count = (INT)(sizeof(z3Room4) / sizeof(z3Room4[0])); }
    }

    return PickFrom(list, count, ordinal);
}

static bool TryAppendRoomFileAt(const char* fileName, FLOAT x, FLOAT y, FLOAT z, FLOAT yawDegrees)
{
    const char* prefixes[] =
    {
        "game:\\GFX\\map\\",
        ".\\GFX\\map\\",
        "..\\..\\GFX\\map\\",
    };

    char path[512];
    for (int i = 0; i < (int)(sizeof(prefixes) / sizeof(prefixes[0])); ++i)
    {
        _snprintf(path, sizeof(path), "%s%s", prefixes[i], fileName);
        path[sizeof(path) - 1] = 0;
        if (AppendRMeshVisible(path, x, y, z, yawDegrees))
        {
            ++g_facilityRoomCount;
            return true;
        }
    }

    ++g_facilityLoadFailures;
    return false;
}

static bool TryAppendRoomFile(const char* fileName, FLOAT x, FLOAT z, FLOAT yawDegrees)
{
    return TryAppendRoomFileAt(fileName, x, 0.0f, z, yawDegrees);
}

static FLOAT WrapDegrees(FLOAT angle)
{
    while (angle < 0.0f) angle += 360.0f;
    while (angle >= 360.0f) angle -= 360.0f;
    return angle;
}

static bool AngleIs(FLOAT angle, FLOAT target)
{
    return fabsf(WrapDegrees(angle) - target) < 0.5f;
}

static XMVECTOR DoorLocalToWorld(const NativeDoor& door, FLOAT lx, FLOAT ly, FLOAT lz)
{
    FLOAT radians = door.angle * (XM_PI / 180.0f);
    FLOAT c = cosf(radians);
    FLOAT s = sinf(radians);
    return XMVectorSet(door.x + lx * c + lz * s, door.y + ly, door.z - lx * s + lz * c, 0.0f);
}

static void AddDoorBox(const NativeDoor& door, FLOAT cx, FLOAT cy, FLOAT cz, FLOAT hx, FLOAT hy, FLOAT hz, DWORD color)
{
    XMVECTOR p000 = DoorLocalToWorld(door, cx - hx, cy - hy, cz - hz);
    XMVECTOR p001 = DoorLocalToWorld(door, cx - hx, cy - hy, cz + hz);
    XMVECTOR p010 = DoorLocalToWorld(door, cx - hx, cy + hy, cz - hz);
    XMVECTOR p011 = DoorLocalToWorld(door, cx - hx, cy + hy, cz + hz);
    XMVECTOR p100 = DoorLocalToWorld(door, cx + hx, cy - hy, cz - hz);
    XMVECTOR p101 = DoorLocalToWorld(door, cx + hx, cy - hy, cz + hz);
    XMVECTOR p110 = DoorLocalToWorld(door, cx + hx, cy + hy, cz - hz);
    XMVECTOR p111 = DoorLocalToWorld(door, cx + hx, cy + hy, cz + hz);

    AddQuad(g_doorVerts, p000, p100, p110, p010, color);
    AddQuad(g_doorVerts, p101, p001, p011, p111, color);
    AddQuad(g_doorVerts, p001, p000, p010, p011, color);
    AddQuad(g_doorVerts, p100, p101, p111, p110, color);
    AddQuad(g_doorVerts, p010, p110, p111, p011, color);
    AddQuad(g_doorVerts, p001, p101, p100, p000, color);
}

static FLOAT DoorSlideProgress(const NativeDoor& door)
{
    FLOAT radians = Clamp(door.openState, 0.0f, 180.0f) * (XM_PI / 180.0f);
    return (1.0f - cosf(radians)) * 0.5f;
}

static FLOAT DoorOpenSpeed(const NativeDoor& door)
{
    FLOAT fpsFactor = 1.0f;
    switch (door.dir)
    {
    case 1: return 0.8f * fpsFactor;
    case 4: return 1.4f * fpsFactor;
    default: return 2.0f * (door.fastOpen + 1) * fpsFactor;
    }
}

static void DoorDimensions(const NativeDoor& door, FLOAT* panelHalfWidth, FLOAT* height, FLOAT* thickness, FLOAT* slide)
{
    if (door.dir == 1)
    {
        *panelHalfWidth = 1.18f;
        *height = 3.05f;
        *thickness = 0.12f;
        *slide = 0.95f;
    }
    else if (door.dir == 2)
    {
        *panelHalfWidth = 0.78f;
        *height = 2.55f;
        *thickness = 0.11f;
        *slide = 0.78f;
    }
    else if (door.dir == 3)
    {
        *panelHalfWidth = 0.56f;
        *height = 2.30f;
        *thickness = 0.08f;
        *slide = 0.44f;
    }
    else
    {
        *panelHalfWidth = 0.56f;
        *height = 2.32f;
        *thickness = 0.08f;
        *slide = 0.72f;
    }
}

static void AppendModelLocal(std::vector<RMeshSurface>& dest, const NativeModel& model, const NativeDoor& door,
    FLOAT lx, FLOAT ly, FLOAT lz, FLOAT sx, FLOAT sy, FLOAT sz, FLOAT yawOffset, DWORD tint)
{
    XMVECTOR pos = DoorLocalToWorld(door, lx, ly, lz);
    AppendModel(dest, model, XMVectorGetX(pos), XMVectorGetY(pos), XMVectorGetZ(pos),
        sx, sy, sz, door.angle + yawOffset, tint);
}

static bool AppendDoorModels(std::vector<RMeshSurface>& dest, const NativeDoor& door, bool includeButtons)
{
    if (!g_modelDoorFrame.loaded)
        return false;

    FLOAT panelHalfWidth, height, thickness, slide;
    DoorDimensions(door, &panelHalfWidth, &height, &thickness, &slide);
    FLOAT progress = DoorSlideProgress(door);
    FLOAT base = panelHalfWidth * 0.94f;
    FLOAT offset = slide * progress;
    DWORD neutralTint = D3DCOLOR_XRGB(215, 215, 210);
    DWORD heavyTint = D3DCOLOR_XRGB(190, 195, 200);
    DWORD lockTint = door.locked || door.keycard || door.code[0] ? D3DCOLOR_XRGB(255, 155, 130) : D3DCOLOR_XRGB(210, 230, 230);

    AppendModelLocal(dest, g_modelDoorFrame, door, 0.0f, 0.0f, 0.0f,
        NATIVE_ROOM_SCALE, NATIVE_ROOM_SCALE, NATIVE_ROOM_SCALE, 0.0f, neutralTint);

    if (door.dir == 2 && g_modelHeavyDoor1.loaded && g_modelHeavyDoor2.loaded)
    {
        AppendModelLocal(dest, g_modelHeavyDoor1, door, -offset, 0.0f, 0.0f,
            NATIVE_ROOM_SCALE, NATIVE_ROOM_SCALE, NATIVE_ROOM_SCALE, 0.0f, heavyTint);
        AppendModelLocal(dest, g_modelHeavyDoor2, door, offset, 0.0f, 0.0f,
            NATIVE_ROOM_SCALE, NATIVE_ROOM_SCALE, NATIVE_ROOM_SCALE, 0.0f, heavyTint);
    }
    else if (g_modelDoorPanel.loaded)
    {
        const FLOAT panelScale = 0.095f;
        AppendModelLocal(dest, g_modelDoorPanel, door, -base - offset, 0.0f, 0.0f,
            panelScale, panelScale, panelScale, 0.0f, neutralTint);
        AppendModelLocal(dest, g_modelDoorPanel, door, base + offset, 0.0f, 0.0f,
            panelScale, panelScale, panelScale, 180.0f, neutralTint);
    }
    else
    {
        return false;
    }

    if (includeButtons)
    {
        const NativeModel* buttonModel = &g_modelButton;
        if (door.code[0] && g_modelButtonCode.loaded)
            buttonModel = &g_modelButtonCode;
        else if (door.keycard > 0 && g_modelButtonKeycard.loaded)
            buttonModel = &g_modelButtonKeycard;
        else if (door.keycard < 0 && g_modelButtonScanner.loaded)
            buttonModel = &g_modelButtonScanner;

        if (buttonModel->loaded)
        {
            AppendModelLocal(dest, *buttonModel, door, 0.60f, 0.70f, -0.10f,
                0.030f, 0.030f, 0.030f, 0.0f, lockTint);
            AppendModelLocal(dest, *buttonModel, door, -0.60f, 0.70f, 0.10f,
                0.030f, 0.030f, 0.030f, 180.0f, lockTint);
        }
    }

    return true;
}

static void BuildDoorVisuals()
{
    g_doorVerts.clear();
    g_doorSurfaces.clear();
    DWORD frameColor = D3DCOLOR_XRGB(52, 55, 60);
    DWORD panelColor = D3DCOLOR_XRGB(92, 96, 102);
    DWORD heavyColor = D3DCOLOR_XRGB(68, 70, 74);
    DWORD buttonColor = D3DCOLOR_XRGB(115, 125, 130);
    DWORD lockColor = D3DCOLOR_XRGB(150, 70, 55);

    for (size_t i = 0; i < g_doors.size(); ++i)
    {
        const NativeDoor& door = g_doors[i];
        FLOAT playerX = XMVectorGetX(g_playerPos);
        FLOAT playerZ = XMVectorGetZ(g_playerPos);
        if (fabsf(playerX - door.x) > g_renderDistance + 8.0f || fabsf(playerZ - door.z) > g_renderDistance + 8.0f)
            continue;

        if (AppendDoorModels(g_doorSurfaces, door, true))
            continue;

        FLOAT panelHalfWidth, height, thickness, slide;
        DoorDimensions(door, &panelHalfWidth, &height, &thickness, &slide);
        FLOAT progress = DoorSlideProgress(door);
        FLOAT base = panelHalfWidth * 0.94f;
        FLOAT offset = slide * progress;
        FLOAT panelY = height * 0.5f;
        DWORD color = door.dir == 2 ? heavyColor : panelColor;

        AddDoorBox(door, 0.0f, height + 0.10f, 0.0f, panelHalfWidth * 2.35f, 0.10f, thickness * 1.6f, frameColor);
        AddDoorBox(door, -(panelHalfWidth * 2.35f), panelY, 0.0f, 0.10f, panelY, thickness * 1.6f, frameColor);
        AddDoorBox(door, panelHalfWidth * 2.35f, panelY, 0.0f, 0.10f, panelY, thickness * 1.6f, frameColor);

        AddDoorBox(door, -base - offset, panelY, 0.0f, panelHalfWidth, panelY, thickness, color);
        AddDoorBox(door, base + offset, panelY, 0.0f, panelHalfWidth, panelY, thickness, color);

        AddDoorBox(door, 0.62f, 0.82f, -0.18f, 0.08f, 0.14f, 0.035f, door.locked || door.keycard || door.code[0] ? lockColor : buttonColor);
        AddDoorBox(door, -0.62f, 0.82f, 0.18f, 0.08f, 0.14f, 0.035f, door.locked || door.keycard || door.code[0] ? lockColor : buttonColor);
    }
}

static void AddNativeDoor(FLOAT x, FLOAT y, FLOAT z, FLOAT angle, INT dir, bool open, bool locked, INT keycard = 0, const char* code = "")
{
    NativeDoor door;
    door.x = x;
    door.y = y;
    door.z = z;
    door.angle = WrapDegrees(angle);
    door.dir = dir;
    door.keycard = keycard;
    door.fastOpen = 0;
    door.openState = open ? 180.0f : 0.0f;
    door.timer = 0.0f;
    door.timerState = 0.0f;
    door.locked = locked;
    door.open = open;
    door.autoClose = false;
    _snprintf(door.code, sizeof(door.code), "%s", code ? code : "");
    door.code[sizeof(door.code) - 1] = 0;
    g_doors.push_back(door);
}

static bool TryToggleNearestDoor()
{
    FLOAT playerX = XMVectorGetX(g_playerPos);
    FLOAT playerZ = XMVectorGetZ(g_playerPos);
    FLOAT bestDistSq = 1.35f * 1.35f;
    INT best = -1;

    for (size_t i = 0; i < g_doors.size(); ++i)
    {
        const NativeDoor& door = g_doors[i];
        if (door.openState > 0.1f && door.openState < 179.9f)
            continue;

        XMVECTOR buttons[2] =
        {
            DoorLocalToWorld(door, 0.62f, 0.82f, -0.18f),
            DoorLocalToWorld(door, -0.62f, 0.82f, 0.18f)
        };

        for (int b = 0; b < 2; ++b)
        {
            FLOAT dx = playerX - XMVectorGetX(buttons[b]);
            FLOAT dz = playerZ - XMVectorGetZ(buttons[b]);
            FLOAT distSq = dx * dx + dz * dz;
            if (distSq < bestDistSq)
            {
                bestDistSq = distSq;
                best = (INT)i;
            }
        }
    }

    if (best < 0)
        return false;

    NativeDoor& door = g_doors[(size_t)best];
    if (door.locked)
    {
        PlayBeep();
        SetStatus(door.open ? "DOOR LOCKED: BUTTON DID NOTHING" : "DOOR LOCKED");
        return true;
    }
    if (door.keycard > 0)
    {
        PlayBeep();
        SetStatus("KEYCARD %d REQUIRED", door.keycard);
        return true;
    }
    if (door.code[0] != 0)
    {
        PlayBeep();
        SetStatus("CODE REQUIRED: %s", door.code);
        return true;
    }

    door.open = !door.open;
    if (door.open && door.timer > 0.0f)
        door.timerState = door.timer;
    ++g_doorToggleCount;
    PlayBeep();
    SetStatus("DOOR %d: %s", best, door.open ? "OPENING" : "CLOSING");
    return true;
}

static bool ResolveDoorPanelCollision(const NativeDoor& door, FLOAT panelCenter, FLOAT panelHalfWidth, FLOAT panelHalfHeight, FLOAT thickness, FLOAT* playerX, FLOAT* playerZ)
{
    const FLOAT radius = 0.38f;
    FLOAT radians = door.angle * (XM_PI / 180.0f);
    FLOAT c = cosf(radians);
    FLOAT s = sinf(radians);
    FLOAT dx = *playerX - door.x;
    FLOAT dz = *playerZ - door.z;
    FLOAT localX = dx * c - dz * s;
    FLOAT localZ = dx * s + dz * c;
    FLOAT minX = panelCenter - panelHalfWidth;
    FLOAT maxX = panelCenter + panelHalfWidth;
    FLOAT minZ = -thickness;
    FLOAT maxZ = thickness;
    FLOAT closestX = Clamp(localX, minX, maxX);
    FLOAT closestZ = Clamp(localZ, minZ, maxZ);
    FLOAT pushX = localX - closestX;
    FLOAT pushZ = localZ - closestZ;
    FLOAT distSq = pushX * pushX + pushZ * pushZ;

    if (distSq >= radius * radius)
        return false;

    if (distSq > 0.000001f)
    {
        FLOAT dist = sqrtf(distSq);
        FLOAT push = radius - dist + 0.003f;
        localX += (pushX / dist) * push;
        localZ += (pushZ / dist) * push;
    }
    else
    {
        FLOAT left = fabsf(localX - minX);
        FLOAT right = fabsf(maxX - localX);
        FLOAT back = fabsf(localZ - minZ);
        FLOAT front = fabsf(maxZ - localZ);
        FLOAT best = min(min(left, right), min(back, front));
        if (best == left)
            localX = minX - radius - 0.003f;
        else if (best == right)
            localX = maxX + radius + 0.003f;
        else if (best == back)
            localZ = minZ - radius - 0.003f;
        else
            localZ = maxZ + radius + 0.003f;
    }

    *playerX = door.x + localX * c + localZ * s;
    *playerZ = door.z - localX * s + localZ * c;
    return true;
}

static bool ResolveDoorCollision()
{
    FLOAT playerX = XMVectorGetX(g_playerPos);
    FLOAT playerZ = XMVectorGetZ(g_playerPos);
    bool collided = false;

    for (size_t i = 0; i < g_doors.size(); ++i)
    {
        const NativeDoor& door = g_doors[i];
        if (fabsf(playerX - door.x) > 3.2f || fabsf(playerZ - door.z) > 3.2f)
            continue;

        FLOAT panelHalfWidth, height, thickness, slide;
        DoorDimensions(door, &panelHalfWidth, &height, &thickness, &slide);
        FLOAT progress = DoorSlideProgress(door);
        FLOAT base = panelHalfWidth * 0.94f;
        FLOAT offset = slide * progress;
        collided = ResolveDoorPanelCollision(door, -base - offset, panelHalfWidth, height * 0.5f, thickness, &playerX, &playerZ) || collided;
        collided = ResolveDoorPanelCollision(door, base + offset, panelHalfWidth, height * 0.5f, thickness, &playerX, &playerZ) || collided;
    }

    if (collided)
    {
        g_playerPos = XMVectorSetX(g_playerPos, playerX);
        g_playerPos = XMVectorSetZ(g_playerPos, playerZ);
        g_collisionFlash = 0.25f;
        ++g_collisionCount;
    }

    return collided;
}

static void UpdateNativeDoors(FLOAT dt)
{
    FLOAT fpsFactor = dt * 70.0f;
    for (size_t i = 0; i < g_doors.size(); ++i)
    {
        NativeDoor& door = g_doors[i];
        FLOAT speed = DoorOpenSpeed(door) * fpsFactor;
        if (door.open)
        {
            if (door.openState < 180.0f)
                door.openState = min(180.0f, door.openState + speed);
            else
            {
                door.fastOpen = 0;
                if (door.timerState > 0.0f)
                {
                    door.timerState = max(0.0f, door.timerState - fpsFactor);
                    if (door.timerState <= 0.0f)
                        door.open = false;
                }
                else if (door.autoClose)
                {
                    FLOAT dx = XMVectorGetX(g_playerPos) - door.x;
                    FLOAT dz = XMVectorGetZ(g_playerPos) - door.z;
                    if (dx * dx + dz * dz < 2.1f * 2.1f)
                    {
                        door.open = false;
                        door.autoClose = false;
                        SetStatus("DOOR AUTOCLOSE");
                    }
                }
            }
        }
        else
        {
            if (door.openState > 0.0f)
                door.openState = max(0.0f, door.openState - speed);
            else
                door.fastOpen = 0;
        }
    }

    BuildDoorVisuals();
}

static void SetFacilityCell(bool cells[10][18], INT x, INT y)
{
    if (x >= 0 && x < 10 && y >= 0 && y < 18)
        cells[x][y] = true;
}

static INT FacilityZoneForY(INT y)
{
    if (y < 7)
        return 3;
    if (y < 12)
        return 2;
    return 1;
}

static void PickShapeAndAngle(bool xm, bool xp, bool zm, bool zp, INT* shape, FLOAT* yaw)
{
    INT count = (xm ? 1 : 0) + (xp ? 1 : 0) + (zm ? 1 : 0) + (zp ? 1 : 0);
    *shape = 2;
    *yaw = 0.0f;

    if (count == 1)
    {
        *shape = 1;
        if (zp) *yaw = 180.0f;
        else if (xm) *yaw = 270.0f;
        else if (xp) *yaw = 90.0f;
        else *yaw = 0.0f;
    }
    else if (count == 2)
    {
        if (xm && xp)
        {
            *shape = 2;
            *yaw = 90.0f;
        }
        else if (zm && zp)
        {
            *shape = 2;
            *yaw = 0.0f;
        }
        else
        {
            *shape = 3;
            if (xm && zp) *yaw = 180.0f;
            else if (xp && zp) *yaw = 90.0f;
            else if (xm && zm) *yaw = 270.0f;
            else *yaw = 0.0f;
        }
    }
    else if (count == 3)
    {
        *shape = 4;
        if (!zm) *yaw = 180.0f;
        else if (!xm) *yaw = 90.0f;
        else if (!xp) *yaw = 270.0f;
        else *yaw = 0.0f;
    }
    else
    {
        *shape = 5;
        *yaw = 0.0f;
    }
}

static void RecountNativeMapRooms()
{
    ZeroMemory(g_map.roomAmount, sizeof(g_map.roomAmount));

    for (INT y = 1; y < MAP_HEIGHT; ++y)
    {
        INT zone = GetZoneIndex(y);
        for (INT x = 1; x < MAP_WIDTH; ++x)
        {
            if (g_map.temp[x][y] <= 0)
                continue;

            INT neighborCount = MinCell(g_map.temp[x + 1][y]) + MinCell(g_map.temp[x - 1][y]) +
                MinCell(g_map.temp[x][y + 1]) + MinCell(g_map.temp[x][y - 1]);

            if (g_map.temp[x][y] < 255)
                g_map.temp[x][y] = neighborCount;

            if (g_map.temp[x][y] == 1)
            {
                ++g_map.roomAmount[ROOM1][zone];
            }
            else if (g_map.temp[x][y] == 2)
            {
                if ((MinCell(g_map.temp[x + 1][y]) + MinCell(g_map.temp[x - 1][y])) == 2 ||
                    (MinCell(g_map.temp[x][y + 1]) + MinCell(g_map.temp[x][y - 1])) == 2)
                    ++g_map.roomAmount[ROOM2][zone];
                else
                    ++g_map.roomAmount[ROOM2C][zone];
            }
            else if (g_map.temp[x][y] == 3)
            {
                ++g_map.roomAmount[ROOM3][zone];
            }
            else if (g_map.temp[x][y] == 4)
            {
                ++g_map.roomAmount[ROOM4][zone];
            }
        }
    }
}

static void AssignNativeMapRoomSlots()
{
    ZeroMemory(g_map.roomSlots, sizeof(g_map.roomSlots));
    ZeroMemory(g_map.roomId, sizeof(g_map.roomId));

    INT minPos, maxPos;

    SetRoomSlot(ROOM1, 0, "start");
    minPos = 1;
    maxPos = g_map.roomAmount[ROOM1][0] - 1;
    SetRoomLikeBlitz("roompj", ROOM1, (INT)floorf(0.1f * (FLOAT)g_map.roomAmount[ROOM1][0]), minPos, maxPos);
    SetRoomLikeBlitz("914", ROOM1, (INT)floorf(0.3f * (FLOAT)g_map.roomAmount[ROOM1][0]), minPos, maxPos);
    SetRoomLikeBlitz("room1archive", ROOM1, (INT)floorf(0.5f * (FLOAT)g_map.roomAmount[ROOM1][0]), minPos, maxPos);
    SetRoomLikeBlitz("room205", ROOM1, (INT)floorf(0.6f * (FLOAT)g_map.roomAmount[ROOM1][0]), minPos, maxPos);

    SetRoomSlot(ROOM2C, 0, "lockroom");
    SetRoomSlot(ROOM2, 0, "room2closets");
    minPos = 1;
    maxPos = g_map.roomAmount[ROOM2][0] - 1;
    SetRoomLikeBlitz("room2testroom2", ROOM2, (INT)floorf(0.1f * (FLOAT)g_map.roomAmount[ROOM2][0]), minPos, maxPos);
    SetRoomLikeBlitz("room2scps", ROOM2, (INT)floorf(0.2f * (FLOAT)g_map.roomAmount[ROOM2][0]), minPos, maxPos);
    SetRoomLikeBlitz("room2storage", ROOM2, (INT)floorf(0.3f * (FLOAT)g_map.roomAmount[ROOM2][0]), minPos, maxPos);
    SetRoomLikeBlitz("room2gw_b", ROOM2, (INT)floorf(0.4f * (FLOAT)g_map.roomAmount[ROOM2][0]), minPos, maxPos);
    SetRoomLikeBlitz("room2sl", ROOM2, (INT)floorf(0.5f * (FLOAT)g_map.roomAmount[ROOM2][0]), minPos, maxPos);
    SetRoomLikeBlitz("room012", ROOM2, (INT)floorf(0.55f * (FLOAT)g_map.roomAmount[ROOM2][0]), minPos, maxPos);
    SetRoomLikeBlitz("room2scps2", ROOM2, (INT)floorf(0.6f * (FLOAT)g_map.roomAmount[ROOM2][0]), minPos, maxPos);
    SetRoomLikeBlitz("room1123", ROOM2, (INT)floorf(0.7f * (FLOAT)g_map.roomAmount[ROOM2][0]), minPos, maxPos);
    SetRoomLikeBlitz("room2elevator", ROOM2, (INT)floorf(0.85f * (FLOAT)g_map.roomAmount[ROOM2][0]), minPos, maxPos);

    if (g_map.roomAmount[ROOM3][0] > 0)
        SetRoomSlot(ROOM3, g_map.roomAmount[ROOM3][0] / 2, "room3storage");
    if (g_map.roomAmount[ROOM2C][0] > 0)
        SetRoomSlot(ROOM2C, g_map.roomAmount[ROOM2C][0] / 2, "room1162");
    if (g_map.roomAmount[ROOM4][0] > 0)
        SetRoomSlot(ROOM4, g_map.roomAmount[ROOM4][0] / 3, "room4info");

    minPos = g_map.roomAmount[ROOM1][0];
    maxPos = g_map.roomAmount[ROOM1][0] + g_map.roomAmount[ROOM1][1] - 1;
    SetRoomLikeBlitz("room079", ROOM1, minPos + (INT)floorf(0.15f * (FLOAT)g_map.roomAmount[ROOM1][1]), minPos, maxPos);
    SetRoomLikeBlitz("room106", ROOM1, minPos + (INT)floorf(0.3f * (FLOAT)g_map.roomAmount[ROOM1][1]), minPos, maxPos);
    SetRoomLikeBlitz("008", ROOM1, minPos + (INT)floorf(0.4f * (FLOAT)g_map.roomAmount[ROOM1][1]), minPos, maxPos);
    SetRoomLikeBlitz("room035", ROOM1, minPos + (INT)floorf(0.5f * (FLOAT)g_map.roomAmount[ROOM1][1]), minPos, maxPos);
    SetRoomLikeBlitz("coffin", ROOM1, minPos + (INT)floorf(0.7f * (FLOAT)g_map.roomAmount[ROOM1][1]), minPos, maxPos);

    minPos = g_map.roomAmount[ROOM2][0];
    maxPos = g_map.roomAmount[ROOM2][0] + g_map.roomAmount[ROOM2][1] - 1;
    SetRoomSlot(ROOM2, minPos + (INT)floorf(0.1f * (FLOAT)g_map.roomAmount[ROOM2][1]), "room2nuke");
    SetRoomLikeBlitz("room2tunnel", ROOM2, minPos + (INT)floorf(0.25f * (FLOAT)g_map.roomAmount[ROOM2][1]), minPos, maxPos);
    SetRoomLikeBlitz("room049", ROOM2, minPos + (INT)floorf(0.4f * (FLOAT)g_map.roomAmount[ROOM2][1]), minPos, maxPos);
    SetRoomLikeBlitz("room2shaft", ROOM2, minPos + (INT)floorf(0.6f * (FLOAT)g_map.roomAmount[ROOM2][1]), minPos, maxPos);
    SetRoomLikeBlitz("testroom", ROOM2, minPos + (INT)floorf(0.7f * (FLOAT)g_map.roomAmount[ROOM2][1]), minPos, maxPos);
    SetRoomLikeBlitz("room2servers", ROOM2, minPos + (INT)floorf(0.9f * (FLOAT)g_map.roomAmount[ROOM2][1]), minPos, maxPos);

    if (g_map.roomAmount[ROOM3][1] > 0)
        SetRoomSlot(ROOM3, g_map.roomAmount[ROOM3][0] + g_map.roomAmount[ROOM3][1] / 3, "room513");
    if (g_map.roomAmount[ROOM3][1] > 1)
        SetRoomSlot(ROOM3, g_map.roomAmount[ROOM3][0] + (g_map.roomAmount[ROOM3][1] * 2) / 3, "room966");
    if (g_map.roomAmount[ROOM2C][1] > 0)
        SetRoomSlot(ROOM2C, g_map.roomAmount[ROOM2C][0] + g_map.roomAmount[ROOM2C][1] / 2, "room2cpit");

    if (g_map.roomAmount[ROOM1][2] > 0)
        SetRoomSlot(ROOM1, g_map.roomAmount[ROOM1][0] + g_map.roomAmount[ROOM1][1], "room1lifts");
    if (g_map.roomAmount[ROOM1][2] > 1)
        SetRoomSlot(ROOM1, g_map.roomAmount[ROOM1][0] + g_map.roomAmount[ROOM1][1] + g_map.roomAmount[ROOM1][2] - 2, "exit1");
    if (g_map.roomAmount[ROOM1][2] > 0)
        SetRoomSlot(ROOM1, g_map.roomAmount[ROOM1][0] + g_map.roomAmount[ROOM1][1] + g_map.roomAmount[ROOM1][2] - 1, "gateaentrance");

    minPos = g_map.roomAmount[ROOM2][0] + g_map.roomAmount[ROOM2][1];
    maxPos = g_map.roomAmount[ROOM2][0] + g_map.roomAmount[ROOM2][1] + g_map.roomAmount[ROOM2][2] - 1;
    SetRoomSlot(ROOM2, minPos + (INT)floorf(0.1f * (FLOAT)g_map.roomAmount[ROOM2][2]), "room2poffices");
    SetRoomLikeBlitz("room2cafeteria", ROOM2, minPos + (INT)floorf(0.2f * (FLOAT)g_map.roomAmount[ROOM2][2]), minPos, maxPos);
    SetRoomLikeBlitz("room2sroom", ROOM2, minPos + (INT)floorf(0.3f * (FLOAT)g_map.roomAmount[ROOM2][2]), minPos, maxPos);
    SetRoomLikeBlitz("room2servers2", ROOM2, minPos + (INT)floorf(0.4f * (FLOAT)g_map.roomAmount[ROOM2][2]), minPos, maxPos);
    SetRoomLikeBlitz("room2offices", ROOM2, minPos + (INT)floorf(0.45f * (FLOAT)g_map.roomAmount[ROOM2][2]), minPos, maxPos);
    SetRoomLikeBlitz("room2offices4", ROOM2, minPos + (INT)floorf(0.5f * (FLOAT)g_map.roomAmount[ROOM2][2]), minPos, maxPos);
    SetRoomLikeBlitz("room860", ROOM2, minPos + (INT)floorf(0.6f * (FLOAT)g_map.roomAmount[ROOM2][2]), minPos, maxPos);
    SetRoomLikeBlitz("medibay", ROOM2, minPos + (INT)floorf(0.7f * (FLOAT)g_map.roomAmount[ROOM2][2]), minPos, maxPos);
    SetRoomLikeBlitz("room2poffices2", ROOM2, minPos + (INT)floorf(0.8f * (FLOAT)g_map.roomAmount[ROOM2][2]), minPos, maxPos);
    SetRoomLikeBlitz("room2offices2", ROOM2, minPos + (INT)floorf(0.9f * (FLOAT)g_map.roomAmount[ROOM2][2]), minPos, maxPos);

    SetRoomSlot(ROOM2C, g_map.roomAmount[ROOM2C][0] + g_map.roomAmount[ROOM2C][1], "room2ccont");
    SetRoomSlot(ROOM2C, g_map.roomAmount[ROOM2C][0] + g_map.roomAmount[ROOM2C][1] + 1, "lockroom2");
    if (g_map.roomAmount[ROOM3][2] > 0)
        SetRoomSlot(ROOM3, g_map.roomAmount[ROOM3][0] + g_map.roomAmount[ROOM3][1] + g_map.roomAmount[ROOM3][2] / 3, "room3servers");
    if (g_map.roomAmount[ROOM3][2] > 1)
        SetRoomSlot(ROOM3, g_map.roomAmount[ROOM3][0] + g_map.roomAmount[ROOM3][1] + (g_map.roomAmount[ROOM3][2] * 2) / 3, "room3servers2");
    if (g_map.roomAmount[ROOM3][2] > 2)
        SetRoomSlot(ROOM3, g_map.roomAmount[ROOM3][0] + g_map.roomAmount[ROOM3][1] + g_map.roomAmount[ROOM3][2] / 2, "room3offices");
}

static void GenerateNativeMapGrid()
{
    ZeroMemory(&g_map, sizeof(g_map));
    SeedNativeMap(g_mapSeed);

    INT x = MAP_WIDTH / 2;
    INT y = MAP_HEIGHT - 2;
    for (INT i = y; i <= MAP_HEIGHT - 1; ++i)
        g_map.temp[x][i] = 1;

    while (y >= 2)
    {
        INT width = RandInt(10, 15);
        if (x > (INT)(MAP_WIDTH * 0.6f))
            width = -width;
        else if (x > (INT)(MAP_WIDTH * 0.4f))
            x = x - width / 2;

        if (x + width > MAP_WIDTH - 3)
            width = MAP_WIDTH - 3 - x;
        else if (x + width < 2)
            width = -x + 2;

        x = min(x, x + width);
        width = abs(width);
        for (INT i = x; i <= x + width; ++i)
            g_map.temp[min(i, MAP_WIDTH)][y] = 1;

        INT height = RandInt(3, 4);
        if (y - height < 1)
            height = y - 1;
        if (height <= 0)
            break;

        if (GetZoneIndex(y - height) != GetZoneIndex(y - height + 1))
            --height;
        if (height <= 0)
            break;

        INT yHallways = RandInt(4, 5);
        INT nextX = x;
        for (INT i = 1; i <= yHallways; ++i)
        {
            INT x2 = ClampInt(RandInt(x, x + width - 1), 2, MAP_WIDTH - 2);
            INT guard = 0;
            while (x2 < MAP_WIDTH - 2 &&
                (g_map.temp[x2][y - 1] || g_map.temp[x2 - 1][y - 1] || g_map.temp[x2 + 1][y - 1]) &&
                guard++ < MAP_WIDTH)
            {
                ++x2;
            }

            if (x2 < x + width)
            {
                INT tempHeight;
                if (i == 1)
                {
                    tempHeight = height;
                    x2 = (RandInt(1, 2) == 1) ? x : (x + width);
                }
                else
                {
                    tempHeight = RandInt(1, height);
                }

                for (INT y2 = y - tempHeight; y2 <= y; ++y2)
                    g_map.temp[x2][y2] = (GetZoneIndex(y2) != GetZoneIndex(y2 + 1)) ? 255 : 1;
                if (tempHeight == height)
                    nextX = x2;
            }
        }

        x = nextX;
        y -= height;
    }

    RecountNativeMapRooms();
    AssignNativeMapRoomSlots();
}

static bool AppendGeneratedRoom(INT x, INT y, INT shape, FLOAT yaw, const char* roomName)
{
    INT zoneIndex = GetZoneIndex(y);
    INT ordinal = g_map.roomId[shape];
    const char* fileName = ResolveRoomMeshName(roomName, zoneIndex, shape, x * 37 + y * 11 + ordinal);

    FLOAT worldX = (FLOAT)(x - (MAP_WIDTH / 2)) * 8.0f;
    FLOAT worldZ = (FLOAT)(y - (MAP_HEIGHT - 2)) * 8.0f;
    bool ok = TryAppendRoomFile(fileName, worldX, worldZ, yaw);

    if (roomName && _stricmp(roomName, "start") == 0)
    {
        g_map.startX = worldX;
        g_map.startZ = worldZ;
        g_map.foundStart = true;
    }

    return ok;
}

static bool InstantiateNativeMap()
{
    bool anyLoaded = false;

    for (INT y = MAP_HEIGHT - 1; y >= 1; --y)
    {
        for (INT x = 1; x <= MAP_WIDTH - 2; ++x)
        {
            if (g_map.temp[x][y] <= 0)
                continue;

            bool xm = g_map.temp[x - 1][y] > 0;
            bool xp = g_map.temp[x + 1][y] > 0;
            bool zm = g_map.temp[x][y - 1] > 0;
            bool zp = g_map.temp[x][y + 1] > 0;

            INT shape = ROOM2;
            FLOAT yaw = 0.0f;
            const char* roomName = "";

            if (g_map.temp[x][y] == 255)
            {
                shape = ROOM2;
                roomName = (y > MAP_HEIGHT / 2) ? "checkpoint1" : "checkpoint2";
            }
            else
            {
                INT neighborCount = MinCell(g_map.temp[x + 1][y]) + MinCell(g_map.temp[x - 1][y]) +
                    MinCell(g_map.temp[x][y + 1]) + MinCell(g_map.temp[x][y - 1]);
                PickShapeAndAngle(xm, xp, zm, zp, &shape, &yaw);

                INT id = g_map.roomId[shape];
                if (id >= 0 && id < MAX_MAP_ROOMS && g_map.roomSlots[shape][id][0] != 0)
                    roomName = g_map.roomSlots[shape][id];

                if (neighborCount == 2 && (xm && xp))
                    yaw = (RandInt(1, 2) == 1) ? 90.0f : 270.0f;
                else if (neighborCount == 2 && (zm && zp))
                    yaw = (RandInt(1, 2) == 1) ? 180.0f : 0.0f;
            }

            anyLoaded = AppendGeneratedRoom(x, y, shape, yaw, roomName) || anyLoaded;
            g_map.shape[x][y] = shape;
            g_map.yaw[x][y] = WrapDegrees(yaw);
            if (shape >= ROOM1 && shape <= ROOM4)
                ++g_map.roomId[shape];
        }
    }

    TryAppendRoomFileAt("gatea_opt.rmesh", (FLOAT)((MAP_WIDTH - 1) - (MAP_WIDTH / 2)) * 8.0f, 500.0f, (FLOAT)(1 - (MAP_HEIGHT - 2)) * 8.0f, 0.0f);
    TryAppendRoomFile("pocketdimension1_opt.rmesh", (FLOAT)((MAP_WIDTH - 1) - (MAP_WIDTH / 2)) * 8.0f, (FLOAT)((MAP_HEIGHT - 1) - (MAP_HEIGHT - 2)) * 8.0f, 0.0f);
    TryAppendRoomFileAt("dimension1499_opt.rmesh", (FLOAT)(1 - (MAP_WIDTH / 2)) * 8.0f, 800.0f, (FLOAT)(0 - (MAP_HEIGHT - 2)) * 8.0f, 0.0f);

    return anyLoaded;
}

static void GenerateNativeDoors()
{
    g_doors.clear();

    for (INT y = MAP_HEIGHT - 1; y >= 1; --y)
    {
        for (INT x = 1; x <= MAP_WIDTH - 2; ++x)
        {
            if (g_map.temp[x][y] <= 0)
                continue;

            FLOAT worldX = (FLOAT)(x - (MAP_WIDTH / 2)) * 8.0f;
            FLOAT worldZ = (FLOAT)(y - (MAP_HEIGHT - 2)) * 8.0f;
            INT shape = g_map.shape[x][y];
            FLOAT yaw = WrapDegrees(g_map.yaw[x][y]);
            INT dir = (GetZoneIndex(y) == 1) ? 2 : 0;
            bool shouldSpawnEast = false;
            bool shouldSpawnSouth = false;

            switch (shape)
            {
            case ROOM1:
                shouldSpawnEast = AngleIs(yaw, 90.0f);
                shouldSpawnSouth = AngleIs(yaw, 180.0f);
                break;
            case ROOM2:
                shouldSpawnEast = AngleIs(yaw, 90.0f) || AngleIs(yaw, 270.0f);
                shouldSpawnSouth = AngleIs(yaw, 0.0f) || AngleIs(yaw, 180.0f);
                break;
            case ROOM2C:
                shouldSpawnEast = AngleIs(yaw, 0.0f) || AngleIs(yaw, 90.0f);
                shouldSpawnSouth = AngleIs(yaw, 180.0f) || AngleIs(yaw, 90.0f);
                break;
            case ROOM3:
                shouldSpawnEast = AngleIs(yaw, 0.0f) || AngleIs(yaw, 180.0f) || AngleIs(yaw, 90.0f);
                shouldSpawnSouth = AngleIs(yaw, 180.0f) || AngleIs(yaw, 90.0f) || AngleIs(yaw, 270.0f);
                break;
            default:
                shouldSpawnEast = true;
                shouldSpawnSouth = true;
                break;
            }

            if (shouldSpawnEast && g_map.temp[x + 1][y] > 0)
            {
                bool open = RandInt(-3, 1) > 0;
                AddNativeDoor(worldX + 4.0f, 0.0f, worldZ, 90.0f, dir, open, false);
                if (open && dir == 0 && RandInt(1, 8) == 1)
                    g_doors.back().autoClose = true;
            }
            if (shouldSpawnSouth && g_map.temp[x][y + 1] > 0)
            {
                bool open = RandInt(-3, 1) > 0;
                AddNativeDoor(worldX, 0.0f, worldZ + 4.0f, 0.0f, dir, open, false);
                if (open && dir == 0 && RandInt(1, 8) == 1)
                    g_doors.back().autoClose = true;
            }
        }
    }

    BuildDoorVisuals();
}

static bool LoadFacility()
{
    ClearFacilityGeometry();
    ResetPlayerVitals();
    g_loadingPercent = 5;
    CopyRoomName(g_loadingText, "GENERATING MAP GRID");
    RenderLoadingScreen(g_loadingPercent, g_loadingText);

    GenerateNativeMapGrid();

    g_loadingPercent = 20;
    CopyRoomName(g_loadingText, "ASSIGNING ROOMS");
    RenderLoadingScreen(g_loadingPercent, g_loadingText);

    bool anyLoaded = InstantiateNativeMap();
    GenerateNativeDoors();

    g_loadingPercent = 85;
    CopyRoomName(g_loadingText, "FINALIZING FACILITY");
    RenderLoadingScreen(g_loadingPercent, g_loadingText);

    if (anyLoaded)
    {
        FLOAT startX = g_map.foundStart ? g_map.startX : 0.0f;
        FLOAT startZ = g_map.foundStart ? g_map.startZ : 0.0f;
        g_playerPos = XMVectorSet(startX, 1.7f, startZ - 12.0f, 0.0f);
        g_yaw = XM_PI;
        g_pitch = 0.0f;
        g_loadingPercent = 100;
        CopyRoomName(g_loadingText, "READY");
        RenderLoadingScreen(g_loadingPercent, g_loadingText);
        SetStatus("MAPSYS: %d ROOMS %u TRIS %u DOORS", g_facilityRoomCount, (unsigned)(g_roomVerts.size() / 3), (unsigned)g_doors.size());
        return true;
    }

    BuildFallbackRoom();
    return false;
}

static FLOAT LerpFloat(FLOAT a, FLOAT b, FLOAT t)
{
    return a + (b - a) * Clamp(t, 0.0f, 1.0f);
}

static FLOAT Range01(FLOAT t, FLOAT start, FLOAT end)
{
    if (end <= start)
        return t >= end ? 1.0f : 0.0f;
    return Clamp((t - start) / (end - start), 0.0f, 1.0f);
}

static FLOAT SmoothToward(FLOAT current, FLOAT target, FLOAT speed, FLOAT dt)
{
    FLOAT t = Clamp(speed * dt, 0.0f, 1.0f);
    return current + (target - current) * t;
}

static FLOAT NextBlinkFrequency()
{
    return (FLOAT)RandInt(420, 700) / 70.0f;
}

static void ResetPlayerVitals()
{
    g_blinkFrequency = NextBlinkFrequency();
    g_blinkTimer = g_blinkFrequency;
    g_blinkAlpha = 0.0f;
    g_stamina = 100.0f;
    g_staminaEffect = 1.0f;
    g_crouch = false;
    g_crouchState = 0.0f;
    g_playerSprinting = false;
    g_stepCycle = 0.0f;
    g_stepCueCount = 0;
}

static void UpdateBlink(FLOAT dt, DWORD buttons)
{
    bool blinkPressed = (buttons & XINPUT_GAMEPAD_RIGHT_SHOULDER) != 0;
    bool blinkHit = blinkPressed && ((g_lastButtons & XINPUT_GAMEPAD_RIGHT_SHOULDER) == 0);

    if (blinkHit && g_blinkTimer > 0.0f)
        g_blinkTimer = 0.0f;

    if (g_blinkTimer <= 0.0f)
    {
        g_blinkTimer -= dt;

        if (blinkPressed && g_blinkTimer < -0.14f)
            g_blinkTimer = -0.14f;

        FLOAT phase = Clamp(-g_blinkTimer / 0.285f, 0.0f, 1.0f);
        if (phase < 0.25f)
            g_blinkAlpha = sinf((phase / 0.25f) * XM_PIDIV2);
        else if (phase < 0.75f)
            g_blinkAlpha = 1.0f;
        else
            g_blinkAlpha = sinf(((1.0f - phase) / 0.25f) * XM_PIDIV2);

        if (!blinkPressed && g_blinkTimer <= -0.285f)
        {
            g_blinkFrequency = NextBlinkFrequency();
            g_blinkTimer = g_blinkFrequency;
            g_blinkAlpha = 0.0f;
        }
    }
    else
    {
        g_blinkTimer -= dt * 0.6f;
        g_blinkAlpha = 0.0f;
    }
}

static void UpdatePlayerVitals(FLOAT dt, DWORD buttons, bool moving, bool sprintRequested)
{
    if ((buttons & XINPUT_GAMEPAD_B) && !(g_lastButtons & XINPUT_GAMEPAD_B))
    {
        g_crouch = !g_crouch;
        SetStatus(g_crouch ? "PLAYER CROUCH" : "PLAYER STAND");
    }

    g_crouchState = SmoothToward(g_crouchState, g_crouch ? 1.0f : 0.0f, 8.0f, dt);

    bool canSprint = sprintRequested && moving && !g_crouch && g_stamina > 0.0f;
    g_playerSprinting = canSprint;
    if (canSprint)
    {
        g_stamina -= 28.0f * dt * g_staminaEffect;
        if (g_stamina <= 0.0f)
        {
            g_stamina = -20.0f;
            g_playerSprinting = false;
            SetStatus("PLAYER EXHAUSTED");
        }
    }
    else
    {
        FLOAT regen = moving ? 8.4f : 13.0f;
        g_stamina = min(100.0f, g_stamina + regen * dt);
    }

    if (moving)
    {
        FLOAT stepRate = g_playerSprinting ? 5.8f : (g_crouchState > 0.5f ? 2.0f : 3.7f);
        FLOAT oldStep = g_stepCycle;
        g_stepCycle += dt * stepRate;
        if ((INT)oldStep != (INT)g_stepCycle)
            ++g_stepCueCount;
    }
    else
    {
        g_stepCycle = SmoothToward(g_stepCycle, 0.0f, 4.0f, dt);
    }

    UpdateBlink(dt, buttons);
}

static FLOAT GetPlayerMoveSpeed(FLOAT walkSpeed, FLOAT sprintSpeed)
{
    FLOAT speed = g_playerSprinting ? sprintSpeed : walkSpeed;
    return speed / (1.0f + g_crouchState);
}

static XMVECTOR GetCameraEye()
{
    FLOAT x = XMVectorGetX(g_playerPos);
    FLOAT y = XMVectorGetY(g_playerPos) - 0.42f * g_crouchState;
    FLOAT z = XMVectorGetZ(g_playerPos);
    return XMVectorSet(x, y, z, 0.0f);
}

static void SetIntroText(const char* a, const char* b)
{
    _snprintf(g_introSubtitle, sizeof(g_introSubtitle), "%s", a ? a : "");
    g_introSubtitle[sizeof(g_introSubtitle) - 1] = 0;
    _snprintf(g_introSubtitle2, sizeof(g_introSubtitle2), "%s", b ? b : "");
    g_introSubtitle2[sizeof(g_introSubtitle2) - 1] = 0;
}

static void ResetFrameCounter()
{
    g_fps = 0.0f;
    g_frameMs = 0.0f;
    g_fpsTimer = 0.0f;
    g_fpsFrames = 0;
    QueryPerformanceCounter(&g_lastTime);
}

static void UpdateFrameCounter(FLOAT rawDt)
{
    rawDt = Clamp(rawDt, 0.0f, 5.0f);
    g_frameMs = rawDt * 1000.0f;
    g_fpsTimer += rawDt;
    ++g_fpsFrames;
    if (g_fpsTimer >= 0.50f)
    {
        g_fps = g_fpsTimer > 0.0f ? (FLOAT)g_fpsFrames / g_fpsTimer : 0.0f;
        g_fpsFrames = 0;
        g_fpsTimer = 0.0f;
    }
}

static void AddIntroActor(FLOAT x, FLOAT y, FLOAT z, FLOAT width, FLOAT height, DWORD color)
{
    AddBox(g_scriptedVerts, x - width, y, z - width, x + width, y + height, z + width, color);
}

static bool AddIntroNpcModel(const NativeModel& model, FLOAT x, FLOAT y, FLOAT z, FLOAT scale, FLOAT yaw, DWORD tint)
{
    if (!model.loaded)
        return false;

    IntroModelInstance instance;
    instance.model = &model;
    instance.x = x;
    instance.y = y;
    instance.z = z;
    instance.sx = scale;
    instance.sy = scale;
    instance.sz = scale;
    instance.yaw = yaw;
    instance.tint = tint;
    g_introModelInstances.push_back(instance);
    return true;
}

static void AddIntroDoor(FLOAT x, FLOAT z, bool alongX, DWORD color)
{
    NativeDoor door;
    door.x = x;
    door.y = 0.0f;
    door.z = z;
    door.angle = alongX ? 0.0f : 90.0f;
    door.dir = 0;
    door.keycard = 0;
    door.fastOpen = 0;
    door.openState = 0.0f;
    door.timer = 0.0f;
    door.timerState = 0.0f;
    door.locked = false;
    door.open = false;
    door.autoClose = false;
    door.code[0] = 0;
    if (AppendDoorModels(g_scriptedSurfaces, door, false))
        return;

    if (alongX)
        AddBox(g_scriptedVerts, x - 1.25f, 0.0f, z - 0.07f, x + 1.25f, 2.35f, z + 0.07f, color);
    else
        AddBox(g_scriptedVerts, x - 0.07f, 0.0f, z - 1.25f, x + 0.07f, 2.35f, z + 1.25f, color);
}

static void BuildIntroFrame()
{
    FLOAT t = g_introTimer;
    g_scriptedVerts.clear();
    g_scriptedSurfaces.clear();
    g_introModelInstances.clear();

    DWORD guardColor = D3DCOLOR_XRGB(70, 120, 210);
    DWORD classDColor = D3DCOLOR_XRGB(210, 110, 40);
    DWORD scientistColor = D3DCOLOR_XRGB(210, 210, 195);
    DWORD scpColor = D3DCOLOR_XRGB(130, 120, 95);
    DWORD deadColor = D3DCOLOR_XRGB(120, 30, 25);
    DWORD doorColor = D3DCOLOR_XRGB(70, 75, 82);
    DWORD warningColor = D3DCOLOR_XRGB(180, 35, 30);

    if (t < 18.0f)
        AddIntroDoor(-40.96f, 5.28f, true, doorColor);
    if (t < 72.0f || (t > 84.0f && t < 104.0f))
        AddIntroDoor(2.88f, 3.84f, false, doorColor);
    if (t < 116.0f)
        AddIntroDoor(-10.08f, -6.88f, false, doorColor);

    FLOAT escortA = Range01(t, 22.0f, 58.0f);
    FLOAT guardX = LerpFloat(-39.6f, -5.4f, escortA);
    FLOAT guardZ = LerpFloat(6.3f, 4.9f, escortA);
    if (!AddIntroNpcModel(g_modelNpcGuard, guardX, 0.0f, guardZ, 0.116f, 92.0f, D3DCOLOR_XRGB(210, 220, 235)))
        AddIntroActor(guardX, 0.0f, guardZ, 0.22f, 1.75f, guardColor);
    if (!AddIntroNpcModel(g_modelNpcGuard, guardX - 1.0f, 0.0f, guardZ - 0.8f, 0.116f, 92.0f, D3DCOLOR_XRGB(210, 220, 235)))
        AddIntroActor(guardX - 1.0f, 0.0f, guardZ - 0.8f, 0.22f, 1.75f, guardColor);

    if (t < 99.0f)
    {
        if (!AddIntroNpcModel(g_modelNpcClassD, -0.80f, 0.0f, 5.26f, 0.115f, 180.0f, D3DCOLOR_XRGB(245, 220, 195)))
            AddIntroActor(-0.80f, 0.0f, 5.26f, 0.25f, 1.70f, classDColor);
    }
    else
        AddBox(g_scriptedVerts, -1.35f, 0.05f, 4.95f, -0.25f, 0.32f, 5.55f, deadColor);

    if (t < 104.0f)
    {
        if (!AddIntroNpcModel(g_modelNpcClassD, 6.60f, 0.0f, 5.26f, 0.115f, 180.0f, D3DCOLOR_XRGB(245, 220, 195)))
            AddIntroActor(6.60f, 0.0f, 5.26f, 0.25f, 1.70f, classDColor);
    }
    else
        AddBox(g_scriptedVerts, 6.10f, 0.05f, 4.95f, 7.10f, 0.32f, 5.55f, deadColor);

    if (!AddIntroNpcModel(g_modelNpcClerk.loaded ? g_modelNpcClerk : g_modelNpcClassD, -11.90f, 3.85f, 4.56f, 0.110f, 180.0f, D3DCOLOR_XRGB(240, 240, 225)))
        AddIntroActor(-11.90f, 3.85f, 4.56f, 0.20f, 1.65f, scientistColor);

    if (t < 96.0f)
    {
        if (!AddIntroNpcModel(g_modelNpc173, 14.72f, 0.0f, 9.12f, 0.065f, 180.0f, D3DCOLOR_XRGB(210, 205, 180)))
            AddIntroActor(14.72f, 0.0f, 9.12f, 0.35f, 1.85f, scpColor);
    }
    else if (t < 102.0f)
    {
        if (!AddIntroNpcModel(g_modelNpc173, -0.80f, 0.0f, 5.26f, 0.065f, 180.0f, D3DCOLOR_XRGB(210, 205, 180)))
            AddIntroActor(-0.80f, 0.0f, 5.26f, 0.35f, 1.85f, scpColor);
    }
    else if (t < 110.0f)
    {
        if (!AddIntroNpcModel(g_modelNpc173, 6.60f, 0.0f, 5.26f, 0.065f, 180.0f, D3DCOLOR_XRGB(210, 205, 180)))
            AddIntroActor(6.60f, 0.0f, 5.26f, 0.35f, 1.85f, scpColor);
    }
    else if (t < 119.0f)
    {
        if (!AddIntroNpcModel(g_modelNpc173, -6.08f, 4.70f, 13.12f, 0.065f, 180.0f, D3DCOLOR_XRGB(210, 205, 180)))
            AddIntroActor(-6.08f, 4.70f, 13.12f, 0.35f, 1.85f, scpColor);
    }
    else
    {
        if (!AddIntroNpcModel(g_modelNpc173, -4.00f, 0.0f, 10.72f, 0.065f, 180.0f, D3DCOLOR_XRGB(210, 205, 180)))
            AddIntroActor(-4.00f, 0.0f, 10.72f, 0.35f, 1.85f, scpColor);
    }

    if (t > 108.0f && t < 119.0f)
    {
        if (!AddIntroNpcModel(g_modelNpcGuard, 0.40f, 4.60f, 10.72f, 0.116f, 180.0f, D3DCOLOR_XRGB(255, 185, 170)))
            AddIntroActor(0.40f, 4.60f, 10.72f, 0.22f, 1.75f, warningColor);
    }
    else
    {
        if (!AddIntroNpcModel(g_modelNpcGuard, 0.40f, 4.60f, 10.72f, 0.116f, 180.0f, D3DCOLOR_XRGB(210, 220, 235)))
            AddIntroActor(0.40f, 4.60f, 10.72f, 0.22f, 1.75f, guardColor);
    }
}

static bool LoadIntroSequence()
{
    ClearFacilityGeometry();
    ResetPlayerVitals();

    g_loadingPercent = 8;
    CopyRoomName(g_loadingText, "LOADING INTRO");
    RenderLoadingScreen(g_loadingPercent, g_loadingText);

    bool loaded = TryAppendRoomFileAt("173bright_opt.rmesh", 0.0f, 0.0f, 0.0f, 0.0f);
    if (!loaded)
        loaded = TryAppendRoomFileAt("173bright.rmesh", 0.0f, 0.0f, 0.0f, 0.0f);

    g_loadingPercent = 55;
    CopyRoomName(g_loadingText, "LOADING INTRO MODELS");
    RenderLoadingScreen(g_loadingPercent, g_loadingText);
    LoadIntroNpcAssets();

    g_loadingPercent = 75;
    CopyRoomName(g_loadingText, "SCRIPTING INTRO");
    RenderLoadingScreen(g_loadingPercent, g_loadingText);

    if (!loaded)
        BuildFallbackRoom();

    g_playerPos = XMVectorSet(-40.96f, 1.7f, 1.92f, 0.0f);
    g_yaw = XM_PIDIV2;
    g_pitch = 0.0f;
    g_introTimer = 0.0f;
    g_introBlackout = 0.0f;
    g_introTransferred = false;
    SetIntroText("D9341 WAKE UP", "RIGHT STICK LOOKS AROUND");
    BuildIntroFrame();

    g_loadingPercent = 100;
    CopyRoomName(g_loadingText, "INTRO READY");
    RenderLoadingScreen(g_loadingPercent, g_loadingText);

    SetStatus("INTRO: 173 SEQUENCE LOADED");
    return loaded;
}

static bool LoadRoom()
{
    return LoadFacility();
}

static HRESULT CompileShader(const CHAR* code, const CHAR* profile, ID3DXBuffer** out)
{
    ID3DXBuffer* errors = NULL;
    HRESULT hr = D3DXCompileShader(code, (UINT)strlen(code), NULL, NULL, "main", profile, 0, out, &errors, NULL);
    if (FAILED(hr) && errors)
    {
        OutputDebugStringA((CHAR*)errors->GetBufferPointer());
        errors->Release();
    }
    return hr;
}

static HRESULT InitD3D()
{
    g_d3d = Direct3DCreate9(D3D_SDK_VERSION);
    if (!g_d3d)
        return E_FAIL;

    D3DPRESENT_PARAMETERS pp;
    ZeroMemory(&pp, sizeof(pp));
    pp.BackBufferWidth = 1280;
    pp.BackBufferHeight = 720;
    pp.BackBufferFormat = (D3DFORMAT)MAKESRGBFMT(D3DFMT_A8R8G8B8);
    pp.FrontBufferFormat = (D3DFORMAT)MAKESRGBFMT(D3DFMT_LE_X8R8G8B8);
    pp.BackBufferCount = 1;
    pp.EnableAutoDepthStencil = TRUE;
    pp.AutoDepthStencilFormat = D3DFMT_D24S8;
    pp.SwapEffect = D3DSWAPEFFECT_DISCARD;
    pp.PresentationInterval = D3DPRESENT_INTERVAL_ONE;

    HRESULT hr = g_d3d->CreateDevice(0, D3DDEVTYPE_HAL, NULL, D3DCREATE_HARDWARE_VERTEXPROCESSING, &pp, &g_device);
    if (FAILED(hr))
        return hr;

    ID3DXBuffer* shader = NULL;
    if (FAILED(CompileShader(g_vsCode, "vs_2_0", &shader)))
        return E_FAIL;
    g_device->CreateVertexShader((DWORD*)shader->GetBufferPointer(), &g_vs);
    shader->Release();

    shader = NULL;
    if (FAILED(CompileShader(g_psCode, "ps_2_0", &shader)))
        return E_FAIL;
    g_device->CreatePixelShader((DWORD*)shader->GetBufferPointer(), &g_ps);
    shader->Release();

    static const D3DVERTEXELEMENT9 elements[] =
    {
        { 0, 0, D3DDECLTYPE_FLOAT3, D3DDECLMETHOD_DEFAULT, D3DDECLUSAGE_POSITION, 0 },
        { 0, 12, D3DDECLTYPE_D3DCOLOR, D3DDECLMETHOD_DEFAULT, D3DDECLUSAGE_COLOR, 0 },
        { 0, 16, D3DDECLTYPE_FLOAT2, D3DDECLMETHOD_DEFAULT, D3DDECLUSAGE_TEXCOORD, 0 },
        D3DDECL_END()
    };
    g_device->CreateVertexDeclaration(elements, &g_decl);

    g_device->SetRenderState(D3DRS_CULLMODE, D3DCULL_NONE);
    g_device->SetRenderState(D3DRS_ZENABLE, TRUE);
    g_device->SetRenderState(D3DRS_ALPHABLENDENABLE, TRUE);
    g_device->SetRenderState(D3DRS_SRCBLEND, D3DBLEND_SRCALPHA);
    g_device->SetRenderState(D3DRS_DESTBLEND, D3DBLEND_INVSRCALPHA);
    g_device->SetSamplerState(0, D3DSAMP_MINFILTER, D3DTEXF_LINEAR);
    g_device->SetSamplerState(0, D3DSAMP_MAGFILTER, D3DTEXF_LINEAR);
    g_device->SetSamplerState(0, D3DSAMP_MIPFILTER, D3DTEXF_LINEAR);
    g_device->SetSamplerState(0, D3DSAMP_ADDRESSU, D3DTADDRESS_WRAP);
    g_device->SetSamplerState(0, D3DSAMP_ADDRESSV, D3DTADDRESS_WRAP);

    hr = g_device->CreateTexture(1, 1, 1, 0, D3DFMT_A8R8G8B8, D3DPOOL_DEFAULT, &g_whiteTexture, NULL);
    if (FAILED(hr))
        return hr;

    D3DLOCKED_RECT rect;
    if (SUCCEEDED(g_whiteTexture->LockRect(0, &rect, NULL, 0)))
    {
        *(DWORD*)rect.pBits = 0xffffffff;
        g_whiteTexture->UnlockRect(0);
    }

    return S_OK;
}

static IDirect3DTexture9* LoadUiTexture(const char* relativePath)
{
    const char* prefixes[] =
    {
        "game:\\",
        ".\\",
        "..\\..\\",
    };

    IDirect3DTexture9* texture = NULL;
    char path[512];
    for (int i = 0; i < (int)(sizeof(prefixes) / sizeof(prefixes[0])); ++i)
    {
        _snprintf(path, sizeof(path), "%s%s", prefixes[i], relativePath);
        path[sizeof(path) - 1] = 0;
        if (SUCCEEDED(D3DXCreateTextureFromFile(g_device, path, &texture)))
            return texture;
    }

    return NULL;
}

static void InitMenuAssets()
{
    g_menuBackTexture = LoadUiTexture("GFX\\menu\\back.jpg");
    g_menu173Texture = LoadUiTexture("GFX\\menu\\173back.jpg");
    g_loadingBackTexture = LoadUiTexture("Loadingscreens\\loadingback.jpg");
    g_loadingImageTexture = LoadUiTexture("Loadingscreens\\173.jpg");
}

static void InitModelAssets()
{
    g_modelAssetsLoaded = 0;
    g_modelAssetsFailed = 0;

    LoadXModel(g_modelDoorPanel, "Door01.x");
    LoadXModel(g_modelDoorFrame, "DoorFrame.x");
    LoadXModel(g_modelButton, "Button.x");
    LoadXModel(g_modelButtonKeycard, "ButtonKeycard.x");
    LoadXModel(g_modelButtonCode, "ButtonCode.x");
    LoadXModel(g_modelButtonScanner, "ButtonScanner.x");
    LoadXModel(g_modelHeavyDoor1, "heavydoor1.x");
    LoadXModel(g_modelHeavyDoor2, "heavydoor2.x");
    LoadXModel(g_modelContDoorLeft, "ContDoorLeft.x");
    LoadXModel(g_modelContDoorRight, "ContDoorRight.x");

    SetStatus("MODEL: %d LOADED %d FAILED", g_modelAssetsLoaded, g_modelAssetsFailed);
}

static void LoadIntroNpcAssets()
{
    if (g_introNpcAssetsAttempted)
        return;

    g_introNpcAssetsAttempted = true;
    LoadB3DModel(g_modelNpc173, "173_2.b3d");
    LoadB3DModel(g_modelNpcGuard, "guard.b3d");
    LoadB3DModel(g_modelNpcClassD, "classd.b3d");
    LoadB3DModel(g_modelNpcClerk, "clerk.b3d");

    SetStatus("INTRO MODELS: %d LOADED %d FAILED", g_modelAssetsLoaded, g_modelAssetsFailed);
}

static void InitAudio()
{
    if (FAILED(XAudio2Create(&g_audio, 0)))
        return;
    if (FAILED(g_audio->CreateMasteringVoice(&g_masterVoice)))
        return;

    const int sampleRate = 22050;
    const FLOAT duration = 0.08f;
    const FLOAT frequency = 440.0f;
    int samples = (int)(sampleRate * duration);
    g_beepPcm.resize((size_t)samples * 2);
    for (int i = 0; i < samples; ++i)
    {
        FLOAT t = (FLOAT)i / (FLOAT)sampleRate;
        FLOAT fade = 1.0f - ((FLOAT)i / (FLOAT)samples);
        SHORT sample = (SHORT)(sinf(t * frequency * XM_2PI) * fade * 1800.0f);
        g_beepPcm[(size_t)i * 2 + 0] = (BYTE)(sample & 0xff);
        g_beepPcm[(size_t)i * 2 + 1] = (BYTE)((sample >> 8) & 0xff);
    }

    WAVEFORMATEX wfx;
    ZeroMemory(&wfx, sizeof(wfx));
    wfx.wFormatTag = WAVE_FORMAT_PCM;
    wfx.nChannels = 1;
    wfx.nSamplesPerSec = sampleRate;
    wfx.wBitsPerSample = 16;
    wfx.nBlockAlign = (wfx.nChannels * wfx.wBitsPerSample) / 8;
    wfx.nAvgBytesPerSec = wfx.nSamplesPerSec * wfx.nBlockAlign;

    if (SUCCEEDED(g_audio->CreateSourceVoice(&g_beepVoice, &wfx)))
        g_beepVoice->SetVolume(0.12f);
}

static void PlayBeep()
{
    if (!g_beepVoice || g_beepPcm.empty())
        return;

    g_beepVoice->Stop(0);
    g_beepVoice->FlushSourceBuffers();

    XAUDIO2_BUFFER buffer;
    ZeroMemory(&buffer, sizeof(buffer));
    buffer.AudioBytes = (UINT32)g_beepPcm.size();
    buffer.pAudioData = &g_beepPcm[0];
    buffer.Flags = XAUDIO2_END_OF_STREAM;
    if (SUCCEEDED(g_beepVoice->SubmitSourceBuffer(&buffer)))
        g_beepVoice->Start(0);
}

static void BeginFacilityAfterIntro()
{
    if (g_introTransferred)
        return;

    g_introTransferred = true;
    PlayBeep();
    g_state = STATE_LOADING;
    g_loadingPercent = 0;
    CopyRoomName(g_loadingText, "BREACH HANDOFF");
    RenderLoadingScreen(g_loadingPercent, g_loadingText);
    LoadFacility();
    g_state = STATE_PLAYING;
    SetStatus("INTRO COMPLETE: FACILITY BREACH ACTIVE");
}

static void UpdateIntro(FLOAT dt, const XINPUT_STATE& state, DWORD buttons, FLOAT lx, FLOAT ly)
{
    g_introTimer += dt;

    bool allowMove = (g_introTimer >= 18.0f && g_introTimer < 96.0f) || g_introTimer > 112.0f;
    if (allowMove)
    {
        FLOAT speed = GetPlayerMoveSpeed(2.6f, 5.0f);
        XMVECTOR forward = XMVectorSet(sinf(g_yaw), 0.0f, cosf(g_yaw), 0.0f);
        XMVECTOR right = XMVectorSet(cosf(g_yaw), 0.0f, -sinf(g_yaw), 0.0f);
        g_playerPos += (forward * ly + right * lx) * (speed * dt);
        ResolvePlayerCollision();
        ResolveDoorCollision();
    }

    FLOAT px = XMVectorGetX(g_playerPos);
    FLOAT py = XMVectorGetY(g_playerPos);
    FLOAT pz = XMVectorGetZ(g_playerPos);

    if (g_introTimer < 15.0f)
    {
        px = -40.96f;
        pz = 1.92f;
    }
    else if (g_introTimer < 72.0f && px > 2.0f)
    {
        px = 2.0f;
    }
    else if (g_introTimer >= 72.0f && g_introTimer < 92.0f && px < 4.6f)
    {
        px = min(4.6f, px + dt * 1.4f);
    }

    g_playerPos = XMVectorSet(px, py, pz, 0.0f);

    if ((buttons & XINPUT_GAMEPAD_A) && !(g_lastButtons & XINPUT_GAMEPAD_A))
    {
        PlayBeep();
        XINPUT_VIBRATION vib = { 9000, 17000 };
        XInputSetState(0, &vib);
    }
    else if (!(buttons & XINPUT_GAMEPAD_A))
    {
        XINPUT_VIBRATION vib = { 0, 0 };
        XInputSetState(0, &vib);
    }

    if (g_introTimer < 6.0f)
        SetIntroText("D9341 WAKE UP", "RIGHT STICK LOOKS AROUND");
    else if (g_introTimer < 18.0f)
        SetIntroText("ULGRIN CELL DOOR OPENING", "WAIT FOR ESCORT");
    else if (g_introTimer < 35.0f)
        SetIntroText("FOLLOW THE GUARD TO SCP173", "LEFT STICK MOVES");
    else if (g_introTimer < 58.0f)
        SetIntroText("ESCORT ROUTE ACTIVE", "KEEP MOVING TOWARD THE CHAMBER");
    else if (g_introTimer < 72.0f)
        SetIntroText("ENTER THE CONTAINMENT CHAMBER", "WAIT FOR THE INNER DOOR");
    else if (g_introTimer < 86.0f)
        SetIntroText("CHAMBER DOOR CLOSING", "FRANKLIN BEGINS THE TEST");
    else if (g_introTimer < 96.0f)
        SetIntroText("PLEASE APPROACH SCP173", "MAINTAIN DIRECT EYE CONTACT");
    else if (g_introTimer < 104.0f)
        SetIntroText("POWER FAILURE", "SCP173 HAS MOVED");
    else if (g_introTimer < 112.0f)
        SetIntroText("SECOND SUBJECT DOWN", "SECURITY TEAM FIRING");
    else if (g_introTimer < 124.0f)
        SetIntroText("CONTAINMENT BREACH", "EVACUATE THE CHAMBER");
    else
        SetIntroText("LOADING FACILITY", "BREACH HANDOFF");

    g_introBlackout = 0.0f;
    if ((g_introTimer > 96.0f && g_introTimer < 101.0f) ||
        (g_introTimer > 103.0f && g_introTimer < 106.5f) ||
        (g_introTimer > 116.0f && g_introTimer < 119.5f))
    {
        g_introBlackout = (((INT)(g_introTimer * 12.0f) & 1) != 0) ? 0.85f : 0.20f;
    }
    else if (g_introTimer > 123.0f)
    {
        g_introBlackout = Range01(g_introTimer, 123.0f, 124.0f);
    }

    BuildIntroFrame();

    if (g_introTimer >= 124.0f)
        BeginFacilityAfterIntro();
}

static void Update(FLOAT dt)
{
    XINPUT_STATE state;
    ZeroMemory(&state, sizeof(state));
    DWORD buttons = 0;

    if (XInputGetState(0, &state) == ERROR_SUCCESS)
    {
        buttons = state.Gamepad.wButtons;
        if (g_state == STATE_MENU)
        {
            if ((buttons & (XINPUT_GAMEPAD_A | XINPUT_GAMEPAD_START)) &&
                !(g_lastButtons & (XINPUT_GAMEPAD_A | XINPUT_GAMEPAD_START)))
            {
                PlayBeep();
                g_state = STATE_LOADING;
                g_loadingPercent = 0;
                CopyRoomName(g_loadingText, "STARTING INTRO");
                RenderLoadingScreen(g_loadingPercent, g_loadingText);
                LoadIntroSequence();
                g_state = STATE_INTRO;
                ResetFrameCounter();
            }
            else if ((buttons & XINPUT_GAMEPAD_Y) && !(g_lastButtons & XINPUT_GAMEPAD_Y))
            {
                PlayBeep();
                g_state = STATE_LOADING;
                g_loadingPercent = 0;
                CopyRoomName(g_loadingText, "STARTING FACILITY");
                RenderLoadingScreen(g_loadingPercent, g_loadingText);
                LoadRoom();
                g_state = STATE_PLAYING;
                ResetFrameCounter();
            }
            g_lastButtons = buttons;
            return;
        }

        if (g_state == STATE_LOADING)
        {
            g_lastButtons = buttons;
            return;
        }

        if ((buttons & XINPUT_GAMEPAD_BACK) && !(g_lastButtons & XINPUT_GAMEPAD_BACK))
        {
            g_state = STATE_MENU;
            g_lastButtons = buttons;
            return;
        }

        UpdateNativeDoors(dt);

        FLOAT lx = ApplyDeadZone(state.Gamepad.sThumbLX, XINPUT_GAMEPAD_LEFT_THUMB_DEADZONE);
        FLOAT ly = ApplyDeadZone(state.Gamepad.sThumbLY, XINPUT_GAMEPAD_LEFT_THUMB_DEADZONE);
        FLOAT rx = ApplyDeadZone(state.Gamepad.sThumbRX, XINPUT_GAMEPAD_RIGHT_THUMB_DEADZONE);
        FLOAT ry = ApplyDeadZone(state.Gamepad.sThumbRY, XINPUT_GAMEPAD_RIGHT_THUMB_DEADZONE);

        g_yaw += rx * 2.6f * dt;
        g_pitch = Clamp(g_pitch - ry * 2.0f * dt, -1.25f, 1.25f);

        bool movementAllowed = g_state != STATE_INTRO ||
            ((g_introTimer >= 18.0f && g_introTimer < 96.0f) || g_introTimer > 112.0f);
        bool moving = movementAllowed && (fabsf(lx) > 0.05f || fabsf(ly) > 0.05f);
        bool sprintRequested = movementAllowed &&
            state.Gamepad.bLeftTrigger > XINPUT_GAMEPAD_TRIGGER_THRESHOLD;
        UpdatePlayerVitals(dt, buttons, moving, sprintRequested);

        if (g_state == STATE_INTRO)
        {
            UpdateIntro(dt, state, buttons, lx, ly);
        }
        else
        {
            FLOAT speed = GetPlayerMoveSpeed(3.5f, 7.0f);
            XMVECTOR forward = XMVectorSet(sinf(g_yaw), 0.0f, cosf(g_yaw), 0.0f);
            XMVECTOR right = XMVectorSet(cosf(g_yaw), 0.0f, -sinf(g_yaw), 0.0f);
            g_playerPos += (forward * ly + right * lx) * (speed * dt);
            ResolvePlayerCollision();
            ResolveDoorCollision();

            if ((buttons & XINPUT_GAMEPAD_A) && !(g_lastButtons & XINPUT_GAMEPAD_A))
            {
                if (!TryToggleNearestDoor())
                    PlayBeep();
                XINPUT_VIBRATION vib = { 15000, 24000 };
                XInputSetState(0, &vib);
            }
            else if (!(buttons & XINPUT_GAMEPAD_A))
            {
                XINPUT_VIBRATION vib = { 0, 0 };
                XInputSetState(0, &vib);
            }
        }
    }

    FLOAT y = XMVectorGetY(g_playerPos);
    if (y < 1.7f)
        g_playerPos = XMVectorSetY(g_playerPos, 1.7f);

    if (g_collisionFlash > 0.0f)
        g_collisionFlash = max(0.0f, g_collisionFlash - dt);

    g_lastButtons = buttons;
}

static void DrawVertices(const std::vector<Vertex>& verts, const XMMATRIX& wvp, bool depth, IDirect3DTexture9* texture)
{
    if (verts.empty())
        return;

    g_device->SetRenderState(D3DRS_ZENABLE, depth ? TRUE : FALSE);
    g_device->SetRenderState(D3DRS_CULLMODE, D3DCULL_NONE);
    g_device->SetVertexShader(g_vs);
    g_device->SetPixelShader(g_ps);
    g_device->SetVertexDeclaration(g_decl);
    g_device->SetVertexShaderConstantF(0, (FLOAT*)&wvp, 4);
    g_device->SetTexture(0, texture ? texture : g_whiteTexture);

    const size_t maxVerts = 6000;
    size_t pos = 0;
    while (pos + 2 < verts.size())
    {
        size_t count = verts.size() - pos;
        if (count > maxVerts)
            count = maxVerts - (maxVerts % 3);

        g_device->DrawPrimitiveUP(D3DPT_TRIANGLELIST, (UINT)(count / 3), &verts[pos], sizeof(Vertex));
        pos += count;
    }
}

static UINT DrawModelInstance(const IntroModelInstance& instance, const XMMATRIX& wvp)
{
    if (!instance.model || !instance.model->loaded)
        return 0;

    FLOAT radians = instance.yaw * (XM_PI / 180.0f);
    FLOAT c = cosf(radians);
    FLOAT s = sinf(radians);
    UINT tris = 0;

    const NativeModel& model = *instance.model;
    for (size_t surfaceIndex = 0; surfaceIndex < model.surfaces.size(); ++surfaceIndex)
    {
        const NativeModelSurface& source = model.surfaces[surfaceIndex];
        if (source.verts.empty())
            continue;

        g_modelDrawScratch.clear();
        if (g_modelDrawScratch.capacity() < source.verts.size())
            g_modelDrawScratch.reserve(source.verts.size());

        for (size_t i = 0; i < source.verts.size(); ++i)
        {
            const NativeModelVertex& mv = source.verts[i];
            FLOAT lx = mv.x * instance.sx;
            FLOAT ly = mv.y * instance.sy;
            FLOAT lz = mv.z * instance.sz;
            FLOAT nx = mv.nx * c + mv.nz * s;
            FLOAT nz = -mv.nx * s + mv.nz * c;

            Vertex v;
            v.x = lx * c + lz * s + instance.x;
            v.y = ly + instance.y;
            v.z = -lx * s + lz * c + instance.z;
            v.color = LitModelColor(instance.tint, nx, mv.ny, nz);
            v.u = mv.u;
            v.v = mv.v;
            g_modelDrawScratch.push_back(v);
        }

        tris += (UINT)(g_modelDrawScratch.size() / 3);
        DrawVertices(g_modelDrawScratch, wvp, true, source.texture ? source.texture : g_whiteTexture);
    }

    return tris;
}

static bool IsSurfaceVisibleNearPlayer(const RMeshSurface& surface)
{
    FLOAT px = XMVectorGetX(g_playerPos);
    FLOAT pz = XMVectorGetZ(g_playerPos);
    FLOAT closestX = Clamp(px, surface.minX, surface.maxX);
    FLOAT closestZ = Clamp(pz, surface.minZ, surface.maxZ);
    FLOAT dx = closestX - px;
    FLOAT dz = closestZ - pz;
    FLOAT limit = g_renderDistance;
    return (dx * dx + dz * dz) <= (limit * limit);
}

static bool IsIntroModelVisible(const IntroModelInstance& instance)
{
    FLOAT px = XMVectorGetX(g_playerPos);
    FLOAT pz = XMVectorGetZ(g_playerPos);
    FLOAT dx = instance.x - px;
    FLOAT dz = instance.z - pz;
    FLOAT distSq = dx * dx + dz * dz;
    FLOAT limit = min(g_renderDistance + 6.0f, 32.0f);
    if (distSq > limit * limit)
        return false;

    if (distSq > 36.0f)
    {
        FLOAT forwardX = sinf(g_yaw);
        FLOAT forwardZ = cosf(g_yaw);
        FLOAT facing = dx * forwardX + dz * forwardZ;
        if (facing < -3.0f)
            return false;
    }

    return true;
}

static const char** Glyph(char c)
{
    static const char* blank[] = { "000", "000", "000", "000", "000", "000", "000" };
    static const char* colon[] = { "0", "1", "1", "0", "1", "1", "0" };
    static const char* dot[] = { "0", "0", "0", "0", "0", "1", "1" };
    static const char* dash[] = { "000", "000", "000", "111", "000", "000", "000" };
    static const char* slash[] = { "001", "001", "010", "010", "100", "100", "000" };
    static const char* n0[] = { "111", "101", "101", "101", "101", "101", "111" };
    static const char* n1[] = { "010", "110", "010", "010", "010", "010", "111" };
    static const char* n2[] = { "111", "001", "001", "111", "100", "100", "111" };
    static const char* n3[] = { "111", "001", "001", "111", "001", "001", "111" };
    static const char* n4[] = { "101", "101", "101", "111", "001", "001", "001" };
    static const char* n5[] = { "111", "100", "100", "111", "001", "001", "111" };
    static const char* n6[] = { "111", "100", "100", "111", "101", "101", "111" };
    static const char* n7[] = { "111", "001", "001", "010", "010", "010", "010" };
    static const char* n8[] = { "111", "101", "101", "111", "101", "101", "111" };
    static const char* n9[] = { "111", "101", "101", "111", "001", "001", "111" };
    static const char* a[] = { "111", "101", "101", "111", "101", "101", "101" };
    static const char* b[] = { "110", "101", "101", "110", "101", "101", "110" };
    static const char* c0[] = { "111", "100", "100", "100", "100", "100", "111" };
    static const char* d[] = { "110", "101", "101", "101", "101", "101", "110" };
    static const char* e[] = { "111", "100", "100", "111", "100", "100", "111" };
    static const char* f[] = { "111", "100", "100", "111", "100", "100", "100" };
    static const char* g0[] = { "111", "100", "100", "101", "101", "101", "111" };
    static const char* h[] = { "101", "101", "101", "111", "101", "101", "101" };
    static const char* i[] = { "111", "010", "010", "010", "010", "010", "111" };
    static const char* j[] = { "001", "001", "001", "001", "101", "101", "111" };
    static const char* k[] = { "101", "101", "110", "100", "110", "101", "101" };
    static const char* l[] = { "100", "100", "100", "100", "100", "100", "111" };
    static const char* m[] = { "101", "111", "111", "101", "101", "101", "101" };
    static const char* n[] = { "101", "111", "111", "111", "101", "101", "101" };
    static const char* o[] = { "111", "101", "101", "101", "101", "101", "111" };
    static const char* p[] = { "111", "101", "101", "111", "100", "100", "100" };
    static const char* q[] = { "111", "101", "101", "101", "111", "001", "001" };
    static const char* r[] = { "110", "101", "101", "110", "101", "101", "101" };
    static const char* s[] = { "111", "100", "100", "111", "001", "001", "111" };
    static const char* t[] = { "111", "010", "010", "010", "010", "010", "010" };
    static const char* u[] = { "101", "101", "101", "101", "101", "101", "111" };
    static const char* v[] = { "101", "101", "101", "101", "101", "101", "010" };
    static const char* w[] = { "101", "101", "101", "101", "111", "111", "101" };
    static const char* x[] = { "101", "101", "101", "010", "101", "101", "101" };
    static const char* y[] = { "101", "101", "101", "010", "010", "010", "010" };
    static const char* z[] = { "111", "001", "001", "010", "100", "100", "111" };

    switch (c)
    {
    case ':': return colon; case '.': return dot; case '-': return dash; case '/': return slash;
    case '0': return n0; case '1': return n1; case '2': return n2; case '3': return n3; case '4': return n4;
    case '5': return n5; case '6': return n6; case '7': return n7; case '8': return n8; case '9': return n9;
    case 'A': return a; case 'B': return b; case 'C': return c0; case 'D': return d; case 'E': return e;
    case 'F': return f; case 'G': return g0; case 'H': return h; case 'I': return i; case 'J': return j;
    case 'K': return k; case 'L': return l; case 'M': return m; case 'N': return n; case 'O': return o;
    case 'P': return p; case 'Q': return q; case 'R': return r; case 'S': return s; case 'T': return t;
    case 'U': return u; case 'V': return v; case 'W': return w; case 'X': return x; case 'Y': return y; case 'Z': return z;
    default: return blank;
    }
}

static void DrawText(std::vector<Vertex>& out, const char* text, FLOAT x, FLOAT y, FLOAT scale, DWORD color)
{
    FLOAT cursor = x;
    for (const char* p = text; *p; ++p)
    {
        char ch = *p;
        if (ch >= 'a' && ch <= 'z')
            ch = (char)(ch - 'a' + 'A');

        const char** glyph = Glyph(ch);
        int width = (int)strlen(glyph[0]);
        for (int row = 0; row < 7; ++row)
        {
            for (int col = 0; col < width; ++col)
            {
                if (glyph[row][col] != '1')
                    continue;
                FLOAT x0 = cursor + col * scale;
                FLOAT y0 = y + row * scale;
                FLOAT x1 = x0 + scale;
                FLOAT y1 = y0 + scale;
                AddQuad(out, XMVectorSet(x0, y0, 0.0f, 0), XMVectorSet(x1, y0, 0.0f, 0),
                    XMVectorSet(x1, y1, 0.0f, 0), XMVectorSet(x0, y1, 0.0f, 0), color);
            }
        }
        cursor += (FLOAT)(width + 1) * scale;
    }
}

static void DrawTextCentered(std::vector<Vertex>& out, const char* text, FLOAT centerX, FLOAT y, FLOAT scale, DWORD color)
{
    FLOAT width = 0.0f;
    for (const char* p = text; *p; ++p)
    {
        char ch = *p;
        if (ch >= 'a' && ch <= 'z')
            ch = (char)(ch - 'a' + 'A');
        const char** glyph = Glyph(ch);
        width += (FLOAT)((int)strlen(glyph[0]) + 1) * scale;
    }
    DrawText(out, text, centerX - width * 0.5f, y, scale, color);
}

static void DrawScreenQuad(IDirect3DTexture9* texture, FLOAT x, FLOAT y, FLOAT w, FLOAT h, DWORD color)
{
    g_screenQuadScratch.clear();
    AddQuad(g_screenQuadScratch, XMVectorSet(x, y, 0.0f, 0), XMVectorSet(x + w, y, 0.0f, 0),
        XMVectorSet(x + w, y + h, 0.0f, 0), XMVectorSet(x, y + h, 0.0f, 0), color);
    XMMATRIX ortho = XMMatrixOrthographicOffCenterLH(0.0f, 1280.0f, 720.0f, 0.0f, 0.0f, 1.0f);
    DrawVertices(g_screenQuadScratch, ortho, false, texture ? texture : g_whiteTexture);
}

static void DrawScreenRect(FLOAT x, FLOAT y, FLOAT w, FLOAT h, DWORD color)
{
    DrawScreenQuad(g_whiteTexture, x, y, w, h, color);
}

static void AddHudRect(std::vector<Vertex>& out, FLOAT x, FLOAT y, FLOAT w, FLOAT h, DWORD color)
{
    AddQuad(out, XMVectorSet(x, y, 0.0f, 0), XMVectorSet(x + w, y, 0.0f, 0),
        XMVectorSet(x + w, y + h, 0.0f, 0), XMVectorSet(x, y + h, 0.0f, 0), color);
}

static void DrawMeterBar(std::vector<Vertex>& hud, const char* label, FLOAT x, FLOAT y, FLOAT ratio, DWORD fill)
{
    ratio = Clamp(ratio, 0.0f, 1.0f);
    AddHudRect(hud, x - 2.0f, y - 2.0f, 208.0f, 24.0f, D3DCOLOR_XRGB(230, 230, 220));
    AddHudRect(hud, x, y, 204.0f, 20.0f, D3DCOLOR_ARGB(215, 8, 9, 10));
    AddHudRect(hud, x + 3.0f, y + 3.0f, 198.0f * ratio, 14.0f, fill);
    AddHudRect(hud, x - 50.0f, y - 4.0f, 34.0f, 34.0f, D3DCOLOR_ARGB(220, 0, 0, 0));
    AddHudRect(hud, x - 51.0f, y - 5.0f, 36.0f, 36.0f, D3DCOLOR_ARGB(80, 255, 255, 255));
    DrawText(hud, label, x - 45.0f, y + 7.0f, 2.0f, D3DCOLOR_XRGB(235, 235, 225));
}

static void DrawPlayerVitalsHud(std::vector<Vertex>& hud)
{
    FLOAT blinkRatio = g_blinkTimer > 0.0f ? g_blinkTimer / max(g_blinkFrequency, 0.01f) : 0.0f;
    FLOAT staminaRatio = Clamp(g_stamina / 100.0f, 0.0f, 1.0f);
    DWORD staminaColor = g_stamina < 25.0f ? D3DCOLOR_XRGB(210, 85, 55) : D3DCOLOR_XRGB(110, 195, 120);

    DrawMeterBar(hud, "EYE", 82.0f, 614.0f, blinkRatio, D3DCOLOR_XRGB(175, 205, 230));
    DrawMeterBar(hud, g_crouch ? "LOW" : "RUN", 82.0f, 654.0f, staminaRatio, staminaColor);

    char line[96];
    _snprintf(line, sizeof(line), "STAMINA: %.0f  BLINK: %.1f  STEP: %d",
        max(g_stamina, 0.0f), max(g_blinkTimer, 0.0f), g_stepCueCount);
    DrawText(hud, line, 314.0f, 620.0f, 2.0f, D3DCOLOR_XRGB(180, 190, 190));
    DrawText(hud, g_crouch ? "B: STAND  RB: BLINK  LT: SPRINT" : "B: CROUCH  RB: BLINK  LT: SPRINT",
        314.0f, 650.0f, 2.0f, D3DCOLOR_XRGB(150, 160, 170));
}

static void RenderMainMenu()
{
    g_device->Clear(0L, NULL, D3DCLEAR_TARGET | D3DCLEAR_ZBUFFER | D3DCLEAR_STENCIL, 0xff030305, 1.0f, 0L);

    if (g_menuBackTexture)
        DrawScreenQuad(g_menuBackTexture, 0.0f, 0.0f, 1280.0f, 720.0f, 0xffffffff);
    else
        DrawScreenRect(0.0f, 0.0f, 1280.0f, 720.0f, D3DCOLOR_XRGB(8, 8, 10));

    if (g_menu173Texture)
        DrawScreenQuad(g_menu173Texture, 760.0f, 0.0f, 520.0f, 720.0f, 0xddffffff);

    DrawScreenRect(0.0f, 0.0f, 1280.0f, 720.0f, D3DCOLOR_ARGB(80, 0, 0, 0));

    std::vector<Vertex> hud;
    DrawText(hud, "SCP CONTAINMENT BREACH", 70, 90, 7, D3DCOLOR_XRGB(235, 235, 230));
    DrawText(hud, "SCPCB360", 74, 155, 4, D3DCOLOR_XRGB(170, 190, 210));
    DrawText(hud, "NEW GAME", 90, 275, 5, D3DCOLOR_XRGB(240, 240, 220));
    DrawText(hud, "A OR START INTRO", 90, 330, 3, D3DCOLOR_XRGB(190, 210, 190));
    DrawText(hud, "Y FACILITY TECH DEMO", 90, 370, 3, D3DCOLOR_XRGB(170, 190, 210));
    DrawText(hud, "INTRO SEQUENCE PORT PASS", 90, 430, 2, D3DCOLOR_XRGB(130, 140, 150));
    DrawText(hud, "BACK RETURNS HERE IN GAME", 90, 460, 2, D3DCOLOR_XRGB(130, 140, 150));

    XMMATRIX ortho = XMMatrixOrthographicOffCenterLH(0.0f, 1280.0f, 720.0f, 0.0f, 0.0f, 1.0f);
    DrawVertices(hud, ortho, false, g_whiteTexture);
    g_device->Present(NULL, NULL, NULL, NULL);
}

static void RenderLoadingScreen(INT percent, const char* detail)
{
    percent = ClampInt(percent, 0, 100);
    g_loadingPercent = percent;
    if (detail)
        CopyRoomName(g_loadingText, detail);

    g_device->Clear(0L, NULL, D3DCLEAR_TARGET | D3DCLEAR_ZBUFFER | D3DCLEAR_STENCIL, 0xff050506, 1.0f, 0L);

    if (g_loadingBackTexture)
        DrawScreenQuad(g_loadingBackTexture, 0.0f, 0.0f, 1280.0f, 720.0f, 0xffffffff);
    else
        DrawScreenRect(0.0f, 0.0f, 1280.0f, 720.0f, D3DCOLOR_XRGB(5, 5, 6));

    if (g_loadingImageTexture)
        DrawScreenQuad(g_loadingImageTexture, 850.0f, 90.0f, 330.0f, 420.0f, 0xddffffff);

    DrawScreenRect(0.0f, 0.0f, 1280.0f, 720.0f, D3DCOLOR_ARGB(95, 0, 0, 0));

    FLOAT barX = 260.0f;
    FLOAT barY = 590.0f;
    FLOAT barW = 760.0f;
    FLOAT barH = 24.0f;
    DrawScreenRect(barX - 2.0f, barY - 2.0f, barW + 4.0f, barH + 4.0f, D3DCOLOR_XRGB(150, 150, 150));
    DrawScreenRect(barX, barY, barW, barH, D3DCOLOR_XRGB(28, 30, 34));
    DrawScreenRect(barX, barY, barW * ((FLOAT)percent / 100.0f), barH, D3DCOLOR_XRGB(160, 190, 210));

    char line[128];
    std::vector<Vertex> hud;
    DrawTextCentered(hud, "LOADING", 640.0f, 120.0f, 7, D3DCOLOR_XRGB(235, 235, 230));
    DrawTextCentered(hud, g_loadingText, 640.0f, 210.0f, 3, D3DCOLOR_XRGB(190, 210, 225));
    DrawTextCentered(hud, "SCPCB360 NATIVE XEX", 640.0f, 255.0f, 2, D3DCOLOR_XRGB(145, 155, 165));
    _snprintf(line, sizeof(line), "%d", percent);
    DrawTextCentered(hud, line, 640.0f, 630.0f, 3, D3DCOLOR_XRGB(220, 220, 210));

    XMMATRIX ortho = XMMatrixOrthographicOffCenterLH(0.0f, 1280.0f, 720.0f, 0.0f, 0.0f, 1.0f);
    DrawVertices(hud, ortho, false, g_whiteTexture);
    g_device->Present(NULL, NULL, NULL, NULL);
}

static void Render()
{
    if (g_state == STATE_MENU)
    {
        RenderMainMenu();
        return;
    }
    if (g_state == STATE_LOADING)
    {
        RenderLoadingScreen(g_loadingPercent, g_loadingText);
        return;
    }

    g_device->Clear(0L, NULL, D3DCLEAR_TARGET | D3DCLEAR_ZBUFFER | D3DCLEAR_STENCIL, 0xff09090c, 1.0f, 0L);

    XMVECTOR eye = GetCameraEye();
    XMVECTOR look = XMVectorSet(cosf(g_pitch) * sinf(g_yaw), sinf(g_pitch), cosf(g_pitch) * cosf(g_yaw), 0.0f);
    XMMATRIX view = XMMatrixLookAtLH(eye, eye + look, XMVectorSet(0, 1, 0, 0));
    XMMATRIX proj = XMMatrixPerspectiveFovLH(XM_PIDIV4, 1280.0f / 720.0f, 0.05f, 500.0f);
    XMMATRIX wvp = view * proj;
    g_surfacesDrawn = 0;
    g_surfacesCulled = 0;
    g_modelInstancesDrawn = 0;
    g_modelInstancesCulled = 0;
    g_worldTrisSubmitted = 0;

    if (!g_roomSurfaces.empty())
    {
        for (size_t i = 0; i < g_roomSurfaces.size(); ++i)
        {
            const RMeshSurface& surface = g_roomSurfaces[i];
            if (!IsSurfaceVisibleNearPlayer(surface))
            {
                ++g_surfacesCulled;
                continue;
            }
            ++g_surfacesDrawn;
            g_worldTrisSubmitted += (UINT)(surface.verts.size() / 3);
            DrawVertices(surface.verts, wvp, true, surface.texture);
        }
    }
    else
    {
        g_surfacesDrawn = g_roomVerts.empty() ? 0 : 1;
        g_worldTrisSubmitted = (UINT)(g_roomVerts.size() / 3);
        DrawVertices(g_roomVerts, wvp, true, g_whiteTexture);
    }

    if (!g_doorVerts.empty())
    {
        g_worldTrisSubmitted += (UINT)(g_doorVerts.size() / 3);
        DrawVertices(g_doorVerts, wvp, true, g_whiteTexture);
    }
    for (size_t i = 0; i < g_doorSurfaces.size(); ++i)
    {
        g_worldTrisSubmitted += (UINT)(g_doorSurfaces[i].verts.size() / 3);
        DrawVertices(g_doorSurfaces[i].verts, wvp, true, g_doorSurfaces[i].texture);
    }

    if (g_state == STATE_INTRO)
    {
        g_worldTrisSubmitted += (UINT)(g_scriptedVerts.size() / 3);
        DrawVertices(g_scriptedVerts, wvp, true, g_whiteTexture);
        for (size_t i = 0; i < g_scriptedSurfaces.size(); ++i)
        {
            g_worldTrisSubmitted += (UINT)(g_scriptedSurfaces[i].verts.size() / 3);
            DrawVertices(g_scriptedSurfaces[i].verts, wvp, true, g_scriptedSurfaces[i].texture);
        }
        for (size_t i = 0; i < g_introModelInstances.size(); ++i)
        {
            if (!IsIntroModelVisible(g_introModelInstances[i]))
            {
                ++g_modelInstancesCulled;
                continue;
            }
            ++g_modelInstancesDrawn;
            g_worldTrisSubmitted += DrawModelInstance(g_introModelInstances[i], wvp);
        }
        if (g_introBlackout > 0.0f)
        {
            INT alpha = (INT)(Clamp(g_introBlackout, 0.0f, 1.0f) * 255.0f);
            DrawScreenRect(0.0f, 0.0f, 1280.0f, 720.0f, D3DCOLOR_ARGB(alpha, 0, 0, 0));
        }
        DrawScreenRect(0.0f, 608.0f, 1280.0f, 112.0f, D3DCOLOR_ARGB(120, 0, 0, 0));
    }

    DrawScreenRect(0.0f, 0.0f, 1280.0f, 720.0f, D3DCOLOR_ARGB(g_state == STATE_INTRO ? 18 : 28, 0, 0, 0));

    if (g_blinkAlpha > 0.0f)
    {
        INT alpha = (INT)(Clamp(g_blinkAlpha, 0.0f, 1.0f) * 245.0f);
        DrawScreenRect(0.0f, 0.0f, 1280.0f, 720.0f, D3DCOLOR_ARGB(alpha, 0, 0, 0));
    }

    char line[128];
    std::vector<Vertex> hud;
    DrawText(hud, g_state == STATE_INTRO ? "SCPCB360 INTRO SEQUENCE" : "SCPCB360 TECH DEMO XEX", 18, 18, 4, D3DCOLOR_XRGB(220, 235, 255));
    _snprintf(line, sizeof(line), "FPS: %.1f  MS: %.1f", g_fps, g_frameMs);
    DrawText(hud, line, 18, 54, 3, D3DCOLOR_XRGB(170, 220, 170));
    _snprintf(line, sizeof(line), "POS: X %.2f Y %.2f Z %.2f", XMVectorGetX(g_playerPos), XMVectorGetY(g_playerPos), XMVectorGetZ(g_playerPos));
    DrawText(hud, line, 18, 82, 3, D3DCOLOR_XRGB(220, 220, 190));
    DrawText(hud, g_status, 18, 110, 2, D3DCOLOR_XRGB(180, 190, 210));
    _snprintf(line, sizeof(line), g_collisionFlash > 0.0f ? "COL: HIT %d" : "COL: READY", g_collisionCount);
    DrawText(hud, line, 18, 132, 2, g_collisionFlash > 0.0f ? D3DCOLOR_XRGB(255, 170, 120) : D3DCOLOR_XRGB(150, 180, 150));
    _snprintf(line, sizeof(line), "TEX: %d/%d", g_texturesLoaded, g_texturesFailed);
    DrawText(hud, line, 18, 154, 2, g_texturesFailed > 0 ? D3DCOLOR_XRGB(255, 190, 130) : D3DCOLOR_XRGB(150, 180, 150));
    _snprintf(line, sizeof(line), "MAP: %d BAD", g_facilityLoadFailures);
    DrawText(hud, line, 18, 176, 2, g_facilityLoadFailures > 0 ? D3DCOLOR_XRGB(255, 190, 130) : D3DCOLOR_XRGB(150, 180, 150));
    _snprintf(line, sizeof(line), "DIST: %.0f  SURF: %d/%d  TRI: %u", g_renderDistance, g_surfacesDrawn, g_surfacesDrawn + g_surfacesCulled, g_worldTrisSubmitted);
    DrawText(hud, line, 18, 198, 2, D3DCOLOR_XRGB(150, 180, 150));
    _snprintf(line, sizeof(line), "DOORS: %u  USES: %d", (unsigned)g_doors.size(), g_doorToggleCount);
    DrawText(hud, line, 18, 220, 2, D3DCOLOR_XRGB(150, 180, 150));
    _snprintf(line, sizeof(line), "MODEL: %d/%d INST: %d/%d", g_modelAssetsLoaded, g_modelAssetsFailed,
        g_modelInstancesDrawn, g_modelInstancesDrawn + g_modelInstancesCulled);
    DrawText(hud, line, 18, 242, 2, g_modelAssetsFailed > 0 ? D3DCOLOR_XRGB(255, 190, 130) : D3DCOLOR_XRGB(150, 180, 150));
    DrawText(hud, g_state == STATE_INTRO ? "A: CUE/DOOR  LS: MOVE  RS: LOOK" : "A: BEEP/DOOR  LS: MOVE  RS: LOOK", 18, 264, 2, D3DCOLOR_XRGB(150, 160, 170));
    DrawPlayerVitalsHud(hud);
    if (g_state == STATE_INTRO)
    {
        _snprintf(line, sizeof(line), "INTRO: %.1f", g_introTimer);
        DrawText(hud, line, 18, 286, 2, D3DCOLOR_XRGB(150, 160, 170));
        DrawTextCentered(hud, g_introSubtitle, 640.0f, 626.0f, 3, D3DCOLOR_XRGB(235, 235, 225));
        DrawTextCentered(hud, g_introSubtitle2, 640.0f, 660.0f, 2, D3DCOLOR_XRGB(185, 205, 225));
    }

    XMMATRIX ortho = XMMatrixOrthographicOffCenterLH(0.0f, 1280.0f, 720.0f, 0.0f, 0.0f, 1.0f);
    DrawVertices(hud, ortho, false, g_whiteTexture);

    g_device->Present(NULL, NULL, NULL, NULL);
}

VOID __cdecl main()
{
    QueryPerformanceFrequency(&g_freq);

    if (FAILED(InitD3D()))
    {
        OutputDebugStringA("SCPCB360 native: D3D init failed\n");
        return;
    }

    InitAudio();
    InitMenuAssets();
    InitModelAssets();
    ResetFrameCounter();

    for (;;)
    {
        LARGE_INTEGER now;
        QueryPerformanceCounter(&now);
        FLOAT rawDt = g_freq.QuadPart > 0 ?
            (FLOAT)(now.QuadPart - g_lastTime.QuadPart) / (FLOAT)g_freq.QuadPart : 0.0f;
        g_lastTime = now;
        UpdateFrameCounter(rawDt);

        FLOAT simDt = Clamp(rawDt, 0.0f, 0.1f);
        Update(simDt);
        Render();
    }
}
