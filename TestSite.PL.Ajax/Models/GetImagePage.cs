namespace Photos.PL.WebPages.Models
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using System.Linq;
    using System.Web;
    using System.Web.Helpers;
    using Entites;
    using Logger;

    public static class GetImagePage
    {
        private static readonly string DefaultAlbumCoverFile = 
            ConfigurationManager.AppSettings["defaultAlbumCoverFile"];

        private static readonly string DefaultAlbumCoverType = 
            ConfigurationManager.AppSettings["defaultAlbumCoverType"];

        private static readonly IDictionary<string, Func<HttpRequestBase, Tuple<byte[], string>>> _Queries
            = new Dictionary<string, Func<HttpRequestBase, Tuple<byte[], string>>>();

        static GetImagePage()
        {
            _Queries.Add("album", GetAlbumCover);
            _Queries.Add("photo", GetPhoto);
            _Queries.Add("photothumb", GetThumbPhoto);
        }

        public static IDictionary<string, Func<HttpRequestBase, Tuple<byte[], string>>> Queries
        {
            get { return _Queries; }
        }

        private static Tuple<byte[], string> GetAlbumCover(HttpRequestBase request)
        {
            byte[] result = null;
            Photo photo = null;
            string albumIdStr = request["albumid"];

            if (string.IsNullOrEmpty(albumIdStr))
            {
                Logger.Log.Error(
                    nameof(GetAlbumCover), 
                    new Exception($"Invalid request: null values of {nameof(albumIdStr)}"));
            }

            int albumId = 0;

            try
            {
                albumId = Convert.ToInt32(albumIdStr);
            }
            catch (Exception ex)
            {
                Logger.Log.Error(nameof(GetAlbumCover), ex);
            }

            try
            {
                photo = LogicProvider.PhotoLogic.GetPhotoForAlbumCover(albumId);
            }
            catch (Exception ex)
            {
                Logger.Log.Error(nameof(GetAlbumCover), ex);
            }

            if (photo == null)
            {
                return GetDefaultAlbumCover();
            }

            try
            {
                var image = new WebImage(LogicProvider.PhotoLogic.GetDataById(photo.Id));
                image.Resize(width: 170, height: 170, preserveAspectRatio: true, preventEnlarge: true);
                result = image.GetBytes();
            }
            catch (Exception ex)
            {
                Logger.Log.Error(nameof(GetAlbumCover), ex);
            }

            return new Tuple<byte[], string>(result, photo.Mime);
        }

        private static Tuple<byte[], string> GetDefaultAlbumCover()
        {
            if (!File.Exists(DefaultAlbumCoverFile))
            {
                Logger.Log.Error(nameof(GetDefaultAlbumCover), new Exception("Default album cover not found"));
                return null;
            }

            var imageArray = File.ReadAllBytes(DefaultAlbumCoverFile);

            return new Tuple<byte[], string>(imageArray, DefaultAlbumCoverType);
        }

        private static Tuple<byte[], string> GetPhoto(HttpRequestBase request)
        {
            var photoIdStr = request["photoid"];
            Photo photo = null;
            byte[] photoData = null;

            if (string.IsNullOrEmpty(photoIdStr))
            {
                Logger.Log.Error(
                    nameof(GetPhoto),
                    new Exception($"Invalid request: null values of {nameof(photoIdStr)}"));
                return null;
            }

            int photoId = 0;

            try
            {
                photoId = Convert.ToInt32(photoIdStr);
            }
            catch (Exception ex)
            {
                Logger.Log.Error(nameof(GetPhoto), ex);
                return null;
            }

            try
            {
                photo = LogicProvider.PhotoLogic.GetPhotoById(photoId);
            }
            catch (Exception ex)
            {
                Logger.Log.Error(nameof(GetPhoto), ex);
                return null;
            }

            try
            {
                photoData = LogicProvider.PhotoLogic.GetDataById(photoId);
            }
            catch (Exception ex)
            {
                Logger.Log.Error(nameof(GetPhoto), ex);
                return null;
            }

            return new Tuple<byte[], string>(photoData, photo.Mime);
        }

        private static Tuple<byte[], string> GetThumbPhoto(HttpRequestBase request)
        {
            var result = GetPhoto(request);

            if (result.Item1 != null)
            {
                WebImage image = new WebImage(result.Item1);
                image.Resize(width: 150, height: 150, preserveAspectRatio: true, preventEnlarge: true);
                result = new Tuple<byte[], string>(image.GetBytes(), result.Item2);
            }

            return result;
        }
    }
}