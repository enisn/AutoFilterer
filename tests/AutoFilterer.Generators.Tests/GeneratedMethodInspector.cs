using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace AutoFilterer.Generators.Tests;

internal static class GeneratedMethodInspector
{
    private static readonly OpCode[] oneByteOpCodes = new OpCode[0x100];
    private static readonly OpCode[] twoByteOpCodes = new OpCode[0x100];

    static GeneratedMethodInspector()
    {
        foreach (var field in typeof(OpCodes).GetFields(BindingFlags.Public | BindingFlags.Static))
        {
            if (field.GetValue(null) is not OpCode opCode)
            {
                continue;
            }

            var value = unchecked((ushort)opCode.Value);
            if (value < 0x100)
            {
                oneByteOpCodes[value] = opCode;
            }
            else if ((value & 0xff00) == 0xfe00)
            {
                twoByteOpCodes[value & 0xff] = opCode;
            }
        }
    }

    public static bool CallsMethod(Type filterType, string methodName)
    {
        return GetCalledMethods(GeneratedFilterInvoker.GetGeneratedApplyFilterMethod(filterType))
            .Any(method => method.Name == methodName);
    }

    private static IEnumerable<MethodBase> GetCalledMethods(MethodInfo method)
    {
        var body = method.GetMethodBody();
        if (body == null)
        {
            yield break;
        }

        var il = body.GetILAsByteArray();
        var offset = 0;

        while (offset < il.Length)
        {
            var opCode = ReadOpCode(il, ref offset);

            if (opCode.OperandType == OperandType.InlineMethod)
            {
                var token = ReadInt32(il, ref offset);
                MethodBase resolvedMethod;

                try
                {
                    resolvedMethod = method.Module.ResolveMethod(token, method.DeclaringType?.GetGenericArguments(), method.GetGenericArguments());
                }
                catch
                {
                    continue;
                }

                yield return resolvedMethod;
                continue;
            }

            offset += GetOperandSize(opCode.OperandType, il, offset);
        }
    }

    private static OpCode ReadOpCode(byte[] il, ref int offset)
    {
        var value = il[offset++];
        if (value != 0xfe)
        {
            return oneByteOpCodes[value];
        }

        return twoByteOpCodes[il[offset++]];
    }

    private static int ReadInt32(byte[] il, ref int offset)
    {
        var value = BitConverter.ToInt32(il, offset);
        offset += 4;
        return value;
    }

    private static int GetOperandSize(OperandType operandType, byte[] il, int offset)
    {
        return operandType switch
        {
            OperandType.InlineNone => 0,
            OperandType.ShortInlineBrTarget => 1,
            OperandType.ShortInlineI => 1,
            OperandType.ShortInlineVar => 1,
            OperandType.InlineVar => 2,
            OperandType.InlineI => 4,
            OperandType.InlineBrTarget => 4,
            OperandType.InlineField => 4,
            OperandType.InlineI8 => 8,
            OperandType.InlineR => 8,
            OperandType.InlineSig => 4,
            OperandType.InlineString => 4,
            OperandType.InlineTok => 4,
            OperandType.InlineType => 4,
            OperandType.InlineSwitch => 4 + (BitConverter.ToInt32(il, offset) * 4),
            OperandType.ShortInlineR => 4,
            OperandType.InlineMethod => 0,
            _ => throw new ArgumentOutOfRangeException(nameof(operandType), operandType, null)
        };
    }
}
