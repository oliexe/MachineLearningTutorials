namespace REH0063_MAD1
{
    /// <summary>
    /// Structure used for saving rows of the video game dataset
    /// </summary>
    internal class Videogame
    {
        public int _rank;
        public string _name;
        public string _platform;
        public int _year;
        public string _genre;
        public string _publisher;
        public double _naSales;
        public double _euSales;
        public double _jpSales;
        public double _otherSales;
        public double _globalSales;

        public Videogame(int rank, string name, string platform, int year, string genre, string publisher,
            double naSales, double euSales, double jpSales, double otherSales, double globalSales)
        {
            this._rank = rank;
            this._name = name;
            this._platform = platform;
            this._year = year;
            this._genre = genre;
            this._publisher = publisher;
            this._naSales = naSales;
            this._euSales = euSales;
            this._jpSales = jpSales;
            this._otherSales = otherSales;
            this._globalSales = globalSales;
        }
    }
}