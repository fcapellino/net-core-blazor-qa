using System.Collections.Generic;

namespace BlazorAppQA.Infrastructure.Common
{
    public sealed class ListResult
    {
        public IEnumerable<object> ItemsList { get; set; }
        public int TotalItemCount { get; set; }

        public ListResult()
        {
        }
        public ListResult(IEnumerable<object> itemsList, int totalItemCount)
            : this()
        {
            ItemsList = itemsList;
            TotalItemCount = totalItemCount;
        }
    }
}
