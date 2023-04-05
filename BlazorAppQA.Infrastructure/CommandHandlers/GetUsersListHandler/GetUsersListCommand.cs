namespace BlazorAppQA.Infrastructure.CommandHandlers.GetUsersListHandler
{
    public class GetUsersListCommand
    {
        public string SearchQuery { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}
