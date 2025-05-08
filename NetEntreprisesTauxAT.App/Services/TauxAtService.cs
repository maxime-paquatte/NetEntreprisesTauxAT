using ClosedXML.Excel;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NetEntreprisesTauxAT.App.Helpers;

namespace NetEntreprisesTauxAT.App.Services;

public class TauxAtService
{
    private bool _isProcessing;

    private const string ERROR_SHEET_NAME = "Erreurs";
    private readonly ILogger<TauxAtService> _logger;
    private readonly Options _options;

    private IXLWorksheet? _errorSheet;
    private IXLWorksheet? _mainSheet;
    private Dictionary<string, int> _sirensIdx = new();
    private readonly int _firstColIndex;

    public TauxAtService(IOptions<Options> options, ILogger<TauxAtService> logger)
    {
        _options = options.Value;
        _logger = logger;

        _firstColIndex = !string.IsNullOrEmpty(_options.ResultFirstColName)
            ? XLHelper.GetColumnNumberFromLetter(_options.ResultFirstColName)
            : XLHelper.GetColumnNumberFromLetter(_options.SirenColName) + 1;
    }

    public string Process(IEnumerable<string> files, string excelFileSource)
    {
        if (string.IsNullOrEmpty(_options.SirenColName))
            throw new ArgumentException("The SIREN column name is not set", nameof(_options.SirenColName));
        if (!File.Exists(excelFileSource))
            throw new ArgumentException("The source Excel file does not exist", nameof(excelFileSource));
        if (_isProcessing)
            throw new InvalidOperationException("The service is already processing a file");
        _isProcessing = true;

        _logger.LogDebug("Creating backup of the Excel file");
        var excelFile = Path.ChangeExtension(excelFileSource, ".complete.xlsx");
        File.Copy(excelFileSource, excelFile, true);
        _logger.LogInformation("Copy created at {excelFile}", excelFile);

        var extractPath = Path.Combine(Path.GetTempPath(), "taux-at-" + Guid.NewGuid().ToString("D"));
        _logger.LogInformation("Extracting files to {extractPath}", extractPath);
        var fileCount = ExtractFiles(files, extractPath);
        _logger.LogInformation("{fileCount} files extracted", fileCount);

        _logger.LogInformation("Processing files");
        ProcessFiles(extractPath, excelFile);
        _logger.LogInformation("Processing completed");

        _isProcessing = false;
        return excelFile;
    }


    private int ExtractFiles(IEnumerable<string> files, string extractPath)
    {
        var fileCount = 0;
        Directory.CreateDirectory(extractPath);
        foreach (var file in files)
        {
            _logger.LogDebug("Extracting file {file}", file);

            if (!File.Exists(file))
                throw new FileNotFoundException($"The file {file} does not exist", file);

            if (Path.GetExtension(file).Equals(".zip", StringComparison.OrdinalIgnoreCase))
            {
                fileCount += ZipHelper.Extract(file, extractPath);
            }
            else
            {
                File.Copy(file, Path.Combine(extractPath, Path.GetFileName(file)), true);
                fileCount++;
            }
        }

        return fileCount;
    }


    private void ProcessFiles(string extractPath, string excelFilePath)
    {
        using var workbook = new XLWorkbook(excelFilePath);
        try
        {
            InitMainSheet(workbook);
            CreateErrorSheet(workbook);
            BuildSirenIndex();

            foreach (var file in Directory.EnumerateFiles(extractPath, "*.xml", SearchOption.AllDirectories))
            {
                if (ProcessFile(file))
                    _logger.LogInformation("File {file} processed successfully", file);
                else
                    _logger.LogWarning("File {file} processed with errors", file);
            }
        }
        finally
        {
            workbook.Save();
        }
    }

    private bool ProcessFile(string file)
    {
        try
        {
            var array = TauxAtReader.GetData(file).ToArray();
            if (array.Length == 0)
            {
                _logger.LogWarning("can't find TauxAT data in the file {file}", file);
                AddError(file, "Pas de TauxAT dans le fichier");
                return false;
            }

            foreach (var data in array)
                AddData(data);
           
            return true;
        }
        catch (Exception ex)
        {
            var errorFile = Path.ChangeExtension(file, "err");
            if (ex.Save(errorFile))
                return AddError(file, ex.Message, errorFile);
            return AddError(file, ex.Message, "FAILED TO SAVE");
        }
    }

    private void AddData(Data data)
    {
        if (_mainSheet == null)
            throw new InvalidOperationException("The main sheet is not initialized");
        if (string.IsNullOrEmpty(data.Siren))
        {
            AddError(data.FileName, "Can't find SIREN in the file");
            return;
        }
        
        var row  = GetRowToUse(data.Siren);
        AddDataToRow(data, row);
    }

