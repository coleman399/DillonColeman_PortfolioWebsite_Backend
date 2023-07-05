using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PortfolioWebsite_Backend.Services.AuthService
{
    public class AuthService : IAuthService
    {
        private readonly IMapper _mapper;
        private readonly UserContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(IMapper mapper, UserContext context, IConfiguration configuration)
        {
            _mapper = mapper;
            _context = context;
            _configuration = configuration;
        }

        public async Task<UserServiceResponse<List<GetUserDto>>> GetUsers()
        {
            var serviceResponse = new UserServiceResponse<List<GetUserDto>>();
            try
            {
                //  Return all contacts in response
                var dbUsers = await _context.Users.ToListAsync();
                serviceResponse.Data = dbUsers.Select(c => _mapper.Map<GetUserDto>(c)).ToList();
                return serviceResponse;
            }

            catch (Exception exception)
            {
                serviceResponse.Success = false;
                serviceResponse.Data = null;
                serviceResponse.Message = exception.Message + " " + exception;
                return serviceResponse;
            }
        }

        public async Task<UserServiceResponse<GetUserDto>> AddUser(RegisterUserDto newUser)
        {
            var serviceResponse = new UserServiceResponse<GetUserDto>();
            try
            {
                // Check if user name is valid
                if (!RegexFilters.IsValidUserName(newUser.UserName!)) throw new InvalidUserNameException(newUser.UserName!);

                // Check if password is valid
                if (!RegexFilters.IsValidPassword(newUser.Password!)) throw new InvalidPasswordException(newUser.Password!);

                // Email is always required, check if it's valid
                if (!RegexFilters.IsValidEmail(newUser.Email!)) throw new InvalidEmailException(newUser.Email!);

                // Check if role is valid
                bool validRole = false;
                foreach (var role in Enum.GetValues(typeof(Roles)))
                {
                    if (role.ToString() == newUser.Role)
                    {
                        validRole = true;
                        break;
                    }
                }
                if (!validRole) throw new InvalidRoleException(newUser.Role);

                // Check if email or user name are already being used
                await _context.Users.ForEachAsync(u =>
                {
                    if (u.Email == newUser.Email) throw new UnavailableEmailException();
                    if (u.UserName == newUser.UserName) throw new UnavailableUserNameException();
                });

                // Create user
                newUser.Password = BCrypt.Net.BCrypt.HashPassword(newUser.Password);
                User user = _mapper.Map<User>(newUser);
                _context.Users.Add(user);
                _context.SaveChanges();

                // Check if user was created
                var dbUsers = await _context.Users.ToListAsync();
                var createdUser = dbUsers.FirstOrDefault(u => u.UserName == newUser.UserName)! ?? throw new UserNotSavedException();

                // Return user with response
                serviceResponse.Data = _mapper.Map<GetUserDto>(createdUser);
                serviceResponse.Message = "User added successfully";
                return serviceResponse;
            }
            catch (Exception exception)
            {
                serviceResponse.Success = false;
                serviceResponse.Data = null;
                serviceResponse.Message = exception.Message + " " + exception;
                return serviceResponse;
            }
        }

        public async Task<UserServiceResponse<GetLoggedInUserDto>> LoginUser(LoginUserDto loginUser)
        {
            var serviceResponse = new UserServiceResponse<GetLoggedInUserDto>() { Success = false, Data = null };
            try
            {
                bool userFound = false;
                bool userVerified = false;
                var dbUsers = await _context.Users.ToListAsync();
                dbUsers.ForEach(u =>
                {
                    if (loginUser.Email != null && u.Email == loginUser.Email || loginUser.UserName != null && u.UserName == loginUser.UserName)
                    {
                        userFound = true;
                        if (BCrypt.Net.BCrypt.Verify(loginUser.Password, u.PasswordHash))
                        {
                            userVerified = true;
                            serviceResponse.Data = _mapper.Map<GetLoggedInUserDto>(u);
                            serviceResponse.Data.Token = CreateToken(u);
                            serviceResponse.Success = true;
                        }
                    }
                });
                // Need to change this to it does not tell the user if the email or user name is incorrect
                if (userFound && userVerified)
                {
                    serviceResponse.Message = "User logged in successfully.";
                    return serviceResponse;
                }
                else if (userFound && !userVerified)
                {
                    serviceResponse.Message = "Password is incorrect.";
                    return serviceResponse;
                }
                else
                {
                    serviceResponse.Message = "User could not be found.";
                    return serviceResponse;
                }
            }
            catch (Exception exception)
            {
                serviceResponse.Message = exception.Message + " " + exception;
                return serviceResponse;
            }
        }

        private string CreateToken(User user)
        {
            List<Claim> claims = new()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.UserName),
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Keys:JWT"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddHours(24),
                SigningCredentials = creds
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
