namespace Hospital_Management_System.Errors
{
    public class ApiExceptionError : ApiHandleError
    {
        public string? Details { get; set; }
        public ApiExceptionError(int statuscode, string? message = null, string? Details = null) : base(statuscode, message)
        {
            this.Details = Details;
        }
    }
}
