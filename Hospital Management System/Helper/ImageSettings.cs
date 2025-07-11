namespace Hospital_Management_System.Helper
{
    public  class ImageSettings
    {
        private readonly IWebHostEnvironment env;
        public ImageSettings(IWebHostEnvironment env)
        {
            this.env = env;
        }
        public  async Task<string> UploadImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return null;

            var folderpath = Path.Combine(env.WebRootPath, "Images");
            Directory.CreateDirectory(folderpath);
            var filename = Guid.NewGuid() + Path.GetExtension(file.FileName);
            var filepath= Path.Combine(folderpath, filename);
            using (var stream=new FileStream(filepath,FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return "/Images/" + filename;

        }
    }
}
