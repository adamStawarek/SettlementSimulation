namespace SettlementSimulation.AreaGenerator.Models
{
    public struct Pixel
    {
        public Pixel(byte r, byte g, byte b)
        {
            R = r;
            G = g;
            B = b;
        }

        public byte R { get; set; }
        public byte G { get; set; }
        public byte B { get; set; }
        public byte Intensity => (byte)((R + G + B) / 3);
    }
}