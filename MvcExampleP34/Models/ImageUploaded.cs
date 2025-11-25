using static System.Net.Mime.MediaTypeNames;

namespace MvcExampleP34.Models;

public class ImageUploaded
{
    public int Id { get; set; }
    public string FileName { get; set; } = string.Empty;


    public string? Src
    {
        get
        {
            var dir1 = FileName[0];
            var dir2 = FileName[1];

            return $"/uploads/images/{dir1}/{dir2}/{FileName}";
        }
    }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();

}
