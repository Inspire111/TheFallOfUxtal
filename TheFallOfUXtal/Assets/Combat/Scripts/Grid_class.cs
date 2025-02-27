using UnityEngine;

public class Grid
{
    public int Width;

    public int Height;
    private int[,] gridArray;
    public Grid(int width, int height)
    {
        Width = width;
        Height = height;
        gridArray = new int[width, height];

        for(int x = 0; x < Width;)
        {
            for (int y = 0; y < Height;)
            {

            }
        }
    }
}
