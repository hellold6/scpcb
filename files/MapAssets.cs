// MapAssets.cs — shared mesh/texture handles used by FillRoom

using static SCPCB360.Engine.B3D;

namespace SCPCB360.GameLogic
{
    public static class MapAssets
    {
        public static int LeverBaseObj = -1;
        public static int LeverObj = -1;
        public static int Monitor = -1;
        public static int Monitor2 = -1;
        public static int Monitor3 = -1;
        public static int ButtonObj = -1;
        public static int DoorObj = -1;
        public static int CamBaseObj = -1;
        public static int TeslaTexture = -1;
        private static readonly int[] _lightSprites = new int[4];

        public static void Initialize()
        {
            LeverBaseObj = LoadMesh("GFX/map/leverbase");
            LeverObj = LoadMesh("GFX/map/leverhandle");
            Monitor = LoadMesh("GFX/map/monitor");
            Monitor2 = CopyEntity(Monitor);
            Monitor3 = CopyEntity(Monitor);
            ButtonObj = LoadMesh("GFX/map/Button");
            DoorObj = LoadMesh("GFX/map/door01");
            CamBaseObj = LoadMesh("GFX/map/cambase");
            TeslaTexture = -1; // texture handle stub
        }

        public static int LightSpriteTex(int index)
        {
            if (index < 0 || index >= _lightSprites.Length) return -1;
            return _lightSprites[index];
        }

        public static float MeshWidth(int entity) => 1f;
        public static float MeshHeight(int entity) => 1f;
        public static float MeshDepth(int entity) => 1f;
    }

    public static class MapGlobals
    {
        public static bool Room2GwBrokenDoor;
        public static float Room2GwX;
        public static float Room2GwZ;
    }

    public static class ZoneInfo
    {
        public static bool HasCustomForest;
    }
}