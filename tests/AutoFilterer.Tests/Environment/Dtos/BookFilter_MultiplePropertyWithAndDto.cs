#if LEGACY_NAMESPACE
using AutoFilterer.Enums;
#else
using AutoFilterer;
#endif
using AutoFilterer.Attributes;
using AutoFilterer.Tests.Environment.Models;
using AutoFilterer.Types;

namespace AutoFilterer.Tests.Environment.Dtos;

public class BookFilter_MultiplePropertyWithAndDto : FilterBase
{
    [CompareTo(nameof(Book.Title), nameof(Book.Author), CombineWith = CombineType.And)]
    [StringFilterOptions(StringFilterOption.Contains)]
    public string Query { get; set; }
}
