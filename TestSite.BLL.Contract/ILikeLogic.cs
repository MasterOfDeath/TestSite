namespace TestSite.BLL.Contract
{
    using Entites;

    public interface ILikeLogic
    {
        bool AddLike(Like like);

        Like GetLikeByUserIdAndPhotoId(int userId, int photoId);

        int GetLikesCount(int photoId);
    }
}
