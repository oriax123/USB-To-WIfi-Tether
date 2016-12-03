Imports System.IO
Imports System.Threading
Imports System.Net.NetworkInformation
Public Class Form1

    Dim drag As Boolean
    Dim mousex As Integer
    Dim mousey As Integer

    Dim command As New ProcessStartInfo("CMD.EXE")
    Dim exec As String
    Public Shared main_adapter As String

    Private ipv4Stats As IPv4InterfaceStatistics
    Private nic As NetworkInterface

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
        Form2.Show()
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        'Scans Network Adapters
        Dim nics() As NetworkInterface = NetworkInterface.GetAllNetworkInterfaces
        For Each adapter As NetworkInterface In nics
            adapters.Items.Add(adapter.Name)
        Next

        'Retrieves Saved Details
        TextBox1.Text = My.Settings.wifiname
        TextBox2.Text = My.Settings.password
        adapters.SelectedIndex = My.Settings.adapter_settings

    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click

        If Button2.Text = "Activate" Then
            command.WindowStyle = ProcessWindowStyle.Hidden
            command.CreateNoWindow = True
            command.UseShellExecute = False
            exec = "/C netsh wlan start hostednetwork"
            command.Arguments = exec
            Process.Start(command)
            Label7.Text = "Activated"
            Label7.ForeColor = Color.Green
            Button2.Text = "Deactivate"
            adapters.Enabled = False
            Button1.Enabled = False
            PictureBox1.Enabled = False
            ' select adapter to monitor
            If adapters.SelectedIndex > -1 Then
                nic = NetworkInterface.GetAllNetworkInterfaces(adapters.SelectedIndex)
                ipv4Stats = nic.GetIPv4Statistics
                Timer1.Start()
            End If
        Else

            command.WindowStyle = ProcessWindowStyle.Hidden
            command.CreateNoWindow = True
            command.UseShellExecute = False
            exec = "/C netsh wlan stop hostednetwork"
            command.Arguments = exec
            Process.Start(command)
            Label7.Text = "Deactivated"
            Label7.ForeColor = Color.Red
            Button2.Text = "Activate"
            Timer1.Stop()
            adapters.Enabled = True
            Button1.Enabled = True
            PictureBox1.Enabled = True
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
            My.Settings.adapter_settings = adapters.SelectedIndex

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
        End If
    End Sub

    Private Sub NotifyIcon1_DoubleClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles NotifyIcon1.DoubleClick
        Me.Show()
        Me.WindowState = FormWindowState.Normal
        NotifyIcon1.Visible = False
    End Sub

    Delegate Sub SetLabelText_Delegate(ByVal [Label] As Label, ByVal [text] As String)
    Private Sub SetLabelText_ThreadSafe(ByVal [Label] As Label, ByVal [text] As String)
        If [Label].InvokeRequired Then
            Dim MyDelegate As New SetLabelText_Delegate(AddressOf SetLabelText_ThreadSafe)
            Me.Invoke(MyDelegate, New Object() {[Label], [text]})
        Else
            [Label].Text = [text]
        End If
    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        UpdateStats()
    End Sub

    Private Sub UpdateStats()
        ipv4Stats = System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces(Me.adapters.SelectedIndex).GetIPv4Statistics
        Try
            SetLabelText_ThreadSafe(Me.recieved_data, ipv4Stats.BytesReceived.ToString)
            SetLabelText_ThreadSafe(Me.sent_data, ipv4Stats.BytesSent.ToString)
        Catch ex As Exception
            Console.WriteLine(ex)
        End Try
    End Sub
    Private Sub adapters_SelectedIndexChanged(sender As Object, e As EventArgs) Handles adapters.SelectedIndexChanged
        My.Settings.adapter_settings = adapters.SelectedIndex
        Dim wifi As String = adapters.SelectedText
        EnableDisableICS(My.Settings.main, wifi, True)
    End Sub

    Private Function EnableDisableICS(ByVal sPublicConnectionName As String, ByVal sPrivateConnectionName As String, ByVal bEnable As Boolean)
        Dim bFound As Boolean
        Dim oNetSharingManager, oConnectionCollection, oItem, EveryConnection, objNCProps

        oNetSharingManager = CreateObject("HNetCfg.HNetShare.1")

        oConnectionCollection = oNetSharingManager.EnumEveryConnection
        For Each oItem In oConnectionCollection
            EveryConnection = oNetSharingManager.INetSharingConfigurationForINetConnection(oItem)
            objNCProps = oNetSharingManager.NetConnectionProps(oItem)
            If objNCProps.name = sPrivateConnectionName Then
                bFound = True
                If bEnable Then
                    EveryConnection.EnableSharing(1)
                Else
                    EveryConnection.DisableSharing()
                End If
            End If
        Next

        oConnectionCollection = oNetSharingManager.EnumEveryConnection
        For Each oItem In oConnectionCollection
            EveryConnection = oNetSharingManager.INetSharingConfigurationForINetConnection(oItem)
            objNCProps = oNetSharingManager.NetConnectionProps(oItem)

            If objNCProps.name = sPublicConnectionName Then
                bFound = True
                If bEnable Then
                    EveryConnection.EnableSharing(0)
                Else
                    EveryConnection.DisableSharing()
                End If
            End If
        Next

    End Function

    Private Sub PictureBox3_Click(sender As Object, e As EventArgs) Handles PictureBox3.Click
        'Fixes the "Obtaining IP Adresses, basically just disables and renables.

        If Button2.Text = "Deactivate" Then
            command.WindowStyle = ProcessWindowStyle.Hidden
            command.CreateNoWindow = True
            command.UseShellExecute = False
            exec = "/C netsh wlan start hostednetwork"
            command.Arguments = exec
            Process.Start(command)
            Label7.Text = "Activated"
            Label7.ForeColor = Color.Green
            Button2.Text = "Deactivate"
            adapters.Enabled = False
            Button1.Enabled = False
            PictureBox1.Enabled = False
            ' select adapter to monitor
            If adapters.SelectedIndex > -1 Then
                nic = NetworkInterface.GetAllNetworkInterfaces(adapters.SelectedIndex)
                ipv4Stats = nic.GetIPv4Statistics
                Timer1.Start()
            End If

            Dim wifi As String = adapters.SelectedText
            EnableDisableICS(My.Settings.main, wifi, False)
            Dim i As Integer = 0
            Do Until i = 5
                i += 1
            Loop
            EnableDisableICS(My.Settings.main, wifi, True)
            command.WindowStyle = ProcessWindowStyle.Hidden
            command.CreateNoWindow = True
            command.UseShellExecute = False
            exec = "/C netsh wlan stop hostednetwork"
            command.Arguments = exec
            Process.Start(command)
            Label7.Text = "Deactivated"
            Label7.ForeColor = Color.Red
            Button2.Text = "Activate"
            Timer1.Stop()
            adapters.Enabled = True
            Button1.Enabled = True
            PictureBox1.Enabled = True
            MsgBox("Attempted fix, try to activate now!", MsgBoxStyle.Information)

        Else

            MsgBox("Please Activate Hotspot and try again to attempt to fix the problem.", MsgBoxStyle.Exclamation)
        End If


    End Sub
End Class
