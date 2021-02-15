using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuizWebAPIREST.Models {
    public class ApiInfo {
        public string Title = "";
        public string Version = "";
    }

    [Table("gamedata")]
    public class GameData {
        [Column("id")]
        public long Id { get; set; }
        [Column("gameversion")]
        public string Version { get; set;}
        [Column("creation")]
        public DateTime CreationDate { get; set; }
    }

    [Table("user_table")]
    public class User {
        [Column("id")]
        public int Id { get; set; }
        [Column("useremail")]
        public string Email { get; set; }
        [Column("userpassword")]
        public string Password { get; set; }
        [Column("userrole")]
        public string Role { get; set; }
        [Column("username")]
        public string Name { get; set; }
        [Column("classroom")]
        public long Classroom { get; set; }
        [Column("score")]
        public int Score { get; set; }
        [Column("creation")]
        public DateTime CreationDate { get; set; }
        public User() {
            //usar datatime de API
            this.CreationDate = DateTime.Now;
        }
    }

    [Table("quiz_table")]
    public class PostQuiz {
        [Column("id")]
        public long Id { get; set; }
        [Column("author")]
        public string Author { get; set; }
        [Column("question")]
        public string Question { get; set; }
        [Column("answer")]
        public string Answer { get; set; }
        [Column("notanswer")]
        public string NotAnswer { get; set; }
        [Column("creation")]
        public DateTime CreationDate { get; set; }
        public PostQuiz() {
            //usar datatime de API
            this.CreationDate = DateTime.Now;
        }
    }

    [Table("room_table")]
    public class Room {
        [Column("id")]
        public long Id { get; set; }
        [Column("roomname")]
        public string RoomName { get; set; }
        [Column("description")]
        public string Description { get; set; }
        [Column("teacher")]
        public string Teacher { get; set; }
        [Column("quiz_id")]
        public long Quiz_id { get; set; }
        [Column("creation")]
        public DateTime CreationDate { get; set; }
        public Room() {
            //usar datatime de API
            this.CreationDate = DateTime.Now;
        }
    }
}