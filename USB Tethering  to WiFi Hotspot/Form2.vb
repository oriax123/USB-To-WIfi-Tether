Imports System.Net.NetworkInformation

Public Class Form2

    Dim exec As String
    Dim command As New ProcessStartInfo("CMD.EXE")

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        My.Settings.main = set_adapter.SelectedItem
        My.Settings.adapter_settings = adapters.SelectedIndex
        My.Settings.hotspot = adapters.SelectedItem

        Form1.EnableDisableICS(My.Settings.main, My.Settings.hotspot, True)

        Me.Close()
    End Sub

    Private Sub Form2_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        
        'Creates Wifi Profile
        Command.WindowStyle = ProcessWindowStyle.Hidden
        Command.CreateNoWindow = True
        Command.UseShellExecute = False
        exec = "/C netsh wlan set hostednetwork mode=allow ssid=" + My.Settings.wifiname + " " + "key=" + My.Settings.password
        Command.Arguments = exec
        Process.Start(Command)

        'Scans Network Adapters
        Dim nics() As NetworkInterface = NetworkInterface.GetAllNetworkInterfaces
        For Each adapter As NetworkInterface In nics
            set_adapter.Items.Add(adapter.Name)
            adapters.Items.Add(adapter.Name)
        Next

    End Sub
End Class