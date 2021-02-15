using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuizWebAPIREST.Models;

namespace QuizWebAPIREST.Controllers {
    [ApiController]
    [Route("api/room")]
    public class RoomController : ControllerBase {
        /// <summary>
        /// Used to create a classroom. You must pass a Room data by body. ONLY users with role "professor".
        /// </summary>
        /// <returns>object with message</returns>
        /// <response code="200">Success</response>
        /// <response code="401">Unauthorized. Token not present, invalid or expired</response>
        [HttpPost]
        [Authorize(Roles = "professor")]
        public ActionResult<dynamic> CreateRoom([FromBody] Room model) {
            if (model.RoomName=="") return BadRequest(new {message = "nome da sala não pode ser vazio"});
            using (var dbContext = new Context()) {
                if (dbContext.Rooms.FirstOrDefault(u => u.RoomName == model.RoomName)!=null)
                    return BadRequest(new {message = "Já existe uma sala cadastrada com este nome"});
                
                if (dbContext.Users.FirstOrDefault(u => u.Name == model.Teacher && u.Role == "professor")==null)
                    return BadRequest(new {message = "Professor inválido."});
                
                Room add = new Room();
                add.RoomName = model.RoomName;
                add.Teacher = model.Teacher;
                add.Description = model.Description;
                add.Quiz_id = model.Quiz_id;

                dbContext.Rooms.Add(add);
                dbContext.SaveChanges();
            }
            return Ok(new {message = "room sucessfull created"});
        }

        /// <summary>
        /// Update classroom quiz. You must pass the user email and password by the body. ONLY the classroom author can do it!!!
        /// </summary>
        /// <param name="user"></param>
        /// <param name="roomid">Room id to be updated</param>
        /// <param name="quizid">Quiz id to put in the room</param>
        /// <returns>status messages</returns>
        /// <response code="200">Success</response>
        /// <response code="401">Unauthorized. Token not present, invalid or expired</response>
        [HttpPut("{roomid}/{quizid}")]
        [Authorize(Roles = "professor")]
        public ActionResult<dynamic> UpdateClassroomQuiz([FromBody] User user, long roomid, long quizid) {
            using (var dbContext = new Context()) {
                user = dbContext.Users.FirstOrDefault(u => u.Email == user.Email && u.Password == user.Password);
                if (user == null) return BadRequest(new {message = "Usuário inválido."});

                var room = dbContext.Rooms.FirstOrDefault(u => u.Id == roomid && u.Teacher == user.Name);
                if (room == null) return BadRequest(new {message = "There is no room associeted with requested data."});

                if (dbContext.QuizPosts.FirstOrDefault(u => u.Id == quizid)==null) return BadRequest(new {message = "Invalid quiz id"});
                room.Quiz_id = quizid;
                dbContext.Update(room);
                dbContext.SaveChanges();
            }
            return Ok(new {message = "Success"});
        }
        
        /// <summary>
        /// Get the room list.
        /// </summary>
        /// <returns>List with previously registred rooms</returns>
        /// <response code="200">Success</response>
        [HttpGet("getrooms")]
        [AllowAnonymous]
        public ActionResult<IList<Room>> getRooms() {
            using (var dbContext = new Context()) {
                return Ok(dbContext.Rooms.ToList());
            }
        }
    }
}