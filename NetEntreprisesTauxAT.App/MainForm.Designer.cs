namespace NetEntreprisesTauxAT.App;

partial class MainForm
{
    private OpenFileDialog ZipFileDialog;
    private Label label1;
    private System.Windows.Forms.Button ZipSelectButton;
    private ListBox FilesListBox;
    private Button SubmitButton;
    private SaveFileDialog ExcelSaveFileDialog;
    private TextBox ExcelFileTB;
    private Button ExcelSelectButton;
    private Label label3;
    private ToolTip toolTip1;
    private System.Windows.Forms.Button OpenHelpButton;
    
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }

        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        ZipFileDialog = new System.Windows.Forms.OpenFileDialog();
        label1 = new System.Windows.Forms.Label();
        ZipSelectButton = new System.Windows.Forms.Button();
        FilesListBox = new System.Windows.Forms.ListBox();
        SubmitButton = new System.Windows.Forms.Button();
        ExcelSaveFileDialog = new System.Windows.Forms.SaveFileDialog();
        ExcelFileTB = new System.Windows.Forms.TextBox();
        ExcelSelectButton = new System.Windows.Forms.Button();
        label3 = new System.Windows.Forms.Label();
        toolTip1 = new System.Windows.Forms.ToolTip(components);
        OpenHelpButton = new System.Windows.Forms.Button();
        SuspendLayout();
        // 
        // ZipFileDialog
        // 
        ZipFileDialog.FileName = "ZipFileDialog";
        ZipFileDialog.Filter = "Fichier ZIP ou XML|*.zip;*.xml";
        ZipFileDialog.Multiselect = true;
        // 
        // label1
        // 
        label1.AutoSize = true;
        label1.Location = new System.Drawing.Point(12, 9);
        label1.Name = "label1";
        label1.Size = new System.Drawing.Size(114, 15);
        label1.TabIndex = 1;
        label1.Text = "Fichers Zip ou XML :";
        // 
        // ZipSelectButton
        // 
        ZipSelectButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right));
        ZipSelectButton.Location = new System.Drawing.Point(132, 5);
        ZipSelectButton.Name = "ZipSelectButton";
        ZipSelectButton.Size = new System.Drawing.Size(482, 23);
        ZipSelectButton.TabIndex = 2;
        ZipSelectButton.Text = "Choisir";
        ZipSelectButton.UseVisualStyleBackColor = true;
        ZipSelectButton.Click += ZipSelectButton_Click;
        // 
        // FilesListBox
        // 
        FilesListBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right));
        FilesListBox.FormattingEnabled = true;
        FilesListBox.ItemHeight = 15;
        FilesListBox.Location = new System.Drawing.Point(12, 34);
        FilesListBox.Name = "FilesListBox";
        FilesListBox.Size = new System.Drawing.Size(632, 289);
        FilesListBox.TabIndex = 3;
        toolTip1.SetToolTip(FilesListBox, "Liste des fichiers ZIP, Double clic pour supprimer");
        FilesListBox.MouseDoubleClick += FilesListBox_MouseDoubleClick;
        // 
        // SubmitButton
        // 
        SubmitButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right));
        SubmitButton.Location = new System.Drawing.Point(12, 371);
        SubmitButton.Name = "SubmitButton";
        SubmitButton.Size = new System.Drawing.Size(632, 23);
        SubmitButton.TabIndex = 4;
        SubmitButton.Text = "Valider";
        SubmitButton.UseVisualStyleBackColor = true;
        SubmitButton.Click += SubmitButton_Click;
        // 
        // ExcelSaveFileDialog
        // 
        ExcelSaveFileDialog.Filter = "Fichier Excel|*.xlsx";
        ExcelSaveFileDialog.OverwritePrompt = false;
        // 
        // ExcelFileTB
        // 
        ExcelFileTB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right));
        ExcelFileTB.Location = new System.Drawing.Point(119, 342);
        ExcelFileTB.Name = "ExcelFileTB";
        ExcelFileTB.Size = new System.Drawing.Size(444, 23);
        ExcelFileTB.TabIndex = 6;
        // 
        // ExcelSelectButton
        // 
        ExcelSelectButton.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right));
        ExcelSelectButton.Location = new System.Drawing.Point(569, 342);
        ExcelSelectButton.Name = "ExcelSelectButton";
        ExcelSelectButton.Size = new System.Drawing.Size(75, 23);
        ExcelSelectButton.TabIndex = 7;
        ExcelSelectButton.Text = "Choisir";
        ExcelSelectButton.UseVisualStyleBackColor = true;
        ExcelSelectButton.Click += ExcelSelectButton_Click;
        // 
        // label3
        // 
        label3.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left));
        label3.AutoSize = true;
        label3.Location = new System.Drawing.Point(12, 346);
        label3.Name = "label3";
        label3.Size = new System.Drawing.Size(99, 15);
        label3.TabIndex = 8;
        label3.Text = "Fichier Excel ( ? ) :";
        toolTip1.SetToolTip(label3, ("Le fichier doit contenir les SIREN en colonne C, les données seront ajoutées à pa" + "rtir de la colonne D"));
        // 
        // OpenHelpButton
        // 
        OpenHelpButton.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right));
        OpenHelpButton.Location = new System.Drawing.Point(620, 5);
        OpenHelpButton.Name = "OpenHelpButton";
        OpenHelpButton.Size = new System.Drawing.Size(24, 23);
        OpenHelpButton.TabIndex = 7;
        OpenHelpButton.Text = "?";
        OpenHelpButton.UseVisualStyleBackColor = true;
        OpenHelpButton.Click += HelpButton_Click;
        // 
        // MainForm
        // 
        AllowDrop = true;
        AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
        AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        ClientSize = new System.Drawing.Size(656, 406);
        Controls.Add(label3);
        Controls.Add(OpenHelpButton);
        Controls.Add(ExcelSelectButton);
        Controls.Add(ExcelFileTB);
        Controls.Add(SubmitButton);
        Controls.Add(FilesListBox);
        Controls.Add(ZipSelectButton);
        Controls.Add(label1);
        Text = "Net Entreprise CRM";
        DragDrop += Form1_DragDrop;
        DragEnter += Form1_DragEnter;
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion
}