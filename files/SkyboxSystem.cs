// SkyboxSystem.cs — ports Skybox.bb

using SCPCB360.Engine;
using static SCPCB360.Engine.B3D;

namespace SCPCB360.GameLogic
{
    public static class SkyboxSystem
    {
        private static int _skybox = -1;

        public static void Initialize()
        {
            _skybox = LoadMesh("GFX/skybox");
            if (_skybox != -1)
            {
                ScaleEntity(_skybox, 1000f, 1000f, 1000f);
                EntityType(_skybox, 0);
            }
        }

        public static void Update(int cameraEnt)
        {
            if (_skybox == -1 || cameraEnt == -1) return;

            PositionEntity(_skybox,
                EntityX(cameraEnt, true),
                EntityY(cameraEnt, true),
                EntityZ(cameraEnt, true),
                true);
        }

        public static void Free()
        {
            if (_skybox != -1)
            {
                FreeEntity(_skybox);
                _skybox = -1;
            }
        }
    }
}