using AutoFilterer.Attributes;
using AutoFilterer.Types;
using System;

namespace AutoFilterer.Tests.Environment.Dtos;

public class PreferencesFilter_ArraySearchWithoutAttribute_Guid : FilterBase
{
    public Guid[] OrganizationUnitId { get; set; }
}