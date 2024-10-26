using NAudio.CoreAudioApi;

namespace ThGameMgr.Ex.Game
{
    internal class GameAudio
    {
        public static float GetGameProcessAudioVolume(Process gameProcess)
        {
            MMDeviceEnumerator enumerator = new();
            MMDevice defaultAudioDevice = enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Console);
            if (defaultAudioDevice != null)
            {
                AudioSessionManager audioSessionManager = defaultAudioDevice.AudioSessionManager;
                SessionCollection sessionsCollection = audioSessionManager.Sessions;

                for (int i = 0; i < sessionsCollection.Count; i++)
                {
                    AudioSessionControl session = sessionsCollection[i];
                    uint sessionId = session.GetProcessID;

                    if (sessionId == gameProcess.Id)
                        return session.SimpleAudioVolume.Volume;
                }

                return 0;
            }
            else
            {
                return 0;
            }
        }

        public static void SetGameProcessAudioVolume(Process gameProcess, float volume)
        {
            MMDeviceEnumerator enumerator = new();
            MMDevice defaultAudioDevice = enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Console);
            if (defaultAudioDevice != null)
            {
                AudioSessionManager audioSessionManager = defaultAudioDevice.AudioSessionManager;
                SessionCollection sessionsCollection = audioSessionManager.Sessions;

                for (int i = 0; i < sessionsCollection.Count; i++)
                {
                    AudioSessionControl session = sessionsCollection[i];
                    uint sessionId = session.GetProcessID;

                    if (sessionId == gameProcess.Id)
                    {
                        session.SimpleAudioVolume.Volume = volume;
                        break;
                    }
                }
            }
        }
    }
}
