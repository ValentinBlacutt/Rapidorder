namespace Restaurante.Models
{
    
    public class ComboBebida
    {
        public int Id { get; set; }
        public int ComboId { get; set; }
        public Combo Combo { get; set; }
        public int BebidaId { get; set; }
        public Bebida Bebida { get; set; }

       
        public decimal RecargoPequeno { get; set; }
        public decimal RecargoMediano { get; set; }
        public decimal RecargoGrande { get; set; }
    }
}
