namespace ShippitApp
{
    public class LineHaul
    {
        public int Id { get; set; }
        public String? From { get; set; }
        public String? To { get; set; }
        public int Travel_Time_Seconds { get; set; } = -1;
    }

    public class PathJson
    {
        public String[]? Path { get; set; }
        public int Travel_Time_Total_Seconds { get; set; }
    }
}
