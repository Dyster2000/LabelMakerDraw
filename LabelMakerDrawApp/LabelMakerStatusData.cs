/*
This file is part of LabelMakerDrawBluetooth.

LabelMakerDrawBluetooth is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation version 3 or later.

LabelMakerDrawBluetooth is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with LabelMakerDrawBluetooth. If not, see <https://www.gnu.org/licenses/>.
*/

namespace LabelMakerDrawApp;

public delegate void Notify();

public class LabelMakerStatusData
{
    static readonly int DataSize = 1;

    public enum States
    {
        WaitingForConnect,
        WaitingForInput,
        Receiving,
        Printing
    };

    public States State { get; set; }

    public bool JustConnected { get; set; }

    public event Notify OnDataChanged;

    public LabelMakerStatusData()
    {
        JustConnected = false;
    }

    public void Read(byte[] data)
    {
        if (data.Length == DataSize)
        {
            MessageBuffer buffer = new MessageBuffer(data);
            int offset = 0;

            var state = (States)buffer.ReadByte(ref offset);

            var dataChanged = JustConnected;
            dataChanged = dataChanged || State != state;

            State = state;

            if (dataChanged)
                OnDataChanged?.Invoke();

            JustConnected = false;
        }
    }
}
