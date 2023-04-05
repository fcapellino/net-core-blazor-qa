namespace BlazorAppQA.Infrastructure.CommandHandlers.GetQuestionsListHandler
{
    public class GetQuestionsListCommand
    {
        public string SearchQuery { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public string ProtectedCategoryId { get; set; }
    }
}
