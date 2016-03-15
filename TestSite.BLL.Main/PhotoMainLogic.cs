namespace TestSite.BLL.Main
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Contract;
    using Entites;

    public class PhotoMainLogic : IPhotoLogic
    {
        public bool AddPhoto(Photo photo, byte[] data)
        {
            var result = false;

            if (data == null)
            {
                throw new ArgumentException($"Value {nameof(data)} mustn't be null");
            }

            try
            {
                this.IsValidPhoto(photo);
                result = Stores.PhotoStore.AddPhoto(photo, data);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        public byte[] GetDataById(int photoId)
        {
            if (photoId <= 0)
            {
                throw new ArgumentException($"{nameof(photoId)} mustn't be negative");
            }

            byte[] result = null;

            try
            {
                result = Stores.PhotoStore.GetDataById(photoId);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        public Photo GetPhotoForAlbumCover(int albumId)
        {
            ICollection<Photo> photos = null;

            try
            {
                photos = this.ListPhotosInAlbum(albumId);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            if (photos == null)
            {
                return null;
            }

            return photos.First();
        }

        public Photo GetPhotoById(int photoId)
        {
            if (photoId <= 0)
            {
                throw new ArgumentException($"{nameof(photoId)} mustn't be negative");
            }

            Photo result = null;

            try
            {
                result = Stores.PhotoStore.GetPhotoById(photoId);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        public ICollection<Photo> GetTop10ByLike()
        {
            ICollection<Photo> result = null;

            try
            {
                result = Stores.PhotoStore.GetTop10ByLike();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        public bool InsertPhoto(Photo photo)
        {
            var result = false;

            try
            {
                this.IsValidPhoto(photo);
                result = Stores.PhotoStore.InsertPhoto(photo);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        public ICollection<Photo> ListPhotosInAlbum(int albumId)
        {
            if (albumId < 0)
            {
                throw new ArgumentException($"Value {nameof(albumId)} mustn't be negative");
            }

            ICollection<Photo> result = null;

            try
            {
                result = Stores.PhotoStore.ListPhotosInAlbum(albumId);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        public bool RemovePhoto(int photoId)
        {
            var result = false;

            if (photoId <= 0)
            {
                throw new ArgumentException($"{nameof(photoId)} mustn't be negative");
            }

            try
            {
                result = Stores.PhotoStore.RemovePhoto(photoId);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        public ICollection<Photo> SearchPhotoByName(string searchStr)
        {
            if (string.IsNullOrWhiteSpace(searchStr))
            {
                throw new ArgumentException($"Value {nameof(searchStr)} mustn't be empty");
            }

            ICollection<Photo> result = null;

            try
            {
                result = Stores.PhotoStore.SearchPhotoByName(searchStr);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        private bool IsValidPhoto(Photo photo)
        {
            if (string.IsNullOrWhiteSpace(photo.Name) ||
                string.IsNullOrWhiteSpace(photo.Mime))
            {
                throw new ArgumentException("Arguments mustn't be null or contain only spaces");
            }

            if (photo.AlbumId <= 0 || photo.Size <= 0)
            {
                throw new ArgumentException($"{nameof(photo.AlbumId)} or {nameof(photo.Size)} mustn't be negative");
            }

            if (photo.Date > DateTime.Now)
            {
                throw new ArgumentException("Date mustn't be in the future");
            }

            return true;
        }
    }
}
