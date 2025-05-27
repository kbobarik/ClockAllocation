namespace clock_allocation.Models;

public class HeaderCellInfo
{
    public string Text { get; set; }
    public int Column { get; set; }
    public int Row { get; set; }
    public int ColumnSpan { get; set; } = 1;
    public int RowSpan { get; set; } = 1;
    public bool IsVertical { get; set; } = false;
}