using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserService.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public String UserName { get; set; }
        public String Email { get; set; }
        public Car Car { get; set; }
        public Tag[] Preferences { get; set; }
        public String AvatarURL { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime DeletedAt { get; set; }

        public User()
        {

        }

        public User(Guid id, string userName, Car car, Tag[] preferences, string avatarURL, DateTime createdAt, DateTime updatedAt, DateTime deletedAt)
        {
            Id = id;
            UserName = userName;
            Car = car;
            Preferences = preferences;
            AvatarURL = avatarURL;
            CreatedAt = createdAt;
            UpdatedAt = updatedAt;
            DeletedAt = deletedAt;
        }
    }
}
