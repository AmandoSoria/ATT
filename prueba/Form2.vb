Imports System.IO
Imports System.Data
Imports System.Text
Imports System.Text.Encoding
Imports System.Linq
Imports ExcelDataReader
Imports Microsoft.Web.WebView2.WinForms
Imports Microsoft.Web.WebView2.Core

Public Class Form2

    ' === Editores dinámicos (D en adelante) ===
    Private _editors As New Dictionary(Of Integer, TextBox)
    Private Const FIRST_DYNAMIC_COL As Integer = 3 ' D => índice 3 (A=0,B=1,C=2,D=3)

    ' ======== Excel ========
    Private _rutaActual As String = Nothing
    Private _dsActual As DataSet = Nothing

    ' ======== PDF ========
    Private _carpetaPdf As String = Nothing
    Private Const COL_FOLIO As Integer = 2   ' Columna C (0=A,1=B,2=C)

    ' ======== UI creados por código ========
    Private PnlColumnasHost As Panel          ' contenedor en (467,124)
    Private PnlColumnas As Panel              ' panel interno con AutoScroll
    Private WithEvents ChkComentariosOK As CheckBox
    Private Const COL_Y_INDEX As Integer = 24 ' Columna Y (0-based)

    Private Async Sub Form2_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' Necesario para .xls antiguos
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance)

        ' ----- Panel con scroll para las columnas D.. -----
        PnlColumnasHost = New Panel() With {
            .Name = "PnlColumnasHost",
            .Location = New Point(467, 124), ' <- como pediste
            .Size = New Size(360, 360),
            .BorderStyle = BorderStyle.FixedSingle,
            .Visible = False
        }
        PnlColumnas = New Panel() With {
            .Dock = DockStyle.Fill,
            .AutoScroll = True
        }
        PnlColumnasHost.Controls.Add(PnlColumnas)
        Me.Controls.Add(PnlColumnasHost)

        ' ----- CheckBox "Comentarios = OK" -----
        ChkComentariosOK = New CheckBox() With {
            .Name = "ChkComentariosOK",
            .Text = "Comentarios = OK",
            .AutoSize = True,
            .Enabled = False
        }
        ChkComentariosOK.Left = PnlColumnasHost.Left
        ChkComentariosOK.Top = PnlColumnasHost.Bottom + 8
        Me.Controls.Add(ChkComentariosOK)

        ' UI base
        Btn_Excel.Cursor = Cursors.Hand
        PrepararGrid(DG_Excel)
        ChkEncabezados.Checked = True
        CbHojas.DropDownStyle = ComboBoxStyle.DropDownList

        ' Inicializa WebView2 (visor PDF)
        Try
            If PdfView IsNot Nothing Then
                Await PdfView.EnsureCoreWebView2Async()
            End If
        Catch
            ' Si falta el runtime, el control no funcionará hasta instalarlo.
        End Try
    End Sub

    ' ----------------- Abrir Excel -----------------
    Private Sub Btn_Excel_Click(sender As Object, e As EventArgs) Handles Btn_Excel.Click
        Using ofd As New OpenFileDialog()
            ofd.Title = "Selecciona un archivo de Excel o CSV"
            ofd.Filter = "Excel (*.xlsx;*.xls)|*.xlsx;*.xls|CSV (*.csv)|*.csv|Todos (*.*)|*.*"
            ofd.Multiselect = False

            If ofd.ShowDialog() = DialogResult.OK Then
                _rutaActual = ofd.FileName
                CargarArchivoEnDataSet()
                PoblarComboHojasYMostrar()
            End If
        End Using
    End Sub

    Private Sub ChkEncabezados_CheckedChanged(sender As Object, e As EventArgs) Handles ChkEncabezados.CheckedChanged
        If String.IsNullOrEmpty(_rutaActual) Then Return
        CargarArchivoEnDataSet()
        PoblarComboHojasYMostrar()
    End Sub

    Private Sub CbHojas_SelectedIndexChanged(sender As Object, e As EventArgs) Handles CbHojas.SelectedIndexChanged
        MostrarHojaSeleccionada()
    End Sub

    Private Sub CargarArchivoEnDataSet()
        If String.IsNullOrEmpty(_rutaActual) Then
            _dsActual = Nothing : Return
        End If

        Dim ext = Path.GetExtension(_rutaActual).ToLowerInvariant()

        If ext = ".csv" Then
            Dim dt = LeerCsv(_rutaActual, ChkEncabezados.Checked)
            _dsActual = New DataSet()
            dt.TableName = "CSV"
            _dsActual.Tables.Add(dt)
            Return
        End If

        Using fs = File.Open(_rutaActual, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)
            Using reader As IExcelDataReader =
                If(ext = ".xls",
                   ExcelReaderFactory.CreateBinaryReader(fs),
                   ExcelReaderFactory.CreateOpenXmlReader(fs))

                Dim conf As New ExcelDataSetConfiguration() With {
                    .UseColumnDataType = True,
                    .ConfigureDataTable = Function(__) New ExcelDataTableConfiguration() With {
                        .UseHeaderRow = ChkEncabezados.Checked
                    }
                }
                _dsActual = reader.AsDataSet(conf)
            End Using
        End Using
    End Sub

    Private Sub PoblarComboHojasYMostrar()
        CbHojas.Items.Clear()
        If _dsActual Is Nothing OrElse _dsActual.Tables.Count = 0 Then
            DG_Excel.DataSource = Nothing
            PnlColumnasHost.Visible = False
            ChkComentariosOK.Enabled = False
            Return
        End If

        For Each t As DataTable In _dsActual.Tables
            CbHojas.Items.Add(t.TableName)
        Next
        If CbHojas.Items.Count > 0 Then CbHojas.SelectedIndex = 0
        MostrarHojaSeleccionada()
    End Sub

    Private Sub MostrarHojaSeleccionada()
        If _dsActual Is Nothing OrElse _dsActual.Tables.Count = 0 Then
            DG_Excel.DataSource = Nothing
            PnlColumnasHost.Visible = False
            ChkComentariosOK.Enabled = False
            Return
        End If

        Dim idx As Integer = If(CbHojas.SelectedIndex >= 0, CbHojas.SelectedIndex, 0)
        DG_Excel.DataSource = _dsActual.Tables(idx)
        AutoAjustarColumnas(DG_Excel)

        ConstruirEditoresColumnas()
        ActualizarEditoresDesdeFilaActual()
        SincronizarChkOkConFila()
    End Sub

    ' ====== Construye los editores (labels+textbox) dentro del panel con scroll ======
    Private Sub ConstruirEditoresColumnas()
        _editors.Clear()
        PnlColumnas.Controls.Clear()

        If DG_Excel.DataSource Is Nothing Then
            PnlColumnasHost.Visible = False
            Return
        End If

        Dim totalCols = DG_Excel.Columns.Count
        If totalCols <= FIRST_DYNAMIC_COL Then
            PnlColumnasHost.Visible = False
            Return
        End If

        PnlColumnasHost.SuspendLayout()
        PnlColumnas.SuspendLayout()

        Dim x As Integer = 10
        Dim y As Integer = 10
        Dim labelWidth As Integer = 120
        Dim spacing As Integer = 8
        Dim lineHeight As Integer = 26

        ' Ancho disponible dentro del panel con scroll
        Dim tbWidth As Integer = Math.Max(120, PnlColumnas.ClientSize.Width - x - labelWidth - spacing - 25)

        For colIndex As Integer = FIRST_DYNAMIC_COL To totalCols - 1
            Dim col = DG_Excel.Columns(colIndex)
            Dim header As String = If(String.IsNullOrWhiteSpace(col.HeaderText),
                                      $"Col {colIndex + 1}", col.HeaderText)

            Dim lbl As New Label() With {
                .AutoSize = False,
                .Text = header,
                .Left = x,
                .Top = y + 5,
                .Width = labelWidth
            }

            Dim tb As New TextBox() With {
                .Name = $"TbCol_{colIndex}",
                .Left = x + labelWidth + spacing,
                .Top = y,
                .Width = tbWidth,
                .Tag = colIndex,
                .ReadOnly = True
            }

            PnlColumnas.Controls.Add(lbl)
            PnlColumnas.Controls.Add(tb)
            _editors(colIndex) = tb

            y += lineHeight
        Next

        PnlColumnasHost.Visible = True
        PnlColumnas.ResumeLayout()
        PnlColumnasHost.ResumeLayout()
        PnlColumnas.Anchor = Top
    End Sub

    Private Sub ActualizarEditoresDesdeFilaActual()
        If Not PnlColumnasHost.Visible Then Return
        If DG_Excel.DataSource Is Nothing Then
            For Each tb In _editors.Values : tb.Text = "" : Next
            Return
        End If

        Dim row As DataGridViewRow =
            If(DG_Excel.SelectedRows.Count > 0, DG_Excel.SelectedRows(0), DG_Excel.CurrentRow)
        If row Is Nothing Then
            For Each tb In _editors.Values : tb.Text = "" : Next
            Return
        End If

        For Each kvp In _editors
            Dim idx As Integer = kvp.Key
            Dim tb As TextBox = kvp.Value
            Dim v = If(idx < row.Cells.Count, row.Cells(idx).Value, Nothing)
            tb.Text = If(v Is Nothing, "", v.ToString())
        Next
    End Sub

    ' ----------------- CSV simple -----------------
    Private Function LeerCsv(ruta As String, usarEncabezados As Boolean) As DataTable
        Dim dt As New DataTable()
        Using sr As New StreamReader(ruta, detectEncodingFromByteOrderMarks:=True)
            Dim primera As Boolean = True
            While Not sr.EndOfStream
                Dim linea As String = sr.ReadLine()
                Dim separador As Char = If(linea.Contains(";"c) AndAlso Not linea.Contains(","c), ";"c, ","c)
                Dim campos As String() = linea.Split(separador)

                If primera Then
                    If usarEncabezados Then
                        For Each c In campos : dt.Columns.Add(c.Trim()) : Next
                    Else
                        For i = 0 To campos.Length - 1 : dt.Columns.Add("Col" & (i + 1).ToString()) : Next
                        dt.Rows.Add(campos.Select(Function(x) CType(x, Object)).ToArray())
                    End If
                    primera = False
                Else
                    If campos.Length <> dt.Columns.Count Then Array.Resize(campos, dt.Columns.Count)
                    dt.Rows.Add(campos.Select(Function(x) CType(x, Object)).ToArray())
                End If
            End While
        End Using
        Return dt
    End Function

    ' ----------------- Ajustes del grid -----------------
    Private Sub PrepararGrid(dgv As DataGridView)
        dgv.ReadOnly = True
        dgv.AllowUserToAddRows = False
        dgv.AllowUserToDeleteRows = False
        dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        dgv.MultiSelect = False
        dgv.RowHeadersVisible = False
        dgv.AutoGenerateColumns = True
    End Sub

    Private Sub AutoAjustarColumnas(dgv As DataGridView)
        dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells
        dgv.AutoResizeColumns()
        dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells
    End Sub

    ' ======== PDF: Elegir carpeta ========
    Private Sub BtnElegirCarpeta_Click(sender As Object, e As EventArgs) Handles BtnElegirCarpeta.Click
        Using fbd As New FolderBrowserDialog()
            fbd.Description = "Elige la carpeta donde están los PDFs"
            If fbd.ShowDialog() = DialogResult.OK Then
                _carpetaPdf = fbd.SelectedPath
                MostrarPdfDeFilaSeleccionada()
            End If
        End Using
    End Sub

    ' ======== PDF: reaccionar al cambio de fila del grid ========
    Private Sub DG_Excel_SelectionChanged(sender As Object, e As EventArgs) Handles DG_Excel.SelectionChanged
        MostrarPdfDeFilaSeleccionada()
        ActualizarEditoresDesdeFilaActual()
        SincronizarChkOkConFila()
    End Sub

    Private Sub MostrarPdfDeFilaSeleccionada()
        If String.IsNullOrEmpty(_carpetaPdf) Then Return
        If DG_Excel.DataSource Is Nothing Then Return
        If DG_Excel.SelectedRows.Count = 0 AndAlso DG_Excel.CurrentRow Is Nothing Then Return

        Dim row As DataGridViewRow =
            If(DG_Excel.SelectedRows.Count > 0, DG_Excel.SelectedRows(0), DG_Excel.CurrentRow)
        If row Is Nothing Then Return
        If COL_FOLIO >= row.Cells.Count Then Return

        Dim folioObj = row.Cells(COL_FOLIO).Value
        If folioObj Is Nothing Then Return

        Dim folio As String = folioObj.ToString().Trim()
        If folio = "" Then Return

        Dim pdfPath As String = BuscarPdfPorFolio(_carpetaPdf, folio)
        If Not String.IsNullOrEmpty(pdfPath) AndAlso File.Exists(pdfPath) Then
            MostrarPdf(pdfPath)
        Else
            LimpiarVisorPdf()
        End If
    End Sub

    Private Function BuscarPdfPorFolio(carpeta As String, folio As String) As String
        Dim exacto = Path.Combine(carpeta, $"{folio}.pdf")
        If File.Exists(exacto) Then Return exacto

        Dim match = Directory.EnumerateFiles(carpeta, $"*{folio}*.pdf", SearchOption.TopDirectoryOnly) _
                                .OrderBy(Function(p) p.Length) _
                                .FirstOrDefault()
        Return If(match, String.Empty)
    End Function

    Private Sub MostrarPdf(ruta As String)
        Try
            If PdfView Is Nothing OrElse PdfView.CoreWebView2 Is Nothing Then
                PdfView?.EnsureCoreWebView2Async().Wait(1000)
            End If
            Dim u As New Uri(ruta) ' local: file://
            PdfView.Source = u
        Catch
            ' Opcional: abrir con visor externo
            ' Process.Start(New ProcessStartInfo(ruta) With {.UseShellExecute = True})
        End Try
    End Sub

    Private Sub LimpiarVisorPdf()
        Try
            If PdfView IsNot Nothing AndAlso PdfView.CoreWebView2 IsNot Nothing Then
                PdfView.CoreWebView2.NavigateToString("<html><body style='font-family:sans-serif;color:#666'>Sin PDF</body></html>")
            End If
        Catch
        End Try
    End Sub

    ' ======== Comentarios = OK ========
    Private Function BuscarColumnaComentarios() As Integer
        If DG_Excel Is Nothing OrElse DG_Excel.Columns Is Nothing Then Return -1
        For i = 0 To DG_Excel.Columns.Count - 1
            Dim h = DG_Excel.Columns(i).HeaderText
            If Not String.IsNullOrEmpty(h) AndAlso
               String.Equals(h.Trim(), "comentarios", StringComparison.OrdinalIgnoreCase) Then
                Return i
            End If
        Next
        If DG_Excel.Columns.Count > COL_Y_INDEX Then Return COL_Y_INDEX
        Return -1
    End Function

    Private Sub SincronizarChkOkConFila()
        Dim colComentarios = BuscarColumnaComentarios()
        If colComentarios = -1 Then
            ChkComentariosOK.Enabled = False
            ChkComentariosOK.Checked = False
            Exit Sub
        End If

        ChkComentariosOK.Enabled = True

        Dim row As DataGridViewRow =
            If(DG_Excel.SelectedRows.Count > 0, DG_Excel.SelectedRows(0), DG_Excel.CurrentRow)
        If row Is Nothing Then
            ChkComentariosOK.Checked = False
            Exit Sub
        End If

        Dim v = row.Cells(colComentarios).Value
        ChkComentariosOK.Checked = (v IsNot Nothing AndAlso
                                    v.ToString().Trim().Equals("OK", StringComparison.OrdinalIgnoreCase))
    End Sub

    Private Sub ChkComentariosOK_CheckedChanged(sender As Object, e As EventArgs) Handles ChkComentariosOK.CheckedChanged
        Dim colComentarios = BuscarColumnaComentarios()
        If colComentarios = -1 Then Exit Sub
        If DG_Excel.CurrentRow Is Nothing Then Exit Sub

        If ChkComentariosOK.Checked Then
            DG_Excel.CurrentRow.Cells(colComentarios).Value = "OK"
        Else
            DG_Excel.CurrentRow.Cells(colComentarios).Value = ""
        End If
    End Sub

End Class
