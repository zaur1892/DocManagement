namespace DocManagement.Models
{
    public class DocElement
    {
        public int Id { get; set; }
        public string ColumnName { get; set; }   // Grid-də hansı property göstərilir
        public string DisplayName { get; set; }  // Başlıq
        public int OrderIndex { get; set; }      // Sıralama
        public bool IsVisible { get; set; }      // Göstərilsinmi?
    }
}
