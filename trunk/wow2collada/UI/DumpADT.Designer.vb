<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class DumpADT
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
        Me.chkTextures = New System.Windows.Forms.CheckBox
        Me.chkAlphamaps = New System.Windows.Forms.CheckBox
        Me.chkLayermaps = New System.Windows.Forms.CheckBox
        Me.chkDepthmap = New System.Windows.Forms.CheckBox
        Me.chkCombined = New System.Windows.Forms.CheckBox
        Me.StatusStrip1 = New System.Windows.Forms.StatusStrip
        Me.ToolStripProgressBar1 = New System.Windows.Forms.ToolStripProgressBar
        Me.Button1 = New System.Windows.Forms.Button
        Me.chkXML = New System.Windows.Forms.CheckBox
        Me.StatusStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'chkTextures
        '
        Me.chkTextures.AutoSize = True
        Me.chkTextures.Checked = True
        Me.chkTextures.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkTextures.Location = New System.Drawing.Point(13, 13)
        Me.chkTextures.Name = "chkTextures"
        Me.chkTextures.Size = New System.Drawing.Size(98, 17)
        Me.chkTextures.TabIndex = 1
        Me.chkTextures.Text = "Dump Textures"
        Me.chkTextures.UseVisualStyleBackColor = True
        '
        'chkAlphamaps
        '
        Me.chkAlphamaps.AutoSize = True
        Me.chkAlphamaps.Checked = True
        Me.chkAlphamaps.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkAlphamaps.Location = New System.Drawing.Point(13, 37)
        Me.chkAlphamaps.Name = "chkAlphamaps"
        Me.chkAlphamaps.Size = New System.Drawing.Size(109, 17)
        Me.chkAlphamaps.TabIndex = 2
        Me.chkAlphamaps.Text = "Dump Alphamaps"
        Me.chkAlphamaps.UseVisualStyleBackColor = True
        '
        'chkLayermaps
        '
        Me.chkLayermaps.AutoSize = True
        Me.chkLayermaps.Checked = True
        Me.chkLayermaps.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkLayermaps.Location = New System.Drawing.Point(13, 61)
        Me.chkLayermaps.Name = "chkLayermaps"
        Me.chkLayermaps.Size = New System.Drawing.Size(108, 17)
        Me.chkLayermaps.TabIndex = 3
        Me.chkLayermaps.Text = "Dump Layermaps"
        Me.chkLayermaps.UseVisualStyleBackColor = True
        '
        'chkDepthmap
        '
        Me.chkDepthmap.AutoSize = True
        Me.chkDepthmap.Checked = True
        Me.chkDepthmap.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkDepthmap.Location = New System.Drawing.Point(13, 85)
        Me.chkDepthmap.Name = "chkDepthmap"
        Me.chkDepthmap.Size = New System.Drawing.Size(106, 17)
        Me.chkDepthmap.TabIndex = 4
        Me.chkDepthmap.Text = "Dump Depthmap"
        Me.chkDepthmap.UseVisualStyleBackColor = True
        '
        'chkCombined
        '
        Me.chkCombined.AutoSize = True
        Me.chkCombined.Location = New System.Drawing.Point(13, 109)
        Me.chkCombined.Name = "chkCombined"
        Me.chkCombined.Size = New System.Drawing.Size(237, 17)
        Me.chkCombined.TabIndex = 5
        Me.chkCombined.Text = "Dump Combined Textures (takes a long time)"
        Me.chkCombined.UseVisualStyleBackColor = True
        '
        'StatusStrip1
        '
        Me.StatusStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripProgressBar1})
        Me.StatusStrip1.Location = New System.Drawing.Point(0, 173)
        Me.StatusStrip1.Name = "StatusStrip1"
        Me.StatusStrip1.Size = New System.Drawing.Size(423, 22)
        Me.StatusStrip1.TabIndex = 6
        Me.StatusStrip1.Text = "StatusStrip1"
        '
        'ToolStripProgressBar1
        '
        Me.ToolStripProgressBar1.AutoSize = False
        Me.ToolStripProgressBar1.Name = "ToolStripProgressBar1"
        Me.ToolStripProgressBar1.Size = New System.Drawing.Size(400, 16)
        Me.ToolStripProgressBar1.Style = System.Windows.Forms.ProgressBarStyle.Continuous
        '
        'Button1
        '
        Me.Button1.Location = New System.Drawing.Point(336, 147)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(75, 23)
        Me.Button1.TabIndex = 7
        Me.Button1.Text = "OK"
        Me.Button1.UseVisualStyleBackColor = True
        '
        'chkXML
        '
        Me.chkXML.AutoSize = True
        Me.chkXML.Checked = True
        Me.chkXML.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkXML.Location = New System.Drawing.Point(13, 132)
        Me.chkXML.Name = "chkXML"
        Me.chkXML.Size = New System.Drawing.Size(105, 17)
        Me.chkXML.TabIndex = 8
        Me.chkXML.Text = "Dump XML Data"
        Me.chkXML.UseVisualStyleBackColor = True
        '
        'DumpADT
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(423, 195)
        Me.Controls.Add(Me.chkXML)
        Me.Controls.Add(Me.Button1)
        Me.Controls.Add(Me.StatusStrip1)
        Me.Controls.Add(Me.chkCombined)
        Me.Controls.Add(Me.chkDepthmap)
        Me.Controls.Add(Me.chkLayermaps)
        Me.Controls.Add(Me.chkAlphamaps)
        Me.Controls.Add(Me.chkTextures)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "DumpADT"
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Dump ADT"
        Me.StatusStrip1.ResumeLayout(False)
        Me.StatusStrip1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents chkTextures As System.Windows.Forms.CheckBox
    Friend WithEvents chkAlphamaps As System.Windows.Forms.CheckBox
    Friend WithEvents chkLayermaps As System.Windows.Forms.CheckBox
    Friend WithEvents chkDepthmap As System.Windows.Forms.CheckBox
    Friend WithEvents chkCombined As System.Windows.Forms.CheckBox
    Friend WithEvents StatusStrip1 As System.Windows.Forms.StatusStrip
    Friend WithEvents ToolStripProgressBar1 As System.Windows.Forms.ToolStripProgressBar
    Friend WithEvents Button1 As System.Windows.Forms.Button
    Friend WithEvents chkXML As System.Windows.Forms.CheckBox

End Class
