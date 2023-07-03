﻿namespace Talkie.Models
{
    public class Text
    {
        public int Id { get; set; }
        public int MessageId { get; set; }
        public string Content { get; set; } = string.Empty;

        public Message? Message { get; set; }
    }
}