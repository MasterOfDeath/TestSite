namespace Photos.PL.WebPages.Models
{
    public class AjaxResponse
    {
        public AjaxResponse(string error, object data)
        {
            this.Error = error;
            this.Data = data;
        }

        public AjaxResponse(string error)
            : this(error, null)
        {
        }

        public string Error { get; }

        public object Data { get; }
    }
}