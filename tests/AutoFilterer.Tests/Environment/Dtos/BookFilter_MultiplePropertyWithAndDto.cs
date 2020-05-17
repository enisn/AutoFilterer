using AutoFilterer.Attributes;
using AutoFilterer.Tests.Environment.Models;
using AutoFilterer.Types;

namespace AutoFilterer.Tests.Environment.Dtos
{
    public class BookFilter_MultiplePropertyWithAndDto : FilterBase
    {
        [CompareTo(nameof(Book.Title), nameof(Book.Author), CombineWith = Enums.CombineType.And)]
        [StringFilterOptions(Enums.StringFilterOption.Contains)]
        public string Query { get; set; }
    }
}
