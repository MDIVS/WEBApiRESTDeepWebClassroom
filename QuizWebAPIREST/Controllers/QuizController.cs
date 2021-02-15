using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuizWebAPIREST.Models;

namespace QuizWebAPIREST.Controllers {
    [ApiController]
    [Route("/api/quiz")]
    public class QuizController : ControllerBase {
        /// <summary>
        /// Register a quiz in database. You must pass a quiz object with some data by body. ONLY users with professor" role.
        /// </summary>
        /// <returns>object with message</returns>
        /// <response code="200">Success</response>
        /// <response code="401">Unauthorized. Token not present, invalid or expired</response>
        [HttpPost]
        [Authorize(Roles = "professor")]
        public ActionResult<dynamic> CreateQuiz([FromBody] PostQuiz model) {
            if (model.Question==""||model.Answer==""||model.NotAnswer=="") return BadRequest(new {message = "Verifique os campos em branco."});
            using (var dbContext = new Context()) {
                PostQuiz add = new PostQuiz();
                add.Author = model.Author;
                add.Question = model.Question;
                add.Answer = model.Answer;
                add.NotAnswer = model.NotAnswer;

                dbContext.QuizPosts.Add(add);
                dbContext.SaveChanges();
            }
            return Ok(new {message = "Quiz sucessfull created"});
        }

        /// <summary>
        /// Get a quiz list. ONLY users with "professor" role.
        /// </summary>
        /// <returns>Quiz list</returns>
        /// <response code="200">Success</response>
        /// <response code="401">Unauthorized. Token not present, invalid or expired</response>
        [HttpGet]
        [Authorize(Roles = "professor")]
        public ActionResult<dynamic> GetQuizList() {
            using (var dbContext = new Context()) {
                return Ok(dbContext.QuizPosts.ToList().Select(x => new {
                    Id = x.Id,
                    Author = x.Author,
                    Question = x.Question,
                    CreationDate = x.CreationDate
                }));
            }
        }

        /// <summary>
        /// Get a specific quiz by Id. ONLY users with "professor" role.
        /// </summary>
        /// <param name="id">User id</param>
        /// <returns>User with a specific id</returns>
        /// <response code="200">Success</response>
        /// <response code="401">Unauthorized. Token not present, invalid or expired</response>
        [HttpGet("{id}")]
        [Authorize(Roles = "professor")]
        public ActionResult<dynamic> GetQuizList(long id) {
            using (var dbContext = new Context()) {
                return Ok(dbContext.QuizPosts.FirstOrDefault(x => x.Id == id));
            }
        }
        
        /// <summary>
        /// Get a random public quiz.
        /// </summary>
        /// <returns>Quiz</returns>
        /// <response code="200">Success</response>
        [HttpGet("random")]
        [AllowAnonymous]
        public ActionResult<dynamic> getQuiz() {
            using (var dbContext = new Context()) {
                IList<PostQuiz> randoms = dbContext.QuizPosts.Where((s) => s.Author=="random").ToList();
                Random rnd = new Random();
                var quiz = randoms[rnd.Next(randoms.Count)];
                var answer = rnd.Next()%2 == 0 ? quiz.Answer:quiz.NotAnswer; //escolhe aleatoriamente uma das respostas
                return Ok(new {
                    Id = quiz.Id,
                    Author = quiz.Author,
                    Question = quiz.Question,
                    Answer1 = answer,
                    Answer2 = answer==quiz.NotAnswer ? quiz.Answer:quiz.NotAnswer,
                    CreationDate = quiz.CreationDate
                });
            }
        }
    }
}