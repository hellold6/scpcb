// AudioSystem.cs — ports LoadAllSounds.bb + 3D sound helpers from Main.bb

using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using SCPCB360.Engine;

namespace SCPCB360.GameLogic
{
    public class LoopingSound
    {
        public SoundEffectInstance Instance;
        public int EntityEnt = -1;
        public float Range = 10f;
        public float Volume = 1f;
        public bool Active;
    }

    public static class AudioSystem
    {
        private static ContentManager _content;
        private static readonly Dictionary<string, SoundEffect> _cache = new();
        private static readonly Dictionary<int, LoopingSound> _loops = new();
        private static int _nextLoopId = 1;
        private static readonly SoundEffect[,] _openDoorSfx = new SoundEffect[4, 3];
        private static readonly SoundEffect[,] _closeDoorSfx = new SoundEffect[4, 3];
        private static SoundEffect _keyCardSfx1;
        private static SoundEffect _keyCardSfx2;
        private static SoundEffect _buttonSfx;
        private static SoundEffect _stoneDragSfx;
        private static readonly SoundEffect[] _pickSfx = new SoundEffect[12];

        public static void Initialize(ContentManager content)
        {
            _content = content;
            LoadAllSounds();
        }

        public static void LoadAllSounds()
        {
            for (int i = 0; i < 3; i++)
            {
                _openDoorSfx[0, i] = Load("SFX/Door/DoorOpen" + (i + 1));
                _closeDoorSfx[0, i] = Load("SFX/Door/DoorClose" + (i + 1));
                _openDoorSfx[2, i] = Load("SFX/Door/Door2Open" + (i + 1));
                _closeDoorSfx[2, i] = Load("SFX/Door/Door2Close" + (i + 1));
                _openDoorSfx[3, i] = Load("SFX/Door/ElevatorOpen" + (i + 1));
                _closeDoorSfx[3, i] = Load("SFX/Door/ElevatorClose" + (i + 1));
            }

            for (int i = 0; i < 2; i++)
            {
                _openDoorSfx[1, i] = Load("SFX/Door/BigDoorOpen" + (i + 1));
                _closeDoorSfx[1, i] = Load("SFX/Door/BigDoorClose" + (i + 1));
            }

            _keyCardSfx1 = Load("SFX/Interact/KeyCardUse1");
            _keyCardSfx2 = Load("SFX/Interact/KeyCardUse2");
            _buttonSfx = Load("SFX/Interact/Button2");
            _stoneDragSfx = Load("SFX/SCP/173/StoneDrag");

            for (int i = 0; i < _pickSfx.Length; i++)
                _pickSfx[i] = Load("SFX/Interact/PickItem" + (i + 1));
        }

        public static SoundEffect Load(string path)
        {
            if (_cache.TryGetValue(path, out var cached))
                return cached;

            try
            {
                var sfx = _content.Load<SoundEffect>(path);
                _cache[path] = sfx;
                return sfx;
            }
            catch
            {
                return null;
            }
        }

        public static void PlayDoorSound(bool opening, int dir)
        {
            int d = System.Math.Clamp(dir, 0, 3);
            var bank = opening ? _openDoorSfx : _closeDoorSfx;
            var sfx = bank[d, 0] ?? bank[0, 0];
            sfx?.Play(GameState.SfxVolume, 0f, 0f);
        }

        public static void PlayInteractSound()
            => _buttonSfx?.Play(GameState.SfxVolume * 0.8f, 0f, 0f);

        public static void PlayPickSound(int index)
        {
            if (index == 66) return;
            int i = System.Math.Clamp(index, 1, _pickSfx.Length) - 1;
            _pickSfx[i]?.Play(GameState.SfxVolume, 0f, 0f);
        }

        public static void PlayKeyCardSound(bool success)
            => (success ? _keyCardSfx1 : _keyCardSfx2)?.Play(GameState.SfxVolume, 0f, 0f);

        public static void PlaySound2(SoundEffect sfx, int camEnt, int entityEnt, float range = 10f, float volume = 1f)
        {
            if (sfx == null) return;

            float vol = Compute3DVolume(camEnt, entityEnt, range) * volume * GameState.SfxVolume;
            if (vol > 0.01f)
                sfx.Play(vol, 0f, ComputePan(camEnt, entityEnt));
        }

        public static int LoopSound2(SoundEffect sfx, int chn, int camEnt, int entityEnt,
            float range = 10f, float volume = 1f)
        {
            if (sfx == null) return chn;

            LoopingSound loop;
            if (chn == 0 || !_loops.TryGetValue(chn, out loop) || !loop.Active)
            {
                chn = _nextLoopId++;
                loop = new LoopingSound();
                _loops[chn] = loop;
                loop.Instance = sfx.CreateInstance();
                loop.Instance.IsLooped = true;
                loop.Instance.Play();
                loop.Active = true;
            }

            loop.EntityEnt = entityEnt;
            loop.Range = System.Math.Max(range, 1f);
            loop.Volume = volume;
            ApplyLoopVolume(loop, camEnt);
            return chn;
        }

        public static void UpdateSoundOrigin(int chn, int camEnt, int entityEnt, float range = 10f, float volume = 1f)
        {
            if (chn == 0 || !_loops.TryGetValue(chn, out var loop) || !loop.Active)
                return;

            loop.EntityEnt = entityEnt;
            loop.Range = System.Math.Max(range, 1f);
            loop.Volume = volume;
            ApplyLoopVolume(loop, camEnt);
        }

        private static void ApplyLoopVolume(LoopingSound loop, int camEnt)
        {
            if (loop.Instance == null) return;

            if (loop.Volume <= 0f)
            {
                loop.Instance.Volume = 0f;
                return;
            }

            float vol = Compute3DVolume(camEnt, loop.EntityEnt, loop.Range) * loop.Volume * GameState.SfxVolume;
            loop.Instance.Volume = vol;
            loop.Instance.Pan = ComputePan(camEnt, loop.EntityEnt);
        }

        public static float Compute3DVolume(int camEnt, int entityEnt, float range)
        {
            float dist = B3D.EntityDistance(camEnt, entityEnt);
            if (dist >= range) return 0f;
            return 1f - dist / range;
        }

        private static float ComputePan(int camEnt, int entityEnt)
        {
            float yaw = MathUtil.DeltaYaw(camEnt, entityEnt);
            return (float)System.Math.Sin(-yaw * System.Math.PI / 180.0);
        }

        public static void UpdateLoops(int camEnt)
        {
            foreach (var loop in _loops.Values)
            {
                if (!loop.Active || loop.Instance == null) continue;
                if (!loop.Instance.State.Equals(SoundState.Playing))
                    loop.Instance.Play();
                ApplyLoopVolume(loop, camEnt);
            }
        }

        public static void UpdateMusic() => MusicSystem.Update();

        public static void PauseSounds()
        {
            foreach (var loop in _loops.Values)
                loop.Instance?.Pause();
        }

        public static void ResumeSounds()
        {
            foreach (var loop in _loops.Values)
                loop.Instance?.Resume();
        }

        public static void PlayStoneDrag(int camEnt, int entityEnt, float range = 10f, float volume = 1f)
            => PlaySound2(_stoneDragSfx, camEnt, entityEnt, range, volume);

        public static void KillSounds()
        {
            foreach (var loop in _loops.Values)
            {
                loop.Instance?.Stop();
                loop.Instance?.Dispose();
            }
            _loops.Clear();
        }
    }
}