using System;
using System.Collections.Generic;

namespace Farm.Models
{
    public class AnimalViewModel
    {
        public Animal Animal { get; set; }  
        public List<Cage> CageList { get; set; } 
        public List<Category> CategoryList { get; set; }  
    }
}