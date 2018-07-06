#r "Microsoft.WindowsAzure.Storage"

using Microsoft.WindowsAzure.Storage.Table;

public class ArtistEntity : TableEntity
{
    private string _artistNameNormalized;

    public string ArtistName { get; set; }
    public string ArtistNameNormalized
    {
        get
        {
            return this._artistNameNormalized;
        }
        set
        {
            this._artistNameNormalized = value.ToLower().Trim();
        }
    }
}