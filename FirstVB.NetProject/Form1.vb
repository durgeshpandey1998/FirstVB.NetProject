Imports System.Data.SqlClient
Imports System.Data.SQLite
Imports System.Diagnostics.Eventing.Reader
Imports System.IO

Public Class Form1
    'Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click

    '    Dim allData As String
    '    allData = TextBox1.Text + " " + TextBox2.Text + " " + TextBox3.Text + " " + TextBox4.Text + " " + TextBox5.Text
    '    If TextBox1.Text Is "" Or TextBox2.Text Is "" Or TextBox3.Text Is "" Or TextBox4.Text Is "" Or TextBox5.Text Is "" Then
    '        'MessageBox.Show("Please fill the details")
    '        MsgBox("Please fill in all details.", MsgBoxStyle.Exclamation, "Warning")
    '    Else
    '        Label7.Text = TextBox1.Text
    '        Label8.Text = TextBox2.Text
    '        Label9.Text = TextBox3.Text
    '        Label10.Text = TextBox4.Text
    '        Label11.Text = TextBox5.Text
    '        'MessageBox.Show("Register Success..")
    '        MsgBox("Complete.", MsgBoxStyle.Information, "Information")
    '    End If

    'End Sub
    'Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
    '    TextBox1.Text = ""
    '    TextBox2.Text = ""
    '    TextBox3.Text = ""
    '    TextBox3.Text = ""
    '    TextBox4.Text = ""
    '    TextBox5.Text = ""
    'End Sub

    Private connectionString As String = "Data Source=G://Durgesh-Learning//Asp.net//FirstVB.NetProject//YOUR_DATABASE_FILE.db"
    Private userDataList As New List(Of String())
    Private Sub DeleteUserByFirstName(firstNameToDelete As String)
        Dim connectionString As String = "Data Source=G:\Durgesh-Learning\Asp.net\FirstVB.NetProject\YOUR_DATABASE_FILE.db"

        Using connection As New SQLiteConnection(connectionString)
            connection.Open()

            Dim sql As String = "DELETE FROM UserRegister WHERE FirstName = @firstName"

            Using cmd As New SQLiteCommand(sql, connection)
                cmd.Parameters.AddWithValue("@firstName", firstNameToDelete)
                cmd.ExecuteNonQuery()
                MsgBox("Record Deleted Successfully.", MsgBoxStyle.MsgBoxRight, "Right")
            End Using
        End Using
    End Sub

    Private Function InsertDataByReadingFile()
        Dim filePath As String = "G:\Durgesh-Learning\Asp.net\FirstVB.NetProject\user_data.txt"
        Dim lines As String() = File.ReadAllLines(filePath)
        Dim connection As New SQLiteConnection("Data Source=" & "G://Durgesh-Learning//Asp.net//FirstVB.NetProject//YOUR_DATABASE_FILE.db")
        connection.Open()

        Dim sql As String = "INSERT INTO UserRegister (FirstName, LastName, Email, Phone, Address) " &
                            "VALUES (@firstName, @lastName, @email, @phone, @address)"
        userDataList.Clear()

        For Each line As String In lines

            Dim parts As String() = line.Split(vbTab)
            userDataList.Add(parts)

            If userDataList.Count >= 5 Then

                Using cmd As New SQLiteCommand(sql, connection)
                    cmd.Parameters.AddWithValue($"@firstName", userDataList(0)(0))
                    cmd.Parameters.AddWithValue($"@lastName", userDataList(1)(0))
                    cmd.Parameters.AddWithValue($"@email", userDataList(2)(0))
                    cmd.Parameters.AddWithValue($"@phone", userDataList(3)(0))
                    cmd.Parameters.AddWithValue($"@address", userDataList(4)(0))


                    cmd.ExecuteNonQuery()
                    userDataList.Clear()
                End Using
            Else
                'MessageBox.Show("Skipping line due to insufficient data: " & line)
            End If
        Next
    End Function

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If Trim(TextBox1.Text) = "" OrElse Trim(TextBox2.Text) = "" OrElse Trim(TextBox3.Text) = "" OrElse
               Trim(TextBox4.Text) = "" OrElse Trim(TextBox5.Text) = "" Then
            InsertDataByReadingFile()
            'DeleteUserByFirstName("System.String[]")
            MsgBox("Please fill in all details.", MsgBoxStyle.Exclamation, "Warning")
        Else
            Try
                Dim firstName As String = Trim(TextBox1.Text)
                Dim lastName As String = Trim(TextBox2.Text)
                Dim email As String = Trim(TextBox3.Text)
                Dim phone As String = Trim(TextBox4.Text)
                Dim address As String = Trim(TextBox5.Text)

                If Not IsValidName(firstName) Then
                    MsgBox("Please enter a valid first name (letters only).", MsgBoxStyle.Exclamation, "Warning")
                    Return
                End If

                If Not IsValidName(lastName) Then
                    MsgBox("Please enter a valid last name (letters only).", MsgBoxStyle.Exclamation, "Warning")
                    Return
                End If

                If Not IsValidEmail(email) Then
                    MsgBox("Please enter a valid email address.", MsgBoxStyle.Exclamation, "Warning")
                    Return
                End If

                If Not IsValidPhoneNumber(phone) Then
                    MsgBox("Please enter a valid phone number (up to 10 digits).", MsgBoxStyle.Exclamation, "Warning")
                    Return
                End If

                Dim sql = "INSERT INTO UserRegister (FirstName, LastName, Email, Phone, Address) VALUES (@firstName, @lastName, @email, @phone, @address)"
                Dim databasePath = GetDefaultProjectPath()
                CreateDatabase(databasePath)
                CreateTable(databasePath)

                Using connection As New SQLiteConnection(connectionString)
                    connection.Open()

                    Using command As New SQLiteCommand(sql, connection)
                        command.Parameters.AddWithValue("@firstName", firstName)
                        command.Parameters.AddWithValue("@lastName", lastName)
                        command.Parameters.AddWithValue("@email", email)
                        command.Parameters.AddWithValue("@phone", phone)
                        command.Parameters.AddWithValue("@address", address)

                        command.ExecuteNonQuery()
                    End Using
                End Using

                MsgBox("Registration successful!", MsgBoxStyle.Information, "Information")
                ClearTextBoxes()
                GetDataFromDatabase(GetDataGridView1())
            Catch ex As SQLiteException
                MsgBox("Error connecting to database: " & ex.Message, MsgBoxStyle.MsgBoxHelp, "Error")
            End Try
        End If
    End Sub
    Private Function IsValidName(name As String) As Boolean
        Return Not String.IsNullOrWhiteSpace(name) AndAlso System.Text.RegularExpressions.Regex.IsMatch(name, "^[a-zA-Z]+$")
    End Function


    Private Function IsValidEmail(email As String) As Boolean
        Return Not String.IsNullOrWhiteSpace(email) AndAlso System.Text.RegularExpressions.Regex.IsMatch(email, "^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$")
    End Function

    Private Function IsValidPhoneNumber(phone As String) As Boolean
        Return Not String.IsNullOrWhiteSpace(phone) AndAlso System.Text.RegularExpressions.Regex.IsMatch(phone, "^\d{1,10}$")
    End Function


    Private Sub CreateTable(databasePath As String)
        Dim sql = "CREATE TABLE IF NOT EXISTS UserRegister (FirstName TEXT NOT NULL, LastName TEXT NOT NULL, Email TEXT NOT NULL, Phone TEXT, Address TEXT)"

        Using connection As New SQLiteConnection("Data Source=" & "G://Durgesh-Learning//Asp.net//FirstVB.NetProject//YOUR_DATABASE_FILE.db")
            connection.Open()

            Using command As New SQLiteCommand(sql, connection)
                command.ExecuteNonQuery()
            End Using
        End Using
    End Sub
    Private Sub CreateDatabase(databasePath As String)
        If Not File.Exists(databasePath) Then
            databasePath = "G://Durgesh-Learning//Asp.net//FirstVB.NetProject//YOUR_DATABASE_FILE.db"
            File.Create(databasePath).Close()
        End If
    End Sub
    Private Function GetDefaultProjectPath() As String
        Dim projectPath = "G://Durgesh-Learning//Asp.net//FirstVB.NetProject//YOUR_DATABASE_FILE.db"
        Return projectPath
    End Function

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click

        ClearTextBoxes()
    End Sub

    Private Sub ClearTextBoxes()
        TextBox1.Text = ""
        TextBox2.Text = ""
        TextBox3.Text = ""
        TextBox4.Text = ""
        TextBox5.Text = ""
    End Sub


    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click

        GetDataFromDatabase(GetDataGridView1())
        'Dim dataTable As New DataTable
        'Dim sql As String = "SELECT * FROM UserRegister"

        'Using connection As New SQLiteConnection(connectionString)
        '    connection.Open()

        '    Using adapter As New SQLiteDataAdapter(sql, connection)
        '        adapter.Fill(dataTable)
        '    End Using
        'End Using
        'DataGridView1.DataSource = dataTable ' Bind the DataTable to the GridView

        'For Each row In dataTable.Rows
        '    Dim firstName As String = row("FirstName").ToString
        '    Dim lastName As String = row("LastName").ToString

        '    Label7.Text = firstName & " " & lastName
        'Next
    End Sub

    Private Function GetDataGridView1() As DataGridView
        Return DataGridView1
    End Function

    Private Function GetDataFromDatabase(dataGridView1 As DataGridView)
        Dim dataTable As New DataTable
        Dim sql As String = "SELECT * FROM UserRegister"

        Using connection As New SQLiteConnection(connectionString)
            connection.Open()

            Using adapter As New SQLiteDataAdapter(sql, connection)
                adapter.Fill(dataTable)
            End Using
        End Using
        dataGridView1.DataSource = dataTable
        If dataGridView1.SelectedRows.Count = 0 Then
            'MsgBox("Please select a row to write to file.", MsgBoxStyle.Exclamation, "Warning")
        End If

        Dim Testdata As String = ""
        Dim selectedRowIndex As Integer = dataGridView1.Rows.Count
        Dim selectedRow As DataRow = CType(dataTable.Rows(selectedRowIndex - 2), DataRow)

        For selectedRowIndexs As Integer = 0 To dataGridView1.Rows.Count - 2
            Dim selectedRows As DataRow = CType(dataTable.Rows(selectedRowIndexs), DataRow)
            Dim rowData As String = ""
            For colIndex As Integer = 0 To selectedRows.ItemArray.Length - 1
                rowData &= selectedRows.ItemArray(colIndex).ToString & vbCrLf
            Next
            Testdata &= rowData & vbCrLf
        Next

        'Dim dataToWrite As String = ""
        'For colIndex As Integer = 0 To selectedRow.ItemArray.Length - 1
        '    dataToWrite &= selectedRow.ItemArray(colIndex).ToString & vbCrLf
        'Next


        Dim filePath As String = Path.Combine("G://Durgesh-Learning//Asp.net//FirstVB.NetProject//", "user_data.txt")


        Try
            File.WriteAllText(filePath, Testdata)
            ' MsgBox("Selected data written to file successfully!", MsgBoxStyle.Information, "Information")
        Catch ex As IOException
            MsgBox("Error writing to file: " & ex.Message, MsgBoxStyle.AbortRetryIgnore, "Error")
        End Try
    End Function

    Private Sub OpenFileDialog1_FileOk(sender As Object, e As System.ComponentModel.CancelEventArgs)

    End Sub

    Private Sub TextBox1_TextChanged(sender As Object, e As EventArgs) Handles TextBox1.TextChanged
        Dim currentText As String = TextBox1.Text

        For Each c As Char In currentText
            If Not Char.IsLetter(c) AndAlso Not Char.IsControl(c) Then

                TextBox1.Text = TextBox1.Text.Remove(TextBox1.Text.IndexOf(c), 1)
                TextBox1.SelectionStart = TextBox1.Text.Length
                MsgBox("Only letters are allowed in First Name.", MsgBoxStyle.Exclamation, "Warning")
                Exit Sub
            End If
        Next
    End Sub

    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        DeleteUserByFirstName(TextBox1.Text)
    End Sub
End Class
