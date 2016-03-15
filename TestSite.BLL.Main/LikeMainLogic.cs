namespace TestSite.BLL.Main
{
    using System;
    using BLL.Contract;
    using Entites;

    public class LikeMainLogic : ILikeLogic
    {
        public bool AddLike(Like like)
        {
            var result = false;

            try
            {
                this.IsValidLike(like);
                result = Stores.LikeStore.AddLike(like);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        public Like GetLikeByUserIdAndPhotoId(int userId, int photoId)
        {
            if (userId <= 0 || photoId <= 0)
            {
                throw new ArgumentException($"{nameof(userId)} or {nameof(photoId)} mustn't be negative");
            }

            Like result = null;

            try
            {
                result = Stores.LikeStore.GetLikeByUserIdAndPhotoId(userId, photoId);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        public int GetLikesCount(int photoId)
        {
            if (photoId <= 0)
            {
                throw new ArgumentException($"{nameof(photoId)} mustn't be negative");
            }

            int result = -1;

            try
            {
                result = Stores.LikeStore.GetLikesCount(photoId);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        private bool IsValidLike(Like like)
        {
            if (like.PhotoId <= 0 || like.UserId <= 0)
            {
                throw new ArgumentException($"{nameof(like.PhotoId)} or {nameof(like.UserId)} mustn't be negative");
            }

            if (like.Date > DateTime.Now)
            {
                throw new ArgumentException("Date mustn't be in the future");
            }

            return true;
        }
    }
}
