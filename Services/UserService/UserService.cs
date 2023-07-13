using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace PortfolioWebsite_Backend.Services.UserService
{
    public class UserService : IUserService
    {
        private readonly IMapper _mapper;
        private readonly UserContext _userContext;
        private readonly ContactContext _contactContext;
        private readonly IEmailService _emailService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;

        public UserService(IMapper mapper, UserContext userContext, ContactContext contactContext, IEmailService emailService, IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _mapper = mapper;
            _userContext = userContext;
            _contactContext = contactContext;
            _emailService = emailService;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
        }

        private ClaimsPrincipal ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = (JwtSecurityToken)tokenHandler.ReadToken(token) ?? throw new Exception();
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Security:Keys:JWT"]!));
            TokenValidationParameters parameters = new TokenValidationParameters()
            {
                ValidateIssuerSigningKey = true,
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateLifetime = true,
                IssuerSigningKey = key,
                ClockSkew = TimeSpan.Zero
            };
            SecurityToken securityToken;
            return tokenHandler.ValidateToken(token, parameters, out securityToken);
        }

        private string CreateAccessToken(User user)
        {
            List<Claim> claims = new()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.UserName),
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Security:Keys:JWT"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddHours(5).AddSeconds(1),
                SigningCredentials = creds
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var accessToken = tokenHandler.WriteToken(token);
            return accessToken ?? throw new Exception("Access token could not be created.");
        }

        private RefreshToken CreateRefreshToken(User user)
        {
            var refreshToken = new RefreshToken
            {
                Id = user.RefreshToken!.Id,
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                UserId = user.Id,
                User = user,
                CreatedAt = DateTime.Now,
                ExpiresAt = DateTime.Now.AddDays(1),
            };
            return refreshToken;
        }

        private void SetRefreshToken(RefreshToken refreshToken)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = refreshToken.ExpiresAt,

            };
            _httpContextAccessor.HttpContext!.Response.Cookies.Append("refreshToken", refreshToken.Token, cookieOptions);
            _httpContextAccessor.HttpContext!.Response.Cookies.Append("refreshTokenId", refreshToken.Id.ToString(), cookieOptions);
        }

        private string CreateForgotPasswordToken(User user)
        {
            List<Claim> claims = new()
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.UserName),
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Security:Keys:JWT"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddMinutes(5),
                SigningCredentials = creds
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var forgotPasswordToken = tokenHandler.WriteToken(token);
            return forgotPasswordToken ?? throw new Exception("Forgot password token could not be created.");
        }

        public void TokenCheck()
        {
            int userId = int.Parse(_httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new UserNotFoundException());
            int refreshTokenId = int.Parse(_httpContextAccessor.HttpContext.Request.Cookies["refreshTokenId"] ?? throw new UnauthorizedAccessException());
            string accessToken = _httpContextAccessor.HttpContext.Request.Headers["Authorization"]!;
            string refreshToken = _httpContextAccessor.HttpContext.Request.Cookies["refreshToken"] ?? throw new UnauthorizedAccessException();
            var dbUser = _userContext.Users.FirstOrDefault(u => u.Id == userId) ?? throw new UserNotFoundException(userId);
            if (dbUser.AccessToken != accessToken.Remove(0, 7)) throw new UnauthorizedAccessException();
            if (refreshToken != dbUser.RefreshToken!.Token) throw new UnauthorizedAccessException();
            if (refreshTokenId != dbUser.RefreshToken.Id) throw new UnauthorizedAccessException();
            if (dbUser.RefreshToken.ExpiresAt < DateTime.Now) throw new UnauthorizedAccessException();
        }

        public async Task<UserServiceResponse<List<GetUserDto>>> GetUsers()
        {
            var serviceResponse = new UserServiceResponse<List<GetUserDto>>() { Data = null };
            try
            {
                if (_httpContextAccessor.HttpContext != null)
                {
                    TokenCheck();

                    //  Return all contacts in response
                    var dbUsers = await _userContext.Users.ToListAsync();
                    serviceResponse.Data = dbUsers.Select(c => _mapper.Map<GetUserDto>(c)).ToList();
                }
                else
                {
                    throw new HttpContextFailureException();
                }
            }
            catch (Exception exception)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = exception.Message + " " + exception;
            }
            return serviceResponse;
        }

        public async Task<UserServiceResponse<GetUserDto>> RegisterUser(RegisterUserDto newUser)
        {
            var serviceResponse = new UserServiceResponse<GetUserDto>() { Data = null };
            try
            {
                // Check if user is valid
                if (!RegexFilters.IsValidUserName(newUser.UserName!)) throw new InvalidUserNameException(newUser.UserName!);
                if (!RegexFilters.IsValidPassword(newUser.Password!)) throw new InvalidPasswordException(newUser.Password!);
                if (!RegexFilters.IsValidEmail(newUser.Email!)) throw new InvalidEmailException(newUser.Email!);

                // Check if user role is valid
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
                if (!newUser.Role.Equals(Roles.User.ToString()))
                {
                    if (!_httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.Role)!.Equals(Roles.SuperUser.ToString()))
                    {
                        throw new UnauthorizedAccessException();
                    }
                }

                // Check if email or user name are already being used
                await _userContext.Users.ForEachAsync(u =>
                {
                    if (u.Email == newUser.Email) throw new UnavailableEmailException();
                    if (u.UserName == newUser.UserName) throw new UnavailableUserNameException();
                });

                // Create user
                newUser.Password = BCrypt.Net.BCrypt.HashPassword(newUser.Password);
                User user = _mapper.Map<User>(newUser);
                _userContext.Users.Add(user);
                _userContext.SaveChanges();

                // Check if user was created
                var dbUsers = await _userContext.Users.ToListAsync();
                var createdUser = dbUsers.FirstOrDefault(u => u.Email == newUser.Email)! ?? throw new UserNotSavedException();

                // Return user with updated response
                serviceResponse.Data = _mapper.Map<GetUserDto>(createdUser);
                serviceResponse.Message = "User added successfully";

                // Email confirmation
                List<string> sendTo = new() { createdUser.Email };
                var email = new AccountCreatedEmailDto()
                {
                    To = sendTo
                };
                await _emailService.SendAccountHasBeenCreatedNotification(email);

            }
            catch (Exception exception)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = exception.Message + " " + exception;
            }
            return serviceResponse;
        }

        public async Task<UserServiceResponse<GetLoggedInUserDto>> LoginUser(LoginUserDto loginUser)
        {
            var serviceResponse = new UserServiceResponse<GetLoggedInUserDto>() { Data = null };
            try
            {
                bool userFound = false;
                bool userVerified = false;
                await _userContext.Users.ForEachAsync(u =>
                {
                    if (loginUser.Email != null && u.Email == loginUser.Email || loginUser.UserName != null && u.UserName == loginUser.UserName)
                    {
                        userFound = true;
                        if (BCrypt.Net.BCrypt.Verify(loginUser.Password, u.PasswordHash))
                        {
                            userVerified = true;
                            u.AccessToken = CreateAccessToken(u);
                            u.RefreshToken = CreateRefreshToken(u);
                            SetRefreshToken(u.RefreshToken);
                            serviceResponse.Data = _mapper.Map<GetLoggedInUserDto>(u);
                        }
                    }
                });
                _userContext.SaveChanges();

                // Need to change this to it does not tell the user if the email or user name is incorrect
                if (userFound && userVerified)
                {
                    serviceResponse.Message = "User logged in successfully.";
                }
                else if (userFound && !userVerified)
                {
                    serviceResponse.Message = "Password is incorrect.";
                }
                else
                {
                    serviceResponse.Message = "User could not be found.";
                }
            }
            catch (Exception exception)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = exception.Message + " " + exception;
            }
            return serviceResponse;
        }

        public async Task<UserServiceResponse<GetLoggedInUserDto>> UpdateUser(int id, UpdateUserDto updateUser)
        {
            var serviceResponse = new UserServiceResponse<GetLoggedInUserDto>() { Data = null };
            try
            {
                // Check if update user is valid
                if (!RegexFilters.IsValidUserName(updateUser.UserName!)) throw new InvalidUserNameException(updateUser.UserName!);
                if (!RegexFilters.IsValidPassword(updateUser.Password!)) throw new InvalidPasswordException(updateUser.Password!);
                if (!RegexFilters.IsValidEmail(updateUser.Email!)) throw new InvalidEmailException(updateUser.Email!);

                if (_httpContextAccessor.HttpContext != null)
                {
                    TokenCheck();
                    serviceResponse.Success = false;

                    // Check if user exists
                    var dbUsers = await _userContext.Users.ToListAsync();
                    var dbUser = dbUsers.FirstOrDefault(u => u.Id == id) ?? throw new UserNotFoundException(id);

                    // Check if email or user name are already being used
                    dbUsers.ForEach(u =>
                    {
                        if (u.Email == updateUser.Email && u.Id != id) throw new UnavailableEmailException();
                        if (u.UserName == updateUser.UserName && u.Id != id) throw new UnavailableUserNameException();
                    });

                    // Check role
                    if (_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Role)!.Equals(Roles.User.ToString()))
                    {
                        // Check if user is authorized to update account, if not, throw exception
                        if (dbUser.Id.ToString() != _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier))
                        {
                            serviceResponse.Success = true;
                            throw new UnauthorizedAccessException();
                        }
                    }

                    // Update user's contacts with new email
                    List<Contact> dbContacts = _contactContext.Contacts.Where(c => c.Email == dbUser.Email).ToList();
                    if (dbContacts.Count != 0 && dbUser.Email != updateUser.Email)
                    {
                        foreach (var contact in dbContacts)
                        {
                            contact.Email = updateUser.Email;
                            _contactContext.Contacts.Update(contact);
                        }
                        _contactContext.SaveChanges();

                        // Verify user's contacts were updated with new email
                        await _contactContext.Contacts.ForEachAsync(c =>
                        {
                            if (c.Email == dbUser.Email) throw new ContactsFailedToUpdateException();
                        });
                    }

                    // Update user 
                    // BCrypt Note: Password needs to be stored in a new variable before updating user
                    //   -hopefully that will change in the future
                    var passwordHash = BCrypt.Net.BCrypt.HashPassword(updateUser.Password);
                    dbUser.PasswordHash = passwordHash;
                    _userContext.Users.Update(_mapper.Map(updateUser, dbUser));
                    _userContext.SaveChanges();

                    // Verify user was updated
                    dbUsers = await _userContext.Users.ToListAsync();
                    dbUser = dbUsers.FirstOrDefault(u => u.Id == id) ?? throw new UserNotFoundException(id);
                    if (dbUser.Email != updateUser.Email && dbUser.UserName != updateUser.UserName || !BCrypt.Net.BCrypt.Verify(updateUser.Password, dbUser.PasswordHash)) throw new UserFailedToUpdateException();

                    // Email confirmation
                    List<string> sendTo = new() { dbUser.Email };
                    var email = new AccountUpdatedEmailDto()
                    {
                        To = sendTo
                    };
                    await _emailService.SendAccountHasBeenUpdatedNotification(email);

                    // Sign user out, keep admin signed in unless admin is updating their own account
                    serviceResponse.Data = _mapper.Map<GetLoggedInUserDto>(dbUser);
                    if (_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Role)!.Equals(Roles.User.ToString()) || dbUser.Email != updateUser.Email || dbUser.UserName != updateUser.UserName)
                    {
                        await Logout();
                        serviceResponse.Data.Token = string.Empty;
                        serviceResponse.Success = true;
                        serviceResponse.Message = "Account updated successfully. User logged out.";
                        return serviceResponse;
                    }
                    dbUser.AccessToken = CreateAccessToken(dbUser);
                    dbUser.RefreshToken = CreateRefreshToken(dbUser);
                    serviceResponse.Data.Token = dbUser.AccessToken;
                    SetRefreshToken(dbUser.RefreshToken);
                    _userContext.Users.Update(dbUser);
                    _userContext.SaveChanges();

                    // Verify user's token was updated
                    dbUsers = await _userContext.Users.ToListAsync();
                    dbUser = dbUsers.FirstOrDefault(u => u.Id == id) ?? throw new UserNotFoundException(id);
                    if (dbUser.AccessToken != serviceResponse.Data.Token) throw new UserFailedToUpdateException("AccessToken failed to update.");

                    // Return success message
                    serviceResponse.Success = true;
                    serviceResponse.Message = "User updated successfully.";
                }
                else
                {
                    throw new HttpContextFailureException();
                }
            }
            catch (Exception exception)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = exception.Message + " " + exception;
            }
            return serviceResponse;
        }

        public async Task<UserServiceResponse<DeleteUserDto>> DeleteUser(int id)
        {
            var serviceResponse = new UserServiceResponse<DeleteUserDto>() { Data = null };
            try
            {
                if (_httpContextAccessor.HttpContext != null)
                {
                    TokenCheck();
                    serviceResponse.Success = false;

                    // Check if user exists
                    var user = _userContext.Users.FirstOrDefault(c => c.Id == id) ?? throw new UserNotFoundException(id);

                    // Check role
                    if (_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Role)!.Equals(Roles.User.ToString()))
                    {
                        // Check if user is authorized to delete account, if not, throw exception
                        if (user.Email != _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Email))
                        {
                            serviceResponse.Success = true;
                            throw new UnauthorizedAccessException();
                        }
                    }

                    // Delete user's contacts
                    await _contactContext.Contacts.ForEachAsync(c =>
                    {
                        if (c.Email == user.Email)
                        {
                            _contactContext.Contacts.Remove(c);
                        }
                    });
                    _contactContext.SaveChanges();

                    // Verify user's contacts were deleted
                    await _contactContext.Contacts.ForEachAsync(c =>
                    {
                        if (c.Email == user.Email) throw new ContactsFailedToDeleteException();
                    });

                    // Delete user
                    _userContext.Users.Remove(user);
                    _userContext.SaveChanges();

                    // Verify user was deleted
                    var dbUser = await _userContext.Users.FirstOrDefaultAsync(c => c.Id == id);
                    if (dbUser != null) throw new UserNotDeletedException(id);

                    // Update response
                    serviceResponse.Success = true;
                    serviceResponse.Data = new DeleteUserDto();
                    serviceResponse.Message = "User deleted successfully.";

                    // Log user out
                    await Logout();

                    // Email confirmation
                    List<string> sendTo = new() { user.Email };
                    var email = new AccountDeletedEmailDto()
                    {
                        To = sendTo
                    };
                    await _emailService.SendAccountHasBeenDeletedNotification(email);
                }
                else
                {
                    throw new HttpContextFailureException();
                }
            }
            catch (Exception exception)
            {
                serviceResponse.Message = exception.Message + " " + exception;
            }
            return serviceResponse;
        }

        public async Task<UserServiceResponse<GetLoggedInUserDto>> RefreshToken()
        {
            var serviceResponse = new UserServiceResponse<GetLoggedInUserDto>() { Data = null };
            try
            {
                if (_httpContextAccessor.HttpContext != null)
                {
                    TokenCheck();

                    // Find user
                    int userId = int.Parse(_httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new UserNotFoundException());
                    var dbUser = _userContext.Users.FirstOrDefault(u => u.Id == userId) ?? throw new UserNotFoundException(userId);

                    // Update user's token and refresh token
                    dbUser.AccessToken = CreateAccessToken(dbUser);
                    dbUser.RefreshToken = CreateRefreshToken(dbUser);
                    _userContext.Users.Update(dbUser);
                    _userContext.SaveChanges();

                    // Verify user's token was updated
                    var dbUsers = await _userContext.Users.ToListAsync();
                    dbUser = _userContext.Users.FirstOrDefault(u => u.Id == userId) ?? throw new UserNotFoundException(userId);
                    if (dbUser.AccessToken != dbUser.AccessToken) throw new UserFailedToUpdateException("AccessToken failed to update.");
                    if (dbUser.RefreshToken!.Id != dbUser.RefreshToken.Id) throw new UserFailedToUpdateException("RefreshToken failed to update.");
                    if (dbUser.RefreshToken.User != dbUser) throw new UserFailedToUpdateException("RefreshToken failed to update.");
                    if (dbUser.RefreshToken.UserId != dbUser.Id) throw new UserFailedToUpdateException("RefreshToken failed to update.");
                    if (dbUser.RefreshToken.Token != dbUser.RefreshToken.Token) throw new UserFailedToUpdateException("RefreshToken failed to update.");
                    if (dbUser.RefreshToken.ExpiresAt != dbUser.RefreshToken.ExpiresAt) throw new UserFailedToUpdateException("RefreshToken failed to update.");
                    if (dbUser.RefreshToken.CreatedAt != dbUser.RefreshToken.CreatedAt) throw new UserFailedToUpdateException("RefreshToken failed to update.");

                    // Update response
                    SetRefreshToken(dbUser.RefreshToken);
                    serviceResponse.Data = _mapper.Map<GetLoggedInUserDto>(dbUser);
                    serviceResponse.Data.Token = dbUser.AccessToken;
                }
                else
                {
                    throw new HttpContextFailureException();
                }
            }
            catch (Exception exception)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = exception.Message + " " + exception;
            }
            return serviceResponse;
        }

        public async Task<UserServiceResponse<GetLoggedOutUserDto>> Logout()
        {
            var serviceResponse = new UserServiceResponse<GetLoggedOutUserDto>() { Success = false, Data = null };
            try
            {
                if (_httpContextAccessor.HttpContext != null)
                {
                    // find user
                    int userId = int.Parse(_httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new UserNotFoundException());
                    var dbUser = _userContext.Users.FirstOrDefault(u => u.Id == userId) ?? throw new UserNotFoundException(userId);

                    // delete access and refresh token
                    dbUser.AccessToken = string.Empty;
                    dbUser.RefreshToken = null;
                    _userContext.Users.Update(dbUser);
                    _httpContextAccessor.HttpContext.Response.Cookies.Delete("refreshToken");
                    _httpContextAccessor.HttpContext.Response.Cookies.Delete("refreshTokenId");

                    // Verify user's token was updated
                    var dbUsers = await _userContext.Users.ToListAsync();
                    dbUser = _userContext.Users.FirstOrDefault(u => u.Id == userId) ?? throw new UserNotFoundException(userId);
                    if (dbUser.AccessToken != string.Empty) throw new UserFailedToUpdateException("AccessToken failed to update.");
                    if (dbUser.RefreshToken != null) throw new UserFailedToUpdateException("RefreshToken failed to update.");

                    // update response
                    serviceResponse.Success = true;
                    serviceResponse.Data = new GetLoggedOutUserDto();
                    serviceResponse.Message = "User logged out successfully.";
                }
                else
                {
                    throw new HttpContextFailureException();
                }
            }
            catch (Exception exception)
            {
                serviceResponse.Message = exception.Message + " " + exception;
            }
            return serviceResponse;
        }

        public async Task<UserServiceResponse<GetForgotPasswordUserDto>> ForgotPassword(LoginUserDto user)
        {

            var serviceResponse = new UserServiceResponse<GetForgotPasswordUserDto>() { Success = false, Data = null };
            try
            {
                // Verify user exists
                var dbUsers = await _userContext.Users.ToListAsync();
                var dbUser = (user.UserName.IsNullOrEmpty() ? dbUsers.FirstOrDefault(u => u.Email == user.Email) : dbUsers.FirstOrDefault(u => u.UserName == user.UserName)) ?? throw new UserNotFoundException();

                // Generate token
                var token = CreateForgotPasswordToken(dbUser);

                // Save token
                var forgotPasswordToken = new ForgotPasswordToken
                {
                    Id = dbUser.ForgotPasswordToken!.Id,
                    Token = token,
                    UserId = dbUser.Id,
                    User = dbUser,
                    CreatedAt = DateTime.Now,
                    ExpiresAt = DateTime.Now.AddMinutes(30),
                };
                dbUser.ForgotPasswordToken = forgotPasswordToken;
                _userContext.Users.Update(dbUser);

                // Verify token was saved
                dbUsers = await _userContext.Users.ToListAsync();
                dbUser = (dbUsers.FirstOrDefault(u => u.Id == dbUser.Id));
                if (dbUser!.ForgotPasswordToken!.Token != token) throw new UserFailedToUpdateException("ForgotPasswordToken failed to update.");

                // Send email with token
                List<string> sendTo = new() { dbUser.Email! };
                var email = new ForgotPasswordEmailDto()
                {
                    To = sendTo,
                    Body = "Click the link below to reset your password." + Environment.NewLine + _configuration.GetSection("Security:Issuer:Url") + "/api/Auth/forgotPassword/" + token
                };
                await _emailService.SendForgetPassword(email);

                // Update response
                serviceResponse.Success = true;
                serviceResponse.Data = new GetForgotPasswordUserDto();
                serviceResponse.Message = "Forgot Password Operation Successfully Completed";
            }
            catch (Exception exception)
            {
                serviceResponse.Message = exception.Message + " " + exception;
            }
            return serviceResponse;
        }

        public async Task<UserServiceResponse<GetResetPasswordUserDto>> ResetPasswordConfirmation(string token)
        {
            // Verify token
            var serviceResponse = new UserServiceResponse<GetForgotPasswordUserDto>() { Success = false, Data = null };
            try
            {
                // Find user
                ClaimsPrincipal principal = new JwtSecurityTokenHandler().ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration.GetSection("Security:Issuer:SigningKey").Value!)),
                    ValidateIssuer = true,
                    ValidIssuer = _configuration.GetSection("Security:Issuer:Url").Value,
                    ValidateAudience = true,
                    ValidAudience = _configuration.GetSection("Security:Issuer:Url").Value,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);


            }
            catch (Exception exception)
            {

                throw;
            }

            // Create new password

            // Update user

            // Send email with link to reset password

            // Update response

            throw new NotImplementedException();
        }

        public async Task<UserServiceResponse<GetLoggedInUserDto>> ResetPassword(LoginUserDto user)
        {
            // Verify user 

            // Update password

            // Save user

            // Update response

            throw new NotImplementedException();
        }
    }
}
