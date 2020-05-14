using AutoFilterer.Attributes;
using AutoFilterer.Tests.Envirorment.Models;
using AutoFilterer.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoFilterer.Tests.Envirorment.Dtos
{
    public class BookFilter_MultiplePropertyWithOrDto : FilterBase
    {
        [CompareTo(nameof(Book.Title), nameof(Book.Author), CombineWith = Enums.CombineType.Or)]
        [StringFilterOptions(Enums.StringFilterOption.Contains)]
        public string Query { get; set; }
    }
}
