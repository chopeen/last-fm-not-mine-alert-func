#r "Microsoft.WindowsAzure.Storage"

using Microsoft.WindowsAzure.Storage.Table;

public class ArtistEntity : TableEntity
{
    public string ArtistName { get; set; }
    public string ArtistNameNormalized
    {
        get
        {
            return this.ArtistName.ToLower().Trim();
        }
    }
}