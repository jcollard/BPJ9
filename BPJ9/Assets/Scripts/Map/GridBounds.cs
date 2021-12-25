using System.Collections;
using System.Collections.Generic;
using System;

[System.Serializable]
public class GridBounds : IEnumerable<(int, int)>
{
    public int Left, Top, Right, Bottom;
    public (int, int) Center;

    public GridBounds(int Top, int Right, int Bottom, int Left)
    {
        this.Left = Left;
        this.Top = Top;
        this.Right = Right;
        this.Bottom = Bottom;
        this.Center = ((Top - Bottom) / 2, (Right - Left) / 2);
    }

    public GridBounds((int row, int col) Center, int Width, int Height)
    {
        this.Left = Center.col - Width;
        this.Right = Center.col + Width;
        this.Top = Center.row + Height;
        this.Bottom = Center.row - Height;
        this.Center = Center;
    }

    public GridBounds(GridBounds center, int expandSize)
    {
        this.Center = center.Center;
        this.Left = center.Left - expandSize;
        this.Right = center.Right + expandSize;
        this.Bottom = center.Bottom - expandSize;
        this.Top = center.Top + expandSize;
    }

    public IEnumerable<(int, int)> DifferenceFromCenter(GridBounds other, (int row, int col) center)
    {
        int Left = Math.Min(this.Left, other.Left);
        int Right = Math.Max(this.Right, other.Right);
        int Top = Math.Max(this.Top, other.Top);
        int Bottom = Math.Min(this.Bottom, other.Bottom);
        GridBounds WrappedBounds = new GridBounds(Top, Right, Bottom, Left);
        (int row, int col) pos = center;

        int MaxDistance = ((Math.Max(Top - Bottom, Right - Left)/2)*2)+1;
        
        int Distance = 0;
        // DOWN_RIGHT, DOWN_LEFT, UP_LEFT, UP_RIGHT
        (int, int)[] diags = { (-1, 1), (-1, -1), (1, -1), (1, 1) };
        
        // We keep a position if it is in one or the other
        Predicate<(int, int)> keep = (p) =>
            (this.Contains(p) && !other.Contains(p)) || 
            (other.Contains(p) && !this.Contains(p));
        if (keep(pos)) yield return pos;
        while (Distance <= MaxDistance)
        {
            Distance++;
            // Move up 1 row
            pos.row++;
            // Follow each diagonal the distance from the center
            foreach ((int offR, int offC) diag in diags)
            {
                for (int moves = 0; moves < Distance; moves++)
                {
                    pos.row += diag.offR;
                    pos.col += diag.offC;
                    // TODO: Could be optimized for very bad rectangle pairs
                    if (keep(pos)) yield return pos;
                }
            }
        }
    }

    public IEnumerable<(int, int)> Difference(GridBounds other)
    {
        // TODO: This can be improved to calculate the 4 rectangles
        //       that are outside of the union rectangle
        //       For now, we will go with the naive version
        foreach ((int, int) pos in this)
        {
            if (other.Contains(pos)) continue;
            yield return pos;
        }

        foreach ((int, int) pos in other)
        {
            if (this.Contains(pos)) continue;
            yield return pos;
        }
    }


    public bool Contains((int row, int col) pos) =>
        pos.col >= Left && pos.col <= Right && pos.row >= Bottom && pos.row <= Top;

    public IEnumerator<(int, int)> GetEnumerator()
    {
        for (int row = Bottom; row <= Top; row++)
            for (int col = Left; col <= Right; col++)
                yield return (row, col);
    }

    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

    public override string ToString()
    {
        return $"GridBounds (T:{Top},L:{Left})->(B:{Bottom},R:{Right})";
    }

}