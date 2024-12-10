﻿using System;

namespace ImGuiColorTextEditNet;

// Represents a character coordinate from the user's point of view,
// i.e. consider a uniform grid (assuming fixed-width font) on the
// screen as it is rendered, and each cell has its own coordinate, starting from 0.
// Tabs are counted as between 1 and mTabSize empty spaces, depending on
// how many spaces are necessary to reach the next tab stop.
// For example, coordinate (1, 5) represents the character 'B' in a line "\tABC", when mTabSize = 4,
// because it is rendered as "    ABC" on the screen.
internal struct Coordinates : IEquatable<Coordinates>
{
    public int Line;
    public int Column;

    public Coordinates() { Line = 0; Column = 0; }
    public Coordinates(int line, int column)
    {
        //Util.Assert(line >= 0);
        //Util.Assert(column >= 0);
        Line = line;
        Column = column;
    }

    public static Coordinates Invalid => new() { Line = -1, Column = -1 };

    public static bool operator ==(Coordinates x, Coordinates y) => x.Line == y.Line && x.Column == y.Column;
    public static bool operator !=(Coordinates x, Coordinates y) => x.Line != y.Line || x.Column != y.Column;
    public static bool operator <(Coordinates x, Coordinates y) => x.Line != y.Line ? x.Line < y.Line : x.Column < y.Column;
    public static bool operator >(Coordinates x, Coordinates y) => x.Line != y.Line ? x.Line > y.Line : x.Column > y.Column;
    public static bool operator <=(Coordinates x, Coordinates y) => x.Line != y.Line ? x.Line < y.Line : x.Column <= y.Column;
    public static bool operator >=(Coordinates x, Coordinates y) => x.Line != y.Line ? x.Line > y.Line : x.Column >= y.Column;

    public bool Equals(Coordinates other) => Line == other.Line && Column == other.Column;
    public override bool Equals(object? obj) => obj is Coordinates other && Equals(other);

    public override int GetHashCode()
    {
        unchecked
        {
            return (Line * 397) ^ Column;
        }
    }

    public override string ToString() => $"{Line}:{Column}";
}