
using System.Text.Json.Serialization;

namespace OnlineShop.Models.Entity
{
    public class ProductImage
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        //[JsonIgnore]
        public byte[] Data { get; set; }
        public int Order { get; set; }
        public long ProductId { get; set; }

        public string GetImageUrl()
        {
            string mimeType = GetMimeType(FileName);
            string base64Image = Convert.ToBase64String(Data);
            return $"data:{mimeType};base64,{base64Image}";
        }

        private string GetMimeType(string fileName)
        {
            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            return extension switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".bmp" => "image/bmp",
                ".tiff" => "image/tiff",
                _ => "application/octet-stream", 
            };
        }
    }
}
