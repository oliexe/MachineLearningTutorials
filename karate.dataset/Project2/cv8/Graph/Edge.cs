namespace cv8.Graph
{
    public class Edge<T>
    {
        public Vertex<T> VertexA { get; }
        public Vertex<T> VertexB { get; }
        public T Weight { get; }

        public Edge(Vertex<T> vertexA, Vertex<T> vertexB)
        {
            this.VertexA = vertexA;
            this.VertexB = vertexB;
        }
        public Edge( Vertex<T> vertexA, Vertex<T> vertexB, T weight )
        {
            this.VertexA = vertexA;
            this.VertexB = vertexB;
            this.Weight = weight;
        }
    }
}
