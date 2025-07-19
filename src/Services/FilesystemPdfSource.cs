namespace REBoP.Services;

public class FilesystemPdfSource(FileInfo File) : IPdfSource
{
    public FileStream OpenRead()
    {
        if (!File.Exists)
            throw new FileNotFoundException("Cannot open file for reading", File.FullName);

        return File.OpenRead();
    }
}
