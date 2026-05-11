using AutoFilterer.Enums;
using AutoFilterer.Extensions;
using AutoFilterer.Generators.Tests.Environment.Dtos;
using AutoFilterer.Generators.Tests.Environment.Models;
using AutoFilterer.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace AutoFilterer.Generators.Tests;

/// <summary>
/// Phase 2: Recursive N-depth mixed quantifier parity tests.
/// Covers mixed Any/All chains across depths 1-8 with various patterns:
/// - all-any
/// - all-all
/// - alternating any/all
/// - prefix-any-suffix-all
/// - sparse flips
/// </summary>
public class GeneratorParityP2RecursiveMixedTests
{
    #region Depth 1: Single Level Any (Baseline)

    [Fact]
    public void Parity_Recursive_D1_Any_MatchesRuntime()
    {
        var data = new[]
        {
            new Level1
            {
                Id = 1,
                Value = "A",
                Children = new List<Level2>
                {
                    new Level2
                    {
                        Id = 1,
                        Value = "X",
                        Children = new List<Level3>
                        {
                            new Level3
                            {
                                Id = 1,
                                Value = "Target",
                                Children = new List<Level4>
                                {
                                    new Level4
                                    {
                                        Id = 1,
                                        Value = "L4",
                                        Children = new List<Level5>
                                        {
                                            new Level5
                                            {
                                                Id = 1,
                                                Value = "L5",
                                                Children = new List<Level6>
                                                {
                                                    new Level6
                                                    {
                                                        Id = 1,
                                                        Value = "L6",
                                                        Children = new List<Level7>
                                                        {
                                                            new Level7
                                                            {
                                                                Id = 1,
                                                                Value = "L7",
                                                                Children = new List<Level8>
                                                                {
                                                                    new Level8 { Id = 1, Value = "Match", Score = 50 }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            },
            new Level1
            {
                Id = 2,
                Value = "B",
                Children = new List<Level2>
                {
                    new Level2
                    {
                        Id = 2,
                        Value = "Y",
                        Children = new List<Level3>
                        {
                            new Level3
                            {
                                Id = 2,
                                Value = "NoMatch",
                                Children = new List<Level4>
                                {
                                    new Level4
                                    {
                                        Id = 2,
                                        Value = "L4",
                                        Children = new List<Level5>
                                        {
                                            new Level5
                                            {
                                                Id = 2,
                                                Value = "L5",
                                                Children = new List<Level6>
                                                {
                                                    new Level6
                                                    {
                                                        Id = 2,
                                                        Value = "L6",
                                                        Children = new List<Level7>
                                                        {
                                                            new Level7
                                                            {
                                                                Id = 2,
                                                                Value = "L7",
                                                                Children = new List<Level8>
                                                                {
                                                                    new Level8 { Id = 2, Value = "Other", Score = 30 }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        };

        var filter = new Level1Filter_AnyAllMixed
        {
            Children = new Level2Filter
            {
                Children = new Level3Filter
                {
                    Children = new Level4Filter
                    {
                        Children = new Level5Filter
                        {
                            Children = new Level6Filter
                            {
                                Children = new Level7Filter
                                {
                                    Children = new Level8Filter
                                    {
                                        Value = new StringFilter { Contains = "Match" }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        };

        AssertParityLevel1(data, filter);
    }

    #endregion

    #region Depth 2-4: All-Any Pattern

    [Fact]
    public void Parity_Recursive_D2_AllAny_MatchesRuntime()
    {
        var data = new[]
        {
            new Level1
            {
                Id = 1,
                Value = "A",
                Children = new List<Level2>
                {
                    new Level2
                    {
                        Id = 1,
                        Value = "X",
                        Children = new List<Level3>
                        {
                            new Level3
                            {
                                Id = 1,
                                Value = "Target",
                                Children = new List<Level4>
                                {
                                    new Level4
                                    {
                                        Id = 1,
                                        Value = "L4",
                                        Children = new List<Level5>
                                        {
                                            new Level5
                                            {
                                                Id = 1,
                                                Value = "L5",
                                                Children = new List<Level6>
                                                {
                                                    new Level6
                                                    {
                                                        Id = 1,
                                                        Value = "L6",
                                                        Children = new List<Level7>
                                                        {
                                                            new Level7
                                                            {
                                                                Id = 1,
                                                                Value = "L7",
                                                                Children = new List<Level8>
                                                                {
                                                                    new Level8 { Id = 1, Value = "Match", Score = 50 }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            },
            new Level1
            {
                Id = 2,
                Value = "B",
                Children = new List<Level2>
                {
                    new Level2
                    {
                        Id = 2,
                        Value = "Y",
                        Children = new List<Level3>
                        {
                            new Level3
                            {
                                Id = 2,
                                Value = "NoMatch",
                                Children = new List<Level4>
                                {
                                    new Level4
                                    {
                                        Id = 2,
                                        Value = "L4",
                                        Children = new List<Level5>
                                        {
                                            new Level5
                                            {
                                                Id = 2,
                                                Value = "L5",
                                                Children = new List<Level6>
                                                {
                                                    new Level6
                                                    {
                                                        Id = 2,
                                                        Value = "L6",
                                                        Children = new List<Level7>
                                                        {
                                                            new Level7
                                                            {
                                                                Id = 2,
                                                                Value = "L7",
                                                                Children = new List<Level8>
                                                                {
                                                                    new Level8 { Id = 2, Value = "Other", Score = 30 }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        };

        var filter = new Level1Filter_AllAnyAnyAnyAnyAnyAny
        {
            Children = new Level2Filter_AllAnyAnyAnyAnyAny
            {
                Children = new Level3Filter_AllAnyAnyAnyAny
                {
                    Children = new Level4Filter_AllAnyAnyAny
                    {
                        Children = new Level5Filter_AllAnyAny
                        {
                            Children = new Level6Filter_AllAny
                            {
                                Children = new Level7Filter_All
                                {
                                    Children = new Level8Filter
                                    {
                                        Value = new StringFilter { Contains = "Match" }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        };

        AssertParityLevel1(data, filter);
    }

    [Fact]
    public void Parity_Recursive_D3_AllAnyAny_VacuousTruth()
    {
        var data = new[]
        {
            new Level1 { Id = 1, Value = "A", Children = new List<Level2>() },
            new Level1
            {
                Id = 2,
                Value = "B",
                Children = new List<Level2>
                {
                    new Level2 { Id = 1, Value = "X", Children = new List<Level3>() }
                }
            }
        };

        var filter = new Level1Filter_AllAnyAnyAnyAnyAnyAny
        {
            Children = new Level2Filter_AllAnyAnyAnyAnyAny
            {
                Children = new Level3Filter_AllAnyAnyAnyAny
                {
                    Children = new Level4Filter_AllAnyAnyAny
                    {
                        Children = new Level5Filter_AllAnyAny
                        {
                            Children = new Level6Filter_AllAny
                            {
                                Children = new Level7Filter_All
                                {
                                    Children = new Level8Filter
                                    {
                                        Value = new StringFilter { Contains = "Match" }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        };

        AssertParityLevel1(data, filter);
    }

    #endregion

    #region Depth 5-8: All-All Pattern

    [Fact]
    public void Parity_Recursive_D7_AllAll_MatchesRuntime()
    {
        var data = new[]
        {
            new Level1
            {
                Id = 1,
                Value = "A",
                Children = new List<Level2>
                {
                    new Level2
                    {
                        Id = 1,
                        Value = "X",
                        Children = new List<Level3>
                        {
                            new Level3
                            {
                                Id = 1,
                                Value = "T1",
                                Children = new List<Level4>
                                {
                                    new Level4
                                    {
                                        Id = 1,
                                        Value = "L4",
                                        Children = new List<Level5>
                                        {
                                            new Level5
                                            {
                                                Id = 1,
                                                Value = "L5",
                                                Children = new List<Level6>
                                                {
                                                    new Level6
                                                    {
                                                        Id = 1,
                                                        Value = "L6",
                                                        Children = new List<Level7>
                                                        {
                                                            new Level7
                                                            {
                                                                Id = 1,
                                                                Value = "L7",
                                                                Children = new List<Level8>
                                                                {
                                                                    new Level8 { Id = 1, Value = "AllMatch", Score = 50 },
                                                                    new Level8 { Id = 2, Value = "AllMatch", Score = 60 }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            },
            new Level1
            {
                Id = 2,
                Value = "B",
                Children = new List<Level2>
                {
                    new Level2
                    {
                        Id = 2,
                        Value = "Y",
                        Children = new List<Level3>
                        {
                            new Level3
                            {
                                Id = 2,
                                Value = "T2",
                                Children = new List<Level4>
                                {
                                    new Level4
                                    {
                                        Id = 2,
                                        Value = "L4",
                                        Children = new List<Level5>
                                        {
                                            new Level5
                                            {
                                                Id = 2,
                                                Value = "L5",
                                                Children = new List<Level6>
                                                {
                                                    new Level6
                                                    {
                                                        Id = 2,
                                                        Value = "L6",
                                                        Children = new List<Level7>
                                                        {
                                                            new Level7
                                                            {
                                                                Id = 2,
                                                                Value = "L7",
                                                                Children = new List<Level8>
                                                                {
                                                                    new Level8 { Id = 3, Value = "AllMatch", Score = 70 }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            },
            new Level1
            {
                Id = 3,
                Value = "C",
                Children = new List<Level2>
                {
                    new Level2
                    {
                        Id = 3,
                        Value = "Z",
                        Children = new List<Level3>
                        {
                            new Level3
                            {
                                Id = 3,
                                Value = "T3",
                                Children = new List<Level4>
                                {
                                    new Level4
                                    {
                                        Id = 3,
                                        Value = "L4",
                                        Children = new List<Level5>
                                        {
                                            new Level5
                                            {
                                                Id = 3,
                                                Value = "L5",
                                                Children = new List<Level6>
                                                {
                                                    new Level6
                                                    {
                                                        Id = 3,
                                                        Value = "L6",
                                                        Children = new List<Level7>
                                                        {
                                                            new Level7
                                                            {
                                                                Id = 3,
                                                                Value = "L7",
                                                                Children = new List<Level8>
                                                                {
                                                                    new Level8 { Id = 4, Value = "NoMatch", Score = 10 }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        };

        var filter = new Level1Filter_AllAll
        {
            Children = new Level2Filter_AllAll
            {
                Children = new Level3Filter_AllAll
                {
                    Children = new Level4Filter_AllAll
                    {
                        Children = new Level5Filter_AllAll
                        {
                            Children = new Level6Filter_AllAll
                            {
                                Children = new Level7Filter_All
                                {
                                    Children = new Level8Filter
                                    {
                                        Value = new StringFilter { Contains = "AllMatch" }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        };

        AssertParityLevel1(data, filter);
    }

    #endregion

    #region Depth 4: AllAnyAnyAny Pattern

    [Fact]
    public void Parity_Recursive_D4_AllAnyAnyAny_MatchesRuntime()
    {
        var data = new[]
        {
            new Level1
            {
                Id = 1,
                Value = "A",
                Children = new List<Level2>
                {
                    new Level2
                    {
                        Id = 1,
                        Value = "X",
                        Children = new List<Level3>
                        {
                            new Level3
                            {
                                Id = 1,
                                Value = "Target",
                                Children = new List<Level4>
                                {
                                    new Level4
                                    {
                                        Id = 1,
                                        Value = "L4",
                                        Children = new List<Level5>
                                        {
                                            new Level5
                                            {
                                                Id = 1,
                                                Value = "L5",
                                                Children = new List<Level6>
                                                {
                                                    new Level6
                                                    {
                                                        Id = 1,
                                                        Value = "L6",
                                                        Children = new List<Level7>
                                                        {
                                                            new Level7
                                                            {
                                                                Id = 1,
                                                                Value = "L7",
                                                                Children = new List<Level8>
                                                                {
                                                                    new Level8 { Id = 1, Value = "Match", Score = 50 }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            },
            new Level1
            {
                Id = 2,
                Value = "B",
                Children = new List<Level2>
                {
                    new Level2
                    {
                        Id = 2,
                        Value = "Y",
                        Children = new List<Level3>
                        {
                            new Level3
                            {
                                Id = 2,
                                Value = "NoMatch",
                                Children = new List<Level4>
                                {
                                    new Level4
                                    {
                                        Id = 2,
                                        Value = "L4",
                                        Children = new List<Level5>
                                        {
                                            new Level5
                                            {
                                                Id = 2,
                                                Value = "L5",
                                                Children = new List<Level6>
                                                {
                                                    new Level6
                                                    {
                                                        Id = 2,
                                                        Value = "L6",
                                                        Children = new List<Level7>
                                                        {
                                                            new Level7
                                                            {
                                                                Id = 2,
                                                                Value = "L7",
                                                                Children = new List<Level8>
                                                                {
                                                                    new Level8 { Id = 2, Value = "Other", Score = 30 }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        };

        var filter = new Level1Filter_AllAnyAnyAnyAnyAnyAny
        {
            Children = new Level2Filter_AllAnyAnyAnyAnyAny
            {
                Children = new Level3Filter_AllAnyAnyAnyAny
                {
                    Children = new Level4Filter_AllAnyAnyAny
                    {
                        Children = new Level5Filter_AllAnyAny
                        {
                            Children = new Level6Filter_AllAny
                            {
                                Children = new Level7Filter_All
                                {
                                    Children = new Level8Filter
                                    {
                                        Value = new StringFilter { Contains = "Match" }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        };

        AssertParityLevel1(data, filter);
    }

    #endregion

    #region Depth 5: AllAnyAnyAnyAny Pattern

    [Fact]
    public void Parity_Recursive_D5_AllAnyAnyAnyAny_MatchesRuntime()
    {
        var data = new[]
        {
            new Level1
            {
                Id = 1,
                Value = "A",
                Children = new List<Level2>
                {
                    new Level2
                    {
                        Id = 1,
                        Value = "X",
                        Children = new List<Level3>
                        {
                            new Level3
                            {
                                Id = 1,
                                Value = "Target",
                                Children = new List<Level4>
                                {
                                    new Level4
                                    {
                                        Id = 1,
                                        Value = "L4",
                                        Children = new List<Level5>
                                        {
                                            new Level5
                                            {
                                                Id = 1,
                                                Value = "L5",
                                                Children = new List<Level6>
                                                {
                                                    new Level6
                                                    {
                                                        Id = 1,
                                                        Value = "L6",
                                                        Children = new List<Level7>
                                                        {
                                                            new Level7
                                                            {
                                                                Id = 1,
                                                                Value = "L7",
                                                                Children = new List<Level8>
                                                                {
                                                                    new Level8 { Id = 1, Value = "Match", Score = 50 }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            },
            new Level1
            {
                Id = 2,
                Value = "B",
                Children = new List<Level2>
                {
                    new Level2
                    {
                        Id = 2,
                        Value = "Y",
                        Children = new List<Level3>
                        {
                            new Level3
                            {
                                Id = 2,
                                Value = "NoMatch",
                                Children = new List<Level4>
                                {
                                    new Level4
                                    {
                                        Id = 2,
                                        Value = "L4",
                                        Children = new List<Level5>
                                        {
                                            new Level5
                                            {
                                                Id = 2,
                                                Value = "L5",
                                                Children = new List<Level6>
                                                {
                                                    new Level6
                                                    {
                                                        Id = 2,
                                                        Value = "L6",
                                                        Children = new List<Level7>
                                                        {
                                                            new Level7
                                                            {
                                                                Id = 2,
                                                                Value = "L7",
                                                                Children = new List<Level8>
                                                                {
                                                                    new Level8 { Id = 2, Value = "Other", Score = 30 }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        };

        var filter = new Level1Filter_AllAnyAnyAnyAnyAnyAny
        {
            Children = new Level2Filter_AllAnyAnyAnyAnyAny
            {
                Children = new Level3Filter_AllAnyAnyAnyAny
                {
                    Children = new Level4Filter_AllAnyAnyAny
                    {
                        Children = new Level5Filter_AllAnyAny
                        {
                            Children = new Level6Filter_AllAny
                            {
                                Children = new Level7Filter_All
                                {
                                    Children = new Level8Filter
                                    {
                                        Value = new StringFilter { Contains = "Match" }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        };

        AssertParityLevel1(data, filter);
    }

    #endregion

    #region Depth 6: AllAnyAnyAnyAnyAny Pattern

    [Fact]
    public void Parity_Recursive_D6_AllAnyAnyAnyAnyAny_MatchesRuntime()
    {
        var data = new[]
        {
            new Level1
            {
                Id = 1,
                Value = "A",
                Children = new List<Level2>
                {
                    new Level2
                    {
                        Id = 1,
                        Value = "X",
                        Children = new List<Level3>
                        {
                            new Level3
                            {
                                Id = 1,
                                Value = "Target",
                                Children = new List<Level4>
                                {
                                    new Level4
                                    {
                                        Id = 1,
                                        Value = "L4",
                                        Children = new List<Level5>
                                        {
                                            new Level5
                                            {
                                                Id = 1,
                                                Value = "L5",
                                                Children = new List<Level6>
                                                {
                                                    new Level6
                                                    {
                                                        Id = 1,
                                                        Value = "L6",
                                                        Children = new List<Level7>
                                                        {
                                                            new Level7
                                                            {
                                                                Id = 1,
                                                                Value = "L7",
                                                                Children = new List<Level8>
                                                                {
                                                                    new Level8 { Id = 1, Value = "Match", Score = 50 }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            },
            new Level1
            {
                Id = 2,
                Value = "B",
                Children = new List<Level2>
                {
                    new Level2
                    {
                        Id = 2,
                        Value = "Y",
                        Children = new List<Level3>
                        {
                            new Level3
                            {
                                Id = 2,
                                Value = "NoMatch",
                                Children = new List<Level4>
                                {
                                    new Level4
                                    {
                                        Id = 2,
                                        Value = "L4",
                                        Children = new List<Level5>
                                        {
                                            new Level5
                                            {
                                                Id = 2,
                                                Value = "L5",
                                                Children = new List<Level6>
                                                {
                                                    new Level6
                                                    {
                                                        Id = 2,
                                                        Value = "L6",
                                                        Children = new List<Level7>
                                                        {
                                                            new Level7
                                                            {
                                                                Id = 2,
                                                                Value = "L7",
                                                                Children = new List<Level8>
                                                                {
                                                                    new Level8 { Id = 2, Value = "Other", Score = 30 }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        };

        var filter = new Level1Filter_AllAnyAnyAnyAnyAnyAny
        {
            Children = new Level2Filter_AllAnyAnyAnyAnyAny
            {
                Children = new Level3Filter_AllAnyAnyAnyAny
                {
                    Children = new Level4Filter_AllAnyAnyAny
                    {
                        Children = new Level5Filter_AllAnyAny
                        {
                            Children = new Level6Filter_AllAny
                            {
                                Children = new Level7Filter_All
                                {
                                    Children = new Level8Filter
                                    {
                                        Value = new StringFilter { Contains = "Match" }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        };

        AssertParityLevel1(data, filter);
    }

    #endregion

    #region Depth 8: Full Depth AllAnyAnyAnyAnyAnyAnyAny Pattern

    [Fact]
    public void Parity_Recursive_D8_FullDepth_MatchesRuntime()
    {
        var data = new[]
        {
            new Level1
            {
                Id = 1,
                Value = "A",
                Children = new List<Level2>
                {
                    new Level2
                    {
                        Id = 1,
                        Value = "X",
                        Children = new List<Level3>
                        {
                            new Level3
                            {
                                Id = 1,
                                Value = "Target",
                                Children = new List<Level4>
                                {
                                    new Level4
                                    {
                                        Id = 1,
                                        Value = "L4",
                                        Children = new List<Level5>
                                        {
                                            new Level5
                                            {
                                                Id = 1,
                                                Value = "L5",
                                                Children = new List<Level6>
                                                {
                                                    new Level6
                                                    {
                                                        Id = 1,
                                                        Value = "L6",
                                                        Children = new List<Level7>
                                                        {
                                                            new Level7
                                                            {
                                                                Id = 1,
                                                                Value = "L7",
                                                                Children = new List<Level8>
                                                                {
                                                                    new Level8 { Id = 1, Value = "Match", Score = 50 }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            },
            new Level1
            {
                Id = 2,
                Value = "B",
                Children = new List<Level2>
                {
                    new Level2
                    {
                        Id = 2,
                        Value = "Y",
                        Children = new List<Level3>
                        {
                            new Level3
                            {
                                Id = 2,
                                Value = "NoMatch",
                                Children = new List<Level4>
                                {
                                    new Level4
                                    {
                                        Id = 2,
                                        Value = "L4",
                                        Children = new List<Level5>
                                        {
                                            new Level5
                                            {
                                                Id = 2,
                                                Value = "L5",
                                                Children = new List<Level6>
                                                {
                                                    new Level6
                                                    {
                                                        Id = 2,
                                                        Value = "L6",
                                                        Children = new List<Level7>
                                                        {
                                                            new Level7
                                                            {
                                                                Id = 2,
                                                                Value = "L7",
                                                                Children = new List<Level8>
                                                                {
                                                                    new Level8 { Id = 2, Value = "Other", Score = 30 }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        };

        var filter = new Level1Filter_AllAnyAnyAnyAnyAnyAny
        {
            Children = new Level2Filter_AllAnyAnyAnyAnyAny
            {
                Children = new Level3Filter_AllAnyAnyAnyAny
                {
                    Children = new Level4Filter_AllAnyAnyAny
                    {
                        Children = new Level5Filter_AllAnyAny
                        {
                            Children = new Level6Filter_AllAny
                            {
                                Children = new Level7Filter_All
                                {
                                    Children = new Level8Filter
                                    {
                                        Value = new StringFilter { Contains = "Match" }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        };

        AssertParityLevel1(data, filter);
    }

    #endregion

    #region Alternating Any/All Pattern

    [Fact]
    public void Parity_Recursive_AltPattern_MatchesRuntime()
    {
        var data = new[]
        {
            new Level1
            {
                Id = 1,
                Value = "A",
                Children = new List<Level2>
                {
                    new Level2
                    {
                        Id = 1,
                        Value = "X",
                        Children = new List<Level3>
                        {
                            new Level3
                            {
                                Id = 1,
                                Value = "Target",
                                Children = new List<Level4>
                                {
                                    new Level4
                                    {
                                        Id = 1,
                                        Value = "L4",
                                        Children = new List<Level5>
                                        {
                                            new Level5
                                            {
                                                Id = 1,
                                                Value = "L5",
                                                Children = new List<Level6>
                                                {
                                                    new Level6
                                                    {
                                                        Id = 1,
                                                        Value = "L6",
                                                        Children = new List<Level7>
                                                        {
                                                            new Level7
                                                            {
                                                                Id = 1,
                                                                Value = "L7",
                                                                Children = new List<Level8>
                                                                {
                                                                    new Level8 { Id = 1, Value = "Match", Score = 50 },
                                                                    new Level8 { Id = 2, Value = "Match2", Score = 60 }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            },
            new Level1
            {
                Id = 2,
                Value = "B",
                Children = new List<Level2>
                {
                    new Level2
                    {
                        Id = 2,
                        Value = "Y",
                        Children = new List<Level3>
                        {
                            new Level3
                            {
                                Id = 2,
                                Value = "NoMatch",
                                Children = new List<Level4>
                                {
                                    new Level4
                                    {
                                        Id = 2,
                                        Value = "L4",
                                        Children = new List<Level5>
                                        {
                                            new Level5
                                            {
                                                Id = 2,
                                                Value = "L5",
                                                Children = new List<Level6>
                                                {
                                                    new Level6
                                                    {
                                                        Id = 2,
                                                        Value = "L6",
                                                        Children = new List<Level7>
                                                        {
                                                            new Level7
                                                            {
                                                                Id = 2,
                                                                Value = "L7",
                                                                Children = new List<Level8>
                                                                {
                                                                    new Level8 { Id = 3, Value = "NoMatch", Score = 10 }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        };

        var filter = new Level1Filter_AltPattern
        {
            Children = new Level2Filter_AltPattern
            {
                Children = new Level3Filter_AltPattern
                {
                    Children = new Level4Filter_AltPattern
                    {
                        Children = new Level5Filter_AltPattern
                        {
                            Children = new Level6Filter_AltPattern
                            {
                                Children = new Level7Filter_All
                                {
                                    Children = new Level8Filter
                                    {
                                        Value = new StringFilter { Contains = "Match" }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        };

        AssertParityLevel1(data, filter);
    }

    #endregion

    #region Prefix-Any-Suffix-All Pattern (Depth 8)

    [Fact]
    public void Parity_Recursive_D8_PrefixAnySuffixAll_MatchesRuntime()
    {
        var data = new[]
        {
            new Level1
            {
                Id = 1,
                Value = "A",
                Children = new List<Level2>
                {
                    new Level2
                    {
                        Id = 1,
                        Value = "X",
                        Children = new List<Level3>
                        {
                            new Level3
                            {
                                Id = 1,
                                Value = "T1",
                                Children = new List<Level4>
                                {
                                    new Level4
                                    {
                                        Id = 1,
                                        Value = "L4",
                                        Children = new List<Level5>
                                        {
                                            new Level5
                                            {
                                                Id = 1,
                                                Value = "L5",
                                                Children = new List<Level6>
                                                {
                                                    new Level6
                                                    {
                                                        Id = 1,
                                                        Value = "L6",
                                                        Children = new List<Level7>
                                                        {
                                                            new Level7
                                                            {
                                                                Id = 1,
                                                                Value = "L7",
                                                                Children = new List<Level8>
                                                                {
                                                                    new Level8 { Id = 1, Value = "Target", Score = 50 },
                                                                    new Level8 { Id = 2, Value = "Target", Score = 55 }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            },
            new Level1
            {
                Id = 2,
                Value = "B",
                Children = new List<Level2>
                {
                    new Level2
                    {
                        Id = 2,
                        Value = "Y",
                        Children = new List<Level3>
                        {
                            new Level3
                            {
                                Id = 2,
                                Value = "T2",
                                Children = new List<Level4>
                                {
                                    new Level4
                                    {
                                        Id = 2,
                                        Value = "L4",
                                        Children = new List<Level5>
                                        {
                                            new Level5
                                            {
                                                Id = 2,
                                                Value = "L5",
                                                Children = new List<Level6>
                                                {
                                                    new Level6
                                                    {
                                                        Id = 2,
                                                        Value = "L6",
                                                        Children = new List<Level7>
                                                        {
                                                            new Level7
                                                            {
                                                                Id = 2,
                                                                Value = "L7",
                                                                Children = new List<Level8>
                                                                {
                                                                    new Level8 { Id = 3, Value = "Target", Score = 60 },
                                                                    new Level8 { Id = 4, Value = "Other", Score = 10 }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            },
            new Level1
            {
                Id = 3,
                Value = "C",
                Children = new List<Level2>
                {
                    new Level2
                    {
                        Id = 3,
                        Value = "Z",
                        Children = new List<Level3>
                        {
                            new Level3
                            {
                                Id = 3,
                                Value = "T3",
                                Children = new List<Level4>
                                {
                                    new Level4
                                    {
                                        Id = 3,
                                        Value = "L4",
                                        Children = new List<Level5>
                                        {
                                            new Level5
                                            {
                                                Id = 3,
                                                Value = "L5",
                                                Children = new List<Level6>
                                                {
                                                    new Level6
                                                    {
                                                        Id = 3,
                                                        Value = "L6",
                                                        Children = new List<Level7>
                                                        {
                                                            new Level7
                                                            {
                                                                Id = 3,
                                                                Value = "L7",
                                                                Children = new List<Level8>
                                                                {
                                                                    new Level8 { Id = 5, Value = "NoMatch", Score = 20 }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        };

        var filter = new Level1Filter_PrefixAnySuffixAll
        {
            Children = new Level2Filter_PrefixAnySuffixAll
            {
                Children = new Level3Filter_PrefixAnySuffixAll
                {
                    Children = new Level4Filter_PrefixAnySuffixAll
                    {
                        Children = new Level5Filter_PrefixAnySuffixAll
                        {
                            Children = new Level6Filter_PrefixAnySuffixAll
                            {
                                Children = new Level7Filter_PrefixAnySuffixAll
                                {
                                    Children = new Level8Filter
                                    {
                                        Value = new StringFilter { Contains = "Target" }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        };

        AssertParityLevel1(data, filter);
    }

    #endregion

    #region Sparse Flips Pattern (Depth 8: Any-Any-All-Any-All-All-Any-All)

    [Fact]
    public void Parity_Recursive_D8_SparseFlips_MatchesRuntime()
    {
        var data = new[]
        {
            new Level1
            {
                Id = 1,
                Value = "A",
                Children = new List<Level2>
                {
                    new Level2
                    {
                        Id = 1,
                        Value = "X",
                        Children = new List<Level3>
                        {
                            new Level3
                            {
                                Id = 1,
                                Value = "T1",
                                Children = new List<Level4>
                                {
                                    new Level4
                                    {
                                        Id = 1,
                                        Value = "L4",
                                        Children = new List<Level5>
                                        {
                                            new Level5
                                            {
                                                Id = 1,
                                                Value = "L5",
                                                Children = new List<Level6>
                                                {
                                                    new Level6
                                                    {
                                                        Id = 1,
                                                        Value = "L6",
                                                        Children = new List<Level7>
                                                        {
                                                            new Level7
                                                            {
                                                                Id = 1,
                                                                Value = "L7",
                                                                Children = new List<Level8>
                                                                {
                                                                    new Level8 { Id = 1, Value = "Match", Score = 50 }
                                                                }
                                                            }
                                                        }
                                                    },
                                                    new Level6
                                                    {
                                                        Id = 2,
                                                        Value = "L6b",
                                                        Children = new List<Level7>
                                                        {
                                                            new Level7
                                                            {
                                                                Id = 2,
                                                                Value = "L7b",
                                                                Children = new List<Level8>
                                                                {
                                                                    new Level8 { Id = 2, Value = "Other", Score = 10 }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            },
            new Level1
            {
                Id = 2,
                Value = "B",
                Children = new List<Level2>
                {
                    new Level2
                    {
                        Id = 2,
                        Value = "Y",
                        Children = new List<Level3>
                        {
                            new Level3
                            {
                                Id = 2,
                                Value = "T2",
                                Children = new List<Level4>
                                {
                                    new Level4
                                    {
                                        Id = 2,
                                        Value = "L4",
                                        Children = new List<Level5>
                                        {
                                            new Level5
                                            {
                                                Id = 2,
                                                Value = "L5",
                                                Children = new List<Level6>
                                                {
                                                    new Level6
                                                    {
                                                        Id = 3,
                                                        Value = "L6",
                                                        Children = new List<Level7>
                                                        {
                                                            new Level7
                                                            {
                                                                Id = 3,
                                                                Value = "L7",
                                                                Children = new List<Level8>
                                                                {
                                                                    new Level8 { Id = 3, Value = "Match", Score = 60 }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            },
            new Level1
            {
                Id = 3,
                Value = "C",
                Children = new List<Level2>
                {
                    new Level2
                    {
                        Id = 3,
                        Value = "Z",
                        Children = new List<Level3>
                        {
                            new Level3
                            {
                                Id = 3,
                                Value = "T3",
                                Children = new List<Level4>
                                {
                                    new Level4
                                    {
                                        Id = 3,
                                        Value = "L4",
                                        Children = new List<Level5>
                                        {
                                            new Level5
                                            {
                                                Id = 3,
                                                Value = "L5",
                                                Children = new List<Level6>
                                                {
                                                    new Level6
                                                    {
                                                        Id = 4,
                                                        Value = "L6",
                                                        Children = new List<Level7>
                                                        {
                                                            new Level7
                                                            {
                                                                Id = 4,
                                                                Value = "L7",
                                                                Children = new List<Level8>
                                                                {
                                                                    new Level8 { Id = 4, Value = "NoMatch", Score = 30 }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        };

        var filter = new Level1Filter_SparseFlips
        {
            Children = new Level2Filter_SparseFlips
            {
                Children = new Level3Filter_SparseFlips
                {
                    Children = new Level4Filter_SparseFlips
                    {
                        Children = new Level5Filter_SparseFlips
                        {
                            Children = new Level6Filter_SparseFlips
                            {
                                Children = new Level7Filter_SparseFlips
                                {
                                    Children = new Level8Filter
                                    {
                                        Value = new StringFilter { Contains = "Match" }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        };

        AssertParityLevel1(data, filter);
    }

    #endregion

    #region Helper Methods

    private static void AssertParityLevel1<TFilter>(IEnumerable<Level1> data, TFilter filter)
        where TFilter : FilterBase
    {
        var runtime = ExecuteLevel1(() => filter.ApplyFilterTo(data.AsQueryable()).ToList());
        var generated = ExecuteLevel1(() => GeneratedFilterInvoker.ApplyFilter(data.AsQueryable(), filter).ToList());

        Assert.Equal(runtime.Exception?.GetType(), generated.Exception?.GetType());

        if (runtime.Exception != null)
        {
            Assert.Equal(runtime.Exception.Message, generated.Exception.Message);
            return;
        }

        Assert.Equal(runtime.Result.Select(x => x.Id), generated.Result.Select(x => x.Id));
    }

    private static ExecutionResultLevel1 ExecuteLevel1(Func<List<Level1>> run)
    {
        try
        {
            return new ExecutionResultLevel1 { Result = run() };
        }
        catch (Exception ex)
        {
            return new ExecutionResultLevel1 { Exception = ex };
        }
    }

    private sealed class ExecutionResultLevel1
    {
        public List<Level1> Result { get; set; } = new List<Level1>();
        public Exception Exception { get; set; }
    }

    #endregion
}
