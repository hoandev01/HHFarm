using System.Collections.Generic;

namespace Farm.Models
{
    public class UserListViewModel
    {
        public string Name { get; set; }
        public List<User> UserList { get; set; }
        public List<News> NewsList { get; set; }
        public List<Category> CategoryList { get; set; }
        public List<Cage> CageList { get; set; }
        public List<Animal> AnimalList { get; set; }
        public List<Therapy> TherapyList { get; set; }
        public List<Deal> DealList { get; set; }
        public List<Contact> ContactList { get; set; }
    }
}