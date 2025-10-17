using System;
using System.Collections.Generic;

namespace Data.Entities
{
    public class Comment
    {
        public Guid Id { get; set; }
        
        public string Name { get; set; } = null!;
        
        public string Body { get; set; } = null!;
        
        public Guid? ParentCommentId { get; set; }
        
        public Guid GameId { get; set; }
        
        public Comment? ParentComment { get; set; }
        public ICollection<Comment> ChildComments { get; set; } = new List<Comment>();
    }
}