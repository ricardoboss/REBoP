using REBoP.Services;

namespace REBoP.Test.Services;

public static class DirectoryPdfProviderTest
{
    [Test]
    public static async Task TestListsExamplesAsync()
    {
        var examplesDir = new DirectoryInfo(Path.Join(".", "Examples"));
        var source = new DirectoryPdfProvider(examplesDir);

        var pdfs = await source.GetPdfsAsync().ToListAsync();

        Assert.That(pdfs, Has.Count.EqualTo(12));
    }
}
