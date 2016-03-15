namespace TestSite.DAL.Contract
{
    using Entites;

    public interface ILikeStore
    {
        bool AddLike(Like like);

        Like GetLikeByUserIdAndPhotoId(int userId, int photoId);

        int GetLikesCount(int photoId);
    }
}
