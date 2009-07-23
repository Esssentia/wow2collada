<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class RenderFormOpenGL
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.StatusStrip1 = New System.Windows.Forms.StatusStrip
        Me.LabelFile = New System.Windows.Forms.ToolStripStatusLabel
        Me.LabelFPS = New System.Windows.Forms.ToolStripStatusLabel
        Me.MenuStrip1 = New System.Windows.Forms.MenuStrip
        Me.ViewToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.DisplayToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.TexturedToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.WireframeToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.BonesToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.ResetViewToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.RotateToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.DebugModeToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.SplitContainer1 = New System.Windows.Forms.SplitContainer
        Me.SubSets = New System.Windows.Forms.TreeView
        Me.GlControl1 = New OpenTK.GLControl
        Me.StatusStrip1.SuspendLayout()
        Me.MenuStrip1.SuspendLayout()
        Me.SplitContainer1.Panel1.SuspendLayout()
        Me.SplitContainer1.Panel2.SuspendLayout()
        Me.SplitContainer1.SuspendLayout()
        Me.SuspendLayout()
        '
        'StatusStrip1
        '
        Me.StatusStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.LabelFile, Me.LabelFPS})
        Me.StatusStrip1.Location = New System.Drawing.Point(0, 637)
        Me.StatusStrip1.Name = "StatusStrip1"
        Me.StatusStrip1.Size = New System.Drawing.Size(955, 22)
        Me.StatusStrip1.TabIndex = 4
        Me.StatusStrip1.Text = "StatusStrip1"
        '
        'LabelFile
        '
        Me.LabelFile.Name = "LabelFile"
        Me.LabelFile.Size = New System.Drawing.Size(901, 17)
        Me.LabelFile.Spring = True
        Me.LabelFile.Text = "ToolStripStatusLabel1"
        Me.LabelFile.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'LabelFPS
        '
        Me.LabelFPS.Name = "LabelFPS"
        Me.LabelFPS.Size = New System.Drawing.Size(39, 17)
        Me.LabelFPS.Text = "xx FPS"
        Me.LabelFPS.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'MenuStrip1
        '
        Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ViewToolStripMenuItem})
        Me.MenuStrip1.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip1.Name = "MenuStrip1"
        Me.MenuStrip1.Size = New System.Drawing.Size(955, 24)
        Me.MenuStrip1.TabIndex = 5
        Me.MenuStrip1.Text = "MenuStrip1"
        '
        'ViewToolStripMenuItem
        '
        Me.ViewToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.DisplayToolStripMenuItem, Me.ResetViewToolStripMenuItem, Me.RotateToolStripMenuItem, Me.DebugModeToolStripMenuItem})
        Me.ViewToolStripMenuItem.Name = "ViewToolStripMenuItem"
        Me.ViewToolStripMenuItem.Size = New System.Drawing.Size(44, 20)
        Me.ViewToolStripMenuItem.Text = "View"
        '
        'DisplayToolStripMenuItem
        '
        Me.DisplayToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.TexturedToolStripMenuItem, Me.WireframeToolStripMenuItem, Me.BonesToolStripMenuItem})
        Me.DisplayToolStripMenuItem.Name = "DisplayToolStripMenuItem"
        Me.DisplayToolStripMenuItem.Size = New System.Drawing.Size(130, 22)
        Me.DisplayToolStripMenuItem.Text = "Display"
        '
        'TexturedToolStripMenuItem
        '
        Me.TexturedToolStripMenuItem.Checked = True
        Me.TexturedToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked
        Me.TexturedToolStripMenuItem.Name = "TexturedToolStripMenuItem"
        Me.TexturedToolStripMenuItem.Size = New System.Drawing.Size(129, 22)
        Me.TexturedToolStripMenuItem.Text = "Textured"
        '
        'WireframeToolStripMenuItem
        '
        Me.WireframeToolStripMenuItem.Name = "WireframeToolStripMenuItem"
        Me.WireframeToolStripMenuItem.Size = New System.Drawing.Size(129, 22)
        Me.WireframeToolStripMenuItem.Text = "Wireframe"
        '
        'BonesToolStripMenuItem
        '
        Me.BonesToolStripMenuItem.Name = "BonesToolStripMenuItem"
        Me.BonesToolStripMenuItem.Size = New System.Drawing.Size(129, 22)
        Me.BonesToolStripMenuItem.Text = "Bones"
        '
        'ResetViewToolStripMenuItem
        '
        Me.ResetViewToolStripMenuItem.Name = "ResetViewToolStripMenuItem"
        Me.ResetViewToolStripMenuItem.Size = New System.Drawing.Size(130, 22)
        Me.ResetViewToolStripMenuItem.Text = "Reset View"
        '
        'RotateToolStripMenuItem
        '
        Me.RotateToolStripMenuItem.Checked = True
        Me.RotateToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked
        Me.RotateToolStripMenuItem.Name = "RotateToolStripMenuItem"
        Me.RotateToolStripMenuItem.Size = New System.Drawing.Size(130, 22)
        Me.RotateToolStripMenuItem.Text = "Rotate"
        '
        'DebugModeToolStripMenuItem
        '
        Me.DebugModeToolStripMenuItem.Name = "DebugModeToolStripMenuItem"
        Me.DebugModeToolStripMenuItem.Size = New System.Drawing.Size(130, 22)
        Me.DebugModeToolStripMenuItem.Text = "Debug"
        '
        'SplitContainer1
        '
        Me.SplitContainer1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer1.Location = New System.Drawing.Point(0, 24)
        Me.SplitContainer1.Name = "SplitContainer1"
        '
        'SplitContainer1.Panel1
        '
        Me.SplitContainer1.Panel1.Controls.Add(Me.GlControl1)
        '
        'SplitContainer1.Panel2
        '
        Me.SplitContainer1.Panel2.Controls.Add(Me.SubSets)
        Me.SplitContainer1.Panel2MinSize = 0
        Me.SplitContainer1.Size = New System.Drawing.Size(955, 613)
        Me.SplitContainer1.SplitterDistance = 829
        Me.SplitContainer1.TabIndex = 7
        '
        'SubSets
        '
        Me.SubSets.CheckBoxes = True
        Me.SubSets.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SubSets.Location = New System.Drawing.Point(0, 0)
        Me.SubSets.Name = "SubSets"
        Me.SubSets.Size = New System.Drawing.Size(122, 613)
        Me.SubSets.TabIndex = 0
        '
        'GlControl1
        '
        Me.GlControl1.BackColor = System.Drawing.Color.Black
        Me.GlControl1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.GlControl1.Location = New System.Drawing.Point(0, 0)
        Me.GlControl1.Name = "GlControl1"
        Me.GlControl1.Size = New System.Drawing.Size(829, 613)
        Me.GlControl1.TabIndex = 0
        Me.GlControl1.VSync = True
        '
        'RenderFormOpenGL
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(955, 659)
        Me.ControlBox = False
        Me.Controls.Add(Me.SplitContainer1)
        Me.Controls.Add(Me.StatusStrip1)
        Me.Controls.Add(Me.MenuStrip1)
        Me.MainMenuStrip = Me.MenuStrip1
        Me.Name = "RenderFormOpenGL"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Render"
        Me.StatusStrip1.ResumeLayout(False)
        Me.StatusStrip1.PerformLayout()
        Me.MenuStrip1.ResumeLayout(False)
        Me.MenuStrip1.PerformLayout()
        Me.SplitContainer1.Panel1.ResumeLayout(False)
        Me.SplitContainer1.Panel2.ResumeLayout(False)
        Me.SplitContainer1.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents StatusStrip1 As System.Windows.Forms.StatusStrip
    Friend WithEvents LabelFile As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents LabelFPS As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents MenuStrip1 As System.Windows.Forms.MenuStrip
    Friend WithEvents ViewToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents DisplayToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents TexturedToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents WireframeToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ResetViewToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents RotateToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents DebugModeToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents SplitContainer1 As System.Windows.Forms.SplitContainer
    Friend WithEvents SubSets As System.Windows.Forms.TreeView
    Friend WithEvents BonesToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents GlControl1 As OpenTK.GLControl
End Class
