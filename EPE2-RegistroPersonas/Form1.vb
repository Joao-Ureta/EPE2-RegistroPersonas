Imports MySql.Data.MySqlClient

Public Class Form1
    Dim connectionString As String = "server=localhost;userid=jUreta;password=Maju2223;database=registropersonas"
    Private connection As MySqlConnection
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        Try
            'instancia de conexion
            connection = New MySqlConnection(connectionString)

            connection.Open()

            'funcion para cargar comunas
            CargarComunas()

        Catch ex As MySqlException
            'en caso de falla de conexion muestra mensaje de error
            MessageBox.Show("Error al conectar a la base de datos: " & ex.Message)
        Finally
            If connection IsNot Nothing Then connection.Close()
        End Try
    End Sub

    Private Sub CargarComunas()
        Try
            'consulta para obtener las comunas de la base de datos
            Dim query As String = "SELECT NombreComuna FROM comuna"
            Dim command As New MySqlCommand(query, connection)

            'ejecuta y lee la consulta
            Dim reader As MySqlDataReader = command.ExecuteReader()

            'limpia el combobox
            cbxComuna.Items.Clear()

            'agregar "seleccione una comuna" como opcion
            cbxComuna.Items.Add("Seleccione una comuna")

            'agrega las comunas al combobox
            While reader.Read()
                cbxComuna.Items.Add(reader("NombreComuna").ToString())
            End While

            reader.Close()

            'para dejar como primer campo "seleccione comuna"
            cbxComuna.SelectedIndex = 0

        Catch ex As MySqlException
            'mensaje para mostrar error en caso de consulta fallida
            MessageBox.Show("Error en la consulta: " & ex.Message)
        End Try
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Try
            'instancia de conexion
            connection = New MySqlConnection(connectionString)
            connection.Open()

            'consulta para buscar los datos de la tabla personas
            Dim query As String = "SELECT Nombre, Apellido, Sexo, Comuna, Ciudad, Observacion FROM personas WHERE RUT = @RUT"
            Dim command As New MySqlCommand(query, connection)
            command.Parameters.AddWithValue("@RUT", txtRut.Text)

            'ejecuta la consulta
            Dim reader As MySqlDataReader = command.ExecuteReader()

            If reader.HasRows Then
                'si el rut existe, se cargan los datos en los campos
                While reader.Read()
                    txtNombres.Text = reader("Nombre").ToString()
                    txtApellidos.Text = reader("Apellido").ToString()
                    cbxComuna.Text = reader("Comuna").ToString()
                    txtCiudad.Text = reader("Ciudad").ToString()
                    txtObservacion.Text = reader("Observacion").ToString()

                    'metodo para asignar el dato al radioButton
                    Select Case reader("Sexo").ToString()
                        Case "Masculino"
                            rBtnMasculino.Checked = True
                        Case "Femenino"
                            rBtnFemenino.Checked = True
                        Case Else
                            rBtnNulo.Checked = True
                    End Select
                End While

                'mantiene los campos bloqueados si el rut existe
                DesbloquearCampos(False)
            Else
                'si el rut no existe muestra un msgBox de opciones
                Dim resultado As DialogResult
                resultado = MessageBox.Show("RUT no encontrado. ¿Desea ingresar un nuevo registro?", "Confirmar Nuevo Registro", MessageBoxButtons.YesNo, MessageBoxIcon.Question)

                If resultado = DialogResult.Yes Then
                    'si se da opcion a si, se limpia y desbloquean los campos para nuevo ingreso
                    LimpiarCampos()
                    DesbloquearCampos(True)
                End If
            End If


            reader.Close()
        Catch ex As MySqlException
            MessageBox.Show("Error al conectar a la base de datos: " & ex.Message)
        Finally
            'cierre de conexion
            If connection IsNot Nothing Then connection.Close()
        End Try
    End Sub

    'funcion para desbloquear campos si el rut no existe
    Private Sub DesbloquearCampos(estado As Boolean)
        txtNombres.Enabled = estado
        txtApellidos.Enabled = estado
        rBtnMasculino.Enabled = estado
        rBtnFemenino.Enabled = estado
        rBtnNulo.Enabled = estado
        cbxComuna.Enabled = estado
        txtCiudad.Enabled = estado
        txtObservacion.Enabled = estado
        btnGuardar.Enabled = estado
    End Sub

    'funcion para limpiar campos si el rut no existe
    Private Sub LimpiarCampos()
        txtNombres.Text = ""
        txtApellidos.Text = ""
        rBtnMasculino.Checked = False
        rBtnFemenino.Checked = False
        rBtnNulo.Checked = False
        cbxComuna.Text = ""
        txtCiudad.Text = ""
        txtObservacion.Text = ""
    End Sub

    Private Sub btnLimpiar_Click(sender As Object, e As EventArgs) Handles btnLimpiar.Click
        LimpiarCampos2()  'función para limpiar todos los campos e incluir rut
    End Sub

    'funcion de limpiar campos incluido rut
    Private Sub LimpiarCampos2()
        txtNombres.Text = ""
        txtApellidos.Text = ""
        rBtnMasculino.Checked = False
        rBtnFemenino.Checked = False
        rBtnNulo.Checked = False
        cbxComuna.Text = ""
        txtCiudad.Text = ""
        txtObservacion.Text = ""
        txtRut.Text = ""
    End Sub

    Private Sub btnGuardar_Click(sender As Object, e As EventArgs) Handles btnGuardar.Click
        Try
            'instancia de conexion
            connection = New MySqlConnection(connectionString)
            connection.Open()

            'consulta de ingreso de datos
            Dim query As String = "INSERT INTO personas (RUT, Nombre, Apellido, Sexo, Comuna, Ciudad, Observacion) VALUES (@RUT, @Nombre, @Apellido, @Sexo, @Comuna, @Ciudad, @Observacion)"
            Dim command As New MySqlCommand(query, connection)

            'asigna los datos a cada campo
            command.Parameters.AddWithValue("@RUT", txtRut.Text)
            command.Parameters.AddWithValue("@Nombre", txtNombres.Text)
            command.Parameters.AddWithValue("@Apellido", txtApellidos.Text)
            command.Parameters.AddWithValue("@Sexo", If(rBtnMasculino.Checked, "Masculino", If(rBtnFemenino.Checked, "Femenino", "Nulo")))
            command.Parameters.AddWithValue("@Comuna", cbxComuna.Text)
            command.Parameters.AddWithValue("@Ciudad", txtCiudad.Text)
            command.Parameters.AddWithValue("@Observacion", txtObservacion.Text)

            'ejecuta la consulta
            command.ExecuteNonQuery()

            MessageBox.Show("Registro guardado exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information)
            LimpiarCampos2() 'limpia los campos despues de un ingreso exitoso
            DesbloquearCampos(False) 'vuelve a bloquear los campos despues de ingreso exitoso


        Catch ex As MySqlException
            'manejo de error en caso de falla de ingreso
            MessageBox.Show("Error al guardar el registro: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If connection IsNot Nothing Then connection.Close()
        End Try
    End Sub
End Class