    /// <summary>
    /// search the row for the existing SIREN
    /// if it not found or the data already exists, add a new row at the end
    /// </summary>
    /// <param name="siren"></param>
    /// <returns>Row number</returns>
    /// <exception cref="InvalidOperationException">Thrown if the main sheet is not initialized</exception>
    private int GetRowToUse(string siren)
    {
        var lastRow = _mainSheet?.LastRowUsed();
        if (_mainSheet == null || lastRow == null)
            throw new InvalidOperationException("The main sheet is not initialized");

        //search the row for the SIREN
        //if it not found or the data already exists, add a new row at the end
        if (_sirensIdx.TryGetValue(siren, out var row))
        {
            _logger.LogInformation("Row {row} found for SIREN {siren}", row, siren);
            var dataExists = !_mainSheet.Cell(row, _firstColIndex + 1).IsEmpty();
            if (dataExists)
            {
                _logger.LogWarning("Data already exists for SIREN {siren}", siren);
                row = lastRow.RowNumber() + 1;
                //if the data already exists, put a marker in the given SIREN cell
                _mainSheet.Cell(row, _options.SirenColName).Value = "+++";
            }
        }
        else
        {
            row = lastRow.RowNumber() + 1;
            //if the SIREN is not found, put a marker in the given SIREN cell
            _mainSheet.Cell(row, _options.SirenColName).Value = "---";
        }

        return row;
    }

    private void AddDataToRow(Data data, int row)
    {
        if (_mainSheet == null)
            throw new InvalidOperationException("The main sheet is not initialized");
        
        _logger.LogInformation("Adding data for SIREN {siren} at row {row}", data.Siren, row);
        _mainSheet.Cell(row, _firstColIndex + 0).Value = data.Siren;
        _mainSheet.Cell(row, _firstColIndex + 1).Value = data.Etat;
        _mainSheet.Cell(row, _firstColIndex + 2).Value = TauxAtReader.TAUX_AT_TYPE;
        _mainSheet.Cell(row, _firstColIndex + 3).Value = data.CodeCtn;
        _mainSheet.Cell(row, _firstColIndex + 4).Value = data.Section;
        _mainSheet.Cell(row, _firstColIndex + 5).Value = data.CodeRisque;
        _mainSheet.Cell(row, _firstColIndex + 6).Value = data.TemoinBureau;
        _mainSheet.Cell(row, _firstColIndex + 7).Value = data.Taux;
        _mainSheet.Cell(row, _firstColIndex + 8).Value = data.DateDebutEffet;
        _mainSheet.Cell(row, _firstColIndex + 9).Value = data.DateCalcul;
        _mainSheet.Cell(row, _firstColIndex + 10).Value = data.FileName;
    }
    
    private void InitMainSheet(XLWorkbook workbook)
    {
        _mainSheet = workbook.Worksheets.First();

        _mainSheet.Cell(1, _firstColIndex + 0).Value = "siren";
        _mainSheet.Cell(1, _firstColIndex + 1).Value = "etat";
        _mainSheet.Cell(1, _firstColIndex + 2).Value = "type";
        _mainSheet.Cell(1, _firstColIndex + 3).Value = "code_ctn";
        _mainSheet.Cell(1, _firstColIndex + 4).Value = "section";
        _mainSheet.Cell(1, _firstColIndex + 5).Value = "code_risque";
        _mainSheet.Cell(1, _firstColIndex + 6).Value = "temoin_bureau";
        _mainSheet.Cell(1, _firstColIndex + 7).Value = "taux";
        _mainSheet.Cell(1, _firstColIndex + 8).Value = "date_effet";
        _mainSheet.Cell(1, _firstColIndex + 9).Value = "date_calcul";
        _mainSheet.Cell(1, _firstColIndex + 10).Value = "Fichier";
    }


    private void CreateErrorSheet(XLWorkbook workbook)
    {
        if (workbook.Worksheets.Contains(ERROR_SHEET_NAME))
        {
            _logger.LogWarning("The error sheet already exists. Deleting it.");
            workbook.Worksheets.Delete(ERROR_SHEET_NAME);
        }

        _logger.LogInformation("Creating error sheet");
        _errorSheet = workbook.Worksheets.Add(ERROR_SHEET_NAME);
        _errorSheet.Cell("A1").Value = "Fichier";
        _errorSheet.Cell("B1").Value = "Erreur";
        _errorSheet.Cell("C1").Value = "Fichier erreur";
    }

    /// <summary>
    /// Gets the row number of each SIREN in the sheet.
    /// The SIREN column name is defined in the options.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if the main sheet is null.</exception>
    /// <returns>a dictionary with SIREN as key and row number as value.</returns>
    private void BuildSirenIndex()
    {
        if (_mainSheet == null)
            throw new InvalidOperationException("The main sheet is not initialized");

        _sirensIdx.Clear();
        var row = 2;
        IXLCell cell;
        do
        {
            cell = _mainSheet.Cell(row, _options.SirenColName);
            if (cell.TryGetValue(out string val))
            {
                val = val.Replace(" ", "").Trim();
                _sirensIdx.TryAdd(val, row);
            }
            row++;
        } while (!cell.IsEmpty());

        _logger.LogInformation("Found {count} SIRENs in the sheet", _sirensIdx.Count);
    }

    private bool AddError(string file, string message, string? errorFile = "")
    {
        if (_errorSheet == null)
            throw new InvalidOperationException("The error sheet is not initialized");

        var lastRow = _errorSheet.LastRowUsed();
        if (lastRow == null) throw new Exception("The error sheet should not be empty");
        var row = lastRow.RowNumber() + 1;
        _errorSheet.Cell(row, "A").Value = file;
        _errorSheet.Cell(row, "B").Value = message;
        _errorSheet.Cell(row, "C").Value = errorFile;
        return false;
    }


    public class Options
    {
        public required string SirenColName { get; set; }

        public string? ResultFirstColName { get; set; }
    }
}