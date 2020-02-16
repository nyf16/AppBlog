﻿using System;


namespace BlogApp.Models
{
    public class Post
    {
        public int Id { get; set; }

        public string Title { get; set; } = "";
        public string Body { get; set; } = "";
        public string Picture { get; set; } = "";

        public DateTime Created { get; set; } = DateTime.Now;
    }
}
