using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using Hg.DoomHistory.Comparers;

namespace Hg.DoomHistory.Types
{
    public class YesNoConverter : BooleanConverter
    {
        #region Members

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            string t = value as string;
            if (t == "Yes")
            {
                return true;
            }

            if (t == "No")
            {
                return false;
            }

            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value,
            Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                if (value is bool b)
                {
                    return b ? "Yes" : "No";
                }
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }

        #endregion
    }

    public class DiffConverter : Int32Converter
    {
        #region Members

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            string t = value as string;
            int index = Array.IndexOf(GameDetails.DiffValues, t);
            if (index > 0)
            {
                return index;
            }

            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value,
            Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                if (value is int b)
                {
                    return GameDetails.DiffToString(b);
                }
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }

        #endregion
    }

    public class GameDetails
    {
        #region Fields & Properties

        public static readonly string[] DiffValues =
        {
            "I'm too young to die",
            "Hurt me plenty",
            "Ultra-Violence",
            "Nightmare",
            "Ultra-Nightmare"
        };

        public string DateTimeSafe;
        public string DateTimeString;

        [Browsable(false)] public bool HasScreenshots;

        public string MapSafe;

        [Browsable(false)] public string ScreenshotsPath;

        private string _notes;
        private bool _notesLoaded;

        [TypeConverter(typeof(YesNoConverter))]
        [Browsable(true)]
        [ReadOnly(true)]
        [Category("Saved Game Details")]
        [DisplayName("Game Completed")]
        public bool Completed { get; set; }

        [TypeConverter(typeof(DiffConverter))]
        [Browsable(true)]
        [ReadOnly(true)]
        [Category("Saved Game Details")]
        [DisplayName("Difficulty")]
        public int Difficulty { get; set; }

        [Browsable(false)] public string DiffString => DiffToString(Difficulty);

        [TypeConverter(typeof(YesNoConverter))]
        [Browsable(true)]
        [ReadOnly(true)]
        [Category("Saved Game Details")]
        [DisplayName("Death Checkpoint")]
        public bool IsDeath { get; set; }

        [Browsable(true)]
        [ReadOnly(true)]
        [Category("Saved Game Details")]
        [DisplayName("Level #")]
        public int LevelNumber { get; private set; }

        [Browsable(true)]
        [ReadOnly(true)]
        [Category("Saved Game Details")]
        [DisplayName("Map Description")]
        public string MapDesc { get; set; }

        [Browsable(true)]
        [ReadOnly(true)]
        [Category("Saved Game Details")]
        [DisplayName("Map Name")]
        public string MapName { get; set; }

        [Browsable(true)]
        [ReadOnly(false)]
        [Category("User Input")]
        public string Notes
        {
            get
            {
                if (!_notesLoaded)
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(Path) && Directory.Exists(Path))
                        {
                            string notesPath = System.IO.Path.Combine(Path, ".hg.notes");
                            if (File.Exists(notesPath))
                            {
                                _notes = File.ReadAllText(notesPath);
                                _notesLoaded = true;
                            }
                        }
                    }
                    // ReSharper disable once EmptyGeneralCatchClause
                    catch (Exception)
                    {
                    }
                }

                return _notes;
            }
            set
            {
                if (_notes != value)
                {
                    _notes = value;
                    try
                    {
                        if (!string.IsNullOrEmpty(Path) && Directory.Exists(Path))
                        {
                            string notesPath = System.IO.Path.Combine(Path, ".hg.notes");
                            File.WriteAllText(notesPath, _notes);
                        }
                    }
                    // ReSharper disable once EmptyGeneralCatchClause
                    catch (Exception)
                    {
                    }
                }
            }
        }

        [Browsable(false)] public string Path { get; private set; }

        [Browsable(true)]
        [ReadOnly(true)]
        [Category("Saved Game Details")]
        [DisplayName("Total Time Played")]
        public TimeSpan PlayedTime { get; set; }

        [Browsable(true)]
        [ReadOnly(true)]
        [Category("Saved Game Details")]
        [DisplayName("Saved At")]
        public DateTime SavedAt { get; set; }

        #endregion

        #region Members

        public GameDetails(string content)
        {
            _notesLoaded = false;

            IsDeath = false;
            Completed = false;
            Difficulty = 42;
            MapDesc = "";
            MapSafe = "";
            HasScreenshots = false;
            Notes = "";
            Path = "";

            Parse(content);
        }

        public static string DiffToString(int value)
        {
            if (value >= 0 && value < DiffValues.Length)
            {
                return DiffValues[value];
            }

            return "Unknown: (" + value + ")";
        }

        public void SetPath(string timestampPath)
        {
            Path = timestampPath;
        }

        private void Parse(string content)
        {
            if (string.IsNullOrEmpty(content))
            {
                return;
            }

            foreach (string line in content.Split(new[] {'\n', '\r'}, StringSplitOptions.RemoveEmptyEntries))
            {
                if (line.Contains("="))
                {
                    string[] values = line.Split('=');

                    if (values.Length < 2)
                    {
                        continue;
                    }

                    if (values[0] == "completed")
                    {
                        Completed = int.Parse(values[1]) == 1;
                    }

                    if (values[0] == "date")
                    {
                        SavedAt = DateTime.Now;
                        if (int.TryParse(values[1], out var unix))
                        {
                            SavedAt = DateTimeOffset.FromUnixTimeSeconds(unix).LocalDateTime;
                        }

                        // For folder name
                        DateTimeSafe = SavedAt.ToString("yyyy-MM-dd HH.mm.ss");

                        // For list view
                        DateTimeString = SavedAt.ToString("yyyy-MM-dd HH:mm:ss");
                    }

                    if (values[0] == "difficulty")
                    {
                        Difficulty = int.Parse(values[1]);
                        DiffToString(Difficulty);
                    }

                    if (values[0] == "mapDesc")
                    {
                        MapDesc = values[1];
                    }

                    if (values[0] == "mapName")
                    {
                        MapName = values[1];
                    }

                    if (values[0] == "time")
                    {
                        PlayedTime = new TimeSpan(0, 0, int.Parse(values[1]));
                    }
                }
            }

            if (MapDesc != "")
            {
                MapSafe = MapDesc;
            }

            foreach (char invalidPathChar in System.IO.Path.GetInvalidPathChars())
            {
                MapSafe = MapSafe.Replace(invalidPathChar, ' ');
            }

            LevelNumber = MapComparer.MapNameToLevel(MapName);
            if (LevelNumber > 0)
            {
                MapSafe = LevelNumber.ToString().PadRight(2) + " - " + MapSafe;
            }
        }

        #endregion
    }
}