<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class RenderForm
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
        Me.EditToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.ViewToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.DisplayToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.SolidToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.WireframeToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.VerticesToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.ResetViewToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.StatusStrip1 = New System.Windows.Forms.StatusStrip
        Me.StatusLabel1 = New System.Windows.Forms.ToolStripStatusLabel
        Me.ProgressBar1 = New System.Windows.Forms.ToolStripProgressBar
        Me.SplitContainer2 = New System.Windows.Forms.SplitContainer
        Me.SplitContainer1 = New System.Windows.Forms.SplitContainer
        Me.FileList = New System.Windows.Forms.TreeView
        Me.pic3d = New System.Windows.Forms.PictureBox
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
        Me.MenuStrip1.SuspendLayout()
        Me.StatusStrip1.SuspendLayout()
        Me.SplitContainer2.Panel1.SuspendLayout()
        Me.SplitContainer2.Panel2.SuspendLayout()
        Me.SplitContainer2.SuspendLayout()
        Me.SplitContainer1.Panel1.SuspendLayout()
        Me.SplitContainer1.Panel2.SuspendLayout()
        Me.SplitContainer1.SuspendLayout()
        CType(Me.pic3d, System.ComponentModel.ISupportInitialize).BeginInit()
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
        Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.FileToolStripMenuItem, Me.EditToolStripMenuItem, Me.ViewToolStripMenuItem})
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
        Me.ToolStripSeparator1.Size = New System.Drawing.Size(149, 6)
        '
        'SaveAsToolStripMenuItem
        '
        Me.SaveAsToolStripMenuItem.Name = "SaveAsToolStripMenuItem"
        Me.SaveAsToolStripMenuItem.Size = New System.Drawing.Size(152, 22)
        Me.SaveAsToolStripMenuItem.Text = "Save 3D as ..."
        '
        'QuitToolStripMenuItem
        '
        Me.QuitToolStripMenuItem.Name = "QuitToolStripMenuItem"
        Me.QuitToolStripMenuItem.Size = New System.Drawing.Size(152, 22)
        Me.QuitToolStripMenuItem.Text = "Quit"
        '
        'EditToolStripMenuItem
        '
        Me.EditToolStripMenuItem.Name = "EditToolStripMenuItem"
        Me.EditToolStripMenuItem.Size = New System.Drawing.Size(39, 20)
        Me.EditToolStripMenuItem.Text = "Edit"
        '
        'ViewToolStripMenuItem
        '
        Me.ViewToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.DisplayToolStripMenuItem, Me.ResetViewToolStripMenuItem})
        Me.ViewToolStripMenuItem.Name = "ViewToolStripMenuItem"
        Me.ViewToolStripMenuItem.Size = New System.Drawing.Size(44, 20)
        Me.ViewToolStripMenuItem.Text = "View"
        '
        'DisplayToolStripMenuItem
        '
        Me.DisplayToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.SolidToolStripMenuItem, Me.WireframeToolStripMenuItem, Me.VerticesToolStripMenuItem})
        Me.DisplayToolStripMenuItem.Name = "DisplayToolStripMenuItem"
        Me.DisplayToolStripMenuItem.Size = New System.Drawing.Size(130, 22)
        Me.DisplayToolStripMenuItem.Text = "Display"
        '
        'SolidToolStripMenuItem
        '
        Me.SolidToolStripMenuItem.Checked = True
        Me.SolidToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked
        Me.SolidToolStripMenuItem.Name = "SolidToolStripMenuItem"
        Me.SolidToolStripMenuItem.Size = New System.Drawing.Size(129, 22)
        Me.SolidToolStripMenuItem.Text = "Solid"
        '
        'WireframeToolStripMenuItem
        '
        Me.WireframeToolStripMenuItem.Name = "WireframeToolStripMenuItem"
        Me.WireframeToolStripMenuItem.Size = New System.Drawing.Size(129, 22)
        Me.WireframeToolStripMenuItem.Text = "Wireframe"
        '
        'VerticesToolStripMenuItem
        '
        Me.VerticesToolStripMenuItem.Name = "VerticesToolStripMenuItem"
        Me.VerticesToolStripMenuItem.Size = New System.Drawing.Size(129, 22)
        Me.VerticesToolStripMenuItem.Text = "Vertices"
        '
        'ResetViewToolStripMenuItem
        '
        Me.ResetViewToolStripMenuItem.Name = "ResetViewToolStripMenuItem"
        Me.ResetViewToolStripMenuItem.Size = New System.Drawing.Size(130, 22)
        Me.ResetViewToolStripMenuItem.Text = "Reset View"
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
        Me.SplitContainer2.Panel1.Controls.Add(Me.SplitContainer1)
        '
        'SplitContainer2.Panel2
        '
        Me.SplitContainer2.Panel2.Controls.Add(Me.SplitContainer3)
        Me.SplitContainer2.Size = New System.Drawing.Size(1162, 609)
        Me.SplitContainer2.SplitterDistance = 764
        Me.SplitContainer2.TabIndex = 18
        '
        'SplitContainer1
        '
        Me.SplitContainer1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer1.Location = New System.Drawing.Point(0, 0)
        Me.SplitContainer1.Name = "SplitContainer1"
        '
        'SplitContainer1.Panel1
        '
        Me.SplitContainer1.Panel1.Controls.Add(Me.FileList)
        '
        'SplitContainer1.Panel2
        '
        Me.SplitContainer1.Panel2.Controls.Add(Me.pic3d)
        Me.SplitContainer1.Size = New System.Drawing.Size(764, 609)
        Me.SplitContainer1.SplitterDistance = 254
        Me.SplitContainer1.TabIndex = 18
        '
        'FileList
        '
        Me.FileList.Dock = System.Windows.Forms.DockStyle.Fill
        Me.FileList.Location = New System.Drawing.Point(0, 0)
        Me.FileList.Name = "FileList"
        Me.FileList.Size = New System.Drawing.Size(254, 609)
        Me.FileList.TabIndex = 16
        '
        'pic3d
        '
        Me.pic3d.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pic3d.Location = New System.Drawing.Point(0, 0)
        Me.pic3d.Name = "pic3d"
        Me.pic3d.Size = New System.Drawing.Size(506, 609)
        Me.pic3d.TabIndex = 2
        Me.pic3d.TabStop = False
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
        Me.SplitContainer3.Size = New System.Drawing.Size(394, 609)
        Me.SplitContainer3.SplitterDistance = 304
        Me.SplitContainer3.TabIndex = 18
        '
        'ListBox1
        '
        Me.ListBox1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ListBox1.FormattingEnabled = True
        Me.ListBox1.Location = New System.Drawing.Point(0, 0)
        Me.ListBox1.Name = "ListBox1"
        Me.ListBox1.Size = New System.Drawing.Size(394, 303)
        Me.ListBox1.TabIndex = 11
        '
        'TextureBox
        '
        Me.TextureBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TextureBox.ContextMenuStrip = Me.TexturePreviewPopup
        Me.TextureBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TextureBox.Location = New System.Drawing.Point(0, 0)
        Me.TextureBox.Name = "TextureBox"
        Me.TextureBox.Size = New System.Drawing.Size(394, 276)
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
        Me.TrackBar1.Size = New System.Drawing.Size(394, 25)
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
        'RenderForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1162, 655)
        Me.Controls.Add(Me.SplitContainer2)
        Me.Controls.Add(Me.StatusStrip1)
        Me.Controls.Add(Me.MenuStrip1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow
        Me.MainMenuStrip = Me.MenuStrip1
        Me.Name = "RenderForm"
        Me.Text = "wow2collada"
        Me.MenuStrip1.ResumeLayout(False)
        Me.MenuStrip1.PerformLayout()
        Me.StatusStrip1.ResumeLayout(False)
        Me.StatusStrip1.PerformLayout()
        Me.SplitContainer2.Panel1.ResumeLayout(False)
        Me.SplitContainer2.Panel2.ResumeLayout(False)
        Me.SplitContainer2.ResumeLayout(False)
        Me.SplitContainer1.Panel1.ResumeLayout(False)
        Me.SplitContainer1.Panel2.ResumeLayout(False)
        Me.SplitContainer1.ResumeLayout(False)
        CType(Me.pic3d, System.ComponentModel.ISupportInitialize).EndInit()
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
    Friend WithEvents EditToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ViewToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents DisplayToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents SolidToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents WireframeToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents VerticesToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents StatusStrip1 As System.Windows.Forms.StatusStrip
    Friend WithEvents StatusLabel1 As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents ResetViewToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents SplitContainer2 As System.Windows.Forms.SplitContainer
    Friend WithEvents SplitContainer1 As System.Windows.Forms.SplitContainer
    Friend WithEvents FileList As System.Windows.Forms.TreeView
    Friend WithEvents pic3d As System.Windows.Forms.PictureBox
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

End Class
