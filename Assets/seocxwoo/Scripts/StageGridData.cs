[System.Serializable]
public class StageGridData
{
    public int width;
    public int height;
    public float cellSize;
    public int[] grid;
    public int[,] To2DArray()
    {
        int[,] result = new int[width, height];
        int index = 0;
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                result[j, i] = grid[index++];
            }
        }
        return result;
    }
}