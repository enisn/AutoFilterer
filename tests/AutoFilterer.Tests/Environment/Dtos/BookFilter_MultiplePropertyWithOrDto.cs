#if LEGACY_NAMESPACE
using AutoFilterer.Enums;
#endif
using AutoFilterer.Attributes;
using AutoFilterer.Tests.Environment.Models;
using AutoFilterer.Types;

namespace AutoFilterer.Tests.Environment.Dtos;

public class BookFilter_MultiplePropertyWithOrDto : FilterBase
{
    [CompareTo(nameof(Book.Title), nameof(Book.Author), CombineWith = CombineType.Or)]
    [StringFilterOptions(StringFilterOption.Contains)]
    public string Query { get; set; }
}
