Imports MySql.Data.MySqlClient

Public Class Form1

    Dim connectionString As String = "Server=localhost;Database=registropersonas;User ID='root';Password='';"
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

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles btnBuscar.Click
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

                'desbloquea los botones de eliminar y actualizar si el rut existe
                btnEliminar.Enabled = True
                btnActualizar.Enabled = True

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
        btnEliminar.Enabled = False
        btnActualizar.Enabled = False
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
            Dim updateQuery As String = "UPDATE personas SET Nombre = @Nombre, Apellido = @Apellido, Sexo = @Sexo, Comuna = @Comuna, Ciudad = @Ciudad, Observacion = @Observacion WHERE RUT = @RUT"
            Dim command As New MySqlCommand(updateQuery, connection)

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

    'variable global para controlar el estado del botón actualizar
    Private modoEdicion As Boolean = False

    Private Sub btnActualizar_Click(sender As Object, e As EventArgs) Handles btnActualizar.Click
        'si está en modo de edición, se guarda o cancela la actualización
        If modoEdicion Then
            'mostrar mensaje de confirmaciion de guardado
            Dim resultado As DialogResult
            resultado = MessageBox.Show("¿Seguro que Desea guardar los cambios?", "Confirmar actualización", MessageBoxButtons.YesNo, MessageBoxIcon.Question)

            If resultado = DialogResult.Yes Then
                'si la opcion es si, se guardan los cambios
                Try
                    'instancia de conexión
                    connection = New MySqlConnection(connectionString)
                    connection.Open()

                    'consulta para modificar datos
                    Dim query As String = "UPDATE personas SET Nombre = @Nombre, Apellido = @Apellido, Sexo = @Sexo, Comuna = @Comuna, Ciudad = @Ciudad, Observacion = @Observacion WHERE RUT = @RUT"
                    Dim command As New MySqlCommand(query, connection)

                    'se traspasan los datos actualizados
                    command.Parameters.AddWithValue("@RUT", txtRut.Text)
                    command.Parameters.AddWithValue("@Nombre", txtNombres.Text)
                    command.Parameters.AddWithValue("@Apellido", txtApellidos.Text)
                    command.Parameters.AddWithValue("@Sexo", If(rBtnMasculino.Checked, "Masculino", If(rBtnFemenino.Checked, "Femenino", "Nulo")))
                    command.Parameters.AddWithValue("@Comuna", cbxComuna.Text)
                    command.Parameters.AddWithValue("@Ciudad", txtCiudad.Text)
                    command.Parameters.AddWithValue("@Observacion", txtObservacion.Text)

                    'ejecuta la consulta
                    command.ExecuteNonQuery()

                    MessageBox.Show("Registro actualizado exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information)

                    'se vuelven a bloquear los campos
                    DesbloquearCampos(False)
                    modoEdicion = False 'se deshabilita el modo edicion

                Catch ex As MySqlException
                    'manejo de errores
                    MessageBox.Show("Error al actualizar el registro: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Finally
                    If connection IsNot Nothing Then connection.Close()
                End Try

            ElseIf resultado = DialogResult.No Then
                'si la opcion es no, no se guardan los cambios y se deja todo como estaba
                DesbloquearCampos(False)
                modoEdicion = False 'se deshabilita el modo edicion
            End If

        Else
            'si no está en modo de edición, desbloquea los campos para edición
            DesbloquearCampos(True)
            modoEdicion = True 'se habilita el modo de edicion
        End If
    End Sub

    Private Sub btnEliminar_Click(sender As Object, e As EventArgs) Handles btnEliminar.Click
        'mostrar mensaje si es que se desea o no eliminar los datos
        Dim resultado As DialogResult
        resultado = MessageBox.Show("¿Está seguro que desea eliminar este registro?", "Confirmar Eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)

        If resultado = DialogResult.Yes Then
            Try
                'instancia de conexión
                connection = New MySqlConnection(connectionString)
                connection.Open()

                'consulta para eliminar los datos del rut ingresado
                Dim query As String = "DELETE FROM personas WHERE RUT = @RUT"
                Dim command As New MySqlCommand(query, connection)

                ' se asigna el valor del rut
                command.Parameters.AddWithValue("@RUT", txtRut.Text)

                'ejecutar la consulta
                Dim filasAfectadas As Integer = command.ExecuteNonQuery()

                If filasAfectadas > 0 Then
                    'si la aliminacion es exitosa
                    MessageBox.Show("Registro eliminado exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information)

                    'limpiar los campos después de eliminar y quedan bloqueados los campos
                    LimpiarCampos2()
                    btnEliminar.Enabled = False
                    btnActualizar.Enabled = False
                Else
                    'si no se encuentra el registro para eliminar
                    MessageBox.Show("No se encontró el registro para eliminar.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End If

            Catch ex As MySqlException
                'manejo de errores de conexión o consulta
                MessageBox.Show("Error al eliminar el registro: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Finally
                If connection IsNot Nothing Then connection.Close()
            End Try
        End If
    End Sub

    Private Sub btnVisualizar_Click(sender As Object, e As EventArgs) Handles btnVisualizar.Click
        Try
            'instancia de conexion
            connection = New MySqlConnection(connectionString)
            connection.Open()

            'consulta para obtener los datos de la tabla persona (rut, nombre y apellido)
            Dim query As String = "SELECT RUT, Nombre, Apellido FROM personas"
            Dim command As New MySqlCommand(query, connection)

            'ejecutar la consulta
            Dim reader As MySqlDataReader = command.ExecuteReader()

            'variable para guardar la informacion del resultado y agregar separacion
            Dim registros As String = "Listado de Personas:" & Environment.NewLine & "-----------------------------" & Environment.NewLine

            ' concatenar registros
            While reader.Read()
                registros &= reader("RUT").ToString() & " " & reader("Nombre").ToString() & " " & reader("Apellido").ToString() & Environment.NewLine
            End While

            reader.Close()

            'mostrar los registros en un mensaje
            MessageBox.Show(registros, "Registros tabla personas", MessageBoxButtons.OK, MessageBoxIcon.Information)

        Catch ex As MySqlException
            'manejo de errores
            MessageBox.Show("Error al obtener los registros: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            'cerrar la conexion
            If connection IsNot Nothing Then connection.Close()
        End Try
    End Sub
End Class
