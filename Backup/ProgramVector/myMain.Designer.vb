<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class myMain
    Inherits System.Windows.Forms.Form

    'Form 重写 Dispose，以清理组件列表。
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

    'Windows 窗体设计器所必需的
    Private components As System.ComponentModel.IContainer

    '注意: 以下过程是 Windows 窗体设计器所必需的
    '可以使用 Windows 窗体设计器修改它。
    '不要使用代码编辑器修改它。
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container
        Me.GroupBox1 = New System.Windows.Forms.GroupBox
        Me.txtProjCode = New System.Windows.Forms.TextBox
        Me.Label1 = New System.Windows.Forms.Label
        Me.txtChecksum = New System.Windows.Forms.TextBox
        Me.Label5 = New System.Windows.Forms.Label
        Me.txtOPTION = New System.Windows.Forms.TextBox
        Me.lblOPTION = New System.Windows.Forms.Label
        Me.btnFileFilter = New System.Windows.Forms.Button
        Me.txtFileName = New System.Windows.Forms.TextBox
        Me.Label3 = New System.Windows.Forms.Label
        Me.cbbChipList = New System.Windows.Forms.ComboBox
        Me.Label2 = New System.Windows.Forms.Label
        Me.btnGen = New System.Windows.Forms.Button
        Me.btnCancel = New System.Windows.Forms.Button
        Me.ttpMain = New System.Windows.Forms.ToolTip(Me.components)
        Me.GroupBox1.SuspendLayout()
        Me.SuspendLayout()
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.txtProjCode)
        Me.GroupBox1.Controls.Add(Me.Label1)
        Me.GroupBox1.Controls.Add(Me.txtChecksum)
        Me.GroupBox1.Controls.Add(Me.Label5)
        Me.GroupBox1.Controls.Add(Me.txtOPTION)
        Me.GroupBox1.Controls.Add(Me.lblOPTION)
        Me.GroupBox1.Controls.Add(Me.btnFileFilter)
        Me.GroupBox1.Controls.Add(Me.txtFileName)
        Me.GroupBox1.Controls.Add(Me.Label3)
        Me.GroupBox1.Controls.Add(Me.cbbChipList)
        Me.GroupBox1.Controls.Add(Me.Label2)
        Me.GroupBox1.Location = New System.Drawing.Point(12, 12)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(282, 169)
        Me.GroupBox1.TabIndex = 0
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "基本信息"
        '
        'txtProjCode
        '
        Me.txtProjCode.Location = New System.Drawing.Point(88, 49)
        Me.txtProjCode.Name = "txtProjCode"
        Me.txtProjCode.Size = New System.Drawing.Size(158, 21)
        Me.txtProjCode.TabIndex = 2
        Me.ttpMain.SetToolTip(Me.txtProjCode, "输入开发代号")
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(20, 53)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(53, 12)
        Me.Label1.TabIndex = 9
        Me.Label1.Text = "开发代号"
        '
        'txtChecksum
        '
        Me.txtChecksum.Location = New System.Drawing.Point(88, 133)
        Me.txtChecksum.Name = "txtChecksum"
        Me.txtChecksum.Size = New System.Drawing.Size(158, 21)
        Me.txtChecksum.TabIndex = 6
        Me.ttpMain.SetToolTip(Me.txtChecksum, "输入Checksum值（双字节16进制数）")
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(20, 137)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(53, 12)
        Me.Label5.TabIndex = 7
        Me.Label5.Text = "Checksum"
        '
        'txtOPTION
        '
        Me.txtOPTION.Location = New System.Drawing.Point(88, 105)
        Me.txtOPTION.Name = "txtOPTION"
        Me.txtOPTION.Size = New System.Drawing.Size(158, 21)
        Me.txtOPTION.TabIndex = 5
        '
        'lblOPTION
        '
        Me.lblOPTION.AutoSize = True
        Me.lblOPTION.Location = New System.Drawing.Point(32, 109)
        Me.lblOPTION.Name = "lblOPTION"
        Me.lblOPTION.Size = New System.Drawing.Size(41, 12)
        Me.lblOPTION.TabIndex = 5
        Me.lblOPTION.Text = "OPTION"
        '
        'btnFileFilter
        '
        Me.btnFileFilter.Font = New System.Drawing.Font("Calibri", 5.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnFileFilter.Location = New System.Drawing.Point(247, 76)
        Me.btnFileFilter.Name = "btnFileFilter"
        Me.btnFileFilter.Size = New System.Drawing.Size(19, 23)
        Me.btnFileFilter.TabIndex = 4
        Me.btnFileFilter.Text = "..."
        Me.ttpMain.SetToolTip(Me.btnFileFilter, "选择S19文件名")
        Me.btnFileFilter.UseVisualStyleBackColor = True
        '
        'txtFileName
        '
        Me.txtFileName.Location = New System.Drawing.Point(88, 77)
        Me.txtFileName.Name = "txtFileName"
        Me.txtFileName.Size = New System.Drawing.Size(158, 21)
        Me.txtFileName.TabIndex = 3
        Me.ttpMain.SetToolTip(Me.txtFileName, "输入S19文件名")
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(14, 81)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(59, 12)
        Me.Label3.TabIndex = 2
        Me.Label3.Text = "S19文件名"
        '
        'cbbChipList
        '
        Me.cbbChipList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cbbChipList.FormattingEnabled = True
        Me.cbbChipList.Location = New System.Drawing.Point(88, 23)
        Me.cbbChipList.Name = "cbbChipList"
        Me.cbbChipList.Size = New System.Drawing.Size(158, 20)
        Me.cbbChipList.TabIndex = 1
        Me.ttpMain.SetToolTip(Me.cbbChipList, "选择芯片型号")
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(20, 27)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(53, 12)
        Me.Label2.TabIndex = 0
        Me.Label2.Text = "芯片型号"
        '
        'btnGen
        '
        Me.btnGen.Location = New System.Drawing.Point(12, 187)
        Me.btnGen.Name = "btnGen"
        Me.btnGen.Size = New System.Drawing.Size(132, 23)
        Me.btnGen.TabIndex = 7
        Me.btnGen.Text = "生成向量"
        Me.ttpMain.SetToolTip(Me.btnGen, "生成向量，自动检查芯片型号数据容量匹配，checksum值，并生成log或出错信息")
        Me.btnGen.UseVisualStyleBackColor = True
        '
        'btnCancel
        '
        Me.btnCancel.Location = New System.Drawing.Point(162, 187)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(132, 23)
        Me.btnCancel.TabIndex = 8
        Me.btnCancel.Text = "关闭"
        Me.ttpMain.SetToolTip(Me.btnCancel, "取消操作")
        Me.btnCancel.UseVisualStyleBackColor = True
        '
        'myMain
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(305, 224)
        Me.Controls.Add(Me.btnCancel)
        Me.Controls.Add(Me.btnGen)
        Me.Controls.Add(Me.GroupBox1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D
        Me.HelpButton = True
        Me.MaximizeBox = False
        Me.Name = "myMain"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "烧写向量生成器"
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents cbbChipList As System.Windows.Forms.ComboBox
    Friend WithEvents lblOPTION As System.Windows.Forms.Label
    Friend WithEvents btnFileFilter As System.Windows.Forms.Button
    Friend WithEvents txtFileName As System.Windows.Forms.TextBox
    Friend WithEvents txtChecksum As System.Windows.Forms.TextBox
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents txtOPTION As System.Windows.Forms.TextBox
    Friend WithEvents btnGen As System.Windows.Forms.Button
    Friend WithEvents btnCancel As System.Windows.Forms.Button
    Friend WithEvents ttpMain As System.Windows.Forms.ToolTip
    Friend WithEvents txtProjCode As System.Windows.Forms.TextBox
    Friend WithEvents Label1 As System.Windows.Forms.Label

End Class
