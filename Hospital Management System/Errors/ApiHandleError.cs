namespace Hospital_Management_System.Errors
{
    public class ApiHandleError
    {
        public int code {  get; set; }
        public string? message { get; set; }
        public ApiHandleError(int code,string? mess=null)
        {
            this.code=code;
            message = mess ?? ApplyMessage(code);
        }

        private string? ApplyMessage(int code)
        {
            return code switch
            {
                400 => "BadRequest Error",
                401 => "Not Authorized",
                   404 => "NotFound Error",
                500 => "Server Error",
                _ => null
            };
        }
    }
}
