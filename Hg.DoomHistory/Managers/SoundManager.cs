using System.Media;
using Hg.DoomHistory.Properties;

namespace Hg.DoomHistory.Managers
{
    public class SoundManager
    {
        #region Fields & Properties

        private static readonly SoundPlayer SoundPlayerDummy;
        private static readonly SoundPlayer SoundPlayerError;
        private static readonly SoundPlayer SoundPlayerSuccess;

        #endregion

        #region Members

        static SoundManager()
        {
            // Only here to speed up the replay of the other SoundPlayer
            SoundPlayerDummy = new SoundPlayer(Resources.empty);
            SoundPlayerDummy.Load();
            SoundPlayerSuccess = new SoundPlayer(Resources.success);
            SoundPlayerError = new SoundPlayer(Resources.error);
            SoundPlayerSuccess.Load();
            SoundPlayerError.Load();
        }

        public static void PlayError()
        {
            SoundPlayerError.Play();
        }

        public static void PlaySuccess()
        {
            SoundPlayerSuccess.Play();
        }

        public static void PreLoad()
        {
            SoundPlayerDummy.Play();
        }

        #endregion
    }
}