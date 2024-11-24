/*
This file is part of LabelMakerDrawBluetooth.

LabelMakerDrawBluetooth is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation version 3 or later.

LabelMakerDrawBluetooth is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with LabelMakerDrawBluetooth. If not, see <https://www.gnu.org/licenses/>.
*/

namespace LabelMakerDrawApp;

public partial class MainPage : ContentPage
{
    private BleHandler Ble { get; set; }
    private List<List<PointF>> _Points;

    public MainPage()
    {
        InitializeComponent();

        Ble = new BleHandler(this);

        _Points = new List<List<PointF>>();
    }

    protected async override void OnAppearing()
    {
        base.OnAppearing();

        BleConnect.Init(this, Ble);

        Ble.OnConnected += Ble_OnConnected;
        Ble.OnDisconnected += Ble_OnDisconnected;
        Ble.StatusData.OnDataChanged += StatusData_OnStateChanged;

        if (!await CheckBluetoothStatus())
            await RequestBluetoothAccess();
    }

    private async Task<bool> CheckBluetoothStatus()
    {
        try
        {
            var requestStatus = await new BluetoothPermissions().CheckStatusAsync();
            return requestStatus == PermissionStatus.Granted;
        }
        catch (Exception ex)
        {
            return false;
        }
    }

    public async Task<bool> RequestBluetoothAccess()
    {
        try
        {
            var requestStatus = await new BluetoothPermissions().RequestAsync();
            return requestStatus == PermissionStatus.Granted;
        }
        catch (Exception ex)
        {
            return false;
        }
    }

    private void DrawInput_DrawingLineCompleted(object sender, CommunityToolkit.Maui.Core.DrawingLineCompletedEventArgs e)
    {
        List<PointF> points = new List<PointF>();

        foreach (var entry in e.LastDrawingLine.Points)
            points.Add(entry);

        _Points.Add(points);

        if (Ble.IsConnected)
            PrintButton.IsEnabled = true;
    }

    private async void ClearButton_Clicked(object sender, EventArgs e)
    {
        _Points.Clear();
        DrawInput.Clear();
        PrintButton.IsEnabled = false;
    }
    
    private async void PrintButton_Clicked(object sender, EventArgs e)
    {
        List<IntPoint> drawPoints = new List<IntPoint>();
        PointF prevPoint = new PointF();
        byte minX = 255;

        PrintButton.IsEnabled = false;

        foreach (var segment in _Points)
        {
            bool first = true;

            foreach (var point in segment)
            {
                PointF adjustedPoint = new Point(point.X, (DrawInput.Height - point.Y));

                double distanceFromLast = Math.Sqrt(Math.Pow((adjustedPoint.X - prevPoint.X), 2) + Math.Pow((adjustedPoint.Y - prevPoint.Y), 2));

                if (distanceFromLast > 1)
                {
                    var nextPoint = new IntPoint(adjustedPoint, !first);
                    drawPoints.Add(nextPoint);
                    if (nextPoint.X < minX)
                        minX = nextPoint.X;
                    first = false;
                    prevPoint = adjustedPoint;
                }
            }
        }
        // Normalize to minimum X
        List<IntPoint> adjustedDrawPoints = new List<IntPoint>();
        foreach (var point in drawPoints)
        {
            adjustedDrawPoints.Add(new IntPoint(point, minX));
        }

        Ble.DrawCommandData.DrawPoints = adjustedDrawPoints;

        await Ble.SendDrawCommand();
    }

    private void Ble_OnConnected()
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            PrintButton.IsEnabled = _Points.Count > 0;
        });
    }

    private void Ble_OnDisconnected()
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            PrintButton.IsEnabled = false;
        });
    }

    private void StatusData_OnStateChanged()
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            PrintButton.IsEnabled = (_Points.Count > 0) && (Ble.StatusData.State == LabelMakerStatusData.States.WaitingForInput);
        });
    }
}
