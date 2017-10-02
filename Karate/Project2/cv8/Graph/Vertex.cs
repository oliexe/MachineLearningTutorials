namespace cv8.Graph
{
    public class Vertex<T>
    {
        public int ID { get; }
        public T Weight { get; }

        public Vertex( int ID )
        {
            this.ID = ID;
        }

        public Vertex(int ID, T weight )
        {
            this.ID = ID;
            this.Weight = weight;
        }
        
    }
}
