using ControleDeLicitacao.Domain.Iterfaces;
using System.ComponentModel.DataAnnotations;

namespace ControleDeLicitacao.Domain.Entities.Documentos.Ata;

public class AtaLicitacao : IDominio
{
    [Key]
    public int ID {  get; set; }

    [MaxLength(30)]
    public string Responsavel { get; set; } = string.Empty;

    [Required]
    [MaxLength(10)]
    public string Edital { get; set;}
    public int Status { get; set; }
    public int Tipo { get; set; } //redundante
    public int Unidade { get; set; }
    public int EmpresaID { get; set; }
    public int OrgaoID { get; set;}
    public DateTime? DataLicitacao {  get; set; }
    public DateTime? DataAta { get; set; }
    public DateTime? Vigencia { get; set; }
    public double TotalLicitado { get; set; } //redundante
    public ICollection<ItemDeAta> Itens { get; set; }
    public int TotalReajustes { get; set; }
}
