namespace Photos.PL.WebPages.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using BLL.Contract;
    using Entites;
    using Logger;

    public static class LogicProvider
    {
        public static IUserLogic UserLogic { get; } = new BLL.Main.UserMainLogic();

        public static IRoleLogic RoleLogic { get; } = new BLL.Main.RoleMainLogic();

        public static IPhotoLogic PhotoLogic { get; } = new BLL.Main.PhotoMainLogic();

        public static IAlbumLogic AlbumLogic { get; } = new BLL.Main.AlbumMainLogic();

        public static ILikeLogic LikeLogic { get; } = new BLL.Main.LikeMainLogic();

        public static Tuple<int, string> GetUserId(string userName)
        {
            User user = null;
            string error = null;

            if (string.IsNullOrWhiteSpace(userName))
            {
                error = $"The variable {nameof(userName)} mustn't be null or empty";
                Logger.Log.Error(error);
                return new Tuple<int, string>(-1, error);
            }

            try
            {
                user = LogicProvider.UserLogic.GetUserByUserName(userName);
            }
            catch (Exception ex)
            {
                error = ex.Message;
                Logger.Log.Error(nameof(LogicProvider.UserLogic.GetUserByUserName), ex);
            }

            if (user == null)
            {
                error = $"The User {userName} hasn't fount in Data Store";
                Logger.Log.Error(error);
                return new Tuple<int, string>(-1, error);
            }

            return new Tuple<int, string>(user.Id, error);
        }

        public static Tuple<ICollection<Album>, string> GetAlbums(int userId)
        {
            string error = null;
            ICollection<Album> albums = null;

            if (userId < 0)
            {
                error = $"The variable {nameof(userId)} mustn't be negative";
                Logger.Log.Error(error);
                return new Tuple<ICollection<Album>, string>(null, error);
            }

            try
            {
                albums = LogicProvider.AlbumLogic.ListAlbumsByUserId(userId);
            }
            catch (Exception ex)
            {
                error = ex.Message;
                Logger.Log.Error(nameof(LogicProvider.AlbumLogic.ListAlbumsByUserId), ex);
            }

            return new Tuple<ICollection<Album>, string>(albums, error);
        }

        public static Tuple<ICollection<Photo>, string> GetPhotos(int albumId)
        {
            string error = null;
            ICollection<Photo> photos = null;

            if (albumId < 0)
            {
                error = $"The variable {nameof(albumId)} mustn't be negative";
                Logger.Log.Error(error);
                return new Tuple<ICollection<Photo>, string>(null, error);
            }

            try
            {
                photos = PhotoLogic.ListPhotosInAlbum(albumId);
            }
            catch (Exception ex)
            {
                error = ex.Message;
                Logger.Log.Error(nameof(PhotoLogic.ListPhotosInAlbum), ex);
            }

            return new Tuple<ICollection<Photo>, string>(photos, error);
        }

        public static Tuple<User, string> GetUser(string userName)
        {
            string error = null;

            if (string.IsNullOrWhiteSpace(userName))
            {
                error = $"The variable {nameof(userName)} mustn't be empty";
                Logger.Log.Error(error);
                return new Tuple<User, string>(null, error);
            }

            User user = null;

            try
            {
                user = UserLogic.GetUserByUserName(userName);
            }
            catch (Exception ex)
            {
                error = ex.Message;
                Logger.Log.Error(nameof(GetUser), ex);
            }

            return new Tuple<User, string>(user, error);
        }

        public static Tuple<ICollection<Photo>, string> SearchSearchPhotoByName(string searchStr)
        {
            string error = null;
            ICollection<Photo> photos = null;

            if (string.IsNullOrWhiteSpace(searchStr))
            {
                searchStr = null;
            }

            try
            {
                photos = PhotoLogic.SearchPhotoByName(searchStr);
            }
            catch (Exception ex)
            {
                error = ex.Message;
                Logger.Log.Error(nameof(PhotoLogic.SearchPhotoByName), ex);
            }

            return new Tuple<ICollection<Photo>, string>(photos, error);
        }
    }
}