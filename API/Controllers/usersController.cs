using API.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using API.ApiDbContexts;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using Serilog;


namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    

    public class usersController : ControllerBase
    {
        private readonly ApiDbContext _context;
        private readonly TokenService _tokenService;


        public usersController(ApiDbContext context, TokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService; // Initialisation du tokenService

        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult> Login([FromBody] LoginModel login)
        {           
            // Cherchez l'utilisateur dans la base de données
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.u_Mail == login.Username && u.u_Password == login.Password);

           
            // Vérifiez le statut de l'utilisateur (ici vous pouvez ajouter la logique pour le rôle)
            if (user.idStatus == 1)
            {
                var role = "Admin"; // Exemple de rôle
                var token = _tokenService.GenerateToken(login.Username, role);
                return Ok(new { token });
            }
            else if (user.idStatus == 0)
            {
                var role = "Guest"; // Exemple de rôle
                var token = _tokenService.GenerateToken(login.Username, role);
                return Ok(new { token });
            }

            // Si aucun statut valide, retournez une erreur générique
            return StatusCode(500, new
            {
                error = "Internal Server Error: Invalid username or password",
                userIdStatus = user?.idStatus,  // Ajout d'idStatus
                userMail = user?.u_Mail        // Ajout de u_Mail
            });
        }




        /**
         * 
         * Get all 
         * 
         */
        [AllowAnonymous] 
        [HttpGet]
        public async Task<ActionResult<users>> getAllUsers()
        {
            var users = await _context.Users
                .Include(c => c.status)
                .Include(c => c.challenge_Done)
                .ToListAsync();
            if (users == null)
            {
                return NotFound();
            }
            return Ok(users);
        }
        /**
         * 
         * Take only one User
         * 
         */
        [HttpGet("{id}")]
        [Authorize] // L'utilisateur doit être authentifié
        public async Task<ActionResult<users>> getAllOneUser(int id)
        {
            var users = await _context.Users
                .Include(c => c.status)
                .Include(c => c.challenge_Done)
                .FirstOrDefaultAsync(u => u.idUser == id); // Utiliser '==' pour la comparaison
            if (users == null)
            {
                return NotFound();
            }
            return Ok(users);
        }


        [HttpPost("create")]
        [Authorize(Roles = "Admin")] // L'utilisateur doit avoir le rôle "Admin"

        public async Task<ActionResult<users>> PostUser(UserCreateDTO newUserDto)
        {
            // Vérification de la validité du modèle
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Créer un nouvel utilisateur à partir du DTO, sans les propriétés de navigation
            var newUser = new users
            {
                u_Nom = newUserDto.u_Nom,
                u_Mail = newUserDto.u_Mail,
                u_Password = newUserDto.u_Password,
                idgroupe = newUserDto.idgroupe,
                u_NbPoint = newUserDto.u_NbPoint,
                u_idEtab = newUserDto.u_idEtab
            };

            // Ajout de l'utilisateur à la base de données
            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            // Renvoie un code 201 Created avec l'utilisateur créé
            return CreatedAtAction(nameof(getAllOneUser), new { id = newUser.idUser }, newUser);
        }

        // DELETE: api/Users/{id}
        [HttpDelete("delete/{id}")]
        [Authorize(Policy = "AdminOnly")] // L'utilisateur doit avoir le rôle "Admin"

        public async Task<IActionResult> DeleteUser(int id)
        {
            // Recherche de l'utilisateur par ID
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            // Suppression de l'utilisateur de la base de données
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            // Renvoie un code 204 No Content
            return NoContent();
        }

        /**
        [HttpPost("TokenJwt")]
        public static string GenerateJwtToken(string secretKey, users userData, int expirationMinutes = 240)
        {
            // Créer une date d'expiration (exp)
            var expirationTime = DateTime.UtcNow.AddMinutes(expirationMinutes);

            // Créer un ensemble de "claims" (données de l'utilisateur)
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userData.ToString()), // Données utilisateur
                new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString(), ClaimValueTypes.DateTime), // Date de création
                new Claim(JwtRegisteredClaimNames.Exp, expirationTime.ToString(), ClaimValueTypes.DateTime) // Date d'expiration
            };

            // Créer une clé secrète pour signer le JWT
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

            // Créer la signature du token avec un algorithme HMACSHA256
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Créer le token JWT
            var token = new JwtSecurityToken(
                issuer: userData.idStatus.ToString(), // Vous pouvez remplacer par un nom d'émetteur
                claims: claims,
                expires: expirationTime,
                signingCredentials: creds
            );

            // Retourner le token sous forme de chaîne
            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.WriteToken(token);
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] users model)
        {
            // Validation des informations d'identification (vérification dans la base de données)
            var user = _context.Users
                .FirstOrDefault(u => u.u_Mail == model.u_Mail && u.u_Password == model.u_Password);

            // Si l'utilisateur n'est pas trouvé ou les informations sont incorrectes
            if (user == null)
            {
                return Unauthorized("Invalid credentials");
            }

            // Récupérer le rôle de l'utilisateur depuis la base de données
            var role = user.idStatus; // Supposons que le rôle est stocké dans la colonne 'Role'

            // Créer un token pour l'utilisateur avec son rôle
            var token = _jwtTokenService.
        (model.idUser, role);

            // Retourner le token au client
            return Ok(new { Token = token });
        }
        **/
    }
}

public class LoginModel
{
    public string Username { get; set; }
    public string Password { get; set; }
}