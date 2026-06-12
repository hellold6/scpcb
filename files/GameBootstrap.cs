// GameBootstrap.cs — ports InitNewGame / InitLoadGame from Main.bb

using System;
using SCPCB360.Engine;
using static SCPCB360.Engine.B3D;

namespace SCPCB360.GameLogic
{
    public static class GameBootstrap
    {
        public static void InitNewGame()
        {
            GameState.ResetForNewGame();
            MenuSystem.LoadingProgress = 0.1f;

            MapSystem.FreeAllRooms();
            DoorSystem.FreeAll();
            ItemSystem.FreeAll();
            NPCSystem.FreeAll();
            EventSystem.FreeAll();
            ParticleSystem.FreeAll();
            DecalSystem.FreeAll();
            SecurityCamSystem.FreeAll();
            MusicSystem.StopAll();
            DevilParticleSystem.FreeParticles();

            if (!string.IsNullOrEmpty(GameState.RandomSeed))
                SeedRnd(MathUtil.GenerateSeedNumber(GameState.RandomSeed));

            MenuSystem.LoadingProgress = 0.3f;

            if (string.IsNullOrEmpty(GameState.SelectedMap))
                MapSystem.CreateMap();
            else
                MapSystem.LoadMap("Map Creator/Maps/" + GameState.SelectedMap);

            MapSystem.InitWayPoints();
            MenuSystem.LoadingProgress = 0.6f;

            ItemSystem.InitTemplates();

            var curr173 = NPCSystem.CreateNpc(NPCSystem.NpcType173, 0, -30f, 0);
            var curr106 = NPCSystem.CreateNpc(NPCSystem.NpcTypeOldMan, 0, -30f, 0);
            curr106.State = 70f * 60f * Rand(12, 17);

            foreach (var d in DoorSystem.All)
            {
                EntityParent(d.Obj, 0);
                if (d.Obj2 != -1) EntityParent(d.Obj2, 0);
                if (d.FrameObj != -1) EntityParent(d.FrameObj, 0);
            }

            MenuSystem.LoadingProgress = 0.75f;

            PlacePlayerInStartRoom();
            EventSystem.InitEvents();

            foreach (var e in EventSystem.All)
            {
                if (e.EventName == "room2nuke") e.EventState = 1f;
                if (e.EventName == "room106") e.EventState2 = 1f;
                if (e.EventName == "room2sl") e.EventState3 = 1f;
            }

            GameState.BlinkTimer = -10f;
            GameState.BlurTimer = 100;
            GameState.Stamina = 100f;
            GameState.Playable = true;
            GameState.Screen = GameScreen.Playing;
            MenuSystem.LoadingProgress = 1f;

            DevilParticleSystem.InitParticles(GameState.Camera);
            PlayerSystem.ResetEntity();
        }

        public static void InitLoadGame(string saveFolder = null)
        {
            saveFolder ??= SaveSystem.SavePath + (SaveSystem.SaveGames.Count > 0 ? SaveSystem.SaveGames[0] : "") + "/";

            if (string.IsNullOrWhiteSpace(saveFolder) || !System.IO.Directory.Exists(saveFolder.TrimEnd('/','\\')))
            {
                GameState.Screen = GameScreen.MainMenu;
                return;
            }

            GameState.ResetForNewGame();
            MenuSystem.LoadingProgress = 0.2f;

            MapSystem.FreeAllRooms();
            DoorSystem.FreeAll();
            ItemSystem.FreeAll();
            NPCSystem.FreeAll();
            EventSystem.FreeAll();
            ParticleSystem.FreeAll();
            DecalSystem.FreeAll();
            SecurityCamSystem.FreeAll();
            MusicSystem.StopAll();
            DevilParticleSystem.FreeParticles();

            ItemSystem.InitTemplates();
            MenuSystem.LoadingProgress = 0.5f;

            if (!SaveSystem.LoadGame(saveFolder))
            {
                GameState.Screen = GameScreen.MainMenu;
                return;
            }

            MenuSystem.LoadingProgress = 0.9f;
            DevilParticleSystem.InitParticles(GameState.Camera);
            PlayerSystem.ResetEntity();

            GameState.Screen = GameScreen.Playing;
            GameState.Playable = true;
            MenuSystem.LoadingProgress = 1f;
        }

        private static void PlacePlayerInStartRoom()
        {
            foreach (var room in MapSystem.All)
            {
                if (room.RoomName == "start" && !MenuSystem.IntroEnabled)
                {
                    PositionEntity(GameState.Collider,
                        room.x + 3584f * GameState.RoomScale,
                        704f * GameState.RoomScale,
                        room.z + 1024f * GameState.RoomScale,
                        true);
                    GameState.PlayerRoom = room;
                    return;
                }

                if (room.RoomName == "173" && MenuSystem.IntroEnabled)
                {
                    PositionEntity(GameState.Collider, room.x, 1f, room.z, true);
                    GameState.PlayerRoom = room;
                    return;
                }
            }

            if (MapSystem.All.Count > 0)
            {
                var first = MapSystem.All[0];
                PositionEntity(GameState.Collider, first.x, 1f, first.z, true);
                GameState.PlayerRoom = first;
            }
        }
    }
}