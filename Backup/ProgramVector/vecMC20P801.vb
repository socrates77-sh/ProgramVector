Imports System.IO

Module vecMC20P801
    Const ID As Integer = &H281
    Const ADDRL As Integer = &H1C00            'Valid lowest address
    Const ADDRH As Integer = &H1FFF            'Valid highest address
    Dim OTP(ADDRH - ADDRL) As Byte      'All data is stored in this array
    Dim sw As StreamWriter

    Dim sName As String
    Dim sLogFile As String
    Dim sLog As String
    Dim sNameS19 As String    'File name of S19
    Dim sNameCode As String   'File name of Code
    Dim sNameWri As String    'File name of Write Vector
    Dim sNameVer As String    'File name of Verify Vector
    Dim sNow As String        'Time
    Dim OPT1 As Integer
    Dim OPT As Integer
    Dim sNameEnc As String
    Dim CheckSum As Integer
    Dim nNotFF As Integer

    Function GenVec(ByVal sChip As String, ByVal sProj As String, ByVal sS19 As String, _
            ByVal nOPT As Integer, ByVal nCksum As Integer) As Boolean

        Dim sPath As String

        sPath = My.Computer.FileSystem.GetParentPath(sS19) & "\" & sChip & "_" & sProj.ToUpper & "\"
        sNameS19 = sS19
        'sNameCode = "CODE_" & sName & ".HEX"
        sNameWri = sPath & "WRITE_" & sProj.ToUpper & ".VEC"
        sNameVer = sPath & "VERIFY_" & sProj.ToUpper & ".VEC"
        sNameEnc = sPath & "WRITE_ENC_" & sProj.ToUpper & ".VEC"
        sLogFile = My.Computer.FileSystem.GetParentPath(sS19) & "\" & sChip & "_" & sProj.ToUpper & ".log"
        sNow = Now
        OPT = nOPT
        sLog = sNow & vbCrLf

        OPT1 = OPT Or &H80
        If Not AnalyzeS19() Then
            MessageBox.Show("S19文件格式错误，或地址溢出", "Warnning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return False
        End If

        If nCksum <> CheckSum Then
            MessageBox.Show("Checksum不符合！", "Warnning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return False
        End If

        sLog &= "Chip: " & sChip & vbCrLf
        sLog &= "Project name: " & sProj.ToUpper & vbCrLf
        sLog &= "OPTION: " & (OPT Mod &H10000).ToString("X2") & vbCrLf
        sLog &= "Checksum: " & CheckSum.ToString("X4") & vbCrLf
        sLog &= "Count of byte which is not FFH: " & nNotFF.ToString & vbCrLf

        If My.Computer.FileSystem.DirectoryExists(sPath) Then
            MessageBox.Show("路径已存在: " & sPath, "Warnning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return False
        End If
        My.Computer.FileSystem.CreateDirectory(sPath)
        'GenCode()
        GenWrite()
        GenVerify()

        sLog &= "(F) " & sNameWri & vbCrLf
        sLog &= "(F) " & sNameVer & vbCrLf
        If OPT1 <> OPT Then
            GenEnc()
            sLog &= "(F) " & sNameEnc & vbCrLf
        End If

        'Console.WriteLine("===================================================================")
        'Console.WriteLine("OPTION: " & (OPT Mod &H10000).ToString("X2"))
        'Console.WriteLine("Checksum: {0:X4}", CheckSum Mod &H10000)
        'Console.WriteLine("Count of byte which is not FFH: " & nNotFF.ToString)

        'Console.WriteLine(vbCrLf & "Press Enter ...")
        'Console.ReadLine()

        Try
            sw = New StreamWriter(sLogFile)
            sw.WriteLine(sLog)
            sw.Close()
        Catch ex As Exception
            MessageBox.Show(ex.Message.ToString)
        End Try

        MessageBox.Show(sLog & "向量生成成功!")
        Return True
    End Function


    'Read S19 file, analyze it, and extract data then store to OTP() array
    Private Function AnalyzeS19() As Boolean
        Try
            Using sr As StreamReader = New StreamReader(sNameS19)
                Dim sLine As String         'Line of S19 file
                Dim sData As String         'sData stores each data
                Dim sLen As String          'sLen sortes data length of a line, in string format
                Dim sAddr As String         'sAddr stores starting address of a line, in string format
                Dim nLen As Integer         'nLen is converted from sLen
                Dim nAddr As Integer        'nData is converted from sAddr
                Dim idx As Integer

                For idx = 0 To ADDRH - ADDRL
                    OTP(idx) = &HFF
                Next

                While Not sr.EndOfStream
                    sLine = sr.ReadLine()   'Get a line of S19 file
                    'Console.WriteLine(sLine)

                    If (sLine.StartsWith("S0")) Then        'Some S19 file started with a S0xxxxx line, skip such line
                        Continue While

                    ElseIf (sLine.StartsWith("S1")) Then    'Valid data line begin with S1
                        sLen = sLine.Substring(2, 2)        'Get data length (include 4 digit address and 2 digit occupied by itself)
                        nLen = Convert.ToInt32(sLen, 16) - 3
                        sAddr = sLine.Substring(4, 4)       'Get starting address
                        nAddr = Convert.ToInt32(sAddr, 16)

                        For idx = 0 To nLen - 1
                            If (nAddr > ADDRH Or nAddr < ADDRL) Then    'if Address is out of range, print error
                                sr.Close()
                                'Console.WriteLine("ERROR: Address out of range! ")
                                'Console.WriteLine(sLine)
                                'End
                                Return False
                            End If
                            sData = sLine.Substring(8 + 2 * idx, 2) 'Get each data
                            OTP(nAddr - ADDRL) = Convert.ToInt32(sData, 16)    'Store data to OTP()
                            nAddr = nAddr + 1
                        Next

                    ElseIf (sLine.StartsWith("S903")) Then  'S9 indicate the end of S19, the line after it will be omitted                       
                        Exit While

                    Else                                    'any other starting is error
                        sr.Close()
                        'Console.WriteLine("ERROR: S19 format error!")
                        'Console.WriteLine(sLine)
                        'End
                        Return False
                    End If
                End While

                sr.Close()

            End Using
        Catch ex As Exception
            'Console.WriteLine(ex.Message)
            MessageBox.Show(ex.Message.ToString)
            End
        End Try

        CheckSum = 0
        nNotFF = 0
        Dim i As Integer
        For i = 0 To ADDRH - ADDRL
            CheckSum = CheckSum + OTP(i)
            If OTP(i) <> &HFF Then
                nNotFF += 1
            End If
        Next
        CheckSum = CheckSum And &HFFFF
        Return True
    End Function

    Private Sub GenWrite()
        Try
            sw = New StreamWriter(sNameWri)
            Dim i As Integer

            'Console.Write(sNameWri & " ")

            sw.WriteLine(";" & sNow)
            sw.WriteLine(";" & "Write Vector" & vbCrLf)

            PinInfo()
            ProgInit(ID)

            For i = ADDRL To ADDRH
                If (OTP(i - ADDRL) <> &HFF) Then Write(i, OTP(i - ADDRL))
                If (i Mod 16) = 0 Then
                    Console.Write(".")
                End If
            Next

            Write(0, OPT1)

            'Console.WriteLine()

            sw.Close()

        Catch ex As Exception
            MessageBox.Show(ex.Message.ToString)
            End
        End Try
    End Sub

    Private Sub GenVerify()
        Try
            sw = New StreamWriter(sNameVer)
            Dim i As Integer

            'Console.Write(sNameVer & " ")

            sw.WriteLine(";" & sNow)
            sw.WriteLine(";" & "Write Vector" & vbCrLf)

            PinInfo()
            ProgInit(ID)

            For i = ADDRL To ADDRH
                Read(i, OTP(i - ADDRL))
                If (i Mod 16) = 0 Then
                    Console.Write(".")
                End If
            Next

            Read(0, OPT1)

            'Console.WriteLine()

            sw.Close()

        Catch ex As Exception
            MessageBox.Show(ex.Message.ToString)
            End
        End Try
    End Sub

    Private Sub GenEnc()
        Try
            sw = New StreamWriter(sNameEnc)

            'Console.Write(sNameEnc & " ")

            sw.WriteLine(";" & sNow)
            sw.WriteLine(";" & "Write Enc Vector" & vbCrLf)

            PinInfo()
            ProgInit(ID)

            Write(0, &H7F)
            Read(0, OPT)

            'Console.WriteLine()

            sw.Close()

        Catch ex As Exception
            MessageBox.Show(ex.Message.ToString)
            End
        End Try
    End Sub

    Private Sub PinInfo()
        sw.Write("//PPPPPP" & vbCrLf)
        sw.Write("//111111" & vbCrLf)
        sw.Write("//213045" & vbCrLf & vbCrLf)
    End Sub

    Private Sub ProgInit(ByVal nID As Integer)
        Dim i As Integer
        Dim m As Integer
        Dim sVec As String

        sVec = ""

        sVec &= String.Format("  XXXX11; RPT 1000;") & vbCrLf & vbCrLf
        sVec &= String.Format("  XXXX11; RPT 10;        //(INIT) ID:{0:X4}", nID) & vbCrLf
        m = &H1000
        For i = 0 To 27
            If i = 0 Then
                sVec &= String.Format("  XXXX10;                //T{0:D}", i) & vbCrLf
                sVec &= String.Format("  XXXX11;") & vbCrLf
            ElseIf i >= 1 And i <= 13 Then
                If (nID And m) = 0 Then
                    sVec &= String.Format("  XXLX00;                //T{0:D} addr[{1:D}]", i, 13 - i) & vbCrLf
                    sVec &= String.Format("  XXLX01;") & vbCrLf
                Else
                    sVec &= String.Format("  XXLX10;                //T{0:D} addr[{1:D}]", i, 13 - i) & vbCrLf
                    sVec &= String.Format("  XXLX11;") & vbCrLf
                End If
                m = m >> 1
            ElseIf i = 27 Then
                sVec &= String.Format("  XXXX10;                //T{0:D}", i) & vbCrLf
                sVec &= String.Format("  XXXX11;") & vbCrLf
            Else
                sVec &= String.Format("  XXLX10;                //T{0:D}", i) & vbCrLf
                sVec &= String.Format("  XXLX11;") & vbCrLf
            End If
        Next

        sw.Write(sVec)
    End Sub

    Private Sub Write(ByVal nAddr As Integer, ByVal nData As Integer)
        Dim i As Integer
        Dim m As Integer
        Dim md As Integer
        Dim sVec As String

        sVec = ""

        sVec &= String.Format("  XXXX11; RPT 10;        //(WR) Addr:{0:X4} Data:{1:X2}", nAddr, nData) & vbCrLf
        m = &H1000
        md = &H80

        For i = 0 To 27
            If i = 0 Then
                sVec &= String.Format("  XXXX00;                //T{0:D}", i) & vbCrLf
                sVec &= String.Format("  XXHX01;") & vbCrLf
            ElseIf i >= 1 And i <= 13 Then
                If (nAddr And m) = 0 Then
                    sVec &= String.Format("  XXHX00;                //T{0:D} addr[{1:D}]", i, 13 - i) & vbCrLf
                    sVec &= String.Format("  XXHX01;") & vbCrLf
                Else
                    sVec &= String.Format("  XXHX10;                //T{0:D} addr[{1:D}]", i, 13 - i) & vbCrLf
                    sVec &= String.Format("  XXHX11;") & vbCrLf
                End If
                m = m >> 1
            ElseIf i >= 17 And i <= 24 Then
                If (nData And md) = 0 Then
                    sVec &= String.Format("  XXHX00;                //T{0:D} data[{1:D}]", i, 24 - i) & vbCrLf
                    sVec &= String.Format("  XXHX01;") & vbCrLf
                Else
                    sVec &= String.Format("  XXHX10;                //T{0:D} data[{1:D}]", i, 24 - i) & vbCrLf
                    sVec &= String.Format("  XXHX11;") & vbCrLf
                End If
                md = md >> 1
            ElseIf i = 14 Then
                sVec &= String.Format("  XXLX10;                //T{0:D}", i) & vbCrLf
                sVec &= String.Format("  XXLX11;") & vbCrLf
            ElseIf i = 26 Then
                sVec &= String.Format("  XXLX10; RPT 1000;      //T{0:D}", i) & vbCrLf
                sVec &= String.Format("  XXLX11; RPT 1000;") & vbCrLf
            ElseIf i = 27 Then
                sVec &= String.Format("  XXXX10;                //T{0:D}", i) & vbCrLf
                sVec &= String.Format("  XXXX11;") & vbCrLf
            Else
                sVec &= String.Format("  XXHX10;                //T{0:D}", i) & vbCrLf
                sVec &= String.Format("  XXHX11;") & vbCrLf
            End If
        Next

        sw.Write(sVec)
    End Sub

    Private Sub Read(ByVal nAddr As Integer, ByVal nData As Integer)
        Dim i As Integer
        Dim m As Integer
        Dim md As Integer
        Dim sVec As String

        sVec = ""

        sVec &= String.Format("  XXXX11; RPT 10;        //(RD) Addr:{0:X4} Data:{1:X2}", nAddr, nData) & vbCrLf
        m = &H1000
        md = &H80

        For i = 0 To 27
            If i = 0 Then
                sVec &= String.Format("  XXXX10;                //T{0:D}", i) & vbCrLf
                sVec &= String.Format("  XXHX11;") & vbCrLf
            ElseIf i >= 1 And i <= 13 Then
                If (nAddr And m) = 0 Then
                    sVec &= String.Format("  XXHX00;                //T{0:D} addr[{1:D}]", i, 13 - i) & vbCrLf
                    sVec &= String.Format("  XXHX01;") & vbCrLf
                Else
                    sVec &= String.Format("  XXHX10;                //T{0:D} addr[{1:D}]", i, 13 - i) & vbCrLf
                    sVec &= String.Format("  XXHX11;") & vbCrLf
                End If
                m = m >> 1
            ElseIf i >= 17 And i <= 24 Then
                If (nData And md) = 0 Then
                    sVec &= String.Format("  XXLX10;                //T{0:D} data[{1:D}]", i, 24 - i) & vbCrLf
                    sVec &= String.Format("  XXXX11;") & vbCrLf
                Else
                    sVec &= String.Format("  XXHX10;                //T{0:D} data[{1:D}]", i, 24 - i) & vbCrLf
                    sVec &= String.Format("  XXXX11;") & vbCrLf
                End If
                md = md >> 1
            ElseIf i = 14 Then
                sVec &= String.Format("  XXLX10;                //T{0:D}", i) & vbCrLf
                sVec &= String.Format("  XXLX11;") & vbCrLf
            ElseIf i = 16 Then
                sVec &= String.Format("  XXXX10;                //T{0:D}", i) & vbCrLf
                sVec &= String.Format("  XXXX11;") & vbCrLf
            ElseIf i = 27 Then
                sVec &= String.Format("  XXXX10;                //T{0:D}", i) & vbCrLf
                sVec &= String.Format("  XXXX11;") & vbCrLf
            Else
                sVec &= String.Format("  XXHX10;                //T{0:D}", i) & vbCrLf
                sVec &= String.Format("  XXHX11;") & vbCrLf
            End If
        Next

        sw.Write(sVec)
    End Sub



End Module
