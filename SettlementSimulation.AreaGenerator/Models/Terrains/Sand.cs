namespace SettlementSimulation.AreaGenerator.Models.Terrains
{
    public class Sand : Terrain
    {
        public override double Percentile => 0.2;
        public override byte UpperBound { get; set; } = 145;
        public override Pixel Color => new Pixel(215, 215, 115);
    }
}