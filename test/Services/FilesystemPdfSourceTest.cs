using REBoP.Services;

namespace REBoP.Test.Services;

public static class FilesystemPdfSourceTest
{
    [Theory]
    [TestCase("Examples/1.pdf")]
    [TestCase("Examples/2.pdf")]
    [TestCase("Examples/6.pdf")]
    [TestCase("Examples/12.pdf")]
    public static async Task TestOpenReadOpensReadableStream(string path)
    {
        var file = new FileInfo(path);
        Assert.That(file.Exists, Is.True, $"Test file '{path}' does not exist!");

        var pdf = new FilesystemPdfSource(file);
        var stream = pdf.OpenRead();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(stream.CanRead, Is.True);
            Assert.That(stream.CanWrite, Is.False);
        }

        await stream.DisposeAsync();
    }

    [Theory]
    [TestCase("foo.pdf")]
    [TestCase("Examples/1.exe")]
    [TestCase("Examples")]
    public static void TestOpenReadThrowsForNonExistentFile(string path)
    {
        var file = new FileInfo(path);
        var pdf = new FilesystemPdfSource(file);

        var actual = Assert.Throws<FileNotFoundException>(() => pdf.OpenRead());

        Assert.That(actual.FileName, Is.EqualTo(file.FullName));
    }
}
