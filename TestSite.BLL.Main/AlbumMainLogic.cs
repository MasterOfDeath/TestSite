namespace TestSite.BLL.Main
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Contract;
    using Entites;
    using Exceptions;

    public class AlbumMainLogic : IAlbumLogic
    {
        public bool AddAlbum(Album album)
        {
            var result = false;

            try
            {
                this.IsValidAlbum(album);
                result = Stores.AlbumStore.AddAlbum(album);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        public Album GetAlbumById(int albumId)
        {
            if (albumId <= 0)
            {
                throw new ArgumentException($"{nameof(albumId)} mustn't be negative");
            }

            Album result = null;

            try
            {
                result = Stores.AlbumStore.GetAlbumById(albumId);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        public bool InsertAlbum(Album album)
        {
            var result = false;

            try
            {
                this.IsValidAlbum(album);
                result = Stores.AlbumStore.InsertAlbum(album);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        public ICollection<Album> ListAlbumsByUserId(int userId)
        {
            if (userId < 0)
            {
                throw new ArgumentException($"Value {nameof(userId)} mustn't be negative");
            }

            ICollection<Album> result = null;

            try
            {
                result = Stores.AlbumStore.ListAlbumsByUserId(userId);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        public bool RemoveAlbum(int albumId)
        {
            var result = false;

            if (albumId <= 0)
            {
                throw new ArgumentException($"{nameof(albumId)} mustn't be negative");
            }

            try
            {
                result = Stores.AlbumStore.RemoveAlbum(albumId);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        private bool IsValidAlbum(Album album)
        {
            if (string.IsNullOrWhiteSpace(album.Name) ||
                album.Date == null)
            {
                throw new ArgumentException("Arguments mustn't be null or contain only spaces");
            }

            if (album.UserId <= 0)
            {
                throw new ArgumentException($"{nameof(album.UserId)} mustn't be negative");
            }

            if (album.Date > DateTime.Now)
            {
                throw new ArgumentException("Date mustn't be in the future");
            }

            return true;
        }
    }
}
