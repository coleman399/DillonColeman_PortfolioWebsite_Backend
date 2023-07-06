using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PortfolioWebsite_Backend.Services.AuthService
{
    public class AuthService : IAuthService
    {
        private readonly IMapper _mapper;
        private readonly UserContext _userContext;
        private readonly ContactContext _contactContext;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthService(IMapper mapper, UserContext userContext, ContactContext contactContext, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _mapper = mapper;
            _userContext = userContext;
            _contactContext = contactContext;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<UserServiceResponse<List<GetUserDto>>> GetUsers()
        {
            var serviceResponse = new UserServiceResponse<List<GetUserDto>>();
            try
            {
                //  Return all contacts in response
                var dbUsers = await _userContext.Users.ToListAsync();
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
                await _userContext.Users.ForEachAsync(u =>
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

        public async Task<UserServiceResponse<GetLoggedInUserDto>> UpdateUser(int id, UpdateUserDto updateUser)
        {
            var serviceResponse = new UserServiceResponse<GetLoggedInUserDto>() { Success = false, Data = null };
            try
            {
                // Check if user name is valid
                if (!RegexFilters.IsValidUserName(updateUser.UserName!)) throw new InvalidUserNameException(updateUser.UserName!);

                // Check if password is valid
                if (!RegexFilters.IsValidPassword(updateUser.Password!)) throw new InvalidPasswordException(updateUser.Password!);

                // Email is always required, check if it's valid
                if (!RegexFilters.IsValidEmail(updateUser.Email!)) throw new InvalidEmailException(updateUser.Email!);

                if (_httpContextAccessor.HttpContext != null)
                {
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
                            throw new UnauthorizedAccessException("You are not authorized to update user.");
                        }
                    }

                    // Update user's contacts
                    var test = _contactContext.Contacts.Where(c => c.Email == dbUser.Email).ToList().Select(c => { c.Email = updateUser.Email; return c; }).ToList();
                    foreach (var contact in test)
                    {
                        contact.Email = updateUser.Email;
                        _contactContext.Contacts.Update(contact);
                    }
                    _contactContext.SaveChanges();

                    // Verify user's contacts were updated
                    await _contactContext.Contacts.ForEachAsync(c =>
                    {
                        if (c.Email == dbUser.Email) throw new ContactsFailedToUpdateException();
                    });

                    // Update user 
                    // password is being updated *** need to fix this ***
                    updateUser.Password = BCrypt.Net.BCrypt.HashPassword(updateUser.Password!);
                    _userContext.Users.Update(_mapper.Map(updateUser, dbUser));
                    _userContext.SaveChanges();

                    // Verify user was updated
                    dbUsers = await _userContext.Users.ToListAsync();
                    dbUser = dbUsers.FirstOrDefault(u => u.Id == id) ?? throw new UserNotFoundException(id);
                    if (dbUser.Email != updateUser.Email && dbUser.UserName != updateUser.UserName || BCrypt.Net.BCrypt.Verify(dbUser.PasswordHash, updateUser.Password)) throw new UserFailedToUpdateException();

                    // Update response
                    serviceResponse.Success = true;
                    serviceResponse.Data = _mapper.Map<GetLoggedInUserDto>(dbUser);
                    if (_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Role)!.Equals(Roles.User.ToString()))
                    {
                        serviceResponse.Data.Token = CreateToken(dbUser);
                    }
                    serviceResponse.Message = "User updated successfully.";
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

        public async Task<UserServiceResponse<DeleteUserDto>> DeleteUser(int id)
        {
            var serviceResponse = new UserServiceResponse<DeleteUserDto>() { Success = false, Data = null };
            try
            {
                // Check if user exists
                var user = _userContext.Users.FirstOrDefault(c => c.Id == id) ?? throw new UserNotFoundException(id);
                if (_httpContextAccessor.HttpContext != null)
                {
                    // Check role
                    if (_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Role)!.Equals(Roles.User.ToString()))
                    {
                        // Check if user is authorized to delete account, if not, throw exception
                        if (user.Email != _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Email))
                        {
                            serviceResponse.Success = true;
                            throw new UnauthorizedAccessException("You are not authorized to delete user.");
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
                    serviceResponse.Data = _mapper.Map<DeleteUserDto>(user);
                    serviceResponse.Message = "User deleted successfully.";
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
    }
}
