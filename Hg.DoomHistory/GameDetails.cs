using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;

namespace Hg.DoomHistory
{
    public class YesNoConverter : BooleanConverter
    {
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

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            string t = value as string;
            if (t == "Yes")
                return true;
            if (t == "No")
                return false;
            return base.ConvertFrom(context, culture, value);
        }
    }

    public class DiffConverter : Int32Converter
    {
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

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            string t = value as string;
            int index = Array.IndexOf(GameDetails.DiffValues, t);
            if (index > 0)
                return index;
            return base.ConvertFrom(context, culture, value);
        }
    }


    public class GameDetails
    {
        public static readonly string[] DiffValues =
        {
            "I'm too young to die",
            "Hurt me plenty",
            "Ultra-Violence",
            "Nightmare",
            "Ultra-Nightmare"
        };

        private readonly string _content;

        private readonly int _id;

        private string _notes;

        public string DateTimeSafe;
        public string DateTimeString;

        [Browsable(false)] public bool HasScreenshots;

        public string MapSafe;

        [Browsable(false)] public string ScreenshotsPath;

        public GameDetails(int id, string content)
        {
            _id = id;

            IsDeath = false;
            Completed = false;
            Difficulty = 42;
            MapDesc = "";
            MapSafe = "";
            HasScreenshots = false;
            Notes = "";
            Path = "";

            _content = content;

            Parse();
        }


        [Browsable(true)]
        [ReadOnly(false)]
        [Category("User Input")]
        public string Notes
        {
            get
            {
                try
                {
                    string notesPath = System.IO.Path.Combine(Path, ".hg.notes");
                    if (File.Exists(notesPath))
                        _notes = File.ReadAllText(notesPath);
                }
                catch (Exception)
                {
                }

                return _notes;
            }
            set
            {
                _notes = value;
                try
                {
                    string notesPath = System.IO.Path.Combine(Path, ".hg.notes");
                    File.WriteAllText(notesPath, _notes);
                }
                catch (Exception)
                {
                }
            }
        }

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

        [Browsable(true)]
        [ReadOnly(true)]
        [Category("Saved Game Details")]
        [DisplayName("Saved At")]
        public DateTime Time { get; set; }

        [Browsable(true)]
        [ReadOnly(true)]
        [Category("Saved Game Details")]
        [DisplayName("Total Time Played")]
        public TimeSpan PlayedTime { get; set; }

        [TypeConverter(typeof(YesNoConverter))]
        [Browsable(true)]
        [ReadOnly(true)]
        [Category("Saved Game Details")]
        [DisplayName("Death Checkpoint")]
        public bool IsDeath { get; set; }

        [Browsable(false)] public string Path { get; private set; }

        public void SetPath(string parentPath)
        {
            Path = parentPath; //System.IO.Path.Combine(parentPath, DateTimeSafe);
        }

        public static string DiffToString(int value)
        {
            if (value >= 0 && value < DiffValues.Length)
                return DiffValues[value];

            return "Unknown: (" + value + ")";
        }

        private void Parse()
        {
            foreach (string line in _content.Split(new[] {'\n', '\r'}, StringSplitOptions.RemoveEmptyEntries))
            {
                if (line.Contains("="))
                {
                    string[] values = line.Split('=');
                    if (values[0] == "completed")
                    {
                        Completed = int.Parse(values[1]) == 1;
                    }

                    if (values[0] == "date")
                    {
                        Time = DateTime.Now;
                        if (int.TryParse(values[1], out var unix))
                        {
                            Time = DateTimeOffset.FromUnixTimeSeconds(unix).LocalDateTime;
                            DateTimeSafe = Time.ToString("yyyy-MM-dd HH.mm.ss");
                            DateTimeString = Time.ToString("yyyy-MM-dd HH:mm:ss");
                        }
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
                MapSafe = MapDesc;

            foreach (char invalidPathChar in System.IO.Path.GetInvalidPathChars())
            {
                MapSafe = MapSafe.Replace(invalidPathChar, ' ');
            }
        }
    }
}