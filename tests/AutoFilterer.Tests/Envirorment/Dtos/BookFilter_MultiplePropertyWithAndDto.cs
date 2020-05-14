using AutoFilterer.Attributes;
using AutoFilterer.Tests.Envirorment.Models;
using AutoFilterer.Types;

namespace AutoFilterer.Tests.Envirorment.Dtos
{
    public class BookFilter_MultiplePropertyWithAndDto : FilterBase
    {
        [CompareTo(nameof(Book.Title), nameof(Book.Author), CombineWith = Enums.CombineType.And)]
        [StringFilterOptions(Enums.StringFilterOption.Contains)]
        public string Query { get; set; }
    }
}
