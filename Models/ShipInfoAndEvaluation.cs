using System.ComponentModel.DataAnnotations;

namespace Ship_Review_API.Models
{
    public class ShipInfoAndEvaluation
    {

        public string Name { get; set; }
        public int Imo { get; set; }
        public string Type { get; set; }
        public string Flag { get; set; }
        public uint GrossTon { get; set; }
        public uint Dwt { get; set; }
        public ushort Length { get; set; }
        public byte Beam { get; set; }
        public decimal Draught { get; set; }
        public string Photo { get; set; }
        public ushort BuildYear { get; set; }

        public string Owner { get; set; }


        public string Manager { get; set; }

        public byte VesselQualityValue { get; set; }

        public byte CrewPerformanceValue { get; set; }

        public byte CrewAttitudeValue { get; set; }

        public byte FuelEfficiencyValue { get; set; }

        public byte SafetyScoreValue { get; set; }
    }
}
