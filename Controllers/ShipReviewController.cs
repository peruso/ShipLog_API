using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ship_Review_API.Data;
using Ship_Review_API.Models;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Ship_Review_API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ShipReviewController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public ShipReviewController(ApplicationDbContext context)
        {
            _context = context;
        }


        // GET: api/<ShipReviewController>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ShipInfo>>> GetShipReviews()
        {
            try
            {
                var shipReviews = await _context.ShipInfos
                    .Include(si => si.ShipEvaluations)
                    .ToListAsync();

                foreach (var shipReview in shipReviews)
                {
                    var shipEvaluations = await _context.ShipEvaluations
                        .Where(se => se.Imo == shipReview.Imo)
                        .ToListAsync();

                    // shipEvaluations を shipReview.ShipEvaluations に追加
                    shipReview.ShipEvaluations = shipReview.ShipEvaluations
                .Concat(shipEvaluations.OrderByDescending(se => se.ShipEvaluationId))
                .ToList();
                }

                return Ok(shipReviews);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }


        // GET api/<ShipReviewController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ShipInfo>> GetShipInfoById(int id)
        {
            try
            {
                // 指定された id の ShipInfo をデータベースから取得
                var shipInfo = await _context.ShipInfos
                    .Include(si => si.ShipEvaluations)
                    .FirstOrDefaultAsync(si => si.ShipInfoId == id);
                

                if (shipInfo == null)
                {
                    // 指定された id の ShipInfo が見つからなかった場合、404 エラーを返す
                    return NotFound($"ShipInfo with ID {id} not found");
                }
                var shipEvaluations = await _context.ShipEvaluations
                        .Where(se => se.Imo == shipInfo.Imo)
                        .ToListAsync();
                shipInfo.ShipEvaluations = shipInfo.ShipEvaluations
                .Concat(shipEvaluations.OrderByDescending(se => se.ShipEvaluationId))
                .ToList();

                // ShipEvaluations を ShipEvaluationId で降順にソート
                //shipInfo.ShipEvaluations = shipInfo.ShipEvaluations
                //    .OrderByDescending(se => se.ShipEvaluationId)
                //    .ToList();

                // ShipInfo を返す
                return Ok(shipInfo);
            }
            catch (Exception ex)
            {
                // 例外が発生した場合、500 エラーを返す
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        // POST api/<ShipReviewController>
        [HttpPost]
        public void SaveShipReview([FromBody] ShipInfoAndEvaluation shipInfoAndEvaluation)
        {
            try
            {
                //restaurant_reviews reviews = GetRestaurantReviewsFromXml();

                ShipInfo newShip = GetNewShipWithShipInfo(shipInfoAndEvaluation);

                ShipEvaluation newEvaluation = GetNewShipEvaluation(shipInfoAndEvaluation);



                //var newShipInfo = newShip;

                _context.ShipInfos.Add(newShip);

                _context.SaveChanges();

                _context.ShipEvaluations.Add(newEvaluation);

                _context.SaveChanges();

                StatusCode(200, $"ShipInfo with ID {newShip.ShipInfoId} added successfully");

            }
            catch (Exception ex)
            {
                // 例外が発生した場合、500 エラーを返す
                StatusCode(500, $"Error: {ex.Message}");
            }



            // Save the updated 'reviews' back to the XML file (you may need a method for this)




        }

        private ShipInfo GetNewShipWithShipInfo(ShipInfoAndEvaluation shipInfoAndEvaluation)
        {
            ShipInfo newShip = new ShipInfo();
            newShip.Name = shipInfoAndEvaluation.Name;
            newShip.Imo = shipInfoAndEvaluation.Imo;
            newShip.Type = shipInfoAndEvaluation.Type;
            newShip.Flag = shipInfoAndEvaluation.Flag;
            newShip.GrossTon = shipInfoAndEvaluation.GrossTon;
            newShip.Dwt = shipInfoAndEvaluation.Dwt;
            newShip.Length = shipInfoAndEvaluation.Length;
            newShip.Beam = shipInfoAndEvaluation.Beam;
            newShip.Draught = shipInfoAndEvaluation.Draught;
            newShip.Photo = shipInfoAndEvaluation.Photo;
            newShip.BuildYear = shipInfoAndEvaluation.BuildYear;
            newShip.Owner = shipInfoAndEvaluation.Owner;
            newShip.Manager = shipInfoAndEvaluation.Manager;

            return newShip;
        }

        private ShipEvaluation GetNewShipEvaluation(ShipInfoAndEvaluation shipInfoAndEvaluation)
        {
            ShipEvaluation shipEvaluation = new ShipEvaluation();
            shipEvaluation.VesselQualityMax = 5;
            shipEvaluation.VesselQualityMin = 0;
            shipEvaluation.VesselQualityValue = shipInfoAndEvaluation.VesselQualityValue;
            shipEvaluation.CrewPerformanceMax = 5;
            shipEvaluation.CrewPerformanceMin = 0;
            shipEvaluation.CrewPerformanceValue = shipInfoAndEvaluation.CrewPerformanceValue;
            shipEvaluation.CrewAttitudeMax = 5;
            shipEvaluation.CrewAttitudeMin = 0;
            shipEvaluation.CrewAttitudeValue = shipInfoAndEvaluation.CrewAttitudeValue;
            shipEvaluation.FuelEfficiencyMax = 5;
            shipEvaluation.FuelEfficiencyMin = 0;
            shipEvaluation.FuelEfficiencyValue = shipInfoAndEvaluation.FuelEfficiencyValue;
            shipEvaluation.SafetyScoreMax = 5;
            shipEvaluation.SafetyScoreMin = 0;
            shipEvaluation.SafetyScoreValue = shipInfoAndEvaluation.SafetyScoreValue;
            shipEvaluation.Imo = shipInfoAndEvaluation.Imo;
            return shipEvaluation;
        }

        // PUT api/<ShipReviewController>/5
        [HttpPut("{id}")]
        [EnableCors]
        public IActionResult UpdateShipReview([FromBody] ShipInfoAndEvaluation updatedReview)
        {
            try {
                var existingShip = _context.ShipInfos.FirstOrDefault(s => s.Imo == updatedReview.Imo);
                if (existingShip != null)
                {
                    // Update existing ShipInfo
                    UpdateExistingShip(existingShip, updatedReview);

                    var existingEvaluation = _context.ShipEvaluations
                        .Where(se => se.Imo == updatedReview.Imo)
                        .OrderByDescending(se => se.ShipEvaluationId).FirstOrDefault();
                    if (existingEvaluation != null)
                    { 
                        UpdateExistingEvaluation(existingEvaluation, updatedReview);
                        

                                       }

                    _context.SaveChanges();

                }

            }
            catch (Exception ex)
            {
                // Handle exceptions and return an appropriate error response
                return StatusCode(500, new { error = "Internal Server Error", message = ex.Message });
            }

            return Ok();

        }

        private void UpdateExistingShip(ShipInfo existingShip, ShipInfoAndEvaluation updatedReview)
        {
            existingShip.Name = updatedReview.Name;
            existingShip.Imo = updatedReview.Imo;
            existingShip.Type = updatedReview.Type;
            existingShip.Flag = updatedReview.Flag;
            existingShip.GrossTon = updatedReview.GrossTon;
            existingShip.Dwt = updatedReview.Dwt;
            existingShip.Length = updatedReview.Length;
            existingShip.Beam = updatedReview.Beam;
            existingShip.Draught = updatedReview.Draught;
            existingShip.Photo = updatedReview.Photo;
            existingShip.BuildYear = updatedReview.BuildYear;
            existingShip.Owner = updatedReview.Owner;
            existingShip.Manager = updatedReview.Manager;
        }

        private void UpdateExistingEvaluation(ShipEvaluation existingEvaluation, ShipInfoAndEvaluation updatedReview)
        {
            existingEvaluation.VesselQualityValue = updatedReview.VesselQualityValue;
            existingEvaluation.CrewPerformanceValue = updatedReview.CrewPerformanceValue;
            existingEvaluation.CrewAttitudeValue = updatedReview.CrewAttitudeValue;
            existingEvaluation.FuelEfficiencyValue = updatedReview.FuelEfficiencyValue;
            existingEvaluation.SafetyScoreValue = updatedReview.SafetyScoreValue;
        }
      

        // DELETE api/<ShipReviewController>/5
        [HttpDelete("{id}")]
        public void DeleteShipReview(int id)
        {
            try
            {
                var shipInfo = _context.ShipInfos.FirstOrDefault(si => si.ShipInfoId == id);
                if (shipInfo != null)
                {

                    _context.ShipInfos.Remove(shipInfo);
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions and return an appropriate error response
                StatusCode(500, new { error = "Internal Server Error", message = ex.Message });
            }

            StatusCode(200, $"ShipInfo with ID {id} deleted successfully");
        }
    }
}
