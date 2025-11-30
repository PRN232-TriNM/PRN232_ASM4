using EVCS.TriNM.Repositories;
using EVCS.TriNM.Repositories.Models;
using EVCS.TriNM.Services.Extensions;
using EVCS.TriNM.Services.Interfaces;
using EVCS.TriNM.Services.Object.Requests;

namespace EVCS.TriNM.Services.Implements
{
    public class LoginService : ILoginService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly PasswordEncryptionService _passwordEncryptionService;
        private readonly IAuthService _authService;

        public LoginService(IUnitOfWork unitOfWork, PasswordEncryptionService passwordEncryptionService, IAuthService authService)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _passwordEncryptionService = passwordEncryptionService ?? throw new ArgumentNullException(nameof(passwordEncryptionService));
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        }

        public async Task<(bool isSuccess, string resultOrError)> ValidateUserAsync(LoginRequest request)
        {
            try
            {
                // Find user by email
                var user = await _unitOfWork.UserAccountRepository.GetByEmailAsync(request.Email);
                if (user == null)
                {
                    return (false, "Invalid email or password");
                }

                // Check if user is active
                if (!user.IsActive)
                {
                    return (false, "Account is deactivated");
                }

                // Verify password
                if (!_passwordEncryptionService.VerifyPassword(request.Password, user.PasswordHash))
                {
                    return (false, "Invalid email or password");
                }

                // Generate JWT token
                var token = _authService.GenerateJwtToken(user);
                return (true, token);
            }
            catch (Exception ex)
            {
                return (false, $"Login failed: {ex.Message}");
            }
        }
    }
}
