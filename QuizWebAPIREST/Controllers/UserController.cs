using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuizWebAPIREST.Models;
using QuizWebAPIREST.Services;

namespace QuizWebAPIREST.Controllers {
    [ApiController]
    [Route("/api/user")]
    public class UserController : ControllerBase {
        /// <summary>
        /// Login with some previously registred user. You must pass a User by the body.
        /// </summary>
        /// <returns>Object that contains user and token information</returns>
        /// <response code="200">Success</response>
        [HttpPost("login")]
        [AllowAnonymous]
        public ActionResult<dynamic> Authenticate([FromBody] User model) {
            User user;
            using (var dbContext = new Context()) {
                user = dbContext.Users.FirstOrDefault(x => x.Email == model.Email && x.Password == model.Password);
            };
            if (user == null) return NotFound(new {message = "Usuário ou senha inválidos"});
            return Ok(new {
                user = user.Name,
                token = TokenService.GenerateToken(user)
            });
        }
        
        /// <summary>
        /// Register an account in the database. You must pass a user by the body.
        /// </summary>
        /// <returns>object with message</returns>
        /// <response code="200">Success</response>
        /// <response code="400">BadRequest: verifique as mensagens contidas na resposta da API.</response>
        [HttpPost]
        [AllowAnonymous]
        public ActionResult<dynamic> CreateAccount([FromBody] User model) {
            if (model.Email==""||model.Password==""||model.Name=="") return BadRequest(new {message = "Email, senha e nome de usuário devem ser preenchidos"});
            using (var dbContext = new Context()) {
                if (dbContext.Users.FirstOrDefault(u => u.Email == model.Email)!=null)
                    return BadRequest(new {message = "Já existe um usuário cadastrado com este email"});

                if (dbContext.Users.FirstOrDefault(u => u.Name == model.Name)!=null)
                    return BadRequest(new {message = "Nome de usuário não disponível."});
                
                User add = new User();
                add.Email = model.Email;
                add.Password = model.Password;
                add.Name = model.Name;
                add.Role = model.Role;
                add.Score = 0;

                dbContext.Users.Add(add);
                dbContext.SaveChanges();
            }
            return Ok(new {message = "user sucessfull created"});
        }

        /// <summary>
        /// Update user classroom. You must pass the user by the body. ONLY users with "aluno" role.
        /// </summary>
        /// <param name="user">User email and password</param>
        /// <param name="roomid">Room id</param>
        /// <returns>List with previously registred users</returns>
        /// <response code="200">Success</response>
        /// <response code="401">Unauthorized. Token not present, invalid or expired</response>
        [HttpPut("{roomid}")]
        [Authorize(Roles = "aluno")]
        public ActionResult<dynamic> Matricular([FromBody] User user, long roomid) {
            using (var dbContext = new Context()) {
                user = dbContext.Users.FirstOrDefault(u => u.Email == user.Email && u.Password == user.Password);
                if (user == null) return BadRequest(new {message = "Impossível matricular um usuário inválido."});
                if (dbContext.Rooms.FirstOrDefault(u => u.Id == roomid)==null) return BadRequest(new {message = "Room id informado é inválido."});
                user.Classroom = roomid;
                dbContext.Update(user);
                dbContext.SaveChanges();
            }
            return Ok(new {message = "Success"});
        }

        /// <summary>
        /// Get classroom activity. You must pass the user by the body. ONLY users with "aluno" role.
        /// </summary>
        /// <returns>Quiz</returns>
        /// <response code="200">Success</response>
        /// <response code="401">Unauthorized. Token not present, invalid or expired</response>
        [HttpPut("activity")]
        [Authorize(Roles = "aluno")]
        public ActionResult<dynamic> GetActivity([FromBody] User user) {
            using (var dbContext = new Context()) {
                user = dbContext.Users.FirstOrDefault(u => u.Email == user.Email && u.Password == user.Password);
                if (user == null) return BadRequest(new {message = "Usuário inválido."});

                var room = dbContext.Rooms.FirstOrDefault(u => u.Id == user.Classroom);
                if (room==null) return BadRequest(new {message = "Usuário não está matriculado em uma sala de aula válida."});

                var quiz = dbContext.QuizPosts.FirstOrDefault(u => u.Id == room.Quiz_id);
                if (quiz==null) return BadRequest(new {message = "Sala de aula do usuário possui quiz inválido."});

                Random rnd = new Random();
                var answer = rnd.Next()%2 == 0 ? quiz.Answer:quiz.NotAnswer; //escolhe aleatoriamente uma das respostas
                return Ok(new {
                    Author = quiz.Author,
                    Question = quiz.Question,
                    Answer1 = answer,
                    Answer2 = answer==quiz.NotAnswer ? quiz.Answer:quiz.NotAnswer,
                    CreationDate = quiz.CreationDate
                });
            }
        }

