using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dtos
{
    public class CreateArticleDto
    {
        public string Title { get; set; }
        public string SubHeading { get; set; }
        public string Content { get; set; }
        public int UserId { get; set; }
    }
}
