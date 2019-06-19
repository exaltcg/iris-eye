namespace IrisEye.Core.Entities
{
    public class TestStep
    {
        public long Id { get; protected set; }
        public string Expected { get; set; }
        public string Actual { get; set; }
        public int SortIndex { get; set; }
    }
}