using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;

namespace Hg.DoomHistory.Types
{
    public enum HotKeyAction
    {
        MapPrevious,
        MapNext,
        SaveFirst,
        SaveLast,
        SavePrevious,
        SaveNext,
        SaveRestore
    }

    public delegate void HotKeyEventHandler(object sender, KeyEventArgs e, HotKeyToAction hotKeyToAction);

    [Serializable]
    public class HotKeyToAction
    {
        #region Fields & Properties

        public HotKeyAction Action { get; set; }
        public bool Enabled { get; set; }
        public HotKey HotKey { get; set; }

        #endregion

        #region Members

        public static HotKeyToAction DeserializeFromString(string settings)
        {
            byte[] b = Convert.FromBase64String(settings);
            using (var stream = new MemoryStream(b))
            {
                var formatter = new BinaryFormatter();
                stream.Seek(0, SeekOrigin.Begin);
                return (HotKeyToAction) formatter.Deserialize(stream);
            }
        }

        public static string SerializeToString(HotKeyToAction hotKeyToAction)
        {
            using (var stream = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, hotKeyToAction);
                stream.Flush();
                stream.Position = 0;
                return Convert.ToBase64String(stream.ToArray());
            }
        }

        #endregion
    }
}