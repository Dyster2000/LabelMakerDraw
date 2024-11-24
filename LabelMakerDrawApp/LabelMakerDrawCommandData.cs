/*
This file is part of LabelMakerDrawBluetooth.

LabelMakerDrawBluetooth is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation version 3 or later.

LabelMakerDrawBluetooth is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with LabelMakerDrawBluetooth. If not, see <https://www.gnu.org/licenses/>.
*/

namespace LabelMakerDrawApp;

public struct IntPoint
{
    public byte X;
    public byte Y;
    public bool Draw;

    public IntPoint()
    {
        X = 0;
        Y = 0;
        Draw = false;
    }

    public IntPoint(IntPoint src, byte xAdjust)
    {
        X = (byte)(src.X - xAdjust);
        Y = src.Y;
        Draw = src.Draw;
    }

    public IntPoint(PointF src, bool draw)
    {
        X = (byte)Math.Round(src.X);
        Y = (byte)Math.Round(src.Y);
        Draw = draw;
    }

    public static bool operator ==(IntPoint p1, IntPoint p2)
    {
        return p1.X == p2.X && p1.Y == p2.Y;
    }

    public static bool operator !=(IntPoint p1, IntPoint p2)
    {
        return p1.X != p2.X || p1.Y != p2.Y;
    }

    public override string ToString()
    {
        return $"({X}, {Y})";
    }
}

public class LabelMakerDrawCommandData
{
    static readonly int HeaderSize = 4;
    public static readonly int MaxPoints = 5;
    public static readonly int PointSize = 3;

    public List<IntPoint> DrawPoints { get; set; }

    MessageBuffer Buffer;

    public LabelMakerDrawCommandData()
    {
    }

    public byte[] Write(int startIndex)
    {
        int cnt = Math.Min(DrawPoints.Count - startIndex, MaxPoints);
        int offset = 0;

        Buffer = new MessageBuffer(HeaderSize + cnt * PointSize);

        offset = Buffer.Write(offset, (UInt16)startIndex);
        offset = Buffer.Write(offset, (UInt16)DrawPoints.Count);
        for (int i = 0; i < cnt; i++)
        {
            offset = Buffer.Write(offset, DrawPoints[startIndex + i].X);
            offset = Buffer.Write(offset, DrawPoints[startIndex + i].Y);
            offset = Buffer.Write(offset, DrawPoints[startIndex + i].Draw);
        }
        return Buffer.GetData();
    }
}
