using System.Collections.Generic;
using MSC.API.Helpers;

namespace MSC.API.Dto
{
    public class CreatePostRequest
    {
        public string Html {get;set;}
        public PostStatus Status {get;set;}
        public IEnumerable<string> Tags {get;set;}
    }
}