'Edition history
'1.0.0 新建 2012/6/12
'1.0.1 增加MC20P801,MC20P02B型号
'1.1.0 增加MC30P011型号
'1.1.1 修改MC30P011地址访问方式 2013/9/3
'1.2.0 增加MC33P116型号 2014/4/10
'1.2.1 修正MC33P116型号PAGE值 2014/4/15
'1.3.0 增加MC32P21型号 2014/5/19
'1.3.1 更正MC32P21型号Write时序 2014/5/21
'1.3.2 更正MC30P011型号Verify时序 2014/5/23
'1.4.0 增加MC9029,MC30P081,MC34P01型号; MC3x全FF数据不出向量 2014/8/11
'1.4.1 更新MC9029 checksum 2014/8/26
'1.4.2 更正MC9029型号时序 2014/8/27
'1.4.3 更正MC30P081型号时序;更正MC33P116加密向量 2015/1/9
'1.4.4 更正MC33P116型号Verify时序 2015/1/23
'1.5.0 增加MC33P78,MC30P6060,MC32P7510,MC32P7022型号 2015/4/7
'1.6.0 增加MC33P74型号 2015/4/1
'1.7.0 增加MC31P11型号 2015/6/15
'1.8.0 增加MC32P7511型号 2016/1/11
'1.8.1 更新MC32P7511向量格式 2016/2/26
'1.9.0 增加MC32P5222型号 2016/3/15
'1.10.0 增加MC9039型号 2016/3/28
'1.11.0 更新MC30P081,MC34P01,MC30P6060 checksum 2016/4/7
'1.12.0 增加MC32P5312型号 2016/9/5
'1.13.0 增加MC32P7031型号 2016/9/19
'1.13.1 更正MC32P5312, MC32P7031向量问题 2016/9/20
'1.13.2 修改MC32P5312的cksum算法 2016/11/2
'1.14.0 增加MC30P6070, MC30P6080, MC30P6090型号 2016/12/14
'1.14.1 修改MC30P6080的cksum算法 2017/2/23
'1.15.0 增加MC32P7030型号 2018/1/5
'1.16.0 增加MC32P8112型号 2018/10/25
'1.17.0 增加MC32P7541型号 2018/11/20
'1.17.1 更正MC32P7541的AddressIn向量问题 2018/11/21
'1.17.2 更正MC32P7541的DataIn向量问题 2018/11/21


