<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class HexViewer
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
        Me.MenuStrip1 = New System.Windows.Forms.MenuStrip
        Me.FindToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.GotoToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.FindToolStripMenuItem1 = New System.Windows.Forms.ToolStripMenuItem
        Me.HexBox1 = New Be.Windows.Forms.HexBox
        Me.StatusStrip1 = New System.Windows.Forms.StatusStrip
        Me.ToolStripStatusLabel1 = New System.Windows.Forms.ToolStripStatusLabel
        Me.MenuStrip1.SuspendLayout()
        Me.StatusStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'MenuStrip1
        '
        Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.FindToolStripMenuItem})
        Me.MenuStrip1.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip1.Name = "MenuStrip1"
        Me.MenuStrip1.Size = New System.Drawing.Size(658, 24)
        Me.MenuStrip1.TabIndex = 1
        Me.MenuStrip1.Text = "MenuStrip1"
        '
        'FindToolStripMenuItem
        '
        Me.FindToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.GotoToolStripMenuItem, Me.FindToolStripMenuItem1})
        Me.FindToolStripMenuItem.Name = "FindToolStripMenuItem"
        Me.FindToolStripMenuItem.Size = New System.Drawing.Size(37, 20)
        Me.FindToolStripMenuItem.Text = "File"
        '
        'GotoToolStripMenuItem
        '
        Me.GotoToolStripMenuItem.Name = "GotoToolStripMenuItem"
        Me.GotoToolStripMenuItem.Size = New System.Drawing.Size(112, 22)
        Me.GotoToolStripMenuItem.Text = "Goto ..."
        '
        'FindToolStripMenuItem1
        '
        Me.FindToolStripMenuItem1.Name = "FindToolStripMenuItem1"
        Me.FindToolStripMenuItem1.Size = New System.Drawing.Size(152, 22)
        Me.FindToolStripMenuItem1.Text = "Find ..."
        '
        'HexBox1
        '
        Me.HexBox1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.HexBox1.Font = New System.Drawing.Font("Courier New", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.HexBox1.LineInfoForeColor = System.Drawing.Color.Empty
        Me.HexBox1.LineInfoVisible = True
        Me.HexBox1.Location = New System.Drawing.Point(0, 24)
        Me.HexBox1.Name = "HexBox1"
        Me.HexBox1.ReadOnly = True
        Me.HexBox1.ShadowSelectionColor = System.Drawing.Color.FromArgb(CType(CType(100, Byte), Integer), CType(CType(60, Byte), Integer), CType(CType(188, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.HexBox1.Size = New System.Drawing.Size(658, 611)
        Me.HexBox1.StringViewVisible = True
        Me.HexBox1.TabIndex = 2
        Me.HexBox1.UseFixedBytesPerLine = True
        Me.HexBox1.VScrollBarVisible = True
        '
        'StatusStrip1
        '
        Me.StatusStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripStatusLabel1})
        Me.StatusStrip1.Location = New System.Drawing.Point(0, 635)
        Me.StatusStrip1.Name = "StatusStrip1"
        Me.StatusStrip1.Size = New System.Drawing.Size(658, 22)
        Me.StatusStrip1.TabIndex = 3
        Me.StatusStrip1.Text = "StatusStrip1"
        '
        'ToolStripStatusLabel1
        '
        Me.ToolStripStatusLabel1.Name = "ToolStripStatusLabel1"
        Me.ToolStripStatusLabel1.Size = New System.Drawing.Size(643, 17)
        Me.ToolStripStatusLabel1.Spring = True
        Me.ToolStripStatusLabel1.Text = "ToolStripStatusLabel1"
        '
        'HexViewer
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(658, 657)
        Me.Controls.Add(Me.HexBox1)
        Me.Controls.Add(Me.StatusStrip1)
        Me.Controls.Add(Me.MenuStrip1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow
        Me.MainMenuStrip = Me.MenuStrip1
        Me.Name = "HexViewer"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "HexViewer"
        Me.MenuStrip1.ResumeLayout(False)
        Me.MenuStrip1.PerformLayout()
        Me.StatusStrip1.ResumeLayout(False)
        Me.StatusStrip1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents MenuStrip1 As System.Windows.Forms.MenuStrip
    Friend WithEvents FindToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents GotoToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents FindToolStripMenuItem1 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents HexBox1 As Be.Windows.Forms.HexBox
    Friend WithEvents StatusStrip1 As System.Windows.Forms.StatusStrip
    Friend WithEvents ToolStripStatusLabel1 As System.Windows.Forms.ToolStripStatusLabel

End Class
