namespace Hospital_Management_System.Errors
{
    public class ApiValidationError: ApiHandleError
    {
        public IEnumerable<string> Errors { get; set; }
        public ApiValidationError() : base(400)
        {
            Errors = new List<string>();
        }
    }
}
