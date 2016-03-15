namespace TestSite.BLL.Contract
{
    using System.Collections.Generic;
    using TestSite.Entites;

    public interface IAlbumLogic
    {
        bool AddAlbum(Album album);

        bool InsertAlbum(Album album);

        bool RemoveAlbum(int albumId);

        ICollection<Album> ListAlbumsByUserId(int userId);

        Album GetAlbumById(int albumId);
    }
}
