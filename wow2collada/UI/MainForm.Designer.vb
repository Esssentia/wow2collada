<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class MainForm
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing AndAlso components IsNot Nothing Then
            components.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container
        Me.MenuStrip1 = New System.Windows.Forms.MenuStrip
        Me.FileToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator
        Me.SaveAsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.QuitToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.ToolsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.ADTExplorerToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.RegionLoaderToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.TestcasesToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.ADTAzeroth2933ToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.ADTNorthend3233ToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.ADTNorthend2525ToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.WMOHumanFarmToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.M2BloodelfGuardToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.M2HumanMaleToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.StatusStrip1 = New System.Windows.Forms.StatusStrip
        Me.StatusLabel1 = New System.Windows.Forms.ToolStripStatusLabel
        Me.ProgressBar1 = New System.Windows.Forms.ToolStripProgressBar
        Me.SplitContainer2 = New System.Windows.Forms.SplitContainer
        Me.FileList = New System.Windows.Forms.TreeView
        Me.SplitContainer3 = New System.Windows.Forms.SplitContainer
        Me.ListBox1 = New System.Windows.Forms.ListBox
        Me.TextureBox = New System.Windows.Forms.PictureBox
        Me.TexturePreviewPopup = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.TexturePopupSaveAs = New System.Windows.Forms.ToolStripMenuItem
        Me.TexturePopupOpenInViewer = New System.Windows.Forms.ToolStripMenuItem
        Me.TrackBar1 = New System.Windows.Forms.TrackBar
        Me.TextureSaveFile = New System.Windows.Forms.SaveFileDialog
        Me.FileListPopup = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.FileListPopupSaveAs = New System.Windows.Forms.ToolStripMenuItem
        Me.FileListPopupOpenInHexViewer = New System.Windows.Forms.ToolStripMenuItem
        Me.FileListSaveFile = New System.Windows.Forms.SaveFileDialog
        Me.SaveModelDialog = New System.Windows.Forms.SaveFileDialog
        Me.ContinentHeightmapDumperToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.MenuStrip1.SuspendLayout()
        Me.StatusStrip1.SuspendLayout()
        Me.SplitContainer2.Panel1.SuspendLayout()
        Me.SplitContainer2.Panel2.SuspendLayout()
        Me.SplitContainer2.SuspendLayout()
        Me.SplitContainer3.Panel1.SuspendLayout()
        Me.SplitContainer3.Panel2.SuspendLayout()
        Me.SplitContainer3.SuspendLayout()
        CType(Me.TextureBox, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.TexturePreviewPopup.SuspendLayout()
        CType(Me.TrackBar1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.FileListPopup.SuspendLayout()
        Me.SuspendLayout()
        '
        'MenuStrip1
        '
        Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.FileToolStripMenuItem, Me.ToolsToolStripMenuItem, Me.TestcasesToolStripMenuItem})
        Me.MenuStrip1.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip1.Name = "MenuStrip1"
        Me.MenuStrip1.Size = New System.Drawing.Size(1162, 24)
        Me.MenuStrip1.TabIndex = 10
        Me.MenuStrip1.Text = "MenuStrip1"
        '
        'FileToolStripMenuItem
        '
        Me.FileToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripSeparator1, Me.SaveAsToolStripMenuItem, Me.QuitToolStripMenuItem})
        Me.FileToolStripMenuItem.Name = "FileToolStripMenuItem"
        Me.FileToolStripMenuItem.Size = New System.Drawing.Size(37, 20)
        Me.FileToolStripMenuItem.Text = "File"
        '
        'ToolStripSeparator1
        '
        Me.ToolStripSeparator1.Name = "ToolStripSeparator1"
        Me.ToolStripSeparator1.Size = New System.Drawing.Size(138, 6)
        '
        'SaveAsToolStripMenuItem
        '
        Me.SaveAsToolStripMenuItem.Name = "SaveAsToolStripMenuItem"
        Me.SaveAsToolStripMenuItem.Size = New System.Drawing.Size(141, 22)
        Me.SaveAsToolStripMenuItem.Text = "Save 3D as ..."
        '
        'QuitToolStripMenuItem
        '
        Me.QuitToolStripMenuItem.Name = "QuitToolStripMenuItem"
        Me.QuitToolStripMenuItem.Size = New System.Drawing.Size(141, 22)
        Me.QuitToolStripMenuItem.Text = "Quit"
        '
        'ToolsToolStripMenuItem
        '
        Me.ToolsToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ADTExplorerToolStripMenuItem, Me.RegionLoaderToolStripMenuItem, Me.ContinentHeightmapDumperToolStripMenuItem})
        Me.ToolsToolStripMenuItem.Name = "ToolsToolStripMenuItem"
        Me.ToolsToolStripMenuItem.Size = New System.Drawing.Size(48, 20)
        Me.ToolsToolStripMenuItem.Text = "Tools"
        '
        'ADTExplorerToolStripMenuItem
        '
        Me.ADTExplorerToolStripMenuItem.Name = "ADTExplorerToolStripMenuItem"
        Me.ADTExplorerToolStripMenuItem.Size = New System.Drawing.Size(150, 22)
        Me.ADTExplorerToolStripMenuItem.Text = "ADT Explorer"
        '
        'RegionLoaderToolStripMenuItem
        '
        Me.RegionLoaderToolStripMenuItem.Name = "RegionLoaderToolStripMenuItem"
        Me.RegionLoaderToolStripMenuItem.Size = New System.Drawing.Size(150, 22)
        Me.RegionLoaderToolStripMenuItem.Text = "Region Loader"
        '
        'TestcasesToolStripMenuItem
        '
        Me.TestcasesToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ADTAzeroth2933ToolStripMenuItem, Me.ADTNorthend3233ToolStripMenuItem, Me.ADTNorthend2525ToolStripMenuItem, Me.WMOHumanFarmToolStripMenuItem, Me.M2BloodelfGuardToolStripMenuItem, Me.M2HumanMaleToolStripMenuItem})
        Me.TestcasesToolStripMenuItem.Name = "TestcasesToolStripMenuItem"
        Me.TestcasesToolStripMenuItem.Size = New System.Drawing.Size(69, 20)
        Me.TestcasesToolStripMenuItem.Text = "Testcases"
        '
        'ADTAzeroth2933ToolStripMenuItem
        '
        Me.ADTAzeroth2933ToolStripMenuItem.Name = "ADTAzeroth2933ToolStripMenuItem"
        Me.ADTAzeroth2933ToolStripMenuItem.Size = New System.Drawing.Size(183, 22)
        Me.ADTAzeroth2933ToolStripMenuItem.Text = "ADT Azeroth 36/49"
        '
        'ADTNorthend3233ToolStripMenuItem
        '
        Me.ADTNorthend3233ToolStripMenuItem.Name = "ADTNorthend3233ToolStripMenuItem"
        Me.ADTNorthend3233ToolStripMenuItem.Size = New System.Drawing.Size(183, 22)
        Me.ADTNorthend3233ToolStripMenuItem.Text = "ADT Northend 32/33"
        '
        'ADTNorthend2525ToolStripMenuItem
        '
        Me.ADTNorthend2525ToolStripMenuItem.Name = "ADTNorthend2525ToolStripMenuItem"
        Me.ADTNorthend2525ToolStripMenuItem.Size = New System.Drawing.Size(183, 22)
        Me.ADTNorthend2525ToolStripMenuItem.Text = "ADT Northend 25/25"
        '
        'WMOHumanFarmToolStripMenuItem
        '
        Me.WMOHumanFarmToolStripMenuItem.Name = "WMOHumanFarmToolStripMenuItem"
        Me.WMOHumanFarmToolStripMenuItem.Size = New System.Drawing.Size(183, 22)
        Me.WMOHumanFarmToolStripMenuItem.Text = "WMO Human Farm"
        '
        'M2BloodelfGuardToolStripMenuItem
        '
        Me.M2BloodelfGuardToolStripMenuItem.Name = "M2BloodelfGuardToolStripMenuItem"
        Me.M2BloodelfGuardToolStripMenuItem.Size = New System.Drawing.Size(183, 22)
        Me.M2BloodelfGuardToolStripMenuItem.Text = "M2 BloodelfGuard"
        '
        'M2HumanMaleToolStripMenuItem
        '
        Me.M2HumanMaleToolStripMenuItem.Name = "M2HumanMaleToolStripMenuItem"
        Me.M2HumanMaleToolStripMenuItem.Size = New System.Drawing.Size(183, 22)
        Me.M2HumanMaleToolStripMenuItem.Text = "M2 HumanMale"
        '
        'StatusStrip1
        '
        Me.StatusStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.StatusLabel1, Me.ProgressBar1})
        Me.StatusStrip1.Location = New System.Drawing.Point(0, 633)
        Me.StatusStrip1.Name = "StatusStrip1"
        Me.StatusStrip1.Size = New System.Drawing.Size(1162, 22)
        Me.StatusStrip1.TabIndex = 11
        Me.StatusStrip1.Text = "StatusStrip1"
        '
        'StatusLabel1
        '
        Me.StatusLabel1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.StatusLabel1.Name = "StatusLabel1"
        Me.StatusLabel1.Size = New System.Drawing.Size(845, 17)
        Me.StatusLabel1.Spring = True
        Me.StatusLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'ProgressBar1
        '
        Me.ProgressBar1.AutoSize = False
        Me.ProgressBar1.Name = "ProgressBar1"
        Me.ProgressBar1.Size = New System.Drawing.Size(300, 16)
        Me.ProgressBar1.Style = System.Windows.Forms.ProgressBarStyle.Continuous
        '
        'SplitContainer2
        '
        Me.SplitContainer2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer2.Location = New System.Drawing.Point(0, 24)
        Me.SplitContainer2.Name = "SplitContainer2"
        '
        'SplitContainer2.Panel1
        '
        Me.SplitContainer2.Panel1.Controls.Add(Me.FileList)
        '
        'SplitContainer2.Panel2
        '
        Me.SplitContainer2.Panel2.Controls.Add(Me.SplitContainer3)
        Me.SplitContainer2.Size = New System.Drawing.Size(1162, 609)
        Me.SplitContainer2.SplitterDistance = 596
        Me.SplitContainer2.TabIndex = 18
        '
        'FileList
        '
        Me.FileList.Dock = System.Windows.Forms.DockStyle.Fill
        Me.FileList.Location = New System.Drawing.Point(0, 0)
        Me.FileList.Name = "FileList"
        Me.FileList.Size = New System.Drawing.Size(596, 609)
        Me.FileList.TabIndex = 19
        '
        'SplitContainer3
        '
        Me.SplitContainer3.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer3.Location = New System.Drawing.Point(0, 0)
        Me.SplitContainer3.Name = "SplitContainer3"
        Me.SplitContainer3.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'SplitContainer3.Panel1
        '
        Me.SplitContainer3.Panel1.Controls.Add(Me.ListBox1)
        '
        'SplitContainer3.Panel2
        '
        Me.SplitContainer3.Panel2.Controls.Add(Me.TextureBox)
        Me.SplitContainer3.Panel2.Controls.Add(Me.TrackBar1)
        Me.SplitContainer3.Size = New System.Drawing.Size(562, 609)
        Me.SplitContainer3.SplitterDistance = 304
        Me.SplitContainer3.TabIndex = 18
        '
        'ListBox1
        '
        Me.ListBox1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ListBox1.FormattingEnabled = True
        Me.ListBox1.Location = New System.Drawing.Point(0, 0)
        Me.ListBox1.Name = "ListBox1"
        Me.ListBox1.Size = New System.Drawing.Size(562, 303)
        Me.ListBox1.TabIndex = 11
        '
        'TextureBox
        '
        Me.TextureBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TextureBox.ContextMenuStrip = Me.TexturePreviewPopup
        Me.TextureBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TextureBox.Location = New System.Drawing.Point(0, 0)
        Me.TextureBox.Name = "TextureBox"
        Me.TextureBox.Size = New System.Drawing.Size(562, 276)
        Me.TextureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom
        Me.TextureBox.TabIndex = 19
        Me.TextureBox.TabStop = False
        '
        'TexturePreviewPopup
        '
        Me.TexturePreviewPopup.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.TexturePopupSaveAs, Me.TexturePopupOpenInViewer})
        Me.TexturePreviewPopup.Name = "ContextMenuStrip1"
        Me.TexturePreviewPopup.Size = New System.Drawing.Size(167, 48)
        '
        'TexturePopupSaveAs
        '
        Me.TexturePopupSaveAs.Name = "TexturePopupSaveAs"
        Me.TexturePopupSaveAs.Size = New System.Drawing.Size(166, 22)
        Me.TexturePopupSaveAs.Text = "Save as ..."
        '
        'TexturePopupOpenInViewer
        '
        Me.TexturePopupOpenInViewer.Name = "TexturePopupOpenInViewer"
        Me.TexturePopupOpenInViewer.Size = New System.Drawing.Size(166, 22)
        Me.TexturePopupOpenInViewer.Text = "Open in Viewer ..."
        '
        'TrackBar1
        '
        Me.TrackBar1.AutoSize = False
        Me.TrackBar1.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.TrackBar1.LargeChange = 1
        Me.TrackBar1.Location = New System.Drawing.Point(0, 276)
        Me.TrackBar1.Name = "TrackBar1"
        Me.TrackBar1.Size = New System.Drawing.Size(562, 25)
        Me.TrackBar1.TabIndex = 18
        '
        'TextureSaveFile
        '
        '
        'FileListPopup
        '
        Me.FileListPopup.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.FileListPopupSaveAs, Me.FileListPopupOpenInHexViewer})
        Me.FileListPopup.Name = "FileListPopup"
        Me.FileListPopup.Size = New System.Drawing.Size(190, 48)
        '
        'FileListPopupSaveAs
        '
        Me.FileListPopupSaveAs.Name = "FileListPopupSaveAs"
        Me.FileListPopupSaveAs.Size = New System.Drawing.Size(189, 22)
        Me.FileListPopupSaveAs.Text = "Save as ..."
        '
        'FileListPopupOpenInHexViewer
        '
        Me.FileListPopupOpenInHexViewer.Name = "FileListPopupOpenInHexViewer"
        Me.FileListPopupOpenInHexViewer.Size = New System.Drawing.Size(189, 22)
        Me.FileListPopupOpenInHexViewer.Text = "Open in Hex Viewer ..."
        '
        'FileListSaveFile
        '
        '
        'SaveModelDialog
        '
        '
        'ContinentHeightmapDumperToolStripMenuItem
        '
        Me.ContinentHeightmapDumperToolStripMenuItem.Name = "ContinentHeightmapDumperToolStripMenuItem"
        Me.ContinentHeightmapDumperToolStripMenuItem.Size = New System.Drawing.Size(236, 22)
        Me.ContinentHeightmapDumperToolStripMenuItem.Text = "Continent Heightmap Dumper"
        '
        'MainForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1162, 655)
        Me.Controls.Add(Me.SplitContainer2)
        Me.Controls.Add(Me.StatusStrip1)
        Me.Controls.Add(Me.MenuStrip1)
        Me.KeyPreview = True
        Me.MainMenuStrip = Me.MenuStrip1
        Me.Name = "MainForm"
        Me.Text = "wow2collada"
        Me.MenuStrip1.ResumeLayout(False)
        Me.MenuStrip1.PerformLayout()
        Me.StatusStrip1.ResumeLayout(False)
        Me.StatusStrip1.PerformLayout()
        Me.SplitContainer2.Panel1.ResumeLayout(False)
        Me.SplitContainer2.Panel2.ResumeLayout(False)
        Me.SplitContainer2.ResumeLayout(False)
        Me.SplitContainer3.Panel1.ResumeLayout(False)
        Me.SplitContainer3.Panel2.ResumeLayout(False)
        Me.SplitContainer3.ResumeLayout(False)
        CType(Me.TextureBox, System.ComponentModel.ISupportInitialize).EndInit()
        Me.TexturePreviewPopup.ResumeLayout(False)
        CType(Me.TrackBar1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.FileListPopup.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents MenuStrip1 As System.Windows.Forms.MenuStrip
    Friend WithEvents FileToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator1 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents QuitToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents StatusStrip1 As System.Windows.Forms.StatusStrip
    Friend WithEvents StatusLabel1 As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents SplitContainer2 As System.Windows.Forms.SplitContainer
    Friend WithEvents SplitContainer3 As System.Windows.Forms.SplitContainer
    Friend WithEvents ListBox1 As System.Windows.Forms.ListBox
    Friend WithEvents TrackBar1 As System.Windows.Forms.TrackBar
    Friend WithEvents TextureBox As System.Windows.Forms.PictureBox
    Friend WithEvents TexturePreviewPopup As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents TexturePopupSaveAs As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents TexturePopupOpenInViewer As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents TextureSaveFile As System.Windows.Forms.SaveFileDialog
    Friend WithEvents FileListPopup As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents FileListPopupSaveAs As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents FileListPopupOpenInHexViewer As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents FileListSaveFile As System.Windows.Forms.SaveFileDialog
    Friend WithEvents ProgressBar1 As System.Windows.Forms.ToolStripProgressBar
    Friend WithEvents SaveAsToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents SaveModelDialog As System.Windows.Forms.SaveFileDialog
    Friend WithEvents FileList As System.Windows.Forms.TreeView
    Friend WithEvents TestcasesToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ADTAzeroth2933ToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents WMOHumanFarmToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents M2BloodelfGuardToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents M2HumanMaleToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ADTNorthend3233ToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolsToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ADTExplorerToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ADTNorthend2525ToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents RegionLoaderToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ContinentHeightmapDumperToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem

End Class
