namespace Nobeach.Models
{
    public class Pagamento
    {
       public string CopiaECola { get; set; } = string.Empty; 
        public decimal Valor { get; set; }
       public string QrCodeBase64 { get; set; } = string.Empty;
   
    }
}