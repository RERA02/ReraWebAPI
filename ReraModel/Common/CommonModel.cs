using System.ComponentModel.DataAnnotations;

namespace ReraModel.CommonModel
{

  

    public class CommonModel
    {
        public string? Action { get; set; }
        public int Id { get; set; }
        public int isProfileOrProject { get; set; }
        public string? pnrno { get; set; }
        public string? Type { get; set; }


    }

    public class AgentDocumentModel
    {
        public int UserId { get; set; } 
        public string? ProfileType { get; set; }

    }

    public class QueryRequest
    {
        public string Query { get; set; }
    }

    public class SearchSPRequest
    {
        public string SearchText { get; set; }
    }
}
