namespace Fp.Tests;

public class BitUtil_NumberTools
{
    public static (int value, int align, int expected)[] AlignUp_ValidInputs = new[]
    {
        (0, 0, 0), //
        (1, 0, 1), //
        (0, 1, 0), //
        (1, 1, 1), //
        (0, 3, 0), //
        (1, 3, 3), //
        (2, 3, 3), //
        (3, 3, 3), //
    };

    public static (int value, int align, int expected)[] AlignDown_ValidInputs = new[]
    {
        (0, 0, 0), //
        (1, 0, 1), //
        (0, 1, 0), //
        (1, 1, 1), //
        (0, 3, 0), //
        (1, 3, 0), //
        (2, 3, 0), //
        (3, 3, 3), //
    };

    public static (int value, int expected)[] GetBitsForBytes_ValidInputs = new[]
    {
        (0, 0), //
        (1, 8), //
        (2, 16), //
        (3, 24), //
    };

    public static (int value, int expected)[] GetBytesForBits_ValidInputs = new[]
    {
        (0, 0), //
        (1, 1), //
        (8, 1), //
        (9, 2), //
        (16, 2), //
        (17, 3), //
    };

    public static TheoryData<(int value, int align, int expected)> TdAlignUp_ValidInputs = new(AlignUp_ValidInputs);
    public static TheoryData<(int value, int align, int expected)> TdAlignDown_ValidInputs = new(AlignDown_ValidInputs);
    public static TheoryData<(int value, int expected)> TdGetBitsForBytes_ValidInputs = new(GetBitsForBytes_ValidInputs);
    public static TheoryData<(int value, int expected)> TdGetBytesForBits_ValidInputs = new(GetBytesForBits_ValidInputs);

    [Theory]
    [MemberData(nameof(TdAlignUp_ValidInputs))]
    public void AlignUp_ValidInputs_Correct((int value, int align, int expected) input)
    {
        Assert.Equal(input.expected, BitUtil.AlignUp(input.value, input.align));
    }

    [Theory]
    [MemberData(nameof(TdAlignDown_ValidInputs))]
    public void AlignDown_ValidInputs_Correct((int value, int align, int expected) input)
    {
        Assert.Equal(input.expected, BitUtil.AlignDown(input.value, input.align));
    }

    [Theory]
    [MemberData(nameof(TdGetBytesForBits_ValidInputs))]
    public void GetBytesForBits_ValidInputs_Correct((int value, int expected) input)
    {
        Assert.Equal(input.expected, BitUtil.GetBytesForBits(input.value));
    }

    [Theory]
    [MemberData(nameof(TdGetBitsForBytes_ValidInputs))]
    public void GetBitsForBytes_ValidInputs_Correct((int value, int expected) input)
    {
        Assert.Equal(input.expected, BitUtil.GetBitsForBytes(input.value));
    }
}
