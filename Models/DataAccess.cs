using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Ship_Review_API.Models
{
    public class ShipInfo
    {
        [Key]
        public int ShipInfoId { get; set; }

        public string Name { get; set; }
        [Required]
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
        
        public ICollection<ShipEvaluation> ShipEvaluations { get; set; }
    }

    

    public class ShipEvaluation
    {
        [Key]
        public int ShipEvaluationId { get; set; }

        public byte VesselQualityMin { get; set; }
        public byte VesselQualityMax { get; set; }
        public byte VesselQualityValue { get; set; }

        public byte CrewPerformanceMin { get; set; }
        public byte CrewPerformanceMax { get; set; }
        public byte CrewPerformanceValue { get; set; }

        public byte CrewAttitudeMin { get; set; }
        public byte CrewAttitudeMax { get; set; }
        public byte CrewAttitudeValue { get; set; }

        public byte FuelEfficiencyMin { get; set; }
        public byte FuelEfficiencyMax { get; set; }
        public byte FuelEfficiencyValue { get; set; }

        public byte SafetyScoreMin { get; set; }
        public byte SafetyScoreMax { get; set; }
        public byte SafetyScoreValue { get; set; }

        [Required]
        public int Imo { get; set; }
        [ForeignKey("Imo")]
        public virtual ShipInfo ShipInfos { get; set; } = null!;
    }
}
