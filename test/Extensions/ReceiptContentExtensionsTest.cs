using REBoP.Services;

namespace REBoP.Test.Extensions;

public static class ReceiptContentExtensionsTest
{
    [Theory]
    [TestCase("foo\nbar\nquo", "a", false, 1)]
    [TestCase("foo\nbar\nquo", "o", false, 0)]
    [TestCase("foo\nbar\nquo", "o", true, 2)]
    public static void TestFindsIdx(string lines, string containsPredicate, bool inReverse, int expectedLineIdx)
    {
        var contentMock = new Mock<IReceiptContent>();
        var linesMock = new Mock<IReceiptLines>();

        using var linesEnumerator = ((IEnumerable<string>)lines.Split('\n')).GetEnumerator();

        contentMock
            .SetupGet(c => c.Lines)
            .Returns(linesMock.Object)
            .Verifiable();

        linesMock
            .Setup(l => l.GetEnumerator())
            .Returns(linesEnumerator)
            .Verifiable();

        var idx = contentMock.Object.FindLineIdx((l, _) => l.Contains(containsPredicate), inReverse);

        Assert.That(idx, Is.EqualTo(expectedLineIdx));

        contentMock.VerifyAll();
        linesMock.VerifyAll();
    }

    [Theory]
    [TestCase("foo\nbar\nquo\n", "a", "a", "bar")]
    [TestCase("foo\nbar\nquo\n", "o", "a", "foo\nbar")]
    [TestCase("foo\nbar\nquo\n", "o", "o", "foo\nbar\nquo")]
    public static void TestFindsLines(string inputLines, string startPredicate, string endPredicate, string expectedLines)
    {
        var contentMock = new Mock<IReceiptContent>();
        var lines = new ListReceiptLines(inputLines.Split('\n').ToList());

        contentMock
            .SetupGet(c => c.Lines)
            .Returns(lines)
            .Verifiable();

        var actualLines = contentMock.Object.FindLines((l, _) => l.Contains(startPredicate), (l, _) => l.Contains(endPredicate));
        var actual = string.Join('\n', actualLines);

        Assert.That(actual, Is.EqualTo(expectedLines));

        contentMock.VerifyAll();
    }

    [Test]
    public static void TestReturnsNullIfNotInString()
    {
        var contentMock = new Mock<IReceiptContent>();
        var linesMock = new Mock<IReceiptLines>();

        contentMock
            .SetupGet(c => c.Lines)
            .Returns(linesMock.Object)
            .Verifiable();

        linesMock
            .Setup(l => l.GetEnumerator())
            .Returns(Enumerable.Empty<string>().GetEnumerator)
            .Verifiable();

        var actual = contentMock.Object.FindLineIdx((_, _) => true);

        Assert.That(actual, Is.Null);

        contentMock.VerifyAll();
        linesMock.VerifyAll();
    }
}
