Imports System.IO
Imports System.Threading
Public Class Form1

    Dim drag As Boolean
    Dim mousex As Integer
    Dim mousey As Integer

    Dim command As New ProcessStartInfo("CMD.EXE")
    Dim exec As String

    'This enables form dragging on borderstyle none
    Private Sub Form1_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Me.MouseDown
        drag = True
        mousex = System.Windows.Forms.Cursor.Position.X - Me.Left
        mousey = System.Windows.Forms.Cursor.Position.Y - Me.Top
    End Sub

    Private Sub Form1_MouseMove(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Me.MouseMove
        If drag Then
            Me.Top = System.Windows.Forms.Cursor.Position.Y - mousey
            Me.Left = System.Windows.Forms.Cursor.Position.X - mousex
        End If
    End Sub

    Private Sub Form1_MouseUp(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Me.MouseUp
        drag = False
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click

        'Creates Wifi Profile
        command.WindowStyle = ProcessWindowStyle.Hidden
        command.CreateNoWindow = True
        command.UseShellExecute = False
        exec = "/C netsh wlan set hostednetwork mode=allow ssid=" + My.Settings.wifiname + " key=" + My.Settings.password
        command.Arguments = exec
        Process.Start(command)

        MsgBox("Hotspot Setup created! This was only needed once! You can now Activate Hotspot :)",
                   MsgBoxStyle.Information)

    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        'Retrieves Saved Details
        TextBox1.Text = My.Settings.wifiname
        TextBox2.Text = My.Settings.password
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click

        If Button2.Text = "Activate" Then
            command.WindowStyle = ProcessWindowStyle.Hidden
            command.CreateNoWindow = True
            command.UseShellExecute = False
            exec = "/C netsh wlan start hostednetwork"
            command.Arguments = exec
            Process.Start(command)
            LabelX2.Text = "Activated"
            LabelX2.ForeColor = Color.YellowGreen
            Button2.Text = "Deactivate"
        Else
            command.WindowStyle = ProcessWindowStyle.Hidden
            command.CreateNoWindow = True
            command.UseShellExecute = False
            exec = "/C netsh wlan stop hostednetwork"
            command.Arguments = exec
            Process.Start(command)
            LabelX2.Text = "Deactivated"
            LabelX2.ForeColor = Color.Crimson
            Button2.Text = "Activate"
        End If

    End Sub

    Private Sub PictureBox1_Click(sender As Object, e As EventArgs) Handles PictureBox1.Click

        'Saves Wifi Details

        If TextBox1.Text = String.Empty Then
            MsgBox("Please enter info to create hotspot.",
                   MsgBoxStyle.Information)
        Else
            My.Settings.wifiname = TextBox1.Text
            My.Settings.password = TextBox2.Text

            MsgBox("Settings have been saved!",
                   MsgBoxStyle.Information
                   )
        End If

    End Sub

    Private Sub PictureBox2_Click(sender As Object, e As EventArgs) Handles PictureBox2.Click
        Process.Start("https://youtu.be/-vRpGeMenu8")
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Me.Close()

    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        Me.WindowState = FormWindowState.Minimized

    End Sub

    Private Sub Form1_Resize(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Resize
        If Me.WindowState = FormWindowState.Minimized Then
            NotifyIcon1.Visible = True
            Me.Hide()
            NotifyIcon1.BalloonTipText = "Hi from right system tray"
            NotifyIcon1.ShowBalloonTip(500)
        End If
    End Sub

    Private Sub NotifyIcon1_DoubleClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles NotifyIcon1.DoubleClick
        Me.Show()
        Me.WindowState = FormWindowState.Normal
        NotifyIcon1.Visible = False
    End Sub
End Class
