namespace MAD.Data
{
    public class Iris
    {
        public double sepallen;
        public double sepalwid;
        public double petallen;
        public double petalwid;
        public string species;

        public Iris(double sepallen, double sepalwid, double petallen, double petalwid, string species)
        {
            this.sepallen = sepallen;
            this.sepalwid = sepalwid;
            this.petallen = petallen;
            this.petalwid = petalwid;
            this.species = species;
        }
    }
}