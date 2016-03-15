namespace TestSite.BLL.Contract
{
    using System.Collections.Generic;
    using Entites;

    public interface IPhotoLogic
    {
        bool AddPhoto(Photo photo, byte[] data);

        bool InsertPhoto(Photo photo);

        bool RemovePhoto(int photoId);

        ICollection<Photo> ListPhotosInAlbum(int albumId);

        Photo GetPhotoById(int photoId);

        byte[] GetDataById(int photoId);

        Photo GetPhotoForAlbumCover(int albumId);

        ICollection<Photo> SearchPhotoByName(string searchStr);

        ICollection<Photo> GetTop10ByLike();
    }
}
