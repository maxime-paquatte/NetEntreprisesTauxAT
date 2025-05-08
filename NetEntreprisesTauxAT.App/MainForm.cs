using System.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NetEntreprisesTauxAT.App.Helpers;
using NetEntreprisesTauxAT.App.Services;

namespace NetEntreprisesTauxAT.App;


public partial class MainForm : Form
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<MainForm> _logger;
    private readonly IConfiguration _configuration;

    private const string ERROR_LOG_FILE = "last-error.log";
    
    private const string ERROR_TITLE = "Erreur";
    private const string SUCCESS_TITLE = "Succès";
    private const string WARNING_TITLE = "Attention";
    
    private const string EXCEL_EXTENSION = ".xlsx";
    private const string XML_EXTENSION = ".xml";
    private const string ZIP_EXTENSION = ".zip";
    
    public MainForm(IServiceProvider serviceProvider, ILogger<MainForm> logger, IConfiguration configuration)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _configuration = configuration;
        InitializeComponent();
        LoadDefaultFiles();
    }

    private void LoadDefaultFiles()
    {
        var defaultExcelFile = _configuration["NetEntreprisesTauxAT:DefaultExcelFile"];
        _logger.LogDebug("Loading default Excel file: {defaultExcelFile}", defaultExcelFile);
        if (!string.IsNullOrEmpty(defaultExcelFile) && File.Exists(defaultExcelFile))
            ExcelFileTB.Text = defaultExcelFile;
        
        var defaultZipFile = _configuration["NetEntreprisesTauxAT:DefaultZipFile"];
        _logger.LogDebug("Loading default ZIP file: {defaultZipFile}", defaultZipFile);
        if (!string.IsNullOrEmpty(defaultZipFile) && File.Exists(defaultZipFile))
            FilesListBox.Items.Add(defaultZipFile);
    }
    
    private void ZipSelectButton_Click(object sender, EventArgs e)
    {
        if (ZipFileDialog.ShowDialog() != DialogResult.OK)
            return;

        foreach (var fileName in ZipFileDialog.FileNames)
        {
            if (!File.Exists(fileName))
            {
                _logger.LogWarning("The file {fileName} does not exist.", fileName);
                ShowWarning($"Le fichier {fileName} n'existe pas.");
            }
            else
            {
                _logger.LogInformation("Adding file {fileName} to the list.", fileName);
                FilesListBox.Items.Add(fileName);
            }
        }
    }

    private void Form1_DragEnter(object sender, DragEventArgs e)
    {
        if (e.Data == null || !e.Data.GetDataPresent(DataFormats.FileDrop))
            return;
        _logger.LogDebug("DragEnter event triggered with data present.");
        e.Effect = DragDropEffects.Copy;
    }

    private void Form1_DragDrop(object sender, DragEventArgs e)
    {
        var files = e.Data?.GetData(DataFormats.FileDrop) as string[];
        if (files == null) return;
        
        _logger.LogDebug("DragDrop event triggered with {fileCount} files.", files.Length);

        foreach (string file in files)
        {
            if (!File.Exists(file))
            {
                _logger.LogWarning("The file {file} does not exist.", file);
                ShowWarning($"Le fichier {file} n'existe pas.");
                continue;
            }

            string extension = Path.GetExtension(file).ToLower();
            if (extension == EXCEL_EXTENSION)
            {
                _logger.LogInformation("Setting Excel file to {file}.", file);
                ExcelSaveFileDialog.FileName = file;
                ExcelFileTB.Text = file;
            }
            else if (extension == ZIP_EXTENSION || extension == XML_EXTENSION)
            {
                _logger.LogInformation("Adding file {file} to the list.", file);
                FilesListBox.Items.Insert(0, file);
            }
            else
            {
                _logger.LogWarning("The file {file} is not a valid ZIP or XML file.", file);
                ShowWarning($"Le fichier {file} n'est pas un fichier ZIP ou XML valide.");
            }
        }
    }

    private void ExcelSelectButton_Click(object sender, EventArgs e)
    {
        if (ExcelSaveFileDialog.ShowDialog() != DialogResult.OK)
            return;
        _logger.LogInformation("Excel file selected: {fileName}", ExcelSaveFileDialog.FileName);
        ExcelFileTB.Text = ExcelSaveFileDialog.FileName;
    }

    private void FilesListBox_MouseDoubleClick(object sender, MouseEventArgs e)
    {
        int index = FilesListBox.IndexFromPoint(e.Location);
        if (index == -1)
            return;
        _logger.LogInformation("Removing file at index {index} from the list.", index);
        FilesListBox.Items.RemoveAt(index);
    }

    private void SubmitButton_Click(object sender, EventArgs e)
    {
        if (!ValidateInputs())
            return;
           

        UseWaitCursor = true;
        try
        {
            ProcessFiles();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred during file processing.");
            HandleError(ex);
        }
        finally
        {
            UseWaitCursor = false;
        }
    }

    private bool ValidateInputs()
    {
        if (string.IsNullOrEmpty(ExcelFileTB.Text))
        {
            _logger.LogWarning("Excel file path is empty.");
            ShowError("Vous devez choisir un fichier Excel");
            return false;
        }

        if (!File.Exists(ExcelFileTB.Text))
        {
            _logger.LogWarning("The Excel file {filePath} does not exist.", ExcelFileTB.Text);
            ShowError("Le fichier Excel sélectionné n'existe pas");
            return false;
        }

        if (FilesListBox.Items.Count == 0)
        {
            _logger.LogWarning("No files selected in the list.");
            ShowError("Vous devez choisir au moins un fichier ZIP ou XML");
            return false;
        }

        return true;
    }

    private void ProcessFiles()
    {
        var files = FilesListBox.Items.Cast<string>().ToList();
        var excelFile = ExcelFileTB.Text;
        _logger.LogInformation("Processing files: {fileCount} files with Excel file: {excelFile}", files.Count, excelFile);
        //Get a new instance of TauxAtService each time
        var service = _serviceProvider.GetRequiredService<TauxAtService>();
        var processedFile = service.Process(files, excelFile);

        if (MessageBox.Show("Voulez-vous ouvrir le fichier Excel maintenant ?", 
            SUCCESS_TITLE, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
        {
            OpenExcelFile(processedFile);
        }
    }

    private void OpenExcelFile(string filePath)
    {
        try
        {
            if (!File.Exists(filePath))
            {
                _logger.LogWarning("The Excel file {filePath} does not exist.", filePath);
                ShowWarning($"Le fichier Excel {filePath} n'existe pas.");
                return;
            }
            Process.Start(new ProcessStartInfo
            {
                FileName = filePath,
                UseShellExecute = true
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to open Excel file");
            ShowWarning($"Impossible d'ouvrir le fichier Excel : {ex.Message}");
        }
    }

    private void HelpButton_Click(object sender, EventArgs e)
    {
        try
        {
            var helpUrl = _configuration["NetEntreprisesTauxAT:HelpUrl"];
            if(string.IsNullOrEmpty(helpUrl))
                ShowWarning("Aucun lien d'aide configuré.");
            else
                Process.Start(new ProcessStartInfo { FileName = helpUrl, UseShellExecute = true });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to open help link");
            ShowError($"Impossible d'ouvrir le lien d'aide : {ex.Message}");
        }
    }

    private void ShowError(string message)
    {
        MessageBox.Show(message, ERROR_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
    }

    private void ShowWarning(string message)
    {
        MessageBox.Show(message, WARNING_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Warning);
    }

    private void HandleError(Exception ex)
    {
        ShowError($"Une erreur est survenue : {ex.Message}");
        File.WriteAllText(ERROR_LOG_FILE, ex.GetLog());
    }
}