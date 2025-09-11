<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class Form2
    Inherits System.Windows.Forms.Form

    'Form reemplaza a Dispose para limpiar la lista de componentes.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Requerido por el Diseñador de Windows Forms
    Private components As System.ComponentModel.IContainer

    'NOTA: el Diseñador de Windows Forms necesita el siguiente procedimiento
    'Se puede modificar usando el Diseñador de Windows Forms.  
    'No lo modifique con el editor de código.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Form2))
        Me.DG_Excel = New System.Windows.Forms.DataGridView()
        Me.CbHojas = New System.Windows.Forms.ComboBox()
        Me.ChkEncabezados = New System.Windows.Forms.CheckBox()
        Me.LblArchivo = New System.Windows.Forms.Label()
        Me.BtnElegirCarpeta = New System.Windows.Forms.PictureBox()
        Me.Btn_Excel = New System.Windows.Forms.PictureBox()
        Me.PdfView = New Microsoft.Web.WebView2.WinForms.WebView2()
        Me.LblCarpetaPdf = New System.Windows.Forms.Label()
        CType(Me.DG_Excel, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.BtnElegirCarpeta, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Btn_Excel, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.PdfView, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'DG_Excel
        '
        Me.DG_Excel.AllowUserToAddRows = False
        Me.DG_Excel.AllowUserToResizeColumns = False
        Me.DG_Excel.AllowUserToResizeRows = False
        Me.DG_Excel.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DG_Excel.Location = New System.Drawing.Point(13, 133)
        Me.DG_Excel.Name = "DG_Excel"
        Me.DG_Excel.ReadOnly = True
        Me.DG_Excel.Size = New System.Drawing.Size(448, 549)
        Me.DG_Excel.TabIndex = 2
        '
        'CbHojas
        '
        Me.CbHojas.Anchor = System.Windows.Forms.AnchorStyles.Top
        Me.CbHojas.FormattingEnabled = True
        Me.CbHojas.Location = New System.Drawing.Point(552, 22)
        Me.CbHojas.Name = "CbHojas"
        Me.CbHojas.Size = New System.Drawing.Size(121, 21)
        Me.CbHojas.TabIndex = 3
        '
        'ChkEncabezados
        '
        Me.ChkEncabezados.Anchor = System.Windows.Forms.AnchorStyles.Top
        Me.ChkEncabezados.AutoSize = True
        Me.ChkEncabezados.Location = New System.Drawing.Point(695, 25)
        Me.ChkEncabezados.Name = "ChkEncabezados"
        Me.ChkEncabezados.Size = New System.Drawing.Size(91, 17)
        Me.ChkEncabezados.TabIndex = 4
        Me.ChkEncabezados.Text = "Encabezados"
        Me.ChkEncabezados.UseVisualStyleBackColor = True
        '
        'LblArchivo
        '
        Me.LblArchivo.Anchor = System.Windows.Forms.AnchorStyles.Top
        Me.LblArchivo.AutoSize = True
        Me.LblArchivo.Location = New System.Drawing.Point(118, 99)
        Me.LblArchivo.Name = "LblArchivo"
        Me.LblArchivo.Size = New System.Drawing.Size(110, 13)
        Me.LblArchivo.TabIndex = 5
        Me.LblArchivo.Text = "Selecciona el Archivo"
        '
        'BtnElegirCarpeta
        '
        Me.BtnElegirCarpeta.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BtnElegirCarpeta.Image = Global.prueba.My.Resources.Resources.documentation_folder_document_management_files_file_project_icon_142253
        Me.BtnElegirCarpeta.Location = New System.Drawing.Point(1214, 12)
        Me.BtnElegirCarpeta.Name = "BtnElegirCarpeta"
        Me.BtnElegirCarpeta.Size = New System.Drawing.Size(100, 100)
        Me.BtnElegirCarpeta.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.BtnElegirCarpeta.TabIndex = 6
        Me.BtnElegirCarpeta.TabStop = False
        '
        'Btn_Excel
        '
        Me.Btn_Excel.Image = Global.prueba.My.Resources.Resources.Excel2_35735
        Me.Btn_Excel.Location = New System.Drawing.Point(12, 12)
        Me.Btn_Excel.Name = "Btn_Excel"
        Me.Btn_Excel.Size = New System.Drawing.Size(100, 100)
        Me.Btn_Excel.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.Btn_Excel.TabIndex = 0
        Me.Btn_Excel.TabStop = False
        '
        'PdfView
        '
        Me.PdfView.AllowExternalDrop = True
        Me.PdfView.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.PdfView.CreationProperties = Nothing
        Me.PdfView.DefaultBackgroundColor = System.Drawing.Color.White
        Me.PdfView.Location = New System.Drawing.Point(868, 133)
        Me.PdfView.Name = "PdfView"
        Me.PdfView.Size = New System.Drawing.Size(446, 549)
        Me.PdfView.TabIndex = 7
        Me.PdfView.ZoomFactor = 1.0R
        '
        'LblCarpetaPdf
        '
        Me.LblCarpetaPdf.Anchor = System.Windows.Forms.AnchorStyles.Top
        Me.LblCarpetaPdf.AutoSize = True
        Me.LblCarpetaPdf.Location = New System.Drawing.Point(875, 99)
        Me.LblCarpetaPdf.Name = "LblCarpetaPdf"
        Me.LblCarpetaPdf.Size = New System.Drawing.Size(110, 13)
        Me.LblCarpetaPdf.TabIndex = 8
        Me.LblCarpetaPdf.Text = "Selecciona la carpeta"
        '
        'Form2
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1326, 736)
        Me.Controls.Add(Me.LblCarpetaPdf)
        Me.Controls.Add(Me.PdfView)
        Me.Controls.Add(Me.BtnElegirCarpeta)
        Me.Controls.Add(Me.LblArchivo)
        Me.Controls.Add(Me.ChkEncabezados)
        Me.Controls.Add(Me.CbHojas)
        Me.Controls.Add(Me.DG_Excel)
        Me.Controls.Add(Me.Btn_Excel)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "Form2"
        Me.Text = "LectorExcel 1.001"
        Me.WindowState = System.Windows.Forms.FormWindowState.Maximized
        CType(Me.DG_Excel, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.BtnElegirCarpeta, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Btn_Excel, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.PdfView, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents Btn_Excel As PictureBox
    Friend WithEvents DG_Excel As DataGridView
    Friend WithEvents CbHojas As ComboBox
    Friend WithEvents ChkEncabezados As CheckBox
    Friend WithEvents LblArchivo As Label
    Friend WithEvents BtnElegirCarpeta As PictureBox
    Friend WithEvents PdfView As Microsoft.Web.WebView2.WinForms.WebView2
    Friend WithEvents LblCarpetaPdf As Label
End Class
