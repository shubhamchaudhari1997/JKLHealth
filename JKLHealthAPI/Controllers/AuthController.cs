using JKLHealthAPI.Data;
using JKLHealthAPI.Models;
using JKLHealthAPI.Models.Authentication;
using JKLHealthAPI.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JKLHealthAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        private readonly ApplicationDbContext _context;

        public AuthController(IAuthService authService, ApplicationDbContext context)
        {
            _authService = authService;
            _context = context;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            try
            {
                if (model.UserType == UserType.Caregiver && string.IsNullOrEmpty(model.Specialization))
                {
                    return BadRequest(new AuthResponse
                    {
                        IsSuccess = false,
                        Message = "Specialization is required for caregivers"
                    });
                }

                var result = await _authService.RegisterAsync(model);

                if (!result.IsSuccess)
                    return BadRequest(result);

                var userId = result.UserId;

                if (model.UserType == UserType.Caregiver)
                {
                    if (string.IsNullOrEmpty(model.Specialization))
                    {
                        return BadRequest(new AuthResponse
                        {
                            IsSuccess = false,
                            Message = "Specialization is required for caregivers"
                        });
                    }

                    // Create the Caregiver record and link it with the User
                    var caregiver = new Caregiver
                    {
                        Name = $"{model.FirstName} {model.LastName}",
                        Specialization = model.Specialization,
                        IsAvailable = true,
                        IsActive = true,
                        UserId = userId // Link to the AspNetUsers entry
                    };

                    await _context.Caregiver.AddAsync(caregiver);
                    await _context.SaveChangesAsync();
                }

                if (model.UserType == UserType.Patient)
                {
                    // Create the patient record and link it with the User
                    var patient = new Patient
                    {
                        Name = $"{model.FirstName} {model.LastName}",
                        Address = model.Address,
                        MedicalRecord = "",
                        DateOfBirth = DateTime.Now,
                        CaregiverId = 1
                    };

                    await _context.Patient.AddAsync(patient);
                    await _context.SaveChangesAsync();
                }

                return Ok(result);
            }
            catch (DbUpdateException dbEx)
            {
                // Log the database-specific exception
                Console.WriteLine($"Database Error: {dbEx.Message}");
                Console.WriteLine($"Inner Exception: {dbEx.InnerException?.Message}");

                return StatusCode(500, new
                {
                    Error = "A database error occurred.",
                    Details = dbEx.InnerException?.Message ?? dbEx.Message
                });
            }
            catch (Exception ex)
            {
                // Log the generic exception
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");

                return StatusCode(500, new
                {
                    Error = "An unexpected error occurred.",
                    Details = ex.Message
                });
            }
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.LoginAsync(model);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }
    }
}