        /// <summary>
        /// Answer a classroom activity. You must pass the user by the body. ONLY users with "aluno" role.
        /// </summary>
        /// <param name="user">User email and password</param>
        /// <param name="answer">User answer</param>
        /// <returns>Quiz</returns>
        /// <response code="200">Success</response>
        /// <response code="401">Unauthorized. Token not present, invalid or expired</response>
        [HttpPut("answer")]
        [Authorize(Roles = "aluno")]
        public ActionResult<dynamic> DoActivity([FromBody] User user, string answer) {
            using (var dbContext = new Context()) {
                user = dbContext.Users.FirstOrDefault(u => u.Email == user.Email && u.Password == user.Password);
                if (user == null) return BadRequest(new {message = "Usuário inválido."});

                var room = dbContext.Rooms.FirstOrDefault(u => u.Id == user.Classroom);
                if (room==null) return BadRequest(new {message = "Usuário não está matriculado em uma sala de aula válida."});

                var quiz = dbContext.QuizPosts.FirstOrDefault(u => u.Id == room.Quiz_id);
                if (quiz==null) return BadRequest(new {message = "Sala de aula do usuário possui quiz inválido."});

                if (quiz.Answer == answer) {
                    user.Score += 1;
                    dbContext.Update(user);
                    dbContext.SaveChanges();

                    return Ok(new {message = "correct", score = 1});
                } else {
                    return Ok(new {message = "wrong answer", score = 0});
                }
            }
        }

        /// <summary>
        /// Get the users list. ONLY users with "professor" role.
        /// </summary>
        /// <returns>List with previously registred users</returns>
        /// <response code="200">Success</response>
        /// <response code="401">Unauthorized. Token not present, invalid or expired</response>
        [HttpGet]
        [Authorize(Roles = "professor")]
        public ActionResult<IList<User>> getUsers() {
            using (var dbContext = new Context()) {
                return Ok(dbContext.Users.ToList().Select(x => new {
                    Id = x.Id,
                    Email=x.Email,
                    Role = x.Role,
                    Name = x.Name,
                    Score = x.Score
                }));
            }
        }

        /// <summary>
        /// Get a specific user by one Id. ONLY users with "professor" role.
        /// </summary>
        /// <param name="id">User id</param>
        /// <returns>User with a specific id</returns>
        /// <response code="200">Success</response>
        /// <response code="401">Unauthorized. Token not present, invalid or expired</response>
        [HttpGet("{id}")]
        [Authorize(Roles = "professor")]
        public ActionResult<dynamic> getUserbyId(long id) {
            using (var dbContext = new Context()) {
                var user = dbContext.Users.FirstOrDefault(x => x.Id == id);
                return Ok(new {
                    Id = user.Id,
                    Email = user.Email,
                    Role = user.Role,
                    Name = user.Name,
                    Score = user.Score
                });
            }
        }

        /// <summary>
        /// Get user gravatar data by user id.
        /// </summary>
        /// <param name="id">User id</param>
        /// <returns>JSON with gravatar data</returns>
        /// <response code="200">Success</response>
        [HttpGet("gravatar/{id}")]
        [AllowAnonymous]
        public async Task<dynamic> getUserImageAsync(long id) {
            string email;
            using (var dbContext = new Context()) {
                var user = dbContext.Users.FirstOrDefault(x => x.Id == id);
                if (user == null) return BadRequest(new {message = "Invalid user id"});
                email = user.Email;
            }
            MD5_service md5 = new MD5_service();
            email = md5.GerarMD5(email.Trim().ToLower());

            Gravatar client = new Gravatar(email);
            var a = await client.GetGravatarAsync(email);
            return(a);
        }
    }
}