using AutoFilterer.Types;

namespace AutoFilterer;

public static class AutoFiltererConsts
{
    public static bool IgnoreExceptions
    {
        set
        {
            FilterBase.IgnoreExceptions = value;
        }
    }
}
