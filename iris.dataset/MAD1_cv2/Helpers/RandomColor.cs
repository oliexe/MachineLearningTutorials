using System;

namespace MAD.Helpers
{
    public class RandomColor
    {
        public byte[] Get()
        {
            System.Threading.Thread.Sleep(50);
            Random r = new Random(DateTime.UtcNow.Millisecond);

            return new byte[] { (byte)r.Next(0, 255), (byte)r.Next(0, 255), (byte)r.Next(0, 255) };
        }
    }
}