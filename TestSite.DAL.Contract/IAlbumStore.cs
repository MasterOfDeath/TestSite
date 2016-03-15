namespace TestSite.DAL.Contract
{
    using System.Collections.Generic;
    using Entites;

    public interface IAlbumStore
    {
        bool AddAlbum(Album album);

        bool InsertAlbum(Album album);

        bool RemoveAlbum(int albumId);

        ICollection<Album> ListAlbumsByUserId(int userId);

        Album GetAlbumById(int albumId);
    }
}
