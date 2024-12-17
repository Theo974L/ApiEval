using API.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using API.ApiDbContexts;
using Microsoft.AspNetCore.Authorization;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class challengeHistoryController : ControllerBase
    {
        private readonly ApiDbContext _context;

        public challengeHistoryController(ApiDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize] // L'utilisateur doit avoir le rôle "Admin"

        public async Task<ActionResult<challenge>> getAllChallengeHistory()
        {
            var challenge = await _context.challenge_history
                .ToListAsync();
            if (challenge == null)
            {
                return NotFound();
            }
            return Ok(challenge);
        }

        [HttpGet("user/{userId}")]
        [Authorize] // L'utilisateur doit avoir un rôle approprié
        public async Task<ActionResult<IEnumerable<challenge>>> GetChallengesByUser(int userId)
        {
            var userChallenges = await _context.challenge_history
                .Where(ch => ch.idUser == userId)
                .Include(ch => ch.challenge) // Inclut les détails du challenge associé
                .ToListAsync();

            if (userChallenges == null || userChallenges.Count == 0)
            {
                return NotFound($"No challenges found for user with ID {userId}");
            }

            return Ok(userChallenges);
        }

        [HttpPost]
        [Authorize] // L'utilisateur doit avoir un rôle approprié
        public async Task<ActionResult> AddChallengeHistory([FromBody] DTOChallengeHistory newHistory)
        {
            if (newHistory == null || newHistory.idUser <= 0 || newHistory.idChallenge <= 0)
            {
                return BadRequest("Invalid data provided.");
            }

            var ChallengeHistory = new challengeHistory {
                
                id = newHistory.id,
                idUser = newHistory.idUser,
                idChallenge = newHistory.idChallenge,
                valide = newHistory.valide,
            
            };


            try
            {
                _context.challenge_history.Add(ChallengeHistory);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetChallengesByUser), new { userId = newHistory.idUser }, newHistory);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")] // Seul un administrateur peut supprimer
        public async Task<ActionResult> DeleteChallengeHistory(int id)
        {
            var history = await _context.challenge_history.FindAsync(id);
            if (history == null)
            {
                return NotFound($"No challenge history found with ID {id}");
            }

            _context.challenge_history.Remove(history);
            await _context.SaveChangesAsync();

            return NoContent(); // Réponse standard après une suppression
        }




    }
}
