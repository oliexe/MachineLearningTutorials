namespace MAD.Data
{
    public class MeansPoint
    {
        public double Width { get; set; }
        public double Length { get; set; }
        public int Cluster { get; set; }

        public MeansPoint(double width, double lenght)
        {
            Width = width;
            Length = lenght;
            Cluster = 0;
        }

        public MeansPoint()
        {
        }

        public override string ToString()
        {
            return string.Format("{{{0},{1}}}", Width.ToString("f" + 1), Length.ToString("f" + 1));
        }
    }
}