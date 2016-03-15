namespace Photos.PL.WebPages.Models
{
    using System;
    using Logger;

    public static class PhotosPage
    {
        public static int GetLikesCount(int photoId)
        {
            int result = -1;
            try
            {
                result = LogicProvider.LikeLogic.GetLikesCount(photoId);
            }
            catch (Exception ex)
            {
                Logger.Log.Error(nameof(LogicProvider.LikeLogic.GetLikesCount), ex);
            }

            return result;
        }
    }
}