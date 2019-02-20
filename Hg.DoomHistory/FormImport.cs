using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Hg.DoomHistory
{
    public partial class FormImport : Form
    {
        private readonly string _backupFolder;
        private string _importFolder;
        private bool _isImporting;
        private int _scannedFiles;

        private int _scannedFolders;

        public int Imported;

        public FormImport(string backupFolder)
        {
            InitializeComponent();
            _backupFolder = backupFolder;
        }

        private void buttonBackupBrowse_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog.ShowDialog(this) == DialogResult.OK)
            {
                if (Directory.Exists(folderBrowserDialog.SelectedPath))
                {
                    _importFolder = folderBrowserDialog.SelectedPath;
                    textBoxFolder.Text = _importFolder;
                }
                else
                {
                    MessageBox.Show(@"Invalid path", @"Nop", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void buttonImport_Click(object sender, EventArgs e)
        {
            if (_isImporting)
            {
                _isImporting = false;
                buttonImport.Text = @"Import";
            }
            else
            {
                if (!string.IsNullOrEmpty(_importFolder) && Directory.Exists(_importFolder))
                {
                    _scannedFiles = 0;
                    _scannedFolders = 0;
                    Imported = 0;
                    Task.Run(() => StartImport());
                }
            }
        }

        private void SwitchControl(bool enabled)
        {
            buttonBackupBrowse.Enabled = enabled;
            buttonClose.Enabled = enabled;
        }

        private void StartImport()
        {
            Invoke(new Action(() => SwitchControl(false)));
            try
            {
                _isImporting = true;
                Invoke(new Action(() => { buttonImport.Text = @"Stop"; }));
                ScanFolderForSavedGame(_importFolder);
            }
            finally
            {
                Invoke(new Action(() => { buttonImport.Text = @"Import"; }));
                _isImporting = false;
                Invoke(new Action(() => SwitchControl(true)));
                Invoke(new Action(() => MessageBox.Show($@"{Imported} backup imported", @"Report", MessageBoxButtons.OK,
                    MessageBoxIcon.Information)));
            }
        }

        private void Message(string text)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => Message(text)));
            }
            else
            {
                textBoxProgress.Text = text;
                Application.DoEvents();
            }
        }

        private void ScanFolderForSavedGame(string path)
        {
            if (!_isImporting)
                return;

            if (!Directory.Exists(path))
                return;

            try
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(path);

                foreach (FileInfo fileInfo in directoryInfo.GetFiles())
                {
                    if (!_isImporting)
                        return;

                    if (fileInfo.Name == "game.details")
                    {
                        // Backup this folder
                        string content = File.ReadAllText(fileInfo.FullName);

                        int id = 0;
                        if (directoryInfo.Name == "GAME-AUTOSAVE0")
                            id = 1;
                        if (directoryInfo.Name == "GAME-AUTOSAVE1")
                            id = 2;
                        if (directoryInfo.Name == "GAME-AUTOSAVE2")
                            id = 3;

                        if (id > 0)
                        {
                            GameDetails gameDetails = new GameDetails(id, content);

                            string pathSlot = Path.Combine(_backupFolder, "Slot" + id);
                            string pathMap = Path.Combine(pathSlot, gameDetails.MapSafe);
                            Directory.CreateDirectory(pathMap);

                            string timeStampFolderName = gameDetails.DateTimeSafe;
                            string pathTimeStamp = Path.Combine(pathMap, timeStampFolderName);
                            Directory.CreateDirectory(pathTimeStamp);

                            gameDetails.SetPath(pathTimeStamp);

                            string pathGameAutoGame = Path.Combine(pathTimeStamp, "GAME-AUTOSAVE" + (id - 1));
                            Directory.CreateDirectory(pathGameAutoGame);

                            DirectoryInfo source = new DirectoryInfo(path);
                            DirectoryInfo target = new DirectoryInfo(pathGameAutoGame);

                            foreach (FileInfo file in source.GetFiles())
                            {
                                fileInfo.CopyTo(Path.Combine(target.FullName, file.Name), true);
                            }

                            // Try to recover death, notes and screenshot if possible
                            if (directoryInfo.Parent != null && directoryInfo.Parent.Name == timeStampFolderName)
                            {
                                string timeStampSourcePath = directoryInfo.Parent.FullName;
                                
                                CheckAndCopy(timeStampSourcePath, ".hg.death", pathTimeStamp);
                                CheckAndCopy(timeStampSourcePath, ".hg.notes", pathTimeStamp);

                                CheckAndCopy(timeStampSourcePath, timeStampFolderName + ".gif", pathTimeStamp);
                                CheckAndCopy(timeStampSourcePath, timeStampFolderName + ".jpg", pathTimeStamp);
                                CheckAndCopy(timeStampSourcePath, timeStampFolderName + ".png", pathTimeStamp);
                            }

                            Imported++;
                            break;
                        }
                    }

                    _scannedFiles++;

                    Message(
                        $"{_scannedFolders} folders scanned, {_scannedFiles} files scanned, {Imported} backup imported");
                }

                foreach (DirectoryInfo directory in directoryInfo.GetDirectories())
                {
                    if (!_isImporting)
                        return;

                    ScanFolderForSavedGame(directory.FullName);
                }

                _scannedFolders++;
                Message(
                    $"{_scannedFolders} folders scanned, {_scannedFiles} files scanned, {Imported} backup imported");
            }
            catch (Exception)
            {
                // Don't log, recursive function!
            }
        }

        private static void CheckAndCopy(string timeStampSourcePath, string filename, string pathTimeStamp)
        {
            if (File.Exists(Path.Combine(timeStampSourcePath, filename)))
            {
                File.Copy(Path.Combine(timeStampSourcePath, filename), Path.Combine(pathTimeStamp, filename));
            }
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}