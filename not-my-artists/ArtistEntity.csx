#r "Microsoft.WindowsAzure.Storage"

using Microsoft.WindowsAzure.Storage.Table;

public class ArtistEntity : TableEntity
{
    public string ArtistName { get; set; }
    public string ArtistNameNormalized { get; set; }

    public static string Normalize(string artistName)
    {
        return artistName.ToLower().Trim();
    }
}