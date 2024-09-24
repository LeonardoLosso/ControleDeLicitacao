using ControleDeLicitacao.Domain.Iterfaces;
using System.ComponentModel.DataAnnotations;

namespace ControleDeLicitacao.Domain.Entities.Documentos.Baixa;

public class BaixaLicitacao : IDominio
{
    [Key]
    public int ID { get; set; }

    [MaxLength(30)]
    public string Responsavel { get; set; } = string.Empty;

    [Required]
    [MaxLength(10)]
    public string Edital {  get; set; }
    public int Status { get; set; }
    public int Unidade { get; set; }
    public int EmpresaID { get; set; }
    public int OrgaoID { get; set; }
    public DateTime? DataLicitacao { get; set; }
    public DateTime? DataAta { get; set; }
    public DateTime? Vigencia { get; set; }
    public ICollection<ItemDeBaixa> Itens { get; set; }
    public int TotalReajustes { get; set; }
}
