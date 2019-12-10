using SettlementSimulation.AreaGenerator.Models;
using System.Drawing;

namespace SettlementSimulation.Viewer.Commands
{
    public class SetSettlementInfoCommand
    {
        public SettlementInfo SettlementInfo { get; set; }
        public Bitmap HeightMap { get; set; }
        public Bitmap ColorMap { get; set; }
    }
}