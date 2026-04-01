using System;

namespace Fp.Tests;

public class IndexUtility_Tests
{
    private static readonly (Index value, int offset, Index expected)[] s_testCases =
    [
        (0, 0, 0),
        (^0, 0, ^0),
        (0, 1, 1),
        (0, int.MaxValue, int.MaxValue),
        (int.MaxValue, -int.MaxValue, 0),
        (^0, -1, ^1),
        (^0, -int.MaxValue, ^int.MaxValue),
        (^int.MaxValue, int.MaxValue, ^0),
    ];

    private static readonly (Index value, int offset)[] s_errorTestCases =
    [
        (0, -1),
        (int.MaxValue, 1),
        (^0, 1),
        (^int.MaxValue, -1),
    ];

    public static TheoryData<(Index value, int offset, Index expected)> TdTestCases = new(s_testCases);

    public static TheoryData<(Index value, int offset)> TdErrorTestCases = new(s_errorTestCases);

    [Theory]
    [MemberData(nameof(TdTestCases))]
    public void BasicTestCases_Succeed((Index value, int offset, Index expected) testCase)
    {
        Assert.Equal(testCase.expected, IndexUtility.WithOffset(testCase.value, testCase.offset));
    }

    [Theory]
    [MemberData(nameof(TdErrorTestCases))]
    public void ErrorTestCases_Fail((Index value, int offset) testCase)
    {
        Assert.ThrowsAny<ArgumentException>(() => IndexUtility.WithOffset(testCase.value, testCase.offset));
    }
}
