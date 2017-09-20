Imports System.IO

Module vecMC33P116
    Const ID As UInt16 = &H3316
    Const ID_ADDR As UInt16 = &HFFFF
    Const OTP_START As UInt16 = 0           'Valid lowest address
    Const OTP_END As UInt16 = &H3FFF         'Valid highest address
    Const ADDRL As UInt16 = 2 * OTP_START
    Const ADDRH As UInt16 = 2 * OTP_END + 1

    Const PAGE As UInt16 = &HFFB0       'One Page

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
    Dim OPT1 As UInt32
    Dim OPT As UInt32
    Dim sNameEnc As String
    Dim CheckSum As Integer
    Dim nNotFF As Integer

    Function GenVec(ByVal sChip As String, ByVal sProj As String, ByVal sS19 As String, _
            ByVal nOPT As UInt32, ByVal nCksum As UInt16) As Boolean

        Dim sPath As String

        sPath = My.Computer.FileSystem.GetParentPath(sS19) & "\" & sChip & "_" & sProj.ToUpper & "\"
        sNameS19 = sS19
        sNameCode = sPath & "CODE_" & sProj.ToUpper & ".HEX"
        sNameWri = sPath & "WRITE_" & sProj.ToUpper & ".VEC"
        sNameVer = sPath & "VERIFY_" & sProj.ToUpper & ".VEC"
        sNameEnc = sPath & "WRITE_ENC_" & sProj.ToUpper & ".VEC"
        sLogFile = My.Computer.FileSystem.GetParentPath(sS19) & "\" & sChip & "_" & sProj.ToUpper & ".log"
        sNow = Now
        OPT = nOPT
        sLog = sNow & vbCrLf

        OPT1 = OPT Or &H8000L
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
        sLog &= "OPTION: " & OPT.ToString("X8") & vbCrLf
        sLog &= "Checksum: " & CheckSum.ToString("X4") & vbCrLf
        sLog &= "Count of byte which is not FFFFH: " & nNotFF.ToString & vbCrLf

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

                    If sLine.StartsWith("S0") Then        'Some S19 file started with a S0xxxxx line, skip such line
                        Continue While
                    ElseIf sLine.StartsWith("S3") Then
                        Continue While


                    ElseIf sLine.StartsWith("S1") Then    'Valid data line begin with S1
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
        For i = 0 To OTP_END - OTP_START
            CheckSum = CheckSum + OTP(2 * i)
            CheckSum = CheckSum + OTP(2 * i + 1)
            If OTP(2 * i) <> &HFF Or OTP(2 * i + 1) <> &HFF Then
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

            VecHead()
            CmdAddress(ID_ADDR)
            CmdDataOut(ID)
            CmdAddress(&H8000)  'write PAGE
            CmdDataIn(PAGE)
            CmdProg()
            CmdAddress(&H8002)  'write OPBIT1
            CmdDataIn(OPT1 And &HFFFFL)
            CmdProg()
            CmdAddress(&H8004)  'write OPBIT3
            CmdDataIn((OPT1 >> 16) And &HFFFFL)
            CmdProg()

            'CmdDataIn(OPT1 And &HFFFFL)
            'CmdProg()
            'CmdAddrInc()
            'CmdDataIn((OPT1 >> 16) And &HFFFFL)
            'CmdProg()

            'CmdAddress(OTP_START)
            'For i = OTP_START To OTP_END
            '    If OTP(2 * i) <> &HFF Or OTP(2 * i + 1) <> &HFF Then
            '        CmdDataIn(OTP(2 * i + 1) * &H100 + OTP(2 * i))
            '        CmdProg()
            '    End If
            '    CmdAddrInc()
            'Next

            'For i = OTP_START To OTP_END
            '    CmdAddress(i)
            '    CmdDataIn(OTP(2 * i + 1) * &H100 + OTP(2 * i))
            '    CmdProg()
            'Next

            Dim d As UInt16
            For i = OTP_START To OTP_END
                d = OTP(2 * i + 1) * &H100 + OTP(2 * i)
                If d <> &HFFFF Then
                    CmdAddress(i)
                    CmdDataIn(d)
                    CmdProg()
                End If
            Next

            VecTail()

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

            VecHead()
            CmdAddress(ID_ADDR)
            CmdDataOut(ID)
            CmdAddress(&H8000)  'read PAGE
            CmdDataOut(PAGE)
            CmdAddress(&H8002)  'read OPBIT1
            CmdDataOut(OPT1 And &HFFFFL)
            CmdAddress(&H8004)  'read OPBIT3
            CmdDataOut((OPT1 >> 16) And &HFFFFL)

            'CmdAddress(&H2000)
            'CmdDataOut(OPT1 And &HFFFFL)
            'CmdAddrInc()
            'CmdDataOut((OPT1 >> 16) And &HFFFFL)

            'CmdAddress(OTP_START)
            For i = OTP_START To OTP_END
                CmdAddress(i)
                CmdDataOut(OTP(2 * i + 1) * &H100 + OTP(2 * i))
                'CmdAddrInc()
            Next
            VecTail()

            sw.Close()

        Catch ex As Exception
            MessageBox.Show(ex.Message.ToString)
            End
        End Try
    End Sub

    Private Sub GenEnc()
        Try
            sw = New StreamWriter(sNameEnc)

            VecHead()
            CmdAddress(ID_ADDR)
            CmdDataOut(ID)
            CmdAddress(&H8002)
            CmdDataIn(&H7FFF)
            CmdProg()
            CmdDataOut(OPT And &HFFFFL)
            VecTail()

            sw.Close()

        Catch ex As Exception
            MessageBox.Show(ex.Message.ToString)
            End
        End Try
    End Sub

    Private Sub VecHead()
        sw.WriteLine("HEAD P52,P51,P50,P47,P46,P45,P44,P43,P42,P41,P40,P37,P36,P35,P34,P33,P32,P31,P30,P27,P26,P25,P24,P23,P22,P21,P20,P17,P16,P15,P14,P13,P12,P11,P10,P07,P06,P05,P04,P03,P02,P01,P00,P55,P54,P53;")
        'sw.WriteLine("START:RPT 1000 (XXXXXXX00XXX)TS0;")
    End Sub

    Private Sub VecTail()
        'sw.WriteLine("STOP:          (XXXXXXX00XXX);   //ALL PIN OUT HIGH")
    End Sub


    Private Sub SendCommand(ByVal cmd As Byte)
        Dim i As Integer
        Dim m As Byte
        Dim sVec As String

        sVec = ""

        sVec &= String.Format("RPT 5                (XXX0XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX);   //Command 0x{0:X2}", cmd) & vbCrLf

        m = &H1
        For i = 0 To 5
            If cmd And m Then
                sVec &= String.Format("                     (XXX01XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX);") & vbCrLf
                sVec &= String.Format("                     (XXX11XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX);   //T{0:D} cmd[{1:D}]", i, i) & vbCrLf
                sVec &= String.Format("                     (XXX01XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX);") & vbCrLf
            Else
                sVec &= String.Format("                     (XXX00XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX);") & vbCrLf
                sVec &= String.Format("                     (XXX10XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX);   //T{0:D} cmd[{1:D}]", i, i) & vbCrLf
                sVec &= String.Format("                     (XXX00XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX);") & vbCrLf
            End If
            m <<= 1
        Next

        sw.Write(sVec)
    End Sub

    Private Sub AddressIn(ByVal addr As UInt16)
        Dim i As Integer
        Dim m As UInt16
        Dim sVec As String

        sVec = ""

        sVec &= String.Format("RPT 5                (XXX0XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX);   //Address 0x{0:X4}", addr) & vbCrLf
        'sVec &= String.Format("               (XXXXXXX01XXX);   //T{0:D}", 0) & vbCrLf
        'sVec &= String.Format("               (XXXXXXX00XXX);") & vbCrLf

        m = &H1
        For i = 0 To 15
            If addr And m Then
                sVec &= String.Format("                     (XXX01XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX);") & vbCrLf
                sVec &= String.Format("                     (XXX11XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX);   //T{0:D} addr[{1:D}]", i, i) & vbCrLf
                sVec &= String.Format("                     (XXX01XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX);") & vbCrLf
            Else
                sVec &= String.Format("                     (XXX00XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX);") & vbCrLf
                sVec &= String.Format("                     (XXX10XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX);   //T{0:D} addr[{1:D}]", i, i) & vbCrLf
                sVec &= String.Format("                     (XXX00XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX);") & vbCrLf
            End If
            m <<= 1
        Next

        'sVec &= String.Format("               (XXXXXXX01XXX);   //T{0:D}", 15) & vbCrLf
        'sVec &= String.Format("               (XXXXXXX00XXX);") & vbCrLf

        sw.Write(sVec)
    End Sub

    Private Sub DataIn(ByVal data As UInt16)
        Dim i As Integer
        Dim m As UInt16
        Dim sVec As String

        sVec = ""

        sVec &= String.Format("RPT 5                (XXX0XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX);   //DataIn 0x{0:X4}", data) & vbCrLf
        'sVec &= String.Format("               (XXXXXXX01XXX);   //T{0:D}", 0) & vbCrLf
        'sVec &= String.Format("               (XXXXXXX00XXX);") & vbCrLf

        m = &H1
        For i = 0 To 15
            If data And m Then
                sVec &= String.Format("                     (XXX01XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX);") & vbCrLf
                sVec &= String.Format("                     (XXX11XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX);   //T{0:D} data[{1:D}]", i, i) & vbCrLf
                sVec &= String.Format("                     (XXX01XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX);") & vbCrLf
            Else
                sVec &= String.Format("                     (XXX00XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX);") & vbCrLf
                sVec &= String.Format("                     (XXX10XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX);   //T{0:D} data[{1:D}]", i, i) & vbCrLf
                sVec &= String.Format("                     (XXX00XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX);") & vbCrLf
            End If
            m <<= 1
        Next

        'sVec &= String.Format("               (XXXXXXX01XXX);   //T{0:D}", 15) & vbCrLf
        'sVec &= String.Format("               (XXXXXXX00XXX);") & vbCrLf

        sw.Write(sVec)
    End Sub

    Private Sub DataOut(ByVal data As UInt16)
        Dim i As Integer
        Dim m As UInt16
        Dim sVec As String

        sVec = ""

        sVec &= String.Format("RPT 5                (XXX0XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX);   //DataOut 0x{0:X4}", data) & vbCrLf
        'sVec &= String.Format("               (XXXXXXX01XXX);   //T{0:D}", 0) & vbCrLf
        'sVec &= String.Format("               (XXXXXXX00XXX);") & vbCrLf

        m = &H1
        For i = 0 To 15
            If data And m Then
                sVec &= String.Format("                     (XXX00XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX);") & vbCrLf
                sVec &= String.Format("                     (XXX10XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX);   //T{0:D} data[{1:D}]", i, i) & vbCrLf
                sVec &= String.Format("                     (XXX00HXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX);") & vbCrLf
            Else
                sVec &= String.Format("                     (XXX00XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX);") & vbCrLf
                sVec &= String.Format("                     (XXX10XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX);   //T{0:D} data[{1:D}]", i, i) & vbCrLf
                sVec &= String.Format("                     (XXX00LXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX);") & vbCrLf
            End If
            m <<= 1
        Next

        'sVec &= String.Format("               (XXXXXXX01XXX);   //T{0:D}", 15) & vbCrLf
        'sVec &= String.Format("               (XXXXXXX00XXX);") & vbCrLf

        sw.Write(sVec)
    End Sub

    Private Sub CmdAddress(ByVal addr As UInt16)
        SendCommand(&H15)
        AddressIn(addr)
    End Sub

    Private Sub CmdDataIn(ByVal data As UInt16)
        SendCommand(&H16)
        DataIn(data)
    End Sub

    Private Sub CmdDataOut(ByVal data As UInt16)
        SendCommand(&H17)
        DataOut(data)
    End Sub

    Private Sub CmdAddrInc()
        SendCommand(&H14)
    End Sub

    Private Sub CmdProg()
        SendCommand(&H19)
        ProgCtrl()
        'sw.WriteLine("RPT 1000       (XXXXXXX00XXX);   //wait 200us")
        'SendCommand(&H1A)
    End Sub

    Private Sub ProgCtrl()
        Dim i As Integer
        Dim sVec As String

        sVec = ""

        For i = 0 To 7
            sVec &= String.Format("                     (XXX1XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX);   //T{0:D}", i) & vbCrLf
            sVec &= String.Format("RPT 5                (XXX0XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX);") & vbCrLf
        Next
        sVec &= String.Format("RPT 1000             (XXX0XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX);   //wait prog time") & vbCrLf
        For i = 0 To 3
            sVec &= String.Format("                     (XXX1XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX);   //T{0:D}", i) & vbCrLf
            sVec &= String.Format("RPT 5                (XXX0XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX);") & vbCrLf
        Next
        sVec &= String.Format("RPT 20               (XXX0XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX);") & vbCrLf
        For i = 0 To 3
            sVec &= String.Format("                     (XXX1XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX);   //T{0:D}", i) & vbCrLf
            sVec &= String.Format("RPT 5                (XXX0XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX);") & vbCrLf
        Next
        sw.Write(sVec)
    End Sub

    Private Sub GenCode()
        Try
            sw = New StreamWriter(sNameCode)
            Dim i As Integer
            Dim nCode As UInt16
            Dim nCk As UInt32

            nCk = 0
            'For i = ADDRL To ADDRH
            For i = OTP_START To OTP_END
                'CmdAddress(i)
                'CmdDataIn(OTP(2 * i + 1) * &H100 + OTP(2 * i))
                'CmdProg()
                'nCode = OTP(i)
                nCk = nCk + OTP(2 * i) + OTP(2 * i + 1)
                nCode = OTP(2 * i + 1) * &H100 + OTP(2 * i)
                'sw.WriteLine(i.ToString("X4") & ", " & nCode.ToString("X2") & ", " & nCode.ToString)
                sw.WriteLine(i.ToString("X4") & ", " & nCode.ToString("X4") & ", " & nCk.ToString("X16"))
            Next

            sw.Close()

        Catch ex As Exception
            MessageBox.Show(ex.Message.ToString)
            End
        End Try
    End Sub

End Module
