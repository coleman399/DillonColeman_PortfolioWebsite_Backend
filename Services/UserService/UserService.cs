using Microsoft.IdentityModel.Tokens;
using PortfolioWebsite_Backend.Helpers.Constants;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace PortfolioWebsite_Backend.Services.UserService
{
    public class UserService : IUserService
    {
        private readonly bool _performance_testing = false;
        private readonly UserContext _userContext;
        private readonly ContactContext _contactContext;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public UserService(UserContext userContext, ContactContext contactContext, IMapper mapper, IEmailService emailService, IHttpContextAccessor httpContextAccessor, IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            _mapper = mapper;
            _userContext = userContext;
            _contactContext = contactContext;
            _emailService = emailService;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
            if (_webHostEnvironment.EnvironmentName.Equals(Constants.PERFORMANCE_TESTING)) _performance_testing = true;
            if (_performance_testing) SeedDatabase();
        }

        private void SeedDatabase()
        {
            #region User Context
            _userContext.Database.EnsureDeleted();
            _userContext.Database.EnsureCreated();
            _userContext.AddRange(
                new User()
                {
                    Id = 2,
                    UserName = "TestSuperUser",
                    Email = "SuperUserEmail@test.test",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("SuperUserPassword1"),
                    Role = Roles.SuperUser.ToString(),
                    AccessToken = "eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOiIyIiwicm9sZSI6IlN1cGVyVXNlciIsImVtYWlsIjoiU3VwZXJVc2VyRW1haWxAdGVzdC50ZXN0IiwidW5pcXVlX25hbWUiOiJUZXN0U3VwZXJVc2VyIiwibmJmIjoxNjkwNjIwNDU5LCJleHAiOjE4NDg0NzMyNTksImlhdCI6MTY5MDYyMDQ1OX0.tmZWbXDvp2Rr_riohoedwZsP7if5gCgstoX7_Sa513xiix-fLid6Kut1ECK1ywZPBciXbLmZMhp0M1ymd_7ViA",
                    ForgotPasswordToken = new ForgotPasswordToken()
                    {
                        Id = 0,
                        Token = "eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJlbWFpbCI6IlN1cGVyVXNlckVtYWlsQHRlc3QudGVzdCIsInVuaXF1ZV9uYW1lIjoiVGVzdFN1cGVyVXNlciIsIm5iZiI6MTY5MDYyNTk0MywiZXhwIjoxODQ4NDc4NzQzLCJpYXQiOjE2OTA2MjU5NDN9.Pv-gznk_j9P_XjWVzdGwo7oU6S4eXmHV1n5qG3hrpE_v50tlExK1URFm0-xNys3UN4nwt98mBv9llAzgNdN9Og",
                        UserId = 2,
                        IsValidated = true,
                        ExpiresAt = DateTime.Now.AddDays(1),
                    },
                    RefreshToken = new RefreshToken()
                    {
                        Id = 0,
                        Token = "4e+GJZUIP8ItzBvpsIS/FmqSUxxRD0PL7B1J3ZZH1EayrfHz1+L4OO3uJ6FZyaPPHeLcWPJUBcmtJu2u3Sx6sQ==",
                        UserId = 2,
                        ExpiresAt = DateTime.Now.AddDays(1),
                        CreatedAt = DateTime.Now,
                    },
                    CreatedAt = DateTime.Now,
                },
                new User()
                {
                    Id = 3,
                    UserName = "TestAdmin1",
                    Email = "Admin1Email@test.test",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("AdminPassword1"),
                    Role = Roles.Admin.ToString(),
                    AccessToken = "eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOiIzIiwicm9sZSI6IkFkbWluIiwiZW1haWwiOiJBZG1pbjFFbWFpbEB0ZXN0LnRlc3QiLCJ1bmlxdWVfbmFtZSI6IlRlc3RBZG1pbjEiLCJuYmYiOjE2OTA2MjA1OTAsImV4cCI6MTg0ODQ3MzM5MCwiaWF0IjoxNjkwNjIwNTkwfQ.xLDgXwDwAaa0Zc145qf6aib1_RXh02L2ViN0N_3yvc7AOKzV5SMW_dVCrChrwnEzYb_1JlkICLuZMJudd9TroA",
                    ForgotPasswordToken = new ForgotPasswordToken()
                    {
                        Id = 0,
                        Token = "eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJlbWFpbCI6IkFkbWluMUVtYWlsQHRlc3QudGVzdCIsInVuaXF1ZV9uYW1lIjoiVGVzdEFkbWluMSIsIm5iZiI6MTY5MDYyNjA3NiwiZXhwIjoxODQ4NDc4ODc2LCJpYXQiOjE2OTA2MjYwNzZ9.2qaZzipNDBLjhus3yenk6Z3O2IKQoVrXMjjnD0DFM6J_XlCW2hZrPjVPYue-3IPpOiSdNRlsSRVQiTDKIHF-rQ",
                        UserId = 3,
                        IsValidated = true,
                        ExpiresAt = DateTime.Now.AddDays(1),
                    },
                    RefreshToken = new RefreshToken()
                    {
                        Id = 0,
                        Token = "4e+GJZUIP8ItzBvpsIS/FmqSUxxRD0PL7B1J3ZZH1EayrfHz1+L4OO3uJ6FZyaPPHeLcWPJUBcmtJu2u3Sx6sQ==",
                        UserId = 3,
                        ExpiresAt = DateTime.Now.AddDays(1),
                        CreatedAt = DateTime.Now,
                    },
                    CreatedAt = DateTime.Now
                },
                new User()
                {
                    Id = 4,
                    UserName = "TestAdmin2",
                    Email = "Admin2Email@test.test",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("AdminPassword2"),
                    Role = Roles.Admin.ToString(),
                    AccessToken = "eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOiI0Iiwicm9sZSI6IkFkbWluIiwiZW1haWwiOiJBZG1pbjJFbWFpbEB0ZXN0LnRlc3QiLCJ1bmlxdWVfbmFtZSI6IlRlc3RBZG1pbjIiLCJuYmYiOjE2OTA2MjA2MjksImV4cCI6MTg0ODQ3MzQyOSwiaWF0IjoxNjkwNjIwNjI5fQ.feCJN45wsLZIEtnbib_loVubUmOs_1RKiMWf-MYT2gRqqUNjnmvuadI6w3LL4y1sU1G1Zhbwmx30BvUseqegfw",
                    ForgotPasswordToken = new ForgotPasswordToken()
                    {
                        Id = 0,
                        Token = "eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJlbWFpbCI6IkFkbWluMkVtYWlsQHRlc3QudGVzdCIsInVuaXF1ZV9uYW1lIjoiVGVzdEFkbWluMiIsIm5iZiI6MTY5MDYyNjEwOCwiZXhwIjoxODQ4NDc4OTA4LCJpYXQiOjE2OTA2MjYxMDh9.tizerlPldeF3hF031FAg_7tXGJdInEUeNmJlJf7dEITgFzfyg190aob53vuban_RIq16AfQ62zzwy_SpvpzfSA",
                        UserId = 4,
                        IsValidated = true,
                        ExpiresAt = DateTime.Now.AddDays(1),
                    },
                    RefreshToken = new RefreshToken()
                    {
                        Id = 0,
                        Token = "4e+GJZUIP8ItzBvpsIS/FmqSUxxRD0PL7B1J3ZZH1EayrfHz1+L4OO3uJ6FZyaPPHeLcWPJUBcmtJu2u3Sx6sQ==",
                        UserId = 4,
                        ExpiresAt = DateTime.Now.AddDays(1),
                        CreatedAt = DateTime.Now,
                    },
                    CreatedAt = DateTime.Now,
                },
                new User()
                {
                    Id = 5,
                    UserName = "TestUser1",
                    Email = "User1Email@test.test",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("UserPassword1"),
                    Role = Roles.User.ToString(),
                    AccessToken = "eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOiI1Iiwicm9sZSI6IlVzZXIiLCJlbWFpbCI6IlVzZXIxRW1haWxAdGVzdC50ZXN0IiwidW5pcXVlX25hbWUiOiJUZXN0VXNlcjEiLCJuYmYiOjE2OTA2MjA2NTksImV4cCI6MTg0ODQ3MzQ1OSwiaWF0IjoxNjkwNjIwNjU5fQ.el_BuusOJpkrxxtPh7XGVtRQ3CO29IQ9lpg88jQa1Lk4F0RrI1_LAYlsuz1pSPWDcRBqGjCTypYwN4yLU6_uyA",
                    ForgotPasswordToken = new ForgotPasswordToken()
                    {
                        Id = 0,
                        Token = "eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJlbWFpbCI6IlVzZXIxRW1haWxAdGVzdC50ZXN0IiwidW5pcXVlX25hbWUiOiJUZXN0VXNlcjEiLCJuYmYiOjE2OTA2MjU4MTYsImV4cCI6MTg0ODQ3ODYxNiwiaWF0IjoxNjkwNjI1ODE2fQ.CgsVi1mzKjMVvoOzf3oslteQR412sxomJwiEmUQ2BPQyVX7ZrNTWImXRXW1nxZm9HvA212awstC9ioSQ1ZUWtg",
                        UserId = 5,
                        IsValidated = true,
                        ExpiresAt = DateTime.Now.AddDays(1),
                    },
                    RefreshToken = new RefreshToken()
                    {
                        Id = 0,
                        Token = "4e+GJZUIP8ItzBvpsIS/FmqSUxxRD0PL7B1J3ZZH1EayrfHz1+L4OO3uJ6FZyaPPHeLcWPJUBcmtJu2u3Sx6sQ==",
                        UserId = 5,
                        ExpiresAt = DateTime.Now.AddDays(1),
                        CreatedAt = DateTime.Now,
                    },
                    CreatedAt = DateTime.Now,
                },
                new User()
                {
                    Id = 6,
                    UserName = "TestUser2",
                    Email = "User2Email@test.test",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("UserPassword2"),
                    Role = Roles.User.ToString(),
                    AccessToken = "eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOiI2Iiwicm9sZSI6IlVzZXIiLCJlbWFpbCI6IlVzZXIyRW1haWxAdGVzdC50ZXN0IiwidW5pcXVlX25hbWUiOiJUZXN0VXNlcjIiLCJuYmYiOjE2OTA2MjA2ODcsImV4cCI6MTg0ODQ3MzQ4NywiaWF0IjoxNjkwNjIwNjg3fQ.NyMEB2tkAuPPr6iadJ_c-QzitB0ROf8biRJEhzIa6uhE3SfgLDYCzfBcpasByFHkpPhrAnemklMEnOrPVYQakg",
                    ForgotPasswordToken = new ForgotPasswordToken()
                    {
                        Id = 0,
                        Token = "eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJlbWFpbCI6IlVzZXIyRW1haWxAdGVzdC50ZXN0IiwidW5pcXVlX25hbWUiOiJUZXN0VXNlcjIiLCJuYmYiOjE2OTA2MjYxODQsImV4cCI6MTg0ODQ3ODk4NCwiaWF0IjoxNjkwNjI2MTg0fQ.LKum_iS0iFTnKcNv2gdG_qPLkLrIZaagKftI4ynd3wLWphPlfMCaVGxsDC4aMwnSd2kKAQN96ZkGE1vIG--_rw",
                        UserId = 6,
                        IsValidated = true,
                        ExpiresAt = DateTime.Now.AddDays(1),
                    },
                    RefreshToken = new RefreshToken()
                    {
                        Id = 0,
                        Token = "4e+GJZUIP8ItzBvpsIS/FmqSUxxRD0PL7B1J3ZZH1EayrfHz1+L4OO3uJ6FZyaPPHeLcWPJUBcmtJu2u3Sx6sQ==",
                        UserId = 6,
                        ExpiresAt = DateTime.Now.AddDays(1),
                        CreatedAt = DateTime.Now,
                    },
                    CreatedAt = DateTime.Now,
                },
                new User()
                {
                    Id = 7,
                    UserName = "TestUser3",
                    Email = "User3Email@test.test",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("UserPassword3"),
                    Role = Roles.User.ToString(),
                    AccessToken = "eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOiI3Iiwicm9sZSI6IlVzZXIiLCJlbWFpbCI6IlVzZXIzRW1haWxAdGVzdC50ZXN0IiwidW5pcXVlX25hbWUiOiJUZXN0VXNlcjMiLCJuYmYiOjE2OTA2MjA3MTIsImV4cCI6MTg0ODQ3MzUxMiwiaWF0IjoxNjkwNjIwNzEyfQ.stzPc5t88_HxHoDaKfd9yPh4aSJHcfAoL9UAsKP2zb3YGhdSsZEUiDsmJjQTC2cr5GgRQoRsrBmvYc-GgUfdOg",
                    ForgotPasswordToken = new ForgotPasswordToken()
                    {
                        Id = 0,
                        Token = "eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJlbWFpbCI6IlVzZXIzRW1haWxAdGVzdC50ZXN0IiwidW5pcXVlX25hbWUiOiJUZXN0VXNlcjMiLCJuYmYiOjE2OTA2MjYyMjAsImV4cCI6MTg0ODQ3OTAyMCwiaWF0IjoxNjkwNjI2MjIwfQ.a5y_5A5dsblKemD199JqlLBzg7B54oiIu22GMzD0wH9NBSMPxY7wTdX7eQ_OPL0ulGs92QsVnS2CWdeM9gjgCA",
                        UserId = 7,
                        IsValidated = true,
                        ExpiresAt = DateTime.Now.AddDays(1),
                    },
                    RefreshToken = new RefreshToken()
                    {
                        Id = 0,
                        Token = "4e+GJZUIP8ItzBvpsIS/FmqSUxxRD0PL7B1J3ZZH1EayrfHz1+L4OO3uJ6FZyaPPHeLcWPJUBcmtJu2u3Sx6sQ==",
                        UserId = 7,
                        ExpiresAt = DateTime.Now.AddDays(1),
                        CreatedAt = DateTime.Now,
                    },
                    CreatedAt = DateTime.Now,
                });
            _userContext.SaveChanges();
            #endregion

            #region Contact Context
            _contactContext.Database.EnsureDeleted();
            _contactContext.Database.EnsureCreated();
            _contactContext.AddRange(
                new Contact()
                {
                    Id = 1,
                    Name = "TestName1",
                    Email = "User1Email@test.test"
                },
                new Contact()
                {
                    Id = 2,
                    Name = "TestName2",
                    Email = "User2Email@test.test"
                },
                new Contact()
                {
                    Id = 3,
                    Name = "TestName3",
                    Email = "User3Email@test.test"
                });
            _contactContext.SaveChanges();
            #endregion
        }

        private ClaimsPrincipal ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Security:Keys:JWT"]!));
            TokenValidationParameters parameters = new()
            {
                ValidateIssuerSigningKey = true,
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateLifetime = true,
                IssuerSigningKey = key,
                ClockSkew = TimeSpan.Zero
            };
            return tokenHandler.ValidateToken(token, parameters, out _);
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
                //Expires = DateTime.Now.AddHours(5).AddSeconds(1),
                Expires = DateTime.Now.AddYears(5),
                SigningCredentials = creds
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var accessToken = tokenHandler.WriteToken(token);
            return accessToken ?? throw new Exception("Access token could not be created.");
        }

        private static RefreshToken CreateRefreshToken(User user)
        {
            var refreshToken = new RefreshToken
            {
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
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var forgotPasswordToken = tokenHandler.WriteToken(token);
            return forgotPasswordToken ?? throw new Exception("Forgot password token could not be created.");
        }

        public void TokenCheck()
        {
            int userId = int.Parse(_httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new UserNotFoundException());
            string accessToken = _httpContextAccessor.HttpContext.Request.Headers["Authorization"]!;
            string refreshToken = _httpContextAccessor.HttpContext.Request.Cookies["refreshToken"]!;
            var dbUser = _userContext.Users.FirstOrDefault(u => u.Id == userId) ?? throw new UserNotFoundException(userId);
            if (dbUser.AccessToken != accessToken.Remove(0, 7)) throw new UnauthorizedAccessException();
            if (refreshToken != dbUser.RefreshToken!.Token) throw new UnauthorizedAccessException();
            if (dbUser.RefreshToken.ExpiresAt < DateTime.Now) throw new UnauthorizedAccessException();
        }

        public async Task<UserServiceResponse<List<GetUserDto>>> GetUsers()
        {
            var serviceResponse = new UserServiceResponse<List<GetUserDto>>() { Data = null };
            try
            {
                if (_httpContextAccessor.HttpContext != null)
                {
                    //  Return all contacts in response
                    List<User> dbUsers;
                    if (_performance_testing)
                    {
                        dbUsers = _userContext.Users.Local.ToList();
                    }
                    else
                    {
                        TokenCheck();
                        dbUsers = await _userContext.Users.ToListAsync();
                    }
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
                serviceResponse.Message = $"{exception.Message}  {exception}";
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

                // Check if user is authorized to create user with role
                if (!newUser.Role.Equals(Roles.User.ToString()))
                {
                    if (!_httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.Role)!.Equals(Roles.SuperUser.ToString()))
                    {
                        throw new UnauthorizedAccessException();
                    }
                }

                // Check if email or user name are already being used
                if (!_performance_testing)
                    await _userContext.Users.ForEachAsync(u =>
                    {
                        if (u.Email == newUser.Email) throw new UnavailableEmailException();
                        if (u.UserName == newUser.UserName) throw new UnavailableUserNameException();
                    });

                // Create user
                newUser.Password = BCrypt.Net.BCrypt.HashPassword(newUser.Password);
                User createdUser = _mapper.Map<User>(newUser);

                if (!_performance_testing)
                {
                    // Save user
                    _userContext.Users.Add(createdUser);
                    _userContext.SaveChanges();

                    // Check if user was savedd
                    var dbUsers = await _userContext.Users.ToListAsync();
                    createdUser = dbUsers.FirstOrDefault(u => u.Email == newUser.Email)! ?? throw new UserNotSavedException();
                }
                else
                {
                    _userContext.Users.Add(createdUser);

                    // Check if user was saved
                    var dbUsers = _userContext.Users.Local.ToList();
                    createdUser = dbUsers.FirstOrDefault(u => u.Email == newUser.Email)! ?? throw new UserNotSavedException();
                }

                // Return user with updated response
                serviceResponse.Data = _mapper.Map<GetUserDto>(createdUser);
                serviceResponse.Message = "User registered successfully";

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
                serviceResponse.Message = $"{exception.Message}  {exception}";
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
                List<User> dbUsers;
                if (_performance_testing)
                {
                    dbUsers = _userContext.Users.Local.ToList();
                }
                else
                {
                    dbUsers = await _userContext.Users.ToListAsync();
                }
                foreach (User user in dbUsers)
                {
                    if (loginUser.Email != null && user.Email == loginUser.Email || loginUser.UserName != null && user.UserName == loginUser.UserName)
                    {
                        userFound = true;
                        if (BCrypt.Net.BCrypt.Verify(loginUser.Password, user.PasswordHash))
                        {
                            userVerified = true;
                            user.AccessToken = CreateAccessToken(user);
                            user.RefreshToken = CreateRefreshToken(user);
                            SetRefreshToken(user.RefreshToken);
                            serviceResponse.Data = _mapper.Map<GetLoggedInUserDto>(user);
                        }
                    }
                }
                if (userFound && !_performance_testing)
                {
                    // Save tokens
                    _userContext.SaveChanges();

                    // Check if tokens were saved
                    dbUsers = await _userContext.Users.ToListAsync();
                    var dbUser = dbUsers.FirstOrDefault(u => u.Email == loginUser.Email || u.UserName == loginUser.UserName)!;
                    if (dbUser.AccessToken == null || dbUser.RefreshToken == null) throw new UserFailedToUpdateException();
                }
                else if (!userFound)
                {
                    throw new UserNotFoundException();
                }

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
                serviceResponse.Message = $"{exception.Message}  {exception}";
            }
            return serviceResponse;
        }

        public async Task<UserServiceResponse<GetLoggedOutUserDto>> LogoutUser()
        {
            var serviceResponse = new UserServiceResponse<GetLoggedOutUserDto>() { Success = false, Data = null };
            try
            {
                if (_httpContextAccessor.HttpContext != null)
                {
                    if (!_performance_testing)
                    {
                        TokenCheck();

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
                    }
                    else
                    {
                        int userId = int.Parse(_httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new UserNotFoundException());
                        var dbUser = _userContext.Users.Local.FirstOrDefault(u => u.Id == userId) ?? throw new UserNotFoundException(userId);
                        dbUser.AccessToken = string.Empty;
                        dbUser.RefreshToken = null;
                        _userContext.Users.Update(dbUser);

                        var dbUsers = _userContext.Users.Local.ToList();
                        dbUser = _userContext.Users.Local.FirstOrDefault(u => u.Id == userId) ?? throw new UserNotFoundException(userId);
                        if (dbUser.AccessToken != string.Empty) throw new UserFailedToUpdateException("AccessToken failed to update.");
                        if (dbUser.RefreshToken != null) throw new UserFailedToUpdateException("RefreshToken failed to update.");
                    }

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
                serviceResponse.Message = $"{exception.Message}  {exception}";
            }
            return serviceResponse;
        }

        public async Task<UserServiceResponse<GetLoggedInUserDto>> UpdateUser(int id, UpdateUserDto updateUser)
        {
            var serviceResponse = new UserServiceResponse<GetLoggedInUserDto>() { Data = null };
            try
            {
                //Check if update user is valid
                if (!RegexFilters.IsValidUserName(updateUser.UserName!)) throw new InvalidUserNameException(updateUser.UserName!);
                if (!RegexFilters.IsValidPassword(updateUser.Password!)) throw new InvalidPasswordException(updateUser.Password!);
                if (!RegexFilters.IsValidEmail(updateUser.Email!)) throw new InvalidEmailException(updateUser.Email!);

                if (_httpContextAccessor.HttpContext != null)
                {
                    List<User> dbUsers;
                    if (!_performance_testing)
                    {
                        TokenCheck();
                        dbUsers = await _userContext.Users.ToListAsync();

                    }
                    else
                    {
                        dbUsers = _userContext.Users.Local.ToList();
                    }

                    // Check if user exists
                    var dbUser = dbUsers.FirstOrDefault(u => u.Id == id) ?? throw new UserNotFoundException(id);

                    // Check role
                    if (_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Role)!.Equals(Roles.User.ToString()))
                    {
                        // Check if user is authorized to update account, if not, throw exception
                        if (dbUser.Id.ToString() != _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier) && dbUser.Email != _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Email))
                        {
                            return serviceResponse;
                        }
                    }
                    else
                    {
                        if (dbUser.Role.Equals(Roles.SuperUser.ToString()))
                        {
                            if (_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Role)!.Equals(Roles.Admin.ToString()))
                            {
                                return serviceResponse;
                            }
                        }
                    }

                    //Check if email or user name are already being used
                    dbUsers.ForEach(u =>
                    {
                        if (u.Email == updateUser.Email && u.Id != id) throw new UnavailableEmailException();
                        if (u.UserName == updateUser.UserName && u.Id != id) throw new UnavailableUserNameException();
                    });

                    // Update user's contacts with new email
                    List<Contact> dbContacts;
                    if (!_performance_testing)
                    {
                        dbContacts = _contactContext.Contacts.Where(c => c.Email == dbUser.Email).ToList();
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
                        }
                    }
                    else
                    {
                        dbContacts = _contactContext.Contacts.Local.Where(c => c.Email == dbUser.Email).ToList();
                        foreach (var contact in dbContacts)
                        {
                            contact.Email = updateUser.Email;
                            _contactContext.Contacts.Update(contact);
                        }

                        dbContacts = _contactContext.Contacts.Local.Where(c => c.Email == dbUser.Email).ToList();
                        foreach (var contact in dbContacts)
                        {
                            if (contact.Email != dbUser.Email) throw new ContactsFailedToUpdateException();
                        }

                        // Update user 
                        // BCrypt Note: Password needs to be stored in a new variable before updating user
                        //   -hopefully that will change in the future
                        var passwordHash = BCrypt.Net.BCrypt.HashPassword(updateUser.Password);
                        dbUser.PasswordHash = passwordHash;
                        _userContext.Users.Update(_mapper.Map(updateUser, dbUser));

                        dbContacts = _contactContext.Contacts.Local.Where(c => c.Email == dbUser.Email).ToList();
                    }

                    // Email confirmation
                    List<string> sendTo = new() { dbUser.Email };
                    var email = new AccountUpdatedEmailDto()
                    {
                        To = sendTo
                    };
                    await _emailService.SendAccountHasBeenUpdatedNotification(email);

                    // Sign user out, keep admin signed in unless admin is updating their own account
                    serviceResponse.Data = new GetLoggedInUserDto();
                    if (_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Role)!.Equals(Roles.User.ToString()) || dbUser.Email != updateUser.Email || dbUser.UserName != updateUser.UserName)
                    {
                        serviceResponse.Message = "Account updated successfully. User logged out.";
                        return serviceResponse;
                    }
                    dbUser.AccessToken = CreateAccessToken(dbUser);
                    dbUser.RefreshToken = CreateRefreshToken(dbUser);
                    serviceResponse.Data.Token = dbUser.AccessToken;
                    SetRefreshToken(dbUser.RefreshToken);
                    _userContext.Users.Update(dbUser);

                    if (!_performance_testing)
                    {
                        _userContext.SaveChanges();
                        // Verify user's token was updated
                        dbUsers = await _userContext.Users.ToListAsync();
                        dbUser = dbUsers.FirstOrDefault(u => u.Id == id) ?? throw new UserNotFoundException(id);
                        if (dbUser.AccessToken != serviceResponse.Data.Token) throw new UserFailedToUpdateException("AccessToken failed to update.");
                    }

                    // Return success message
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
                serviceResponse.Message = $"{exception.Message}  {exception}";
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
                    // Check if user exists
                    User userToDelete;
                    if (!_performance_testing)
                    {
                        TokenCheck();
                        userToDelete = _userContext.Users.FirstOrDefault(c => c.Id == id) ?? throw new UserNotFoundException(id);
                    }
                    else
                    {
                        userToDelete = _userContext.Users.Local.FirstOrDefault(c => c.Id == id) ?? throw new UserNotFoundException(id);
                    }


                    // Check role
                    if (_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Role)!.Equals(Roles.User.ToString()))
                    {
                        // Check if user is authorized to delete account
                        if (userToDelete.Email != _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Email))
                        {
                            return serviceResponse;
                        }
                    }
                    else
                    {
                        if (userToDelete.Role.Equals(Roles.SuperUser.ToString()))
                        {
                            if (_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Role)!.Equals(Roles.Admin.ToString()))
                            {
                                return serviceResponse;
                            }
                            else
                            {
                                if (userToDelete.Email.Equals(_configuration["SuperUser:Email"]))
                                {
                                    return serviceResponse;
                                }
                            }
                        }
                    }

                    if (!_performance_testing)
                    {
                        // Delete user's contacts
                        await _contactContext.Contacts.ForEachAsync(c =>
                        {
                            if (c.Email == userToDelete.Email)
                            {
                                _contactContext.Contacts.Remove(c);
                            }
                        });
                        _contactContext.SaveChanges();

                        // Verify user's contacts were deleted
                        await _contactContext.Contacts.ForEachAsync(c =>
                        {
                            if (c.Email == userToDelete.Email) throw new ContactsFailedToDeleteException();
                        });
                    }
                    else
                    {
                        var contacts = _contactContext.Contacts.Local.Where(c => c.Email == userToDelete.Email).ToList();
                        foreach (var contact in contacts)
                        {
                            _contactContext.Contacts.Remove(contact);
                        }

                        // Verify user's contacts were deleted
                        contacts = _contactContext.Contacts.Local.Where(c => c.Email == userToDelete.Email).ToList();
                        if (contacts.Count != 0) throw new ContactsFailedToDeleteException();
                    }

                    // Delete user
                    _userContext.Users.Remove(userToDelete);

                    if (!_performance_testing)
                    {
                        _userContext.SaveChanges();
                        // Verify user was deleted
                        var dbUser = await _userContext.Users.FirstOrDefaultAsync(c => c.Id == id);
                        if (dbUser != null) throw new UserNotDeletedException(id);
                    }
                    else
                    {
                        var dbUser = _userContext.Users.Local.FirstOrDefault(c => c.Id == id);
                        if (dbUser != null) throw new UserNotDeletedException(id);
                    }


                    // Update response
                    serviceResponse.Data = new DeleteUserDto();
                    serviceResponse.Message = "User deleted successfully.";

                    // Email confirmation
                    List<string> sendTo = new() { userToDelete.Email };
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
                serviceResponse.Success = false;
                serviceResponse.Message = $"{exception.Message}  {exception}";
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
                    // Find user
                    int userId = int.Parse(_httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new UserNotFoundException());
                    User dbUser;
                    if (!_performance_testing)
                    {
                        TokenCheck();
                        dbUser = _userContext.Users.FirstOrDefault(u => u.Id == userId) ?? throw new UserNotFoundException(userId);
                    }
                    else
                    {
                        dbUser = _userContext.Users.Local.FirstOrDefault(u => u.Id == userId) ?? throw new UserNotFoundException(userId);
                    }

                    // Update user's token and refresh token
                    dbUser.AccessToken = CreateAccessToken(dbUser);
                    dbUser.RefreshToken = CreateRefreshToken(dbUser);
                    _userContext.Users.Update(dbUser);

                    if (!_performance_testing)
                    {
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
                    }

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
                serviceResponse.Message = $"{exception.Message}  {exception}";
            }
            return serviceResponse;
        }

        public async Task<UserServiceResponse<GetForgotPasswordUserDto>> ForgotPassword(ForgotPasswordUserDto user)
        {
            var serviceResponse = new UserServiceResponse<GetForgotPasswordUserDto>() { Data = null };
            try
            {
                // Verify user exists
                List<User> dbUsers;
                User dbUser;
                if (!_performance_testing)
                {
                    dbUsers = await _userContext.Users.ToListAsync();
                }
                else
                {
                    dbUsers = _userContext.Users.Local.ToList();
                }
                dbUser = (user.UserName.IsNullOrEmpty() ? dbUsers.FirstOrDefault(u => u.Email == user.Email) : dbUsers.FirstOrDefault(u => u.UserName == user.UserName)) ?? dbUsers.FirstOrDefault(u => u.Email == user.Email) ?? throw new UserNotFoundException();

                // Generate token
                var token = CreateForgotPasswordToken(dbUser);

                // Save token
                var forgotPasswordToken = new ForgotPasswordToken
                {
                    Token = token,
                    UserId = dbUser.Id,
                    User = dbUser,
                    CreatedAt = DateTime.Now,
                    ExpiresAt = DateTime.Now.AddMinutes(30),
                };
                dbUser.ForgotPasswordToken = forgotPasswordToken;
                _userContext.Users.Update(dbUser);

                //Verify token was saved
                if (!_performance_testing)
                {
                    _userContext.SaveChanges();

                    dbUsers = await _userContext.Users.ToListAsync();
                }
                else
                {
                    dbUsers = _userContext.Users.Local.ToList();
                }
                dbUser = (dbUsers.FirstOrDefault(u => u.Id == dbUser.Id)!);
                if (dbUser!.ForgotPasswordToken!.Token != token) throw new UserFailedToUpdateException("ForgotPasswordToken failed to update.");



                // Send email with token
                List<string> sendTo = new() { dbUser.Email! };
                var email = new ForgotPasswordEmailDto()
                {
                    To = sendTo,
                    Body = $"Click the link below to reset your password. <br><br>\n {_configuration["Security:Issuer:Url"]}/api/Auth/resetPasswordConfirmation?token={token}"
                };
                await _emailService.SendForgetPassword(email);

                // Update response
                serviceResponse.Data = new GetForgotPasswordUserDto();
                serviceResponse.Message = "Forgot Password Operation Complete.";
            }
            catch (Exception exception)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = $"{exception.Message}  {exception}";
            }
            return serviceResponse;
        }

        public async Task<UserServiceResponse<GetResetPasswordUserDto>> ResetPasswordConfirmation(string token)
        {
            var serviceResponse = new UserServiceResponse<GetResetPasswordUserDto>() { Data = null };
            try
            {
                // Validate token
                ClaimsPrincipal claimsPrincipal;
                try
                {
                    claimsPrincipal = ValidateToken(token);
                }
                catch
                {
                    return serviceResponse;
                }

                // Find user
                List<User> dbUsers;
                User dbUser;
                if (!_performance_testing)
                {
                    dbUsers = await _userContext.Users.ToListAsync();
                    dbUser = dbUsers.FirstOrDefault(u => claimsPrincipal.FindFirstValue(ClaimTypes.Email) == u.Email) ?? dbUsers.FirstOrDefault(u => claimsPrincipal.FindFirstValue(ClaimTypes.Name) == u.UserName) ?? throw new UserNotFoundException();
                }
                else
                {
                    dbUsers = _userContext.Users.Local.ToList();
                    dbUser = dbUsers.FirstOrDefault(u => claimsPrincipal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value == u.Email) ?? dbUsers.FirstOrDefault(u => claimsPrincipal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value == u.UserName) ?? throw new UserNotFoundException();
                }

                // Validate that Users ResetPasswordToken is the same as the incoming token then set it to validated
                // add more to filter
                if (dbUser.ForgotPasswordToken == null || !dbUser.ForgotPasswordToken.Token.Equals(token) || dbUser.ForgotPasswordToken.ExpiresAt < DateTime.Now)
                {
                    return serviceResponse;
                }
                else
                {
                    dbUser.ForgotPasswordToken.IsValidated = true;
                    _userContext.Users.Update(dbUser);
                }

                if (!_performance_testing)
                {
                    _userContext.SaveChanges();

                    // Verify that the token was set to validated
                    dbUsers = await _userContext.Users.ToListAsync();
                }
                else
                {
                    dbUsers = _userContext.Users.Local.ToList();
                }

                dbUser = dbUsers.FirstOrDefault(u => dbUser.Id == u.Id)!;
                if (!dbUser!.ForgotPasswordToken!.IsValidated) throw new UserFailedToUpdateException();

                // Update Response
                dbUser.AccessToken = CreateAccessToken(dbUser);
                serviceResponse.Data = new GetResetPasswordUserDto() { Token = dbUser.AccessToken };
                serviceResponse.Message = "Reset Password Confirmation Operation Complete.";
            }
            catch (Exception exception)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = $"{exception.Message}  {exception}";
            }
            return serviceResponse;
        }

        public async Task<UserServiceResponse<PasswordResetUserDto>> ResetPassword(ResetPasswordUserDto resetPasswordDto)
        {
            var serviceResponse = new UserServiceResponse<PasswordResetUserDto>() { Data = null };
            try
            {
                if (!RegexFilters.IsValidPassword(resetPasswordDto.Password)) throw new InvalidPasswordException(resetPasswordDto.Password);

                if (_httpContextAccessor.HttpContext != null)
                {
                    // Find user
                    var userId = int.Parse(_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new UserNotFoundException());

                    User dbUser;
                    if (!_performance_testing)
                    {
                        dbUser = _userContext.Users.FirstOrDefault(u => u.Id == userId) ?? throw new UserNotFoundException(userId);
                    }
                    else
                    {
                        dbUser = _userContext.Users.Local.FirstOrDefault(u => u.Id == userId) ?? throw new UserNotFoundException(userId);
                    }

                    // Update password
                    dbUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword(resetPasswordDto.Password);
                    dbUser.AccessToken = string.Empty;
                    _userContext.Users.Update(dbUser);

                    List<User> dbUsers;
                    if (!_performance_testing)
                    {
                        _userContext.SaveChanges();

                        // Verify user was saved
                        dbUsers = await _userContext.Users.ToListAsync();
                        dbUser = _userContext.Users.FirstOrDefault(u => u.Id == userId) ?? throw new UserNotFoundException(userId);
                    }
                    else
                    {
                        dbUsers = _userContext.Users.Local.ToList();
                        dbUser = _userContext.Users.Local.FirstOrDefault(u => u.Id == userId) ?? throw new UserNotFoundException(userId);
                    }

                    if (!BCrypt.Net.BCrypt.Verify(resetPasswordDto.Password, dbUser.PasswordHash)) throw new UserFailedToUpdateException();

                    // Update response
                    serviceResponse.Data = new PasswordResetUserDto();
                    serviceResponse.Message = "Reset Password Operation Complete.";
                }
                else
                {
                    throw new HttpContextFailureException();
                }

            }
            catch (Exception exception)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = $"{exception.Message}  {exception}";
            }
            return serviceResponse;
        }
    }
}
