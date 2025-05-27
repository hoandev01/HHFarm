using System;
using System.Collections.Generic;

namespace Farm.Models
{
    public class CageViewModel
    {
        public Cage Cage { get; set; }
        public List<User> UserList { get; set; }
        public List<Category> CategoryList { get; set; }
    }
}