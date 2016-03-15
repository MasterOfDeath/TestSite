namespace Photos.PL.WebPages.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Helpers;
    using Entites;
    using Logger;
    
    public static class UsersAjaxPage
    {
        private static readonly IDictionary<string, Func<HttpRequestBase, AjaxResponse>> _Queries
            = new Dictionary<string, Func<HttpRequestBase, AjaxResponse>>();

        static UsersAjaxPage()
        {
            _Queries.Add("clickChangePasswordBtn", ClickChangePasswordBtn);
            _Queries.Add("clickLikeBtn", ClickLikeBtn);
            _Queries.Add("clickNewAlbumSaveBtn", ClickNewAlbumSaveBtn);
            _Queries.Add("clickPromptEditAlbumBtn", ClickPromptEditAlbumBtn);
            _Queries.Add("clickPromptRemoveAlbumBtn", ClickPromptRemoveAlbumBtn);
            _Queries.Add("clickPromptEditPhotoBtn", ClickPromptEditPhotoBtn);
            _Queries.Add("clickPromptRemovePhotoBtn", ClickPromptRemovePhotoBtn);
            _Queries.Add("clickSaveProfileBtn", ClickSaveProfileBtn);
            _Queries.Add("uploadPhoto", UploadPhoto);
        }

        public static IDictionary<string, Func<HttpRequestBase, AjaxResponse>> Queries
        {
            get { return _Queries; }
        }

        private static AjaxResponse ClickChangePasswordBtn(HttpRequestBase request)
        {
            string userIdStr = null;
            string oldPassword = null;
            string newPassword = null;
            var methodName = nameof(ClickChangePasswordBtn);

            try
            {
                userIdStr = request["userid"];
                oldPassword = request["oldpassword"];
                newPassword = request["newpassword"];
            }
            catch (Exception ex)
            {
                return SendError(ex, methodName);
            }

            if (string.IsNullOrWhiteSpace(oldPassword) && string.IsNullOrWhiteSpace(newPassword))
            {
                return SendError(
                    $"Values {nameof(oldPassword)} or {nameof(newPassword)} mustn't be empty",
                    methodName);
            }

            if (string.IsNullOrWhiteSpace(request.Cookies["useridcookie"].Value))
            {
                return SendError(
                    $"Please re-login or allow cookies in your browser",
                    methodName);
            }

            // Security
            if (userIdStr != request.Cookies["useridcookie"].Value)
            {
                return SendError(
                    "You don't have permissions for this operation",
                    $"User {userIdStr} don't have permissions this for operation",
                    methodName);
            }

            int userId = 0;

            try
            {
                userId = Convert.ToInt32(userIdStr);
            }
            catch (Exception ex)
            {
                return SendError(ex, methodName);
            }

            var result = false;

            try
            {
                result = LogicProvider.UserLogic.ChangePassword(userId, oldPassword, newPassword);
            }
            catch (Exception ex)
            {
                return SendError(ex, methodName);
            }

            Logger.Log.Info($"User {userId} has changed his password");

            return new AjaxResponse(null, result);
        }

        private static AjaxResponse ClickLikeBtn(HttpRequestBase request)
        {
            string userIdStr = null;
            string photoIdStr = null;
            var methodName = nameof(ClickLikeBtn);

            try
            {
                userIdStr = request.Cookies["useridcookie"].Value;
                photoIdStr = request["photoid"];
            }
            catch (Exception ex)
            {
                return SendError(ex, methodName);
            }

            int photoId = 0, userId = 0;

            try
            {
                photoId = Convert.ToInt32(photoIdStr);
                userId = Convert.ToInt32(userIdStr);
            }
            catch (Exception ex)
            {
                return SendError(ex, methodName);
            }

            Like like = null;

            try
            {
                like = LogicProvider.LikeLogic.GetLikeByUserIdAndPhotoId(userId, photoId);
            }
            catch (Exception ex)
            {
                return SendError(ex, methodName);
            }

            if (like != null)
            {
                return new AjaxResponse("You allready liked this photo");
            }

            try
            {
                LogicProvider.LikeLogic.AddLike(new Like(photoId, userId, DateTime.Now));
            }
            catch (Exception ex)
            {
                return SendError(ex, methodName);
            }

            var result = -1;

            try
            {
                result = LogicProvider.LikeLogic.GetLikesCount(photoId);
            }
            catch (Exception ex)
            {
                return SendError(ex, methodName);
            }

            Logger.Log.Info($"User: {userIdStr} liked photo: {photoIdStr}");

            return new AjaxResponse(null, result);
        }

        private static AjaxResponse ClickNewAlbumSaveBtn(HttpRequestBase request)
        {
            string userIdStr = null;
            string userIdFromRequestStr = null;
            string name = null;
            var methodName = nameof(ClickNewAlbumSaveBtn);

            try
            {
                userIdStr = request.Cookies["useridcookie"].Value;
                userIdFromRequestStr = request["userid"];
                name = request["name"];
            }
            catch (Exception ex)
            {
                return SendError(ex, methodName);
            }

            if (string.IsNullOrEmpty(name))
            {
                return SendError(
                    $"Invalid request: null value of {nameof(name)}", 
                    methodName);
            }

            int userId, userIdFromRequest;

            try
            {
                userId = Convert.ToInt32(userIdStr);
                userIdFromRequest = Convert.ToInt32(userIdFromRequestStr);
            }
            catch (Exception ex)
            {
                return SendError(ex, methodName);
            }

            // Security. Is UserId owner of PhotoId
            if (userId != userIdFromRequest)
            {
                return SendError(
                    "You don't have permissions for this operation",
                    $"User {userId} don't have permissions this for operation",
                    methodName);
            }

            Album album = new Album(name, DateTime.Now, userId);

            try
            {
                LogicProvider.AlbumLogic.AddAlbum(album);
            }
            catch (Exception ex)
            {
                return SendError(ex, methodName);
            }

            Logger.Log.Info($"User: {userId} has created album: {album.Id}");

            return new AjaxResponse(null, album.Id);
        }

        private static AjaxResponse ClickPromptEditAlbumBtn(HttpRequestBase request)
        {
            string userIdStr = null;
            string albumIdStr = null;
            string name = null;
            var methodName = nameof(ClickPromptEditAlbumBtn);

            try
            {
                userIdStr = request.Cookies["useridcookie"].Value;
                albumIdStr = request["albumid"];
                name = request["name"];
            }
            catch (Exception ex)
            {
                return SendError(ex, methodName);
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                return SendError(
                    $"Values {nameof(name)} mustn't be empty", 
                    methodName);
            }

            if (string.IsNullOrWhiteSpace(userIdStr))
            {
                return SendError(
                    $"Please re-login or allow cookies in your browser",
                    methodName);
            }

            int albumId = 0, userId = 0;

            try
            {
                albumId = Convert.ToInt32(albumIdStr);
                userId = Convert.ToInt32(userIdStr);
            }
            catch (Exception ex)
            {
                return SendError(ex, methodName);
            }

            Album album = null;

            try
            {
                album = LogicProvider.AlbumLogic.GetAlbumById(albumId);
            }
            catch (Exception ex)
            {
                return SendError(ex, methodName);
            }

            if (album == null)
            {
                return SendError($"Album: {albumIdStr} hasn't found", methodName);
            }

            // Security. Is UserId owner of AlbumId
            if (album.UserId != userId)
            {
                return SendError(
                    "You don't have permissions for this operation",
                    $"User {userId} don't have permissions this for operation",
                    methodName);
            }

            album.Name = name;
            var result = false;

            try
            {
                result = LogicProvider.AlbumLogic.InsertAlbum(album);
            }
            catch (Exception ex)
            {
                return SendError(ex, methodName);
            }

            Logger.Log.Info($"User {userId} has edited album {albumId}");

            return new AjaxResponse(null, result);
        }

        private static AjaxResponse ClickPromptEditPhotoBtn(HttpRequestBase request)
        {
            string userIdStr = null;
            string photoIdStr = null;
            string name = null;
            var methodName = nameof(ClickPromptEditAlbumBtn);

            try
            {
                userIdStr = request.Cookies["useridcookie"].Value;
                photoIdStr = request["photoid"];
                name = request["name"];
            }
            catch (Exception ex)
            {
                return SendError(ex, methodName);
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                return SendError($"Values {nameof(name)} mustn't be empty", methodName);
            }

            if (string.IsNullOrWhiteSpace(userIdStr))
            {
                return SendError($"Please re-login or allow cookies in your browser", methodName);
            }

            int photoId = 0, userId = 0;

            try
            {
                photoId = Convert.ToInt32(photoIdStr);
                userId = Convert.ToInt32(userIdStr);
            }
            catch (Exception ex)
            {
                return SendError(ex, methodName);
            }

            Photo photo = null;

            try
            {
                photo = LogicProvider.PhotoLogic.GetPhotoById(photoId);
            }
            catch (Exception ex)
            {
                return SendError(ex, methodName);
            }

            if (photo == null)
            {
                return SendError($"Photo: {photoIdStr} hasn't found", methodName);
            }

            // Security. Is UserId owner of PhotoId
            if (photo.UserId != userId)
            {
                return SendError(
                   "You don't have permissions for this operation",
                   $"User {userId} don't have permissions this for operation",
                   methodName);
            }

            photo.Name = name;
            var result = false;

            try
            {
                result = LogicProvider.PhotoLogic.InsertPhoto(photo);
            }
            catch (Exception ex)
            {
                return SendError(ex, methodName);
            }

            Logger.Log.Info($"User {userId} has edited photo {photoId}");

            return new AjaxResponse(null, result);
        }

        private static AjaxResponse ClickPromptRemoveAlbumBtn(HttpRequestBase request)
        {
            string userIdStr = request.Cookies["useridcookie"].Value;
            string albumIdStr = request["albumid"];
            var methodName = nameof(ClickPromptRemoveAlbumBtn);

            try
            {
                userIdStr = request.Cookies["useridcookie"].Value;
                albumIdStr = request["albumid"];
            }
            catch (Exception ex)
            {
                return SendError(ex, methodName);
            }

            int albumId = 0, userId = 0;

            try
            {
                albumId = Convert.ToInt32(albumIdStr);
                userId = Convert.ToInt32(userIdStr);
            }
            catch (Exception ex)
            {
                return SendError(ex, methodName);
            }

            Album album = null;

            try
            {
                album = LogicProvider.AlbumLogic.GetAlbumById(albumId);
            }
            catch (Exception ex)
            {
                return SendError(ex, methodName);
            }

            if (album == null)
            {
                return SendError($"Album: {albumIdStr} hasn't found", methodName);
            }

            // Security. Is UserId owner of AlbumId
            if (album.UserId != userId)
            {
                return SendError(
                   "You don't have permissions for this operation",
                   $"User {userId} don't have permissions this for operation",
                   methodName);
            }

            var result = false;

            try
            {
                result = LogicProvider.AlbumLogic.RemoveAlbum(albumId);
            }
            catch (Exception ex)
            {
                return SendError(ex, methodName);
            }

            Logger.Log.Info($"User {userIdStr} has removed album: {albumId}");

            return new AjaxResponse(null, result);
        }

        private static AjaxResponse ClickPromptRemovePhotoBtn(HttpRequestBase request)
        {
            string userIdStr = request.Cookies["useridcookie"].Value;
            string photoIdStr = request["photoid"];
            var methodName = nameof(ClickPromptRemovePhotoBtn);

            try
            {
                userIdStr = request.Cookies["useridcookie"].Value;
                photoIdStr = request["photoid"];
            }
            catch (Exception ex)
            {
                return SendError(ex, methodName);
            }

            int photoId = 0, userId = 0;

            try
            {
                photoId = Convert.ToInt32(photoIdStr);
                userId = Convert.ToInt32(userIdStr);
            }
            catch (Exception ex)
            {
                return SendError(ex, methodName);
            }

            Photo photo = null;

            try
            {
                photo = LogicProvider.PhotoLogic.GetPhotoById(photoId);
            }
            catch (Exception ex)
            {
                return SendError(ex, methodName);
            }

            if (photo == null)
            {
                return SendError($"Photo: {photoIdStr} hasn't found", methodName);
            }

            // Security. Is UserId owner of PhotoId
            if (photo.UserId != userId)
            {
                return SendError(
                   "You don't have permissions for this operation",
                   $"User {userId} don't have permissions this for operation",
                   methodName);
            }

            var result = false;

            try
            {
                result = LogicProvider.PhotoLogic.RemovePhoto(photoId);
            }
            catch (Exception ex)
            {
                return SendError(ex, methodName);
            }

            Logger.Log.Info($"User: {userIdStr} has removed photo: {photoIdStr}");

            return new AjaxResponse(null, result);
        }

        private static AjaxResponse ClickSaveProfileBtn(HttpRequestBase request)
        {
            string userIdStr = null;
            string firstName = null;
            string lastName = null;
            var methodName = nameof(ClickSaveProfileBtn);

            try
            {
                userIdStr = request["userid"];
                firstName = request["firstname"];
                lastName = request["lastname"];
            }
            catch (Exception ex)
            {
                return SendError(ex, methodName);
            }

            if (string.IsNullOrWhiteSpace(firstName) && string.IsNullOrWhiteSpace(lastName))
            {
                return SendError(
                    $"Values {nameof(firstName)} or {nameof(lastName)} mustn't be empty",
                    methodName);
            }

            if (string.IsNullOrWhiteSpace(request.Cookies["useridcookie"].Value))
            {
                return SendError(
                    $"Please re-login or allow cookies in your browser",
                    methodName);
            }

            // Security
            if (userIdStr != request.Cookies["useridcookie"].Value)
            {
                return SendError(
                    "You don't have permissions for this operation",
                    $"User {userIdStr} don't have permissions this for operation",
                    methodName);
            }

            int userId = 0;

            try
            {
                userId = Convert.ToInt32(userIdStr);
            }
            catch (Exception ex)
            {
                return SendError(ex, methodName);
            }

            User user = null;

            try
            {
                user = LogicProvider.UserLogic.GetUserById(userId);
            }
            catch (Exception ex)
            {
                return SendError(ex, methodName);
            }

            if (user == null)
            {
                return SendError($"User: {userIdStr} hasn't found", methodName);
            }

            user.FirstName = firstName;
            user.LastName = lastName;

            var result = false;

            try
            {
                result = LogicProvider.UserLogic.InsertUser(user);
            }
            catch (Exception ex)
            {
                return SendError(ex, methodName);
            }

            Logger.Log.Info($"User {userId} has edited his profile");

            return new AjaxResponse(null, result);
        }

        private static AjaxResponse UploadPhoto(HttpRequestBase request)
        {
            string userIdStr = null;
            string name = null;
            string albumIdStr = null;
            string sizeStr = null;
            WebImage image = null;
            var methodName = nameof(UploadPhoto);

            try
            {
                userIdStr = request.Cookies["useridcookie"].Value;
                name = request["name"];
                albumIdStr = request["albumid"];
                sizeStr = request["size"];
                image = WebImage.GetImageFromRequest();
            }
            catch (Exception ex)
            {
                return SendError(ex, methodName);
            }

            if (image == null)
            {
                return SendError("Incorrect format of an image", methodName);
            }

            var imageArray = image.GetBytes();
            var mime = "image/" + image.ImageFormat;

            if (imageArray == null || !imageArray.Any() || string.IsNullOrEmpty(mime))
            {
                return SendError("Incorrect format of the image", methodName);
            }

            int albumId = 0, size = 0, userId;

            try
            {
                albumId = Convert.ToInt32(albumIdStr);
                userId = Convert.ToInt32(userIdStr);
                size = Convert.ToInt32(sizeStr);
            }
            catch (Exception ex)
            {
                return SendError(ex, methodName);
            }

            var result = false;
            var photo = new Photo(name, albumId, size, mime, DateTime.Now, userId);

            try
            {
                result = LogicProvider.PhotoLogic.AddPhoto(photo, imageArray);
            }
            catch (Exception ex)
            {
                return SendError(ex, methodName);
            }

            Logger.Log.Info($"User: {userIdStr} uploaded photo: {photo.Id}");

            return new AjaxResponse(null, result);
        }

        private static AjaxResponse SendError(Exception ex, string sender = null)
        {
            if (sender == null)
            {
                Logger.Log.Error(ex.Message);
            }
            else
            {
                Logger.Log.Error(sender, ex);
            }

            return new AjaxResponse(ex.Message);
        }

        private static AjaxResponse SendError(string message, string logMessage, string sender = null)
        {
            if (sender == null)
            {
                Logger.Log.Error(logMessage);
            }
            else
            {
                Logger.Log.Error(sender, new Exception(logMessage));
            }

            return new AjaxResponse(message);
        }
    }
}