Public Class myMain
    Const sVersion As String = "1.17.2"

    Dim saChipList() As String = { _
        "MC20P01", _
        "MC20P24B", _
        "MC20P22", _
        "MC10P11B", _
        "MC10P01B", _
        "MC10P02", _
        "MC20P801", _
        "MC20P02B", _
        "MC30P011", _
        "MC33P116", _
        "MC32P21", _
        "MC9029", _
        "MC30P081", _
        "MC34P01", _
        "MC33P78", _
        "MC30P6060", _
        "MC32P7510", _
        "MC32P7511", _
        "MC32P7022", _
        "MC33P74", _
        "MC31P11", _
        "MC32P5222", _
        "MC9039", _
        "MC32P5312", _
        "MC32P7031", _
        "MC32P7030", _
        "MC30P6080", _
        "MC30P6070", _
        "MC30P6090", _
        "MC32P8112", _
        "MC32P7541"}

    Dim sChipName As String
    Dim sFileName As String
    Dim sProjCode As String
    Dim nOPTION As UInt64
    Dim nChecksum As Integer


    Private Sub myMain_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.Text &= sVersion
        cbbChipList.Sorted = True
        For Each s As String In saChipList
            cbbChipList.Items.Add(s)
        Next
        'cbbChipList.SelectedIndex = 0
        Dim str As String
        str = "MC10/MC20:" & vbTab & "单字节16进制数" & vbCrLf & _
              "MC30P011/MC31P11:" & vbTab & "四字节16进制数 OPBIT1:OPBIT0" & vbCrLf & _
              "MC33P116:" & vbTab & "四字节16进制数 OPBIT3:OPBIT1" & vbCrLf & _
              "MC32P21/MC32P7510:" & vbTab & "两字节16进制数 OPBIT0" & vbCrLf & _
              "MC30P081/MC32P5312:" & vbTab & "六字节16进制数 OPBIT2:OPTION1:OPTION0" & vbCrLf & _
              "MC34P01/MC9039:" & vbTab & "六字节16进制数 OPBIT3:OPTION2:OPTION1" & vbCrLf & _
              "MC33P78/MC33P74/MC32P7022/MC32P7511/MC33P5222/MC32P7031/MC32P7030:" & vbTab & "四字节16进制数 OPTION2:OPTION0" & vbCrLf & _
              "MC30P6060/MC30P6070/MC30P6080:" & vbTab & "六字节16进制数 OPBIT3:OPTION2:OPTION0" & vbCrLf & _
              "MC32P8112:" & vbTab & "六字节16进制数 OPBIT2:OPTION1:OPTION0" & vbCrLf & _
              "MC32P7541:" & vbTab & "两字节16进制数 UOPTBIT" & vbCrLf

        ttpMain.SetToolTip(txtOPTION, str)


    End Sub

    Private Sub btnCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCancel.Click
        End
    End Sub

    Private Sub btnGen_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnGen.Click
        If sChipName = "" Then
            MessageBox.Show("请选择芯片型号!", "Warnning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If

        If sProjCode = "" Then
            MessageBox.Show("请输入开发代号!", "Warnning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If

        If sFileName = "" Or Not My.Computer.FileSystem.FileExists(sFileName) Then
            MessageBox.Show("请输入正确的S19文件名!", "Warnning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If

        Try
            If sChipName.Contains("MC30P011") Or sChipName.Contains("MC33P116") _
                Or sChipName.Contains("MC33P78") Or sChipName.Contains("MC33P74") Or sChipName.Contains("MC32P7022") _
                Or sChipName.Contains("MC31P11") Or sChipName.Contains("MC32P7511") Or sChipName.Contains("MC32P5222") _
                Or sChipName.Contains("MC32P7031") Or sChipName.Contains("MC32P7030") Then
                nOPTION = Convert.ToUInt32(txtOPTION.Text, 16)
                If nOPTION > &HFFFFFFFFL Then
                    MessageBox.Show("请输入正确的OPTION值!", "Warnning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Exit Sub
                End If
            ElseIf sChipName.Contains("MC32P21") Or sChipName.Contains("MC32P7510") Then
                nOPTION = Convert.ToUInt32(txtOPTION.Text, 16)
                If nOPTION > &HFFFFL Then
                    MessageBox.Show("请输入正确的OPTION值!", "Warnning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Exit Sub
                End If
            ElseIf sChipName.Contains("MC30P081") Or sChipName.Contains("MC32P5312") _
                Or sChipName.Contains("MC34P01") Or sChipName.Contains("MC9039") Or sChipName.Contains("MC30P6060") _
                Or sChipName.Contains("MC30P6080") Or sChipName.Contains("MC30P6070") Or sChipName.Contains("MC30P6090") _
                Or sChipName.Contains("MC32P8112") Then
                nOPTION = Convert.ToUInt64(txtOPTION.Text, 16)
                If nOPTION > &HFFFFFFFFFFFFL Then
                    MessageBox.Show("请输入正确的OPTION值!", "Warnning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Exit Sub
                End If
            ElseIf sChipName.Contains("MC32P7541") Then
                nOPTION = Convert.ToUInt32(txtOPTION.Text, 16)
                If nOPTION > &HFFFFL Then
                    MessageBox.Show("请输入正确的UOPTBIT值!", "Warnning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Exit Sub
                End If
            ElseIf sChipName.Contains("MC9029") Then
                nOPTION = 0
            Else
                nOPTION = Convert.ToUInt32(txtOPTION.Text, 16)
                If nOPTION > &HFF Then
                    MessageBox.Show("请输入正确的OPTION值!", "Warnning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Exit Sub
                End If
            End If

        Catch ex As Exception
            MessageBox.Show("请输入正确的OPTION值!", "Warnning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End Try

        Try
            nChecksum = Convert.ToInt32(txtChecksum.Text, 16)
            If nChecksum > &HFFFF Or nOPTION < 0 Then
                MessageBox.Show("请输入正确的Checksum值!", "Warnning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Sub
            End If
        Catch ex As Exception
            MessageBox.Show("请输入正确的Checksum值!", "Warnning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End Try

        btnGen.Enabled = False
        btnCancel.Enabled = False

        'for debug
        'sChipName = "MC32P7541"
        'sProjCode = "a"
        'sFileName = "D:\work\tool\烧写向量生成器\YTE1408T-7541-7A63-181108-6.s19"
        'nOPTION = &H2131
        'nChecksum = &H7A63
        ''My.Computer.FileSystem.DeleteDirectory("D:\work\tool\烧写向量生成器\MC32P7541_A", FileIO.DeleteDirectoryOption.DeleteAllContents)


        Select Case sChipName
            Case "MC20P01"
                vecMC20P01.GenVec(sChipName, sProjCode, sFileName, nOPTION, nChecksum)
            Case "MC20P24B"
                vecMC20P24B.GenVec(sChipName, sProjCode, sFileName, nOPTION, nChecksum)
            Case "MC20P22"
                vecMC20P22.GenVec(sChipName, sProjCode, sFileName, nOPTION, nChecksum)
            Case "MC10P01B"
                vecMC10P01B.GenVec(sChipName, sProjCode, sFileName, nOPTION, nChecksum)
            Case "MC10P02"
                vecMC10P02.GenVec(sChipName, sProjCode, sFileName, nOPTION, nChecksum)
            Case "MC10P11B"
                vecMC10P11B.GenVec(sChipName, sProjCode, sFileName, nOPTION, nChecksum)
            Case "MC20P801"
                vecMC20P801.GenVec(sChipName, sProjCode, sFileName, nOPTION, nChecksum)
            Case "MC20P02B"
                vecMC20P02B.GenVec(sChipName, sProjCode, sFileName, nOPTION, nChecksum)
            Case "MC30P011"
                vecMC30P011.GenVec(sChipName, sProjCode, sFileName, nOPTION, nChecksum)
            Case "MC33P116"
                vecMC33P116.GenVec(sChipName, sProjCode, sFileName, nOPTION, nChecksum)
            Case "MC32P21"
                vecMC32P21.GenVec(sChipName, sProjCode, sFileName, nOPTION, nChecksum)
            Case "MC9029"
                vecMC9029.GenVec(sChipName, sProjCode, sFileName, nOPTION, nChecksum)
            Case "MC30P081"
                vecMC30P081.GenVec(sChipName, sProjCode, sFileName, nOPTION, nChecksum)
            Case "MC32P5312"
                vecMC32P5312.GenVec(sChipName, sProjCode, sFileName, nOPTION, nChecksum)
            Case "MC34P01"
                vecMC34P01.GenVec(sChipName, sProjCode, sFileName, nOPTION, nChecksum)
            Case "MC9039"
                vecMC9039.GenVec(sChipName, sProjCode, sFileName, nOPTION, nChecksum)
            Case "MC33P78"
                vecMC33P78.GenVec(sChipName, sProjCode, sFileName, nOPTION, nChecksum)
            Case "MC33P74"
                vecMC33P74.GenVec(sChipName, sProjCode, sFileName, nOPTION, nChecksum)
            Case "MC30P6060"
                vecMC30P6060.GenVec(sChipName, sProjCode, sFileName, nOPTION, nChecksum)
            Case "MC30P6080"
                vecMC30P6080.GenVec(sChipName, sProjCode, sFileName, nOPTION, nChecksum)
            Case "MC30P6070"
                vecMC30P6070.GenVec(sChipName, sProjCode, sFileName, nOPTION, nChecksum)
            Case "MC30P6090"
                vecMC30P6090.GenVec(sChipName, sProjCode, sFileName, nOPTION, nChecksum)
            Case "MC32P7510"
                vecMC32P7510.GenVec(sChipName, sProjCode, sFileName, nOPTION, nChecksum)
            Case "MC32P7511"
                vecMC32P7511.GenVec(sChipName, sProjCode, sFileName, nOPTION, nChecksum)
            Case "MC32P5222"
                vecMC32P5222.GenVec(sChipName, sProjCode, sFileName, nOPTION, nChecksum)
            Case "MC32P7022"
                vecMC32P7022.GenVec(sChipName, sProjCode, sFileName, nOPTION, nChecksum)
            Case "MC31P11"
                vecMC31P11.GenVec(sChipName, sProjCode, sFileName, nOPTION, nChecksum)
            Case "MC32P7031"
                vecMC32P7031.GenVec(sChipName, sProjCode, sFileName, nOPTION, nChecksum)
            Case "MC32P7030"
                vecMC32P7030.GenVec(sChipName, sProjCode, sFileName, nOPTION, nChecksum)
            Case "MC32P8112"
                vecMC32P8112.GenVec(sChipName, sProjCode, sFileName, nOPTION, nChecksum)
            Case "MC32P7541"
                vecMC32P7541.GenVec(sChipName, sProjCode, sFileName, nOPTION, nChecksum)
            Case Else
                MessageBox.Show("该型号暂不支持", "Warnning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End Select

        btnGen.Enabled = True
        btnCancel.Enabled = True
    End Sub

    Private Sub cbbChipList_SelectedValueChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cbbChipList.SelectedValueChanged
        sChipName = cbbChipList.SelectedItem.ToString
        If sChipName.Contains("MC9029") Then
            lblOPTION.Visible = False
            txtOPTION.Visible = False
        Else
            lblOPTION.Visible = True
            txtOPTION.Visible = True
        End If
    End Sub


    Private Sub txtFileName_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtFileName.TextChanged
        sFileName = txtFileName.Text
    End Sub


    Private Sub btnFileFilter_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnFileFilter.Click
        Dim dlgOpen As New OpenFileDialog

        dlgOpen.Filter = "s19文件(*.s19)|*.s19"
        dlgOpen.InitialDirectory = My.Application.Info.DirectoryPath

        If dlgOpen.ShowDialog() = Windows.Forms.DialogResult.OK Then
            txtFileName.Text = dlgOpen.FileName
        End If
    End Sub

    Private Sub txtProjCode_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtProjCode.TextChanged
        sProjCode = txtProjCode.Text
    End Sub

End Class
