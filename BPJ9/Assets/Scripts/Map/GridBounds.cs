using System.Collections;
using System.Collections.Generic;
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
        this.Center = ((Top - Bottom)/2, (Right - Left)/2);
    }

    public GridBounds((int row, int col) Center, int Width, int Height)
    {
        this.Left = Center.col - Width;
        this.Right = Center.col + Width;
        this.Top = Center.row + Height;
        this.Bottom = Center.row - Height;
        this.Center = Center;
    }

    public IEnumerable<(int, int)> Difference(GridBounds other)
    {
        // TODO: This can be improved to calculate the 4 rectangles
        //       that are outside of the union rectangle
        //       For now, we will go with the naive version
        foreach ( (int, int) pos in this)
        {
            if (other.Contains(pos)) continue;
            yield return pos;
        }

        foreach ( (int, int) pos in other)
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

}