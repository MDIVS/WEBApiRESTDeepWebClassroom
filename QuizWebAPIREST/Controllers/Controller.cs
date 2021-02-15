using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuizWebAPIREST.Models;
using QuizWebAPIREST.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace QuizWebAPIREST.Controllers {
    [ApiController]
    [Route("/api")]
    public class GameController : ControllerBase {
        /// <summary>
        /// Return a game version list
        /// </summary>
        /// <returns>List with active game versions</returns>
        /// <response code="200">Success</response>
        [HttpGet("gameversion")]
        [AllowAnonymous]
        public ActionResult<List<GameData>> getGameversion() {
            using (var dbContext = new Context()) {
                return Ok(dbContext.GameDatas.ToList());
            }
        }

        /// <summary>
        /// Get the teachers from users previously registred. ONLY users with "aluno" or "professor" role.
        /// </summary>
        /// <returns>List of users with "professor" role</returns>
        /// <response code="200">Success</response>
        /// <response code="401">Unauthorized. Token not present, invalid or expired</response>
        [HttpGet("getteachers")]
        [Authorize(Roles = "aluno,professor")]
        public ActionResult<IList<User>> getTeachers() {
            using (var dbContext = new Context()) {
                IList<User> teachers = dbContext.Users.Where(x => x.Role == "professor").ToList();
                return Ok(teachers.Select((x) => new {
                    Id = x.Id,
                    Email=x.Email,
                    Role = x.Role,
                    Name = x.Name
                }));
            }
        }

        /// <summary>
        /// Get a specific teacher by one Id. ONLY users with "aluno" or "professor" role.
        /// </summary>
        /// <param name="id">Teacher id</param>
        /// <returns>User with a specific id</returns>
        /// <response code="200">Success</response>
        /// <response code="401">Unauthorized. Token not present, invalid or expired</response>
        [HttpGet("getteacherbyid/{id}")]
        [Authorize(Roles = "aluno,professor")]
        public ActionResult<dynamic> getTeacherbyId(long id) {
            using (var dbContext = new Context()) {
                return Ok(dbContext.Users.FirstOrDefault(x => x.Role == "professor" && x.Id == id));
            }
        }

        /// <summary>
        /// Get a quiz by id. ONLY Users with "professor" or "aluno" role.
        /// </summary>
        /// <param name="id">Quiz id</param>
        /// <returns>Quiz</returns>
        /// <response code="200">Success</response>
        /// <response code="401">Unauthorized. Token not present, invalid or expired</response>
        [HttpGet("getquizbyid/{id}")]
        [Authorize(Roles = "professor,aluno")]
        public ActionResult<PostQuiz> getQuizbyId(long id) {
            using (var dbContext = new Context()) {
                return Ok(dbContext.QuizPosts.FirstOrDefault(x => x.Id == id));
            }
        }
    }
}