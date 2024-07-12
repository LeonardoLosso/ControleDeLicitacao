using ControleDeLicitacao.Domain.Iterfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ControleDeLicitacao.Domain.Entities.Documentos.Baixa;

public class BaixaLicitacao : IDominio
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int ID { get; set; }
    public int Status { get; set; }
    
    [Required]
    [MaxLength(10)]
    public string Edital {  get; set; }
    public int EmpresaID { get; set; }
    public int OrgaoID { get; set; }
    public DateTime? DataLicitacao { get; set; }
    public DateTime? DataAta { get; set; }
    public DateTime? Vigencia { get; set; }

    public ICollection<ItemDeBaixa> Itens { get; set; }
}
