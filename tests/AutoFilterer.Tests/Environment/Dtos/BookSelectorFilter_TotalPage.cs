using AutoFilterer.Types;

namespace AutoFilterer.Tests.Environment.Dtos
{
    public class BookSelectorFilter_TotalPage : SelectorFilterBase<BookOutputDto>
    {
        public int? TotalPage { get; set; }
    }
}
