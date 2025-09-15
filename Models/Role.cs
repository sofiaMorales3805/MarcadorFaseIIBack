﻿
namespace MarcadorFaseIIApi.Models;

public class Role
{
        public int Id { get; set; }
        public required string Name { get; set; } = string.Empty;
        public ICollection<User> Users { get; set; } = new List<User>();
}
